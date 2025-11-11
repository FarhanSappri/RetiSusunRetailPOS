using Microsoft.Extensions.DependencyInjection;
using RetiSusun.Core.Interfaces;
using RetiSusun.Data.Models;

namespace RetiSusun.Desktop.Forms;

public partial class SupplierDashboardForm : Form
{
    private readonly User _currentUser;
    private TabControl tabControl = null!;
    private TabPage tabProducts = null!;
    private TabPage tabOrders = null!;
    private TabPage tabReports = null!;
    private Label lblWelcome = null!;

    // Products tab controls
    private DataGridView dgvProducts = null!;
    private Button btnAddProduct = null!;
    private Button btnEditProduct = null!;
    private Button btnDeleteProduct = null!;
    private Button btnRefreshProducts = null!;
    private TextBox txtSearchProduct = null!;

    // Orders tab controls
    private DataGridView dgvOrders = null!;
    private Button btnViewOrder = null!;
    private Button btnUpdateOrderStatus = null!;
    private Button btnRefreshOrders = null!;
    private ComboBox cmbOrderStatus = null!;

    // Reports tab controls
    private Label lblTotalOrders = null!;
    private Label lblTotalSales = null!;
    private Label lblPendingOrders = null!;
    private Label lblProductCount = null!;
    private Button btnGenerateReport = null!;

    public SupplierDashboardForm(User user)
    {
        _currentUser = user;
        InitializeComponent();
        LoadDashboard();
    }

    private void InitializeComponent()
    {
        this.Text = "RetiSusun - Supplier Dashboard";
        this.Size = new Size(1000, 700);
        this.StartPosition = FormStartPosition.CenterScreen;

        // Welcome Label
        lblWelcome = new Label
        {
            Location = new Point(20, 20),
            Size = new Size(960, 30),
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            Text = $"Welcome, {_currentUser.FullName}"
        };

        // Tab Control
        tabControl = new TabControl
        {
            Location = new Point(20, 60),
            Size = new Size(960, 600)
        };

        // Products Tab
        tabProducts = new TabPage("Product Catalog");
        InitializeProductsTab();
        tabControl.TabPages.Add(tabProducts);

        // Orders Tab
        tabOrders = new TabPage("Orders");
        InitializeOrdersTab();
        tabControl.TabPages.Add(tabOrders);

        // Reports Tab
        tabReports = new TabPage("Reports");
        InitializeReportsTab();
        tabControl.TabPages.Add(tabReports);

        this.Controls.Add(lblWelcome);
        this.Controls.Add(tabControl);
    }

    private void InitializeProductsTab()
    {
        // Search
        var lblSearch = new Label
        {
            Text = "Search:",
            Location = new Point(20, 20),
            Size = new Size(60, 25)
        };
        tabProducts.Controls.Add(lblSearch);

        txtSearchProduct = new TextBox
        {
            Location = new Point(90, 20),
            Size = new Size(250, 25)
        };
        txtSearchProduct.TextChanged += TxtSearchProduct_TextChanged;
        tabProducts.Controls.Add(txtSearchProduct);

        // Buttons
        btnAddProduct = new Button
        {
            Text = "Add Product",
            Location = new Point(360, 15),
            Size = new Size(120, 35)
        };
        btnAddProduct.Click += BtnAddProduct_Click;
        tabProducts.Controls.Add(btnAddProduct);

        btnEditProduct = new Button
        {
            Text = "Edit Product",
            Location = new Point(490, 15),
            Size = new Size(120, 35)
        };
        btnEditProduct.Click += BtnEditProduct_Click;
        tabProducts.Controls.Add(btnEditProduct);

        btnDeleteProduct = new Button
        {
            Text = "Delete Product",
            Location = new Point(620, 15),
            Size = new Size(120, 35)
        };
        btnDeleteProduct.Click += BtnDeleteProduct_Click;
        tabProducts.Controls.Add(btnDeleteProduct);

        btnRefreshProducts = new Button
        {
            Text = "Refresh",
            Location = new Point(750, 15),
            Size = new Size(100, 35)
        };
        btnRefreshProducts.Click += BtnRefreshProducts_Click;
        tabProducts.Controls.Add(btnRefreshProducts);

        // Data Grid
        dgvProducts = new DataGridView
        {
            Location = new Point(20, 60),
            Size = new Size(900, 470),
            ReadOnly = true,
            AllowUserToAddRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };
        tabProducts.Controls.Add(dgvProducts);
    }

