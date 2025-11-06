# RetiSusun API Documentation

## Service Interfaces

### IAuthenticationService

Handles user authentication and account management.

#### Methods

- `Task<User?> AuthenticateAsync(string username, string password)`
  - Authenticates a user with username and password
  - Returns User object if successful, null otherwise
  - Updates last login date

- `Task<User> RegisterUserAsync(string username, string password, string fullName, string email, string role, int businessId)`
  - Creates a new user account
  - Hashes password using SHA256
  - Throws exception if username already exists

- `Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)`
  - Changes user password
  - Verifies old password before updating
  - Returns true if successful

- `string HashPassword(string password)`
  - Hashes password using SHA256
  - Returns base64-encoded hash

- `bool VerifyPassword(string password, string hash)`
  - Verifies password against stored hash
  - Returns true if password matches

### IProductService

Manages product inventory and catalog.

#### Methods

- `Task<IEnumerable<Product>> GetAllProductsAsync(int businessId)`
  - Returns all active products for a business
  - Ordered by name

- `Task<Product?> GetProductByIdAsync(int productId)`
  - Gets a single product by ID

- `Task<Product?> GetProductByBarcodeAsync(string barcode, int businessId)`
  - Finds product by barcode
  - Used for barcode scanner integration

- `Task<Product> AddProductAsync(Product product)`
  - Adds new product to inventory

- `Task<Product> UpdateProductAsync(Product product)`
  - Updates existing product

- `Task<bool> DeleteProductAsync(int productId)`
  - Soft deletes product (sets IsActive = false)

- `Task<IEnumerable<Product>> GetLowStockProductsAsync(int businessId)`
  - Returns products below reorder level
  - Ordered by stock quantity (lowest first)

- `Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm, int businessId)`
  - Searches products by name, barcode, or SKU

### ISalesService

Handles sales transactions and reporting.

#### Methods

- `Task<SalesTransaction> CreateSalesTransactionAsync(SalesTransaction transaction)`
  - Creates new sale
  - Generates transaction number
  - Updates product stock
  - Uses database transaction for atomicity

- `Task<SalesTransaction?> GetSalesTransactionByIdAsync(int transactionId)`
  - Gets transaction with items and user details

- `Task<IEnumerable<SalesTransaction>> GetSalesTransactionsAsync(int businessId, DateTime? startDate, DateTime? endDate)`
  - Gets sales history with optional date filtering
  - Excludes voided transactions

- `Task<bool> VoidTransactionAsync(int transactionId, string reason, int userId)`
  - Voids a transaction
  - Restores stock quantities
  - Records void reason

- `Task<decimal> GetTotalSalesAsync(int businessId, DateTime? startDate, DateTime? endDate)`
  - Calculates total sales amount
  - Optional date range filtering

- `Task<string> GenerateReceiptAsync(int transactionId)`
  - Generates formatted receipt text
  - Includes all transaction details

### IRestockingService

Manages inventory restocking operations.

#### Methods

- `Task<RestockingRecord> AddRestockingRecordAsync(RestockingRecord record)`
  - Records inventory restocking
  - Updates product stock quantity
  - Tracks before/after stock levels

- `Task<IEnumerable<RestockingRecord>> GetRestockingHistoryAsync(int productId)`
  - Gets restocking history for a product

- `Task<Dictionary<int, int>> GetRestockingSuggestionsAsync(int businessId)`
  - **AI-Powered Feature**
  - Analyzes 30-day sales trends
  - Calculates optimal restock quantities
  - Returns product ID → suggested quantity mapping
  
  **Algorithm:**
  ```
  1. Find products below reorder level
  2. Get sales data from last 30 days
  3. Calculate average daily sales
  4. Suggest: avgDailySales × 30 days × 1.2 buffer
  5. Ensure minimum of reorderLevel × 2
  ```

- `Task<bool> ApplyRestockingSuggestionAsync(int productId, int suggestedQuantity, int userId)`
  - Applies suggested restock quantity
  - Creates restocking record

### IPurchaseOrderService

Manages supplier purchase orders.

#### Methods

