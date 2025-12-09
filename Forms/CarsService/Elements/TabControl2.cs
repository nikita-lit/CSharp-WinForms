namespace WinForms.CarsService.Elements
{
    public class TabControl2 : UserControl
    {
        private FlowLayoutPanel _headerPanel;
        private Panel _contentPanel;
        private List<Panel> _tabPages = new();

        private int _selectedIndex = -1;
        public int SelectedIndex => _selectedIndex;

        public TabControl2(Color headerBackColor)
        {
            _headerPanel = new();
            _headerPanel.Dock = DockStyle.Top;
            _headerPanel.Height = 30;
            _headerPanel.BackColor = headerBackColor;
            _headerPanel.FlowDirection = FlowDirection.LeftToRight;
            Controls.Add(_headerPanel);

            _contentPanel = new();
            _contentPanel.Dock = DockStyle.Fill;
            _contentPanel.BackColor = Color.White;
            Controls.Add(_contentPanel);
        }

        public void AddTab(Panel tabPage)
        {
            tabPage.Dock = DockStyle.Fill;
            _tabPages.Add(tabPage);

            Button butTab = new();
            butTab.Text = tabPage.Text;
            butTab.Tag = _tabPages.Count - 1;
            butTab.Width = 100;
            butTab.Height = tabHeaderPanel.Height;
            butTab.FlatStyle = FlatStyle.Flat;
            butTab.FlatAppearance.BorderSize = 0;
            butTab.Margin = new Padding(0);
            butTab.Click += (sender, e) => {
                if (sender is Button btn && btn.Tag is int index)
                    SelectTab(index);
            };

            tabHeaderPanel.Controls.Add(butTab);

            if (_tabPages.Count == 1)
                SelectTab(0);
        }

        public void SelectTab(int index)
        {
            if (index < 0 || index >= _tabPages.Count) 
                return;

            tabContentPanel.Controls.Clear();
            tabContentPanel.Controls.Add(_tabPages[index]);

            _selectedIndex = index;

            foreach (Button btn in tabHeaderPanel.Controls)
                btn.BackColor = (int)btn.Tag == index ? Color.White : Color.LightGray;
        }
    }
}
