using Microsoft.Extensions.DependencyInjection;
using RetiSusun.Core.Interfaces;
using RetiSusun.Data.Models;

namespace RetiSusun.Desktop.Forms;

public partial class LoginForm : Form
{
    private TextBox txtUsername = null!;
    private TextBox txtPassword = null!;
    private Button btnLogin = null!;
    private Button btnRegisterBusiness = null!;
    private Button btnRegisterSupplier = null!;
    private Label lblTitle = null!;
    private Label lblUsername = null!;
    private Label lblPassword = null!;

    public LoginForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.Text = "RetiSusun - Login";
        this.Size = new Size(400, 330);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;

        // Title
        lblTitle = new Label
        {
            Text = "RetiSusun POS System",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            Location = new Point(80, 20),
            Size = new Size(250, 35),
            TextAlign = ContentAlignment.MiddleCenter
        };

        // Username
        lblUsername = new Label
        {
            Text = "Username:",
            Location = new Point(50, 80),
            Size = new Size(100, 25)
        };

        txtUsername = new TextBox
        {
            Location = new Point(150, 80),
            Size = new Size(180, 25)
        };

        // Password
        lblPassword = new Label
        {
            Text = "Password:",
            Location = new Point(50, 120),
            Size = new Size(100, 25)
        };

        txtPassword = new TextBox
        {
            Location = new Point(150, 120),
            Size = new Size(180, 25),
            PasswordChar = '*'
        };

        // Login Button
        btnLogin = new Button
        {
            Text = "Login",
            Location = new Point(150, 170),
            Size = new Size(180, 35),
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnLogin.Click += BtnLogin_Click;

        // Register Business Button
        btnRegisterBusiness = new Button
        {
            Text = "Register Business",
            Location = new Point(50, 220),
            Size = new Size(135, 30)
        };
        btnRegisterBusiness.Click += BtnRegisterBusiness_Click;

        // Register Supplier Button
        btnRegisterSupplier = new Button
        {
            Text = "Register Supplier",
            Location = new Point(200, 220),
            Size = new Size(135, 30)
        };
        btnRegisterSupplier.Click += BtnRegisterSupplier_Click;

        // Add controls
        this.Controls.Add(lblTitle);
        this.Controls.Add(lblUsername);
        this.Controls.Add(txtUsername);
        this.Controls.Add(lblPassword);
        this.Controls.Add(txtPassword);
        this.Controls.Add(btnLogin);
        this.Controls.Add(btnRegisterBusiness);
        this.Controls.Add(btnRegisterSupplier);

        this.AcceptButton = btnLogin;
    }

    private async void BtnLogin_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
        {
            MessageBox.Show("Please enter username and password.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            using var scope = Program.ServiceProvider!.CreateScope();
            var authService = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();
            
            var user = await authService.AuthenticateAsync(txtUsername.Text, txtPassword.Text);

            if (user != null)
            {
                this.Hide();
                
                // Route to appropriate dashboard based on account type
                if (user.AccountType == "Supplier")
                {
                    var supplierDashboard = new SupplierDashboardForm(user);
                    supplierDashboard.FormClosed += (s, args) => this.Close();
                    supplierDashboard.Show();
                }
                else
                {
                    var mainForm = new MainForm(user);
                    mainForm.FormClosed += (s, args) => this.Close();
                    mainForm.Show();
                }
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error during login: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnRegisterBusiness_Click(object? sender, EventArgs e)
    {
        var registerForm = new RegistrationForm();
        registerForm.ShowDialog();
    }

    private void BtnRegisterSupplier_Click(object? sender, EventArgs e)
    {
        var registerForm = new SupplierRegistrationForm();
        registerForm.ShowDialog();
    }
}
