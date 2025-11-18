using Microsoft.Data.Sqlite;
using System.Data;

namespace WinForms
{
    public partial class ShopForm : Form
    {
        private SqliteCommand _command;
        private SqliteConnection _connect = new(@"Data Source=Databases/EShop.db;");
        private string _curImagePath = null;

        public ShopForm()
        {
            string folder = Path.Combine(Environment.CurrentDirectory, "Databases");
            Directory.CreateDirectory(folder);

            string folder2 = Path.Combine(Environment.CurrentDirectory, "Images");
            Directory.CreateDirectory(folder2);

            CreateDatabase();
            InitializeComponent();

            Text = "E-pood - Töötaja";

            _pbProductImage.SizeMode = PictureBoxSizeMode.StretchImage;

            UpdateData();
            UpdateCategories();
        }

        private void LoadDefaultImage()
        {
            string path = Path.Combine(Path.GetFullPath(@"..\..\..\Images"), "shop.png");
            _curImagePath = null;
            _pbProductImage.Image = Image.FromFile(path);
        }

        private void CreateDatabase()
        {
            _connect.Open();

            string product = "CREATE TABLE IF NOT EXISTS Product ( "
                + "Id integer NOT NULL PRIMARY KEY AUTOINCREMENT, "
                + "Name varchar(50) NOT NULL, "
                + "Count integer NOT NULL, "
                + "Price float NOT NULL, "
                + "Image varchar(100), "
                + "BinImage blob, "
                + "ProductCategoryId integer, "
                + "FOREIGN KEY(ProductCategoryId) REFERENCES ProductCategory(Id)"
                + " )";

            string productCategory = "CREATE TABLE IF NOT EXISTS ProductCategory ( "
                + "Id integer NOT NULL PRIMARY KEY AUTOINCREMENT, "
                + "Name varchar(50) NOT NULL, "
                + "Description varchar(200)"
                + " )";

            _command = new SqliteCommand(product, _connect);
            _command.ExecuteNonQuery();

            _command = new SqliteCommand(productCategory, _connect);
            _command.ExecuteNonQuery();

            string catData = "INSERT INTO ProductCategory (Name) VALUES ('Leib'), ('Köögiviljad'), ('Puuviljad'), ('Elektroonika');";
            _command = new SqliteCommand(catData, _connect);
            _command.ExecuteNonQuery();

            _connect.Close();
        }

        private void UpdateData()
        {
            _connect.Open();
            DataTable dt = new();

            string query = "SELECT Product.Id, Product.Name, Product.Count, Product.Price, Product.Image, Product.BinImage, "
                + "ProductCategory.Name AS ProductCategory FROM Product INNER JOIN ProductCategory ON Product.ProductCategoryId = ProductCategory.Id";

            using (var command = new SqliteCommand(query, _connect))
            using (var reader = command.ExecuteReader())
            {
                dt.Load(reader);
            }

            _dataGridView1.Columns.Clear();
            _dataGridView1.DataSource = dt;

            LoadDefaultImage();
            _connect.Close();
        }

        private void UpdateCategories()
        {
            _connect.Open();

            DataTable dt = new();
            using (var command = new SqliteCommand("SELECT Id, Name FROM ProductCategory", _connect))
            using (var reader = command.ExecuteReader())
            {
                dt.Load(reader);
            }

            foreach (DataRow item in dt.Rows)
            {
                if (!_cbProductCategory.Items.Contains(item["Name"]))
                    _cbProductCategory.Items.Add(item["Name"]);
                else
                {
                    _command = new SqliteCommand("DELETE FROM ProductCategory WHERE Id=@id", _connect);
                    _command.Parameters.AddWithValue("@id", item["Id"]);
                    _command.ExecuteNonQuery();
                }
            }

            _connect.Close();
        }

        private Form _formPopup;

        private void CreateImage(Image image, int r)
        {
            _formPopup = new Form();
            _formPopup.FormBorderStyle = FormBorderStyle.None;
            _formPopup.StartPosition = FormStartPosition.Manual;
            _formPopup.Size = new Size(200, 200);

            PictureBox pic = new();
            pic.Image = image;
            pic.Dock = DockStyle.Fill;
            pic.SizeMode = PictureBoxSizeMode.Zoom;

            _formPopup.Controls.Add(pic);

            Rectangle rect = _dataGridView1.GetCellDisplayRectangle(5, r, true);
            Point popupPos = _dataGridView1.PointToScreen(rect.Location);

            _formPopup.Location = new Point(popupPos.X + rect.Width, popupPos.Y);
            _formPopup.Show();
        }

