using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using RetiSusun.Core.Interfaces;
using RetiSusun.Data;
using RetiSusun.Data.Models;
using System.Security.Cryptography;
using System.Text;

namespace RetiSusun.Desktop.Forms;

public partial class UserSettingsForm : Form
{
    private readonly User _currentUser;
    private TabControl tabControl = null!;
    
    // Profile tab controls
    private TextBox txtFullName = null!;
    private TextBox txtEmail = null!;
    private TextBox txtUsername = null!;
    
    // Business/Supplier info tab controls
    private TextBox txtBusinessName = null!;
    private TextBox txtAddress = null!;
    private TextBox txtPhone = null!;
    private TextBox txtRegistrationNumber = null!;
    private TextBox txtContactPerson = null!;
    private TextBox txtContactEmail = null!;
    private TextBox txtContactPhone = null!;
    private TextBox txtDescription = null!;
    private TextBox txtLogoPath = null!;
    
    // Password tab controls
    private TextBox txtCurrentPassword = null!;
    private TextBox txtNewPassword = null!;
    private TextBox txtConfirmPassword = null!;
    
    // Preferences tab controls
    private CheckBox chkDarkMode = null!;
    
    public UserSettingsForm(User user)
    {
        _currentUser = user;
        InitializeComponent();
        LoadUserData();
    }
    
    private void InitializeComponent()
    {
        this.Text = "User Settings";
        this.Size = new Size(700, 550);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        
        // Tab Control
        tabControl = new TabControl
        {
            Location = new Point(10, 10),
            Size = new Size(670, 450)
        };
        
        // Profile Tab
        var profileTab = new TabPage("Profile");
        InitializeProfileTab(profileTab);
        tabControl.TabPages.Add(profileTab);
        
        // Business/Supplier Info Tab
        var orgTab = new TabPage(_currentUser.AccountType == "Business" ? "Business Info" : "Supplier Info");
        InitializeOrganizationTab(orgTab);
        tabControl.TabPages.Add(orgTab);
        
        // Password Tab
        var passwordTab = new TabPage("Change Password");
        InitializePasswordTab(passwordTab);
        tabControl.TabPages.Add(passwordTab);
        
        // Preferences Tab
        var preferencesTab = new TabPage("Preferences");
        InitializePreferencesTab(preferencesTab);
        tabControl.TabPages.Add(preferencesTab);
        
        this.Controls.Add(tabControl);
        
        // Save Button
        var btnSave = new Button
        {
            Text = "Save Changes",
            Location = new Point(480, 470),
            Size = new Size(120, 35),
            BackColor = Color.LightGreen
        };
        btnSave.Click += BtnSave_Click;
        
        var btnCancel = new Button
        {
            Text = "Cancel",
            Location = new Point(610, 470),
            Size = new Size(70, 35)
        };
        btnCancel.Click += (s, e) => this.Close();
        
        this.Controls.Add(btnSave);
        this.Controls.Add(btnCancel);
    }
    
    private void InitializeProfileTab(TabPage tab)
    {
        var y = 20;
        
        tab.Controls.Add(new Label { Text = "Full Name:", Location = new Point(20, y), Size = new Size(120, 20) });
        txtFullName = new TextBox { Location = new Point(150, y), Size = new Size(300, 25) };
        tab.Controls.Add(txtFullName);
        y += 40;
        
        tab.Controls.Add(new Label { Text = "Email:", Location = new Point(20, y), Size = new Size(120, 20) });
        txtEmail = new TextBox { Location = new Point(150, y), Size = new Size(300, 25) };
        tab.Controls.Add(txtEmail);
        y += 40;
        
        tab.Controls.Add(new Label { Text = "Username:", Location = new Point(20, y), Size = new Size(120, 20) });
        txtUsername = new TextBox { Location = new Point(150, y), Size = new Size(300, 25), ReadOnly = true, BackColor = Color.LightGray };
        tab.Controls.Add(txtUsername);
        y += 40;
        
        tab.Controls.Add(new Label { Text = "Role:", Location = new Point(20, y), Size = new Size(120, 20) });
        var lblRole = new Label { Text = _currentUser.Role, Location = new Point(150, y), Size = new Size(300, 20), ForeColor = Color.Blue };
        tab.Controls.Add(lblRole);
        y += 40;
        
        tab.Controls.Add(new Label { Text = "Account Type:", Location = new Point(20, y), Size = new Size(120, 20) });
        var lblAccountType = new Label { Text = _currentUser.AccountType, Location = new Point(150, y), Size = new Size(300, 20), ForeColor = Color.Blue };
        tab.Controls.Add(lblAccountType);
    }
    
