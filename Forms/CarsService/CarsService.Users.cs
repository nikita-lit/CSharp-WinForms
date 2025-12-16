using WinForms.CarsService.Models;

namespace WinForms.CarsService
{
    public partial class CarsService
    {
        private TableLayoutPanel _tlpUsers;
        private Form _formAddUser;
        private int _currentUserRow = -1;

        private void SetupUsersTab(Panel users)
        {
            ScrollableControl panel = new();
            panel.Dock = DockStyle.Top;
            panel.Height = 300;
            panel.AutoScroll = true;
            panel.BackColor = Colors.TableBackground;

            _tlpUsers = new();
            _tlpUsers.BackColor = Colors.TableBackground;
            _tlpUsers.AutoSize = true;
            _tlpUsers.Dock = DockStyle.Top;

            panel.Controls.Add(_tlpUsers);

            EventHandler add = (sender, e) =>
            {
                OpenUserForm();
            };

            EventHandler update = (sender, e) =>
            {
                var user = GetCurrentUser();
                if (user != null)
                    OpenUserForm(user);
                else
                    MessageBox.Show(LanguageManager.Get("row_not_selected"), LanguageManager.Get("warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            };

            Panel spacer = new();
            spacer.Height = 7;
            spacer.Dock = DockStyle.Top;

            users.Controls.Add(SetupButtonsPanel(add, update, butDeleteUser_Click, LoadUsers));
            users.Controls.Add(spacer);
            users.Controls.Add(panel);

            LoadUsers();
        }

        private void LoadUsers(string search = "")
        {
            if (_tlpUsers == null)
                return;

            _tlpUsers.Controls.Clear();
            _tlpUsers.RowStyles.Clear();
            _tlpUsers.ColumnStyles.Clear();

            string[] headers = { "id", "user_name", "password", "role" };
            _tlpUsers.ColumnCount = headers.Length;

            for (int i = 0; i < headers.Length; i++)
            {
                var header = headers[i];
                if (header == "id")
                    _tlpUsers.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 5f));
                else
                    _tlpUsers.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20f));

                Label lblHeader = new();
                lblHeader.Text = LanguageManager.Get(header);
                lblHeader.Dock = DockStyle.Top;
                lblHeader.TextAlign = ContentAlignment.MiddleCenter;
                lblHeader.BackColor = Colors.Header;
                lblHeader.ForeColor = Colors.Text;
                lblHeader.Margin = new Padding(3);
                lblHeader.Font = new Font(lblHeader.Font, FontStyle.Bold);

                lblHeader.Paint += (s, e) =>
                {
                    using Pen pen = new(Colors.HeaderLine, 1);
                    e.Graphics.DrawLine(pen, 0, lblHeader.Height - 1, lblHeader.Width, lblHeader.Height - 1);
                };

                LanguageManager.LanguageChanged += () =>
                {
                    lblHeader.Text = LanguageManager.Get(header);
                };

                _tlpUsers.Controls.Add(lblHeader, i, 0);
            }

            var users = _dbContext.Users
                .Where(u => string.IsNullOrEmpty(search) ||
                            u.Name.ToLower().Contains(search.ToLower()) ||
                            u.Role.ToLower().Contains(search.ToLower()))
                .ToList();

            _tlpUsers.RowCount = users.Count + 1;

            for (int row = 0; row < users.Count; row++)
            {
                _tlpUsers.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                var user = users[row];

                Label CreateLabel(string text, int r, bool translate = false)
                {
                    Label label = new();
                    label.Text = translate ? LanguageManager.Get(text) : text;
                    label.Dock = DockStyle.Top;
                    label.TextAlign = ContentAlignment.MiddleCenter;
                    label.BackColor = Colors.Row;
                    label.ForeColor = Colors.Text;
                    label.Margin = new Padding(3);
                    label.Font = new Font("Arial", 12);

                    label.Click += (s, e) =>
                    {
                        if (r == _currentUserRow)
                        {
                            _currentUserRow = -1;
                            RemoveRowsHighlight(_tlpUsers);
                        }
                        else
                        {
                            _currentUserRow = r;
                            HighlightRow(_tlpUsers, r + 1);
                        }
                    };

                    if (translate)
                    {
                        LanguageManager.LanguageChanged += () => {
                            label.Text = LanguageManager.Get(text);
                        };
                    }

                    return label;
                }

                _tlpUsers.Controls.Add(CreateLabel(user.Id.ToString(), row), 0, row + 1);
                _tlpUsers.Controls.Add(CreateLabel(user.Name, row), 1, row + 1);
                _tlpUsers.Controls.Add(CreateLabel(new string('*', user.Password.Length), row, true), 2, row + 1);
                _tlpUsers.Controls.Add(CreateLabel(user.Role, row, true), 3, row + 1);
            }
        }

