using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class SignUpPage_BusinessInformations : Form
    {
        public SignUpPage_BusinessInformations()
        {
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Create an instance of your new Sign Up Page form
            SignUpPage_UserInformation signUpPage2 = new SignUpPage_UserInformation(); // <-- Changed from SignUpForm to SignUpPage

            // Show the new form as a modal dialog.
            // This means the user must close the SignUpPage before they can interact with the Homepage again.
            signUpPage2.ShowDialog();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
