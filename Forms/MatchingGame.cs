using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace WinForms
{
    public partial class MatchingGame : Form
    {
        private TableLayoutPanel _table;
        private Timer _revealTimer;
        private Label _firstClicked;
        private Label _secondClicked;
        private Random _random = new Random();

        private List<string> _icons = new List<string>()
        {
            "!", "!", "N", "N", "k", "k", "b", "b",
            "v", "v", "w", "w", "z", "z", "m", "m"
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
                Label lbl = new();
                lbl.Dock = DockStyle.Fill;
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.Font = new Font("Arial", 32);
                lbl.BackColor = Color.Green;
                lbl.ForeColor = Color.Green;
                lbl.Margin = new Padding(5);
                lbl.Text = "?";
                lbl.Click += Label_Click;
                _table.Controls.Add(lbl);
            }

            AssignIconsToSquares();

            _revealTimer = new();
            _revealTimer.Interval = 750;
            _revealTimer.Tick += RevealTimer_Tick;
        }

        private void AssignIconsToSquares()
        {
            List<string> iconsCopy = new List<string>(_icons);

            foreach (Label label in _table.Controls.OfType<Label>())
            {
                int index = _random.Next(iconsCopy.Count);
                label.Tag = iconsCopy[index];
                iconsCopy.RemoveAt(index);
            }
        }

        private void Label_Click(object sender, EventArgs e)
        {
            if (_revealTimer.Enabled) return;

            Label clickedLabel = sender as Label;
            if (clickedLabel == null) return;

            if (clickedLabel.ForeColor == Color.Black) return;

            if (_firstClicked == null)
            {
                _firstClicked = clickedLabel;
                ShowLabel(clickedLabel);
                return;
            }

            _secondClicked = clickedLabel;
            ShowLabel(clickedLabel);

            if (_firstClicked.Tag.ToString() == _secondClicked.Tag.ToString())
            {
                _firstClicked = null;
                _secondClicked = null;
                CheckForWinner();
            }
            else
                _revealTimer.Start();
        }

        private void RevealTimer_Tick(object sender, EventArgs e)
        {
            _revealTimer.Stop();

            HideLabel(_firstClicked);
            HideLabel(_secondClicked);

            _firstClicked = null;
            _secondClicked = null;
        }

        private void ShowLabel(Label lbl)
        {
            lbl.Text = lbl.Tag.ToString();
            lbl.ForeColor = Color.Black;
        }

        private void HideLabel(Label lbl)
        {
            lbl.Text = "?";
            lbl.ForeColor = lbl.BackColor;
        }

        private void CheckForWinner()
        {
            foreach (Label label in _table.Controls.OfType<Label>())
            {
                if (label.ForeColor != Color.Black)
                    return;
            }

            MessageBox.Show("You matched all pairs!", "Victory");
        }
    }
}