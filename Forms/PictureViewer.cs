using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text.Json;
using System.Windows.Forms;

namespace WinForms
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
        public PictureView Picture;
        public ScrollableControl Scroll;

        public PictureTab(
            PictureData data, 
            TabPage tab,
            PictureView pic, 
            ScrollableControl scroll)
        {
            Data = data;
            TabPage = tab;
            Picture = pic;
            Scroll = scroll;
            Picture.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        public void Update(bool strech, float zoom, bool isPixelated)
        {
            Scroll.VerticalScroll.Value = 0;
            Scroll.HorizontalScroll.Value = 0;

            Picture.IsPixelated = isPixelated;

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

        private bool _strechEnabled = false;
        private bool _pixelationEnabled = false;

        private ToolStripMenuItem _strechItem;
        private CheckBox _strechBox;

        private ToolStripMenuItem _pixelItem;
        private CheckBox _pixelBox;

        private ToolStripMenuItem _zoomItem;
        private Label _zoomLabel;
        private float _zoom = 1.0f;
        private List<int> _zoomPercs = new() { 25, 50, 75, 100, 125, 150, 175, 200, 300 };

        private ToolStripMenuItem _bgColorItem;
        private Color _bgColor = Color.Gray;

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
            BackColor = _bgColor;

            _butsPanel = new Panel() {
                Size = new Size(Width, 50),
                Dock = DockStyle.Bottom,
                Padding = new Padding(10),
                BackColor = Color.White,
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
        }

        protected override void OnLoad(EventArgs e)
        {
            if (Program.Args.Length > 0)
            {
                foreach (var path in Program.Args)
                    if (Path.Exists(path))
                        LoadImage(path);

                UpdateBgColor();
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

        private void UpdateCurrentTab()
        {
            var tab = GetCurrentPictureTab();
            tab?.Update(_strechEnabled, _zoom, _pixelationEnabled);
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
                if (ex is not FileNotFoundException)
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
                MessageBox.Show("Program author is nikita-lit");
            };

            helpMenu.DropDownItems.Add(aboutItem);

            //-------------------------------------
            ToolStripMenuItem viewMenu = new ToolStripMenuItem("View");
            _bgColorItem = new ToolStripMenuItem("Set Background Color");
            _bgColorItem.Click += (sender, e) => {
                AskToChangeBgColor();
            };

            UpdateToolStripItemColor();
            viewMenu.DropDownItems.Add(_bgColorItem);

            ToolStripSeparator sep = new ToolStripSeparator();
            viewMenu.DropDownItems.Add(sep);

            _strechItem = new ToolStripMenuItem("Strech");
            _strechItem.CheckOnClick = true;
            _strechItem.CheckedChanged += Strech_CheckedChanged;

            _pixelItem = new ToolStripMenuItem("Pixelation");
            _pixelItem.CheckOnClick = true;
            _pixelItem.CheckedChanged += Pixelation_CheckedChanged;

            viewMenu.DropDownItems.Add(_strechItem);
            viewMenu.DropDownItems.Add(_pixelItem);

            ToolStripSeparator sep2 = new ToolStripSeparator();
            viewMenu.DropDownItems.Add(sep2);

            _zoomItem = new ToolStripMenuItem($"Zoom {_zoomPerc}%");
            foreach (int perc in _zoomPercs)
            {
                ToolStripMenuItem zoomPerc = new ToolStripMenuItem($"{perc}%");
                zoomPerc.Click += (sender, e) => {
                    SetZoomPercentage(perc);
                };

                _zoomItem.DropDownItems.Add(zoomPerc);
            }

            viewMenu.DropDownItems.Add(_zoomItem);

            //-------------------------------------
            menu.Items.Add(_fileMenu);
            menu.Items.Add(viewMenu);
            menu.Items.Add(helpMenu);

            Controls.Add(menu);
            MainMenuStrip = menu;
        }

        private void Pixelation_CheckedChanged(object sender, EventArgs e)
        {
            bool pixel = false;

            if (sender == _pixelBox)
            {
                pixel = _pixelBox.Checked;
                if (_pixelItem.Checked != pixel)
                    _pixelItem.Checked = pixel;
            }
            else if (sender == _pixelItem)
            {
                pixel = _pixelItem.Checked;
                if (_pixelBox.Checked != pixel)
                    _pixelBox.Checked = pixel;
            }

            _pixelationEnabled = pixel;

            UpdateCurrentTab();
        }

        private void Strech_CheckedChanged(object sender, EventArgs e)
        {
            bool strech = false;

            if (sender == _strechBox)
            {
                strech = _strechBox.Checked;
                if (_strechItem.Checked != strech)
                    _strechItem.Checked = strech;
            }
            else if (sender == _strechItem)
            {
                strech = _strechItem.Checked;
                if (_strechBox.Checked != strech)
                    _strechBox.Checked = strech;
            }

            _strechEnabled = strech;

            UpdateCurrentTab();
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
            foreach (string path in revHis.GetRange(0, Math.Min(revHis.Count, 10)))
            {
                ToolStripMenuItem hisItem = new ToolStripMenuItem($"{num}: {path}");
                hisItem.Click += (sendSer, e) => {
                    LoadImage(path);
                };

                _fileMenu.DropDownItems.Add(hisItem);
                _historyItems.Add(hisItem);
                num++;
            }

            if (revHis.Count > 0)
            {
                ToolStripSeparator sep2 = new ToolStripSeparator();
                _fileMenu.DropDownItems.Add(sep2);
            }

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

            PictureView pic = new PictureView();
            pic.BackColor = Color.Transparent;
            pic.SizeMode = PictureBoxSizeMode.Normal;
            pic.Size = new Size(data.Image.Width, data.Image.Height);
            pic.Image = data.Image;

            scroll.Controls.Add(pic);
            tab.Controls.Add(scroll);
            tab.Tag = new PictureTab(data, tab, pic, scroll);

            _tabControl.TabPages.Add(tab);
            _tabControl.SelectedTab = tab;
        }

        private void InitTabControl()
        {
            _tabControl = new TabControl();
            _tabControl.Dock = DockStyle.Fill;
            _tabControl.BackColor = Color.White;
            _tabControl.SizeChanged += (sender, e) => {
                UpdateCurrentTab();
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

            ToolStripSeparator sep = new ToolStripSeparator();
            _tabContextMenu.Items.Add(sep);

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

            _tabControl.Visible = false;
            Controls.Add(_tabControl);
        }

        private void OnTabChanged()
        {
            var tab = GetCurrentPictureTab();
            if (tab != null)
            {
                tab.Update(_strechEnabled, _zoom, _pixelationEnabled);
                Text = "Picture Viewer - " + Path.GetFileName(tab.Data.FilePath);
                _tabControl.Visible = true;
            }
            else
            {
                Text = "Picture Viewer";
                _tabControl.Visible = false;
            }

            if (_tabControl.Visible)
                BackColor = Color.White;
            else
                BackColor = _bgColor;
        }

        private void InitCheckBox()
        {
            _strechBox = new CheckBox()
            {
                Text = "Stretch",
                Dock = DockStyle.Left,
            };
            _strechBox.CheckedChanged += Strech_CheckedChanged;
            _butsPanel.Controls.Add(_strechBox);

            _pixelBox = new CheckBox()
            {
                Text = "Pixelation",
                Dock = DockStyle.Left,
            };
            _pixelBox.CheckedChanged += Pixelation_CheckedChanged;
            _butsPanel.Controls.Add(_pixelBox);
        }

        private void InitButtons()
        {
            Button zoom1 = new Button()
            {
                Text = "Zoom In",
                Dock = DockStyle.Right,
                AutoSize = true,
            };
            zoom1.Click += (sender, e) => {
                int index = _zoomPercs.IndexOf(_zoomPerc) + 1;
                if (index > -1 && index < _zoomPercs.Count)
                    SetZoomPercentage(_zoomPercs[index]);
            };

            Button zoom2 = new Button()
            {
                Text = "Zoom Out",
                Dock = DockStyle.Right,
                AutoSize = true,
            };
            zoom2.Click += (sender, e) => {
                int index = _zoomPercs.IndexOf(_zoomPerc) + -1;
                if (index > -1 &&  index < _zoomPercs.Count)
                    SetZoomPercentage(_zoomPercs[index]);
            };

            _zoomLabel = new Label()
            {
                Text = $"{_zoomPerc}%",
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Right,
                Width = 40,
            };
            
            _butsPanel.Controls.Add(_zoomLabel);
            _butsPanel.Controls.Add(zoom1);
            _butsPanel.Controls.Add(zoom2);
        }

        private int _zoomPerc = 100;

        private void SetZoomPercentage(int percent)
        {
            _zoomPerc = percent;
            _zoomPerc = Math.Clamp(_zoomPerc, 10, 400);
            _zoomItem.Text = $"Zoom {_zoomPerc}%";
            _zoomLabel.Text = $"{_zoomPerc}%";
            SetZoom(_zoomPerc);
        }

        private void SetZoom(int percent)
        {
            _zoom = (percent / 100.0f);
            foreach (TabPage tab in _tabControl.TabPages)
            {
                var picTab = tab.Tag as PictureTab;
                picTab.Update(_strechEnabled, _zoom, _pixelationEnabled);
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

        private void UpdateToolStripItemColor()
        {
            _bgColorItem.Image?.Dispose();

            var bmp = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(bmp))
                g.Clear(_bgColor);

            _bgColorItem.Image = bmp;
        }

        private void AskToChangeBgColor()
        {
            if (_colorDialog.ShowDialog() == DialogResult.OK)
            {
                _bgColor = _colorDialog.Color;
                if (_tabControl.Visible)
                    BackColor = Color.White;
                else
                    BackColor = _bgColor;

                UpdateBgColor();
            }
        }

        private void UpdateBgColor()
        {

            UpdateToolStripItemColor();
            foreach (TabPage tab in _tabControl.TabPages)
            {
                var picTab = tab.Tag as PictureTab;
                picTab.Scroll.BackColor = _bgColor;
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
                        _tabControl.SelectedIndex = i;
                        break;
                    }
                }
            }
        }
    }
}
