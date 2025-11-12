using Microsoft.Data.Sqlite;
using System.Data;

namespace WinForms
{
    public partial class EShop : Form
    {
        private SqliteCommand _command;
        private SqliteConnection _connect = new(@"Data Source=Databases/EShop.db;");

        public EShop()
        {
            string folder = Path.Combine(Environment.CurrentDirectory, "Databases");
            Directory.CreateDirectory(folder);

            CreateDatabase();
            InitializeComponent();

            _pbProductImage.SizeMode = PictureBoxSizeMode.StretchImage;

            UpdateData();
            UpdateCategories();
        }

        private void CreateDatabase()
        {
            _connect.Open();

            string product = "CREATE TABLE IF NOT EXISTS Product ( "
                + "Id int NOT NULL PRIMARY KEY, "
                + "Name varchar(50) NOT NULL, "
                + "Count int NOT NULL, "
                + "Price float NOT NULL, "
                + "Image varchar(100), "
                + "BinImage blob, "
                + "ProductCategoryId int, "
                + "FOREIGN KEY(ProductCategoryId) REFERENCES ProductCategory(Id)"
                + " )";

            string productCategory = "CREATE TABLE IF NOT EXISTS ProductCategory ( "
                + "Id int NOT NULL PRIMARY KEY, "
                + "Name varchar(50) NOT NULL, "
                + "Description varchar(200)"
                + " )";

            _command = new SqliteCommand(product, _connect);
            _command.ExecuteNonQuery();

            _command = new SqliteCommand(productCategory, _connect);
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
            DataGridViewComboBoxColumn comboBox = new();
            comboBox.DataPropertyName = "ProductCategory";
            HashSet<string> keys = new();

            foreach (DataRow row in dt.Rows)
            {
                string catName = row["Name"].ToString();
                if (!keys.Contains(catName))
                {
                    keys.Add(catName);
                    comboBox.Items.Add(catName);
                }
            }

            _dataGridView1.Columns.Add(comboBox);
            _pbProductImage.Image = Image.FromFile(Path.Combine(Path.GetFullPath(@"..\..\..\Images"), "shop.png"));
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

        private Form _popupForm;

        private void CreateImage(Image image, int r)
        {
            _popupForm = new Form();
            _popupForm.FormBorderStyle = FormBorderStyle.None;
            _popupForm.StartPosition = FormStartPosition.Manual;
            _popupForm.Size = new Size(200, 200);

            PictureBox pic = new();
            pic.Image = image;
            pic.Dock = DockStyle.Fill;
            pic.SizeMode = PictureBoxSizeMode.Zoom;

            _popupForm.Controls.Add(pic);

            Rectangle rect = _dataGridView1.GetCellDisplayRectangle(4, r, true);
            Point popupPos = _dataGridView1.PointToScreen(rect.Location);

            _popupForm.Location = new Point(popupPos.X + rect.Width, popupPos.Y);
            _popupForm.Show();
        }

        private void _butAddProductCategory_Click(object sender, EventArgs e)
        {
            bool on = false;
            foreach (var item in _cbProductCategory.Items)
            {
                if (item.ToString() == _cbProductCategory.Text)
                    on = true;
            }

            if (!on)
            {
                _command = new SqliteCommand("INSERT INTO ProductCategory (Name) VALUES (@cat)", _connect);
                _connect.Open();
                _command.Parameters.AddWithValue("@cat", _cbProductCategory.Text);
                _command.ExecuteNonQuery();
                _connect.Close();
                _cbProductCategory.Items.Clear();
                UpdateCategories();
                MessageBox.Show($"Kategooria {_cbProductCategory.Text} on lisatud!");
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

        private void _butFindFile_Click(object sender, EventArgs e)
        {
            _openFileDialog = new OpenFileDialog();
            _openFileDialog.InitialDirectory = @"C:\Users\opilane\Pictures";
            _openFileDialog.Multiselect = true;
            _openFileDialog.Filter = "Images Files(*.jpeg;*.bmp;*.png;*.jpg)|*.jpeg;*.bmp;*.png;*.jpg";
            string product = _textProductName.Text;

            FileInfo openInfo = new(@"C:\Users\opilane\Pictures" + _openFileDialog.FileName);
            if (_openFileDialog.ShowDialog() == DialogResult.OK && product != null)
            {
                _saveFileDialog = new SaveFileDialog();
                _saveFileDialog.InitialDirectory = Path.GetFullPath(@"..\..\Images");

                string ext = Path.GetExtension(_openFileDialog.FileName);
                _saveFileDialog.FileName = product + ext;
                _saveFileDialog.Filter = "Images" + ext + "|" + ext;

                if (_saveFileDialog.ShowDialog() == DialogResult.OK && product != null)
                {
                    File.Copy(_openFileDialog.FileName, _saveFileDialog.FileName);
                    _pbProductImage.Image = Image.FromFile(_saveFileDialog.FileName);
                }
            }
            else
                MessageBox.Show("Puudub toode nimetus või oli vajatud Cancel");
        }
    }
}
