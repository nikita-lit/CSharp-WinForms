using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace WinForms
{
    public partial class MatchingGame : Form
    {
        private Label _labelTimer;
        private TableLayoutPanel _table;
        private Timer _revealTimer;
        private Timer _timer;
        private int _timeLeft;
        private int _time = 60;
        private PictureBox _firstClicked;
        private PictureBox _secondClicked;
        private Random _random = new Random();
        private int _gridHeight = 4;
        private int _gridWidth = 4;
        private bool _isStarted = false;
        private Button _startButton;
        private ComboBox _cbGrid, _cbDiff;
        private int _score = 0;

        private Difficulty _difficulty = Difficulty.Easy;

        List<string> _icons = ["about.png", "bagguete.png", "close_box_red.png", "shop.png", "rtx5090.png", "mango.png", "esimene.jpg", "teine.jpg"];

        public MatchingGame()
        {
            Text = "Matching Game";
            Width = 800;
            Height = 900;

            Panel panel = new();
            panel.Size = new Size(650, 650);
            panel.BackColor = Color.Transparent;
            panel.Location = new Point((Width / 2)-(panel.Width / 2), (Height / 2) - (panel.Height / 2)-50);

            _table = new();
            _table.RowCount = _gridHeight;
            _table.ColumnCount = _gridWidth;
            _table.Dock = DockStyle.Fill;
            _table.BackColor = Color.LightGreen;
            panel.Controls.Add(_table);

            _labelTimer = new();
            _labelTimer.Text = "Time Left: " + _timeLeft;
            _labelTimer.Font = new Font("Arial", 18);
            _labelTimer.AutoSize = true;
            _labelTimer.Visible = false;

            _startButton = new();
            _startButton.Text = "Start Game!";
            _startButton.Font = new Font("Arial", 18);
            _startButton.AutoSize = true;
            _startButton.Click += (sender, e) => {
                if (!_isStarted)
                    StartGame();
                else
                    EndGame();
            };

            void SetTableSize(int size)
            {
                _gridHeight = size;
                _gridWidth = size;
                _table.RowCount = _gridHeight;
                _table.ColumnCount = _gridWidth;
                _table.RowStyles.Clear();
                _table.ColumnStyles.Clear();

                for (int i = 0; i < _gridHeight; i++)
                    _table.RowStyles.Add(new RowStyle(SizeType.Percent, 25.0f));

                for (int i = 0; i < _gridWidth; i++)
                    _table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25.0f));

                if (size == 4)
                    _time = GetTimeByDifficulty(60);
                else if (size == 6)
                    _time = GetTimeByDifficulty(120);
                else if (size == 8)
                    _time = GetTimeByDifficulty(180);
                else
                    _time = GetTimeByDifficulty(60);
            }

            SetTableSize(4);

            _cbGrid = new();
            _cbGrid.DropDownStyle = ComboBoxStyle.DropDownList;
            _cbGrid.Font = new Font("Arial", 16);
            _cbGrid.Items.AddRange(["4x4", "6x6", "8x8"]);
            _cbGrid.SelectedIndex = 0;
            _cbGrid.Location = new Point((Width / 2) - (_cbGrid.Width / 2) + 300, Height - 140);
            _cbGrid.SelectedValueChanged += (sender, e) => {
                if (_cbGrid.SelectedItem.ToString() == "4x4")
                    SetTableSize(4);
                else if (_cbGrid.SelectedItem.ToString() == "6x6")
                    SetTableSize(6);
                else if (_cbGrid.SelectedItem.ToString() == "8x8")
                    SetTableSize(8);
            };

            _cbDiff = new();
            _cbDiff.DropDownStyle = ComboBoxStyle.DropDownList;
            _cbDiff.Font = new Font("Arial", 16);
            _cbDiff.Items.AddRange(["Easy", "Medium", "Hard"]);
            _cbDiff.SelectedIndex = 0;
            _cbDiff.Location = new Point((Width / 2) - (_cbDiff.Width / 2) + 165, Height - 140);
            _cbDiff.SelectedIndexChanged += (sender, e) => {
                _difficulty = (Difficulty)_cbDiff.SelectedIndex;
            };

            Controls.Add(panel);
            Controls.Add(_labelTimer);
            Controls.Add(_startButton);
            Controls.Add(_cbGrid);
            Controls.Add(_cbDiff);

            _startButton.Location = new Point((Width / 2) - (_startButton.Width / 2), Height - 140);
            _labelTimer.Location = new Point((800 - 650) - (_labelTimer.Width / 2), 25);

            _revealTimer = new();
            _revealTimer.Interval = 750;
            _revealTimer.Tick += RevealTimer_Tick;

            _timer = new();
            _timer.Interval = 1000;
            _timer.Tick += _timer_Tick;
        }

        private void ClearPictures()
        {
            for (int i = _table.Controls.Count - 1; i >= 0; i--)
            {
                if (_table.Controls[i] is PictureBox)
                    _table.Controls.RemoveAt(i);
            }
        }

        private void GeneratePictures()
        {
            ClearPictures();

            for (int i = 0; i < _gridHeight * _gridWidth; i++)
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
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            if (_timeLeft > 0)
            {
                _timeLeft--;
                _labelTimer.Text = "Time Left: " + _timeLeft;
                if (_timeLeft <= 10)
                    _labelTimer.ForeColor = Color.Red;
            }
            else
                EndGame();
        }

        private void StartGame()
        {
            if (_isStarted) 
                return;

            GeneratePictures();

            _score = 0;
            _isStarted = true;
            _cbGrid.Visible = false;
            _timeLeft = _time;
            _labelTimer.Text = "Time Left: " + _timeLeft;
            _labelTimer.Visible = true;
            _timer.Start();

            _startButton.Text = "End Game";
        }

        private void EndGame()
        {
            if (!_isStarted) 
                return;

            _timer.Stop();
            _timeLeft = _time;
            _isStarted = false;

            Console.WriteLine(_score);
            if (_score >= (_gridWidth * _gridHeight) / 2)
                MessageBox.Show("You matched all pairs!", "Victory");
            else
                MessageBox.Show("Not enough points!", "Defeat");

            _score = 0;
            _cbGrid.Visible = true;
            _labelTimer.Visible = false;
            ClearPictures();

            _startButton.Text = "Start Game!";
        }

        private void AssignIconsToSquares()
        {
            int totalCells = _gridHeight * _gridWidth;
            int needPairs = totalCells / 2;

            List<string> iconsCopy = new List<string>();
            for (int i = 0; i < needPairs; i++)
            {
                string icon = _icons[i % _icons.Count];
                iconsCopy.Add(icon);
                iconsCopy.Add(icon);
            }

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

            if (_firstClicked.Tag.ToString() == _secondClicked.Tag.ToString() 
                && _secondClicked.BackColor != Color.Black)
            {
                _firstClicked = null;
                _secondClicked = null;
                _score++;
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

            EndGame();
        }

        private int GetTimeByDifficulty(int baseTime)
        {
            if (_difficulty == Difficulty.Easy)
                return baseTime;
            else if (_difficulty == Difficulty.Medium)
                return baseTime * 2 / 3;
            else if (_difficulty == Difficulty.Hard)
                return baseTime / 2;
            else
                return baseTime;
        }

    }
}