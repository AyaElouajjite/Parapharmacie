using Microsoft.AspNetCore.Mvc;
using Parapharmacie.Data;
using Parapharmacie.Models;
using Parapharmacie.ViewModels;

namespace Parapharmacie.Controllers
{
    using Parapharmacie.Filters;

    [RoleAuthorize(Role.Gerant, Role.Supplier)]
    public class SupplierOrdersController : Controller
    {
        public IActionResult Index()
        {
            var orders = InMemoryData.SupplierOrders
                .OrderByDescending(o => o.OrderDate)
                .ToList();
            return View(orders);
        }

        public IActionResult Details(int id)
        {
            var order = InMemoryData.SupplierOrders.FirstOrDefault(o => o.Id == id);
            if (order == null) return NotFound();

            order.Supplier = InMemoryData.Suppliers.FirstOrDefault(s => s.Id == order.SupplierId);
            foreach (var item in order.Items)
            {
                item.Product = InMemoryData.Products.FirstOrDefault(p => p.Id == item.ProductId);
            }

            return View(order);
        }

        [RoleAuthorize(Role.Gerant)]
        public IActionResult Create()
        {
            var vm = new SupplierOrderCreateViewModel
            {
                Items = new List<SupplierOrderItemInput> { new SupplierOrderItemInput { Quantity = 10 } }
            };
            ViewBag.Suppliers = InMemoryData.Suppliers;
            ViewBag.Products = InMemoryData.Products;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize(Role.Gerant)]
        public IActionResult Create(SupplierOrderCreateViewModel vm)
        {
            ViewBag.Suppliers = InMemoryData.Suppliers;
            ViewBag.Products = InMemoryData.Products;

            if (!ModelState.IsValid) return View(vm);

            var order = new SupplierOrder
            {
                SupplierId = vm.SupplierId,
                OrderDate = DateTime.UtcNow,
                Status = "En attente"
            };

            foreach (var item in vm.Items)
            {
                var product = InMemoryData.Products.FirstOrDefault(p => p.Id == item.ProductId);
                if (product == null)
                {
                    ModelState.AddModelError("", "Produit introuvable.");
                    continue;
                }

                order.Items.Add(new SupplierOrderItem
                {
                    ProductId = product.Id,
                    QuantityOrdered = item.Quantity,
                    QuantityReceived = 0
                });
            }

            if (!ModelState.IsValid) return View(vm);

            InMemoryData.AddSupplierOrder(order);
            return RedirectToAction(nameof(Details), new { id = order.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize(Role.Gerant)]
        public IActionResult Receive(int id)
        {

            var order = InMemoryData.SupplierOrders.FirstOrDefault(o => o.Id == id);
            if (order == null) return NotFound();
            if (order.Status == "Réceptionnée") return RedirectToAction(nameof(Details), new { id });

            foreach (var item in order.Items)
            {
                var product = InMemoryData.Products.FirstOrDefault(p => p.Id == item.ProductId);
                if (product != null)
                {
                    product.Stock += item.QuantityOrdered;
                    item.QuantityReceived = item.QuantityOrdered;
                }
            }

            order.Status = "Réceptionnée";
            order.ReceivedDate = DateTime.UtcNow;
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}

