using System.Text.Json;

namespace WinForms
{
    public enum Difficulty 
    { 
        Easy, 
        Medium, 
        Hard 
    }

    public class QuizResult
    {
        public DateTime Date { get; set; }
        public Difficulty Difficulty { get; set; }
        public int Correct { get; set; }
        public int Total { get; set; }
        public List<string> Answers { get; set; } = new List<string>();
    }

    public partial class MathQuiz : Form
    {
        private Font _font;
        private Label _labelTimer;
        private System.Windows.Forms.Timer _timer;
        private int _timeLeft;
        private int _time = 60;
        private Panel _butStartPanel;
        private bool _isStarted = false;
        private Button _startButton;
        private Button _historyButton;
        private ComboBox _cbDiff;
        private Label _mathQuiz;
        private Panel _numPanel;

        private Difficulty _difficulty = Difficulty.Easy;

        public MathQuiz()
        {
            InitializeComponent();

            Text = "Math Quiz";
            Size = new Size(700, 600);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MinimizeBox = false;
            MaximizeBox = false;

            _font = new Font("Arial", 25);

            _mathQuiz = new Label();
            _mathQuiz.Text = "Math Quiz";
            _mathQuiz.Font = _font;
            _mathQuiz.TextAlign = ContentAlignment.MiddleCenter;
            _mathQuiz.AutoSize = true;

            Controls.Add(_mathQuiz);
            _mathQuiz.Location = new Point((Width / 2) - (_mathQuiz.Width / 2), (Height / 2) - (_mathQuiz.Width / 2));

            CreateStartButton();
            CreateTimer();
        }

        private void CreateTimer()
        {
            _labelTimer = new Label();
            _labelTimer.Text = "Time Left: " + _time;
            _labelTimer.Dock = DockStyle.Top;
            _labelTimer.Font = _font;
            _labelTimer.Height = 50;
            _labelTimer.TextAlign = ContentAlignment.MiddleCenter;

            _timer = new();
            _timer.Interval = 1000;
            _timer.Tick += _timer_Tick;
        }

        private void CreateStartButton()
        {
            _butStartPanel = new Panel();
            _butStartPanel.Dock = DockStyle.Bottom;
            _butStartPanel.Padding = new Padding(20);
            _butStartPanel.Height = 120;

            _startButton = new Button();
            _startButton.Dock = DockStyle.Fill;
            _startButton.Font = _font;
            _startButton.Text = "Start the quiz!";
            _startButton.Click += (sender, e) => {
                if (!_isStarted)
                    StartQuiz();
                else
                    EndQuiz();
            };

            _historyButton = new Button();
            _historyButton.Dock = DockStyle.Left;
            _historyButton.Font = new Font("Arial", 12);
            _historyButton.Text = "History";
            _historyButton.Width = 80;
            _historyButton.Click += (sender, e) => {
                if (!_isStarted)
                    ShowHistory();
            };

            Panel p1 = new();
            p1.Width = 150;
            p1.Dock = DockStyle.Right;

            _cbDiff = new();
            _cbDiff.DropDownStyle = ComboBoxStyle.DropDownList;
            _cbDiff.Dock = DockStyle.Top;
            _cbDiff.Font = new Font("Arial", 16);
            _cbDiff.Items.AddRange(["Easy", "Medium", "Hard"]);
            _cbDiff.SelectedIndex = 0;
            _cbDiff.SelectedIndexChanged += (sender, e) => {
                _difficulty = (Difficulty)_cbDiff.SelectedIndex;
            };

            _numPanel = new();
            _numPanel.Size = new Size(120, 40);
            _numPanel.Dock = DockStyle.Top;

            Label label = new();
            label.Dock = DockStyle.Left;
            label.TextAlign = ContentAlignment.TopCenter;
            label.Text = "Time: ";
            label.BackColor = Color.Transparent;
            label.Font = new Font("Arial", 14);
            label.Width = 70;

            NumericUpDown numTime = new();
            numTime.Dock = DockStyle.Right;
            numTime.Font = new Font("Arial", 16);
            numTime.Width = 80;
            numTime.Minimum = 5;
            numTime.Maximum = 600;
            numTime.Value = 60;
            numTime.ValueChanged += (sender, e) => {
                _time = (int)numTime.Value;
            };

            _numPanel.Controls.Add(label);
            _numPanel.Controls.Add(numTime);

            p1.Controls.Add(_cbDiff);
            p1.Controls.Add(_numPanel);

            Panel spacer = new();
            spacer.Width = 10;
            spacer.Dock = DockStyle.Right;

            Panel spacer2 = new();
            spacer2.Width = 10;
            spacer2.Dock = DockStyle.Left;

            _butStartPanel.Controls.Add(_startButton);
            _butStartPanel.Controls.Add(spacer2);
            _butStartPanel.Controls.Add(_historyButton);
            _butStartPanel.Controls.Add(spacer);
            _butStartPanel.Controls.Add(p1);
            Controls.Add(_butStartPanel);
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
                EndQuiz();
        }

