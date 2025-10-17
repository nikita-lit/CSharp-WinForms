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

    public partial class PictureViewer : Form
    {
        private Panel _butsPanel;
        private OpenFileDialog _openFileDialog;
        private ColorDialog _colorDialog;
        private PictureBox _picture;
        private PictureData _curPicData;

        public PictureViewer()
        {
            InitializeComponent();

            Height = 800;
            Width = 1280;
            Text = "Picture Viewer";

            _butsPanel = new Panel() {
                Size = new Size(Width, 50),
                Dock = DockStyle.Bottom,
                Padding = new Padding(10),
            };

            InitPicture();
            InitCheckBox();
            InitButtons();

            Controls.Add(_butsPanel);

            _openFileDialog = new OpenFileDialog();
            _openFileDialog.Filter = "JPEG Files (*.jpg)|*.jpg|PNG Files (*.png)|*.png|All files (*.*)|*.*";

            _colorDialog = new ColorDialog();

            if (Program.Args.Length > 0)
            {
                var path = Program.Args[0];
                if (Path.Exists(path))
                    LoadImage(path);
            }
        }

        private void LoadImage(string path)
        {
            try
            {
                _curPicData = new PictureData(path);
                _picture.Image = _curPicData.Image;
                Text = "Picture Viewer - " + Path.GetFileName(_curPicData.FilePath);
            }
            catch (Exception ex)
            { 
               MessageBox.Show(ex.ToString(), "Image loading error!", MessageBoxButtons.OK, MessageBoxIcon.Error); 
            }
        }

        private void InitPicture()
        {
            _picture = new PictureBox();
            _picture.Dock = DockStyle.Fill;
            _picture.BackColor = Color.Gray;

            Controls.Add(_picture);
        }

        private void InitCheckBox()
        {
            CheckBox strechCBox = new CheckBox()
            {
                Text = "Strech",
                Dock = DockStyle.Left,
            };
            strechCBox.CheckedChanged += (sender, e) => {
                if (strechCBox.Checked)
                    _picture.SizeMode = PictureBoxSizeMode.StretchImage;
                else
                    _picture.SizeMode = PictureBoxSizeMode.Normal;
            };
            _butsPanel.Controls.Add(strechCBox);
        }

        private void InitButtons()
        {
            Button showBut = new Button()
            {
                Text = "Show a picture",
                Dock = DockStyle.Right,
                AutoSize = true,
            };
            showBut.Click += ShowBut_Click;
            _butsPanel.Controls.Add(showBut);

            Button colorBut = new Button()
            {
                Text = "Set the background color",
                Dock = DockStyle.Right,
                AutoSize = true,
            };
            colorBut.Click += ColorBut_Click;
            _butsPanel.Controls.Add(colorBut);

            Button clearBut = new Button()
            {
                Text = "Clear the picture",
                Dock = DockStyle.Right,
                AutoSize = true,
            };
            clearBut.Click += ClearBut_Click;
            _butsPanel.Controls.Add(clearBut);

            Button closeBut = new Button()
            {
                Text = "Close",
                Dock = DockStyle.Right,
                AutoSize = true,
            };
            closeBut.Click += CloseBut_Click;
            _butsPanel.Controls.Add(closeBut);
        }

        private void ShowBut_Click(object sender, EventArgs e)
        {
            if (_openFileDialog.ShowDialog() == DialogResult.OK)
                LoadImage(_openFileDialog.FileName);
        }

        private void ColorBut_Click(object sender, EventArgs e)
        {
            if (_colorDialog.ShowDialog() == DialogResult.OK)
                _picture.BackColor = _colorDialog.Color;
        }

        private void ClearBut_Click(object sender, EventArgs e)
        {
            Text = "Picture Viewer";
            _picture.Image = null;
            _curPicData = null;
        }

        private void CloseBut_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
