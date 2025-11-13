namespace WinForms
{
    public partial class EShop : Form
    {
        public CustomerForm CustomerForm;
        public ShopForm ShopForm;

        public EShop()
        {
            Text = "Valik";

            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CustomerForm = new();
            CustomerForm.Show();
            Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ShopForm = new();
            ShopForm.Show();
            Hide();
        }
    }
}
