using System.Windows.Forms;

namespace WinFormsUlesanne
{
    public partial class MainForm : Form
    {
        public TreeView Tree;

        public PictureBox Picture;
        public Label PictureFileName;
        private string _curPicFilePath = @"..\..\..\Images\esimene.jpg";

        public CheckBox CheckBox1, CheckBox2, CheckBox3, CheckBox4;
        public RadioButton RadioBtn1, RadioBtn2;
        public TabControl TabControl;

        public MainForm()
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
            Picture.Location = new Point(160, 30);
            Picture.SizeMode = PictureBoxSizeMode.StretchImage;
            Picture.Image = Image.FromFile(_curPicFilePath);
            Picture.DoubleClick += Picture_DoubleClick;

            Button randPic = new Button();
            randPic.Text = "Juhuslik pilt";
            randPic.Size = new Size(90, 25);
            randPic.Location = new Point((250 / 2) - (90 / 2), 220);
            randPic.TextAlign = ContentAlignment.MiddleCenter;
            randPic.Click += Picture_DoubleClick;

            Picture.Controls.Add(randPic);

            PictureFileName = new Label();
            PictureFileName.Text = Path.GetFileName(_curPicFilePath);
            PictureFileName.BackColor = Color.Transparent;
            PictureFileName.Location = new Point(10, 10);

            Picture.Controls.Add(PictureFileName);

            CheckBox1 = new CheckBox();
            CheckBox1.Text = "Pilt 1";
            CheckBox1.Size = new Size(100, 20);
            CheckBox1.Location = new Point(160, 30);
            CheckBox1.CheckedChanged += CheckBox_CheckedChanged;

            CheckBox2 = new CheckBox();
            CheckBox2.Text = "Pilt 2";
            CheckBox2.Size = new Size(100, 20);
            CheckBox2.Location = new Point(160, 50);
            CheckBox2.CheckedChanged += CheckBox_CheckedChanged;

            CheckBox3 = new CheckBox();
            CheckBox3.Text = "Pilt 3";
            CheckBox3.Size = new Size(100, 20);
            CheckBox3.Location = new Point(160, 70);
            CheckBox3.CheckedChanged += CheckBox_CheckedChanged;

            CheckBox4 = new CheckBox();
            CheckBox4.Text = "Pilt 4";
            CheckBox4.Size = new Size(100, 20);
            CheckBox4.Location = new Point(160, 90);
            CheckBox4.CheckedChanged += CheckBox_CheckedChanged;

            PictureB1 = new PictureBox();
            PictureB1.Size = new Size(100, 100);
            PictureB1.Location = new Point(270, 20);
            PictureB1.SizeMode = PictureBoxSizeMode.StretchImage;
            PictureB1.Image = Image.FromFile(@"..\..\..\Images\esimene.jpg");
            PictureB1.Visible = false;
            
            Label label1 = new Label();
            label1.Text = "Pilt 1.";
            label1.BackColor = Color.Transparent;
            label1.Location = new Point(3, 70);
            label1.TextAlign = ContentAlignment.BottomCenter;
            PictureB1.Controls.Add(label1);

            PictureB2 = new PictureBox();
            PictureB2.Size = new Size(100, 100);
            PictureB2.Location = new Point(270, 120);
            PictureB2.SizeMode = PictureBoxSizeMode.StretchImage;
            PictureB2.Image = Image.FromFile(@"..\..\..\Images\teine.jpg");
            PictureB2.Visible = false;

            Label label2 = new Label();
            label2.Text = "Pilt 2.";
            label2.BackColor = Color.Transparent;
            label2.Location = new Point(3, 70);
            label2.TextAlign = ContentAlignment.BottomCenter;
            PictureB2.Controls.Add(label2);

            PictureB3 = new PictureBox();
            PictureB3.Size = new Size(100, 100);
            PictureB3.Location = new Point(270, 220);
            PictureB3.SizeMode = PictureBoxSizeMode.StretchImage;
            PictureB3.Image = Image.FromFile(@"..\..\..\Images\kolmas.jpg");
            PictureB3.Visible = false;

            Label label3 = new Label();
            label3.Text = "Pilt 3.";
            label3.BackColor = Color.Transparent;
            label3.Location = new Point(3, 70);
            label3.TextAlign = ContentAlignment.BottomCenter;
            PictureB3.Controls.Add(label3);

