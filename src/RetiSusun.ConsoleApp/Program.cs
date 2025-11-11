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
        services.AddScoped<ISupplierService, SupplierService>();
        services.AddScoped<ISupplierProductService, SupplierProductService>();
        services.AddScoped<ISupplierOrderService, SupplierOrderService>();
    }

    private static async Task RunDemoAsync()
    {
        using var scope = _serviceProvider!.CreateScope();
        var authService = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();
        var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
        var salesService = scope.ServiceProvider.GetRequiredService<ISalesService>();
        var restockingService = scope.ServiceProvider.GetRequiredService<IRestockingService>();
        var supplierService = scope.ServiceProvider.GetRequiredService<ISupplierService>();
        var supplierProductService = scope.ServiceProvider.GetRequiredService<ISupplierProductService>();
        var supplierOrderService = scope.ServiceProvider.GetRequiredService<ISupplierOrderService>();
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
            transaction.TaxRate = 6.00m; // 6% tax
            transaction.TaxAmount = transaction.SubTotal * (transaction.TaxRate / 100);
            transaction.DiscountAmount = 0;
            transaction.TotalAmount = transaction.SubTotal + transaction.TaxAmount - transaction.DiscountAmount;
            transaction.AmountPaid = 50.00m;
            transaction.ChangeAmount = transaction.AmountPaid - transaction.TotalAmount;

            var savedTransaction = await salesService.CreateSalesTransactionAsync(transaction);
            Console.WriteLine($"✓ Transaction created: {savedTransaction.TransactionNumber}");
            Console.WriteLine($"  Total: RM{savedTransaction.TotalAmount:F2}");
            Console.WriteLine($"  Items sold: {savedTransaction.Items.Count}");

            // Generate and display receipt
            Console.WriteLine("\n   RECEIPT PREVIEW:");
            Console.WriteLine("   ─────────────────────────────────────────");
            var receipt = await salesService.GenerateReceiptAsync(savedTransaction.TransactionId);
            foreach (var line in receipt.Split('\n'))
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

        Console.WriteLine("\n7. Creating Sample Supplier...");
        Console.WriteLine("───────────────────────────────────────────────────────────");

        var existingSupplier = await dbContext.Suppliers.FirstOrDefaultAsync();
        Supplier supplier;

        if (existingSupplier == null)
        {
            supplier = new Supplier
            {
                CompanyName = "Global Wholesale Distributors",
                Address = "456 Industrial Park, Kota Kinabalu, Sabah",
                Phone = "088-789012",
                Email = "sales@globalwholesale.com",
                RegistrationNumber = "GWD-2024-001",
                ContactPersonName = "Sarah Lee",
                ContactPersonEmail = "sarah@globalwholesale.com",
                ContactPersonPhone = "019-8765432",
                Description = "Leading wholesale distributor for retail goods in Sabah",
                IsActive = true,
                IsOpenForBusiness = true
            };
            supplier = await supplierService.CreateSupplierAsync(supplier);
            Console.WriteLine($"✓ Supplier created: {supplier.CompanyName}");

            // Create supplier admin user
            var supplierAdmin = await authService.RegisterSupplierUserAsync(
                "supplier_admin",
                "supplier123",
                "Sarah Lee",
                "sarah@globalwholesale.com",
                "Admin",
                supplier.SupplierId
            );
            Console.WriteLine($"✓ Supplier admin user created: {supplierAdmin.Username}");
        }
        else
        {
            supplier = existingSupplier;
            Console.WriteLine($"✓ Using existing supplier: {supplier.CompanyName}");
        }

        Console.WriteLine("\n8. Adding Supplier Products to Catalog...");
        Console.WriteLine("───────────────────────────────────────────────────────────");

        var existingSupplierProducts = await supplierProductService.GetProductsBySupplierIdAsync(supplier.SupplierId);
        if (!existingSupplierProducts.Any())
        {
            var supplierProducts = new[]
            {
                new SupplierProduct
                {
                    Name = "Bulk Mineral Water 1.5L (24-Pack)",
                    Description = "Carton of 24 bottles of mineral water",
                    Brand = "Spritzer",
                    Barcode = "SUP-8888002",
                    SKU = "SUP-BEV-001",
                    WholesalePrice = 25.00m,
                    MinimumOrderQuantity = 10,
                    AvailableStock = 500,
                    Category = "Beverages",
                    Unit = "carton",
                    SupplierId = supplier.SupplierId
                },
                new SupplierProduct
                {
                    Name = "Instant Noodles Curry (30-Pack)",
                    Description = "Wholesale pack of 30 Maggi curry noodles",
                    Brand = "Maggi",
                    Barcode = "SUP-8888004",
                    SKU = "SUP-FOOD-001",
                    WholesalePrice = 20.00m,
                    MinimumOrderQuantity = 5,
                    AvailableStock = 1000,
                    Category = "Food",
                    Unit = "carton",
                    SupplierId = supplier.SupplierId
                },
                new SupplierProduct
                {
                    Name = "Milo Powder 1kg (12-Pack)",
                    Description = "Bulk pack of Milo chocolate malt drink",
                    Brand = "Nestle",
                    Barcode = "SUP-8888005",
                    SKU = "SUP-BEV-002",
                    WholesalePrice = 140.00m,
                    MinimumOrderQuantity = 5,
                    AvailableStock = 200,
                    Category = "Beverages",
                    Unit = "carton",
                    SupplierId = supplier.SupplierId
                },
                new SupplierProduct
                {
                    Name = "Dove Soap 100g (48-Pack)",
                    Description = "Wholesale carton of beauty soap bars",
                    Brand = "Unilever",
                    Barcode = "SUP-8888007",
                    SKU = "SUP-CARE-001",
                    WholesalePrice = 120.00m,
                    MinimumOrderQuantity = 3,
                    AvailableStock = 150,
                    Category = "Personal Care",
                    Unit = "carton",
                    SupplierId = supplier.SupplierId
                },
                new SupplierProduct
                {
                    Name = "Pringles Original 107g (12-Pack)",
                    Description = "Carton of 12 Pringles potato crisps",
                    Brand = "Pringles",
                    Barcode = "SUP-8888008",
                    SKU = "SUP-SNACK-001",
                    WholesalePrice = 45.00m,
                    MinimumOrderQuantity = 5,
                    AvailableStock = 300,
                    Category = "Snacks",
                    Unit = "carton",
                    SupplierId = supplier.SupplierId
                }
            };

            foreach (var product in supplierProducts)
            {
                await supplierProductService.CreateProductAsync(product);
                Console.WriteLine($"✓ Added: {product.Name} - Wholesale: RM{product.WholesalePrice} | Stock: {product.AvailableStock}");
            }
        }
        else
        {
            Console.WriteLine($"✓ Using {existingSupplierProducts.Count()} existing supplier products");
        }

        Console.WriteLine("\n9. Browsing Available Suppliers...");
        Console.WriteLine("───────────────────────────────────────────────────────────");

        var availableSuppliers = await supplierService.GetOpenForBusinessSuppliersAsync();
        Console.WriteLine($"✓ Found {availableSuppliers.Count()} suppliers open for business:");
        foreach (var sup in availableSuppliers)
        {
            Console.WriteLine($"  • {sup.CompanyName}");
            Console.WriteLine($"    Contact: {sup.ContactPersonName} ({sup.ContactPersonEmail})");
            var productCount = await supplierProductService.GetActiveProductsBySupplierIdAsync(sup.SupplierId);
            Console.WriteLine($"    Available Products: {productCount.Count()}");
        }

        Console.WriteLine("\n10. Placing Order from Supplier...");
        Console.WriteLine("───────────────────────────────────────────────────────────");

        var supplierProductsList = (await supplierProductService.GetActiveProductsBySupplierIdAsync(supplier.SupplierId)).ToList();
        if (supplierProductsList.Any())
        {
            var order = new SupplierOrder
            {
                SupplierId = supplier.SupplierId,
                BusinessId = business.BusinessId,
                Status = "Pending",
                DeliveryAddress = business.Address,
                Items = new List<SupplierOrderItem>
                {
                    new SupplierOrderItem
                    {
                        SupplierProductId = supplierProductsList[0].SupplierProductId,
                        Quantity = 10,
                        UnitPrice = supplierProductsList[0].WholesalePrice,
                        TotalPrice = supplierProductsList[0].WholesalePrice * 10
                    },
                    new SupplierOrderItem
                    {
                        SupplierProductId = supplierProductsList[1].SupplierProductId,
                        Quantity = 5,
                        UnitPrice = supplierProductsList[1].WholesalePrice,
                        TotalPrice = supplierProductsList[1].WholesalePrice * 5
                    }
                }
            };

            order.TotalAmount = order.Items.Sum(i => i.TotalPrice);
            var savedOrder = await supplierOrderService.CreateOrderAsync(order);
            
            Console.WriteLine($"✓ Order created: {savedOrder.OrderNumber}");
            Console.WriteLine($"  Status: {savedOrder.Status}");
            Console.WriteLine($"  Total: RM{savedOrder.TotalAmount:F2}");
            Console.WriteLine($"  Items: {savedOrder.Items.Count}");

            foreach (var item in savedOrder.Items)
            {
                var product = await supplierProductService.GetProductByIdAsync(item.SupplierProductId);
                Console.WriteLine($"    - {product!.Name}: {item.Quantity} x RM{item.UnitPrice} = RM{item.TotalPrice}");
            }
        }

        Console.WriteLine("\n11. Supplier Dashboard Summary...");
        Console.WriteLine("───────────────────────────────────────────────────────────");

        var supplierOrders = await supplierOrderService.GetOrdersBySupplierIdAsync(supplier.SupplierId);
        var supplierTotalSales = await supplierOrderService.GetTotalSalesBySupplierIdAsync(supplier.SupplierId);
        var orderStatusSummary = await supplierOrderService.GetOrderStatusSummaryAsync(supplier.SupplierId);

        Console.WriteLine($"Supplier: {supplier.CompanyName}");
        Console.WriteLine($"Total Orders: {supplierOrders.Count()}");
        Console.WriteLine($"Total Sales: RM{supplierTotalSales:F2}");
        Console.WriteLine("Order Status Breakdown:");
        foreach (var status in orderStatusSummary)
        {
            Console.WriteLine($"  {status.Key}: {status.Value}");
        }
    }
}