    private void InitializeOrdersTab()
    {
        // Filter
        var lblFilter = new Label
        {
            Text = "Filter by Status:",
            Location = new Point(20, 20),
            Size = new Size(100, 25)
        };
        tabOrders.Controls.Add(lblFilter);

        cmbOrderStatus = new ComboBox
        {
            Location = new Point(130, 20),
            Size = new Size(150, 25),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cmbOrderStatus.Items.AddRange(new object[] { "All", "Pending", "Confirmed", "Preparing", "Shipped", "Delivered", "Cancelled" });
        cmbOrderStatus.SelectedIndex = 0;
        cmbOrderStatus.SelectedIndexChanged += CmbOrderStatus_SelectedIndexChanged;
        tabOrders.Controls.Add(cmbOrderStatus);

        // Buttons
        btnViewOrder = new Button
        {
            Text = "View Details",
            Location = new Point(300, 15),
            Size = new Size(120, 35)
        };
        btnViewOrder.Click += BtnViewOrder_Click;
        tabOrders.Controls.Add(btnViewOrder);

        btnUpdateOrderStatus = new Button
        {
            Text = "Update Status",
            Location = new Point(430, 15),
            Size = new Size(120, 35)
        };
        btnUpdateOrderStatus.Click += BtnUpdateOrderStatus_Click;
        tabOrders.Controls.Add(btnUpdateOrderStatus);

        btnRefreshOrders = new Button
        {
            Text = "Refresh",
            Location = new Point(560, 15),
            Size = new Size(100, 35)
        };
        btnRefreshOrders.Click += BtnRefreshOrders_Click;
        tabOrders.Controls.Add(btnRefreshOrders);

        // Data Grid
        dgvOrders = new DataGridView
        {
            Location = new Point(20, 60),
            Size = new Size(900, 470),
            ReadOnly = true,
            AllowUserToAddRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };
        tabOrders.Controls.Add(dgvOrders);
    }

    private void InitializeReportsTab()
    {
        int y = 30;

        // Dashboard cards
        lblTotalOrders = CreateStatLabel("Total Orders: Loading...", ref y);
        tabReports.Controls.Add(lblTotalOrders);

        lblPendingOrders = CreateStatLabel("Pending Orders: Loading...", ref y);
        tabReports.Controls.Add(lblPendingOrders);

        lblTotalSales = CreateStatLabel("Total Sales: Loading...", ref y);
        tabReports.Controls.Add(lblTotalSales);

        lblProductCount = CreateStatLabel("Active Products: Loading...", ref y);
        tabReports.Controls.Add(lblProductCount);

        btnGenerateReport = new Button
        {
            Text = "Generate Detailed Report",
            Location = new Point(50, y + 20),
            Size = new Size(200, 40)
        };
        btnGenerateReport.Click += BtnGenerateReport_Click;
        tabReports.Controls.Add(btnGenerateReport);
    }

    private Label CreateStatLabel(string text, ref int y)
    {
        var label = new Label
        {
            Text = text,
            Location = new Point(50, y),
            Size = new Size(800, 40),
            Font = new Font("Segoe UI", 12, FontStyle.Regular),
            BorderStyle = BorderStyle.FixedSingle,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(10)
        };
        y += 50;
        return label;
    }

    private async void LoadDashboard()
    {
        await LoadProducts();
        await LoadOrders();
        await LoadReports();
    }

    private async Task LoadProducts()
    {
        try
        {
            using var scope = Program.ServiceProvider!.CreateScope();
            var productService = scope.ServiceProvider.GetRequiredService<ISupplierProductService>();

            var products = await productService.GetProductsBySupplierIdAsync(_currentUser.SupplierId!.Value);

            dgvProducts.DataSource = products.Select(p => new
            {
                p.SupplierProductId,
                p.Name,
                p.Brand,
                p.Category,
                WholesalePrice = $"RM{p.WholesalePrice:F2}",
                p.AvailableStock,
                p.MinimumOrderQuantity,
                Status = p.IsActive ? "Active" : "Inactive"
            }).ToList();

            if (dgvProducts.Columns.Contains("SupplierProductId"))
                dgvProducts.Columns["SupplierProductId"]!.Visible = false;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading products: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task LoadOrders()
    {
        try
        {
            using var scope = Program.ServiceProvider!.CreateScope();
            var orderService = scope.ServiceProvider.GetRequiredService<ISupplierOrderService>();

            var orders = cmbOrderStatus.SelectedIndex == 0
                ? await orderService.GetOrdersBySupplierIdAsync(_currentUser.SupplierId!.Value)
                : await orderService.GetOrdersByStatusAsync(_currentUser.SupplierId!.Value, cmbOrderStatus.SelectedItem!.ToString()!);

            dgvOrders.DataSource = orders.Select(o => new
            {
                o.SupplierOrderId,
                o.OrderNumber,
                OrderDate = o.OrderDate.ToString("yyyy-MM-dd"),
                BusinessName = o.Business.BusinessName,
                o.Status,
                TotalAmount = $"RM{o.TotalAmount:F2}",
                ItemCount = o.Items.Count
            }).ToList();

            if (dgvOrders.Columns.Contains("SupplierOrderId"))
                dgvOrders.Columns["SupplierOrderId"]!.Visible = false;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading orders: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task LoadReports()
    {
        try
        {
            using var scope = Program.ServiceProvider!.CreateScope();
            var orderService = scope.ServiceProvider.GetRequiredService<ISupplierOrderService>();
            var productService = scope.ServiceProvider.GetRequiredService<ISupplierProductService>();

            var orders = await orderService.GetOrdersBySupplierIdAsync(_currentUser.SupplierId!.Value);
            var totalSales = await orderService.GetTotalSalesBySupplierIdAsync(_currentUser.SupplierId!.Value);
            var products = await productService.GetActiveProductsBySupplierIdAsync(_currentUser.SupplierId!.Value);
            var statusSummary = await orderService.GetOrderStatusSummaryAsync(_currentUser.SupplierId!.Value);

            lblTotalOrders.Text = $"Total Orders: {orders.Count()}";
            lblPendingOrders.Text = $"Pending Orders: {statusSummary.GetValueOrDefault("Pending", 0)}";
            lblTotalSales.Text = $"Total Sales: RM{totalSales:F2}";
            lblProductCount.Text = $"Active Products: {products.Count()}";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading reports: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnAddProduct_Click(object? sender, EventArgs e)
    {
        ShowSupplierProductDialog(null);
    }

    private void BtnEditProduct_Click(object? sender, EventArgs e)
    {
        if (dgvProducts.SelectedRows.Count == 0)
        {
            MessageBox.Show("Please select a product to edit.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        
        var productId = (int)dgvProducts.SelectedRows[0].Cells["SupplierProductId"].Value;
        using var scope = Program.ServiceProvider!.CreateScope();
        var productService = scope.ServiceProvider.GetRequiredService<ISupplierProductService>();
        var product = productService.GetProductByIdAsync(productId).Result;
        
        if (product != null)
        {
            ShowSupplierProductDialog(product);
        }
    }
    
    private void ShowSupplierProductDialog(SupplierProduct? product)
    {
        var isEdit = product != null;
        var form = new Form
        {
            Text = isEdit ? "Edit Product" : "Add New Product",
            Size = new Size(500, 650),
            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false
        };
        
        var y = 20;
        var labelWidth = 130;
        var fieldWidth = 300;
        
        // Name
        form.Controls.Add(new Label { Text = "Product Name:", Location = new Point(20, y), Size = new Size(labelWidth, 20) });
        var txtName = new TextBox { Location = new Point(160, y), Size = new Size(fieldWidth, 25), Text = product?.Name ?? "" };
        form.Controls.Add(txtName);
        y += 35;
        
        // Brand
        form.Controls.Add(new Label { Text = "Brand:", Location = new Point(20, y), Size = new Size(labelWidth, 20) });
        var txtBrand = new TextBox { Location = new Point(160, y), Size = new Size(fieldWidth, 25), Text = product?.Brand ?? "" };
        form.Controls.Add(txtBrand);
        y += 35;
        
        // Description
        form.Controls.Add(new Label { Text = "Description:", Location = new Point(20, y), Size = new Size(labelWidth, 20) });
        var txtDescription = new TextBox { Location = new Point(160, y), Size = new Size(fieldWidth, 50), Multiline = true, Text = product?.Description ?? "" };
        form.Controls.Add(txtDescription);
        y += 65;
        
        // Category
        form.Controls.Add(new Label { Text = "Category:", Location = new Point(20, y), Size = new Size(labelWidth, 20) });
        var txtCategory = new TextBox { Location = new Point(160, y), Size = new Size(fieldWidth, 25), Text = product?.Category ?? "" };
        form.Controls.Add(txtCategory);
        y += 35;
        
        // Barcode
        form.Controls.Add(new Label { Text = "Barcode:", Location = new Point(20, y), Size = new Size(labelWidth, 20) });
        var txtBarcode = new TextBox { Location = new Point(160, y), Size = new Size(fieldWidth, 25), Text = product?.Barcode ?? "" };
        form.Controls.Add(txtBarcode);
        y += 35;
        
        // SKU
        form.Controls.Add(new Label { Text = "SKU:", Location = new Point(20, y), Size = new Size(labelWidth, 20) });
        var txtSKU = new TextBox { Location = new Point(160, y), Size = new Size(fieldWidth, 25), Text = product?.SKU ?? "" };
        form.Controls.Add(txtSKU);
        y += 35;
        
        // Product Image
        form.Controls.Add(new Label { Text = "Product Image:", Location = new Point(20, y), Size = new Size(labelWidth, 20) });
        var txtImagePath = new TextBox { Location = new Point(160, y), Size = new Size(230, 25), Text = product?.ImagePath ?? "", ReadOnly = true };
        form.Controls.Add(txtImagePath);
        var btnBrowseImage = new Button { Text = "Browse...", Location = new Point(395, y), Size = new Size(65, 25) };
        btnBrowseImage.Click += (s, e) =>
        {
            using var openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp|All Files|*.*",
                Title = "Select Product Image"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtImagePath.Text = openFileDialog.FileName;
            }
        };
        form.Controls.Add(btnBrowseImage);
        y += 35;
        
        // Wholesale Price
        form.Controls.Add(new Label { Text = "Wholesale Price (RM):", Location = new Point(20, y), Size = new Size(labelWidth, 20) });
        var nudWholesalePrice = new NumericUpDown { Location = new Point(160, y), Size = new Size(140, 25), DecimalPlaces = 2, Maximum = 100000, Value = product?.WholesalePrice ?? 0 };
        form.Controls.Add(nudWholesalePrice);
        y += 35;
        
        // Minimum Order Quantity
        form.Controls.Add(new Label { Text = "Min Order Quantity:", Location = new Point(20, y), Size = new Size(labelWidth, 20) });
        var nudMinOrder = new NumericUpDown { Location = new Point(160, y), Size = new Size(140, 25), Minimum = 1, Maximum = 100000, Value = product?.MinimumOrderQuantity ?? 1 };
        form.Controls.Add(nudMinOrder);
        y += 35;
        
        // Available Stock
        form.Controls.Add(new Label { Text = "Available Stock:", Location = new Point(20, y), Size = new Size(labelWidth, 20) });
        var nudStock = new NumericUpDown { Location = new Point(160, y), Size = new Size(140, 25), Maximum = 100000, Value = product?.AvailableStock ?? 0 };
        form.Controls.Add(nudStock);
        y += 35;
        
        // Unit
        form.Controls.Add(new Label { Text = "Unit:", Location = new Point(20, y), Size = new Size(labelWidth, 20) });
        var txtUnit = new TextBox { Location = new Point(160, y), Size = new Size(140, 25), Text = product?.Unit ?? "pcs" };
        form.Controls.Add(txtUnit);
        y += 35;
        
        // Active Status
        var chkActive = new CheckBox { Text = "Active (available for ordering)", Location = new Point(160, y), Size = new Size(250, 25), Checked = product?.IsActive ?? true };
        form.Controls.Add(chkActive);
        y += 45;
        
        // Buttons
        var btnSave = new Button
        {
            Text = isEdit ? "Update" : "Add",
            Location = new Point(160, y),
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
            
            if (nudWholesalePrice.Value <= 0)
            {
                MessageBox.Show("Wholesale price must be greater than zero!", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            try
            {
                var productData = new SupplierProduct
                {
                    SupplierProductId = product?.SupplierProductId ?? 0,
                    Name = txtName.Text.Trim(),
                    Brand = txtBrand.Text.Trim(),
                    Description = txtDescription.Text.Trim(),
                    Category = txtCategory.Text.Trim(),
                    Barcode = txtBarcode.Text.Trim(),
                    SKU = txtSKU.Text.Trim(),
                    ImagePath = txtImagePath.Text,
                    WholesalePrice = nudWholesalePrice.Value,
                    MinimumOrderQuantity = (int)nudMinOrder.Value,
                    AvailableStock = (int)nudStock.Value,
                    Unit = txtUnit.Text.Trim(),
                    SupplierId = _currentUser.SupplierId!.Value,
                    IsActive = chkActive.Checked,
                    LastUpdatedDate = DateTime.UtcNow
                };
                
                using var scope = Program.ServiceProvider!.CreateScope();
                var productService = scope.ServiceProvider.GetRequiredService<ISupplierProductService>();
                
                if (isEdit)
                {
                    productData.CreatedDate = product!.CreatedDate;
                    await productService.UpdateProductAsync(productData);
                    MessageBox.Show("Product updated successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    await productService.CreateProductAsync(productData);
                    MessageBox.Show("Product added successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                
                await LoadProducts();
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
            Location = new Point(270, y),
            Size = new Size(100, 35)
        };
        btnCancel.Click += (s, e) => form.Close();
        
        form.Controls.Add(btnSave);
        form.Controls.Add(btnCancel);
        
        form.ShowDialog();
    }

    private async void BtnDeleteProduct_Click(object? sender, EventArgs e)
    {
        if (dgvProducts.SelectedRows.Count == 0)
        {
            MessageBox.Show("Please select a product to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var result = MessageBox.Show("Are you sure you want to delete this product?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (result == DialogResult.Yes)
        {
            try
            {
                var productId = (int)dgvProducts.SelectedRows[0].Cells["SupplierProductId"].Value;
                using var scope = Program.ServiceProvider!.CreateScope();
                var productService = scope.ServiceProvider.GetRequiredService<ISupplierProductService>();
                await productService.DeleteProductAsync(productId);
                await LoadProducts();
                MessageBox.Show("Product deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting product: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private async void BtnRefreshProducts_Click(object? sender, EventArgs e)
    {
        await LoadProducts();
    }

    private async void TxtSearchProduct_TextChanged(object? sender, EventArgs e)
    {
        // Simple search implementation - filter the existing data
        if (string.IsNullOrWhiteSpace(txtSearchProduct.Text))
        {
            await LoadProducts();
        }
    }

    private void BtnViewOrder_Click(object? sender, EventArgs e)
    {
        if (dgvOrders.SelectedRows.Count == 0)
        {
            MessageBox.Show("Please select an order to view.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        MessageBox.Show("View order details - to be implemented with detailed form", "Coming Soon", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private async void BtnUpdateOrderStatus_Click(object? sender, EventArgs e)
    {
        if (dgvOrders.SelectedRows.Count == 0)
        {
            MessageBox.Show("Please select an order to update.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var orderId = (int)dgvOrders.SelectedRows[0].Cells["SupplierOrderId"].Value;
        var currentStatus = dgvOrders.SelectedRows[0].Cells["Status"].Value.ToString();

        var statusForm = new Form
        {
            Text = "Update Order Status",
            Size = new Size(350, 200),
            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false
        };

        var lblStatus = new Label
        {
            Text = "New Status:",
            Location = new Point(20, 30),
            Size = new Size(80, 25)
        };

        var cmbStatus = new ComboBox
        {
            Location = new Point(110, 30),
            Size = new Size(200, 25),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cmbStatus.Items.AddRange(new object[] { "Confirmed", "Preparing", "Shipped", "Delivered", "Cancelled" });
        cmbStatus.SelectedItem = currentStatus;

        var btnUpdate = new Button
        {
            Text = "Update",
            Location = new Point(110, 80),
            Size = new Size(90, 35)
        };

        var btnCancel = new Button
        {
            Text = "Cancel",
            Location = new Point(220, 80),
            Size = new Size(90, 35),
            DialogResult = DialogResult.Cancel
        };

        btnUpdate.Click += async (s, ev) =>
        {
            try
            {
                using var scope = Program.ServiceProvider!.CreateScope();
                var orderService = scope.ServiceProvider.GetRequiredService<ISupplierOrderService>();
                await orderService.UpdateOrderStatusAsync(orderId, cmbStatus.SelectedItem!.ToString()!);
                statusForm.DialogResult = DialogResult.OK;
                statusForm.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating status: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        };

        statusForm.Controls.AddRange(new Control[] { lblStatus, cmbStatus, btnUpdate, btnCancel });
        
        if (statusForm.ShowDialog() == DialogResult.OK)
        {
            await LoadOrders();
            await LoadReports();
            MessageBox.Show("Order status updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private async void BtnRefreshOrders_Click(object? sender, EventArgs e)
    {
        await LoadOrders();
    }

    private async void CmbOrderStatus_SelectedIndexChanged(object? sender, EventArgs e)
    {
        await LoadOrders();
    }

    private void BtnGenerateReport_Click(object? sender, EventArgs e)
    {
        MessageBox.Show("Detailed report generation - to be implemented", "Coming Soon", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
