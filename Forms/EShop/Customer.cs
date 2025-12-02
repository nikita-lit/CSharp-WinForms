using Microsoft.Data.Sqlite;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using System.Data;
using System.Text.Json;

namespace WinForms
{

    public class Customer
    {
        public decimal Money { get; set; }
    }

    public partial class CustomerForm : Form
    {
        private SqliteConnection _connect = new(@"Data Source=Databases/EShop.db;");

        private TabControl _tabControl;
        private Button _butCart;

        private Customer _customer = new();
        public static string CustomerDataPath => Path.Combine(Program.GetDirectory(), "Data/customer_data.json");

        public CustomerForm()
        {
            Directory.CreateDirectory(Path.Combine(Program.GetDirectory(), "Data"));
            Directory.CreateDirectory(Path.Combine(Program.GetDirectory(), "Data/Checks"));
            LoadCustomer();

            InitializeComponent();
            GlobalFontSettings.FontResolver = new FontResolver();

            Text = "E-pood - Klient";
            Size = new Size(750, 350);

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

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            SaveCustomer();
        }

        private void LoadCustomer()
        {
            try
            {
                var json = File.ReadAllText(CustomerDataPath);
                _customer = JsonSerializer.Deserialize<Customer>(json);
            }
            catch (Exception ex)
            {
                if (ex is not FileNotFoundException)
                    MessageBox.Show(ex.ToString(), "Customer data loading error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else if (ex is FileNotFoundException)
                    _customer.Money = 1000;
            }
        }

        private void SaveCustomer()
        {
            try
            {
                string json = JsonSerializer.Serialize(_customer);
                File.WriteAllText(CustomerDataPath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Customer data saving error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateCategories()
        {
            int selectedIndex = _tabControl.SelectedIndex;

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

            if (selectedIndex >= 0 && selectedIndex < _tabControl.TabCount)
                _tabControl.SelectedIndex = selectedIndex;
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
                if (Cart.ContainsKey(id))
                    count -= Cart[id];
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
                inStock.Text = (count > 0 ? $"Laos - {count} tk"  : "Otsas");
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
                    int t = realCount - alreadyInCart;
                    if (t > 0)
                        MessageBox.Show($"Laos on ainult {t} tk saadaval!", "Pole piisavalt tooteid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    else
                        MessageBox.Show($"Toode on laost otsas!", "Pole piisavalt tooteid", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }

                _formCount.Close();

                if (!Cart.ContainsKey(id))
                    Cart[id] = count;
                else
                    Cart[id] += count;

                UpdateCategories();

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

        private decimal _sum;

        private void OpenCart()
        {
            _formCart = new();
            _formCart.Text = "E-pood - Ostukorv";
            _formCart.Size = new Size(300, 550);
            _formCart.FormBorderStyle = FormBorderStyle.FixedSingle;
            _formCart.MinimizeBox = false;
            _formCart.MaximizeBox = false;

            Label money = new();
            money.Text = "Teie saldo: " + _customer.Money + " €";
            money.Dock = DockStyle.Top;
            money.TextAlign = ContentAlignment.MiddleCenter;
            money.Font = new Font("Arial", 12);

            TableLayoutPanel table = new();
            table.Dock = DockStyle.Fill;
            table.Padding = new Padding(5);
            table.ColumnCount = 1;
            table.RowCount = Cart.Count;
            table.AutoScroll = true;

            Label lSum = new();
            DataTable dt = GetProductsTables();
            UpdateProductsInCart(table, dt, lSum);

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
            butBuy.Click += (sender, e) => {
                BuyProducts(_sum);
            };

            lSum.Dock = DockStyle.Bottom;
            lSum.BackColor = Color.LightGray;
            lSum.TextAlign = ContentAlignment.MiddleCenter;
            lSum.Font = font;
            lSum.Height = 40;
            lSum.Text = "Summa: " + _sum + " €";

            panel.Controls.Add(lSum);
            panel.Controls.Add(butBuy);

            _formCart.Controls.Add(table);
            _formCart.Controls.Add(money);
            _formCart.Controls.Add(panel);
            _formCart.ShowDialog();
        }

        private void UpdateProductsInCart(TableLayoutPanel table, DataTable dt, Label lSum)
        {
            foreach (Control control in table.Controls)
                table.Controls.Remove(control);

            _sum = 0;

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

                Button but = new();
                but.Text = "X";
                but.ForeColor = Color.White;
                but.BackColor = Color.Red;
                but.Width = 30;
                but.Dock = DockStyle.Right;
                but.Font = new Font("Arial", 10, FontStyle.Bold);

                but.Click += (sender, e) =>
                {
                    Cart.Remove(kv.Key);
                    UpdateProductsInCart(table, dt, lSum);
                };

                prod.Controls.Add(label);
                prod.Controls.Add(label2);
                prod.Controls.Add(but);

                table.Controls.Add(prod);
                _sum += price * kv.Value;
            }

            lSum.Text = "Summa: " + _sum + " €";
        }

        private void BuyProducts(decimal sum)
        {
            if (Cart.Count <= 0)
            {
                MessageBox.Show("Ostukorv on tühi!", "Viga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_customer.Money < sum)
            {
                MessageBox.Show("Sul ei ole piisavalt raha!", "Viga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (var kv in Cart)
            {
                int id = kv.Key;
                int needCount = kv.Value;

                int realCount = GetProductCount(id);

                if (realCount < needCount)
                {
                    MessageBox.Show(
                        $"Toodet ID={id} ei ole piisavalt laos!\nSaadaval: {realCount}, vaja: {needCount}",
                        "Laovarude viga",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }
            }

            _customer.Money -= sum;
            SaveCustomer();

            _connect.Open();
            try
            {
                foreach (var kv in Cart)
                {
                    using (var command = new SqliteCommand("UPDATE Product SET Count = Count - @cnt WHERE Id = @id", _connect))
                    {
                        command.Parameters.AddWithValue("@cnt", kv.Value);
                        command.Parameters.AddWithValue("@id", kv.Key);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Viga andmebaasi uuendamisel!", "Viga", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            _connect.Close();

            CreatePDFCheck(_sum);
            Cart.Clear();
            _formCart?.Close();

            UpdateCategories();
            MessageBox.Show("Ost sooritati edukalt!", "Valmis", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void CreatePDFCheck(decimal sum)
        {
            try
            {
                string file = Path.Combine(Program.GetDirectory(), "Data/Checks", $"check_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.pdf");

                PdfDocument pdf = new PdfDocument();
                pdf.Info.Title = "E-pood Check";

                var page = pdf.AddPage();
                var gfx =XGraphics.FromPdfPage(page);

                var fontTitle = new XFont("Arial", 16);
                var fontText = new XFont("Arial", 12);

                int y = 40;

                gfx.DrawString("E-pood – Ostutšekk", fontTitle, XBrushes.Black, 20, y);
                y += 40;

                gfx.DrawString($"Kuupäev: {DateTime.Now}", fontText, XBrushes.Black, 20, y);
                y += 30;

                gfx.DrawString("Ostetud tooted:", fontText, XBrushes.Black, 20, y);
                y += 25;

                DataTable dt = GetProductsTables();

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

                    string name = row["Name"].ToString();
                    decimal price = Convert.ToDecimal(row["Price"]);
                    decimal cost = price * kv.Value;

                    gfx.DrawString($"{name} — {kv.Value} tk — {cost} €",
                        fontText, XBrushes.Black, 20, y);

                    y += 20;
                }

                y += 10;
                gfx.DrawLine(XPens.Black, 20, y, 300, y);
                y += 20;

                gfx.DrawString($"Kokku makstud: " + sum + " €", new XFont("Arial", 14), XBrushes.Black, 20, y);

                pdf.Save(file);

                MessageBox.Show($"Tšekk on loodud!\n\nAsukoht:\n{file}", "Tšekk valmis", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "PDF loomise viga", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

    public class FontResolver : IFontResolver
    {
        public byte[] GetFont(string faceName)
        {
            switch (faceName)
            {
                case "Arial":
                    return File.ReadAllBytes(@"..\..\..\fonts\arial.ttf");
            }
            return null;
        }

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            familyName = familyName.ToLower();

            if (familyName == "arial")
                return new FontResolverInfo("Arial");

            return null;
        }
    }
}
