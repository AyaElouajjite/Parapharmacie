using Microsoft.AspNetCore.Mvc;
using Parapharmacie.Data;
using Parapharmacie.Models;
using Parapharmacie.ViewModels;

namespace Parapharmacie.Controllers
{
    using Parapharmacie.Filters;

    [RoleAuthorize(Role.Admin, Role.Caissier, Role.Gerant)]
    public class SalesController : Controller
    {
        public IActionResult Index()
        {
            var sales = InMemoryData.Sales
                .OrderByDescending(s => s.Date)
                .ToList();
            return View(sales);
        }

        public IActionResult Details(int id)
        {
            var sale = InMemoryData.Sales.FirstOrDefault(s => s.Id == id);
            if (sale == null) return NotFound();

            sale.Customer = sale.CustomerId.HasValue
                ? InMemoryData.Customers.FirstOrDefault(c => c.Id == sale.CustomerId)
                : null;

            foreach (var item in sale.Items)
            {
                item.Product = InMemoryData.Products.FirstOrDefault(p => p.Id == item.ProductId);
            }

            return View(sale);
        }

        public IActionResult Create()
        {
            var vm = new SaleCreateViewModel
            {
                Items = new List<SaleItemInput> { new SaleItemInput { Quantity = 1 } }
            };
            ViewBag.Customers = InMemoryData.Customers;
            ViewBag.Products = InMemoryData.Products;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SaleCreateViewModel vm)
        {
            ViewBag.Customers = InMemoryData.Customers;
            ViewBag.Products = InMemoryData.Products;

            if (vm.Items == null || vm.Items.Count == 0)
            {
                ModelState.AddModelError("", "Ajoutez au moins un produit.");
            }

            vm.Items ??= new List<SaleItemInput>();

            foreach (var line in vm.Items)
            {
                var product = InMemoryData.Products.FirstOrDefault(p => p.Id == line.ProductId);
                if (product == null)
                {
                    ModelState.AddModelError("", "Produit introuvable.");
                    continue;
                }

                if (line.Quantity > product.Stock)
                {
                    ModelState.AddModelError("", $"Stock insuffisant pour {product.Name}.");
                }
            }

            if (!ModelState.IsValid) return View(vm);

            var sale = new Sale
            {
                CustomerId = vm.CustomerId,
                Date = DateTime.UtcNow
            };

            decimal total = 0;
            foreach (var line in vm.Items)
            {
                var product = InMemoryData.Products.First(p => p.Id == line.ProductId);
                var item = new SaleItem
                {
                    ProductId = product.Id,
                    Quantity = line.Quantity,
                    UnitPrice = product.Price,
                    DiscountPercent = line.DiscountPercent
                };
                total += item.LineTotal;

                product.Stock -= line.Quantity;
                sale.Items.Add(item);
            }

            sale.Total = Math.Round(total, 2);

            if (sale.CustomerId.HasValue)
            {
                var customer = InMemoryData.Customers.FirstOrDefault(c => c.Id == sale.CustomerId.Value);
                if (customer != null)
                {
                    var earnedPoints = (int)Math.Floor(sale.Total / 10); // 1 point every 10€
                    customer.LoyaltyPoints += earnedPoints;
                    sale.Customer = customer;
                }
            }

            InMemoryData.AddSale(sale);
            return RedirectToAction(nameof(Details), new { id = sale.Id });
        }
    }
}

