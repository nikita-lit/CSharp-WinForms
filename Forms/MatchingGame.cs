using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace WinForms
{
    public partial class MatchingGame : Form
    {
        private TableLayoutPanel _tableLayoutPanel1;
        private Timer _revealTimer;
        private Timer _stopwatchTimer;
        private Label _timerLabel;
        private Panel _menuPanel;
        private Timer _colorFadeTimer;

        private List<string> _icons;
        private Label _firstClicked;
        private Label _secondClicked;
        private int _elapsedSeconds;
        private bool _gameRunning;
        private float _hue;

        private int _rows;
        private int _cols;

        private enum Difficulty { Easy, Normal, Hard }
        private Difficulty _currentDifficulty;

        public MatchingGame()
        {
            InitializeComponent();
            DoubleBuffered = true;
            ShowMenu();
        }

        private void ShowMenu()
        {
            Controls.Clear();
            _gameRunning = false;
            _firstClicked = null;
            _secondClicked = null;

            _menuPanel = new Panel();
            _menuPanel.Dock = DockStyle.Fill;
            _menuPanel.BackColor = Color.SteelBlue;

            Label title = new Label();
            title.Text = "🧩 Mängu sobitamine";
            title.Font = new Font("Segoe UI", 36, FontStyle.Bold);
            title.ForeColor = Color.White;
            title.Dock = DockStyle.Top;
            title.Height = 150;
            title.TextAlign = ContentAlignment.BottomCenter;

            Button easyBtn = CreateMenuButton("Lihtne", Difficulty.Easy);
            Button normalBtn = CreateMenuButton("Normaalne", Difficulty.Normal);
            Button hardBtn = CreateMenuButton("Raske", Difficulty.Hard);

            FlowLayoutPanel buttonRow = new FlowLayoutPanel();
            buttonRow.Dock = DockStyle.Top;
            buttonRow.FlowDirection = FlowDirection.LeftToRight;
            buttonRow.WrapContents = false;
            buttonRow.AutoSize = true;
            buttonRow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            buttonRow.Padding = new Padding(0, 20, 0, 0);
            buttonRow.BackColor = Color.Transparent;

            buttonRow.Controls.Add(easyBtn);
            buttonRow.Controls.Add(normalBtn);
            buttonRow.Controls.Add(hardBtn);

            buttonRow.Width = easyBtn.Width + normalBtn.Width + hardBtn.Width + 40;
            buttonRow.Left = (ClientSize.Width - buttonRow.Width) / 2;

            _menuPanel.Controls.Add(title);
            _menuPanel.Controls.Add(buttonRow);
            Controls.Add(_menuPanel);

            _colorFadeTimer = new Timer();
            _colorFadeTimer.Interval = 40;
            _colorFadeTimer.Tick += ColorFadeTimer_Tick;
            _hue = 210f;
            _colorFadeTimer.Start();
        }

        private void ColorFadeTimer_Tick(object sender, EventArgs e)
        {
            _hue += 1.2f;
            if (_hue > 360f) _hue = 0f;
            _menuPanel.BackColor = ColorFromHSV(_hue, 0.45, 1);
        }

        private Button CreateMenuButton(string text, Difficulty diff)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Width = 160;
            btn.Height = 60;
            btn.BackColor = Color.White;
            btn.ForeColor = Color.SteelBlue;
            btn.Font = new Font("Arial", 16);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;

            btn.Click += delegate (object sender, EventArgs e)
            {
                _currentDifficulty = diff;
                SetupGame();
            };

            btn.MouseEnter += delegate (object sender, EventArgs e)
            {
                btn.BackColor = Color.LightGray;
            };

            btn.MouseLeave += delegate (object sender, EventArgs e)
            {
                btn.BackColor = Color.White;
            };

            return btn;
        }

        private void SetupGame()
        {
            if (_colorFadeTimer != null)
            {
                _colorFadeTimer.Stop();
                _colorFadeTimer.Dispose();
                _colorFadeTimer = null;
            }

            Controls.Clear();
            _gameRunning = true;
            _firstClicked = null;
            _secondClicked = null;
            _elapsedSeconds = 0;

            switch (_currentDifficulty)
            {
                case Difficulty.Easy:
                    _rows = 3;
                    _cols = 4;
                    _icons = new List<string>();
                    _icons.Add("!"); _icons.Add("!");
                    _icons.Add("N"); _icons.Add("N");
                    _icons.Add(","); _icons.Add(",");
                    _icons.Add("k"); _icons.Add("k");
                    _icons.Add("b"); _icons.Add("b");
                    _icons.Add("v"); _icons.Add("v");
                    break;

                case Difficulty.Normal:
                    _rows = 4;
                    _cols = 4;
                    _icons = new List<string>();
                    _icons.Add("!"); _icons.Add("!");
                    _icons.Add("N"); _icons.Add("N");
                    _icons.Add(","); _icons.Add(",");
                    _icons.Add("k"); _icons.Add("k");
                    _icons.Add("b"); _icons.Add("b");
                    _icons.Add("v"); _icons.Add("v");
                    _icons.Add("w"); _icons.Add("w");
                    _icons.Add("z"); _icons.Add("z");
                    break;

                case Difficulty.Hard:
                    _rows = 5;
                    _cols = 4;
                    _icons = new List<string>();
                    _icons.Add("!"); _icons.Add("!");
                    _icons.Add("N"); _icons.Add("N");
                    _icons.Add(","); _icons.Add(",");
                    _icons.Add("k"); _icons.Add("k");
                    _icons.Add("b"); _icons.Add("b");
                    _icons.Add("v"); _icons.Add("v");
                    _icons.Add("w"); _icons.Add("w");
                    _icons.Add("z"); _icons.Add("z");
                    _icons.Add("$"); _icons.Add("$");
                    _icons.Add("@"); _icons.Add("@");
                    break;
            }

            _tableLayoutPanel1 = new TableLayoutPanel();
            _tableLayoutPanel1.RowCount = _rows;
            _tableLayoutPanel1.ColumnCount = _cols;
            _tableLayoutPanel1.Dock = DockStyle.Fill;
            _tableLayoutPanel1.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;

            for (int r = 0; r < _rows; r++)
                _tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / _rows));

            for (int c = 0; c < _cols; c++)
                _tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / _cols));

            Controls.Add(_tableLayoutPanel1);

            _timerLabel = new Label();
            _timerLabel.AutoSize = true;
            _timerLabel.Padding = new Padding(10, 5, 10, 5);
            _timerLabel.TextAlign = ContentAlignment.MiddleLeft;
            _timerLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            _timerLabel.Text = "Aeg: 0s";
            _timerLabel.BackColor = Color.SteelBlue;
            _timerLabel.ForeColor = Color.White;
            _timerLabel.Dock = DockStyle.Top;
            Controls.Add(_timerLabel);
            Controls.SetChildIndex(_timerLabel, 0);

            _revealTimer = new Timer();
            _revealTimer.Interval = (_currentDifficulty == Difficulty.Hard) ? 500 : 750;
            _revealTimer.Tick += RevealTimer_Tick;

            _stopwatchTimer = new Timer();
            _stopwatchTimer.Interval = 1000;
            _stopwatchTimer.Tick += StopwatchTimer_Tick;

            AssignIconsToSquares();
            _stopwatchTimer.Start();
        }

        private void RevealTimer_Tick(object sender, EventArgs e)
        {
            _revealTimer.Stop();
            _firstClicked.ForeColor = _firstClicked.BackColor;
            _secondClicked.ForeColor = _secondClicked.BackColor;
            _firstClicked = null;
            _secondClicked = null;
        }

        private void StopwatchTimer_Tick(object sender, EventArgs e)
        {
            _elapsedSeconds++;
            _timerLabel.Text = $"Aeg: {_elapsedSeconds}s";
        }

        private void AssignIconsToSquares()
        {
            Random rand = new Random();
            var shuffled = _icons.OrderBy(x => rand.Next()).ToList();
            int i = 0;

            _tableLayoutPanel1.Controls.Clear();

            for (int r = 0; r < _rows; r++)
            {
                for (int c = 0; c < _cols; c++)
                {
                    if (i >= shuffled.Count) break;

                    Label lbl = new Label
                    {
                        Dock = DockStyle.Fill,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Font = new Font("Webdings", 48, FontStyle.Bold),
                        BackColor = Color.CornflowerBlue,
                        ForeColor = Color.CornflowerBlue,
                        Text = shuffled[i]
                    };
                    lbl.Click += Label_Click;
                    _tableLayoutPanel1.Controls.Add(lbl, c, r);
                    i++;
                }
            }
        }

        private void Label_Click(object sender, EventArgs e)
        {
            if (!_gameRunning || _revealTimer.Enabled) return;

            Label clickedLabel = sender as Label;
            if (clickedLabel == null) return;
            if (clickedLabel.ForeColor == Color.Black) return;

            clickedLabel.ForeColor = Color.Black;

            if (_firstClicked == null)
            {
                _firstClicked = clickedLabel;
                return;
            }

            _secondClicked = clickedLabel;

            if (_firstClicked.Text == _secondClicked.Text)
            {
                AnimateMatch(_firstClicked);
                AnimateMatch(_secondClicked);
                _firstClicked = null;
                _secondClicked = null;
                CheckForWinner();
            }
            else
            {
                _revealTimer.Start();
            }
        }
        private void CheckForWinner()
        {
            bool hasHidden = false;
            foreach (Control control in _tableLayoutPanel1.Controls)
            {
                Label lbl = control as Label;
                if (lbl != null && lbl.ForeColor == lbl.BackColor)
                {
                    hasHidden = true;
                    break;
                }
            }

            if (!hasHidden)
            {
                _stopwatchTimer.Stop();
                _gameRunning = false;
                MessageBox.Show("Sa leidsid kõik ikoonid üles " + _elapsedSeconds + " sekundiga!", "Õnnitlused!!!!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ShowMenu();
            }
        }

        private void AnimateMatch(Label lbl)
        {
            Color start = lbl.BackColor;
            Color end = Color.LightGreen;
            int steps = 10;
            int currentStep = 0;

            Timer fadeTimer = new Timer();
            fadeTimer.Interval = 30;
            fadeTimer.Tick += delegate (object sender, EventArgs e)
            {
                float t = (float)currentStep / (float)steps;
                int r = (int)(start.R + (end.R - start.R) * t);
                int g = (int)(start.G + (end.G - start.G) * t);
                int b = (int)(start.B + (end.B - start.B) * t);
                lbl.BackColor = Color.FromArgb(r, g, b);

                currentStep += 1;
                if (currentStep > steps)
                {
                    lbl.BackColor = end;
                    fadeTimer.Stop();
                    fadeTimer.Dispose();
                }
            };
            fadeTimer.Start();
        }

        private Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = (int)Math.Floor(hue / 60) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);
            value = value * 255;
            int v = (int)value;
            int p = (int)(value * (1 - saturation));
            int q = (int)(value * (1 - f * saturation));
            int t = (int)(value * (1 - (1 - f) * saturation));

            if (hi == 0) return Color.FromArgb(255, v, t, p);
            if (hi == 1) return Color.FromArgb(255, q, v, p);
            if (hi == 2) return Color.FromArgb(255, p, v, t);
            if (hi == 3) return Color.FromArgb(255, p, q, v);
            if (hi == 4) return Color.FromArgb(255, t, p, v);
            return Color.FromArgb(255, v, p, q);
        }
    }
}