            PictureB4 = new PictureBox();
            PictureB4.Size = new Size(100, 100);
            PictureB4.Location = new Point(270, 320);
            PictureB4.SizeMode = PictureBoxSizeMode.StretchImage;
            PictureB4.Image = Image.FromFile(@"..\..\..\Images\esimene.jpg");
            PictureB4.Visible = false;

            Label label4 = new Label();
            label4.Text = "Pilt 4.";
            label4.BackColor = Color.Transparent;
            label4.Location = new Point(3, 70);
            label4.TextAlign = ContentAlignment.BottomCenter;
            PictureB4.Controls.Add(label4);

            RadioBtn1 = new RadioButton();
            RadioBtn1.Text = "Must teema";
            RadioBtn1.Location = new Point(170, 30);

            RadioBtn2 = new RadioButton();
            RadioBtn2.Text = "Valge teema";
            RadioBtn2.Location = new Point(170, 50);
            RadioBtn1.CheckedChanged += RadioBtn_CheckedChanged;
            RadioBtn2.CheckedChanged += RadioBtn_CheckedChanged;

            RadioBtn2.Checked = true;

            var menu = new MenuStrip();
            ToolStripMenuItem fileMenu = new ToolStripMenuItem("Fail");

            ToolStripMenuItem showMsgItem = new ToolStripMenuItem("Abi");
            showMsgItem.Click += (sender, e) => {
                MessageBox.Show("Tere! Abi ei ole!", "Info");
            };

            ToolStripMenuItem closeItem = new ToolStripMenuItem("Välja");
            closeItem.Click += (sender, e) => {
                Close();
            };

            fileMenu.DropDownItems.Add(showMsgItem);
            fileMenu.DropDownItems.Add(closeItem);
            menu.Items.Add(fileMenu);
            Controls.Add(menu);
            MainMenuStrip = menu;
        }

        public PictureBox PictureB1, PictureB2, PictureB3, PictureB4;

        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            var checkbox = (CheckBox)sender;

            if (checkbox == CheckBox1)
                PictureB1.Visible = checkbox.Checked;
            else if (checkbox == CheckBox2)
                PictureB2.Visible = checkbox.Checked;
            else if (checkbox == CheckBox3)
                PictureB3.Visible = checkbox.Checked;
            else if (checkbox == CheckBox4)
                PictureB4.Visible = checkbox.Checked;
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
            _curPicFilePath = @"..\..\..\Images\" + images[lastImgIndex];
            PictureFileName.Text = Path.GetFileName(_curPicFilePath);

