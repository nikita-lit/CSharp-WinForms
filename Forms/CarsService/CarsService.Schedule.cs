using System.Drawing;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WinForms.CarsService
{
    public partial class CarsService
    {
        private TableLayoutPanel _tlpSchedule;
        private DateTime _weekStart;

        private const int START_HOUR = 8;
        private const int END_HOUR = 18;

        private void SetupScheduleTab(Panel parent)
        {
            parent.Padding = new Padding(8);

            _weekStart = GetStartOfWeek(DateTime.Today);

            _tlpSchedule = new TableLayoutPanel();
            _tlpSchedule.Dock = DockStyle.Fill;
            _tlpSchedule.ColumnCount = 8;
            _tlpSchedule.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            _tlpSchedule.BackColor = Colors.Background;

            parent.Controls.Add(_tlpSchedule);

            BuildScheduleTable();
            LoadScheduleData();
        }

        private void BuildScheduleTable()
        {
            _tlpSchedule.Controls.Clear();
            _tlpSchedule.ColumnStyles.Clear();
            _tlpSchedule.RowStyles.Clear();

            _tlpSchedule.RowCount = (END_HOUR - START_HOUR) + 1;

            _tlpSchedule.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
            for (int i = 0; i < 7; i++)
            {
                _tlpSchedule.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / 7));
            }

            _tlpSchedule.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            for (int i = START_HOUR; i < END_HOUR; i++)
            {
                _tlpSchedule.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / (END_HOUR - START_HOUR)));
            }

            _tlpSchedule.Controls.Add(CreateHeaderLabel("time", Color.Transparent), 0, 0);

            for (int d = 0; d < 7; d++)
            {
                DateTime date = _weekStart.AddDays(d);
                string text = date.DayOfWeek.ToString().ToLower();

                var color = Color.Transparent;
                if (date.DayOfWeek == DateTime.Now.DayOfWeek)
                    color = Colors.HeaderLine;

                _tlpSchedule.Controls.Add(CreateHeaderLabel(text, color, date.ToString("dd.MM")), d + 1, 0);
            }

            for (int h = START_HOUR; h < END_HOUR; h++)
            {
                _tlpSchedule.Controls.Add(CreateTimeLabel(h + ":00"), 0, (h - START_HOUR) + 1);
            }
        }

        private void LoadScheduleData()
        {
            if (_tlpSchedule == null)
                return;

            for (int i = _tlpSchedule.Controls.Count - 1; i >= 0; i--)
            {
                var control = _tlpSchedule.Controls[i];
                int col = _tlpSchedule.GetColumn(control);
                int row = _tlpSchedule.GetRow(control);

                if (col != 0 && row != 0)
                {
                    _tlpSchedule.Controls.RemoveAt(i);
                    control.Dispose();
                }
            }

            foreach (var cs in CarServiceData.GetAll())
            {
                if (cs.StartTime < _weekStart ||
                    cs.StartTime >= _weekStart.AddDays(7))
                    continue;

                int col = (cs.StartTime.Date - _weekStart.Date).Days + 1;
                int row = (cs.StartTime.Hour - START_HOUR) + 1;

                if (row < 1 || row >= _tlpSchedule.RowCount)
                    continue;

                Panel block = new Panel();
                block.Dock = DockStyle.Fill;
                block.Margin = new Padding(2);

                var now = DateTime.Now;
                var color = Colors.Row;
                if (cs.StartTime.Day == now.Day && cs.StartTime.Hour == now.Hour)
                    color = Colors.HeaderLine;

                block.BackColor = color;

                Label lbl = new Label();
                lbl.Dock = DockStyle.Fill;
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.ForeColor = Colors.Text;
                lbl.Font = new Font("Arial", 8);

                var car = cs.Car;
                lbl.Text =
                    cs.StartTime.ToString("HH:mm") +
                    " - " +
                    cs.EndTime.ToString("HH:mm") + "\n" +
                    car.Brand + " " + car.Model +
                    " (" + car.RegistrationNumber + ")" + "\n" +
                    cs.Service.Name;

                block.Controls.Add(lbl);
                _tlpSchedule.Controls.Add(block, col, row);
            }
        }

        private Label CreateHeaderLabel(string text, Color bgColor, string text2 = null)
        {
            Label lbl = new Label();
            lbl.Text = LanguageManager.Get(text) + (text2 != null ? "\n" + text2 : "");
            lbl.Dock = DockStyle.Fill;
            lbl.TextAlign = ContentAlignment.MiddleCenter;
            lbl.Font = new Font("Arial", 9, FontStyle.Bold);
            lbl.ForeColor = Colors.Text;
            lbl.BackColor = bgColor;

            LanguageManager.LanguageChanged += () => {
                lbl.Text = LanguageManager.Get(text) + (text2 != null ? "\n" + text2 : "");
            };

            return lbl;
        }

        private Label CreateTimeLabel(string text)
        {
            Label lbl = new Label();
            lbl.Text = text;
            lbl.Dock = DockStyle.Fill;
            lbl.TextAlign = ContentAlignment.MiddleCenter;
            lbl.ForeColor = Colors.Text;

            return lbl;
        }

        private DateTime GetStartOfWeek(DateTime date)
        {
            int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.AddDays(-diff).Date;
        }
    }
}