using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace WinForms
{
    public partial class MatchingGame : Form
    {
        private TableLayoutPanel _table;
        private Timer _revealTimer;
        private PictureBox _firstClicked;
        private PictureBox _secondClicked;
        private Random _random = new Random();

        private List<string> _icons = new List<string>()
        {
            "about.png", "about.png", "bagguete.png", "bagguete.png", "close_box_red.png", "close_box_red.png", "shop.png", "shop.png",
            "rtx5090.png", "rtx5090.png", "mango.png", "mango.png", "esimene.jpg", "esimene.jpg", "teine.jpg", "teine.jpg"
        };

        public MatchingGame()
        {
            Text = "Matching Game";
            Width = 600;
            Height = 600;

            _table = new();
            _table.RowCount = 4;
            _table.ColumnCount = 4;
            _table.Dock = DockStyle.Fill;
            _table.BackColor = Color.LightGreen;

            Controls.Add(_table);

            for (int i = 0; i < 4; i++)
            {
                _table.RowStyles.Add(new RowStyle(SizeType.Percent, 25.0f));
                _table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25.0f));
            }

            for (int i = 0; i < 16; i++)
            {
                PictureBox pic = new();
                pic.Dock = DockStyle.Fill;
                pic.SizeMode = PictureBoxSizeMode.StretchImage;
                pic.BackColor = Color.Transparent;
                pic.BackColor = Color.Green;
                pic.ForeColor = Color.Green;
                pic.Margin = new Padding(5);
                pic.Click += pic_Click;

                _table.Controls.Add(pic);
            }

            AssignIconsToSquares();

            _revealTimer = new();
            _revealTimer.Interval = 750;
            _revealTimer.Tick += RevealTimer_Tick;
        }

        private void AssignIconsToSquares()
        {
            List<string> iconsCopy = new List<string>(_icons);

            foreach (PictureBox pic in _table.Controls.OfType<PictureBox>())
            {
                int index = _random.Next(iconsCopy.Count);
                pic.Image = null;
                pic.Tag = iconsCopy[index];
                iconsCopy.RemoveAt(index);
            }
        }

        private void pic_Click(object sender, EventArgs e)
        {
            if (_revealTimer.Enabled) 
                return;

            var clickedPic = sender as PictureBox;
            if (clickedPic == null) 
                return;

            if (clickedPic.ForeColor == Color.Black)
                return;

            if (_firstClicked == null)
            {
                _firstClicked = clickedPic;
                ShowPictureBox(clickedPic);
                return;
            }

            _secondClicked = clickedPic;
            ShowPictureBox(clickedPic);

            if (_firstClicked.Tag.ToString() == _secondClicked.Tag.ToString())
            {
                _firstClicked = null;
                _secondClicked = null;
                Check();
            }
            else
                _revealTimer.Start();
        }

        private void RevealTimer_Tick(object sender, EventArgs e)
        {
            _revealTimer.Stop();

            HidePictureBox(_firstClicked);
            HidePictureBox(_secondClicked);

            _firstClicked = null;
            _secondClicked = null;
        }

        private void ShowPictureBox(PictureBox pic)
        {
            pic.Image = Image.FromFile(@"..\..\..\Images\" + pic.Tag.ToString());
            pic.ForeColor = Color.Black;
        }

        private void HidePictureBox(PictureBox pic)
        {
            pic.ForeColor = pic.BackColor;
            pic.Image = null;
        }

        private void Check()
        {
            foreach (PictureBox pic in _table.Controls.OfType<PictureBox>())
            {
                if (pic.ForeColor != Color.Black)
                    return;
            }

            MessageBox.Show("You matched all pairs!", "Victory");
        }
    }
}