        private void _butAddProductCategory_Click(object sender, EventArgs e)
        {
            string product = _cbProductCategory.Text.Trim();

            if (string.IsNullOrEmpty(product))
                return;

            bool on = false;
            foreach (var item in _cbProductCategory.Items)
            {
                if (item.ToString() == product)
                    on = true;
            }

            if (!on)
            {
                _command = new SqliteCommand("INSERT INTO ProductCategory (Name) VALUES (@cat)", _connect);
                _connect.Open();
                _command.Parameters.AddWithValue("@cat", product);
                _command.ExecuteNonQuery();
                _connect.Close();
                _cbProductCategory.Items.Clear();
                UpdateCategories();
                MessageBox.Show($"Kategooria {product} on lisatud!");
            }
            else
                MessageBox.Show("Selline kategooriat on juba olemas!");
        }

        private void _butRemoveProductCategory_Click(object sender, EventArgs e)
        {
            if (_cbProductCategory.SelectedItem != null)
            {
                _connect.Open();
                string value = _cbProductCategory.SelectedItem.ToString();
                _command = new SqliteCommand("DELETE FROM ProductCategory WHERE Name=@cat", _connect);
                _command.Parameters.AddWithValue("@cat", value);
                _command.ExecuteNonQuery();
                _connect.Close();
                _cbProductCategory.Items.Clear();
                UpdateCategories();
            }
        }

        private SaveFileDialog _saveFileDialog;
        private OpenFileDialog _openFileDialog;
        private string _extension = null;

        private void _butFindFile_Click(object sender, EventArgs e)
        {
            string product = _textProductName.Text.Trim();

            if (string.IsNullOrEmpty(product))
            {
                MessageBox.Show("Toote nime ei ole sisestatud");
                return;
            }

            _openFileDialog = new();
            _openFileDialog.InitialDirectory = @"C:\Users\opilane\Pictures";
            _openFileDialog.Multiselect = true;
            _openFileDialog.Filter = "Images Files (*.jpeg;*.bmp;*.png;*.jpg)|*.jpeg;*.bmp;*.png;*.jpg";

            if (_openFileDialog.ShowDialog() == DialogResult.OK && product != null)
            {
                _saveFileDialog = new();
                _saveFileDialog.InitialDirectory = Path.Combine(Environment.CurrentDirectory, "Images");

                _extension = Path.GetExtension(_openFileDialog.FileName);
                _saveFileDialog.FileName = product + _extension;
                _saveFileDialog.Filter = "Images" + _extension + "|" + _extension;

                if (_saveFileDialog.ShowDialog() == DialogResult.OK && product != null)
                {
                    File.Copy(_openFileDialog.FileName, _saveFileDialog.FileName);
                    _curImagePath = _saveFileDialog.FileName;
                    _pbProductImage.Image = Image.FromFile(_saveFileDialog.FileName);
                }
            }
            else
                MessageBox.Show("Puudub toode nimetus või oli vajatud Cancel");
        }

        private byte[] _imageData;