        private void StartQuiz()
        {
            if (_isStarted) return;

            GenerateExamples(4);

            _mathQuiz.Visible = false;

            _isStarted = true;
            _timeLeft = _time;
            _labelTimer.Text = "Time Left: " + _timeLeft;
            _timer.Start();

            _cbDiff.Visible = false;
            _numPanel.Visible = false;
            _historyButton.Visible = false;

            Controls.Add(_labelTimer);
            _startButton.Text = "End Quiz";
        }

        private void EndQuiz()
        {
            if (!_isStarted) return;

            _timer.Stop();
            _timeLeft = _time;
            _isStarted = false;

            CheckAnswers();
            RemoveExamples();
            Controls.Remove(_labelTimer);

            _startButton.Text = "Start Quiz";
            _cbDiff.Visible = true;
            _numPanel.Visible = true;
            _mathQuiz.Visible = true;
            _historyButton.Visible = true;
        }

        private void GenerateExamples(int count = 4)
        {
            for (int i = Controls.Count - 1; i >= 0; i--)
            {
                if (Controls[i] is TableLayoutPanel)
                    Controls.RemoveAt(i);
            }

            for (int i = 0; i < count; i++)
                CreateExample();
        }

        private void RemoveExamples()
        {
            for (int i = Controls.Count - 1; i >= 0; i--)
            {
                if (Controls[i] is TableLayoutPanel)
                    Controls.RemoveAt(i);
            }
        }

