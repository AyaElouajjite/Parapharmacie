using Parapharmacie.Models;

namespace Parapharmacie.Data
{
    /// <summary>
    /// Very small in-memory data store used to keep the sample self contained.
    /// </summary>
    public static class InMemoryData
    {
        private static int _productId = 1;
        private static int _customerId = 1;
        private static int _saleId = 1;
        private static int _supplierId = 1;
        private static int _supplierOrderId = 1;
        private static int _categoryId = 1;
        private static int _userId = 1;

        public static List<Product> Products { get; } = new();
        public static List<Customer> Customers { get; } = new();
        public static List<Sale> Sales { get; } = new();
        public static List<Supplier> Suppliers { get; } = new();
        public static List<SupplierOrder> SupplierOrders { get; } = new();
        public static List<Category> Categories { get; } = new();
        public static List<User> Users { get; } = new();

        public static void Seed()
        {
            if (Products.Any())
            {
                return;
            }

            var hygiene = AddCategory(new Category { Name = "Hygiène" });
            var complement = AddCategory(new Category { Name = "Compléments" });
            var beaute = AddCategory(new Category { Name = "Beauté" });

            AddProduct(new Product 
            { 
                Name = "Gel hydroalcoolique", 
                Reference = "GEL001", 
                Price = 4.5m, 
                Stock = 120, 
                ReorderLevel = 30, 
                CategoryId = hygiene.Id,
                Description = "Gel hydroalcoolique désinfectant pour les mains. Tue 99,9% des bactéries et virus. Formule douce et non desséchante. Format pratique 100ml.",
                ImageUrl = "https://images.unsplash.com/photo-1584308666744-24d5c474f2ae?w=400"
            });
            AddProduct(new Product 
            { 
                Name = "Vitamine C 1000", 
                Reference = "VITC", 
                Price = 12m, 
                Stock = 80, 
                ReorderLevel = 20, 
                CategoryId = complement.Id,
                Description = "Complément alimentaire à base de vitamine C. Contribue au fonctionnement normal du système immunitaire et à réduire la fatigue. Comprimés à libération prolongée.",
                ImageUrl = "https://images.unsplash.com/photo-1550572017-edd951b55104?w=400"
            });
            AddProduct(new Product 
            { 
                Name = "Crème solaire SPF50", 
                Reference = "SUN50", 
                Price = 18m, 
                Stock = 45, 
                ReorderLevel = 10, 
                CategoryId = beaute.Id,
                Description = "Crème solaire haute protection SPF50+. Protection UVA et UVB. Résistante à l'eau. Texture légère et non grasse. Idéale pour toute la famille.",
                ImageUrl = "https://images.unsplash.com/photo-1556228578-0d85b1a4d571?w=400"
            });
            AddProduct(new Product 
            { 
                Name = "Shampooing doux bébé", 
                Reference = "SHB001", 
                Price = 6.5m, 
                Stock = 60, 
                ReorderLevel = 15, 
                CategoryId = hygiene.Id,
                Description = "Shampooing doux spécialement formulé pour les bébés. pH neutre, sans paraben, sans sulfate. Nettoie en douceur les cheveux et le cuir chevelu délicat.",
                ImageUrl = "https://images.unsplash.com/photo-1607613009820-a29f7bb81c04?w=400"
            });
            AddProduct(new Product 
            { 
                Name = "Magnésium + Vitamine B6", 
                Reference = "MAGB6", 
                Price = 15.5m, 
                Stock = 50, 
                ReorderLevel = 12, 
                CategoryId = complement.Id,
                Description = "Complément alimentaire associant magnésium et vitamine B6. Contribue à réduire la fatigue et au fonctionnement normal du système nerveux. Boîte de 60 comprimés.",
                ImageUrl = "https://images.unsplash.com/photo-1550572017-edd951b55104?w=400"
            });
            AddProduct(new Product 
            { 
                Name = "Sérum visage anti-âge", 
                Reference = "SER001", 
                Price = 24.9m, 
                Stock = 35, 
                ReorderLevel = 8, 
                CategoryId = beaute.Id,
                Description = "Sérum visage anti-âge à l'acide hyaluronique. Hydrate intensément et réduit l'apparition des rides. Texture légère, pénétration rapide. Flacon 30ml.",
                ImageUrl = "https://images.unsplash.com/photo-1556228578-0d85b1a4d571?w=400"
            });

            AddCustomer(new Customer { FullName = "Alice Dupont", Email = "alice@example.com", Phone = "0601020304", LoyaltyPoints = 120 });
            AddCustomer(new Customer { FullName = "Karim Bouaziz", Email = "karim@example.com", Phone = "0611223344", LoyaltyPoints = 60 });

            AddSupplier(new Supplier { Name = "Fournisseur Santé", Email = "contact@fournisseursante.fr", Phone = "0142501122" });
            AddSupplier(new Supplier { Name = "Bien-être Plus", Email = "hello@bienetreplus.fr", Phone = "0142501133" });

            // Demo users (password = 123)
            AddUser(new User { Username = "admin", Password = "123", Role = Role.Admin });
            AddUser(new User { Username = "gerant", Password = "123", Role = Role.Gerant });
            AddUser(new User { Username = "caissier", Password = "123", Role = Role.Caissier });
            AddUser(new User { Username = "client", Password = "123", Role = Role.Client, CustomerId = 1 });
            AddUser(new User { Username = "fournisseur", Password = "123", Role = Role.Supplier, SupplierId = 1 });
        }

        public static Product AddProduct(Product product)
        {
            product.Id = _productId++;
            Products.Add(product);
            return product;
        }

        public static Category AddCategory(Category category)
        {
            category.Id = _categoryId++;
            Categories.Add(category);
            return category;
        }

        public static Customer AddCustomer(Customer customer)
        {
            customer.Id = _customerId++;
            Customers.Add(customer);
            return customer;
        }

        public static Supplier AddSupplier(Supplier supplier)
        {
            supplier.Id = _supplierId++;
            Suppliers.Add(supplier);
            return supplier;
        }

        public static User AddUser(User user)
        {
            user.Id = _userId++;
            Users.Add(user);
            return user;
        }

        public static Sale AddSale(Sale sale)
        {
            sale.Id = _saleId++;
            Sales.Add(sale);
            return sale;
        }

        public static SupplierOrder AddSupplierOrder(SupplierOrder order)
        {
            order.Id = _supplierOrderId++;
            SupplierOrders.Add(order);
            return order;
        }
    }
}

