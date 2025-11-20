using Microsoft.Data.Sqlite;
using System.Data;

namespace WinForms
{
    public class Client
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public float Money { get; set; }
    }

    //pdf file like check
    //user (money)
    //products buying

    public partial class CustomerForm : Form
    {
        private SqliteConnection _connect = new(@"Data Source=Databases/EShop.db;");

        private TabControl _tabControl;
        private Button _butCart;

        public CustomerForm()
        {
            InitializeComponent();
            Test();

            Text = "E-pood - Klient";
            Size = new Size(750, 650);

            _tabControl = new();
            _tabControl.Dock = DockStyle.Fill;

            _butCart = new();
            _butCart.Text = "Ostukorv";
            _butCart.Dock = DockStyle.Right;
            _butCart.Click += (sender, e) => {
                OpenCart();
            };

            Panel panel = new();
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

        private void UpdateProducts(TabPage page, int catID)
        {
            FlowLayoutPanel flow = new();
            flow.Dock = DockStyle.Fill;
            flow.VerticalScroll.Enabled = true;
            flow.AutoScroll = true;
            page.Controls.Add(flow);

            DataTable dt = GetProductsTables(catID);

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
                    pic.Image = Image.FromStream(ms);

                Label lCount = new();
                lCount.Location = new Point(0, 200-80);
                lCount.TextAlign = ContentAlignment.MiddleCenter;
                lCount.Text = name + " 1 tk - " + price + " €";
                lCount.Size = new Size(170, 25);
                lCount.BackColor = Color.White;

                Label inStock = new();
                inStock.Location = new Point(10, 0);
                inStock.TextAlign = ContentAlignment.TopLeft;
                inStock.Text = (count > 0 ? $"Laos - {count} tk"  : "Otses");
                inStock.AutoSize = true;
                inStock.BackColor = Color.White;
                inStock.ForeColor = (count > 0 ? Color.Green : Color.Red);

                Button button = new();
                button.Dock = DockStyle.Bottom;
                button.BackColor = Color.White;
                button.Height = 40;
                button.Text = "Lisa ostukorvi";
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

        private Form _formCart;
        private Form _formCount;
        private Dictionary<int, int> Cart = new(); // product id - count

        private void AddProductToCart(int id)
        {
            _formCount = new();
            _formCount.Text = "E-pood - Mitu tükki sa tahad?";
            _formCount.Size = new Size(250, 150);
            _formCount.FormBorderStyle = FormBorderStyle.FixedSingle;
            _formCount.MinimizeBox = false;
            _formCount.MaximizeBox = false;
            _formCount.Padding = new Padding(10);

            TableLayoutPanel tlp = new();
            tlp.Dock = DockStyle.Fill;
            tlp.RowCount = 3;
            tlp.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tlp.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            Label label = new();
            label.Dock = DockStyle.Fill;
            label.Text = "Mitu tükki sa tahad?";

            NumericUpDown num = new();
            num.Dock = DockStyle.Fill;
            num.Minimum = 0;
            num.Maximum = 100;
            num.Value = 1;

            Panel panel = new();
            panel.Dock = DockStyle.Fill;

            Button button = new();
            button.Dock = DockStyle.Fill;
            button.Text = "OK";
            button.Click += (sender, e) => {
                int count = Convert.ToInt32(num.Value);
                int realCount = GetProductCount(id);
                int alreadyInCart = Cart.ContainsKey(id) ? Cart[id] : 0;

                if (count + alreadyInCart > realCount)
                {
                    MessageBox.Show($"Laos on ainult {realCount - alreadyInCart} tk saadaval!", "Pole piisavalt kaupa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _formCount.Close();

                if (!Cart.ContainsKey(id))
                    Cart[id] = count;
                else
                    Cart[id] += count;

                MessageBox.Show("Toode on ostukorvi lisatud.");
            };

            panel.Controls.Add(button);

            Button button2 = new();
            button2.Dock = DockStyle.Right;
            button2.Text = "Tühista";
            button2.Click += (sender, e) => {
                _formCount.Close();
            };

            panel.Controls.Add(button2);

            tlp.Controls.Add(label);
            tlp.Controls.Add(num);
            tlp.Controls.Add(panel);

            _formCount.Controls.Add(tlp);
            _formCount.ShowDialog();
        }

        private void OpenCart()
        {
            _formCart = new();
            _formCart.Text = "E-pood - Ostukorv";
            _formCart.Size = new Size(300, 550);
            _formCart.FormBorderStyle = FormBorderStyle.FixedSingle;
            _formCart.MinimizeBox = false;
            _formCart.MaximizeBox = false;

            TableLayoutPanel table = new();
            table.Dock = DockStyle.Fill;
            table.Padding = new Padding(5);
            table.ColumnCount = 1;
            table.RowCount = Cart.Count;
            table.AutoScroll = true;

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
                prod.Dock = DockStyle.Top;
                prod.BackColor = Color.GhostWhite;
                prod.BorderStyle = BorderStyle.FixedSingle;

                Font font1 = new("Arial", 10);

                Label label = new();
                label.Dock = DockStyle.Fill;
                label.TextAlign = ContentAlignment.MiddleLeft;
                label.Text = row["Name"].ToString() + " - " + kv.Value + "tk";
                label.Font = font1;

                Label label2 = new();
                label2.Dock = DockStyle.Right;
                label2.TextAlign = ContentAlignment.MiddleRight;
                label2.Text = (price * kv.Value) + " €";
                label2.Font = font1;

                prod.Controls.Add(label);
                prod.Controls.Add(label2);

                table.Controls.Add(prod);

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
            butBuy.Font = new Font("Arial", 15, FontStyle.Bold); 
            butBuy.Text = "Ostan";

            Label lSum = new();
            lSum.Dock = DockStyle.Bottom;
            lSum.BackColor = Color.LightGray;
            lSum.TextAlign = ContentAlignment.MiddleCenter;
            lSum.Font = font;
            lSum.Height = 40;
            lSum.Text = "Summa: " + sum + " €";

            panel.Controls.Add(lSum);
            panel.Controls.Add(butBuy);

            _formCart.Controls.Add(table);
            _formCart.Controls.Add(panel);
            _formCart.ShowDialog();
        }

        private void BuyProducts()
        {

        }

        private DataTable GetProductsTables(int catID = 0)
        {
            DataTable dt = new();

            string query = "SELECT Id, Name, Count, Price, Image, BinImage "
                + "FROM Product";

            if (catID > 0)
                query += " WHERE ProductCategoryId = @cat";

            _connect.Open();
            using (var command = new SqliteCommand(query, _connect))
            {
                if (catID > 0)
                    command.Parameters.AddWithValue("@cat", catID);

                using (var reader = command.ExecuteReader())
                    dt.Load(reader);
            }
            _connect.Close();

            return dt;
        }

        private int GetProductCount(int id)
        {
            int count = 0;

            _connect.Open();
            using (var cmd = new SqliteCommand("SELECT Count FROM Product WHERE Id = @id", _connect))
            {
                cmd.Parameters.AddWithValue("@id", id);
                object result = cmd.ExecuteScalar();
                if (result != null)
                    count = Convert.ToInt32(result);
            }
            _connect.Close();

            return count;
        }
    }
}
