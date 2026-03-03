using Microsoft.AspNetCore.Mvc;
using Parapharmacie.Data;
using Parapharmacie.Models;
using System.Text.Json;

namespace Parapharmacie.Controllers
{
    public class CartController : Controller
    {
        private List<CartItem> GetCart()
        {
            var cartJson = HttpContext.Session.GetString("Cart");
            if (string.IsNullOrEmpty(cartJson))
            {
                return new List<CartItem>();
            }
            return JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
        }

        private void SaveCart(List<CartItem> cart)
        {
            var cartJson = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString("Cart", cartJson);
        }

        public IActionResult Index()
        {
            var cart = GetCart();
            foreach (var item in cart)
            {
                item.Product = InMemoryData.Products.FirstOrDefault(p => p.Id == item.ProductId);
            }
            return View(cart);
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity = 1)
        {
            var product = InMemoryData.Products.FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                return NotFound();
            }

            if (quantity <= 0 || quantity > product.Stock)
            {
                TempData["Error"] = "Quantité invalide ou stock insuffisant.";
                return RedirectToAction("Index", "Catalog");
            }

            var cart = GetCart();
            var existingItem = cart.FirstOrDefault(c => c.ProductId == productId);
            
            if (existingItem != null)
            {
                var newQuantity = existingItem.Quantity + quantity;
                if (newQuantity > product.Stock)
                {
                    TempData["Error"] = "Stock insuffisant pour cette quantité.";
                    return RedirectToAction("Index", "Catalog");
                }
                existingItem.Quantity = newQuantity;
            }
            else
            {
                cart.Add(new CartItem { ProductId = productId, Quantity = quantity });
            }

            SaveCart(cart);
            TempData["Success"] = "Produit ajouté au panier.";
            return RedirectToAction("Index", "Catalog");
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            
            if (item == null)
            {
                return NotFound();
            }

            var product = InMemoryData.Products.FirstOrDefault(p => p.Id == productId);
            if (product == null || quantity <= 0 || quantity > product.Stock)
            {
                TempData["Error"] = "Quantité invalide ou stock insuffisant.";
                return RedirectToAction("Index");
            }

            item.Quantity = quantity;
            SaveCart(cart);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item != null)
            {
                cart.Remove(item);
                SaveCart(cart);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove("Cart");
            return RedirectToAction("Index");
        }

        public IActionResult Checkout()
        {
            var cart = GetCart();
            if (!cart.Any())
            {
                TempData["Error"] = "Votre panier est vide.";
                return RedirectToAction("Index");
            }

            foreach (var item in cart)
            {
                item.Product = InMemoryData.Products.FirstOrDefault(p => p.Id == item.ProductId);
                if (item.Product == null || item.Quantity > item.Product.Stock)
                {
                    TempData["Error"] = "Certains produits ne sont plus disponibles en stock.";
                    return RedirectToAction("Index");
                }
            }

            var customerId = HttpContext.Session.GetInt32("CustomerId");
            var customer = customerId.HasValue 
                ? InMemoryData.Customers.FirstOrDefault(c => c.Id == customerId.Value)
                : null;

            ViewBag.Customer = customer;
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ProcessPayment(string cardNumber, string cardHolder, string expiryMonth, string expiryYear, string cvv, string billingAddress)
        {
            var cart = GetCart();
            if (!cart.Any())
            {
                TempData["Error"] = "Votre panier est vide.";
                return RedirectToAction("Index");
            }

            // Validation simple du formulaire
            if (string.IsNullOrWhiteSpace(cardNumber) || 
                string.IsNullOrWhiteSpace(cardHolder) || 
                string.IsNullOrWhiteSpace(expiryMonth) || 
                string.IsNullOrWhiteSpace(expiryYear) || 
                string.IsNullOrWhiteSpace(cvv) ||
                string.IsNullOrWhiteSpace(billingAddress))
            {
                TempData["Error"] = "Veuillez remplir tous les champs du formulaire de paiement.";
                return RedirectToAction("Checkout");
            }

            // Validation de la carte (format simple)
            var cleanCardNumber = cardNumber.Replace(" ", "").Replace("-", "");
            if (cleanCardNumber.Length < 13 || cleanCardNumber.Length > 19 || !cleanCardNumber.All(char.IsDigit))
            {
                TempData["Error"] = "Numéro de carte invalide.";
                return RedirectToAction("Checkout");
            }

            if (cvv.Length < 3 || cvv.Length > 4 || !cvv.All(char.IsDigit))
            {
                TempData["Error"] = "Code CVV invalide.";
                return RedirectToAction("Checkout");
            }

            // Vérifier le stock avant de finaliser
            foreach (var item in cart)
            {
                item.Product = InMemoryData.Products.FirstOrDefault(p => p.Id == item.ProductId);
                if (item.Product == null || item.Quantity > item.Product.Stock)
                {
                    TempData["Error"] = $"Le produit {item.Product?.Name ?? "inconnu"} n'est plus disponible en stock suffisant.";
                    return RedirectToAction("Index");
                }
            }

            // Créer la vente
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            var sale = new Sale
            {
                CustomerId = customerId,
                Date = DateTime.UtcNow
            };

            decimal total = 0;
            foreach (var item in cart)
            {
                var product = item.Product!;
                var saleItem = new SaleItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price,
                    DiscountPercent = 0
                };
                total += saleItem.LineTotal;
                sale.Items.Add(saleItem);

                // Mettre à jour le stock
                product.Stock -= item.Quantity;
            }

            sale.Total = Math.Round(total * 1.20m, 2); // TTC avec TVA 20%

            // Mettre à jour les points de fidélité
            if (customerId.HasValue)
            {
                var customer = InMemoryData.Customers.FirstOrDefault(c => c.Id == customerId.Value);
                if (customer != null)
                {
                    var earnedPoints = (int)Math.Floor(total / 10); // 1 point tous les 10€
                    customer.LoyaltyPoints += earnedPoints;
                    sale.Customer = customer;
                }
            }

            var savedSale = InMemoryData.AddSale(sale);

            // Vider le panier
            HttpContext.Session.Remove("Cart");

            // Stocker les informations de paiement pour le reçu
            HttpContext.Session.SetString("LastPaymentCard", $"{cardNumber.Substring(0, 4)}****{cardNumber.Substring(cardNumber.Length - 4)}");
            HttpContext.Session.SetString("LastPaymentHolder", cardHolder);

            return RedirectToAction("Receipt", new { saleId = savedSale.Id });
        }

        public IActionResult Receipt(int saleId)
        {
            var sale = InMemoryData.Sales.FirstOrDefault(s => s.Id == saleId);
            if (sale == null)
            {
                return NotFound();
            }

            sale.Customer = sale.CustomerId.HasValue
                ? InMemoryData.Customers.FirstOrDefault(c => c.Id == sale.CustomerId)
                : null;

            foreach (var item in sale.Items)
            {
                item.Product = InMemoryData.Products.FirstOrDefault(p => p.Id == item.ProductId);
            }

            ViewBag.CardNumber = HttpContext.Session.GetString("LastPaymentCard");
            ViewBag.CardHolder = HttpContext.Session.GetString("LastPaymentHolder");

            return View(sale);
        }
    }
}

