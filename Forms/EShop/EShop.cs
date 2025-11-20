namespace WinForms
{
    public partial class EShop : Form
    {
        public CustomerForm CustomerForm;
        public ShopForm ShopForm;

        public EShop()
        {
            InitializeComponent();

            Text = "Valik";
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MinimizeBox = false;
            MaximizeBox = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (ShopForm != null && ShopForm.Visible)
                ShopForm.Close();

            CustomerForm = new();
            CustomerForm.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (CustomerForm != null && CustomerForm.Visible)
                CustomerForm.Close();

            ShopForm = new();
            ShopForm.Show();
        }
    }
}