            Picture.Image = Image.FromFile(_curPicFilePath);
        }

        private TabPage CreateTabPage(string title)
        {
            var tab = new TabPage(title);

            Button remBut = new Button();
            remBut.Text = "Eemalda leht";
            remBut.Location = new Point(10, 10);
            remBut.Size = new Size(90, 30);
            remBut.TextAlign = ContentAlignment.MiddleCenter;
            remBut.Click += (sender, ev) => {
                if (TabControl.TabPages.Count > 0)
                    TabControl.TabPages.RemoveAt(TabControl.SelectedIndex);
            };

            tab.Controls.Add(remBut);

            TextBox textBx = new TextBox();
            textBx.Location = new Point(10, 45);
            textBx.Size = new Size(370, 150);
            textBx.Name = "TextBox";
            textBx.Multiline = true;
            textBx.ScrollBars = ScrollBars.Vertical;
            textBx.ForeColor = Color.Gray;
            textBx.Text = "Sisesta tekst siia...";

            void DoPlaceHolderText()
            {
                textBx.ForeColor = Color.Gray;
                textBx.Text = "Sisesta tekst siia...";
            }

            textBx.LostFocus += (sender, ev) => {
                if (string.IsNullOrEmpty(textBx.Text))
                    DoPlaceHolderText();
            };

            textBx.GotFocus += (sender, ev) => {
                if (textBx.ForeColor == Color.Gray)
                {
                    textBx.Clear();
                    textBx.ForeColor = Color.Black;
                }
            };

            tab.Controls.Add(textBx);

            Button clearBut = new Button();
            clearBut.Text = "Puhasta";
            clearBut.Location = new Point(100, 10);
            clearBut.Size = new Size(90, 30);
            clearBut.TextAlign = ContentAlignment.MiddleCenter;
            clearBut.Click += (sender, ev) => {
                textBx.Clear();
                DoPlaceHolderText();
            };

            tab.Controls.Add(clearBut);

            return tab;
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
                    Controls.Add(PictureB1);

                    Controls.Add(CheckBox2);
                    Controls.Add(PictureB2);

                    Controls.Add(CheckBox3);
                    Controls.Add(PictureB3);

                    Controls.Add(CheckBox4);
                    Controls.Add(PictureB4);

                    Button button2 = new Button();
                    button2.Text = "Kuva valitud pildid";
                    button2.Location = new Point(160, 150);
                    button2.Size = new Size(90, 25);
                    button2.TextAlign = ContentAlignment.MiddleCenter;
                    button2.Click += (sender, ev) => {
                        List<string> list = new();
                        if (PictureB1.Visible)
                            list.Add("Pilt 1");
                        if (PictureB2.Visible)
                            list.Add("Pilt 2");
                        if (PictureB3.Visible)
                            list.Add("Pilt 3");
                        if (PictureB4.Visible)
                            list.Add("Pilt 4");

                        if (list.Count > 0)
                            MessageBox.Show("Sa valisid: " + string.Join(", ", list), "Valitud pildid", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else
                            MessageBox.Show("Sa pole midagi valinud.", "Valitud pildid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    };

                    Button button3 = new Button();
                    button3.Text = "Puhasta";
                    button3.Location = new Point(160, 180);
                    button3.Size = new Size(90, 25);
                    button3.TextAlign = ContentAlignment.MiddleCenter;
                    button3.Click += (sender, ev) => {
                        CheckBox1.Checked = false;
                        CheckBox2.Checked = false;
                        CheckBox3.Checked = false;
                        CheckBox4.Checked = false;
                    };

                    Controls.Add(button2);
                    Controls.Add(button3);
                    break;
                case "Raadionupud":
                    Controls.Add(RadioBtn1);
                    Controls.Add(RadioBtn2);
                    break;
                case "MessageBox":
                    PictureBox pic = new PictureBox();
                    pic.Size = new Size(100, 100);
                    pic.Location = new Point(270, 25);
                    pic.SizeMode = PictureBoxSizeMode.StretchImage;
                    pic.Image = Image.FromFile(@"..\..\..\Images\about.png");
                    pic.Visible = false;

                    Button button = new Button();
                    button.Text = "Küsimus";
                    button.Location = new Point(160, 35);
                    button.Size = new Size(90, 30);
                    button.TextAlign = ContentAlignment.MiddleCenter;
                    button.Click += (sender, ev) => {
                        var answer = MessageBox.Show("Kas see on õige? 2 + 2 = 5?", "Küsimus", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (answer == DialogResult.No)
                        {
                            MessageBox.Show("Tubli!", "Küsimus", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            button.ForeColor = Color.Green;
                            pic.Image = Image.FromFile(@"..\..\..\Images\about.png");
                        }
                        else
                        {
                            MessageBox.Show("Vale!", "Küsimus", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            button.ForeColor = Color.Red;
                            pic.Image = Image.FromFile(@"..\..\..\Images\close_box_red.png");
                        }

                        pic.Visible = true;
                    };

                    Controls.Add(button);
                    Controls.Add(pic);
                    break;
                case "TabControl":
                    TabControl = new TabControl();
                    TabControl.Location = new Point(170, 30);
                    TabControl.Size = new Size(400, 300);

                    TabControl.TabPages.Add(CreateTabPage("Tab 1"));
                    TabControl.TabPages.Add(CreateTabPage("Tab 2"));

                    var tabAdd = new TabPage("+");
                    TabControl.Selected += (sender, e) => {
                        if (e.TabPage == tabAdd)
                        {
                            string title = "Tab " + (TabControl.TabPages.Count).ToString();
                            TabPage newTab = CreateTabPage(title);
                            TabControl.TabPages.Add(newTab);
                            TabControl.TabPages.Remove(tabAdd);
                            TabControl.TabPages.Add(tabAdd);
                            TabControl.SelectedTab = newTab;
                        }
                    };

                    TabControl.TabPages.Add(tabAdd);
                    Controls.Add(TabControl);
                    break;
                case "ListBox":
                    ListBox listBox = new ListBox();
                    listBox.Location = new Point(170, 30);
                    listBox.Size = new Size(120, 150);

                    listBox.Items.Add("Valge");
                    listBox.Items.Add("Punane");
                    listBox.Items.Add("Roheline");
                    listBox.Items.Add("Sinine");
                    listBox.Items.Add("Kollane");
                    listBox.Items.Add("Hall");

                    listBox.SelectedIndexChanged += (sender, ev) => {
                        switch (listBox.SelectedItem.ToString())
                        {
                            case "Valge": BackColor = Color.White; break;
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
