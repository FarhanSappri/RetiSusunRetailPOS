namespace WinFormsApp1
{
    public partial class Homepage : Form
    {
        public Homepage()
        {
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Create an instance of your new Sign Up Page form
            Dashboard dashboard = new Dashboard(); // <-- Changed from SignUpForm to SignUpPage

            // Show the new form as a modal dialog.
            // This means the user must close the SignUpPage before they can interact with the Homepage again.
            dashboard.ShowDialog();
        }

        private void label2_Click_1(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Create an instance of your new Sign Up Page form
            SignUpPage_BusinessInformations signUpPage = new SignUpPage_BusinessInformations(); // <-- Changed from SignUpForm to SignUpPage

            // Show the new form as a modal dialog.
            // This means the user must close the SignUpPage before they can interact with the Homepage again.
            signUpPage.ShowDialog();
        }
    }
}
