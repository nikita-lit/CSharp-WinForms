using System.Diagnostics;
using System.Text.Json;

namespace WinFormsUlesanne
{
    public class PictureData
    {
        public readonly string FilePath;
        public readonly Bitmap Image;

        public Size Size => Image.Size;
        public string FileName => Path.GetFileName(FilePath);

        public PictureData(string path)
        {
            FilePath = path;
            Image = (Bitmap)Bitmap.FromFile(path);
        }
    }

    public class PictureTab
    {
        public PictureData Data;
        public TabPage TabPage;
        public PictureBox Picture;
        public ScrollableControl Scroll;

        public PictureTab(
            PictureData data, 
            TabPage tab, 
            PictureBox pic, 
            ScrollableControl scroll)
        {
            Data = data;
            TabPage = tab;
            Picture = pic;
            Scroll = scroll;
            Picture.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        public void UpdateSizeMode(bool strech, float zoom)
        {
            Scroll.VerticalScroll.Value = 0;
            Scroll.HorizontalScroll.Value = 0;

            if (strech)
                Picture.Size = Scroll.Size;
            else
                Picture.Size = new Size((int)((float)Data.Image.Width * zoom), (int)((float)Data.Image.Height * zoom));
        }
    }

    public partial class PictureViewer : Form
    {
        private Panel _butsPanel;
        private OpenFileDialog _openFileDialog;
        private ColorDialog _colorDialog;
        private ToolStripMenuItem _fileMenu;
        private TabControl _tabControl;
        private ContextMenuStrip _tabContextMenu;
        private CheckBox _strechCBox;

        private float _zoom = 1.5f;
        private Color _bgColor = Color.Gray;
        private bool ImageStrech => _strechCBox.Checked;

        public PictureTab GetCurrentPictureTab()
        {
            if (_tabControl.SelectedTab == null)
                return null;

            return _tabControl.SelectedTab.Tag as PictureTab;
        }

        private List<string> _history = new();
        private List<ToolStripMenuItem> _historyItems = new();
        public static string HistoryPath => Path.Combine(Program.GetDirectory(), "history.json");

        public PictureViewer()
        {
            InitializeComponent();

            LoadHistory();

            Height = 800;
            Width = 1280;
            Text = "Picture Viewer";

            _butsPanel = new Panel() {
                Size = new Size(Width, 50),
                Dock = DockStyle.Bottom,
                Padding = new Padding(10),
            };

            InitTabControl();
            InitCheckBox();
            InitButtons();
            InitMenu();

            Controls.Add(_butsPanel);

            _openFileDialog = new OpenFileDialog();
            _openFileDialog.Filter = "JPEG Files (*.jpg)|*.jpg|PNG Files (*.png)|*.png|All files (*.*)|*.*";
            _openFileDialog.Multiselect = true;

            _colorDialog = new ColorDialog();

            if (Program.Args.Length > 0)
            {
                foreach (var path in Program.Args)
                    if (Path.Exists(path))
                        LoadImage(path);
            }
        }

        private void LoadImage(string path)
        {
            try
            {
                foreach (TabPage tab in _tabControl.TabPages)
                {
                    var picTab = tab.Tag as PictureTab;
                    if (string.Equals(picTab.Data.FilePath, path, StringComparison.OrdinalIgnoreCase))
                    {
                        _tabControl.SelectedTab = picTab.TabPage;
                        return;
                    }
                }

                var data = new PictureData(path);
                CreateImagePage(data);

                OnTabChanged();

                AddHistory(data);
                LoadFileMenu();
            }
            catch (Exception ex)
            { 
               MessageBox.Show(ex.ToString(), "Image loading error!", MessageBoxButtons.OK, MessageBoxIcon.Error); 
            }
        }

        private void CloseTab()
        {
            if (_tabControl.SelectedIndex < 0)
                return;

            _tabControl.TabPages.RemoveAt(_tabControl.SelectedIndex);
        }

        private void CloseAllTab()
        {
            _tabControl.TabPages.Clear();
        }

        private void CloseTab(int index)
        {
            if (index < 0 || index > (_tabControl.TabCount - 1))
                return;

            _tabControl.TabPages.RemoveAt(index);
        }