        private void CheckAnswers()
        {
            int correctCount = 0;
            string resultsText = "";

            var examples = Controls.OfType<TableLayoutPanel>()
                       .OrderBy(t => t.Top)
                       .ToList();

            var resultEntry = new QuizResult();
            resultEntry.Date = DateTime.Now;
            resultEntry.Difficulty = _difficulty;
            resultEntry.Total = examples.Count;

            foreach (var layout in examples)
            {
                Label firstNum = layout.Controls[0] as Label;
                Label oper = layout.Controls[1] as Label;
                Label secondNum = layout.Controls[2] as Label;
                Panel answerPanel = layout.Controls[4] as Panel;

                if (answerPanel != null && answerPanel.Controls[0] is NumericUpDown num)
                {
                    float answer = Convert.ToSingle(num.Tag);
                    float userValue = Convert.ToSingle(num.Value);

                    if (userValue == answer)
                    {
                        num.BackColor = Color.LightGreen;
                        correctCount++;
                    }
                    else
                        num.BackColor = Color.LightCoral;

                    resultsText += $"{firstNum.Text} {oper.Text} {secondNum.Text} = {answer}  (You: {userValue})\n";
                }
            }

            resultEntry.Correct = correctCount;
            _history.Add(resultEntry);
            SaveHistory();

            resultsText += $"\nCorrect answers: {correctCount} out of {Controls.OfType<TableLayoutPanel>().Count()}";
            MessageBox.Show(resultsText, "Quiz Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void CreateExample()
        {
            Random rand = Random.Shared;
            int a = 0, b = 0;
            char[] ops;

            switch (_difficulty)
            {
                case Difficulty.Easy:
                    a = rand.Next(1, 11);
                    b = rand.Next(1, 11);
                    ops = ['+', '-'];
                    break;
                case Difficulty.Medium:
                    a = rand.Next(1, 31);
                    b = rand.Next(1, 31);
                    ops = ['+', '-', '*'];
                    break;
                case Difficulty.Hard:
                    a = rand.Next(1, 51);
                    b = rand.Next(1, 51);
                    ops = ['+', '-', '*', '/'];
                    break;
                default:
                    a = rand.Next(1, 11);
                    b = rand.Next(1, 11);
                    ops = ['+', '-'];
                    break;
            }

            char op = ops[rand.Next(ops.Length)];

            if (op == '/')
            {
                b = rand.Next(1, 20);
                a = b * rand.Next(1, 10);
            }

            float result;
            if (op == '+') 
                result = a + b;
            else if (op == '-') 
                result = a - b;
            else if (op == '*') 
                result = a * b;
            else if (op == '/') 
                result = (float)a / b;
            else 
                result = 0;

            var layout = new TableLayoutPanel();
            layout.Dock = DockStyle.Top;
            layout.Height = 80;
            layout.ColumnCount = 5;
            layout.RowCount = 1;

            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));

            var firstNum1 = new Label();
            firstNum1.Text = a.ToString();
            firstNum1.TextAlign = ContentAlignment.MiddleCenter;
            firstNum1.Dock = DockStyle.Fill;
            firstNum1.Font = _font;

            var oper1 = new Label();
            oper1.Text = op.ToString();
            oper1.TextAlign = ContentAlignment.MiddleCenter;
            oper1.Dock = DockStyle.Fill;
            oper1.Font = _font;

            var secondNum1 = new Label();
            secondNum1.Text = b.ToString();
            secondNum1.TextAlign = ContentAlignment.MiddleCenter;
            secondNum1.Dock = DockStyle.Fill;
            secondNum1.Font = _font;

            var equal = new Label();
            equal.Text = "=";
            equal.TextAlign = ContentAlignment.MiddleCenter;
            equal.Dock = DockStyle.Fill;
            equal.Font = _font;

            var panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.Font = _font;

            var answer = new NumericUpDown();
            answer.Anchor = AnchorStyles.None;
            answer.TextAlign = HorizontalAlignment.Center;
            answer.DecimalPlaces = 2;
            answer.Font = _font;
            answer.Location = new Point(
                (panel.Width - answer.Width) / 2,
                (panel.Height - answer.Height) / 2
            );

            answer.Tag = result;
            answer.Minimum = -10000000;
            answer.Maximum = 10000000;

            panel.Controls.Add(answer);

            layout.Controls.Add(firstNum1, 0, 0);
            layout.Controls.Add(oper1, 1, 0);
            layout.Controls.Add(secondNum1, 2, 0);
            layout.Controls.Add(equal, 3, 0);
            layout.Controls.Add(panel, 4, 0);

            Controls.Add(layout);
        }

        public static string HistoryPath => Path.Combine(Program.GetDirectory(), "quiz_history.json");

        private List<QuizResult> _history = new();

        private void LoadHistory()
        {
            if (File.Exists(HistoryPath))
            {
                string json = File.ReadAllText(HistoryPath);
                _history = JsonSerializer.Deserialize<List<QuizResult>>(json) ?? new List<QuizResult>();
            }
        }

        private void SaveHistory()
        {
            string json = JsonSerializer.Serialize(_history, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(HistoryPath, json);
        }

        private void ShowHistory()
        {
            LoadHistory();
            if (_history.Count == 0)
            {
                MessageBox.Show("No quiz history yet.", "History", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string historyText = string.Join("\n\n", _history.Select(h =>$"{h.Date}: {h.Difficulty}, Correct: {h.Correct}/{h.Total}\n" + string.Join("\n", h.Answers)));

            MessageBox.Show(historyText, "Quiz History", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
