using Microsoft.Extensions.DependencyInjection;
using RetiSusun.Core.Interfaces;
using RetiSusun.Data.Models;

namespace RetiSusun.Desktop.Forms;

public partial class LoginForm : Form
{
    private TextBox txtUsername = null!;
    private TextBox txtPassword = null!;
    private Button btnLogin = null!;
    private Button btnRegister = null!;
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
        this.Size = new Size(400, 300);
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
            Size = new Size(80, 30)
        };
        btnLogin.Click += BtnLogin_Click;

        // Register Button
        btnRegister = new Button
        {
            Text = "Register",
            Location = new Point(250, 170),
            Size = new Size(80, 30)
        };
        btnRegister.Click += BtnRegister_Click;

        // Add controls
        this.Controls.Add(lblTitle);
        this.Controls.Add(lblUsername);
        this.Controls.Add(txtUsername);
        this.Controls.Add(lblPassword);
        this.Controls.Add(txtPassword);
        this.Controls.Add(btnLogin);
        this.Controls.Add(btnRegister);

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
                var mainForm = new MainForm(user);
                mainForm.FormClosed += (s, args) => this.Close();
                mainForm.Show();
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

    private void BtnRegister_Click(object? sender, EventArgs e)
    {
        var registerForm = new RegistrationForm();
        registerForm.ShowDialog();
    }
}
