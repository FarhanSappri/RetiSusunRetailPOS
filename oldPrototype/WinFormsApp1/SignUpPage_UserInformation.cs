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
    public partial class SignUpPage_UserInformation : Form
    {
        public SignUpPage_UserInformation()
        {
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Create an instance of your new Sign Up Page form
            ConfirmRegistration finishRegister = new ConfirmRegistration(); // <-- Changed from SignUpForm to SignUpPage

            // Show the new form as a modal dialog.
            // This means the user must close the SignUpPage before they can interact with the Homepage again.
            finishRegister.ShowDialog();
        }
    }
}
