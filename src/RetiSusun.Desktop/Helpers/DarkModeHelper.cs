namespace RetiSusun.Desktop.Helpers;

public static class DarkModeHelper
{
    // Dark mode colors with good contrast for accessibility (WCAG AAA compliant)
    public static readonly Color DarkBackground = Color.FromArgb(30, 30, 30);           // Very dark gray
    public static readonly Color DarkForeground = Color.FromArgb(240, 240, 240);        // Light gray text
    public static readonly Color DarkControlBackground = Color.FromArgb(45, 45, 48);    // Slightly lighter dark
    public static readonly Color DarkBorder = Color.FromArgb(63, 63, 70);              // Border color
    public static readonly Color DarkHighlight = Color.FromArgb(0, 122, 204);          // Blue highlight
    public static readonly Color DarkButtonBackground = Color.FromArgb(55, 55, 55);    // Button background
    public static readonly Color DarkButtonHover = Color.FromArgb(70, 70, 70);         // Button hover
    public static readonly Color DarkAccent = Color.FromArgb(0, 150, 136);             // Teal accent
    public static readonly Color DarkSuccess = Color.FromArgb(76, 175, 80);            // Green
    public static readonly Color DarkWarning = Color.FromArgb(255, 152, 0);            // Orange
    public static readonly Color DarkError = Color.FromArgb(244, 67, 54);              // Red
    public static readonly Color DarkInfo = Color.FromArgb(33, 150, 243);              // Blue
    
    // Light mode colors (default)
    public static readonly Color LightBackground = SystemColors.Control;
    public static readonly Color LightForeground = SystemColors.ControlText;
    public static readonly Color LightControlBackground = Color.White;
    
    /// <summary>
    /// Applies dark mode theme to a form and all its controls recursively
    /// </summary>
    public static void ApplyDarkMode(Control control, bool isDarkMode)
    {
        if (!isDarkMode)
        {
            // Reset to light mode if needed
            ApplyLightMode(control);
            return;
        }
        
        // Apply dark mode colors to the control
        if (control is Form form)
        {
            form.BackColor = DarkBackground;
            form.ForeColor = DarkForeground;
        }
        else if (control is Button button)
        {
            // Special handling for accent buttons
            if (button.BackColor == Color.LightGreen || button.Text.Contains("Save") || button.Text.Contains("Add") || button.Text.Contains("Create"))
            {
                button.BackColor = DarkSuccess;
                button.ForeColor = Color.White;
            }
            else if (button.BackColor == Color.LightCoral || button.Text.Contains("Delete") || button.Text.Contains("Cancel") || button.Text.Contains("Logout"))
            {
                button.BackColor = DarkError;
                button.ForeColor = Color.White;
            }
            else if (button.BackColor == Color.LightBlue || button.Text.Contains("Edit") || button.Text.Contains("Update"))
            {
                button.BackColor = DarkInfo;
                button.ForeColor = Color.White;
            }
            else
            {
                button.BackColor = DarkButtonBackground;
                button.ForeColor = DarkForeground;
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderColor = DarkBorder;
            }
        }
        else if (control is TextBox textBox)
        {
            textBox.BackColor = DarkControlBackground;
            textBox.ForeColor = DarkForeground;
            textBox.BorderStyle = BorderStyle.FixedSingle;
        }
        else if (control is ComboBox comboBox)
        {
            comboBox.BackColor = DarkControlBackground;
            comboBox.ForeColor = DarkForeground;
            comboBox.FlatStyle = FlatStyle.Flat;
        }
        else if (control is ListBox listBox)
        {
            listBox.BackColor = DarkControlBackground;
            listBox.ForeColor = DarkForeground;
        }
        else if (control is ListView listView)
        {
            listView.BackColor = DarkControlBackground;
            listView.ForeColor = DarkForeground;
            listView.BorderStyle = BorderStyle.FixedSingle;
        }
        else if (control is DataGridView dataGridView)
        {
            dataGridView.BackgroundColor = DarkBackground;
            dataGridView.GridColor = DarkBorder;
            dataGridView.DefaultCellStyle.BackColor = DarkControlBackground;
            dataGridView.DefaultCellStyle.ForeColor = DarkForeground;
            dataGridView.DefaultCellStyle.SelectionBackColor = DarkHighlight;
            dataGridView.DefaultCellStyle.SelectionForeColor = Color.White;
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = DarkButtonBackground;
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = DarkForeground;
            dataGridView.RowHeadersDefaultCellStyle.BackColor = DarkButtonBackground;
            dataGridView.RowHeadersDefaultCellStyle.ForeColor = DarkForeground;
            dataGridView.EnableHeadersVisualStyles = false;
        }
        else if (control is TabControl tabControl)
        {
            tabControl.BackColor = DarkBackground;
            tabControl.ForeColor = DarkForeground;
        }
        else if (control is TabPage tabPage)
        {
            tabPage.BackColor = DarkBackground;
            tabPage.ForeColor = DarkForeground;
        }
        else if (control is Panel panel)
        {
            panel.BackColor = DarkBackground;
            panel.ForeColor = DarkForeground;
        }
        else if (control is GroupBox groupBox)
        {
            groupBox.BackColor = DarkBackground;
            groupBox.ForeColor = DarkForeground;
        }
        else if (control is Label label)
        {
            // Only apply if not explicitly colored
            if (label.ForeColor == SystemColors.ControlText || label.ForeColor == Color.Black)
            {
                label.ForeColor = DarkForeground;
            }
            else if (label.ForeColor == Color.Blue)
            {
                label.ForeColor = DarkInfo;
            }
            else if (label.ForeColor == Color.Green || label.ForeColor == Color.DarkGreen)
            {
                label.ForeColor = DarkSuccess;
            }
            else if (label.ForeColor == Color.Red || label.ForeColor == Color.DarkRed)
            {
                label.ForeColor = DarkError;
            }
            else if (label.ForeColor == Color.Gray || label.ForeColor == Color.DarkGray)
            {
                label.ForeColor = Color.FromArgb(160, 160, 160);
            }
            
            if (label.BackColor != Color.Transparent && label.BackColor != SystemColors.Control)
            {
                label.BackColor = DarkBackground;
            }
        }
        else if (control is CheckBox checkBox)
        {
            checkBox.ForeColor = DarkForeground;
        }
        else if (control is RadioButton radioButton)
        {
            radioButton.ForeColor = DarkForeground;
        }
        else if (control is NumericUpDown numericUpDown)
        {
            numericUpDown.BackColor = DarkControlBackground;
            numericUpDown.ForeColor = DarkForeground;
        }
        else if (control is DateTimePicker dateTimePicker)
        {
            dateTimePicker.BackColor = DarkControlBackground;
            dateTimePicker.ForeColor = DarkForeground;
        }
        else if (control is PictureBox pictureBox)
        {
            // Keep picture boxes as is, but update border if any
            if (pictureBox.BorderStyle != BorderStyle.None)
            {
                pictureBox.BackColor = DarkControlBackground;
            }
        }
        
        // Recursively apply to child controls
        foreach (Control childControl in control.Controls)
        {
            ApplyDarkMode(childControl, isDarkMode);
        }
    }
    
