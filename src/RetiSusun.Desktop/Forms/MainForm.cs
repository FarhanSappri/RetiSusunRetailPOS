using RetiSusun.Data.Models;
using RetiSusun.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

namespace RetiSusun.Desktop.Forms;

public partial class MainForm : Form
{
    private readonly User _currentUser;
    private TabControl tabControl = null!;
    private Label lblWelcome = null!;
    
    // POS Components
    private ListView productsListView = null!;
    private ListView cartListView = null!;
    private Label lblSubTotal = null!;
    private Label lblTax = null!;
    private Label lblDiscount = null!;
    private Label lblTotal = null!;
    private NumericUpDown nudTaxRate = null!;
    private NumericUpDown nudDiscount = null!;
    private TextBox txtAmountPaid = null!;
    private Label lblChange = null!;
    private ComboBox cboPaymentMethod = null!;
    private List<Product> allProducts = new();
    private Dictionary<int, CartItem> cartItems = new();
    private decimal taxRate = 6.00m; // Default 6% tax
    private decimal discountAmount = 0.00m;

    public MainForm(User user)
    {
        _currentUser = user;
        InitializeComponent();
        lblWelcome.Text = $"Welcome, {_currentUser.FullName} ({_currentUser.Role})";
        LoadProductsAsync();
    }
    
