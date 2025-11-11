using Microsoft.Data.SqlClient;
using System.Data;

namespace WinForms
{
    public partial class EShop : Form
    {
        private SqlCommand _command;
        private SqlConnection _connect = new("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\opilane\\source\\repos\\CSharp-WinForms\\Databases\\EShopDB.mdf;Integrated Security=True");
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
            }
            else
                MessageBox.Show("Selline kategooriat on juba olemas!");
        }
    }
}