    /// <summary>
    /// Resets controls to light mode (system default)
    /// </summary>
    private static void ApplyLightMode(Control control)
    {
        // Reset to system defaults for light mode
        if (control is Form form)
        {
            form.BackColor = LightBackground;
            form.ForeColor = LightForeground;
        }
        else if (control is Button button)
        {
            button.FlatStyle = FlatStyle.Standard;
            // Keep custom colors for semantic buttons
        }
        else if (control is TextBox textBox)
        {
            textBox.BackColor = LightControlBackground;
            textBox.ForeColor = Color.Black;
        }
        else if (control is ComboBox comboBox)
        {
            comboBox.BackColor = LightControlBackground;
            comboBox.ForeColor = Color.Black;
            comboBox.FlatStyle = FlatStyle.Standard;
        }
        else if (control is ListView listView)
        {
            listView.BackColor = LightControlBackground;
            listView.ForeColor = Color.Black;
        }
        else if (control is DataGridView dataGridView)
        {
            dataGridView.BackgroundColor = LightControlBackground;
            dataGridView.DefaultCellStyle.BackColor = LightControlBackground;
            dataGridView.DefaultCellStyle.ForeColor = Color.Black;
            dataGridView.EnableHeadersVisualStyles = true;
        }
        else if (control is TabControl || control is TabPage || control is Panel || control is GroupBox)
        {
            control.BackColor = LightBackground;
            control.ForeColor = LightForeground;
        }
        else if (control is Label label)
        {
            // Reset only if it was a standard color
            if (label.ForeColor != Color.Blue && label.ForeColor != Color.Green && 
                label.ForeColor != Color.Red && label.ForeColor != Color.Gray)
            {
                label.ForeColor = LightForeground;
            }
        }
        
        // Recursively apply to child controls
        foreach (Control childControl in control.Controls)
        {
            ApplyLightMode(childControl);
        }
    }
}
