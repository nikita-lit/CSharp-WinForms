namespace WinForms
{
    public partial class MainForm : Form
    {
        public ExampleForm ExampleForm;
        public PictureViewer PictureViewer;
        public MathQuiz MathQuiz;
        public MatchingGame MatchingGame;
        public EShop EShop;

        public MainForm()
        {
            InitializeComponent();

            Text = "Vorm valik";
            Size = new Size(300, 300);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MinimizeBox = false;
            MaximizeBox = false;

            Panel butsPanel = new();
            butsPanel.Dock = DockStyle.Fill;
            butsPanel.Padding = new Padding(30);

            Button but = new();
            but.Text = "Example Form";
            but.Size = new Size(200, 30);
            but.Dock = DockStyle.Top;
            but.Click += (sender, e) => {
                ExampleForm = new ExampleForm();
                ExampleForm.Show();
            };

            Button but2 = new();
            but2.Text = "Picture Viewer";
            but2.Size = new Size(200, 30);
            but2.Dock = DockStyle.Top;
            but2.Click += (sender, e) => {
                PictureViewer = new PictureViewer();
                PictureViewer.Show();
            };

            Button but3 = new();
            but3.Text = "Math Quiz";
            but3.Size = new Size(200, 30);
            but3.Dock = DockStyle.Top;
            but3.Click += (sender, e) => {
                MathQuiz = new MathQuiz();
                MathQuiz.Show();
            };

            Button but4 = new();
            but4.Text = "Matching Game";
            but4.Size = new Size(200, 30);
            but4.Dock = DockStyle.Top;
            but4.Click += (sender, e) => {
                MatchingGame = new MatchingGame();
                MatchingGame.Show();
            };

            Button but5 = new();
            but5.Text = "EShop";
            but5.Size = new Size(200, 30);
            but5.Dock = DockStyle.Top;
            but5.Click += (sender, e) => {
                Hide();
                EShop = new EShop();
                EShop.Show();
            };

            butsPanel.Controls.Add(but5);
            butsPanel.Controls.Add(but4);
            butsPanel.Controls.Add(but3);
            butsPanel.Controls.Add(but2);
            butsPanel.Controls.Add(but);

            Controls.Add(butsPanel);
        }
    }
}
