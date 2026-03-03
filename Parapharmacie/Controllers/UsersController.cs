using Microsoft.AspNetCore.Mvc;
using Parapharmacie.Data;
using Parapharmacie.Filters;
using Parapharmacie.Models;

namespace Parapharmacie.Controllers
{
    [RoleAuthorize(Role.Admin)]
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            var users = InMemoryData.Users.OrderBy(u => u.Username).ToList();
            return View(users);
        }

        public IActionResult Create()
        {
            return View(new User());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(User user)
        {
            if (!ModelState.IsValid) return View(user);
            InMemoryData.AddUser(user);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var user = InMemoryData.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, User user)
        {
            var existing = InMemoryData.Users.FirstOrDefault(u => u.Id == id);
            if (existing == null) return NotFound();
            if (!ModelState.IsValid) return View(user);

            existing.Username = user.Username;
            existing.Password = user.Password;
            existing.Role = user.Role;
            existing.CustomerId = user.CustomerId;
            existing.SupplierId = user.SupplierId;
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var user = InMemoryData.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var user = InMemoryData.Users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                InMemoryData.Users.Remove(user);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}




