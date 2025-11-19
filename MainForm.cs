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

            Height = 300;
            Width = 300;
            Text = "Vorm valik";

            Panel butsPanel = new Panel() {
                Dock = DockStyle.Fill,
                Padding = new Padding(30)
            };

            Button but = new Button()  {
                Text = "Example Form",
                Size = new Size(200, 30),
                Dock = DockStyle.Top,
            };
            but.Click += (sender, e) => {
                ExampleForm = new ExampleForm();
                ExampleForm.Show();
                Hide();
            };

            Button but2 = new Button() {
                Text = "Picture Viewer",
                Size = new Size(200, 30),
                Dock = DockStyle.Top,
            };
            but2.Click += (sender, e) => {
                PictureViewer = new PictureViewer();
                PictureViewer.Show();
                Hide();
            };

            Button but3 = new Button() {
                Text = "Math Quiz",
                Size = new Size(200, 30),
                Dock = DockStyle.Top,
            };
            but3.Click += (sender, e) => {
                MathQuiz = new MathQuiz();
                MathQuiz.Show();
                Hide();
            };

            Button but4 = new Button() {
                Text = "Matching Game",
                Size = new Size(200, 30),
                Dock = DockStyle.Top,
            };
            but4.Click += (sender, e) => {
                MatchingGame = new MatchingGame();
                MatchingGame.Show();
                Hide();
            };

            Button but5 = new Button() {
                Text = "EShop",
                Size = new Size(200, 30),
                Dock = DockStyle.Top,
            };
            but5.Click += (sender, e) => {
                EShop = new EShop();
                EShop.Show();
                Hide();
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
