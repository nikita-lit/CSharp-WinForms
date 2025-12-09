namespace WinForms.CarsService.Elements
{
    public class TabControl2 : UserControl
    {
        private FlowLayoutPanel _headerPanel;
        private Panel _contentPanel;
        private List<Panel> _tabPages = new();

        private int _selectedIndex = -1;
        public int SelectedIndex => _selectedIndex;

        private Color _headerBg;
        private Color _contentBg;
        private Color _butBgColor;
        private Color _bugBgActive;

        public TabControl2(Color headerBg, Color contentBg, Color butBg, Color bugBgActive)
        {
            _headerBg = headerBg;
            _contentBg = contentBg;
            _butBgColor = butBg;
            _bugBgActive = bugBgActive;

            _headerPanel = new();
            _headerPanel.Dock = DockStyle.Top;
            _headerPanel.Height = 30;
            _headerPanel.BackColor = _headerBg;
            _headerPanel.FlowDirection = FlowDirection.LeftToRight;
            Controls.Add(_headerPanel);

            _contentPanel = new();
            _contentPanel.Dock = DockStyle.Fill;
            _contentPanel.BackColor = _contentBg;
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
            butTab.Height = _headerPanel.Height;
            butTab.FlatStyle = FlatStyle.Flat;
            butTab.FlatAppearance.BorderSize = 0;
            butTab.Margin = new Padding(0);
            butTab.ForeColor = Color.White;
            butTab.Click += (sender, e) => {
                if (sender is Button btn && btn.Tag is int index)
                    SelectTab(index);
            };

            _headerPanel.Controls.Add(butTab);

            if (_tabPages.Count == 1)
                SelectTab(0);
        }

        public void SelectTab(int index)
        {
            if (index < 0 || index >= _tabPages.Count) 
                return;

            _contentPanel.Controls.Clear();
            _contentPanel.Controls.Add(_tabPages[index]);

            _selectedIndex = index;

            foreach (Button btn in _headerPanel.Controls)
                btn.BackColor = (int)btn.Tag == index ? _bugBgActive : _butBgColor;
        }
    }
}
