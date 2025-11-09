using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RetiSusun.Core.Interfaces;
using RetiSusun.Core.Services;
using RetiSusun.Data;
using RetiSusun.Data.Models;

namespace RetiSusun.ConsoleApp;

class Program
{
    private static ServiceProvider? _serviceProvider;

    static async Task Main(string[] args)
    {
        Console.WriteLine("═══════════════════════════════════════════════════════════");
        Console.WriteLine("    RetiSusun - Retail POS & Inventory Management System");
        Console.WriteLine("═══════════════════════════════════════════════════════════\n");

        // Setup DI
        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();

        // Initialize database
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<RetiSusunDbContext>();
            await context.Database.EnsureCreatedAsync();
            Console.WriteLine("✓ Database initialized successfully\n");
        }

        // Run demo
        await RunDemoAsync();

        Console.WriteLine("\n═══════════════════════════════════════════════════════════");
        Console.WriteLine("Demo completed! Press any key to exit...");
        Console.ReadKey();
    }

    private static void ConfigureServices(ServiceCollection services)
    {
        services.AddDbContext<RetiSusunDbContext>(options =>
            options.UseSqlite("Data Source=retisusun_demo.db"));

        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ISalesService, SalesService>();
        services.AddScoped<IRestockingService, RestockingService>();
        services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
    }

    private static async Task RunDemoAsync()
    {
        using var scope = _serviceProvider!.CreateScope();
        var authService = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();
        var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
        var salesService = scope.ServiceProvider.GetRequiredService<ISalesService>();
        var restockingService = scope.ServiceProvider.GetRequiredService<IRestockingService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<RetiSusunDbContext>();

        Console.WriteLine("1. Creating Business and Admin User...");
        Console.WriteLine("───────────────────────────────────────────────────────────");

        // Check if business already exists
        var existingBusiness = await dbContext.Businesses.FirstOrDefaultAsync();
        Business business;

        if (existingBusiness == null)
        {
            business = new Business
            {
                BusinessName = "Demo Retail Store",
                Address = "123 Main Street, Kota Kinabalu, Sabah",
                Phone = "088-123456",
                OwnerName = "John Doe",
                OwnerEmail = "john@demostore.com",
                OwnerPhone = "013-1234567"
            };
            dbContext.Businesses.Add(business);
            await dbContext.SaveChangesAsync();
            Console.WriteLine($"✓ Business created: {business.BusinessName}");

            // Create admin user
            var adminUser = await authService.RegisterUserAsync(
                "admin",
                "admin123",
                "John Doe",
                "john@demostore.com",
                "Admin",
                business.BusinessId
            );
            Console.WriteLine($"✓ Admin user created: {adminUser.Username}");
        }
        else
        {
            business = existingBusiness;
            Console.WriteLine($"✓ Using existing business: {business.BusinessName}");
        }

        Console.WriteLine("\n2. Adding Sample Products...");
        Console.WriteLine("───────────────────────────────────────────────────────────");

        var existingProducts = await productService.GetAllProductsAsync(business.BusinessId);
        if (!existingProducts.Any())
        {
            var products = new[]
            {
                new Product
                {
                    Name = "Nasi Lemak",
                    Description = "Traditional Malaysian breakfast",
                    Brand = "Kak Tie",
                    Barcode = "8888001",
                    SKU = "FOOD-001",
                    CostPrice = 3.00m,
                    SellingPrice = 5.50m,
                    StockQuantity = 50,
                    MinimumStockLevel = 10,
                    ReorderLevel = 20,
                    Category = "Food",
                    Unit = "pcs",
                    BusinessId = business.BusinessId
                },
                new Product
                {
                    Name = "Mineral Water 1.5L",
                    Description = "Bottled drinking water",
                    Brand = "Spritzer",
                    Barcode = "8888002",
                    SKU = "BEV-001",
                    CostPrice = 1.20m,
                    SellingPrice = 2.00m,
                    StockQuantity = 100,
                    MinimumStockLevel = 20,
                    ReorderLevel = 40,
                    Category = "Beverages",
                    Unit = "bottle",
                    BusinessId = business.BusinessId
                },
                new Product
                {
                    Name = "White Bread",
                    Description = "Fresh white bread loaf",
                    Brand = "Gardenia",
                    Barcode = "8888003",
                    SKU = "BAKERY-001",
                    CostPrice = 2.50m,
                    SellingPrice = 4.20m,
                    StockQuantity = 30,
                    MinimumStockLevel = 5,
                    ReorderLevel = 15,
                    Category = "Bakery",
                    Unit = "loaf",
                    BusinessId = business.BusinessId
                },
                new Product
                {
                    Name = "Instant Noodles Curry",
                    Description = "Maggi curry flavor",
                    Brand = "Maggi",
                    Barcode = "8888004",
                    SKU = "FOOD-002",
                    CostPrice = 0.80m,
                    SellingPrice = 1.50m,
                    StockQuantity = 200,
                    MinimumStockLevel = 50,
                    ReorderLevel = 100,
                    Category = "Food",
                    Unit = "pack",
                    BusinessId = business.BusinessId
                },
                new Product
                {
                    Name = "Milo Powder 1kg",
                    Description = "Chocolate malt drink powder",
                    Brand = "Nestle",
                    Barcode = "8888005",
                    SKU = "BEV-002",
                    CostPrice = 12.50m,
                    SellingPrice = 18.90m,
                    StockQuantity = 45,
                    MinimumStockLevel = 10,
                    ReorderLevel = 20,
                    Category = "Beverages",
                    Unit = "pack",
                    BusinessId = business.BusinessId
                },
                new Product
                {
                    Name = "Maggi Kari",
                    Description = "Instant curry noodles",
                    Brand = "Maggi",
                    Barcode = "8888006",
                    SKU = "FOOD-003",
                    CostPrice = 0.75m,
                    SellingPrice = 1.40m,
                    StockQuantity = 180,
                    MinimumStockLevel = 40,
                    ReorderLevel = 80,
                    Category = "Food",
                    Unit = "pack",
                    BusinessId = business.BusinessId
                },
                new Product
                {
                    Name = "Dove Soap 100g",
                    Description = "Beauty cream soap bar",
                    Brand = "Unilever",
                    Barcode = "8888007",
                    SKU = "CARE-001",
                    CostPrice = 2.80m,
                    SellingPrice = 4.50m,
                    StockQuantity = 60,
                    MinimumStockLevel = 15,
                    ReorderLevel = 30,
                    Category = "Personal Care",
                    Unit = "pcs",
                    BusinessId = business.BusinessId
                },
                new Product
                {
                    Name = "Pringles Original 107g",
                    Description = "Potato crisps",
                    Brand = "Pringles",
                    Barcode = "8888008",
                    SKU = "SNACK-001",
                    CostPrice = 4.20m,
                    SellingPrice = 6.90m,
                    StockQuantity = 80,
                    MinimumStockLevel = 20,
                    ReorderLevel = 40,
                    Category = "Snacks",
                    Unit = "can",
                    BusinessId = business.BusinessId
                },
                new Product
                {
                    Name = "Nescafe Classic 200g",
                    Description = "Instant coffee powder",
                    Brand = "Nestle",
                    Barcode = "8888009",
                    SKU = "BEV-003",
                    CostPrice = 11.00m,
                    SellingPrice = 16.50m,
                    StockQuantity = 35,
                    MinimumStockLevel = 8,
                    ReorderLevel = 15,
                    Category = "Beverages",
                    Unit = "jar",
                    BusinessId = business.BusinessId
                },
                new Product
                {
                    Name = "Dutch Lady Milk 1L",
                    Description = "Full cream fresh milk",
                    Brand = "Dutch Lady",
                    Barcode = "8888010",
                    SKU = "DAIRY-001",
                    CostPrice = 5.50m,
                    SellingPrice = 8.20m,
                    StockQuantity = 25,
                    MinimumStockLevel = 10,
                    ReorderLevel = 20,
                    Category = "Dairy",
                    Unit = "carton",
                    BusinessId = business.BusinessId
                }
            };

            foreach (var product in products)
            {
                await productService.AddProductAsync(product);
                Console.WriteLine($"✓ Added: {product.Name} ({product.Brand}) - Stock: {product.StockQuantity} | Price: RM{product.SellingPrice}");
            }
        }
        else
        {
            Console.WriteLine($"✓ Using {existingProducts.Count()} existing products");
        }

        Console.WriteLine("\n3. Processing Sample Sales Transaction...");
        Console.WriteLine("───────────────────────────────────────────────────────────");

        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.BusinessId == business.BusinessId);
        if (user != null)
        {
            var allProducts = (await productService.GetAllProductsAsync(business.BusinessId)).ToList();

            var transaction = new SalesTransaction
            {
                BusinessId = business.BusinessId,
                UserId = user.UserId,
                PaymentMethod = "Cash",
                Items = new List<SalesTransactionItem>
                {
                    new SalesTransactionItem
                    {
                        ProductId = allProducts[0].ProductId,
                        Quantity = 2,
                        UnitPrice = allProducts[0].SellingPrice,
                        TotalPrice = allProducts[0].SellingPrice * 2
                    },
                    new SalesTransactionItem
                    {
                        ProductId = allProducts[1].ProductId,
                        Quantity = 3,
                        UnitPrice = allProducts[1].SellingPrice,
                        TotalPrice = allProducts[1].SellingPrice * 3
                    }
                }
            };

            transaction.SubTotal = transaction.Items.Sum(i => i.TotalPrice);
            transaction.TaxAmount = 0;
            transaction.DiscountAmount = 0;
            transaction.TotalAmount = transaction.SubTotal;
            transaction.AmountPaid = 20.00m;
            transaction.ChangeAmount = transaction.AmountPaid - transaction.TotalAmount;

            var savedTransaction = await salesService.CreateSalesTransactionAsync(transaction);
            Console.WriteLine($"✓ Transaction created: {savedTransaction.TransactionNumber}");
            Console.WriteLine($"  Total: RM{savedTransaction.TotalAmount:F2}");
            Console.WriteLine($"  Items sold: {savedTransaction.Items.Count}");

            // Generate and display receipt
            Console.WriteLine("\n   RECEIPT PREVIEW:");
            Console.WriteLine("   ─────────────────────────────────────────");
            var receipt = await salesService.GenerateReceiptAsync(savedTransaction.TransactionId);
            foreach (var line in receipt.Split('\n').Take(15))
            {
                Console.WriteLine($"   {line}");
            }
        }

        Console.WriteLine("\n4. Checking Low Stock Products...");
        Console.WriteLine("───────────────────────────────────────────────────────────");

        var lowStockProducts = await productService.GetLowStockProductsAsync(business.BusinessId);
        if (lowStockProducts.Any())
        {
            Console.WriteLine($"⚠ {lowStockProducts.Count()} products need restocking:");
            foreach (var product in lowStockProducts)
            {
                Console.WriteLine($"  • {product.Name}: {product.StockQuantity} units (reorder at {product.ReorderLevel})");
            }
        }
        else
        {
            Console.WriteLine("✓ All products are adequately stocked");
        }

        Console.WriteLine("\n5. Getting Restocking Suggestions (AI-Powered)...");
        Console.WriteLine("───────────────────────────────────────────────────────────");

        var suggestions = await restockingService.GetRestockingSuggestionsAsync(business.BusinessId);
        if (suggestions.Any())
        {
            Console.WriteLine($"💡 {suggestions.Count} restocking suggestions:");
            foreach (var suggestion in suggestions)
            {
                var product = await productService.GetProductByIdAsync(suggestion.Key);
                if (product != null)
                {
                    Console.WriteLine($"  • {product.Name}: Suggest ordering {suggestion.Value} units");
                }
            }
        }
        else
        {
            Console.WriteLine("✓ No restocking needed at this time");
        }

        Console.WriteLine("\n6. Sales Summary...");
        Console.WriteLine("───────────────────────────────────────────────────────────");

        var totalSales = await salesService.GetTotalSalesAsync(business.BusinessId);
        var transactions = await salesService.GetSalesTransactionsAsync(business.BusinessId);
        
        Console.WriteLine($"Total Sales: RM{totalSales:F2}");
        Console.WriteLine($"Total Transactions: {transactions.Count()}");
        Console.WriteLine($"Average Transaction: RM{(transactions.Any() ? totalSales / transactions.Count() : 0):F2}");
    }
}
