namespace WinFormsUlesanne
{
    public partial class Form1 : Form
    {
        public TreeView Tree;
        public PictureBox Picture;
        public CheckBox CheckBox1, CheckBox2, CheckBox3, CheckBox4;
        public RadioButton RadioBtn1, RadioBtn2;
        public TabControl TabControl;

        public Form1()
        {
            InitializeComponent();

            Height = 600;
            Width = 800;
            Text = "Vorm elementidega";

            Tree = new TreeView();
            Tree.Dock = DockStyle.Left;
            Tree.AfterSelect += Tree_AfterSelect;
            Tree.Width = 150;
            TreeNode tn = new TreeNode("Elemendid");
            tn.Nodes.Add(new TreeNode("Piltid"));
            tn.Nodes.Add(new TreeNode("Märkeruudud"));
            tn.Nodes.Add(new TreeNode("Raadionupud"));
            tn.Nodes.Add(new TreeNode("MessageBox"));
            tn.Nodes.Add(new TreeNode("TabControl"));
            tn.Nodes.Add(new TreeNode("ListBox"));

            Tree.Nodes.Add(tn);
            Controls.Add(Tree);

            Picture = new PictureBox();
            Picture.Size = new Size(250, 250);
            Picture.Location = new Point(170, 20);
            Picture.SizeMode = PictureBoxSizeMode.StretchImage;
            Picture.Image = Image.FromFile(@"..\..\..\Images\esimene.jpg");
            Picture.DoubleClick += Picture_DoubleClick;

            CheckBox1 = new CheckBox();
            CheckBox1.Text = "Pilt 1";
            CheckBox1.Size = new Size(100, 20);
            CheckBox1.Location = new Point(160, 20);
            CheckBox1.CheckedChanged += CheckBox_CheckedChanged;

            CheckBox2 = new CheckBox();
            CheckBox2.Text = "Pilt 2";
            CheckBox2.Size = new Size(100, 20);
            CheckBox2.Location = new Point(160, 40);
            CheckBox2.CheckedChanged += CheckBox_CheckedChanged;

            CheckBox3 = new CheckBox();
            CheckBox3.Text = "Pilt 3";
            CheckBox3.Size = new Size(100, 20);
            CheckBox3.Location = new Point(160, 60);
            CheckBox3.CheckedChanged += CheckBox_CheckedChanged;

            CheckBox4 = new CheckBox();
            CheckBox4.Text = "Pilt 4";
            CheckBox4.Size = new Size(100, 20);
            CheckBox4.Location = new Point(160, 80);
            CheckBox4.CheckedChanged += CheckBox_CheckedChanged;

            RadioBtn1 = new RadioButton();
            RadioBtn1.Text = "Must teema";
            RadioBtn1.Location = new Point(170, 20);

            RadioBtn2 = new RadioButton();
            RadioBtn2.Text = "Valge teema";
            RadioBtn2.Location = new Point(170, 40);
            RadioBtn1.CheckedChanged += RadioBtn_CheckedChanged;
            RadioBtn2.CheckedChanged += RadioBtn_CheckedChanged;

            RadioBtn2.Checked = true;

            var menu = new MenuStrip();
            ToolStripMenuItem fileMenu = new ToolStripMenuItem("Fail");

            ToolStripMenuItem showMsgItem = new ToolStripMenuItem("Abi");
            showMsgItem.Click += (s, e) => {
                MessageBox.Show("Tere! Abi ei ole!", "Info");
            };

            ToolStripMenuItem closeItem = new ToolStripMenuItem("Välja");
            closeItem.Click += (s, e) => {
                Close();
            };

            fileMenu.DropDownItems.Add(showMsgItem);
            fileMenu.DropDownItems.Add(closeItem);
            menu.Items.Add(fileMenu);
            Controls.Add(menu);
            MainMenuStrip = menu;
        }

        Dictionary<CheckBox, PictureBox> pictures = new();
        
        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            var checkbox = (CheckBox)sender;

            if (pictures.ContainsKey(checkbox))
            {
                Controls.Remove(pictures[checkbox]);
                pictures.Remove(checkbox);
                return;
            }

            var pic = new PictureBox();
            pictures.Add(checkbox, pic);

