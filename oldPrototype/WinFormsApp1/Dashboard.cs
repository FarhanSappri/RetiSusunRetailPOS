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
    public partial class Dashboard : Form
    {
        public Dashboard()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            // Create an instance of your new Sign Up Page form
            POS pos = new POS(); // <-- Changed from SignUpForm to SignUpPage

            // Show the new form as a modal dialog.
            // This means the user must close the SignUpPage before they can interact with the Homepage again.
            pos.ShowDialog();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Create an instance of your new Sign Up Page form
            PurchaseOrder purchaseOrder = new PurchaseOrder(); // <-- Changed from SignUpForm to SignUpPage

            // Show the new form as a modal dialog.
            // This means the user must close the SignUpPage before they can interact with the Homepage again.
            purchaseOrder.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Create an instance of your new Sign Up Page form
            ManageInventory manageInventory = new ManageInventory(); // <-- Changed from SignUpForm to SignUpPage

            // Show the new form as a modal dialog.
            // This means the user must close the SignUpPage before they can interact with the Homepage again.
            manageInventory.ShowDialog();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            StockEntry stockEntry = new StockEntry();
            stockEntry.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Sales sales = new Sales();
            sales.ShowDialog();
        }
    }
}
