using Microsoft.AspNetCore.Mvc;
using Parapharmacie.Data;
using Parapharmacie.Filters;
using Parapharmacie.Models;

namespace Parapharmacie.Controllers
{
    [RoleAuthorize(Role.Admin)]
    public class CategoriesController : Controller
    {
        public IActionResult Index()
        {
            var categories = InMemoryData.Categories.OrderBy(c => c.Name).ToList();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View(new Category());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (!ModelState.IsValid) return View(category);
            InMemoryData.AddCategory(category);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var category = InMemoryData.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Category category)
        {
            var existing = InMemoryData.Categories.FirstOrDefault(c => c.Id == id);
            if (existing == null) return NotFound();
            if (!ModelState.IsValid) return View(category);
            existing.Name = category.Name;
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var category = InMemoryData.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var category = InMemoryData.Categories.FirstOrDefault(c => c.Id == id);
            if (category != null)
            {
                InMemoryData.Categories.Remove(category);
                // detach category from products
                foreach (var p in InMemoryData.Products.Where(p => p.CategoryId == id))
                {
                    p.CategoryId = null;
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}




