namespace WinFormsApp1
{
    partial class SignUpPage_BusinessInformations
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SignUpPage_BusinessInformations));
            panel1 = new Panel();
            pictureBox1 = new PictureBox();
            panel2 = new Panel();
            label12 = new Label();
            label11 = new Label();
            label10 = new Label();
            numericUpDown1 = new NumericUpDown();
            label9 = new Label();
            comboBox1 = new ComboBox();
            label8 = new Label();
            textBox4 = new TextBox();
            label7 = new Label();
            label6 = new Label();
            textBox3 = new TextBox();
            label3 = new Label();
            richTextBox1 = new RichTextBox();
            label2 = new Label();
            textBox2 = new TextBox();
            button2 = new Button();
            label5 = new Label();
            textBox1 = new TextBox();
            label4 = new Label();
            label1 = new Label();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.AutoSize = true;
            panel1.BackColor = Color.Black;
            panel1.Controls.Add(pictureBox1);
            panel1.ForeColor = SystemColors.ControlLightLight;
            panel1.Location = new Point(0, -2);
            panel1.Name = "panel1";
            panel1.Size = new Size(1134, 135);
            panel1.TabIndex = 1;
            panel1.Paint += panel1_Paint;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.ErrorImage = null;
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(413, 3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(361, 119);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // panel2
            // 
            panel2.AutoSize = true;
            panel2.BackColor = SystemColors.WindowFrame;
            panel2.Controls.Add(label12);
            panel2.Controls.Add(label11);
            panel2.Controls.Add(label10);
            panel2.Controls.Add(numericUpDown1);
            panel2.Controls.Add(label9);
            panel2.Controls.Add(comboBox1);
            panel2.Controls.Add(label8);
            panel2.Controls.Add(textBox4);
            panel2.Controls.Add(label7);
            panel2.Controls.Add(label6);
            panel2.Controls.Add(textBox3);
            panel2.Controls.Add(label3);
            panel2.Controls.Add(richTextBox1);
            panel2.Controls.Add(label2);
            panel2.Controls.Add(textBox2);
            panel2.Controls.Add(button2);
            panel2.Controls.Add(label5);
            panel2.Controls.Add(textBox1);
            panel2.Controls.Add(label4);
            panel2.Controls.Add(label1);
            panel2.Location = new Point(0, 126);
            panel2.Name = "panel2";
            panel2.Size = new Size(1134, 477);
            panel2.TabIndex = 2;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            label12.ForeColor = SystemColors.ButtonHighlight;
            label12.Location = new Point(501, 42);
            label12.Name = "label12";
            label12.Size = new Size(193, 25);
            label12.TabIndex = 24;
            label12.Text = "Business Information";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label11.ForeColor = SystemColors.ControlLightLight;
            label11.Location = new Point(501, 353);
            label11.Name = "label11";
            label11.RightToLeft = RightToLeft.No;
            label11.Size = new Size(244, 17);
            label11.TabIndex = 23;
            label11.Text = "You can change these informations later.";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label10.ForeColor = SystemColors.ControlLightLight;
            label10.Location = new Point(449, 353);
            label10.Name = "label10";
            label10.RightToLeft = RightToLeft.No;
            label10.Size = new Size(46, 17);
            label10.TabIndex = 22;
            label10.Text = "Note :";
            // 
            // numericUpDown1
            // 
            numericUpDown1.Location = new Point(531, 310);
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(235, 23);
            numericUpDown1.TabIndex = 21;
            numericUpDown1.TextAlign = HorizontalAlignment.Center;
            numericUpDown1.ValueChanged += numericUpDown1_ValueChanged;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label9.ForeColor = SystemColors.ControlLightLight;
            label9.Location = new Point(417, 315);
            label9.Name = "label9";
            label9.RightToLeft = RightToLeft.No;
            label9.Size = new Size(111, 17);
            label9.TabIndex = 20;
            label9.Text = "No. of Employee";
            // 
            // comboBox1
            // 
            comboBox1.DisplayMember = "Supermarket";
            comboBox1.FormattingEnabled = true;
            comboBox1.ItemHeight = 15;
            comboBox1.Items.AddRange(new object[] { "Supermarket", "Convenience Store", "Retail Store" });
            comboBox1.Location = new Point(531, 281);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(235, 23);
            comboBox1.TabIndex = 19;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label8.ForeColor = SystemColors.ControlLightLight;
            label8.Location = new Point(417, 287);
            label8.Name = "label8";
            label8.RightToLeft = RightToLeft.No;
            label8.Size = new Size(94, 17);
            label8.TabIndex = 18;
            label8.Text = "Business Type";
            // 
            // textBox4
            // 
            textBox4.Location = new Point(531, 252);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(234, 23);
            textBox4.TabIndex = 17;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label7.ForeColor = SystemColors.ControlLightLight;
            label7.Location = new Point(417, 258);
            label7.Name = "label7";
            label7.RightToLeft = RightToLeft.No;
            label7.Size = new Size(58, 17);
            label7.TabIndex = 16;
            label7.Text = "Number";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label6.ForeColor = SystemColors.ControlLightLight;
            label6.Location = new Point(417, 241);
            label6.Name = "label6";
            label6.RightToLeft = RightToLeft.No;
            label6.Size = new Size(108, 17);
            label6.TabIndex = 15;
            label6.Text = "Business Phone ";
            // 
            // textBox3
            // 
            textBox3.Location = new Point(531, 207);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(234, 23);
            textBox3.TabIndex = 14;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.ForeColor = SystemColors.ControlLightLight;
            label3.Location = new Point(416, 213);
            label3.Name = "label3";
            label3.Size = new Size(104, 17);
            label3.TabIndex = 13;
            label3.Text = "Business E-Mail";
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new Point(531, 143);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(234, 58);
            richTextBox1.TabIndex = 12;
            richTextBox1.Text = "";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.ForeColor = SystemColors.ControlLightLight;
            label2.Location = new Point(416, 144);
            label2.Name = "label2";
            label2.Size = new Size(114, 17);
            label2.TabIndex = 11;
            label2.Text = "Business Address";
            // 
            // textBox2
            // 
            textBox2.Location = new Point(531, 114);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(234, 23);
            textBox2.TabIndex = 10;
            // 
            // button2
            // 
            button2.BackColor = SystemColors.Window;
            button2.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button2.Location = new Point(554, 387);
            button2.Name = "button2";
            button2.Size = new Size(102, 32);
            button2.TabIndex = 9;
            button2.Text = "Next";
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.ForeColor = SystemColors.ControlLightLight;
            label5.Location = new Point(416, 115);
            label5.Name = "label5";
            label5.Size = new Size(101, 17);
            label5.TabIndex = 5;
            label5.Text = "Business Name";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(531, 85);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(234, 23);
            textBox1.TabIndex = 4;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.ForeColor = SystemColors.ControlLightLight;
            label4.Location = new Point(416, 86);
            label4.Name = "label4";
            label4.Size = new Size(106, 17);
            label4.TabIndex = 3;
            label4.Text = "Company Name";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = SystemColors.ButtonHighlight;
            label1.Location = new Point(467, 10);
            label1.Name = "label1";
            label1.Size = new Size(273, 32);
            label1.TabIndex = 0;
            label1.Text = "RetiSusun Registration";
            // 
            // SignUpPage_BusinessInformations
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1134, 601);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Name = "SignUpPage_BusinessInformations";
            Text = "Form2";
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private PictureBox pictureBox1;
        private Panel panel2;
        private Button button2;
        private Label label5;
        private TextBox textBox1;
        private Label label4;
        private Label label1;
        private Label label8;
        private TextBox textBox4;
        private Label label7;
        private Label label6;
        private TextBox textBox3;
        private Label label3;
        private RichTextBox richTextBox1;
        private Label label2;
        private TextBox textBox2;
        private ComboBox comboBox1;
        private Label label11;
        private Label label10;
        private NumericUpDown numericUpDown1;
        private Label label9;
        private Label label12;
    }
}