    private void InitializeOrganizationTab(TabPage tab)
    {
        var y = 20;
        var isBusiness = _currentUser.AccountType == "Business";
        
        tab.Controls.Add(new Label { Text = isBusiness ? "Business Name:" : "Company Name:", Location = new Point(20, y), Size = new Size(130, 20) });
        txtBusinessName = new TextBox { Location = new Point(160, y), Size = new Size(400, 25) };
        tab.Controls.Add(txtBusinessName);
        y += 35;
        
        tab.Controls.Add(new Label { Text = "Address:", Location = new Point(20, y), Size = new Size(130, 20) });
        txtAddress = new TextBox { Location = new Point(160, y), Size = new Size(400, 50), Multiline = true };
        tab.Controls.Add(txtAddress);
        y += 60;
        
        tab.Controls.Add(new Label { Text = "Phone:", Location = new Point(20, y), Size = new Size(130, 20) });
        txtPhone = new TextBox { Location = new Point(160, y), Size = new Size(200, 25) };
        tab.Controls.Add(txtPhone);
        y += 35;
        
        tab.Controls.Add(new Label { Text = "Registration #:", Location = new Point(20, y), Size = new Size(130, 20) });
        txtRegistrationNumber = new TextBox { Location = new Point(160, y), Size = new Size(200, 25) };
        tab.Controls.Add(txtRegistrationNumber);
        y += 35;
        
        if (!isBusiness)
        {
            tab.Controls.Add(new Label { Text = "Contact Person:", Location = new Point(20, y), Size = new Size(130, 20) });
            txtContactPerson = new TextBox { Location = new Point(160, y), Size = new Size(400, 25) };
            tab.Controls.Add(txtContactPerson);
            y += 35;
            
            tab.Controls.Add(new Label { Text = "Contact Email:", Location = new Point(20, y), Size = new Size(130, 20) });
            txtContactEmail = new TextBox { Location = new Point(160, y), Size = new Size(400, 25) };
            tab.Controls.Add(txtContactEmail);
            y += 35;
            
            tab.Controls.Add(new Label { Text = "Contact Phone:", Location = new Point(20, y), Size = new Size(130, 20) });
            txtContactPhone = new TextBox { Location = new Point(160, y), Size = new Size(200, 25) };
            tab.Controls.Add(txtContactPhone);
            y += 35;
            
            tab.Controls.Add(new Label { Text = "Description:", Location = new Point(20, y), Size = new Size(130, 20) });
            txtDescription = new TextBox { Location = new Point(160, y), Size = new Size(400, 50), Multiline = true };
            tab.Controls.Add(txtDescription);
            y += 60;
            
            tab.Controls.Add(new Label { Text = "Logo:", Location = new Point(20, y), Size = new Size(130, 20) });
            txtLogoPath = new TextBox { Location = new Point(160, y), Size = new Size(330, 25), ReadOnly = true };
            tab.Controls.Add(txtLogoPath);
            var btnBrowseLogo = new Button { Text = "Browse...", Location = new Point(495, y), Size = new Size(65, 25) };
            btnBrowseLogo.Click += (s, e) =>
            {
                using var openFileDialog = new OpenFileDialog
                {
                    Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp|All Files|*.*",
                    Title = "Select Company Logo"
                };
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtLogoPath.Text = openFileDialog.FileName;
                }
            };
            tab.Controls.Add(btnBrowseLogo);
        }
    }
    
    private void InitializePasswordTab(TabPage tab)
    {
        var y = 20;
        
        var lblInfo = new Label
        {
            Text = "Leave password fields blank if you don't want to change your password.",
            Location = new Point(20, y),
            Size = new Size(600, 40),
            Font = new Font("Segoe UI", 9, FontStyle.Italic),
            ForeColor = Color.Gray
        };
        tab.Controls.Add(lblInfo);
        y += 50;
        
        tab.Controls.Add(new Label { Text = "Current Password:", Location = new Point(20, y), Size = new Size(150, 20) });
        txtCurrentPassword = new TextBox { Location = new Point(180, y), Size = new Size(300, 25), PasswordChar = '*' };
        tab.Controls.Add(txtCurrentPassword);
        y += 40;
        
        tab.Controls.Add(new Label { Text = "New Password:", Location = new Point(20, y), Size = new Size(150, 20) });
        txtNewPassword = new TextBox { Location = new Point(180, y), Size = new Size(300, 25), PasswordChar = '*' };
        tab.Controls.Add(txtNewPassword);
        y += 40;
        
        tab.Controls.Add(new Label { Text = "Confirm Password:", Location = new Point(20, y), Size = new Size(150, 20) });
        txtConfirmPassword = new TextBox { Location = new Point(180, y), Size = new Size(300, 25), PasswordChar = '*' };
        tab.Controls.Add(txtConfirmPassword);
    }
    
    private void InitializePreferencesTab(TabPage tab)
    {
        var y = 20;
        
        var lblAppearance = new Label
        {
            Text = "Appearance",
            Location = new Point(20, y),
            Size = new Size(200, 25),
            Font = new Font("Segoe UI", 12, FontStyle.Bold)
        };
        tab.Controls.Add(lblAppearance);
        y += 40;
        
        chkDarkMode = new CheckBox
        {
            Text = "Enable Dark Mode",
            Location = new Point(40, y),
            Size = new Size(400, 25)
        };
        tab.Controls.Add(chkDarkMode);
        y += 35;
        
        var lblDarkModeInfo = new Label
        {
            Text = "Note: Dark mode will be applied after restarting the application.",
            Location = new Point(60, y),
            Size = new Size(500, 40),
            Font = new Font("Segoe UI", 9, FontStyle.Italic),
            ForeColor = Color.Gray
        };
        tab.Controls.Add(lblDarkModeInfo);
    }
    
    private async void LoadUserData()
    {
        try
        {
            using var scope = Program.ServiceProvider!.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RetiSusunDbContext>();
            
            // Load user data
            var user = await context.Users
                .Include(u => u.Business)
                .Include(u => u.Supplier)
                .FirstOrDefaultAsync(u => u.UserId == _currentUser.UserId);
            
            if (user == null) return;
            
            // Profile data
            txtFullName.Text = user.FullName;
            txtEmail.Text = user.Email ?? "";
            txtUsername.Text = user.Username;
            chkDarkMode.Checked = user.DarkModeEnabled;
            
            // Organization data
            if (_currentUser.AccountType == "Business" && user.Business != null)
            {
                txtBusinessName.Text = user.Business.BusinessName;
                txtAddress.Text = user.Business.Address ?? "";
                txtPhone.Text = user.Business.Phone ?? "";
                txtRegistrationNumber.Text = user.Business.RegistrationNumber ?? "";
            }
            else if (_currentUser.AccountType == "Supplier" && user.Supplier != null)
            {
                txtBusinessName.Text = user.Supplier.CompanyName;
                txtAddress.Text = user.Supplier.Address ?? "";
                txtPhone.Text = user.Supplier.Phone ?? "";
                txtRegistrationNumber.Text = user.Supplier.RegistrationNumber ?? "";
                if (txtContactPerson != null) txtContactPerson.Text = user.Supplier.ContactPersonName ?? "";
                if (txtContactEmail != null) txtContactEmail.Text = user.Supplier.ContactPersonEmail ?? "";
                if (txtContactPhone != null) txtContactPhone.Text = user.Supplier.ContactPersonPhone ?? "";
                if (txtDescription != null) txtDescription.Text = user.Supplier.Description ?? "";
                if (txtLogoPath != null) txtLogoPath.Text = user.Supplier.LogoPath ?? "";
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading user data: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    
    private async void BtnSave_Click(object? sender, EventArgs e)
    {
        try
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Full name is required.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            // Validate password change if attempted
            if (!string.IsNullOrWhiteSpace(txtCurrentPassword.Text) || 
                !string.IsNullOrWhiteSpace(txtNewPassword.Text) || 
                !string.IsNullOrWhiteSpace(txtConfirmPassword.Text))
            {
                if (string.IsNullOrWhiteSpace(txtCurrentPassword.Text))
                {
                    MessageBox.Show("Please enter your current password.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                if (string.IsNullOrWhiteSpace(txtNewPassword.Text))
                {
                    MessageBox.Show("Please enter a new password.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                if (txtNewPassword.Text != txtConfirmPassword.Text)
                {
                    MessageBox.Show("New password and confirmation do not match.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                if (txtNewPassword.Text.Length < 6)
                {
                    MessageBox.Show("Password must be at least 6 characters long.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            
            using var scope = Program.ServiceProvider!.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RetiSusunDbContext>();
            
            // Load user with related entities
            var user = await context.Users
                .Include(u => u.Business)
                .Include(u => u.Supplier)
                .FirstOrDefaultAsync(u => u.UserId == _currentUser.UserId);
            
            if (user == null)
            {
                MessageBox.Show("User not found!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            // Update user profile
            user.FullName = txtFullName.Text.Trim();
            user.Email = txtEmail.Text.Trim();
            user.DarkModeEnabled = chkDarkMode.Checked;
            
            // Update password if requested
            if (!string.IsNullOrWhiteSpace(txtCurrentPassword.Text))
            {
                var currentPasswordHash = ComputeHash(txtCurrentPassword.Text);
                if (currentPasswordHash != user.PasswordHash)
                {
                    MessageBox.Show("Current password is incorrect.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                
                user.PasswordHash = ComputeHash(txtNewPassword.Text);
            }
            
            // Update organization data
            if (_currentUser.AccountType == "Business" && user.Business != null)
            {
                user.Business.BusinessName = txtBusinessName.Text.Trim();
                user.Business.Address = txtAddress.Text.Trim();
                user.Business.Phone = txtPhone.Text.Trim();
                user.Business.RegistrationNumber = txtRegistrationNumber.Text.Trim();
            }
            else if (_currentUser.AccountType == "Supplier" && user.Supplier != null)
            {
                user.Supplier.CompanyName = txtBusinessName.Text.Trim();
                user.Supplier.Address = txtAddress.Text.Trim();
                user.Supplier.Phone = txtPhone.Text.Trim();
                user.Supplier.RegistrationNumber = txtRegistrationNumber.Text.Trim();
                
                if (txtContactPerson != null)
                    user.Supplier.ContactPersonName = txtContactPerson.Text.Trim();
                if (txtContactEmail != null)
                    user.Supplier.ContactPersonEmail = txtContactEmail.Text.Trim();
                if (txtContactPhone != null)
                    user.Supplier.ContactPersonPhone = txtContactPhone.Text.Trim();
                if (txtDescription != null)
                    user.Supplier.Description = txtDescription.Text.Trim();
                if (txtLogoPath != null)
                    user.Supplier.LogoPath = txtLogoPath.Text;
                
                user.Supplier.LastUpdatedDate = DateTime.UtcNow;
            }
            
            await context.SaveChangesAsync();
            
            // Update current user object
            _currentUser.FullName = user.FullName;
            _currentUser.Email = user.Email;
            _currentUser.DarkModeEnabled = user.DarkModeEnabled;
            
            MessageBox.Show("Settings saved successfully!", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving settings: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    
    private static string ComputeHash(string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
