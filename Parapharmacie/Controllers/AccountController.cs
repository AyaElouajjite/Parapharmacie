using Microsoft.AspNetCore.Mvc;
using Parapharmacie.Data;
using Parapharmacie.Models;

namespace Parapharmacie.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string username, string password)
        {
            var user = InMemoryData.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user == null)
            {
                ViewBag.Error = "Identifiants invalides";
                return View();
            }

            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Role", user.Role.ToString());
            if (user.CustomerId.HasValue)
            {
                HttpContext.Session.SetInt32("CustomerId", user.CustomerId.Value);
            }
            if (user.SupplierId.HasValue)
            {
                HttpContext.Session.SetInt32("SupplierId", user.SupplierId.Value);
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(string fullName, string email, string phone, string password)
        {
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Nom, email et mot de passe sont requis.";
                return View();
            }

            if (InMemoryData.Users.Any(u => u.Username.Equals(email, StringComparison.OrdinalIgnoreCase)))
            {
                ViewBag.Error = "Un compte existe déjà avec cet email.";
                return View();
            }

            var customer = InMemoryData.AddCustomer(new Models.Customer
            {
                FullName = fullName,
                Email = email,
                Phone = phone,
                LoyaltyPoints = 0
            });

            var user = InMemoryData.AddUser(new Models.User
            {
                Username = email,
                Password = password,
                Role = Models.Role.Client,
                CustomerId = customer.Id
            });

            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Role", user.Role.ToString());
            HttpContext.Session.SetInt32("CustomerId", customer.Id);

            return RedirectToAction("Dashboard", "ClientPortal");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}

