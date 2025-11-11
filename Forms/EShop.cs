using Microsoft.Data.SqlClient;
using System.Data;

namespace WinForms
{
    public partial class EShop : Form
    {
        private SqlCommand _command;
        private SqlConnection _connect = new(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Databases\EShopDB.mdf;Integrated Security=True");
        private SqlDataAdapter _adapterProduct, _adapterProductCategory;

        public EShop()
        {
            InitializeComponent();
            UpdateCategories();
        }

        private void UpdateCategories()
        {
            _connect.Open();
            _adapterProduct = new SqlDataAdapter("SELECT Id, Name FROM ProductCategory", _connect);
            DataTable dt = new();
            _adapterProduct.Fill(dt);

            foreach (DataRow item in dt.Rows)
            {
                if (!_cbProductCategory.Items.Contains(item["Name"]))
                    _cbProductCategory.Items.Add(item["Name"]);
                else
                {
                    _command = new SqlCommand("DELETE FROM ProductCategory WHERE Id=@id", _connect);
                    _command.Parameters.AddWithValue("@id", item["Id"]);
                    _command.ExecuteNonQuery();
                }
            }

            _connect.Close();
        }

        private void _butAddProductCategory_Click(object sender, EventArgs e)
        {
            bool on = false;
            foreach(var item in _cbProductCategory.Items)
            {
                if (item.ToString() == _cbProductCategory.Text)
                    on = true;
            }

            if (!on)
            {
                _command = new SqlCommand("INSERT INTO ProductCategory (Name) VALUES (@cat)", _connect);
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
            if(_cbProductCategory.SelectedItem != null)
            {
                _connect.Open();
                string value = _cbProductCategory.SelectedItem.ToString();
                _command = new SqlCommand("DELETE FROM ProductCategory WHERE Name=@cat", _connect);
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
            string product = "test";

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
