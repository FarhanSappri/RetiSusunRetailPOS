using Microsoft.Extensions.DependencyInjection;
using RetiSusun.Core.Interfaces;
using RetiSusun.Data;
using RetiSusun.Data.Models;

namespace RetiSusun.Desktop.Forms;

public partial class RegistrationForm : Form
{
    private TextBox txtBusinessName = null!;
    private TextBox txtBusinessAddress = null!;
    private TextBox txtBusinessPhone = null!;
    private TextBox txtOwnerName = null!;
    private TextBox txtOwnerEmail = null!;
    private TextBox txtOwnerPhone = null!;
    private TextBox txtUsername = null!;
    private TextBox txtPassword = null!;
    private TextBox txtConfirmPassword = null!;
    private Button btnRegister = null!;
    private Button btnCancel = null!;

    public RegistrationForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.Text = "RetiSusun - Business Registration";
        this.Size = new Size(500, 600);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;

        int yPos = 20;
        int labelX = 30;
        int textX = 180;
        int spacing = 40;

        // Business Information
        var lblBusinessSection = new Label
        {
            Text = "Business Information",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Location = new Point(labelX, yPos),
            Size = new Size(250, 25)
        };
        this.Controls.Add(lblBusinessSection);
        yPos += 35;

        AddLabelAndTextBox("Business Name:", ref txtBusinessName, ref yPos, labelX, textX, spacing);
        AddLabelAndTextBox("Address:", ref txtBusinessAddress, ref yPos, labelX, textX, spacing);
        AddLabelAndTextBox("Phone:", ref txtBusinessPhone, ref yPos, labelX, textX, spacing);

        // Owner Information
        var lblOwnerSection = new Label
        {
            Text = "Owner Information",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Location = new Point(labelX, yPos),
            Size = new Size(250, 25)
        };
        this.Controls.Add(lblOwnerSection);
        yPos += 35;

        AddLabelAndTextBox("Owner Name:", ref txtOwnerName, ref yPos, labelX, textX, spacing);
        AddLabelAndTextBox("Owner Email:", ref txtOwnerEmail, ref yPos, labelX, textX, spacing);
        AddLabelAndTextBox("Owner Phone:", ref txtOwnerPhone, ref yPos, labelX, textX, spacing);

        // Login Credentials
        var lblLoginSection = new Label
        {
            Text = "Login Credentials",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Location = new Point(labelX, yPos),
            Size = new Size(250, 25)
        };
        this.Controls.Add(lblLoginSection);
        yPos += 35;

        AddLabelAndTextBox("Username:", ref txtUsername, ref yPos, labelX, textX, spacing);
        AddLabelAndTextBox("Password:", ref txtPassword, ref yPos, labelX, textX, spacing, true);
        AddLabelAndTextBox("Confirm Password:", ref txtConfirmPassword, ref yPos, labelX, textX, spacing, true);

        // Buttons
        btnRegister = new Button
        {
            Text = "Register",
            Location = new Point(200, yPos + 10),
            Size = new Size(100, 35)
        };
        btnRegister.Click += BtnRegister_Click;

        btnCancel = new Button
        {
            Text = "Cancel",
            Location = new Point(320, yPos + 10),
            Size = new Size(100, 35)
        };
        btnCancel.Click += (s, e) => this.Close();

        this.Controls.Add(btnRegister);
        this.Controls.Add(btnCancel);
    }

    private void AddLabelAndTextBox(string labelText, ref TextBox textBox, ref int yPos, int labelX, int textX, int spacing, bool isPassword = false)
    {
        var label = new Label
        {
            Text = labelText,
            Location = new Point(labelX, yPos),
            Size = new Size(140, 25)
        };

        textBox = new TextBox
        {
            Location = new Point(textX, yPos),
            Size = new Size(250, 25)
        };

        if (isPassword)
            textBox.PasswordChar = '*';

        this.Controls.Add(label);
        this.Controls.Add(textBox);
        yPos += spacing;
    }

    private async void BtnRegister_Click(object? sender, EventArgs e)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(txtBusinessName.Text) ||
            string.IsNullOrWhiteSpace(txtOwnerName.Text) ||
            string.IsNullOrWhiteSpace(txtUsername.Text) ||
            string.IsNullOrWhiteSpace(txtPassword.Text))
        {
            MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (txtPassword.Text != txtConfirmPassword.Text)
        {
            MessageBox.Show("Passwords do not match.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            using var scope = Program.ServiceProvider!.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<RetiSusunDbContext>();
            var authService = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();

            // Create business
            var business = new Business
            {
                BusinessName = txtBusinessName.Text,
                Address = txtBusinessAddress.Text,
                Phone = txtBusinessPhone.Text,
                OwnerName = txtOwnerName.Text,
                OwnerEmail = txtOwnerEmail.Text,
                OwnerPhone = txtOwnerPhone.Text,
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };

            dbContext.Businesses.Add(business);
            await dbContext.SaveChangesAsync();

            // Create admin user
            await authService.RegisterUserAsync(
                txtUsername.Text,
                txtPassword.Text,
                txtOwnerName.Text,
                txtOwnerEmail.Text ?? "",
                "Admin",
                business.BusinessId
            );

            MessageBox.Show("Registration successful! You can now login.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error during registration: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
