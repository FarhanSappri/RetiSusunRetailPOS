namespace WinFormsApp1
{
    partial class Dashboard
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            PictureBox pictureBox1;
            panel1 = new Panel();
            button8 = new Button();
            button7 = new Button();
            label1 = new Label();
            label2 = new Label();
            button6 = new Button();
            button5 = new Button();
            button4 = new Button();
            button3 = new Button();
            button2 = new Button();
            button1 = new Button();
            panel2 = new Panel();
            panel3 = new Panel();
            panel7 = new Panel();
            label9 = new Label();
            label10 = new Label();
            label11 = new Label();
            panel9 = new Panel();
            label6 = new Label();
            label15 = new Label();
            label16 = new Label();
            label17 = new Label();
            panel4 = new Panel();
            label5 = new Label();
            label4 = new Label();
            label3 = new Label();
            pictureBox1 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panel1.SuspendLayout();
            panel3.SuspendLayout();
            panel7.SuspendLayout();
            panel9.SuspendLayout();
            panel4.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.profile;
            pictureBox1.Location = new Point(12, 438);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(69, 67);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // panel1
            // 
            panel1.AutoSize = true;
            panel1.BackColor = Color.Black;
            panel1.Controls.Add(button8);
            panel1.Controls.Add(button7);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(pictureBox1);
            panel1.Controls.Add(button6);
            panel1.Controls.Add(button5);
            panel1.Controls.Add(button4);
            panel1.Controls.Add(button3);
            panel1.Controls.Add(button2);
            panel1.Controls.Add(button1);
            panel1.ForeColor = SystemColors.ControlLightLight;
            panel1.Location = new Point(-1, 45);
            panel1.Name = "panel1";
            panel1.Size = new Size(189, 536);
            panel1.TabIndex = 3;
            // 
            // button8
            // 
            button8.FlatStyle = FlatStyle.Popup;
            button8.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button8.ForeColor = SystemColors.ControlLightLight;
            button8.Location = new Point(14, 195);
            button8.Name = "button8";
            button8.Size = new Size(170, 42);
            button8.TabIndex = 9;
            button8.Text = "Stock Entry";
            button8.TextAlign = ContentAlignment.MiddleLeft;
            button8.UseVisualStyleBackColor = true;
            button8.Click += button8_Click;
            // 
            // button7
            // 
            button7.FlatStyle = FlatStyle.Popup;
            button7.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button7.ForeColor = SystemColors.ControlLightLight;
            button7.Location = new Point(13, 317);
            button7.Name = "button7";
            button7.Size = new Size(170, 42);
            button7.TabIndex = 8;
            button7.Text = "Business Information";
            button7.TextAlign = ContentAlignment.MiddleLeft;
            button7.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(87, 456);
            label1.Name = "label1";
            label1.Size = new Size(92, 20);
            label1.TabIndex = 1;
            label1.Text = "farhansappri";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(87, 476);
            label2.Name = "label2";
            label2.Size = new Size(45, 15);
            label2.TabIndex = 2;
            label2.Text = "Owner";
            label2.Click += label2_Click;
            // 
            // button6
            // 
            button6.FlatStyle = FlatStyle.Popup;
            button6.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button6.ForeColor = SystemColors.ControlLightLight;
            button6.Location = new Point(13, 59);
            button6.Name = "button6";
            button6.Size = new Size(170, 42);
            button6.TabIndex = 7;
            button6.Text = "Point-of-Sale (POS)";
            button6.TextAlign = ContentAlignment.MiddleLeft;
            button6.UseVisualStyleBackColor = true;
            button6.Click += button6_Click;
            // 
            // button5
            // 
            button5.FlatStyle = FlatStyle.Popup;
            button5.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button5.ForeColor = SystemColors.ControlLightLight;
            button5.Location = new Point(13, 281);
            button5.Name = "button5";
            button5.Size = new Size(170, 42);
            button5.TabIndex = 6;
            button5.Text = "Settings";
            button5.TextAlign = ContentAlignment.MiddleLeft;
            button5.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            button4.FlatStyle = FlatStyle.Popup;
            button4.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button4.ForeColor = SystemColors.ControlLightLight;
            button4.Location = new Point(13, 238);
            button4.Name = "button4";
            button4.Size = new Size(170, 42);
            button4.TabIndex = 5;
            button4.Text = "Sales";
            button4.TextAlign = ContentAlignment.MiddleLeft;
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // button3
            // 
            button3.FlatStyle = FlatStyle.Popup;
            button3.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button3.ForeColor = SystemColors.ControlLightLight;
            button3.Location = new Point(13, 150);
            button3.Name = "button3";
            button3.Size = new Size(170, 42);
            button3.TabIndex = 4;
            button3.Text = "Manage Inventory";
            button3.TextAlign = ContentAlignment.MiddleLeft;
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button2
            // 
            button2.FlatStyle = FlatStyle.Popup;
            button2.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button2.ForeColor = SystemColors.ControlLightLight;
            button2.Location = new Point(13, 102);
            button2.Name = "button2";
            button2.Size = new Size(170, 42);
            button2.TabIndex = 3;
            button2.Text = "Purchase Order";
            button2.TextAlign = ContentAlignment.MiddleLeft;
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button1
            // 
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button1.ForeColor = SystemColors.ControlLightLight;
            button1.Location = new Point(13, 11);
            button1.Name = "button1";
            button1.Size = new Size(170, 42);
            button1.TabIndex = 0;
            button1.Text = "Dashboard\r\n";
            button1.TextAlign = ContentAlignment.MiddleLeft;
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // panel2
            // 
            panel2.BackColor = Color.ForestGreen;
            panel2.BorderStyle = BorderStyle.FixedSingle;
            panel2.Location = new Point(-1, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(1137, 50);
            panel2.TabIndex = 4;
            // 
            // panel3
            // 
            panel3.BackColor = SystemColors.ActiveBorder;
            panel3.Controls.Add(panel7);
            panel3.Controls.Add(panel9);
            panel3.Controls.Add(panel4);
            panel3.Location = new Point(188, 48);
            panel3.Name = "panel3";
            panel3.Size = new Size(948, 533);
            panel3.TabIndex = 5;
            // 
            // panel7
            // 
            panel7.BackColor = Color.Red;
            panel7.BorderStyle = BorderStyle.FixedSingle;
            panel7.Controls.Add(label9);
            panel7.Controls.Add(label10);
            panel7.Controls.Add(label11);
            panel7.Location = new Point(258, 28);
            panel7.Name = "panel7";
            panel7.Size = new Size(174, 104);
            panel7.TabIndex = 5;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 9F, FontStyle.Underline, GraphicsUnit.Point, 0);
            label9.ForeColor = SystemColors.ButtonHighlight;
            label9.Location = new Point(73, 70);
            label9.Name = "label9";
            label9.Size = new Size(68, 15);
            label9.TabIndex = 4;
            label9.Text = "More info...";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            label10.ForeColor = SystemColors.ButtonHighlight;
            label10.Location = new Point(73, 42);
            label10.Name = "label10";
            label10.Size = new Size(92, 15);
            label10.TabIndex = 2;
            label10.Text = "CRITICAL ITEMS";
            label10.Click += label10_Click;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label11.ForeColor = SystemColors.ButtonHighlight;
            label11.Location = new Point(98, 14);
            label11.Name = "label11";
            label11.Size = new Size(23, 25);
            label11.TabIndex = 1;
            label11.Text = "2";
            label11.Click += label11_Click;
            // 
            // panel9
            // 
            panel9.BackColor = Color.RoyalBlue;
            panel9.BorderStyle = BorderStyle.FixedSingle;
            panel9.Controls.Add(label6);
            panel9.Controls.Add(label15);
            panel9.Controls.Add(label16);
            panel9.Controls.Add(label17);
            panel9.Location = new Point(466, 28);
            panel9.Name = "panel9";
            panel9.Size = new Size(181, 104);
            panel9.TabIndex = 5;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            label6.ForeColor = SystemColors.ButtonHighlight;
            label6.Location = new Point(124, 17);
            label6.Name = "label6";
            label6.Size = new Size(0, 21);
            label6.TabIndex = 5;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Font = new Font("Segoe UI", 9F, FontStyle.Underline, GraphicsUnit.Point, 0);
            label15.ForeColor = SystemColors.ButtonHighlight;
            label15.Location = new Point(73, 70);
            label15.Name = "label15";
            label15.Size = new Size(68, 15);
            label15.TabIndex = 4;
            label15.Text = "More info...";
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            label16.ForeColor = SystemColors.ButtonHighlight;
            label16.Location = new Point(73, 42);
            label16.Name = "label16";
            label16.Size = new Size(104, 15);
            label16.TabIndex = 2;
            label16.Text = "STOCK ON HAND";
            label16.Click += label16_Click;
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label17.ForeColor = SystemColors.ButtonHighlight;
            label17.Location = new Point(73, 14);
            label17.Name = "label17";
            label17.Size = new Size(45, 25);
            label17.TabIndex = 1;
            label17.Text = "250";
            // 
            // panel4
            // 
            panel4.BackColor = Color.LimeGreen;
            panel4.BorderStyle = BorderStyle.FixedSingle;
            panel4.Controls.Add(label5);
            panel4.Controls.Add(label4);
            panel4.Controls.Add(label3);
            panel4.Location = new Point(43, 28);
            panel4.Name = "panel4";
            panel4.Size = new Size(181, 104);
            panel4.TabIndex = 0;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 9F, FontStyle.Underline, GraphicsUnit.Point, 0);
            label5.ForeColor = SystemColors.ButtonHighlight;
            label5.Location = new Point(73, 70);
            label5.Name = "label5";
            label5.Size = new Size(68, 15);
            label5.TabIndex = 4;
            label5.Text = "More info...";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            label4.ForeColor = SystemColors.ButtonHighlight;
            label4.Location = new Point(73, 42);
            label4.Name = "label4";
            label4.Size = new Size(75, 15);
            label4.TabIndex = 2;
            label4.Text = "DAILY SALES";
            label4.Click += label4_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.ForeColor = SystemColors.ButtonHighlight;
            label3.Location = new Point(70, 14);
            label3.Name = "label3";
            label3.Size = new Size(107, 25);
            label3.TabIndex = 1;
            label3.Text = "RM 100.00";
            // 
            // Dashboard
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1133, 581);
            Controls.Add(panel3);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Name = "Dashboard";
            Text = "Dashboard";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel3.ResumeLayout(false);
            panel7.ResumeLayout(false);
            panel7.PerformLayout();
            panel9.ResumeLayout(false);
            panel9.PerformLayout();
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private Panel panel2;
        private Panel panel3;
        private PictureBox pictureBox1;
        private Label label2;
        private Label label1;
        private Button button1;
        private Button button3;
        private Button button2;
        private Button button5;
        private Button button4;
        private Button button6;
        private Button button7;
        private Panel panel4;
        private Label label4;
        private Label label3;
        private Label label5;
        private Panel panel7;
        private Label label9;
        private Label label10;
        private Label label11;
        private Panel panel9;
        private Label label15;
        private Label label16;
        private Label label17;
        private Label label6;
        private Button button8;
    }
}