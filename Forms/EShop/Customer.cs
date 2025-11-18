using Microsoft.Data.Sqlite;
using System.Data;
using System.Windows.Forms;

namespace WinForms
{
    public partial class CustomerForm : Form
    {
        private SqliteConnection _connect = new(@"Data Source=Databases/EShop.db;");

        private TabControl _tabControl;
        private Button _butCart;

        public CustomerForm()
        {
            InitializeComponent();

            Text = "E-pood - Klient";
            Size = new Size(750, 650);

            _tabControl = new TabControl();
            _tabControl.Dock = DockStyle.Fill;

            _butCart = new Button();
            _butCart.Text = "Корзина";
            _butCart.Dock = DockStyle.Right;
            _butCart.Click += (sender, e) => {
                OpenCart();
            };

            var panel = new Panel();
            panel.Dock = DockStyle.Bottom;
            panel.Padding = new Padding(5);
            panel.Height = 40;

            panel.Controls.Add(_butCart);

            Controls.Add(_tabControl);
            Controls.Add(panel);

            UpdateCategories();
        }

        private void UpdateCategories()
        {
            foreach (TabPage tab in _tabControl.TabPages)
                _tabControl.TabPages.Remove(tab);

            _connect.Open();

            DataTable dt = new();
            using (var command = new SqliteCommand("SELECT Id, Name FROM ProductCategory", _connect))
                using (var reader = command.ExecuteReader())
                    dt.Load(reader);

            foreach (DataRow item in dt.Rows)
            {
                var tab = new TabPage(item["Name"].ToString());
                UpdateProducts(tab, Convert.ToInt32(item["Id"]));

                _tabControl.TabPages.Add(tab);
            }

            _connect.Close();
        }

        private DataTable GetProductsTablesByCategoryId(int catID)
        {
            DataTable dt = new();

            string query = "SELECT Id, Name, Count, Price, Image, BinImage "
                + "FROM Product WHERE ProductCategoryId = @cat";

            _connect.Open();

            using (var command = new SqliteCommand(query, _connect))
            {
                command.Parameters.AddWithValue("@cat", catID);

                using (var reader = command.ExecuteReader())
                    dt.Load(reader);
            }

            _connect.Close();

            return dt;
        }

        private DataTable GetProductsTables()
        {
            DataTable dt = new();
            string query = "SELECT Id, Name, Count, Price, Image, BinImage "
                + "FROM Product";

            _connect.Open();

            using (var command = new SqliteCommand(query, _connect))
                using (var reader = command.ExecuteReader())
                    dt.Load(reader);

            _connect.Close();

            return dt;
        }

        private void UpdateProducts(TabPage page, int catID)
        {
            FlowLayoutPanel flow = new();
            flow.Dock = DockStyle.Fill;
            flow.VerticalScroll.Enabled = true;
            flow.AutoScroll = true;
            page.Controls.Add(flow);

            DataTable dt = GetProductsTablesByCategoryId(catID);

            for (int i = 0; i < 25; i++)
            {
               
            foreach (DataRow item in dt.Rows)
            {
                var id = Convert.ToInt32(item["Id"]);
                var name = item["Name"].ToString();
                var count = Convert.ToInt32(item["Count"]);
                var price = item["Price"].ToString();

                Panel panel = new();
                panel.Size = new Size(170, 200);
                panel.BackColor = Color.LightGray;
                panel.BorderStyle = BorderStyle.FixedSingle;

                PictureBox pic = new();
                pic.Dock = DockStyle.Fill;
                pic.SizeMode = PictureBoxSizeMode.StretchImage;
                pic.Padding = new Padding(5, 5, 5, 40);

                var data = item["BinImage"] as byte[];
                using (var ms = new MemoryStream(data))
                {
                    pic.Image = Image.FromStream(ms);
                }

                Label lCount = new();
                lCount.Location = new Point(0, 200-80);
                lCount.TextAlign = ContentAlignment.MiddleCenter;
                lCount.Text = name + " 1 tk - " + price + " €";
                lCount.Size = new Size(170, 25);
                lCount.BackColor = Color.White;

                Label inStock = new();
                inStock.Location = new Point(10, 0);
                inStock.TextAlign = ContentAlignment.TopLeft;
                inStock.Text = (count > 0 ? "Laos" : "Otses");
                inStock.AutoSize = true;
                inStock.BackColor = Color.White;
                inStock.ForeColor = (count > 0 ? Color.Green : Color.Red);

                Button button = new();
                button.Dock = DockStyle.Bottom;
                button.BackColor = Color.White;
                button.Height = 40;
                button.Text = "В Корзину";
                button.Click += (sender, e) => {
                    AddProductToCart(id);
                };

                panel.Controls.Add(pic);
                pic.Controls.Add(lCount);
                pic.Controls.Add(inStock);
                panel.Controls.Add(button);
                flow.Controls.Add(panel);
            }
            }
        }

        private Form _formCart;
        private Dictionary<int, int> Cart = new(); // product id - num

        private void AddProductToCart(int id)
        {
            if (Cart.ContainsKey(id))
            {
                Cart[id] += 1;
                return;
            }

            Cart.Add(id, 1);
        }

        private void OpenCart()
        {
            _formCart = new Form();
            _formCart.Text = "E-pood - Ostukorv";
            _formCart.Size = new Size(300, 550);
            _formCart.FormBorderStyle = FormBorderStyle.FixedSingle;
            _formCart.MinimizeBox = false;
            _formCart.MaximizeBox = false;

            FlowLayoutPanel flow = new();
            flow.Dock = DockStyle.Fill;
            flow.Padding = new Padding(5);
            flow.FlowDirection = FlowDirection.TopDown;
            flow.AutoScroll = true;

            DataTable dt = GetProductsTables();
            decimal sum = 0;

            foreach (var kv in Cart)
            {
                DataRow row = null;
                foreach (DataRow item in dt.Rows)
                    if (Convert.ToInt32(item["Id"]) == kv.Key)
                    {
                        row = item;
                        break;
                    }

                if (row == null)
                    continue;

                decimal price = Convert.ToDecimal(row["Price"]);
                Panel prod = new();
                prod.Height = 30;
                prod.BackColor = Color.GhostWhite;
                prod.BorderStyle = BorderStyle.FixedSingle;

                Label label = new();
                label.Dock = DockStyle.Fill;
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.Text = row["Name"].ToString() + " " + kv.Value + "tk   " + (price * kv.Value) + " €"; ;
                prod.Controls.Add(label);

                flow.Controls.Add(prod);

                sum += price * kv.Value;
            }

            Panel panel = new();
            panel.Dock = DockStyle.Bottom;
            panel.Padding = new Padding(10);

            Font font = new("Arial", 15);

            Button butBuy = new();
            butBuy.Dock = DockStyle.Bottom;
            butBuy.Height = 50;
            butBuy.BackColor = Color.Green;
            butBuy.ForeColor = Color.White;
            butBuy.Font = font; 
            butBuy.Text = "Osta";

            Label lSum = new();
            lSum.Dock = DockStyle.Bottom;
            lSum.BackColor = Color.LightGray;
            lSum.TextAlign = ContentAlignment.MiddleCenter;
            lSum.Font = font;
            lSum.Height = 40;
            lSum.Text = "Summa: " + sum + " €";

            panel.Controls.Add(lSum);
            panel.Controls.Add(butBuy);

            _formCart.Controls.Add(flow);
            _formCart.Controls.Add(panel);
            _formCart.ShowDialog();
        }
    }
}
