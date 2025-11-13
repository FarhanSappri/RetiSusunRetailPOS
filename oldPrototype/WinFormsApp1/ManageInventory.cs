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
    public partial class ManageInventory : Form
    {
        public ManageInventory()
        {
            InitializeComponent();
            AddInventoryData();

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void AddInventoryData()
        {
            // Define headers
            string[] headers = { "#", "Product Code", "Barcode", "Description", "Brand", "Category", "Price" };
            for (int col = 0; col < headers.Length; col++)
            {
                Label header = new Label();
                header.Text = headers[col];
                header.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                header.TextAlign = ContentAlignment.MiddleCenter;
                header.Dock = DockStyle.Fill;
                header.BackColor = Color.LightGray;
                tableLayoutPanel1.Controls.Add(header, col, 0); // Add to row 0
            }

            // Define data rows
            string[,] data = new string[5, 7]
            {
        { "1", "111111", "111111111", "Milo Softpack",      "Nestle",     "Powdered Beverage", "10.90" },
        { "2", "111112", "111111112", "Koko Krunch",        "Nestle",     "Cereal",            "11.90" },
        { "3", "111113", "111111113", "Full Cream Milk",    "Dutch Lady", "Milk",              "7.20"  },
        { "4", "111114", "111111114", "Honey Stars",        "Nestle",     "Cereal",            "9.90"  },
        { "5", "111115", "111111115", "Nescafe Gold Blend", "Nestle",     "Coffee",            "12.90" }
            };

            // Add rows
            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    Label cell = new Label();
                    cell.Text = data[row, col];
                    cell.TextAlign = ContentAlignment.MiddleLeft;
                    cell.Dock = DockStyle.Fill;
                    tableLayoutPanel1.Controls.Add(cell, col, row + 1); // +1 to skip header row
                }
            }
        }

    }
}
