using Timer = System.Windows.Forms.Timer;

namespace WinForms
{
    public partial class MatchingGame : Form
    {
        private bool _isStarted = false;
        private int _score = 0;
        private int _mistakes = 0;
        private int _maxMistakes = 0;
        private int _gridSize = 4;
        private int _timeLeft;
        private int _time = 60;

        private Timer _revealTimer;
        private Timer _timer;

        private Label _labelTimer;
        private Label _labelMistakes;
        private TableLayoutPanel _table;

        private PictureBox _firstClicked;
        private PictureBox _secondClicked;
        private Button _startButton;
        private ComboBox _cbGrid, _cbDiff;

        private Timer _highlightTimer;
        private Color _targetColor;
        private PictureBox _highlightedPic1;
        private PictureBox _highlightedPic2;
        private Label _matchingGame;
        private int _highlightStep;

        private Difficulty _difficulty = Difficulty.Easy;

        List<string> _icons = ["about.png", "bagguete.png", "close_box_red.png", "shop.png", "rtx5090.png", "mango.png", "esimene.jpg", "teine.jpg"];

        public MatchingGame()
        {
            Text = "Matching Game";
            Width = 800;
            Height = 900;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MinimizeBox = false;
            MaximizeBox = false;

            Panel panel = new();
            panel.Size = new Size(650, 650);
            panel.BackColor = Color.Transparent;
            panel.Location = new Point((Width / 2)-(panel.Width / 2), (Height / 2) - (panel.Height / 2)-50);

            _table = new();
            _table.RowCount = _gridSize;
            _table.ColumnCount = _gridSize;
            _table.Dock = DockStyle.Fill;
            _table.BackColor = Color.LightGreen;
            panel.Controls.Add(_table);

            _labelTimer = new();
            _labelTimer.Text = "Time Left: " + _timeLeft + "s";
            _labelTimer.Font = new Font("Arial", 18);
            _labelTimer.AutoSize = true;
            _labelTimer.Visible = false;

            _labelMistakes = new();
            _labelMistakes.Text = "Mistakes: " + _mistakes + (_maxMistakes > 0 ? "/"+ _maxMistakes : "");
            _labelMistakes.Font = new Font("Arial", 18);
            _labelMistakes.AutoSize = true;
            _labelMistakes.Visible = false;

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
                _gridSize = size;
                _table.RowCount = _gridSize;
                _table.ColumnCount = _gridSize;
                _table.RowStyles.Clear();
                _table.ColumnStyles.Clear();

                for (int i = 0; i < _gridSize; i++)
                    _table.RowStyles.Add(new RowStyle(SizeType.Percent, 25.0f));

                for (int i = 0; i < _gridSize; i++)
                    _table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25.0f));

