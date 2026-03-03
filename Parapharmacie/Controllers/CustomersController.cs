using Microsoft.AspNetCore.Mvc;
using Parapharmacie.Data;
using Parapharmacie.Models;

namespace Parapharmacie.Controllers
{
    using Parapharmacie.Filters;

    [RoleAuthorize(Role.Admin, Role.Caissier, Role.Gerant)]
    public class CustomersController : Controller
    {
        public IActionResult Index()
        {
            var customers = InMemoryData.Customers.OrderBy(c => c.FullName).ToList();
            return View(customers);
        }

        public IActionResult Details(int id)
        {
            var customer = InMemoryData.Customers.FirstOrDefault(c => c.Id == id);
            if (customer == null) return NotFound();
            var purchases = InMemoryData.Sales.Where(s => s.CustomerId == id).OrderByDescending(s => s.Date).ToList();
            ViewBag.Purchases = purchases;
            return View(customer);
        }

        public IActionResult Create()
        {
            return View(new Customer());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Customer customer)
        {
            if (!ModelState.IsValid) return View(customer);
            InMemoryData.AddCustomer(customer);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var customer = InMemoryData.Customers.FirstOrDefault(c => c.Id == id);
            if (customer == null) return NotFound();
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Customer customer)
        {
            var existing = InMemoryData.Customers.FirstOrDefault(c => c.Id == id);
            if (existing == null) return NotFound();
            if (!ModelState.IsValid) return View(customer);

            existing.FullName = customer.FullName;
            existing.Email = customer.Email;
            existing.Phone = customer.Phone;
            existing.LoyaltyPoints = customer.LoyaltyPoints;
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var customer = InMemoryData.Customers.FirstOrDefault(c => c.Id == id);
            if (customer == null) return NotFound();
            return View(customer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var customer = InMemoryData.Customers.FirstOrDefault(c => c.Id == id);
            if (customer != null)
            {
                InMemoryData.Customers.Remove(customer);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

