using Microsoft.AspNetCore.Mvc;
using Parapharmacie.Data;
using Parapharmacie.Models;

namespace Parapharmacie.Controllers
{
    using Parapharmacie.Filters;

    [RoleAuthorize(Role.Admin, Role.Gerant)]
    public class ProductsController : Controller
    {
        public IActionResult Index()
        {
            var products = InMemoryData.Products.OrderBy(p => p.Name).ToList();
            return View(products);
        }

        public IActionResult Details(int id)
        {
            var product = InMemoryData.Products.FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }

        public IActionResult Create()
        {
            ViewBag.Categories = InMemoryData.Categories;
            return View(new Product());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product)
        {
            ViewBag.Categories = InMemoryData.Categories;
            if (!ModelState.IsValid) return View(product);

            InMemoryData.AddProduct(product);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var product = InMemoryData.Products.FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();
            ViewBag.Categories = InMemoryData.Categories;
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Product product)
        {
            var existing = InMemoryData.Products.FirstOrDefault(p => p.Id == id);
            if (existing == null) return NotFound();
            ViewBag.Categories = InMemoryData.Categories;
            if (!ModelState.IsValid) return View(product);

            existing.Name = product.Name;
            existing.Reference = product.Reference;
            existing.Price = product.Price;
            existing.Stock = product.Stock;
            existing.ReorderLevel = product.ReorderLevel;
            existing.CategoryId = product.CategoryId;
            existing.Description = product.Description;
            existing.ImageUrl = product.ImageUrl;
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var product = InMemoryData.Products.FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var product = InMemoryData.Products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                InMemoryData.Products.Remove(product);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

