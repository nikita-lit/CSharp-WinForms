namespace WinFormsUlesanne
{
    public partial class MainForm : Form
    {
        public ExampleForm ExampleForm;
        public PictureViewer PictureViewer;

        public MainForm()
        {
            InitializeComponent();

            Height = 200;
            Width = 300;
            Text = "Vorm valik";

            Panel butsPanel = new Panel() {
                Dock = DockStyle.Fill,
                Padding = new Padding(30)
            };

            Button but = new Button()  {
                Text = "Vorm 1",
                Size = new Size(200, 30),
                Dock = DockStyle.Top,
            };
            but.Click += (sender, e) => {
                ExampleForm = new ExampleForm();
                ExampleForm.Show();
                Hide();
            };

            Button but2 = new Button() {
                Text = "Vorm 2",
                Size = new Size(200, 30),
                Dock = DockStyle.Top,
            };
            but2.Click += (sender, e) => {
                PictureViewer = new PictureViewer();
                PictureViewer.Show();
                Hide();
            };

            Button but3 = new Button() {
                Text = "Vorm 3",
                Size = new Size(200, 30),
                Dock = DockStyle.Top,
            };
            but3.Click += (sender, e) => {
                ExampleForm = new ExampleForm();
                ExampleForm.Show();
                Hide();
            };

            butsPanel.Controls.Add(but3);
            butsPanel.Controls.Add(but2);
            butsPanel.Controls.Add(but);

            Controls.Add(butsPanel);
        }
    }
}