        private void _dataGridView1_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == 5)
            {
                _imageData = _dataGridView1.Rows[e.RowIndex].Cells["BinImage"].Value as byte[];
                if (_imageData != null)
                {
                    using (MemoryStream ms = new MemoryStream(_imageData))
                    {
                        Image image = Image.FromStream(ms);
                        CreateImage(image, e.RowIndex);
                    }
                }
            }
        }

        private void _dataGridView1_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (_formPopup != null && !_formPopup.IsDisposed)
            {
                _formPopup.Close();
            }
        }

        private void _butAddProduct_Click(object sender, EventArgs e)
        {
            string name = _textProductName.Text.Trim();
            decimal count = _numProductCount.Value;
            decimal price = _numProductPrice.Value;
            int cat = _cbProductCategory.SelectedIndex;
            byte[] imageBytes = null;

            try
            {
                imageBytes = File.ReadAllBytes(_curImagePath);
            }
            catch
            {
                MessageBox.Show("Pilti ei ole valitud!");
            }

            bool valid = !string.IsNullOrEmpty(name) && cat >= 0 && imageBytes != null;

            if (valid)
            {
                try
                {
                    _connect.Open();

                    string query = "INSERT INTO Product (Name, Count, Price, Image, BinImage, ProductCategoryId) VALUES "
                    + "(@name, @count, @price, @image, @binimage, @cat)";

                    _command = new SqliteCommand(query, _connect);
                    _command.Parameters.AddWithValue("@name", name);
                    _command.Parameters.AddWithValue("@count", count);
                    _command.Parameters.AddWithValue("@price", price);
                    _command.Parameters.AddWithValue("@image", name + _extension);

                    _command.Parameters.AddWithValue("@binimage", imageBytes);
                    _command.Parameters.AddWithValue("@cat", cat + 1);

                    _command.ExecuteNonQuery();
                    _connect.Close();

                    Clear();
                    UpdateData();

                    MessageBox.Show("Andmed lisatud! ");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Andmebaasiga VIGA! " + ex.Message);
                }
            }
            else
                MessageBox.Show("Vale andmed!");
        }

        private void _butClearProduct_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void Clear()
        {
            _textProductName.Text = "";
            _numProductCount.Value = 0;
            _numProductPrice.Value = 0;
            _cbProductCategory.SelectedIndex = -1;

            LoadDefaultImage();
        }

        private int GetSelectedProductId()
        {
            object value;

            try
            {
                value = _dataGridView1.SelectedRows[0].Cells["Id"].Value;
            }
            catch
            {
                return -1;
            }

            return Convert.ToInt32(value);
        }

        private void _butRemoveProduct_Click(object sender, EventArgs e)
        {
            int id = GetSelectedProductId();
            if (id > 0)
            {
                _connect.Open();
                _command = new SqliteCommand("DELETE FROM Product WHERE Id=@id", _connect);
                _command.Parameters.AddWithValue("@id", id);
                _command.ExecuteNonQuery();
                _connect.Close();

                UpdateData();

                MessageBox.Show("Andmed tabelist Product on kustutatud!");
                return;
            }
            else
                MessageBox.Show("Viga Product tabelist andmete kustutamisega!");
        }

        private void _butUpdateProduct_Click(object sender, EventArgs e)
        {
            string name = _textProductName.Text.Trim();
            decimal count = _numProductCount.Value;
            decimal price = _numProductPrice.Value;
            int cat = _cbProductCategory.SelectedIndex;
            byte[] imageBytes = null;
            int id = GetSelectedProductId();

            try
            {
                imageBytes = File.ReadAllBytes(_curImagePath);
            }
            catch
            {
                MessageBox.Show("Pilti ei ole valitud!");
            }

            bool valid = !string.IsNullOrEmpty(name) && cat >= 0 && imageBytes != null && id > 0;

            if (valid)
            {
                try
                {
                    string query = "UPDATE Product SET Name=@name, Count=@count, Price=@price, "
                        + "Image=@image, BinImage=@binimage, ProductCategoryId=@cat WHERE Id=@id";

                    _connect.Open();
                    _command = new SqliteCommand(query, _connect);

                    _command.Parameters.AddWithValue("@name", name);
                    _command.Parameters.AddWithValue("@count", count);
                    _command.Parameters.AddWithValue("@price", price);
                    _command.Parameters.AddWithValue("@image", name + _extension);
                    _command.Parameters.AddWithValue("@binimage", imageBytes);
                    _command.Parameters.AddWithValue("@cat", cat + 1);
                    _command.Parameters.AddWithValue("@id", id);

                    _command.ExecuteNonQuery();

                    _connect.Close();

                    UpdateData();

                    MessageBox.Show("Andmed uuendatud!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Andmebaasiga VIGA! " + ex.Message);
                }
            }
            else
                MessageBox.Show("Vale andmed!");
        }

        private void _dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                int id = GetSelectedProductId();

                if (id > 0)
                {
                    try
                    {
                        _textProductName.Text = _dataGridView1.SelectedRows[0].Cells["Name"].Value.ToString();
                        _numProductCount.Value = Convert.ToDecimal(_dataGridView1.SelectedRows[0].Cells["Count"].Value);
                        _numProductPrice.Value = Convert.ToDecimal(_dataGridView1.SelectedRows[0].Cells["Price"].Value);
                        _cbProductCategory.SelectedItem = Convert.ToString(_dataGridView1.SelectedRows[0].Cells["ProductCategory"].Value);

                        if (_dataGridView1.CurrentRow.Cells["BinImage"].Value is byte[] imageData)
                        {
                            using (MemoryStream ms = new MemoryStream(imageData))
                            {
                                Image image = Image.FromStream(ms);
                                _pbProductImage.Image = image;
                            }
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Viga!");
                    }
                }
            }
            catch { }
        }
    }
}