    private class CartItem
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Total => UnitPrice * Quantity;
    }

    private void InitializeComponent()
    {
        this.Text = "RetiSusun - Point of Sale System";
        this.Size = new Size(1200, 800);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.WindowState = FormWindowState.Maximized;

        // Welcome Label
        lblWelcome = new Label
        {
            Text = "Welcome",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Location = new Point(20, 10),
            Size = new Size(500, 30)
        };
        this.Controls.Add(lblWelcome);

        // Tab Control
        tabControl = new TabControl
        {
            Location = new Point(10, 50),
            Size = new Size(this.ClientSize.Width - 20, this.ClientSize.Height - 60)
        };

        // POS Tab
        var posTab = new TabPage("Point of Sale");
        var posPanel = CreatePOSPanel();
        posPanel.Dock = DockStyle.Fill;
        posTab.Controls.Add(posPanel);
        tabControl.TabPages.Add(posTab);

        // Products Tab
        var productsTab = new TabPage("Products");
        var productsPanel = CreateProductsPanel();
        productsPanel.Dock = DockStyle.Fill;
        productsTab.Controls.Add(productsPanel);
        tabControl.TabPages.Add(productsTab);

        // Sales Tab
        var salesTab = new TabPage("Sales History");
        var salesPanel = CreateSalesPanel();
        salesPanel.Dock = DockStyle.Fill;
        salesTab.Controls.Add(salesPanel);
        tabControl.TabPages.Add(salesTab);

        // Restocking Tab
        var restockingTab = new TabPage("Restocking");
        var restockingPanel = CreateRestockingPanel();
        restockingPanel.Dock = DockStyle.Fill;
        restockingTab.Controls.Add(restockingPanel);
        tabControl.TabPages.Add(restockingTab);

        // Purchase Orders Tab
        var purchaseOrdersTab = new TabPage("Purchase Orders");
        var purchaseOrdersPanel = CreatePurchaseOrdersPanel();
        purchaseOrdersPanel.Dock = DockStyle.Fill;
        purchaseOrdersTab.Controls.Add(purchaseOrdersPanel);
        tabControl.TabPages.Add(purchaseOrdersTab);

        // Reports Tab
        if (_currentUser.Role == "Admin" || _currentUser.Role == "Manager")
        {
            var reportsTab = new TabPage("Reports");
            var reportsPanel = CreateReportsPanel();
            reportsPanel.Dock = DockStyle.Fill;
            reportsTab.Controls.Add(reportsPanel);
            tabControl.TabPages.Add(reportsTab);
        }

        this.Controls.Add(tabControl);
        this.Resize += (s, e) =>
        {
            tabControl.Size = new Size(this.ClientSize.Width - 20, this.ClientSize.Height - 60);
        };
    }

    private Panel CreatePOSPanel()
    {
        var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
        
        // Title
        var lblTitle = new Label
        {
            Text = "Point of Sale System",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            Location = new Point(10, 10),
            Size = new Size(400, 35)
        };
        panel.Controls.Add(lblTitle);
        
        // Products Section (Left side)
        var grpProducts = new GroupBox
        {
            Text = "Available Products",
            Location = new Point(10, 50),
            Size = new Size(500, 600),
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        
        productsListView = new ListView
        {
            Location = new Point(10, 25),
            Size = new Size(480, 520),
            View = View.Details,
            FullRowSelect = true,
            GridLines = true,
            Font = new Font("Segoe UI", 9)
        };
        
        productsListView.Columns.Add("Name", 180);
        productsListView.Columns.Add("Brand", 100);
        productsListView.Columns.Add("Price (RM)", 80);
        productsListView.Columns.Add("Stock", 60);
        productsListView.DoubleClick += ProductsListView_DoubleClick;
        
        var btnRefreshProducts = new Button
        {
            Text = "Refresh Products",
            Location = new Point(10, 550),
            Size = new Size(150, 35),
            Font = new Font("Segoe UI", 9)
        };
        btnRefreshProducts.Click += (s, e) => LoadProductsAsync();
        
        grpProducts.Controls.Add(productsListView);
        grpProducts.Controls.Add(btnRefreshProducts);
        panel.Controls.Add(grpProducts);
        
        // Cart Section (Right side)
        var grpCart = new GroupBox
        {
            Text = "Shopping Cart",
            Location = new Point(520, 50),
            Size = new Size(650, 400),
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        
        cartListView = new ListView
        {
            Location = new Point(10, 25),
            Size = new Size(630, 280),
            View = View.Details,
            FullRowSelect = true,
            GridLines = true,
            Font = new Font("Segoe UI", 9)
        };
        
        cartListView.Columns.Add("Product", 220);
        cartListView.Columns.Add("Brand", 100);
        cartListView.Columns.Add("Qty", 50);
        cartListView.Columns.Add("Unit Price", 80);
        cartListView.Columns.Add("Total", 80);
        
        var btnRemoveItem = new Button
        {
            Text = "Remove Item",
            Location = new Point(10, 315),
            Size = new Size(120, 35),
            Font = new Font("Segoe UI", 9)
        };
        btnRemoveItem.Click += BtnRemoveItem_Click;
        
        var btnIncreaseQty = new Button
        {
            Text = "+ Qty",
            Location = new Point(140, 315),
            Size = new Size(80, 35),
            Font = new Font("Segoe UI", 9)
        };
        btnIncreaseQty.Click += BtnIncreaseQty_Click;
        
        var btnDecreaseQty = new Button
        {
            Text = "- Qty",
            Location = new Point(230, 315),
            Size = new Size(80, 35),
            Font = new Font("Segoe UI", 9)
        };
        btnDecreaseQty.Click += BtnDecreaseQty_Click;
        
        var btnClearCart = new Button
        {
            Text = "Clear Cart",
            Location = new Point(520, 315),
            Size = new Size(120, 35),
            Font = new Font("Segoe UI", 9),
            BackColor = Color.LightCoral
        };
        btnClearCart.Click += (s, e) => ClearCart();
        
        grpCart.Controls.Add(cartListView);
        grpCart.Controls.Add(btnRemoveItem);
        grpCart.Controls.Add(btnIncreaseQty);
        grpCart.Controls.Add(btnDecreaseQty);
        grpCart.Controls.Add(btnClearCart);
        panel.Controls.Add(grpCart);
        
        // Payment Section
        var grpPayment = new GroupBox
        {
            Text = "Payment & Checkout",
            Location = new Point(520, 460),
            Size = new Size(650, 190),
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        
        // Tax Rate
        var lblTaxRate = new Label
        {
            Text = "Tax Rate (%):",
            Location = new Point(15, 30),
            Size = new Size(100, 20),
            Font = new Font("Segoe UI", 9)
        };
        nudTaxRate = new NumericUpDown
        {
            Location = new Point(120, 28),
            Size = new Size(80, 25),
            DecimalPlaces = 2,
            Minimum = 0,
            Maximum = 100,
            Value = 6.00m,
            Font = new Font("Segoe UI", 9)
        };
        nudTaxRate.ValueChanged += (s, e) => { taxRate = nudTaxRate.Value; CalculateTotals(); };
        
        // Discount
        var lblDiscountLabel = new Label
        {
            Text = "Discount (RM):",
            Location = new Point(220, 30),
            Size = new Size(100, 20),
            Font = new Font("Segoe UI", 9)
        };
        nudDiscount = new NumericUpDown
        {
            Location = new Point(325, 28),
            Size = new Size(80, 25),
            DecimalPlaces = 2,
            Minimum = 0,
            Maximum = 10000,
            Value = 0,
            Font = new Font("Segoe UI", 9)
        };
        nudDiscount.ValueChanged += (s, e) => { discountAmount = nudDiscount.Value; CalculateTotals(); };
        
        // Totals
        lblSubTotal = new Label
        {
            Text = "Subtotal: RM 0.00",
            Location = new Point(430, 30),
            Size = new Size(200, 20),
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleRight
        };
        
        lblTax = new Label
        {
            Text = "Tax: RM 0.00",
            Location = new Point(430, 55),
            Size = new Size(200, 20),
            Font = new Font("Segoe UI", 9),
            TextAlign = ContentAlignment.MiddleRight
        };
        
        lblDiscount = new Label
        {
            Text = "Discount: RM 0.00",
            Location = new Point(430, 80),
            Size = new Size(200, 20),
            Font = new Font("Segoe UI", 9),
            TextAlign = ContentAlignment.MiddleRight
        };
        
        lblTotal = new Label
        {
            Text = "TOTAL: RM 0.00",
            Location = new Point(430, 105),
            Size = new Size(200, 25),
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            ForeColor = Color.DarkGreen,
            TextAlign = ContentAlignment.MiddleRight
        };
        
        // Payment Method
        var lblPayment = new Label
        {
            Text = "Payment:",
            Location = new Point(15, 65),
            Size = new Size(100, 20),
            Font = new Font("Segoe UI", 9)
        };
        cboPaymentMethod = new ComboBox
        {
            Location = new Point(120, 63),
            Size = new Size(120, 25),
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 9)
        };
        cboPaymentMethod.Items.AddRange(new object[] { "Cash", "Debit Card", "Credit Card" });
        cboPaymentMethod.SelectedIndex = 0;
        
        // Amount Paid
        var lblAmountPaid = new Label
        {
            Text = "Amount Paid:",
            Location = new Point(15, 100),
            Size = new Size(100, 20),
            Font = new Font("Segoe UI", 9)
        };
        txtAmountPaid = new TextBox
        {
            Location = new Point(120, 98),
            Size = new Size(120, 25),
            Font = new Font("Segoe UI", 9),
            Text = "0.00"
        };
        txtAmountPaid.TextChanged += (s, e) => CalculateChange();
        
        // Change
        lblChange = new Label
        {
            Text = "Change: RM 0.00",
            Location = new Point(15, 130),
            Size = new Size(225, 25),
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            ForeColor = Color.Blue
        };
        
        // Process Payment Button
        var btnProcessPayment = new Button
        {
            Text = "Process Payment",
            Location = new Point(430, 135),
            Size = new Size(200, 40),
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            BackColor = Color.LightGreen
        };
        btnProcessPayment.Click += BtnProcessPayment_Click;
        
        grpPayment.Controls.AddRange(new Control[] {
            lblTaxRate, nudTaxRate, lblDiscountLabel, nudDiscount,
            lblSubTotal, lblTax, lblDiscount, lblTotal,
            lblPayment, cboPaymentMethod, lblAmountPaid, txtAmountPaid,
            lblChange, btnProcessPayment
        });
        panel.Controls.Add(grpPayment);
        
        return panel;
    }
    
    private async void LoadProductsAsync()
    {
        try
        {
            if (!_currentUser.BusinessId.HasValue)
            {
                MessageBox.Show("User is not associated with a business!", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            using var scope = Program.ServiceProvider!.CreateScope();
            var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
            
            allProducts = (await productService.GetAllProductsAsync(_currentUser.BusinessId.Value)).ToList();
            
            productsListView.Items.Clear();
            foreach (var product in allProducts)
            {
                var item = new ListViewItem(product.Name);
                item.SubItems.Add(product.Brand ?? "N/A");
                item.SubItems.Add(product.SellingPrice.ToString("F2"));
                item.SubItems.Add(product.StockQuantity.ToString());
                item.Tag = product;
                productsListView.Items.Add(item);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading products: {ex.Message}", "Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    
    private void ProductsListView_DoubleClick(object? sender, EventArgs e)
    {
        if (productsListView.SelectedItems.Count == 0) return;
        
        var product = productsListView.SelectedItems[0].Tag as Product;
        if (product == null) return;
        
        if (product.StockQuantity <= 0)
        {
            MessageBox.Show("This product is out of stock!", "Out of Stock", 
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        
        AddToCart(product);
    }
    
    private void AddToCart(Product product)
    {
        if (cartItems.ContainsKey(product.ProductId))
        {
            cartItems[product.ProductId].Quantity++;
        }
        else
        {
            cartItems[product.ProductId] = new CartItem
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Brand = product.Brand ?? "N/A",
                UnitPrice = product.SellingPrice,
                Quantity = 1
            };
        }
        
        UpdateCartDisplay();
    }
    
    private void UpdateCartDisplay()
    {
        cartListView.Items.Clear();
        
        foreach (var item in cartItems.Values)
        {
            var listItem = new ListViewItem(item.Name);
            listItem.SubItems.Add(item.Brand);
            listItem.SubItems.Add(item.Quantity.ToString());
            listItem.SubItems.Add(item.UnitPrice.ToString("F2"));
            listItem.SubItems.Add(item.Total.ToString("F2"));
            listItem.Tag = item;
            cartListView.Items.Add(listItem);
        }
        
        CalculateTotals();
    }
    
    private void CalculateTotals()
    {
        decimal subtotal = cartItems.Values.Sum(i => i.Total);
        decimal tax = subtotal * (taxRate / 100);
        decimal total = subtotal + tax - discountAmount;
        
        lblSubTotal.Text = $"Subtotal: RM {subtotal:F2}";
        lblTax.Text = $"Tax ({taxRate}%): RM {tax:F2}";
        lblDiscount.Text = $"Discount: RM {discountAmount:F2}";
        lblTotal.Text = $"TOTAL: RM {total:F2}";
        
        CalculateChange();
    }
    
    private void CalculateChange()
    {
        if (decimal.TryParse(txtAmountPaid.Text, out decimal amountPaid))
        {
            decimal subtotal = cartItems.Values.Sum(i => i.Total);
            decimal tax = subtotal * (taxRate / 100);
            decimal total = subtotal + tax - discountAmount;
            decimal change = amountPaid - total;
            
            lblChange.Text = $"Change: RM {change:F2}";
            lblChange.ForeColor = change >= 0 ? Color.Blue : Color.Red;
        }
        else
        {
            lblChange.Text = "Change: RM 0.00";
        }
    }
    
    private void BtnRemoveItem_Click(object? sender, EventArgs e)
    {
        if (cartListView.SelectedItems.Count == 0) return;
        
        var cartItem = cartListView.SelectedItems[0].Tag as CartItem;
        if (cartItem != null)
        {
            cartItems.Remove(cartItem.ProductId);
            UpdateCartDisplay();
        }
    }
    
    private void BtnIncreaseQty_Click(object? sender, EventArgs e)
    {
        if (cartListView.SelectedItems.Count == 0) return;
        
        var cartItem = cartListView.SelectedItems[0].Tag as CartItem;
        if (cartItem != null)
        {
            var product = allProducts.FirstOrDefault(p => p.ProductId == cartItem.ProductId);
            if (product != null && cartItem.Quantity < product.StockQuantity)
            {
                cartItem.Quantity++;
                UpdateCartDisplay();
            }
            else
            {
                MessageBox.Show("Cannot add more. Stock limit reached!", "Stock Limit", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
    
    private void BtnDecreaseQty_Click(object? sender, EventArgs e)
    {
        if (cartListView.SelectedItems.Count == 0) return;
        
        var cartItem = cartListView.SelectedItems[0].Tag as CartItem;
        if (cartItem != null)
        {
            cartItem.Quantity--;
            if (cartItem.Quantity <= 0)
            {
                cartItems.Remove(cartItem.ProductId);
            }
            UpdateCartDisplay();
        }
    }
    
    private void ClearCart()
    {
        if (cartItems.Count == 0) return;
        
        var result = MessageBox.Show("Are you sure you want to clear the cart?", 
            "Clear Cart", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        
        if (result == DialogResult.Yes)
        {
            cartItems.Clear();
            UpdateCartDisplay();
            txtAmountPaid.Text = "0.00";
        }
    }
    
    private async void BtnProcessPayment_Click(object? sender, EventArgs e)
    {
        if (cartItems.Count == 0)
        {
            MessageBox.Show("Cart is empty! Please add items before checkout.", 
                "Empty Cart", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        
        if (!decimal.TryParse(txtAmountPaid.Text, out decimal amountPaid))
        {
            MessageBox.Show("Please enter a valid amount paid.", 
                "Invalid Amount", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        
        decimal subtotal = cartItems.Values.Sum(i => i.Total);
        decimal tax = subtotal * (taxRate / 100);
        decimal total = subtotal + tax - discountAmount;
        
        if (cboPaymentMethod.SelectedItem?.ToString() == "Cash" && amountPaid < total)
        {
            MessageBox.Show("Amount paid is less than total!", 
                "Insufficient Payment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        
        try
        {
            using var scope = Program.ServiceProvider!.CreateScope();
            var salesService = scope.ServiceProvider.GetRequiredService<ISalesService>();
            
            var transaction = new SalesTransaction
            {
                BusinessId = _currentUser.BusinessId!.Value,
                UserId = _currentUser.UserId,
                SubTotal = subtotal,
                TaxRate = taxRate,
                TaxAmount = tax,
                DiscountAmount = discountAmount,
                TotalAmount = total,
                PaymentMethod = cboPaymentMethod.SelectedItem?.ToString() ?? "Cash",
                AmountPaid = amountPaid,
                ChangeAmount = amountPaid - total,
                Items = cartItems.Values.Select(ci => new SalesTransactionItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.UnitPrice,
                    DiscountAmount = 0,
                    TotalPrice = ci.Total
                }).ToList()
            };
            
            var savedTransaction = await salesService.CreateSalesTransactionAsync(transaction);
            
            // Generate and show receipt
            var receipt = await salesService.GenerateReceiptAsync(savedTransaction.TransactionId);
            MessageBox.Show(receipt, "Transaction Receipt", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            // Clear cart and refresh products
            cartItems.Clear();
            UpdateCartDisplay();
            txtAmountPaid.Text = "0.00";
            nudDiscount.Value = 0;
            LoadProductsAsync();
            
            MessageBox.Show("Payment processed successfully!", 
                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error processing payment: {ex.Message}", 
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private Panel CreateProductsPanel()
    {
        var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
        
        // Title
        var lblTitle = new Label
        {
            Text = "Product Management",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            Location = new Point(10, 10),
            Size = new Size(400, 35)
        };
        panel.Controls.Add(lblTitle);
        
        // Products ListView
        var productsManagementListView = new ListView
        {
            Location = new Point(10, 50),
            Size = new Size(1000, 500),
            View = View.Details,
            FullRowSelect = true,
            GridLines = true,
            Font = new Font("Segoe UI", 9)
        };
        
        productsManagementListView.Columns.Add("ID", 50);
        productsManagementListView.Columns.Add("Name", 180);
        productsManagementListView.Columns.Add("Brand", 120);
        productsManagementListView.Columns.Add("Category", 100);
        productsManagementListView.Columns.Add("Price (RM)", 80);
        productsManagementListView.Columns.Add("Stock", 70);
        productsManagementListView.Columns.Add("Barcode", 100);
        productsManagementListView.Columns.Add("SKU", 80);
        productsManagementListView.Columns.Add("Unit", 70);
        
        // Load products into management view
        LoadProductsManagement(productsManagementListView);
        
        // Buttons
        var btnAddProduct = new Button
        {
            Text = "Add New Product",
            Location = new Point(10, 560),
            Size = new Size(150, 40),
            Font = new Font("Segoe UI", 10),
            BackColor = Color.LightGreen
        };
        btnAddProduct.Click += (s, e) => ShowProductDialog(null, productsManagementListView);
        
        var btnEditProduct = new Button
        {
            Text = "Edit Product",
            Location = new Point(170, 560),
            Size = new Size(150, 40),
            Font = new Font("Segoe UI", 10),
            BackColor = Color.LightBlue
        };
        btnEditProduct.Click += (s, e) =>
        {
            if (productsManagementListView.SelectedItems.Count > 0)
            {
                var product = productsManagementListView.SelectedItems[0].Tag as Product;
                ShowProductDialog(product, productsManagementListView);
            }
            else
            {
                MessageBox.Show("Please select a product to edit.", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        };
        
        var btnDeleteProduct = new Button
        {
            Text = "Delete Product",
            Location = new Point(330, 560),
            Size = new Size(150, 40),
            Font = new Font("Segoe UI", 10),
            BackColor = Color.LightCoral
        };
        btnDeleteProduct.Click += async (s, e) =>
        {
            if (productsManagementListView.SelectedItems.Count > 0)
            {
                var product = productsManagementListView.SelectedItems[0].Tag as Product;
                if (product != null)
                {
                    var result = MessageBox.Show(
                        $"Are you sure you want to delete '{product.Name}'?",
                        "Confirm Delete",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);
                    
                    if (result == DialogResult.Yes)
                    {
                        await DeleteProduct(product.ProductId);
                        LoadProductsManagement(productsManagementListView);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a product to delete.", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        };
        
        var btnRefresh = new Button
        {
            Text = "Refresh List",
            Location = new Point(490, 560),
            Size = new Size(150, 40),
            Font = new Font("Segoe UI", 10)
        };
        btnRefresh.Click += (s, e) => LoadProductsManagement(productsManagementListView);
        
        // Info label
        var lblInfo = new Label
        {
            Text = "Double-click a product to edit it. Products can be managed here and will appear in the POS system.",
            Location = new Point(10, 610),
            Size = new Size(800, 40),
            Font = new Font("Segoe UI", 9, FontStyle.Italic),
            ForeColor = Color.Gray
        };
        
        productsManagementListView.DoubleClick += (s, e) =>
        {
            if (productsManagementListView.SelectedItems.Count > 0)
            {
                var product = productsManagementListView.SelectedItems[0].Tag as Product;
                ShowProductDialog(product, productsManagementListView);
            }
        };
        
        panel.Controls.AddRange(new Control[] {
            productsManagementListView, btnAddProduct, btnEditProduct, 
            btnDeleteProduct, btnRefresh, lblInfo
        });
        
        return panel;
    }
    
    private async void LoadProductsManagement(ListView listView)
    {
        try
        {
            if (!_currentUser.BusinessId.HasValue)
            {
                MessageBox.Show("User is not associated with a business!", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            using var scope = Program.ServiceProvider!.CreateScope();
            var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
            
            var products = await productService.GetAllProductsAsync(_currentUser.BusinessId.Value);
            
            listView.Items.Clear();
            foreach (var product in products)
            {
                var item = new ListViewItem(product.ProductId.ToString());
                item.SubItems.Add(product.Name);
                item.SubItems.Add(product.Brand ?? "N/A");
                item.SubItems.Add(product.Category ?? "N/A");
                item.SubItems.Add(product.SellingPrice.ToString("F2"));
                item.SubItems.Add(product.StockQuantity.ToString());
                item.SubItems.Add(product.Barcode ?? "N/A");
                item.SubItems.Add(product.SKU ?? "N/A");
                item.SubItems.Add(product.Unit ?? "pcs");
                item.Tag = product;
                listView.Items.Add(item);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading products: {ex.Message}", "Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    
    private void ShowProductDialog(Product? product, ListView listView)
    {
        var isEdit = product != null;
        var form = new Form
        {
            Text = isEdit ? "Edit Product" : "Add New Product",
            Size = new Size(500, 600),
            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false
        };
        
        var y = 20;
        var labelWidth = 120;
        var fieldWidth = 300;
        
        // Name
        form.Controls.Add(new Label { Text = "Name:", Location = new Point(20, y), Size = new Size(labelWidth, 20) });
        var txtName = new TextBox { Location = new Point(150, y), Size = new Size(fieldWidth, 25), Text = product?.Name ?? "" };
        form.Controls.Add(txtName);
        y += 35;
        
        // Brand
        form.Controls.Add(new Label { Text = "Brand:", Location = new Point(20, y), Size = new Size(labelWidth, 20) });
        var txtBrand = new TextBox { Location = new Point(150, y), Size = new Size(fieldWidth, 25), Text = product?.Brand ?? "" };
        form.Controls.Add(txtBrand);
        y += 35;
        
        // Description
        form.Controls.Add(new Label { Text = "Description:", Location = new Point(20, y), Size = new Size(labelWidth, 20) });
        var txtDescription = new TextBox { Location = new Point(150, y), Size = new Size(fieldWidth, 50), Multiline = true, Text = product?.Description ?? "" };
        form.Controls.Add(txtDescription);
        y += 65;
        
        // Category
        form.Controls.Add(new Label { Text = "Category:", Location = new Point(20, y), Size = new Size(labelWidth, 20) });
        var txtCategory = new TextBox { Location = new Point(150, y), Size = new Size(fieldWidth, 25), Text = product?.Category ?? "" };
        form.Controls.Add(txtCategory);
        y += 35;
        
        // Barcode
        form.Controls.Add(new Label { Text = "Barcode:", Location = new Point(20, y), Size = new Size(labelWidth, 20) });
        var txtBarcode = new TextBox { Location = new Point(150, y), Size = new Size(fieldWidth, 25), Text = product?.Barcode ?? "" };
        form.Controls.Add(txtBarcode);
        y += 35;
        
        // SKU
        form.Controls.Add(new Label { Text = "SKU:", Location = new Point(20, y), Size = new Size(labelWidth, 20) });
        var txtSKU = new TextBox { Location = new Point(150, y), Size = new Size(fieldWidth, 25), Text = product?.SKU ?? "" };
        form.Controls.Add(txtSKU);
        y += 35;
        
        // Cost Price
        form.Controls.Add(new Label { Text = "Cost Price (RM):", Location = new Point(20, y), Size = new Size(labelWidth, 20) });
        var nudCostPrice = new NumericUpDown { Location = new Point(150, y), Size = new Size(140, 25), DecimalPlaces = 2, Maximum = 100000, Value = product?.CostPrice ?? 0 };
        form.Controls.Add(nudCostPrice);
        y += 35;
        
        // Selling Price
        form.Controls.Add(new Label { Text = "Selling Price (RM):", Location = new Point(20, y), Size = new Size(labelWidth, 20) });
        var nudSellingPrice = new NumericUpDown { Location = new Point(150, y), Size = new Size(140, 25), DecimalPlaces = 2, Maximum = 100000, Value = product?.SellingPrice ?? 0 };
        form.Controls.Add(nudSellingPrice);
        y += 35;
        
        // Stock Quantity
        form.Controls.Add(new Label { Text = "Stock Quantity:", Location = new Point(20, y), Size = new Size(labelWidth, 20) });
        var nudStock = new NumericUpDown { Location = new Point(150, y), Size = new Size(140, 25), Maximum = 100000, Value = product?.StockQuantity ?? 0 };
        form.Controls.Add(nudStock);
        y += 35;
        
        // Unit
        form.Controls.Add(new Label { Text = "Unit:", Location = new Point(20, y), Size = new Size(labelWidth, 20) });
        var txtUnit = new TextBox { Location = new Point(150, y), Size = new Size(140, 25), Text = product?.Unit ?? "pcs" };
        form.Controls.Add(txtUnit);
        y += 45;
        
        // Buttons
        var btnSave = new Button
        {
            Text = isEdit ? "Update" : "Add",
            Location = new Point(150, y),
            Size = new Size(100, 35),
            BackColor = Color.LightGreen
        };
        btnSave.Click += async (s, e) =>
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Product name is required!", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            try
            {
                var productData = new Product
                {
                    ProductId = product?.ProductId ?? 0,
                    Name = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(txtName.Text.ToLower()),
                    Brand = txtBrand.Text,
                    Description = txtDescription.Text,
                    Category = txtCategory.Text,
                    Barcode = txtBarcode.Text,
                    SKU = txtSKU.Text,
                    CostPrice = nudCostPrice.Value,
                    SellingPrice = nudSellingPrice.Value,
                    StockQuantity = (int)nudStock.Value,
                    Unit = txtUnit.Text,
                    BusinessId = _currentUser.BusinessId!.Value,
                    MinimumStockLevel = product?.MinimumStockLevel ?? 10,
                    ReorderLevel = product?.ReorderLevel ?? 20,
                    IsActive = product?.IsActive ?? true
                };
                
                using var scope = Program.ServiceProvider!.CreateScope();
                var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
                
                if (isEdit)
                {
                    await productService.UpdateProductAsync(productData);
                    MessageBox.Show("Product updated successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    await productService.AddProductAsync(productData);
                    MessageBox.Show("Product added successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                
                LoadProductsManagement(listView);
                form.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving product: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        };
        
        var btnCancel = new Button
        {
            Text = "Cancel",
            Location = new Point(260, y),
            Size = new Size(100, 35)
        };
        btnCancel.Click += (s, e) => form.Close();
        
        form.Controls.Add(btnSave);
        form.Controls.Add(btnCancel);
        
        form.ShowDialog();
    }
    
    private async Task DeleteProduct(int productId)
    {
        try
        {
            using var scope = Program.ServiceProvider!.CreateScope();
            var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
            
            await productService.DeleteProductAsync(productId);
            MessageBox.Show("Product deleted successfully!", "Success", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error deleting product: {ex.Message}", "Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private Panel CreateSalesPanel()
    {
        var panel = new Panel();
        var label = new Label
        {
            Text = "Sales History - Coming Soon\n\nThis will include:\n- View all transactions\n- Search by date/customer\n- Void transactions\n- Reprint receipts",
            Font = new Font("Segoe UI", 12),
            Location = new Point(50, 50),
            Size = new Size(600, 200)
        };
        panel.Controls.Add(label);
        return panel;
    }

    private Panel CreateRestockingPanel()
    {
        var panel = new Panel();
        var label = new Label
        {
            Text = "Restocking Management - Coming Soon\n\nThis will include:\n- View restocking history\n- AI-powered restocking suggestions\n- Manual restocking\n- Stock level tracking",
            Font = new Font("Segoe UI", 12),
            Location = new Point(50, 50),
            Size = new Size(600, 200)
        };
        panel.Controls.Add(label);
        return panel;
    }

    private Panel CreatePurchaseOrdersPanel()
    {
        var panel = new Panel();
        var label = new Label
        {
            Text = "Purchase Orders - Coming Soon\n\nThis will include:\n- Create purchase orders\n- Track supplier orders\n- Receive inventory\n- Order history",
            Font = new Font("Segoe UI", 12),
            Location = new Point(50, 50),
            Size = new Size(600, 200)
        };
        panel.Controls.Add(label);
        return panel;
    }

    private Panel CreateReportsPanel()
    {
        var panel = new Panel();
        var label = new Label
        {
            Text = "Business Reports - Coming Soon\n\nThis will include:\n- Sales reports\n- Inventory reports\n- Profit/Loss analysis\n- Best-selling products\n- Export to PDF/Excel",
            Font = new Font("Segoe UI", 12),
            Location = new Point(50, 50),
            Size = new Size(600, 200)
        };
        panel.Controls.Add(label);
        return panel;
    }
}
