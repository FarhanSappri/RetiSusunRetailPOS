namespace WinFormsApp1
{
    partial class SignUpPage_UserInformation
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SignUpPage_UserInformation));
            panel1 = new Panel();
            pictureBox1 = new PictureBox();
            panel2 = new Panel();
            textBox5 = new TextBox();
            textBox4 = new TextBox();
            label3 = new Label();
            textBox3 = new TextBox();
            label12 = new Label();
            label11 = new Label();
            label10 = new Label();
            label8 = new Label();
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
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.AutoSize = true;
            panel1.BackColor = Color.Black;
            panel1.Controls.Add(pictureBox1);
            panel1.ForeColor = SystemColors.ControlLightLight;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(1134, 135);
            panel1.TabIndex = 2;
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
            panel2.Controls.Add(textBox5);
            panel2.Controls.Add(textBox4);
            panel2.Controls.Add(label3);
            panel2.Controls.Add(textBox3);
            panel2.Controls.Add(label12);
            panel2.Controls.Add(label11);
            panel2.Controls.Add(label10);
            panel2.Controls.Add(label8);
            panel2.Controls.Add(label2);
            panel2.Controls.Add(textBox2);
            panel2.Controls.Add(button2);
            panel2.Controls.Add(label5);
            panel2.Controls.Add(textBox1);
            panel2.Controls.Add(label4);
            panel2.Controls.Add(label1);
            panel2.Location = new Point(0, 128);
            panel2.Name = "panel2";
            panel2.Size = new Size(1134, 452);
            panel2.TabIndex = 3;
            // 
            // textBox5
            // 
            textBox5.Location = new Point(531, 256);
            textBox5.Name = "textBox5";
            textBox5.Size = new Size(234, 23);
            textBox5.TabIndex = 28;
            // 
            // textBox4
            // 
            textBox4.Location = new Point(531, 214);
            textBox4.Name = "textBox4";
            textBox4.PasswordChar = '*';
            textBox4.Size = new Size(234, 23);
            textBox4.TabIndex = 27;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.ForeColor = SystemColors.ControlLightLight;
            label3.Location = new Point(408, 215);
            label3.Name = "label3";
            label3.Size = new Size(120, 17);
            label3.TabIndex = 26;
            label3.Text = "Confirm Password";
            // 
            // textBox3
            // 
            textBox3.Location = new Point(531, 171);
            textBox3.Name = "textBox3";
            textBox3.PasswordChar = '*';
            textBox3.Size = new Size(234, 23);
            textBox3.TabIndex = 25;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            label12.ForeColor = SystemColors.ButtonHighlight;
            label12.Location = new Point(501, 42);
            label12.Name = "label12";
            label12.Size = new Size(176, 25);
            label12.TabIndex = 24;
            label12.Text = "Owner Information";
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
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label8.ForeColor = SystemColors.ControlLightLight;
            label8.Location = new Point(408, 262);
            label8.Name = "label8";
            label8.RightToLeft = RightToLeft.No;
            label8.Size = new Size(101, 17);
            label8.TabIndex = 18;
            label8.Text = "Phone Number";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.ForeColor = SystemColors.ControlLightLight;
            label2.Location = new Point(408, 171);
            label2.Name = "label2";
            label2.Size = new Size(66, 17);
            label2.TabIndex = 11;
            label2.Text = "Password";
            label2.Click += label2_Click;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(531, 128);
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
            label5.Location = new Point(408, 129);
            label5.Name = "label5";
            label5.Size = new Size(51, 17);
            label5.TabIndex = 5;
            label5.Text = "E-Mail ";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(531, 85);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(234, 23);
            textBox1.TabIndex = 4;
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.ForeColor = SystemColors.ControlLightLight;
            label4.Location = new Point(408, 86);
            label4.Name = "label4";
            label4.Size = new Size(71, 17);
            label4.TabIndex = 3;
            label4.Text = "Full Name";
            label4.Click += label4_Click;
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
            // SignUpPage_UserInformation
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1134, 581);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Name = "SignUpPage_UserInformation";
            Text = "SignUpPage_UserInformation";
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private PictureBox pictureBox1;
        private Panel panel2;
        private Label label12;
        private Label label11;
        private Label label10;
        private Label label8;
        private Label label2;
        private TextBox textBox2;
        private Button button2;
        private Label label5;
        private TextBox textBox1;
        private Label label4;
        private Label label1;
        private Label label3;
        private TextBox textBox3;
        private TextBox textBox5;
        private TextBox textBox4;
    }
}