- `Task<PurchaseOrder> CreatePurchaseOrderAsync(PurchaseOrder purchaseOrder)`
  - Creates new purchase order
  - Generates order number
  - Sets status to Pending

- `Task<PurchaseOrder?> GetPurchaseOrderByIdAsync(int purchaseOrderId)`
  - Gets purchase order with items

- `Task<IEnumerable<PurchaseOrder>> GetPurchaseOrdersAsync(int businessId, string? status)`
  - Gets all purchase orders
  - Optional status filtering

- `Task<PurchaseOrder> UpdatePurchaseOrderAsync(PurchaseOrder purchaseOrder)`
  - Updates purchase order details

- `Task<bool> ReceivePurchaseOrderAsync(int purchaseOrderId, int userId)`
  - Marks order as received
  - Updates product stock
  - Creates restocking records
  - Sets actual delivery date

## Data Models

### Business
- BusinessId (PK)
- BusinessName
- Address, Phone, Email
- OwnerName, OwnerEmail, OwnerPhone
- CreatedDate, IsActive

### User
- UserId (PK)
- Username (unique)
- PasswordHash
- FullName, Email
- Role (Admin/Manager/Cashier)
- BusinessId (FK)
- IsActive, CreatedDate, LastLoginDate

### Product
- ProductId (PK)
- Name, Description
- Barcode, SKU
- CostPrice, SellingPrice
- StockQuantity
- MinimumStockLevel, ReorderLevel
- Category, Unit
- BusinessId (FK)
- IsActive, CreatedDate, LastRestockedDate

### SalesTransaction
- TransactionId (PK)
- TransactionNumber (unique)
- TransactionDate
- SubTotal, TaxAmount, DiscountAmount, TotalAmount
- PaymentMethod
- AmountPaid, ChangeAmount
- UserId (FK), BusinessId (FK)
- IsVoided, VoidedDate, VoidReason

### SalesTransactionItem
- TransactionItemId (PK)
- TransactionId (FK)
- ProductId (FK)
- Quantity
- UnitPrice, DiscountAmount, TotalPrice

### PurchaseOrder
- PurchaseOrderId (PK)
- OrderNumber (unique)
- OrderDate, ExpectedDeliveryDate, ActualDeliveryDate
- Status (Pending/Ordered/Received/Cancelled)
- SupplierName, SupplierEmail, SupplierPhone
- TotalAmount
- BusinessId (FK)
- CreatedByUserId, ReceivedByUserId

### PurchaseOrderItem
- PurchaseOrderItemId (PK)
- PurchaseOrderId (FK)
- ProductId (FK)
- QuantityOrdered, QuantityReceived
- UnitCost, TotalCost

### RestockingRecord
- RestockingRecordId (PK)
- ProductId (FK)
- RestockDate
- QuantityAdded
- StockBeforeRestock, StockAfterRestock
- UnitCost, TotalCost
- Source (Manual/PurchaseOrder)
- RestockedByUserId
- PurchaseOrderId (optional)

## Database Configuration

### Connection String

SQLite: `Data Source=retisusun.db`

For production with SQL Server:
```csharp
services.AddDbContext<RetiSusunDbContext>(options =>
    options.UseSqlServer("Server=.;Database=RetiSusun;Trusted_Connection=True;"));
```

### Indexes

- User.Username (unique)
- Product.Barcode
- Product.SKU
- SalesTransaction.TransactionNumber (unique)
- PurchaseOrder.OrderNumber (unique)

## Error Handling

All services use async/await with proper exception handling. Common exceptions:

- `InvalidOperationException` - Business rule violation
- `DbUpdateException` - Database constraint violation
- `ArgumentException` - Invalid input parameters

## Security Considerations

1. **Password Security**: SHA256 hashing (consider bcrypt for production)
2. **SQL Injection**: Protected by EF Core parameterization
3. **Authorization**: Implement role-based checks in UI layer
4. **Transactions**: Database transactions ensure data consistency

## Performance Optimization

1. Use `.AsNoTracking()` for read-only queries
2. Implement caching for frequently accessed data
3. Use pagination for large result sets
4. Index frequently queried columns
5. Consider async operations for I/O-bound work
