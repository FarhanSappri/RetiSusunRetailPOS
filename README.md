# RetiSusun - Retail Point-of-Sale and Inventory Management System

[![.NET Version](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/download)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

## 📖 Overview

**RetiSusun** is a comprehensive Point-of-Sale (POS) and Inventory Management System designed specifically for small and micro retail enterprises in Malaysia. The system addresses critical challenges such as human error, poor restocking decisions, and lack of accessible sales data that traditional retail systems face.

### Key Features

#### Business Owner Side:
- ✅ **User Management** - Multi-user support with role-based access control (Admin, Manager, Cashier)
- ✅ **Product Management** - Comprehensive inventory tracking with barcode support
- ✅ **Point of Sale** - Fast and efficient sales transaction processing
- ✅ **Smart Restocking** - AI-powered restocking suggestions based on sales trends
- ✅ **Purchase Orders** - Supplier order management and tracking
- ✅ **Supplier Browsing** - Browse available suppliers and their product catalogs
- ✅ **Direct Ordering** - Place orders directly from supplier catalogs
- ✅ **Sales Reports** - Detailed business analytics and reporting
- ✅ **Receipt Generation** - Professional receipt printing
- ✅ **Multi-Business Support** - Manage multiple retail locations

#### Supplier Side:
- ✅ **Supplier Registration** - Easy registration process for wholesale suppliers
- ✅ **Product Catalog Management** - Manage wholesale product listings
- ✅ **Order Management** - View and process incoming orders from businesses
- ✅ **Order Status Tracking** - Update order status (Pending, Confirmed, Preparing, Shipped, Delivered)
- ✅ **Sales Analytics** - Track total sales, order counts, and business metrics
- ✅ **Inventory Tracking** - Monitor available stock for wholesale products
- ✅ **Business Dashboard** - Comprehensive dashboard for managing supplier operations

## 🏗️ Architecture

The project follows a **3-tier architecture** pattern:

```
┌─────────────────────────────────────┐
│     Presentation Layer              │
│  (WinForms Desktop Application)    │
└─────────────────────────────────────┘
              ↓
┌─────────────────────────────────────┐
│      Business Logic Layer           │
│     (RetiSusun.Core)                │
│  - Services & Interfaces            │
│  - Business Rules                   │
└─────────────────────────────────────┘
              ↓
┌─────────────────────────────────────┐
│       Data Access Layer             │
│     (RetiSusun.Data)                │
│  - Entity Framework Core            │
│  - SQLite Database                  │
└─────────────────────────────────────┘
```

## 🛠️ Technology Stack

- **Framework**: .NET 8.0
- **UI**: Windows Forms (WinForms)
- **Database**: SQLite (with Entity Framework Core)
- **ORM**: Entity Framework Core 9.0
- **Language**: C# 12
- **Architecture**: 3-Tier Architecture
- **Design Patterns**: Repository Pattern, Dependency Injection

## 📋 Prerequisites

- [.NET 8.0 SDK or later](https://dotnet.microsoft.com/download)
- Windows 10/11 (for WinForms application)
- Visual Studio 2022 or VS Code (recommended)

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/FarhanSappri/RetiSusunRetailPOS.git
cd RetiSusunRetailPOS
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Build the Solution

```bash
dotnet build
```

### 4. Run the Console Demo (Cross-Platform)

```bash
dotnet run --project src/RetiSusun.ConsoleApp/RetiSusun.ConsoleApp.csproj
```

### 5. Run the Desktop Application (Windows Only)

**Note**: The WinForms application can only be built and run on Windows.

```bash
# On Windows:
cd src/RetiSusun.Desktop
dotnet run
```

## 📦 Project Structure

```
RetiSusunRetailPOS/
├── src/
│   ├── RetiSusun.Data/          # Data Access Layer
│   │   ├── Models/              # Entity models
│   │   │   ├── Business.cs
│   │   │   ├── User.cs
│   │   │   ├── Product.cs
│   │   │   ├── SalesTransaction.cs
│   │   │   ├── PurchaseOrder.cs
│   │   │   ├── RestockingRecord.cs
│   │   │   ├── Supplier.cs
│   │   │   ├── SupplierProduct.cs
│   │   │   ├── SupplierOrder.cs
│   │   │   └── SupplierOrderItem.cs
│   │   └── RetiSusunDbContext.cs
│   │
│   ├── RetiSusun.Core/          # Business Logic Layer
│   │   ├── Interfaces/          # Service interfaces
│   │   │   ├── IAuthenticationService.cs
│   │   │   ├── IProductService.cs
│   │   │   ├── ISalesService.cs
│   │   │   ├── IRestockingService.cs
│   │   │   ├── IPurchaseOrderService.cs
│   │   │   ├── ISupplierService.cs
│   │   │   ├── ISupplierProductService.cs
│   │   │   └── ISupplierOrderService.cs
│   │   └── Services/            # Service implementations
│   │       ├── AuthenticationService.cs
│   │       ├── ProductService.cs
│   │       ├── SalesService.cs
│   │       ├── RestockingService.cs
│   │       ├── PurchaseOrderService.cs
│   │       ├── SupplierService.cs
│   │       ├── SupplierProductService.cs
│   │       └── SupplierOrderService.cs
│   │
│   ├── RetiSusun.Desktop/       # WinForms UI (Windows only)
│   │   ├── Forms/
│   │   │   ├── LoginForm.cs
│   │   │   ├── RegistrationForm.cs
│   │   │   ├── MainForm.cs
│   │   │   ├── SupplierRegistrationForm.cs
│   │   │   └── SupplierDashboardForm.cs
│   │   └── Program.cs
│   │
│   └── RetiSusun.ConsoleApp/    # Console Demo (Cross-platform)
│       └── Program.cs
│
├── RetiSusun.sln                # Solution file
├── .gitignore
└── README.md
```

## 🎯 Core Modules

### 1. Authentication & User Management
- User registration with business information
- Supplier registration with company details
- Secure password hashing (SHA256)
- Role-based access control (Admin, Manager, Cashier)
- Account type separation (Business, Supplier)
- Multi-business and multi-supplier support

### 2. Product Management (Business Side)
- Add, edit, and delete products
- Barcode scanning support
- Stock level tracking
- Low stock alerts
- Category and SKU management

### 3. Supplier Product Management (Supplier Side)
- Manage wholesale product catalog
- Set wholesale pricing and minimum order quantities
- Track available stock levels
- Categorize products for easy browsing
- Active/inactive product status management

### 4. Point of Sale
- Fast transaction processing
- Multiple payment methods (Cash, Card, E-Wallet)
- Real-time stock updates
- Receipt generation

### 5. Restocking Management
- Manual restocking
- Restocking history tracking
- **AI-Powered Suggestions**: Analyzes sales trends to recommend optimal restock quantities
- Integration with purchase orders

### 6. Purchase Order Management
- Create and manage supplier orders
- Track order status (Pending, Ordered, Received, Cancelled)
- Automatic stock updates on receipt
- Supplier information management
- **Browse Suppliers**: View available suppliers and their catalogs
- **Direct Ordering**: Place orders from supplier product catalogs

### 7. Supplier Order Management
- View incoming orders from businesses
- Update order status (Pending, Confirmed, Preparing, Shipped, Delivered, Cancelled)
- Track order details and delivery information
- Monitor order fulfillment progress

### 8. Sales Reports & Analytics (Business Side)
- Daily, weekly, monthly sales reports
- Best-selling products
- Inventory valuation
- Profit/Loss analysis

### 9. Supplier Analytics & Reports
- Total sales tracking
- Order status summaries
- Active product counts
- Pending order monitoring
- Business performance metrics

## 🤖 AI-Powered Restocking Engine

The system includes an intelligent restocking suggestion engine that:

1. Analyzes sales data from the last 30 days
2. Calculates average daily sales for each product
3. Considers current stock levels and reorder points
4. Suggests optimal restock quantities with a 20% safety buffer
5. Adapts recommendations based on sales patterns

**Algorithm**:
```csharp
suggestedQuantity = (averageDailySales × 30 days × 1.2 buffer)
orderQuantity = max(0, suggestedQuantity - currentStock)
```

## 💾 Database Schema

The system uses SQLite with the following main entities:

### Business Side:
- **Business** - Store business information
- **User** - System users with roles (Business or Supplier account type)
- **Product** - Inventory items
- **SalesTransaction** - Sales records
- **SalesTransactionItem** - Individual items in transactions
- **PurchaseOrder** - Supplier orders (can link to registered suppliers)
- **PurchaseOrderItem** - Items in purchase orders
- **RestockingRecord** - Stock replenishment history

### Supplier Side:
- **Supplier** - Supplier company information and contact details
- **SupplierProduct** - Wholesale products in supplier catalogs
- **SupplierOrder** - Orders from businesses to suppliers
- **SupplierOrderItem** - Items in supplier orders

### Relationships:
- Users can belong to either a Business or a Supplier
- PurchaseOrders can optionally link to registered Suppliers
- SupplierOrders connect Suppliers with Businesses
- SupplierOrderItems reference SupplierProducts

## 🔐 Security Features

- Password hashing using SHA256
- User session management
- Role-based access control
- Transaction integrity with database transactions
- Input validation and sanitization

## 📊 Sample Data

The console demo automatically creates sample data including:

### Business Side:
- A demo retail business
- An admin user (username: `admin`, password: `admin123`)
- Sample products (Nasi Lemak, Mineral Water, White Bread, Instant Noodles, etc.)
- Sample sales transactions

### Supplier Side:
- A demo supplier (Global Wholesale Distributors)
- A supplier admin user (username: `supplier_admin`, password: `supplier123`)
- Sample wholesale products with bulk pricing
- Sample supplier orders from businesses

## 🎨 User Interface

### Login Screen
- Simple username/password authentication
- Link to registration for new businesses
- Link to registration for new suppliers
- Automatic routing based on account type

### Business Registration Screen
- Business information collection
- Owner details
- Admin user creation

### Supplier Registration Screen
- Company information collection
- Contact person details
- Admin user creation
- Business description

### Business Owner Dashboard (Main Form)
- Tabbed interface with:
  - Point of Sale
  - Product Management
  - Sales History
  - Restocking Management
  - Purchase Orders (with Browse Suppliers feature)
  - Business Reports (Admin/Manager only)

### Supplier Dashboard
- Tabbed interface with:
  - Product Catalog Management
  - Order Management (view and process orders)
  - Sales Analytics and Reports
- Features for updating order status
- Inventory tracking for wholesale products

## 🧪 Testing

Run the console application to see a complete demo of all features:

```bash
dotnet run --project src/RetiSusun.ConsoleApp/RetiSusun.ConsoleApp.csproj
```

The demo will:
1. Create a business and admin user
2. Add sample products
3. Process a sales transaction
4. Check for low stock products
5. Generate restocking suggestions
6. Display sales summary
7. Create a sample supplier
8. Add supplier products to catalog
9. Browse available suppliers
10. Place an order from supplier
11. Display supplier dashboard summary

## 📝 Development Notes

### Building on Non-Windows Platforms

The WinForms desktop application requires Windows to build and run. However, the core business logic and data layers can be developed and tested on any platform using the console application.

To exclude the Desktop project on non-Windows systems:
```bash
dotnet sln remove src/RetiSusun.Desktop/RetiSusun.Desktop.csproj
```

### Database Migrations

The system uses `EnsureCreated()` for automatic database creation. For production, consider using EF Core migrations:

```bash
dotnet ef migrations add InitialCreate --project src/RetiSusun.Data
dotnet ef database update --project src/RetiSusun.Data
```

## 🔄 Future Enhancements

- [ ] Web-based dashboard (ASP.NET Core)
- [ ] Mobile app (MAUI)
- [ ] Cloud synchronization
- [ ] Advanced analytics and charts
- [ ] Multi-language support
- [ ] Barcode scanner hardware integration
- [ ] Receipt printer integration
- [ ] Export to Excel/PDF
- [ ] Customer loyalty program
- [ ] Multi-currency support

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👥 Author

**Hammad Farhan Bin Sappri** (BI22110301)
- Bachelor of Computer Science with Honours (Software Engineering)
- Faculty of Computing and Informatics
- Universiti Malaysia Sabah
- 2025

## 🙏 Acknowledgments

- Based on research and requirements from "RetiSusun: Retail Point-of-Sale and Inventory System" thesis
- Developed as part of Final Year Project (FYP)
- Supervised by Dr. Suraya Alias

## 📞 Support

For issues, questions, or contributions, please open an issue on GitHub.

---

**Note**: This system is designed for educational purposes and small retail businesses. For production use, additional security hardening and testing is recommended.
