namespace WinFormsUlesanne
{
    public partial class PictureViewer : Form
    {
        public Panel ButsPanel;

        public PictureViewer()
        {
            InitializeComponent();

            Height = 800;
            Width = 1280;
            Text = "Picture Viewer";

            ButsPanel = new Panel()
            {
                Width = Width,
                Height = 80,
                Dock = DockStyle.Bottom,
            };
            ButsPanel.BackColor = Color.Red;

            Controls.Add(ButsPanel);
        }
    }
}