                UpdateTimeAndMistakes(size);
            }

            void UpdateTimeAndMistakes(int size)
            {
                if (size == 4)
                    _time = GetTimeByDifficulty(60);
                else if (size == 6)
                    _time = GetTimeByDifficulty(120);
                else if (size == 8)
                    _time = GetTimeByDifficulty(180);
                else
                    _time = GetTimeByDifficulty(60);

                _maxMistakes = GetMaxMistakes(size * size);
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
                UpdateTimeAndMistakes(_gridSize);
            };

            Controls.Add(panel);
            Controls.Add(_labelTimer);
            Controls.Add(_labelMistakes);
            Controls.Add(_startButton);
            Controls.Add(_cbGrid);
            Controls.Add(_cbDiff);

            _startButton.Location = new Point((Width / 2) - (_startButton.Width / 2), Height - 140);
            _labelTimer.Location = new Point((800 - 650) - (_labelTimer.Width / 2), 25);
            _labelMistakes.Location = new Point((800 - 350) - (_labelTimer.Width / 2), 25);

            _matchingGame = new Label();
            _matchingGame.Text = "Matching Game";
            _matchingGame.Font = new Font("Arial", 25);
            _matchingGame.TextAlign = ContentAlignment.MiddleCenter;
            _matchingGame.AutoSize = true;

            Controls.Add(_matchingGame);
            _matchingGame.Location = new Point((Width / 2) - (_matchingGame.Width / 2), (Height / 2) - (_matchingGame.Width / 2));
            _matchingGame.BringToFront();

            _revealTimer = new();
            _revealTimer.Interval = 750;
            _revealTimer.Tick += RevealTimer_Tick;

            _timer = new();
            _timer.Interval = 1000;
            _timer.Tick += _timer_Tick;

            _highlightTimer = new Timer();
            _highlightTimer.Interval = 50;
            _highlightTimer.Tick += _highlightTimer_Tick;
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

            for (int i = 0; i < _gridSize * _gridSize; i++)
            {
                PictureBox pic = new();
                pic.Dock = DockStyle.Fill;
                pic.SizeMode = PictureBoxSizeMode.StretchImage;
                pic.BackColor = Color.Green;
                pic.ForeColor = Color.Green;
                pic.Margin = new Padding(5);
                pic.Click += pic_Click;

                _table.Controls.Add(pic);
            }

            SetIconsForPictures();
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            if (_timeLeft > 0)
            {
                _timeLeft--;
                _labelTimer.Text = "Time Left: " + _timeLeft + "s";
                if (_timeLeft <= 15)
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

            _mistakes = 0;
            _score = 0;
            _isStarted = true;
            _cbGrid.Visible = false;
            _cbDiff.Visible = false;
            _matchingGame.Visible = false;
            _timeLeft = _time;
            _labelMistakes.Text = "Mistakes: " + _mistakes + (_maxMistakes > 0 ? "/" + _maxMistakes : "");
            _labelTimer.Text = "Time Left: " + _timeLeft + "s";
            _labelTimer.Visible = true;
            _labelMistakes.Visible = true;
            _labelTimer.ForeColor = Color.Black;
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

            if (_score >= (_gridSize * _gridSize) / 2)
                MessageBox.Show("You matched all pairs!", "Victory");
            else if (_mistakes >= _maxMistakes)
                MessageBox.Show("Too many mistakes!", "Defeat");
            else
                MessageBox.Show("Not enough points!", "Defeat");

            _mistakes = 0;
            _score = 0;
            _cbGrid.Visible = true;
            _cbDiff.Visible = true;
            _matchingGame.Visible = true;
            _labelTimer.Visible = false;
            _labelMistakes.Visible = false;
            ClearPictures();

            _startButton.Text = "Start Game!";
        }

        private void SetIconsForPictures()
        {
            int picCount = _gridSize * _gridSize;
            int pairsCount = picCount / 2;

            List<string> iconsCopy = new List<string>();
            for (int i = 0; i < pairsCount; i++)
            {
                string icon = _icons[i];
                iconsCopy.Add(icon);
                iconsCopy.Add(icon);
            }

            foreach (PictureBox pic in _table.Controls.OfType<PictureBox>())
            {
                int index = Random.Shared.Next(iconsCopy.Count);
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
                HighlightPair(_firstClicked, _secondClicked, true);
                _firstClicked = null;
                _secondClicked = null;
                _score++;
                Check();
            }
            else
            {
                _mistakes++;
                _labelMistakes.Text = "Mistakes: " + _mistakes + (_maxMistakes > 0 ? "/" + _maxMistakes : "");
                _revealTimer.Start();
                HighlightPair(_firstClicked, _secondClicked, false);

                if (_mistakes >=  _maxMistakes)
                    EndGame();
            }
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

        private int GetMaxMistakes(int baseMistakes)
        {
            if (_difficulty == Difficulty.Easy)
                return baseMistakes;
            else if (_difficulty == Difficulty.Medium)
                return baseMistakes * 2 / 3;
            else if (_difficulty == Difficulty.Hard)
                return baseMistakes / 2;
            else
                return baseMistakes;
        }

        private void _highlightTimer_Tick(object sender, EventArgs e)
        {
            _highlightStep++;
            float ratio = _highlightStep / 10f;

            if (ratio > 1) 
                ratio = 1;

            _highlightedPic1.BackColor = LerpColor(Color.Green, _targetColor, ratio);
            _highlightedPic2.BackColor = LerpColor(Color.Green, _targetColor, ratio);

            if (_highlightStep >= 10)
            {
                _highlightTimer.Stop();

                _highlightedPic1.BackColor = Color.Green;
                _highlightedPic2.BackColor = Color.Green;
            }
        }


        private void HighlightPair(PictureBox pic1, PictureBox pic2, bool isCorrect)
        {
            _highlightedPic1 = pic1;
            _highlightedPic2 = pic2;
            _highlightStep = 0;
            _targetColor = isCorrect ? Color.LimeGreen : Color.Red;
            _highlightTimer.Start();
        }

        private Color LerpColor(Color c1, Color c2, float ratio)
        {
            int r = (int)(c1.R + (c2.R - c1.R) * ratio);
            int g = (int)(c1.G + (c2.G - c1.G) * ratio);
            int b = (int)(c1.B + (c2.B - c1.B) * ratio);
            return Color.FromArgb(r, g, b);
        }
    }
}