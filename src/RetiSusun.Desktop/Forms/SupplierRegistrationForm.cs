using Microsoft.Extensions.DependencyInjection;
using RetiSusun.Core.Interfaces;
using RetiSusun.Data.Models;

namespace RetiSusun.Desktop.Forms;

public partial class SupplierRegistrationForm : Form
{
    private TextBox txtCompanyName = null!;
    private TextBox txtAddress = null!;
    private TextBox txtPhone = null!;
    private TextBox txtEmail = null!;
    private TextBox txtRegistrationNumber = null!;
    private TextBox txtContactPersonName = null!;
    private TextBox txtContactPersonEmail = null!;
    private TextBox txtContactPersonPhone = null!;
    private TextBox txtDescription = null!;
    private TextBox txtUsername = null!;
    private TextBox txtPassword = null!;
    private TextBox txtConfirmPassword = null!;
    private Button btnRegister = null!;
    private Button btnCancel = null!;
    private Label lblTitle = null!;

    public SupplierRegistrationForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.Text = "RetiSusun - Supplier Registration";
        this.Size = new Size(600, 700);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.AutoScroll = true;

        int y = 20;

        // Title
        lblTitle = new Label
        {
            Text = "Supplier Registration",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            Location = new Point(180, y),
            Size = new Size(250, 35),
            TextAlign = ContentAlignment.MiddleCenter
        };
        y += 50;

        // Company Information Section
        AddSectionLabel("Company Information", ref y);

        AddField("Company Name:", ref y, out txtCompanyName);
        AddField("Registration Number:", ref y, out txtRegistrationNumber);
        AddMultilineField("Address:", ref y, out txtAddress, 60);
        AddField("Phone:", ref y, out txtPhone);
        AddField("Email:", ref y, out txtEmail);
        AddMultilineField("Description:", ref y, out txtDescription, 60);

        // Contact Person Section
        y += 10;
        AddSectionLabel("Contact Person", ref y);
        AddField("Name:", ref y, out txtContactPersonName);
        AddField("Email:", ref y, out txtContactPersonEmail);
        AddField("Phone:", ref y, out txtContactPersonPhone);

        // Admin Account Section
        y += 10;
        AddSectionLabel("Admin Account", ref y);
        AddField("Username:", ref y, out txtUsername);
        AddField("Password:", ref y, out txtPassword, true);
        AddField("Confirm Password:", ref y, out txtConfirmPassword, true);

        // Buttons
        y += 20;
        btnRegister = new Button
        {
            Text = "Register",
            Location = new Point(200, y),
            Size = new Size(90, 35)
        };
        btnRegister.Click += BtnRegister_Click;

        btnCancel = new Button
        {
            Text = "Cancel",
            Location = new Point(310, y),
            Size = new Size(90, 35)
        };
        btnCancel.Click += BtnCancel_Click;

        // Add all controls
        this.Controls.AddRange(new Control[] {
            lblTitle, txtCompanyName, txtAddress, txtPhone, txtEmail,
            txtRegistrationNumber, txtContactPersonName, txtContactPersonEmail,
            txtContactPersonPhone, txtDescription, txtUsername, txtPassword,
            txtConfirmPassword, btnRegister, btnCancel
        });
    }

    private void AddSectionLabel(string text, ref int y)
    {
        var label = new Label
        {
            Text = text,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Location = new Point(50, y),
            Size = new Size(500, 25)
        };
        this.Controls.Add(label);
        y += 35;
    }

    private void AddField(string labelText, ref int y, out TextBox textBox, bool isPassword = false)
    {
        var label = new Label
        {
            Text = labelText,
            Location = new Point(50, y + 3),
            Size = new Size(150, 25)
        };
        this.Controls.Add(label);

        textBox = new TextBox
        {
            Location = new Point(210, y),
            Size = new Size(300, 25)
        };
        if (isPassword)
            textBox.PasswordChar = '*';

        y += 35;
    }

    private void AddMultilineField(string labelText, ref int y, out TextBox textBox, int height)
    {
        var label = new Label
        {
            Text = labelText,
            Location = new Point(50, y + 3),
            Size = new Size(150, 25)
        };
        this.Controls.Add(label);

        textBox = new TextBox
        {
            Location = new Point(210, y),
            Size = new Size(300, height),
            Multiline = true,
            ScrollBars = ScrollBars.Vertical
        };

        y += height + 10;
    }

    private async void BtnRegister_Click(object? sender, EventArgs e)
    {
        try
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(txtCompanyName.Text))
            {
                MessageBox.Show("Company name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtContactPersonName.Text))
            {
                MessageBox.Show("Contact person name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Username is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Password is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Passwords do not match.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnRegister.Enabled = false;
            btnRegister.Text = "Registering...";

            var supplierService = Program.ServiceProvider!.GetRequiredService<ISupplierService>();
            var authService = Program.ServiceProvider!.GetRequiredService<IAuthenticationService>();

            // Create supplier
            var supplier = new Supplier
            {
                CompanyName = txtCompanyName.Text,
                Address = txtAddress.Text,
                Phone = txtPhone.Text,
                Email = txtEmail.Text,
                RegistrationNumber = txtRegistrationNumber.Text,
                ContactPersonName = txtContactPersonName.Text,
                ContactPersonEmail = txtContactPersonEmail.Text,
                ContactPersonPhone = txtContactPersonPhone.Text,
                Description = txtDescription.Text,
                IsActive = true,
                IsOpenForBusiness = true
            };

            supplier = await supplierService.CreateSupplierAsync(supplier);

            // Create admin user
            await authService.RegisterSupplierUserAsync(
                txtUsername.Text,
                txtPassword.Text,
                txtContactPersonName.Text,
                txtContactPersonEmail.Text,
                "Admin",
                supplier.SupplierId
            );

            MessageBox.Show("Supplier registered successfully! You can now login.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Registration failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            btnRegister.Enabled = true;
            btnRegister.Text = "Register";
        }
    }

    private void BtnCancel_Click(object? sender, EventArgs e)
    {
        this.DialogResult = DialogResult.Cancel;
        this.Close();
    }
}
