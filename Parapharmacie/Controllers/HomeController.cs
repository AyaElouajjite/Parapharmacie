using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Parapharmacie.Models;

namespace Parapharmacie.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("Role");
            
            // Rediriger vers le dashboard approprié selon le rôle
            if (!string.IsNullOrEmpty(role))
            {
                if (role == "Admin")
                    return RedirectToAction("AdminDashboard");
                if (role == "Gerant")
                    return RedirectToAction("GerantDashboard");
                if (role == "Caissier")
                    return RedirectToAction("CaissierDashboard");
                if (role == "Supplier")
                    return RedirectToAction("SupplierDashboard");
            }
            
            // Pour les clients ou non connectés, afficher la page d'accueil publique
            ViewBag.Products = Parapharmacie.Data.InMemoryData.Products;
            ViewBag.Sales = Parapharmacie.Data.InMemoryData.Sales;
            ViewBag.Customers = Parapharmacie.Data.InMemoryData.Customers;
            ViewBag.SupplierOrders = Parapharmacie.Data.InMemoryData.SupplierOrders;
            return View();
        }

        [Parapharmacie.Filters.RoleAuthorize(Models.Role.Admin)]
        public IActionResult AdminDashboard()
        {
            ViewBag.Products = Parapharmacie.Data.InMemoryData.Products;
            ViewBag.Sales = Parapharmacie.Data.InMemoryData.Sales;
            ViewBag.Customers = Parapharmacie.Data.InMemoryData.Customers;
            ViewBag.Users = Parapharmacie.Data.InMemoryData.Users;
            ViewBag.Categories = Parapharmacie.Data.InMemoryData.Categories;
            ViewBag.SupplierOrders = Parapharmacie.Data.InMemoryData.SupplierOrders;
            return View();
        }

        [Parapharmacie.Filters.RoleAuthorize(Models.Role.Gerant)]
        public IActionResult GerantDashboard()
        {
            ViewBag.Products = Parapharmacie.Data.InMemoryData.Products;
            ViewBag.Sales = Parapharmacie.Data.InMemoryData.Sales;
            ViewBag.Customers = Parapharmacie.Data.InMemoryData.Customers;
            ViewBag.SupplierOrders = Parapharmacie.Data.InMemoryData.SupplierOrders;
            ViewBag.Suppliers = Parapharmacie.Data.InMemoryData.Suppliers;
            return View();
        }

        [Parapharmacie.Filters.RoleAuthorize(Models.Role.Caissier)]
        public IActionResult CaissierDashboard()
        {
            ViewBag.Products = Parapharmacie.Data.InMemoryData.Products;
            ViewBag.Sales = Parapharmacie.Data.InMemoryData.Sales;
            ViewBag.Customers = Parapharmacie.Data.InMemoryData.Customers;
            return View();
        }

        [Parapharmacie.Filters.RoleAuthorize(Models.Role.Supplier)]
        public IActionResult SupplierDashboard()
        {
            var supplierId = HttpContext.Session.GetInt32("SupplierId");
            var orders = Parapharmacie.Data.InMemoryData.SupplierOrders
                .Where(o => o.SupplierId == supplierId)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
            ViewBag.Orders = orders;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