            int heightOffset = 0;
            foreach ( var picture in pictures.Values )
                heightOffset += picture.Height;

            pic.Size = new Size(100, 100);
            pic.Location = new Point(280, 20 + heightOffset);
            pic.SizeMode = PictureBoxSizeMode.StretchImage;
            pic.Image = Image.FromFile(@"..\..\..\Images\esimene.jpg");
            pic.DoubleClick += Picture_DoubleClick;

            Controls.Add(pic);
        }

        int lastImgIndex = 0;
        private void Picture_DoubleClick(object sender, EventArgs e)
        { 
            string[] images = { "esimene.jpg", "teine.jpg", "kolmas.jpg" };

            int num = 0;
            while (true)
            {
                num = Random.Shared.Next(0, 3);
                if (num != lastImgIndex)
                    break;
            }

            lastImgIndex = num;
            string fail = images[lastImgIndex];
            Picture.Image = Image.FromFile(@"..\..\..\Images\" + fail);
        }

        private void Tree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Controls.Clear();
            Controls.Add(Tree);
            Controls.Add(MainMenuStrip);

            switch (e.Node.Text)
            {
                case "Piltid":
                    Controls.Add(Picture);
                    break;
                case "Märkeruudud":
                    Controls.Add(CheckBox1);
                    Controls.Add(CheckBox2);
                    Controls.Add(CheckBox3);
                    Controls.Add(CheckBox4);
                    break;
                case "Raadionupud":
                    Controls.Add(RadioBtn1);
                    Controls.Add(RadioBtn2);
                    break;
                case "MessageBox":
                    var answer = MessageBox.Show("Kas sul on kass?", "Küsimus", MessageBoxButtons.YesNo);
                    if (answer == DialogResult.Yes)
                        MessageBox.Show("Väga hea", "Küsimus", MessageBoxButtons.OK);
                    break;
                case "TabControl":
                    TabControl = new TabControl();
                    TabControl.Location = new Point(170, 20);
                    TabControl.Size = new Size(400, 300);

                    var tab1 = new TabPage("Esimene leht");
                    var tab2 = new TabPage("Teine leht");

                    Label tab2Label = new Label();
                    tab2Label.Text = "See on teine vaheleht";
                    tab2Label.AutoSize = true;
                    tab2Label.Location = new Point(20, 20);
                    tab2.Controls.Add(tab2Label);

                    TabControl.TabPages.Add(tab1);
                    TabControl.TabPages.Add(tab2);

                    Button removeTabButton = new Button();
                    removeTabButton.Text = "Eemalda leht";
                    removeTabButton.Location = new Point(20, 20);
                    removeTabButton.Size = new Size(90, 30);
                    removeTabButton.TextAlign = ContentAlignment.MiddleCenter;
                    removeTabButton.Click += (s, ev) => {
                        if (TabControl.TabPages.Count > 0)
                            TabControl.TabPages.RemoveAt(TabControl.SelectedIndex);
                    };
                    tab1.Controls.Add(removeTabButton);

                    Controls.Add(TabControl);
                    break;
                case "ListBox":
                    ListBox listBox = new ListBox();
                    listBox.Location = new Point(170, 20);
                    listBox.Size = new Size(120, 150);

                    listBox.Items.AddRange([ "Punane", "Roheline", "Sinine", "Kollane", "Hall" ]);
                    listBox.SelectedIndexChanged += (s, ev) =>
                    {
                        switch (listBox.SelectedItem.ToString())
                        {
                            case "Punane": BackColor = Color.Red; break;
                            case "Roheline": BackColor = Color.Green; break;
                            case "Sinine": BackColor = Color.Blue; break;
                            case "Kollane": BackColor = Color.Yellow; break;
                            case "Hall": BackColor = Color.Gray; break;
                        }
                    };

                    Controls.Add(listBox);
                    break;
            }
        }

        private void RadioBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (RadioBtn1.Checked)
            {
                BackColor = Color.Black;
                RadioBtn2.ForeColor = Color.White;
                RadioBtn1.ForeColor = Color.White;
            }
            else if (RadioBtn2.Checked)
            {
                BackColor = Color.White;
                RadioBtn2.ForeColor = Color.Black;
                RadioBtn1.ForeColor = Color.Black;
            }
        }
    }
}
