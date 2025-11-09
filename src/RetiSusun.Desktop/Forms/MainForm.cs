using RetiSusun.Data.Models;
using RetiSusun.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using RetiSusun.Data;

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
        var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
        
        // Title
        var lblTitle = new Label
        {
            Text = "Sales History",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            Location = new Point(10, 10),
            Size = new Size(400, 35)
        };
        panel.Controls.Add(lblTitle);
        
        // Date Filter Section
        var grpFilter = new GroupBox
        {
            Text = "Filter by Date",
            Location = new Point(10, 50),
            Size = new Size(1000, 70),
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        
        var lblFromDate = new Label
        {
            Text = "From:",
            Location = new Point(15, 30),
            Size = new Size(50, 20),
            Font = new Font("Segoe UI", 9)
        };
        var dtpFromDate = new DateTimePicker
        {
            Location = new Point(70, 28),
            Size = new Size(200, 25),
            Format = DateTimePickerFormat.Short,
            Value = DateTime.Today.AddDays(-30)
        };
        
        var lblToDate = new Label
        {
            Text = "To:",
            Location = new Point(290, 30),
            Size = new Size(30, 20),
            Font = new Font("Segoe UI", 9)
        };
        var dtpToDate = new DateTimePicker
        {
            Location = new Point(325, 28),
            Size = new Size(200, 25),
            Format = DateTimePickerFormat.Short,
            Value = DateTime.Today
        };
        
        var btnFilter = new Button
        {
            Text = "Apply Filter",
            Location = new Point(540, 25),
            Size = new Size(120, 30),
            Font = new Font("Segoe UI", 9)
        };
        
        var btnShowAll = new Button
        {
            Text = "Show All",
            Location = new Point(670, 25),
            Size = new Size(100, 30),
            Font = new Font("Segoe UI", 9)
        };
        
        grpFilter.Controls.AddRange(new Control[] {
            lblFromDate, dtpFromDate, lblToDate, dtpToDate, btnFilter, btnShowAll
        });
        panel.Controls.Add(grpFilter);
        
        // Sales ListView
        var salesListView = new ListView
        {
            Location = new Point(10, 130),
            Size = new Size(1000, 420),
            View = View.Details,
            FullRowSelect = true,
            GridLines = true,
            Font = new Font("Segoe UI", 9)
        };
        
        salesListView.Columns.Add("Date", 150);
        salesListView.Columns.Add("Transaction #", 150);
        salesListView.Columns.Add("Cashier", 120);
        salesListView.Columns.Add("Items", 60);
        salesListView.Columns.Add("Subtotal", 80);
        salesListView.Columns.Add("Tax", 70);
        salesListView.Columns.Add("Discount", 70);
        salesListView.Columns.Add("Total", 90);
        salesListView.Columns.Add("Payment", 100);
        salesListView.Columns.Add("Status", 80);
        
        panel.Controls.Add(salesListView);
        
        // Buttons
        var btnViewDetails = new Button
        {
            Text = "View Details",
            Location = new Point(10, 560),
            Size = new Size(120, 35),
            Font = new Font("Segoe UI", 9),
            BackColor = Color.LightBlue
        };
        btnViewDetails.Click += (s, e) =>
        {
            if (salesListView.SelectedItems.Count > 0)
            {
                var transaction = salesListView.SelectedItems[0].Tag as SalesTransaction;
                if (transaction != null)
                {
                    ShowSalesTransactionDetails(transaction.TransactionId);
                }
            }
            else
            {
                MessageBox.Show("Please select a transaction to view details.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        };
        
        var btnReprintReceipt = new Button
        {
            Text = "Reprint Receipt",
            Location = new Point(140, 560),
            Size = new Size(130, 35),
            Font = new Font("Segoe UI", 9),
            BackColor = Color.LightGreen
        };
        btnReprintReceipt.Click += async (s, e) =>
        {
            if (salesListView.SelectedItems.Count > 0)
            {
                var transaction = salesListView.SelectedItems[0].Tag as SalesTransaction;
                if (transaction != null)
                {
                    await ReprintReceipt(transaction.TransactionId);
                }
            }
            else
            {
                MessageBox.Show("Please select a transaction to reprint receipt.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        };
        
        var btnRefreshSales = new Button
        {
            Text = "Refresh",
            Location = new Point(280, 560),
            Size = new Size(100, 35),
            Font = new Font("Segoe UI", 9)
        };
        
        // Summary Labels
        var lblTotalTransactions = new Label
        {
            Text = "Total Transactions: 0",
            Location = new Point(450, 565),
            Size = new Size(200, 25),
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleRight
        };
        
        var lblTotalSales = new Label
        {
            Text = "Total Sales: RM 0.00",
            Location = new Point(660, 565),
            Size = new Size(200, 25),
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.DarkGreen,
            TextAlign = ContentAlignment.MiddleRight
        };
        
        panel.Controls.AddRange(new Control[] {
            btnViewDetails, btnReprintReceipt, btnRefreshSales,
            lblTotalTransactions, lblTotalSales
        });
        
        // Load initial data
        LoadSalesHistory(salesListView, lblTotalTransactions, lblTotalSales, null, null);
        
        // Wire up event handlers
        btnFilter.Click += (s, e) => LoadSalesHistory(salesListView, lblTotalTransactions, lblTotalSales, 
            dtpFromDate.Value.Date, dtpToDate.Value.Date.AddDays(1).AddSeconds(-1));
        
        btnShowAll.Click += (s, e) => LoadSalesHistory(salesListView, lblTotalTransactions, lblTotalSales, null, null);
        
        btnRefreshSales.Click += (s, e) => LoadSalesHistory(salesListView, lblTotalTransactions, lblTotalSales, 
            dtpFromDate.Value.Date, dtpToDate.Value.Date.AddDays(1).AddSeconds(-1));
        
        salesListView.DoubleClick += (s, e) =>
        {
            if (salesListView.SelectedItems.Count > 0)
            {
                var transaction = salesListView.SelectedItems[0].Tag as SalesTransaction;
                if (transaction != null)
                {
                    ShowSalesTransactionDetails(transaction.TransactionId);
                }
            }
        };
        
        return panel;
    }
    
    private async void LoadSalesHistory(ListView listView, Label lblCount, Label lblTotal, 
        DateTime? startDate, DateTime? endDate)
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
            var salesService = scope.ServiceProvider.GetRequiredService<ISalesService>();
            
            var transactions = await salesService.GetSalesTransactionsAsync(
                _currentUser.BusinessId.Value, startDate, endDate);
            
            listView.Items.Clear();
            
            decimal totalSales = 0;
            int totalCount = 0;
            
            // Group by date
            var groupedTransactions = transactions
                .GroupBy(t => t.TransactionDate.Date)
                .OrderByDescending(g => g.Key);
            
            foreach (var dateGroup in groupedTransactions)
            {
                foreach (var transaction in dateGroup.OrderByDescending(t => t.TransactionDate))
                {
                    var item = new ListViewItem(transaction.TransactionDate.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                    item.SubItems.Add(transaction.TransactionNumber);
                    item.SubItems.Add(transaction.User?.FullName ?? "N/A");
                    item.SubItems.Add(transaction.Items.Count.ToString());
                    item.SubItems.Add(transaction.SubTotal.ToString("F2"));
                    item.SubItems.Add(transaction.TaxAmount.ToString("F2"));
                    item.SubItems.Add(transaction.DiscountAmount.ToString("F2"));
                    item.SubItems.Add(transaction.TotalAmount.ToString("F2"));
                    item.SubItems.Add(transaction.PaymentMethod);
                    item.SubItems.Add(transaction.IsVoided ? "Voided" : "Completed");
                    
                    if (transaction.IsVoided)
                    {
                        item.ForeColor = Color.Red;
                    }
                    
                    item.Tag = transaction;
                    listView.Items.Add(item);
                    
                    if (!transaction.IsVoided)
                    {
                        totalSales += transaction.TotalAmount;
                        totalCount++;
                    }
                }
            }
            
            lblCount.Text = $"Total Transactions: {totalCount}";
            lblTotal.Text = $"Total Sales: RM {totalSales:F2}";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading sales history: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    
    private async void ShowSalesTransactionDetails(int transactionId)
    {
        try
        {
            using var scope = Program.ServiceProvider!.CreateScope();
            var salesService = scope.ServiceProvider.GetRequiredService<ISalesService>();
            
            var transaction = await salesService.GetSalesTransactionByIdAsync(transactionId);
            if (transaction == null)
            {
                MessageBox.Show("Transaction not found!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            var form = new Form
            {
                Text = $"Transaction Details - {transaction.TransactionNumber}",
                Size = new Size(600, 500),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.Sizable
            };
            
            var txtDetails = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true,
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 10),
                Padding = new Padding(10)
            };
            
            var details = new StringBuilder();
            details.AppendLine($"Transaction Number: {transaction.TransactionNumber}");
            details.AppendLine($"Date: {transaction.TransactionDate.ToLocalTime():yyyy-MM-dd HH:mm:ss}");
            details.AppendLine($"Cashier: {transaction.User?.FullName ?? "N/A"}");
            details.AppendLine($"Status: {(transaction.IsVoided ? "VOIDED" : "COMPLETED")}");
            if (transaction.IsVoided)
            {
                details.AppendLine($"Voided Date: {transaction.VoidedDate?.ToLocalTime():yyyy-MM-dd HH:mm:ss}");
                details.AppendLine($"Void Reason: {transaction.VoidReason}");
            }
            details.AppendLine();
            details.AppendLine("=== ITEMS ===");
            
            foreach (var item in transaction.Items)
            {
                details.AppendLine($"{item.Product?.Name ?? "N/A"}");
                details.AppendLine($"  Qty: {item.Quantity} x RM {item.UnitPrice:F2} = RM {item.TotalPrice:F2}");
            }
            
            details.AppendLine();
            details.AppendLine("=== PAYMENT ===");
            details.AppendLine($"Subtotal:       RM {transaction.SubTotal:F2}");
            details.AppendLine($"Tax ({transaction.TaxRate}%):     RM {transaction.TaxAmount:F2}");
            details.AppendLine($"Discount:       RM {transaction.DiscountAmount:F2}");
            details.AppendLine($"Total:          RM {transaction.TotalAmount:F2}");
            details.AppendLine($"Payment Method: {transaction.PaymentMethod}");
            details.AppendLine($"Amount Paid:    RM {transaction.AmountPaid:F2}");
            details.AppendLine($"Change:         RM {transaction.ChangeAmount:F2}");
            
            if (!string.IsNullOrEmpty(transaction.Notes))
            {
                details.AppendLine();
                details.AppendLine($"Notes: {transaction.Notes}");
            }
            
            txtDetails.Text = details.ToString();
            form.Controls.Add(txtDetails);
            
            form.ShowDialog();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error showing transaction details: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    
    private async Task ReprintReceipt(int transactionId)
    {
        try
        {
            using var scope = Program.ServiceProvider!.CreateScope();
            var salesService = scope.ServiceProvider.GetRequiredService<ISalesService>();
            
            var receipt = await salesService.GenerateReceiptAsync(transactionId);
            MessageBox.Show(receipt, "Receipt Reprint",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error reprinting receipt: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private Panel CreateRestockingPanel()
    {
        var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
        
        // Title
        var lblTitle = new Label
        {
            Text = "Restocking Management",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            Location = new Point(10, 10),
            Size = new Size(400, 35)
        };
        panel.Controls.Add(lblTitle);
        
        // Tab Control for different views
        var restockTabControl = new TabControl
        {
            Location = new Point(10, 50),
            Size = new Size(1000, 600)
        };
        
        // Products Catalog Tab
        var catalogTab = new TabPage("Products Catalog");
        var catalogPanel = CreateRestockingCatalogPanel();
        catalogPanel.Dock = DockStyle.Fill;
        catalogTab.Controls.Add(catalogPanel);
        restockTabControl.TabPages.Add(catalogTab);
        
        // Restocking Suggestions Tab
        var suggestionsTab = new TabPage("AI Suggestions");
        var suggestionsPanel = CreateRestockingSuggestionsPanel();
        suggestionsPanel.Dock = DockStyle.Fill;
        suggestionsTab.Controls.Add(suggestionsPanel);
        restockTabControl.TabPages.Add(suggestionsTab);
        
        // Restocking History Tab
        var historyTab = new TabPage("Restocking History");
        var historyPanel = CreateRestockingHistoryPanel();
        historyPanel.Dock = DockStyle.Fill;
        historyTab.Controls.Add(historyPanel);
        restockTabControl.TabPages.Add(historyTab);
        
        panel.Controls.Add(restockTabControl);
        
        return panel;
    }
    
    private Panel CreateRestockingCatalogPanel()
    {
        var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
        
        var lblInfo = new Label
        {
            Text = "Product Catalog for Restocking - View all products with pricing and stock information",
            Font = new Font("Segoe UI", 10, FontStyle.Italic),
            Location = new Point(10, 10),
            Size = new Size(900, 25),
            ForeColor = Color.Gray
        };
        panel.Controls.Add(lblInfo);
        
        var catalogListView = new ListView
        {
            Location = new Point(10, 40),
            Size = new Size(950, 420),
            View = View.Details,
            FullRowSelect = true,
            GridLines = true,
            Font = new Font("Segoe UI", 9)
        };
        
        catalogListView.Columns.Add("Product Name", 180);
        catalogListView.Columns.Add("Brand", 100);
        catalogListView.Columns.Add("Category", 100);
        catalogListView.Columns.Add("Current Stock", 90);
        catalogListView.Columns.Add("Unit", 60);
        catalogListView.Columns.Add("Cost Price", 90);
        catalogListView.Columns.Add("Selling Price", 90);
        catalogListView.Columns.Add("Reorder Level", 90);
        catalogListView.Columns.Add("Status", 90);
        
        panel.Controls.Add(catalogListView);
        
        // Manual Restock Button
        var btnManualRestock = new Button
        {
            Text = "Manual Restock",
            Location = new Point(10, 470),
            Size = new Size(150, 35),
            Font = new Font("Segoe UI", 10),
            BackColor = Color.LightGreen
        };
        btnManualRestock.Click += (s, e) =>
        {
            if (catalogListView.SelectedItems.Count > 0)
            {
                var product = catalogListView.SelectedItems[0].Tag as Product;
                if (product != null)
                {
                    ShowManualRestockDialog(product, catalogListView);
                }
            }
            else
            {
                MessageBox.Show("Please select a product to restock.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        };
        
        var btnRefresh = new Button
        {
            Text = "Refresh",
            Location = new Point(170, 470),
            Size = new Size(100, 35),
            Font = new Font("Segoe UI", 10)
        };
        btnRefresh.Click += (s, e) => LoadRestockingCatalog(catalogListView);
        
        panel.Controls.AddRange(new Control[] { btnManualRestock, btnRefresh });
        
        // Load initial data
        LoadRestockingCatalog(catalogListView);
        
        return panel;
    }
    
    private async void LoadRestockingCatalog(ListView listView)
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
            
            foreach (var product in products.OrderBy(p => p.Name))
            {
                var item = new ListViewItem(product.Name);
                item.SubItems.Add(product.Brand ?? "N/A");
                item.SubItems.Add(product.Category ?? "N/A");
                item.SubItems.Add(product.StockQuantity.ToString());
                item.SubItems.Add(product.Unit ?? "pcs");
                item.SubItems.Add($"RM {product.CostPrice:F2}");
                item.SubItems.Add($"RM {product.SellingPrice:F2}");
                item.SubItems.Add(product.ReorderLevel.ToString());
                
                string status;
                if (product.StockQuantity <= product.MinimumStockLevel)
                {
                    status = "Critical";
                    item.BackColor = Color.LightCoral;
                }
                else if (product.StockQuantity <= product.ReorderLevel)
                {
                    status = "Low Stock";
                    item.BackColor = Color.LightYellow;
                }
                else
                {
                    status = "In Stock";
                }
                
                item.SubItems.Add(status);
                item.Tag = product;
                listView.Items.Add(item);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading catalog: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    
    private void ShowManualRestockDialog(Product product, ListView catalogListView)
    {
        var form = new Form
        {
            Text = $"Manual Restock - {product.Name}",
            Size = new Size(450, 400),
            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false
        };
        
        var y = 20;
        
        // Product Info
        form.Controls.Add(new Label { 
            Text = $"Product: {product.Name}", 
            Location = new Point(20, y), 
            Size = new Size(400, 25),
            Font = new Font("Segoe UI", 11, FontStyle.Bold)
        });
        y += 35;
        
        form.Controls.Add(new Label { 
            Text = $"Current Stock: {product.StockQuantity} {product.Unit}", 
            Location = new Point(20, y), 
            Size = new Size(300, 20),
            Font = new Font("Segoe UI", 9)
        });
        y += 30;
        
        form.Controls.Add(new Label { 
            Text = $"Reorder Level: {product.ReorderLevel} {product.Unit}", 
            Location = new Point(20, y), 
            Size = new Size(300, 20),
            Font = new Font("Segoe UI", 9)
        });
        y += 40;
        
        // Quantity to Add
        form.Controls.Add(new Label { 
            Text = "Quantity to Add:", 
            Location = new Point(20, y), 
            Size = new Size(150, 20) 
        });
        var nudQuantity = new NumericUpDown { 
            Location = new Point(180, y - 2), 
            Size = new Size(150, 25), 
            Maximum = 100000,
            Value = product.ReorderLevel
        };
        form.Controls.Add(nudQuantity);
        y += 35;
        
        // Unit Cost
        form.Controls.Add(new Label { 
            Text = "Unit Cost (RM):", 
            Location = new Point(20, y), 
            Size = new Size(150, 20) 
        });
        var nudUnitCost = new NumericUpDown { 
            Location = new Point(180, y - 2), 
            Size = new Size(150, 25), 
            DecimalPlaces = 2,
            Maximum = 100000,
            Value = product.CostPrice
        };
        form.Controls.Add(nudUnitCost);
        y += 35;
        
        // Total Cost (calculated)
        var lblTotalCost = new Label { 
            Text = $"Total Cost: RM {(product.CostPrice * product.ReorderLevel):F2}", 
            Location = new Point(20, y), 
            Size = new Size(300, 25),
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.DarkGreen
        };
        form.Controls.Add(lblTotalCost);
        
        nudQuantity.ValueChanged += (s, e) => 
            lblTotalCost.Text = $"Total Cost: RM {(nudUnitCost.Value * nudQuantity.Value):F2}";
        nudUnitCost.ValueChanged += (s, e) => 
            lblTotalCost.Text = $"Total Cost: RM {(nudUnitCost.Value * nudQuantity.Value):F2}";
        
        y += 40;
        
        // Notes
        form.Controls.Add(new Label { 
            Text = "Notes:", 
            Location = new Point(20, y), 
            Size = new Size(150, 20) 
        });
        y += 25;
        var txtNotes = new TextBox { 
            Location = new Point(20, y), 
            Size = new Size(390, 60), 
            Multiline = true 
        };
        form.Controls.Add(txtNotes);
        y += 75;
        
        // Buttons
        var btnSave = new Button
        {
            Text = "Restock",
            Location = new Point(130, y),
            Size = new Size(100, 35),
            BackColor = Color.LightGreen
        };
        btnSave.Click += async (s, e) =>
        {
            if (nudQuantity.Value <= 0)
            {
                MessageBox.Show("Please enter a valid quantity!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            try
            {
                using var scope = Program.ServiceProvider!.CreateScope();
                var restockingService = scope.ServiceProvider.GetRequiredService<IRestockingService>();
                
                var record = new RestockingRecord
                {
                    ProductId = product.ProductId,
                    QuantityAdded = (int)nudQuantity.Value,
                    UnitCost = nudUnitCost.Value,
                    TotalCost = nudUnitCost.Value * nudQuantity.Value,
                    RestockedByUserId = _currentUser.UserId,
                    Source = "Manual",
                    Notes = txtNotes.Text
                };
                
                await restockingService.AddRestockingRecordAsync(record);
                
                MessageBox.Show($"Successfully restocked {nudQuantity.Value} {product.Unit} of {product.Name}!",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                LoadRestockingCatalog(catalogListView);
                form.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding restock record: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        };
        
        var btnCancel = new Button
        {
            Text = "Cancel",
            Location = new Point(240, y),
            Size = new Size(100, 35)
        };
        btnCancel.Click += (s, e) => form.Close();
        
        form.Controls.Add(btnSave);
        form.Controls.Add(btnCancel);
        
        form.ShowDialog();
    }
    
    private Panel CreateRestockingSuggestionsPanel()
    {
        var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
        
        var lblInfo = new Label
        {
            Text = "AI-Powered Restocking Suggestions based on sales trends",
            Font = new Font("Segoe UI", 10, FontStyle.Italic),
            Location = new Point(10, 10),
            Size = new Size(900, 25),
            ForeColor = Color.Gray
        };
        panel.Controls.Add(lblInfo);
        
        var suggestionsListView = new ListView
        {
            Location = new Point(10, 40),
            Size = new Size(950, 420),
            View = View.Details,
            FullRowSelect = true,
            GridLines = true,
            Font = new Font("Segoe UI", 9)
        };
        
        suggestionsListView.Columns.Add("Product Name", 200);
        suggestionsListView.Columns.Add("Current Stock", 100);
        suggestionsListView.Columns.Add("Reorder Level", 100);
        suggestionsListView.Columns.Add("Suggested Qty", 100);
        suggestionsListView.Columns.Add("Est. Cost", 100);
        suggestionsListView.Columns.Add("Priority", 100);
        suggestionsListView.Columns.Add("Reason", 200);
        
        panel.Controls.Add(suggestionsListView);
        
        var btnGenerateSuggestions = new Button
        {
            Text = "Generate Suggestions",
            Location = new Point(10, 470),
            Size = new Size(180, 35),
            Font = new Font("Segoe UI", 10),
            BackColor = Color.LightBlue
        };
        btnGenerateSuggestions.Click += (s, e) => LoadRestockingSuggestions(suggestionsListView);
        
        var btnApplySuggestion = new Button
        {
            Text = "Apply Selected",
            Location = new Point(200, 470),
            Size = new Size(150, 35),
            Font = new Font("Segoe UI", 10),
            BackColor = Color.LightGreen
        };
        btnApplySuggestion.Click += async (s, e) =>
        {
            if (suggestionsListView.SelectedItems.Count > 0)
            {
                var suggestionData = suggestionsListView.SelectedItems[0].Tag as dynamic;
                if (suggestionData != null)
                {
                    await ApplyRestockingSuggestion(
                        suggestionData.ProductId, 
                        suggestionData.SuggestedQuantity,
                        suggestionsListView);
                }
            }
            else
            {
                MessageBox.Show("Please select a suggestion to apply.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        };
        
        panel.Controls.AddRange(new Control[] { btnGenerateSuggestions, btnApplySuggestion });
        
        // Load initial suggestions
        LoadRestockingSuggestions(suggestionsListView);
        
        return panel;
    }
    
    private async void LoadRestockingSuggestions(ListView listView)
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
            var restockingService = scope.ServiceProvider.GetRequiredService<IRestockingService>();
            var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
            
            var suggestions = await restockingService.GetRestockingSuggestionsAsync(_currentUser.BusinessId.Value);
            
            listView.Items.Clear();
            
            if (!suggestions.Any())
            {
                MessageBox.Show("No restocking suggestions at this time. All products are adequately stocked!",
                    "No Suggestions", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            foreach (var suggestion in suggestions.OrderByDescending(s => s.Value))
            {
                var product = await productService.GetProductByIdAsync(suggestion.Key);
                if (product == null) continue;
                
                var item = new ListViewItem(product.Name);
                item.SubItems.Add(product.StockQuantity.ToString());
                item.SubItems.Add(product.ReorderLevel.ToString());
                item.SubItems.Add(suggestion.Value.ToString());
                item.SubItems.Add($"RM {(product.CostPrice * suggestion.Value):F2}");
                
                string priority;
                if (product.StockQuantity <= product.MinimumStockLevel)
                {
                    priority = "High";
                    item.BackColor = Color.LightCoral;
                }
                else if (product.StockQuantity <= product.ReorderLevel)
                {
                    priority = "Medium";
                    item.BackColor = Color.LightYellow;
                }
                else
                {
                    priority = "Low";
                }
                item.SubItems.Add(priority);
                
                string reason = product.StockQuantity <= product.MinimumStockLevel 
                    ? "Critical stock level" 
                    : "Below reorder level";
                item.SubItems.Add(reason);
                
                item.Tag = new { ProductId = product.ProductId, SuggestedQuantity = suggestion.Value };
                listView.Items.Add(item);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading suggestions: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    
    private async Task ApplyRestockingSuggestion(int productId, int suggestedQuantity, ListView suggestionsListView)
    {
        try
        {
            var result = MessageBox.Show(
                $"Apply restocking suggestion for {suggestedQuantity} units?",
                "Confirm Restock",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            
            if (result != DialogResult.Yes)
                return;
            
            using var scope = Program.ServiceProvider!.CreateScope();
            var restockingService = scope.ServiceProvider.GetRequiredService<IRestockingService>();
            
            var success = await restockingService.ApplyRestockingSuggestionAsync(
                productId, suggestedQuantity, _currentUser.UserId);
            
            if (success)
            {
                MessageBox.Show("Restocking suggestion applied successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadRestockingSuggestions(suggestionsListView);
            }
            else
            {
                MessageBox.Show("Failed to apply restocking suggestion.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error applying suggestion: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    
    private Panel CreateRestockingHistoryPanel()
    {
        var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
        
        var lblInfo = new Label
        {
            Text = "Complete history of all restocking activities",
            Font = new Font("Segoe UI", 10, FontStyle.Italic),
            Location = new Point(10, 10),
            Size = new Size(900, 25),
            ForeColor = Color.Gray
        };
        panel.Controls.Add(lblInfo);
        
        var historyListView = new ListView
        {
            Location = new Point(10, 40),
            Size = new Size(950, 420),
            View = View.Details,
            FullRowSelect = true,
            GridLines = true,
            Font = new Font("Segoe UI", 9)
        };
        
        historyListView.Columns.Add("Date", 150);
        historyListView.Columns.Add("Product", 180);
        historyListView.Columns.Add("Qty Added", 80);
        historyListView.Columns.Add("Before", 70);
        historyListView.Columns.Add("After", 70);
        historyListView.Columns.Add("Unit Cost", 90);
        historyListView.Columns.Add("Total Cost", 90);
        historyListView.Columns.Add("Source", 100);
        historyListView.Columns.Add("Notes", 150);
        
        panel.Controls.Add(historyListView);
        
        var btnRefresh = new Button
        {
            Text = "Refresh",
            Location = new Point(10, 470),
            Size = new Size(120, 35),
            Font = new Font("Segoe UI", 10)
        };
        btnRefresh.Click += (s, e) => LoadRestockingHistory(historyListView);
        
        panel.Controls.Add(btnRefresh);
        
        // Load initial data
        LoadRestockingHistory(historyListView);
        
        return panel;
    }
    
    private async void LoadRestockingHistory(ListView listView)
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
            var context = scope.ServiceProvider.GetRequiredService<RetiSusunDbContext>();
            
            var records = await context.RestockingRecords
                .Include(r => r.Product)
                .Where(r => r.Product.BusinessId == _currentUser.BusinessId.Value)
                .OrderByDescending(r => r.RestockDate)
                .Take(100)
                .ToListAsync();
            
            listView.Items.Clear();
            
            foreach (var record in records)
            {
                var item = new ListViewItem(record.RestockDate.ToLocalTime().ToString("yyyy-MM-dd HH:mm"));
                item.SubItems.Add(record.Product?.Name ?? "N/A");
                item.SubItems.Add(record.QuantityAdded.ToString());
                item.SubItems.Add(record.StockBeforeRestock.ToString());
                item.SubItems.Add(record.StockAfterRestock.ToString());
                item.SubItems.Add($"RM {record.UnitCost:F2}");
                item.SubItems.Add($"RM {record.TotalCost:F2}");
                item.SubItems.Add(record.Source);
                item.SubItems.Add(record.Notes ?? "");
                
                listView.Items.Add(item);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading restocking history: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private Panel CreatePurchaseOrdersPanel()
    {
        var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
        
        // Title
        var lblTitle = new Label
        {
            Text = "Purchase Orders Management",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            Location = new Point(10, 10),
            Size = new Size(400, 35)
        };
        panel.Controls.Add(lblTitle);
        
        // Filter Section
        var grpFilter = new GroupBox
        {
            Text = "Filter",
            Location = new Point(10, 50),
            Size = new Size(500, 70),
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        
        var lblStatus = new Label
        {
            Text = "Status:",
            Location = new Point(15, 30),
            Size = new Size(60, 20),
            Font = new Font("Segoe UI", 9)
        };
        var cboStatus = new ComboBox
        {
            Location = new Point(80, 28),
            Size = new Size(150, 25),
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 9)
        };
        cboStatus.Items.AddRange(new object[] { "All", "Pending", "Ordered", "Received", "Cancelled" });
        cboStatus.SelectedIndex = 0;
        
        var btnFilterPO = new Button
        {
            Text = "Apply Filter",
            Location = new Point(245, 25),
            Size = new Size(100, 30),
            Font = new Font("Segoe UI", 9)
        };
        
        grpFilter.Controls.AddRange(new Control[] { lblStatus, cboStatus, btnFilterPO });
        panel.Controls.Add(grpFilter);
        
        // Purchase Orders ListView
        var poListView = new ListView
        {
            Location = new Point(10, 130),
            Size = new Size(1000, 400),
            View = View.Details,
            FullRowSelect = true,
            GridLines = true,
            Font = new Font("Segoe UI", 9)
        };
        
        poListView.Columns.Add("Order #", 120);
        poListView.Columns.Add("Order Date", 120);
        poListView.Columns.Add("Supplier", 150);
        poListView.Columns.Add("Status", 80);
        poListView.Columns.Add("Items", 60);
        poListView.Columns.Add("Total Amount", 100);
        poListView.Columns.Add("Expected Date", 120);
        poListView.Columns.Add("Actual Date", 120);
        poListView.Columns.Add("Notes", 150);
        
        panel.Controls.Add(poListView);
        
        // Buttons
        var btnCreatePO = new Button
        {
            Text = "Create Purchase Order",
            Location = new Point(10, 540),
            Size = new Size(180, 35),
            Font = new Font("Segoe UI", 10),
            BackColor = Color.LightGreen
        };
        btnCreatePO.Click += (s, e) => ShowCreatePurchaseOrderDialog(poListView, cboStatus);
        
        var btnViewPO = new Button
        {
            Text = "View Details",
            Location = new Point(200, 540),
            Size = new Size(120, 35),
            Font = new Font("Segoe UI", 10),
            BackColor = Color.LightBlue
        };
        btnViewPO.Click += (s, e) =>
        {
            if (poListView.SelectedItems.Count > 0)
            {
                var po = poListView.SelectedItems[0].Tag as PurchaseOrder;
                if (po != null)
                {
                    ShowPurchaseOrderDetails(po.PurchaseOrderId, poListView, cboStatus);
                }
            }
            else
            {
                MessageBox.Show("Please select a purchase order to view.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        };
        
        var btnReceivePO = new Button
        {
            Text = "Receive Order",
            Location = new Point(330, 540),
            Size = new Size(130, 35),
            Font = new Font("Segoe UI", 10),
            BackColor = Color.LightCyan
        };
        btnReceivePO.Click += async (s, e) =>
        {
            if (poListView.SelectedItems.Count > 0)
            {
                var po = poListView.SelectedItems[0].Tag as PurchaseOrder;
                if (po != null)
                {
                    await ReceivePurchaseOrder(po.PurchaseOrderId, poListView, cboStatus);
                }
            }
            else
            {
                MessageBox.Show("Please select a purchase order to receive.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        };
        
        var btnRefreshPO = new Button
        {
            Text = "Refresh",
            Location = new Point(470, 540),
            Size = new Size(100, 35),
            Font = new Font("Segoe UI", 10)
        };
        
        panel.Controls.AddRange(new Control[] { 
            btnCreatePO, btnViewPO, btnReceivePO, btnRefreshPO 
        });
        
        // Load initial data
        LoadPurchaseOrders(poListView, null);
        
        // Wire up event handlers
        btnFilterPO.Click += (s, e) =>
        {
            string? status = cboStatus.SelectedItem?.ToString();
            if (status == "All") status = null;
            LoadPurchaseOrders(poListView, status);
        };
        
        btnRefreshPO.Click += (s, e) =>
        {
            string? status = cboStatus.SelectedItem?.ToString();
            if (status == "All") status = null;
            LoadPurchaseOrders(poListView, status);
        };
        
        poListView.DoubleClick += (s, e) =>
        {
            if (poListView.SelectedItems.Count > 0)
            {
                var po = poListView.SelectedItems[0].Tag as PurchaseOrder;
                if (po != null)
                {
                    ShowPurchaseOrderDetails(po.PurchaseOrderId, poListView, cboStatus);
                }
            }
        };
        
        return panel;
    }
    
    private async void LoadPurchaseOrders(ListView listView, string? status)
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
            var poService = scope.ServiceProvider.GetRequiredService<IPurchaseOrderService>();
            
            var orders = await poService.GetPurchaseOrdersAsync(_currentUser.BusinessId.Value, status);
            
            listView.Items.Clear();
            
            foreach (var po in orders)
            {
                var item = new ListViewItem(po.OrderNumber);
                item.SubItems.Add(po.OrderDate.ToLocalTime().ToString("yyyy-MM-dd"));
                item.SubItems.Add(po.SupplierName ?? "N/A");
                item.SubItems.Add(po.Status);
                item.SubItems.Add(po.Items.Count.ToString());
                item.SubItems.Add($"RM {po.TotalAmount:F2}");
                item.SubItems.Add(po.ExpectedDeliveryDate?.ToLocalTime().ToString("yyyy-MM-dd") ?? "N/A");
                item.SubItems.Add(po.ActualDeliveryDate?.ToLocalTime().ToString("yyyy-MM-dd") ?? "N/A");
                item.SubItems.Add(po.Notes ?? "");
                
                // Color code by status
                switch (po.Status)
                {
                    case "Pending":
                        item.BackColor = Color.LightYellow;
                        break;
                    case "Ordered":
                        item.BackColor = Color.LightBlue;
                        break;
                    case "Received":
                        item.BackColor = Color.LightGreen;
                        break;
                    case "Cancelled":
                        item.BackColor = Color.LightGray;
                        break;
                }
                
                item.Tag = po;
                listView.Items.Add(item);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading purchase orders: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    
    private void ShowCreatePurchaseOrderDialog(ListView poListView, ComboBox cboStatus)
    {
        var form = new Form
        {
            Text = "Create Purchase Order",
            Size = new Size(700, 600),
            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.Sizable
        };
        
        var y = 20;
        
        // Supplier Information
        var grpSupplier = new GroupBox
        {
            Text = "Supplier Information",
            Location = new Point(20, y),
            Size = new Size(640, 120),
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        
        grpSupplier.Controls.Add(new Label { Text = "Supplier Name:", Location = new Point(10, 25), Size = new Size(100, 20), Font = new Font("Segoe UI", 9) });
        var txtSupplierName = new TextBox { Location = new Point(120, 23), Size = new Size(200, 25), Font = new Font("Segoe UI", 9) };
        grpSupplier.Controls.Add(txtSupplierName);
        
        grpSupplier.Controls.Add(new Label { Text = "Email:", Location = new Point(340, 25), Size = new Size(60, 20), Font = new Font("Segoe UI", 9) });
        var txtSupplierEmail = new TextBox { Location = new Point(410, 23), Size = new Size(210, 25), Font = new Font("Segoe UI", 9) };
        grpSupplier.Controls.Add(txtSupplierEmail);
        
        grpSupplier.Controls.Add(new Label { Text = "Phone:", Location = new Point(10, 55), Size = new Size(100, 20), Font = new Font("Segoe UI", 9) });
        var txtSupplierPhone = new TextBox { Location = new Point(120, 53), Size = new Size(200, 25), Font = new Font("Segoe UI", 9) };
        grpSupplier.Controls.Add(txtSupplierPhone);
        
        grpSupplier.Controls.Add(new Label { Text = "Expected Date:", Location = new Point(340, 55), Size = new Size(100, 20), Font = new Font("Segoe UI", 9) });
        var dtpExpectedDate = new DateTimePicker { Location = new Point(440, 53), Size = new Size(180, 25), Format = DateTimePickerFormat.Short };
        dtpExpectedDate.Value = DateTime.Today.AddDays(7);
        grpSupplier.Controls.Add(dtpExpectedDate);
        
        grpSupplier.Controls.Add(new Label { Text = "Notes:", Location = new Point(10, 85), Size = new Size(100, 20), Font = new Font("Segoe UI", 9) });
        var txtNotes = new TextBox { Location = new Point(120, 83), Size = new Size(500, 25), Font = new Font("Segoe UI", 9) };
        grpSupplier.Controls.Add(txtNotes);
        
        form.Controls.Add(grpSupplier);
        y += 130;
        
        // Items Section
        var lblItems = new Label
        {
            Text = "Order Items:",
            Location = new Point(20, y),
            Size = new Size(150, 25),
            Font = new Font("Segoe UI", 11, FontStyle.Bold)
        };
        form.Controls.Add(lblItems);
        y += 30;
        
        var itemsListView = new ListView
        {
            Location = new Point(20, y),
            Size = new Size(640, 250),
            View = View.Details,
            FullRowSelect = true,
            GridLines = true,
            Font = new Font("Segoe UI", 9)
        };
        itemsListView.Columns.Add("Product", 220);
        itemsListView.Columns.Add("Quantity", 100);
        itemsListView.Columns.Add("Unit Cost", 100);
        itemsListView.Columns.Add("Total", 100);
        form.Controls.Add(itemsListView);
        y += 260;
        
        var btnAddItem = new Button
        {
            Text = "Add Item",
            Location = new Point(20, y),
            Size = new Size(100, 30),
            Font = new Font("Segoe UI", 9)
        };
        btnAddItem.Click += (s, e) => ShowAddPOItemDialog(itemsListView);
        
        var btnRemoveItem = new Button
        {
            Text = "Remove Item",
            Location = new Point(130, y),
            Size = new Size(100, 30),
            Font = new Font("Segoe UI", 9)
        };
        btnRemoveItem.Click += (s, e) =>
        {
            if (itemsListView.SelectedItems.Count > 0)
            {
                itemsListView.Items.Remove(itemsListView.SelectedItems[0]);
                UpdatePOTotal(itemsListView, form.Controls);
            }
        };
        
        var lblTotal = new Label
        {
            Text = "Total: RM 0.00",
            Location = new Point(500, y + 5),
            Size = new Size(150, 25),
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            ForeColor = Color.DarkGreen,
            TextAlign = ContentAlignment.MiddleRight
        };
        form.Controls.Add(lblTotal);
        
        form.Controls.AddRange(new Control[] { btnAddItem, btnRemoveItem });
        y += 45;
        
        // Save and Cancel buttons
        var btnSave = new Button
        {
            Text = "Create Order",
            Location = new Point(220, y),
            Size = new Size(120, 35),
            BackColor = Color.LightGreen,
            Font = new Font("Segoe UI", 10)
        };
        btnSave.Click += async (s, e) =>
        {
            if (string.IsNullOrWhiteSpace(txtSupplierName.Text))
            {
                MessageBox.Show("Please enter supplier name!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            if (itemsListView.Items.Count == 0)
            {
                MessageBox.Show("Please add at least one item to the order!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            try
            {
                using var scope = Program.ServiceProvider!.CreateScope();
                var poService = scope.ServiceProvider.GetRequiredService<IPurchaseOrderService>();
                
                var po = new PurchaseOrder
                {
                    BusinessId = _currentUser.BusinessId!.Value,
                    SupplierName = txtSupplierName.Text,
                    SupplierEmail = txtSupplierEmail.Text,
                    SupplierPhone = txtSupplierPhone.Text,
                    ExpectedDeliveryDate = dtpExpectedDate.Value,
                    Notes = txtNotes.Text,
                    CreatedByUserId = _currentUser.UserId,
                    Items = new List<PurchaseOrderItem>()
                };
                
                foreach (ListViewItem item in itemsListView.Items)
                {
                    var itemData = item.Tag as dynamic;
                    if (itemData != null)
                    {
                        po.Items.Add(new PurchaseOrderItem
                        {
                            ProductId = itemData.ProductId,
                            QuantityOrdered = itemData.Quantity,
                            UnitCost = itemData.UnitCost,
                            TotalCost = itemData.TotalCost
                        });
                    }
                }
                
                po.TotalAmount = po.Items.Sum(i => i.TotalCost);
                
                await poService.CreatePurchaseOrderAsync(po);
                
                MessageBox.Show("Purchase order created successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                string? status = cboStatus.SelectedItem?.ToString();
                if (status == "All") status = null;
                LoadPurchaseOrders(poListView, status);
                form.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating purchase order: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        };
        
        var btnCancel = new Button
        {
            Text = "Cancel",
            Location = new Point(350, y),
            Size = new Size(100, 35),
            Font = new Font("Segoe UI", 10)
        };
        btnCancel.Click += (s, e) => form.Close();
        
        form.Controls.AddRange(new Control[] { btnSave, btnCancel });
        
        form.ShowDialog();
    }
    
    private async void ShowAddPOItemDialog(ListView itemsListView)
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
            
            var form = new Form
            {
                Text = "Add Item to Purchase Order",
                Size = new Size(450, 250),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };
            
            var y = 20;
            
            form.Controls.Add(new Label { Text = "Product:", Location = new Point(20, y), Size = new Size(100, 20) });
            var cboProduct = new ComboBox 
            { 
                Location = new Point(130, y - 2), 
                Size = new Size(280, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            
            foreach (var product in products)
            {
                cboProduct.Items.Add(product);
                cboProduct.DisplayMember = "Name";
            }
            
            if (cboProduct.Items.Count > 0)
                cboProduct.SelectedIndex = 0;
            
            form.Controls.Add(cboProduct);
            y += 40;
            
            form.Controls.Add(new Label { Text = "Quantity:", Location = new Point(20, y), Size = new Size(100, 20) });
            var nudQuantity = new NumericUpDown 
            { 
                Location = new Point(130, y - 2), 
                Size = new Size(150, 25),
                Minimum = 1,
                Maximum = 100000,
                Value = 1
            };
            form.Controls.Add(nudQuantity);
            y += 40;
            
            form.Controls.Add(new Label { Text = "Unit Cost (RM):", Location = new Point(20, y), Size = new Size(100, 20) });
            var nudUnitCost = new NumericUpDown 
            { 
                Location = new Point(130, y - 2), 
                Size = new Size(150, 25),
                DecimalPlaces = 2,
                Maximum = 100000,
                Value = 0
            };
            
            cboProduct.SelectedIndexChanged += (s, e) =>
            {
                if (cboProduct.SelectedItem is Product product)
                {
                    nudUnitCost.Value = product.CostPrice;
                }
            };
            
            if (cboProduct.SelectedItem is Product selectedProduct)
            {
                nudUnitCost.Value = selectedProduct.CostPrice;
            }
            
            form.Controls.Add(nudUnitCost);
            y += 50;
            
            var btnAdd = new Button
            {
                Text = "Add",
                Location = new Point(130, y),
                Size = new Size(100, 35),
                BackColor = Color.LightGreen
            };
            btnAdd.Click += (s, e) =>
            {
                if (cboProduct.SelectedItem is Product product)
                {
                    var item = new ListViewItem(product.Name);
                    item.SubItems.Add(nudQuantity.Value.ToString());
                    item.SubItems.Add($"RM {nudUnitCost.Value:F2}");
                    var total = nudQuantity.Value * nudUnitCost.Value;
                    item.SubItems.Add($"RM {total:F2}");
                    
                    item.Tag = new 
                    { 
                        ProductId = product.ProductId,
                        Quantity = (int)nudQuantity.Value,
                        UnitCost = nudUnitCost.Value,
                        TotalCost = total
                    };
                    
                    itemsListView.Items.Add(item);
                    UpdatePOTotal(itemsListView, form.Parent!.Controls);
                    form.Close();
                }
            };
            
            var btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(240, y),
                Size = new Size(100, 35)
            };
            btnCancel.Click += (s, e) => form.Close();
            
            form.Controls.AddRange(new Control[] { btnAdd, btnCancel });
            
            form.ShowDialog();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading products: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    
    private void UpdatePOTotal(ListView itemsListView, Control.ControlCollection controls)
    {
        decimal total = 0;
        foreach (ListViewItem item in itemsListView.Items)
        {
            var itemData = item.Tag as dynamic;
            if (itemData != null)
            {
                total += itemData.TotalCost;
            }
        }
        
        foreach (Control control in controls)
        {
            if (control is Label label && label.Text.StartsWith("Total:"))
            {
                label.Text = $"Total: RM {total:F2}";
                break;
            }
        }
    }
    
    private async void ShowPurchaseOrderDetails(int purchaseOrderId, ListView poListView, ComboBox cboStatus)
    {
        try
        {
            using var scope = Program.ServiceProvider!.CreateScope();
            var poService = scope.ServiceProvider.GetRequiredService<IPurchaseOrderService>();
            
            var po = await poService.GetPurchaseOrderByIdAsync(purchaseOrderId);
            if (po == null)
            {
                MessageBox.Show("Purchase order not found!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            var form = new Form
            {
                Text = $"Purchase Order Details - {po.OrderNumber}",
                Size = new Size(600, 550),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.Sizable
            };
            
            var txtDetails = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true,
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 10),
                Padding = new Padding(10)
            };
            
            var details = new StringBuilder();
            details.AppendLine($"Order Number: {po.OrderNumber}");
            details.AppendLine($"Order Date: {po.OrderDate.ToLocalTime():yyyy-MM-dd}");
            details.AppendLine($"Status: {po.Status}");
            details.AppendLine($"Supplier: {po.SupplierName ?? "N/A"}");
            details.AppendLine($"Supplier Email: {po.SupplierEmail ?? "N/A"}");
            details.AppendLine($"Supplier Phone: {po.SupplierPhone ?? "N/A"}");
            details.AppendLine($"Expected Delivery: {(po.ExpectedDeliveryDate?.ToLocalTime().ToString("yyyy-MM-dd") ?? "N/A")}");
            details.AppendLine($"Actual Delivery: {(po.ActualDeliveryDate?.ToLocalTime().ToString("yyyy-MM-dd") ?? "N/A")}");
            details.AppendLine();
            details.AppendLine("=== ITEMS ===");
            
            foreach (var item in po.Items)
            {
                details.AppendLine($"{item.Product?.Name ?? "N/A"}");
                details.AppendLine($"  Ordered: {item.QuantityOrdered}, Received: {item.QuantityReceived}");
                details.AppendLine($"  Unit Cost: RM {item.UnitCost:F2}, Total: RM {item.TotalCost:F2}");
            }
            
            details.AppendLine();
            details.AppendLine($"Total Amount: RM {po.TotalAmount:F2}");
            
            if (!string.IsNullOrEmpty(po.Notes))
            {
                details.AppendLine();
                details.AppendLine($"Notes: {po.Notes}");
            }
            
            txtDetails.Text = details.ToString();
            form.Controls.Add(txtDetails);
            
            // Add buttons panel at the bottom
            var buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                Padding = new Padding(10)
            };
            
            if (po.Status == "Pending")
            {
                var btnMarkOrdered = new Button
                {
                    Text = "Mark as Ordered",
                    Location = new Point(10, 10),
                    Size = new Size(150, 30),
                    BackColor = Color.LightBlue
                };
                btnMarkOrdered.Click += async (s, e) =>
                {
                    po.Status = "Ordered";
                    await poService.UpdatePurchaseOrderAsync(po);
                    MessageBox.Show("Status updated to Ordered", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    string? status = cboStatus.SelectedItem?.ToString();
                    if (status == "All") status = null;
                    LoadPurchaseOrders(poListView, status);
                    form.Close();
                };
                buttonPanel.Controls.Add(btnMarkOrdered);
            }
            
            var btnClose = new Button
            {
                Text = "Close",
                Location = new Point(430, 10),
                Size = new Size(100, 30)
            };
            btnClose.Click += (s, e) => form.Close();
            buttonPanel.Controls.Add(btnClose);
            
            form.Controls.Add(buttonPanel);
            
            form.ShowDialog();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error showing purchase order details: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    
    private async Task ReceivePurchaseOrder(int purchaseOrderId, ListView poListView, ComboBox cboStatus)
    {
        try
        {
            using var scope = Program.ServiceProvider!.CreateScope();
            var poService = scope.ServiceProvider.GetRequiredService<IPurchaseOrderService>();
            
            var po = await poService.GetPurchaseOrderByIdAsync(purchaseOrderId);
            if (po == null)
            {
                MessageBox.Show("Purchase order not found!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            if (po.Status != "Ordered")
            {
                MessageBox.Show($"Cannot receive order with status '{po.Status}'. Only orders with status 'Ordered' can be received.",
                    "Invalid Status", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            var result = MessageBox.Show(
                $"Receive purchase order {po.OrderNumber}?\n\nThis will update inventory for all items in the order.",
                "Confirm Receipt",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            
            if (result != DialogResult.Yes)
                return;
            
            var success = await poService.ReceivePurchaseOrderAsync(purchaseOrderId, _currentUser.UserId);
            
            if (success)
            {
                MessageBox.Show("Purchase order received successfully! Inventory has been updated.",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                string? status = cboStatus.SelectedItem?.ToString();
                if (status == "All") status = null;
                LoadPurchaseOrders(poListView, status);
            }
            else
            {
                MessageBox.Show("Failed to receive purchase order.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error receiving purchase order: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private Panel CreateReportsPanel()
    {
        var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
        
        // Title
        var lblTitle = new Label
        {
            Text = "Business Reports & Analytics",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            Location = new Point(10, 10),
            Size = new Size(400, 35)
        };
        panel.Controls.Add(lblTitle);
        
        // Report Type Selection
        var grpReportType = new GroupBox
        {
            Text = "Report Type",
            Location = new Point(10, 50),
            Size = new Size(1000, 120),
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        
        var cboReportType = new ComboBox
        {
            Location = new Point(15, 30),
            Size = new Size(200, 25),
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 9)
        };
        cboReportType.Items.AddRange(new object[] { 
            "End of Day", 
            "End of Week", 
            "End of Month", 
            "Quarterly", 
            "Yearly",
            "Custom Date Range"
        });
        cboReportType.SelectedIndex = 0;
        
        var lblDateRange = new Label
        {
            Text = "Date Range:",
            Location = new Point(230, 33),
            Size = new Size(80, 20),
            Font = new Font("Segoe UI", 9)
        };
        
        var dtpFromDate = new DateTimePicker
        {
            Location = new Point(315, 30),
            Size = new Size(150, 25),
            Format = DateTimePickerFormat.Short
        };
        
        var lblTo = new Label
        {
            Text = "to",
            Location = new Point(475, 33),
            Size = new Size(20, 20),
            Font = new Font("Segoe UI", 9)
        };
        
        var dtpToDate = new DateTimePicker
        {
            Location = new Point(500, 30),
            Size = new Size(150, 25),
            Format = DateTimePickerFormat.Short
        };
        
        // Auto-set date range based on report type
        cboReportType.SelectedIndexChanged += (s, e) =>
        {
            var today = DateTime.Today;
            switch (cboReportType.SelectedItem?.ToString())
            {
                case "End of Day":
                    dtpFromDate.Value = today;
                    dtpToDate.Value = today;
                    dtpFromDate.Enabled = false;
                    dtpToDate.Enabled = false;
                    break;
                case "End of Week":
                    var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
                    dtpFromDate.Value = startOfWeek;
                    dtpToDate.Value = startOfWeek.AddDays(6);
                    dtpFromDate.Enabled = false;
                    dtpToDate.Enabled = false;
                    break;
                case "End of Month":
                    dtpFromDate.Value = new DateTime(today.Year, today.Month, 1);
                    dtpToDate.Value = dtpFromDate.Value.AddMonths(1).AddDays(-1);
                    dtpFromDate.Enabled = false;
                    dtpToDate.Enabled = false;
                    break;
                case "Quarterly":
                    var quarter = (today.Month - 1) / 3;
                    dtpFromDate.Value = new DateTime(today.Year, quarter * 3 + 1, 1);
                    dtpToDate.Value = dtpFromDate.Value.AddMonths(3).AddDays(-1);
                    dtpFromDate.Enabled = false;
                    dtpToDate.Enabled = false;
                    break;
                case "Yearly":
                    dtpFromDate.Value = new DateTime(today.Year, 1, 1);
                    dtpToDate.Value = new DateTime(today.Year, 12, 31);
                    dtpFromDate.Enabled = false;
                    dtpToDate.Enabled = false;
                    break;
                case "Custom Date Range":
                    dtpFromDate.Enabled = true;
                    dtpToDate.Enabled = true;
                    break;
            }
        };
        
        cboReportType.SelectedIndex = 0; // Trigger initial date range
        
        var btnGenerateReport = new Button
        {
            Text = "Generate Report",
            Location = new Point(670, 27),
            Size = new Size(150, 30),
            Font = new Font("Segoe UI", 10),
            BackColor = Color.LightGreen
        };
        
        var btnExportReport = new Button
        {
            Text = "Export to Text",
            Location = new Point(830, 27),
            Size = new Size(130, 30),
            Font = new Font("Segoe UI", 9)
        };
        
        // Quick Stats
        var lblQuickStats = new Label
        {
            Text = "Quick Stats:",
            Location = new Point(15, 70),
            Size = new Size(100, 20),
            Font = new Font("Segoe UI", 9, FontStyle.Bold)
        };
        
        var lblTotalSales = new Label
        {
            Text = "Total Sales: RM 0.00",
            Location = new Point(120, 70),
            Size = new Size(180, 20),
            Font = new Font("Segoe UI", 9),
            ForeColor = Color.DarkGreen
        };
        
        var lblTotalTransactions = new Label
        {
            Text = "Transactions: 0",
            Location = new Point(310, 70),
            Size = new Size(150, 20),
            Font = new Font("Segoe UI", 9)
        };
        
        var lblAvgTransaction = new Label
        {
            Text = "Avg/Transaction: RM 0.00",
            Location = new Point(470, 70),
            Size = new Size(200, 20),
            Font = new Font("Segoe UI", 9)
        };
        
        grpReportType.Controls.AddRange(new Control[] {
            cboReportType, lblDateRange, dtpFromDate, lblTo, dtpToDate,
            btnGenerateReport, btnExportReport, lblQuickStats,
            lblTotalSales, lblTotalTransactions, lblAvgTransaction
        });
        panel.Controls.Add(grpReportType);
        
        // Report Display Area
        var txtReport = new TextBox
        {
            Location = new Point(10, 180),
            Size = new Size(1000, 450),
            Multiline = true,
            ScrollBars = ScrollBars.Both,
            ReadOnly = true,
            Font = new Font("Consolas", 9),
            BackColor = Color.White
        };
        panel.Controls.Add(txtReport);
        
        // Generate Report Button Handler
        btnGenerateReport.Click += async (s, e) =>
        {
            await GenerateReport(
                cboReportType.SelectedItem?.ToString() ?? "End of Day",
                dtpFromDate.Value.Date,
                dtpToDate.Value.Date.AddDays(1).AddSeconds(-1),
                txtReport,
                lblTotalSales,
                lblTotalTransactions,
                lblAvgTransaction
            );
        };
        
        // Export Button Handler
        btnExportReport.Click += (s, e) =>
        {
            if (string.IsNullOrWhiteSpace(txtReport.Text))
            {
                MessageBox.Show("Please generate a report first!", "No Report",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            var saveDialog = new SaveFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                FileName = $"Report_{cboReportType.SelectedItem}_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
            };
            
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    System.IO.File.WriteAllText(saveDialog.FileName, txtReport.Text);
                    MessageBox.Show($"Report exported successfully to:\n{saveDialog.FileName}",
                        "Export Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error exporting report: {ex.Message}",
                        "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        };
        
        // Generate initial report
        _ = GenerateReport(
            "End of Day",
            DateTime.Today,
            DateTime.Today.AddDays(1).AddSeconds(-1),
            txtReport,
            lblTotalSales,
            lblTotalTransactions,
            lblAvgTransaction
        );
        
        return panel;
    }
    
    private async Task GenerateReport(
        string reportType,
        DateTime startDate,
        DateTime endDate,
        TextBox txtReport,
        Label lblTotalSales,
        Label lblTotalTransactions,
        Label lblAvgTransaction)
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
            var salesService = scope.ServiceProvider.GetRequiredService<ISalesService>();
            var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
            var context = scope.ServiceProvider.GetRequiredService<RetiSusunDbContext>();
            
            var report = new StringBuilder();
            
            // Header
            report.AppendLine("========================================");
            report.AppendLine($"       {reportType.ToUpper()} REPORT");
            report.AppendLine("========================================");
            report.AppendLine($"Period: {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}");
            report.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            report.AppendLine($"Generated By: {_currentUser.FullName}");
            report.AppendLine("========================================");
            report.AppendLine();
            
            // Get transactions
            var transactions = await salesService.GetSalesTransactionsAsync(
                _currentUser.BusinessId.Value, startDate, endDate);
            
            var transactionsList = transactions.ToList();
            var totalSales = transactionsList.Sum(t => t.TotalAmount);
            var transactionCount = transactionsList.Count;
            var avgTransaction = transactionCount > 0 ? totalSales / transactionCount : 0;
            
            // Update quick stats
            lblTotalSales.Text = $"Total Sales: RM {totalSales:F2}";
            lblTotalTransactions.Text = $"Transactions: {transactionCount}";
            lblAvgTransaction.Text = $"Avg/Transaction: RM {avgTransaction:F2}";
            
            // Sales Summary
            report.AppendLine("=== SALES SUMMARY ===");
            report.AppendLine($"Total Transactions:     {transactionCount}");
            report.AppendLine($"Total Sales Amount:     RM {totalSales:F2}");
            report.AppendLine($"Average Transaction:    RM {avgTransaction:F2}");
            
            var subtotal = transactionsList.Sum(t => t.SubTotal);
            var totalTax = transactionsList.Sum(t => t.TaxAmount);
            var totalDiscount = transactionsList.Sum(t => t.DiscountAmount);
            
            report.AppendLine($"Subtotal:               RM {subtotal:F2}");
            report.AppendLine($"Total Tax:              RM {totalTax:F2}");
            report.AppendLine($"Total Discount:         RM {totalDiscount:F2}");
            report.AppendLine();
            
            // Payment Methods Breakdown
            report.AppendLine("=== PAYMENT METHODS ===");
            var paymentMethodGroups = transactionsList
                .GroupBy(t => t.PaymentMethod)
                .OrderByDescending(g => g.Sum(t => t.TotalAmount));
            
            foreach (var group in paymentMethodGroups)
            {
                var methodTotal = group.Sum(t => t.TotalAmount);
                var methodCount = group.Count();
                report.AppendLine($"{group.Key,-15} Count: {methodCount,5}   Amount: RM {methodTotal,10:F2}");
            }
            report.AppendLine();
            
            // Top Selling Products
            report.AppendLine("=== TOP 10 SELLING PRODUCTS ===");
            var productSales = await context.SalesTransactionItems
                .Include(sti => sti.Product)
                .Include(sti => sti.SalesTransaction)
                .Where(sti => sti.SalesTransaction.BusinessId == _currentUser.BusinessId.Value
                    && sti.SalesTransaction.TransactionDate >= startDate
                    && sti.SalesTransaction.TransactionDate <= endDate
                    && !sti.SalesTransaction.IsVoided)
                .GroupBy(sti => new { sti.ProductId, sti.Product!.Name })
                .Select(g => new
                {
                    ProductName = g.Key.Name,
                    TotalQuantity = g.Sum(sti => sti.Quantity),
                    TotalRevenue = g.Sum(sti => sti.TotalPrice)
                })
                .OrderByDescending(p => p.TotalQuantity)
                .Take(10)
                .ToListAsync();
            
            int rank = 1;
            foreach (var product in productSales)
            {
                report.AppendLine($"{rank,2}. {product.ProductName,-30} Qty: {product.TotalQuantity,5}   Revenue: RM {product.TotalRevenue,10:F2}");
                rank++;
            }
            
            if (!productSales.Any())
            {
                report.AppendLine("No product sales in this period.");
            }
            report.AppendLine();
            
            // Daily Breakdown (for reports covering multiple days)
            var daysDiff = (endDate.Date - startDate.Date).Days;
            if (daysDiff > 0 && daysDiff <= 366)
            {
                report.AppendLine("=== DAILY BREAKDOWN ===");
                var dailyGroups = transactionsList
                    .GroupBy(t => t.TransactionDate.Date)
                    .OrderBy(g => g.Key);
                
                foreach (var dayGroup in dailyGroups)
                {
                    var dayTotal = dayGroup.Sum(t => t.TotalAmount);
                    var dayCount = dayGroup.Count();
                    report.AppendLine($"{dayGroup.Key:yyyy-MM-dd}   Transactions: {dayCount,4}   Sales: RM {dayTotal,10:F2}");
                }
                report.AppendLine();
            }
            
            // Inventory Status
            report.AppendLine("=== INVENTORY STATUS ===");
            var products = await productService.GetAllProductsAsync(_currentUser.BusinessId.Value);
            var productsList = products.ToList();
            
            var totalProducts = productsList.Count;
            var lowStockProducts = productsList.Count(p => p.StockQuantity <= p.ReorderLevel);
            var outOfStockProducts = productsList.Count(p => p.StockQuantity <= 0);
            var inventoryValue = productsList.Sum(p => p.StockQuantity * p.CostPrice);
            var potentialRevenue = productsList.Sum(p => p.StockQuantity * p.SellingPrice);
            
            report.AppendLine($"Total Products:         {totalProducts}");
            report.AppendLine($"Low Stock Products:     {lowStockProducts}");
            report.AppendLine($"Out of Stock Products:  {outOfStockProducts}");
            report.AppendLine($"Inventory Value (Cost): RM {inventoryValue:F2}");
            report.AppendLine($"Potential Revenue:      RM {potentialRevenue:F2}");
            report.AppendLine();
            
            // Low Stock Alert
            if (lowStockProducts > 0)
            {
                report.AppendLine("=== LOW STOCK ALERT ===");
                var lowStockList = productsList
                    .Where(p => p.StockQuantity <= p.ReorderLevel)
                    .OrderBy(p => p.StockQuantity)
                    .Take(10);
                
                foreach (var product in lowStockList)
                {
                    var status = product.StockQuantity <= 0 ? "OUT OF STOCK" :
                                 product.StockQuantity <= product.MinimumStockLevel ? "CRITICAL" : "LOW";
                    report.AppendLine($"{product.Name,-30} Stock: {product.StockQuantity,5}   Reorder: {product.ReorderLevel,5}   [{status}]");
                }
                report.AppendLine();
            }
            
            // Profit Analysis (estimated)
            var estimatedCost = await context.SalesTransactionItems
                .Include(sti => sti.Product)
                .Include(sti => sti.SalesTransaction)
                .Where(sti => sti.SalesTransaction.BusinessId == _currentUser.BusinessId.Value
                    && sti.SalesTransaction.TransactionDate >= startDate
                    && sti.SalesTransaction.TransactionDate <= endDate
                    && !sti.SalesTransaction.IsVoided)
                .SumAsync(sti => sti.Quantity * sti.Product!.CostPrice);
            
            var grossProfit = subtotal - estimatedCost;
            var profitMargin = subtotal > 0 ? (grossProfit / subtotal) * 100 : 0;
            
            report.AppendLine("=== PROFIT ANALYSIS (ESTIMATED) ===");
            report.AppendLine($"Total Revenue:          RM {subtotal:F2}");
            report.AppendLine($"Estimated COGS:         RM {estimatedCost:F2}");
            report.AppendLine($"Gross Profit:           RM {grossProfit:F2}");
            report.AppendLine($"Profit Margin:          {profitMargin:F2}%");
            report.AppendLine();
            
            report.AppendLine("========================================");
            report.AppendLine("           END OF REPORT");
            report.AppendLine("========================================");
            
            txtReport.Text = report.ToString();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error generating report: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
