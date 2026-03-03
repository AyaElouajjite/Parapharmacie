using Microsoft.AspNetCore.Mvc;
using Parapharmacie.Data;
using Parapharmacie.Filters;
using Parapharmacie.Models;
using Parapharmacie.ViewModels;

namespace Parapharmacie.Controllers
{
    [RoleAuthorize(Role.Client)]
    public class ClientPortalController : Controller
    {
        private int? CurrentCustomerId => HttpContext.Session.GetInt32("CustomerId");

        public IActionResult Dashboard()
        {
            var customerId = CurrentCustomerId;
            if (customerId == null) return RedirectToAction("Login", "Account");

            var customer = InMemoryData.Customers.FirstOrDefault(c => c.Id == customerId);
            var purchases = InMemoryData.Sales.Where(s => s.CustomerId == customerId).OrderByDescending(s => s.Date).ToList();
            foreach (var sale in purchases)
            {
                foreach (var item in sale.Items)
                {
                    item.Product = InMemoryData.Products.FirstOrDefault(p => p.Id == item.ProductId);
                }
            }

            ViewBag.Customer = customer;
            ViewBag.Purchases = purchases;
            return View();
        }

        public IActionResult Commande()
        {
            var vm = new SaleCreateViewModel
            {
                CustomerId = CurrentCustomerId,
                Items = new List<SaleItemInput> { new SaleItemInput { Quantity = 1 } }
            };
            ViewBag.Products = InMemoryData.Products;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Commande(SaleCreateViewModel vm)
        {
            vm.CustomerId = CurrentCustomerId;
            ViewBag.Products = InMemoryData.Products;

            if (vm.CustomerId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (vm.Items == null || !vm.Items.Any())
            {
                ModelState.AddModelError("", "Ajoutez au moins un produit.");
            }

            foreach (var line in vm.Items ?? new List<SaleItemInput>())
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
            foreach (var line in vm.Items!)
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
            var customer = InMemoryData.Customers.FirstOrDefault(c => c.Id == sale.CustomerId);
            if (customer != null)
            {
                customer.LoyaltyPoints += (int)Math.Floor(sale.Total / 10);
                sale.Customer = customer;
            }

            InMemoryData.AddSale(sale);
            return RedirectToAction(nameof(Dashboard));
        }
    }
}