        private void LoadHistory()
        {
            try
            {
                var json = File.ReadAllText(HistoryPath);
                _history = JsonSerializer.Deserialize<List<string>>(json);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "History loading error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddHistory(PictureData data)
        {
            if (_history.Contains(data.FilePath))
                return;

            _history.Add(data.FilePath);

            try
            {
                string json = JsonSerializer.Serialize(_history);
                File.WriteAllText(Path.Combine(Program.GetDirectory(), "history.json"), json);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "History saving error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitMenu()
        {
            var menu = new MenuStrip();

            //-------------------------------------
            _fileMenu = new ToolStripMenuItem("File");
            LoadFileMenu();

            //-------------------------------------
            ToolStripMenuItem helpMenu = new ToolStripMenuItem("Help");
            ToolStripMenuItem aboutItem = new ToolStripMenuItem("About");
            aboutItem.Click += (sender, e) => {
            };

            helpMenu.DropDownItems.Add(aboutItem);

            //-------------------------------------
            ToolStripMenuItem viewMenu = new ToolStripMenuItem("View");
            ToolStripMenuItem bgColorItem = new ToolStripMenuItem("Set Background Color");
            bgColorItem.Click += (sender, e) => {
                AskToChangeBgColor();
            };

            viewMenu.DropDownItems.Add(bgColorItem);

            //-------------------------------------
            menu.Items.Add(_fileMenu);
            menu.Items.Add(viewMenu);
            menu.Items.Add(helpMenu);

            Controls.Add(menu);
            MainMenuStrip = menu;
        }

        private void LoadFileMenu()
        {
            _fileMenu.DropDownItems.Clear();

            //-------------------------------------
            ToolStripMenuItem openItem = new ToolStripMenuItem("Open...");
            openItem.Click += (sender, e) => {
                AskToOpenFile();
            };

            ToolStripMenuItem closeItem = new ToolStripMenuItem("Close");
            closeItem.Click += (sender, e) => {
                CloseTab();
            };

            ToolStripMenuItem closeAllItem = new ToolStripMenuItem("Close All");
            closeAllItem.Click += (sender, e) => {
                CloseAllTab();
            };

            ToolStripSeparator sep = new ToolStripSeparator();

            _fileMenu.DropDownItems.Add(openItem);
            _fileMenu.DropDownItems.Add(closeItem);
            _fileMenu.DropDownItems.Add(closeAllItem);
            _fileMenu.DropDownItems.Add(sep);

            //-------------------------------------
            var revHis = _history.ToList();
            revHis.Reverse();

            int num = 1;
            foreach (string path in revHis.GetRange(0, 10))
            {
                ToolStripMenuItem hisItem = new ToolStripMenuItem($"{num}: {path}");
                hisItem.Click += (sender, e) => {
                    LoadImage(path);
                };

                _fileMenu.DropDownItems.Add(hisItem);
                _historyItems.Add(hisItem);
                num++;
            }

            ToolStripSeparator sep2 = new ToolStripSeparator();
            _fileMenu.DropDownItems.Add(sep2);

            //-------------------------------------
            ToolStripMenuItem exitItem = new ToolStripMenuItem("Exit");
            exitItem.Click += (sender, e) => {
                Close();
            };

            _fileMenu.DropDownItems.Add(exitItem);
        }

        private void CreateImagePage(PictureData data)
        {
            TabPage tab = new TabPage($"{_tabControl.TabCount + 1} - {data.FileName}");
            tab.BackColor = Color.Transparent;

            ScrollableControl scroll = new ScrollableControl();
            scroll.AutoScroll = true;
            scroll.BackColor = _bgColor;
            scroll.Dock = DockStyle.Fill;

            PictureBox pic = new PictureBox();
            pic.BackColor = Color.Transparent;
            pic.SizeMode = PictureBoxSizeMode.Normal;
            pic.Size = new Size(data.Image.Width, data.Image.Height);
            pic.Image = data.Image;

            scroll.Controls.Add(pic);
            tab.Controls.Add(scroll);
            tab.Tag = new PictureTab(data, tab, pic, scroll);

            _tabControl.TabPages.Add(tab);
            _tabControl.SelectedTab = tab;

            SetZoom(2f);
        }

        private void InitTabControl()
        {
            _tabControl = new TabControl();
            _tabControl.Dock = DockStyle.Fill;
            _tabControl.BackColor = Color.Red;
            _tabControl.SizeChanged += (sender, e) => {
                var tab = GetCurrentPictureTab();
                tab?.UpdateSizeMode(ImageStrech, _zoom);
            };
            _tabControl.SelectedIndexChanged += (sender, e) => {
                OnTabChanged();
            };
            _tabControl.MouseDown += TabControl1_MouseDown;

            _tabContextMenu = new ContextMenuStrip();
            ToolStripMenuItem close = new ToolStripMenuItem("Close");
            ToolStripMenuItem open = new ToolStripMenuItem("Open");
            ToolStripMenuItem openExplorer = new ToolStripMenuItem("Open in File Explorer");

            _tabContextMenu.Items.Add(close);
            _tabContextMenu.Items.Add(open);
            _tabContextMenu.Items.Add(openExplorer);

            open.Click += (sender, e) => {
                var index = (int)_tabContextMenu.Tag;
                _tabControl.SelectedIndex = index;
            };
            close.Click += (sender, e) => {
                var index = (int)_tabContextMenu.Tag;
                CloseTab(index);
            };
            openExplorer.Click += (sender, e) => {
                var index = (int)_tabContextMenu.Tag;
                var filePath = (_tabControl.TabPages[index].Tag as PictureTab).Data.FilePath;
                var dir = Path.GetDirectoryName(filePath);
                if (Path.Exists(dir))
                    Process.Start("explorer.exe", $"/select, {filePath}");
            };

            Controls.Add(_tabControl);
        }

        private void OnTabChanged()
        {
            var tab = GetCurrentPictureTab();
            if (tab != null)
            {
                tab.UpdateSizeMode(ImageStrech, _zoom);
                Text = "Picture Viewer - " + Path.GetFileName(tab.Data.FilePath);
            }
            else
            {
                Text = "Picture Viewer";
                _strechCBox.Checked = false;
            }
        }

        private void InitCheckBox()
        {
            _strechCBox = new CheckBox()
            {
                Text = "Stretch",
                Dock = DockStyle.Left,
            };
            _strechCBox.CheckedChanged += (sender, e) => {
                var tab = GetCurrentPictureTab();
                if (tab != null)
                    tab.UpdateSizeMode(ImageStrech, _zoom);
            };
            _butsPanel.Controls.Add(_strechCBox);
        }

        private void InitButtons()
        {
            //Button showBut = new Button()
            //{
            //    Text = "Show a picture",
            //    Dock = DockStyle.Right,
            //    AutoSize = true,
            //};
            //showBut.Click += ShowBut_Click;
            //_butsPanel.Controls.Add(showBut);
        }

        private void SetZoom(float zoom)
        {
            if (ImageStrech) return;

            _zoom = zoom;
            foreach (TabPage tab in _tabControl.TabPages)
            {
                var picTab = tab.Tag as PictureTab;
                var width = (int)((float)picTab.Data.Image.Width * _zoom);
                var height = (int)((float)picTab.Data.Image.Height * _zoom);
                picTab.Picture.Size = new Size(width, height);
            }
        }

        private void AskToOpenFile()
        {
            if (_openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (var fileName in _openFileDialog.FileNames)
                    LoadImage(fileName);
            }
        }

        private void AskToChangeBgColor()
        {
            if (_colorDialog.ShowDialog() == DialogResult.OK)
            {
                _bgColor = _colorDialog.Color;
                foreach (TabPage tab in _tabControl.TabPages)
                {
                    var picTab = tab.Tag as PictureTab;
                    picTab.Scroll.BackColor = _bgColor;
                }
            }
        }

        private void TabControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                for (int i = 0; i < _tabControl.TabCount; i++)
                {
                    Rectangle tabRect = _tabControl.GetTabRect(i);
                    if (tabRect.Contains(e.Location))
                    {
                        _tabContextMenu.Tag = i;
                        _tabContextMenu.Show(_tabControl, e.Location);
                        break;
                    }
                }
            }
        }
    }
}
