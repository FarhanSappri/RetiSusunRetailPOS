using RetiSusun.Data.Models;

namespace RetiSusun.Desktop.Forms;

public partial class MainForm : Form
{
    private readonly User _currentUser;
    private TabControl tabControl = null!;
    private Label lblWelcome = null!;

    public MainForm(User user)
    {
        _currentUser = user;
        InitializeComponent();
        lblWelcome.Text = $"Welcome, {_currentUser.FullName} ({_currentUser.Role})";
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
        var panel = new Panel();
        var label = new Label
        {
            Text = "Point of Sale - Coming Soon\n\nThis will include:\n- Barcode scanning\n- Product selection\n- Cart management\n- Payment processing\n- Receipt printing",
            Font = new Font("Segoe UI", 12),
            Location = new Point(50, 50),
            Size = new Size(600, 200)
        };
        panel.Controls.Add(label);
        return panel;
    }

    private Panel CreateProductsPanel()
    {
        var panel = new Panel();
        var label = new Label
        {
            Text = "Product Management - Coming Soon\n\nThis will include:\n- Add/Edit/Delete products\n- Manage inventory levels\n- Set prices\n- Low stock alerts",
            Font = new Font("Segoe UI", 12),
            Location = new Point(50, 50),
            Size = new Size(600, 200)
        };
        panel.Controls.Add(label);
        return panel;
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
