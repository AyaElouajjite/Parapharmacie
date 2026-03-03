using Microsoft.AspNetCore.Mvc;
using Parapharmacie.Data;
using Parapharmacie.Models;

namespace Parapharmacie.Controllers
{
    public class CatalogController : Controller
    {
        public IActionResult Index(string? search = null, int? categoryId = null)
        {
            var products = InMemoryData.Products.AsEnumerable();

            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p => 
                    p.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    (p.Description != null && p.Description.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    p.Reference.Contains(search, StringComparison.OrdinalIgnoreCase)
                );
            }

            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId);
            }

            ViewBag.Categories = InMemoryData.Categories;
            ViewBag.SearchTerm = search;
            ViewBag.CategoryId = categoryId;

            return View(products.ToList());
        }

        public IActionResult Details(int id)
        {
            var product = InMemoryData.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
    }
}