        private User GetCurrentUser()
        {
            if (_currentUserRow >= 0)
            {
                var users = _dbContext.Users.ToList();
                return users[_currentUserRow];
            }
            return null;
        }

        private void OpenUserForm(User user = null)
        {
            bool isUserValid = user != null;
            string t = isUserValid
                ? LanguageManager.Get("update")
                : LanguageManager.Get("add");

            _formAddUser = new();
            _formAddUser.Text = $"{LanguageManager.Get("car_service")} - {t} {LanguageManager.Get("user")}";
            _formAddUser.Size = new Size(270, 340);
            _formAddUser.Padding = new Padding(20);
            _formAddUser.FormBorderStyle = FormBorderStyle.FixedSingle;
            _formAddUser.MinimizeBox = false;
            _formAddUser.MaximizeBox = false;
            _formAddUser.BackColor = Colors.Background;
            MakeDark(_formAddUser.Handle);

            //----------------------------------------
            Label lblName = new();
            lblName.Text = LanguageManager.Get("name") + ":";
            lblName.Dock = DockStyle.Top;
            lblName.ForeColor = Colors.Text;

            TextBox txtName = new();
            txtName.Dock = DockStyle.Top;
            if (isUserValid)
                txtName.Text = user.Name;

            //----------------------------------------
            Label lblPassword = new();
            lblPassword.Text = LanguageManager.Get("password") + ":";
            lblPassword.Dock = DockStyle.Top;
            lblPassword.ForeColor = Colors.Text;

            TextBox txtPassword = new();
            txtPassword.Dock = DockStyle.Top;
            if (isUserValid)
                txtPassword.Text = user.Password;

            //----------------------------------------
            Label lblRole = new();
            lblRole.Text = LanguageManager.Get("role") + ":";
            lblRole.Dock = DockStyle.Top;
            lblRole.ForeColor = Colors.Text;

            ComboBox cbRole = new();
            cbRole.Dock = DockStyle.Top;
            cbRole.DropDownStyle = ComboBoxStyle.DropDownList;

            cbRole.Items.Add(new KeyValuePair<string, string>("admin", LanguageManager.Get("admin")));
            cbRole.Items.Add(new KeyValuePair<string, string>("worker", LanguageManager.Get("worker")));
            cbRole.Items.Add(new KeyValuePair<string, string>("customer", LanguageManager.Get("customer")));

            cbRole.DisplayMember = "Value";
            cbRole.ValueMember = "Key";

            if (isUserValid)
            {
                foreach (KeyValuePair<string, string> kv in cbRole.Items)
                {
                    if (kv.Key == user.Role)
                        cbRole.SelectedItem = kv;
                }
            }

            //----------------------------------------
            Button but = new();
            but.Text = t;
            but.Dock = DockStyle.Bottom;
            but.Height = 30;
            SetupButtonStyle(but);

            but.Click += (s, e) =>
            {
                var role = ((KeyValuePair<string, string>)cbRole.SelectedItem).Key;
                bool isValid =
                    !string.IsNullOrWhiteSpace(txtName.Text) &&
                    !string.IsNullOrWhiteSpace(txtPassword.Text) &&
                    !string.IsNullOrWhiteSpace(role);

                if (isValid)
                {
                    if (!isUserValid)
                    {
                        _dbContext.Users.Add(new User { Name = txtName.Text, Role = role, Password = txtPassword.Text });
                    }
                    else
                    {
                        user.Name = txtName.Text;
                        user.Password = txtPassword.Text;
                        user.Role = role;
                    }

                    _dbContext.SaveChanges();
                    _formAddUser.Close();
                    LoadUsers();
                }
                else
                {
                    MessageBox.Show(LanguageManager.Get("invalid"), LanguageManager.Get("warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            _formAddUser.Controls.Add(cbRole);
            _formAddUser.Controls.Add(lblRole);
            _formAddUser.Controls.Add(txtPassword);
            _formAddUser.Controls.Add(lblPassword);
            _formAddUser.Controls.Add(txtName);
            _formAddUser.Controls.Add(lblName);
            _formAddUser.Controls.Add(but);

            _formAddUser.ShowDialog();
        }

        private void butDeleteUser_Click(object sender, EventArgs e)
        {
            var user = GetCurrentUser();
            if (user != null)
            {
                if (MessageBox.Show(LanguageManager.Get("are_you_sure"), LanguageManager.Get("warning"), MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _dbContext.Users.Remove(user);
                    _dbContext.SaveChanges();
                    LoadUsers();
                }
            }
            else
            {
                MessageBox.Show(LanguageManager.Get("row_not_selected"), LanguageManager.Get("warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

    }
}