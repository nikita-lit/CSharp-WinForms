namespace WinForms
{
    partial class EShop
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            _dataGridView1 = new DataGridView();
            _pbProductImage = new PictureBox();
            _butAddProduct = new Button();
            _butRemoveProduct = new Button();
            _butUpdateProduct = new Button();
            _cbProductCategory = new ComboBox();
            label1 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            _textProductName = new TextBox();
            _numProductCount = new NumericUpDown();
            _numProductPrice = new NumericUpDown();
            _butClearProduct = new Button();
            _butAddProductCategory = new Button();
            _butRemoveProductCategory = new Button();
            _butFindFile = new Button();
            ((System.ComponentModel.ISupportInitialize)_dataGridView1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_pbProductImage).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_numProductCount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_numProductPrice).BeginInit();
            SuspendLayout();
            // 
            // _dataGridView1
            // 
            _dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            _dataGridView1.Location = new Point(12, 241);
            _dataGridView1.Name = "_dataGridView1";
            _dataGridView1.Size = new Size(845, 239);
            _dataGridView1.TabIndex = 0;
            _dataGridView1.CellMouseEnter += _dataGridView1_CellMouseEnter;
            _dataGridView1.CellMouseLeave += _dataGridView1_CellMouseLeave;
            // 
            // _pbProductImage
            // 
            _pbProductImage.Location = new Point(667, 12);
            _pbProductImage.Name = "_pbProductImage";
            _pbProductImage.Size = new Size(190, 190);
            _pbProductImage.TabIndex = 1;
            _pbProductImage.TabStop = false;
            // 
            // _butAddProduct
            // 
            _butAddProduct.Location = new Point(12, 212);
            _butAddProduct.Name = "_butAddProduct";
            _butAddProduct.Size = new Size(75, 23);
            _butAddProduct.TabIndex = 2;
            _butAddProduct.Text = "Lisa";
            _butAddProduct.UseVisualStyleBackColor = true;
            _butAddProduct.Click += _butAddProduct_Click;
            // 
            // _butRemoveProduct
            // 
            _butRemoveProduct.Location = new Point(93, 212);
            _butRemoveProduct.Name = "_butRemoveProduct";
            _butRemoveProduct.Size = new Size(75, 23);
            _butRemoveProduct.TabIndex = 3;
            _butRemoveProduct.Text = "Kustuta";
            _butRemoveProduct.UseVisualStyleBackColor = true;
            // 
            // _butUpdateProduct
            // 
            _butUpdateProduct.Location = new Point(174, 212);
            _butUpdateProduct.Name = "_butUpdateProduct";
            _butUpdateProduct.Size = new Size(75, 23);
            _butUpdateProduct.TabIndex = 4;
            _butUpdateProduct.Text = "Uuenda";
            _butUpdateProduct.UseVisualStyleBackColor = true;
            // 
            // _cbProductCategory
            // 
            _cbProductCategory.FormattingEnabled = true;
            _cbProductCategory.Location = new Point(102, 146);
            _cbProductCategory.Name = "_cbProductCategory";
            _cbProductCategory.Size = new Size(121, 23);
            _cbProductCategory.TabIndex = 5;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(13, 149);
            label1.Name = "label1";
            label1.Size = new Size(74, 15);
            label1.TabIndex = 6;
            label1.Text = "Kategooriad:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(13, 115);
            label3.Name = "label3";
            label3.Size = new Size(36, 15);
            label3.TabIndex = 8;
            label3.Text = "Hind:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(13, 81);
            label4.Name = "label4";
            label4.Size = new Size(43, 15);
            label4.TabIndex = 9;
            label4.Text = "Kogus:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(13, 44);
            label5.Name = "label5";
            label5.Size = new Size(43, 15);
            label5.TabIndex = 10;
            label5.Text = "Toode:";
            // 
            // _textProductName
            // 
            _textProductName.Location = new Point(102, 41);
            _textProductName.Name = "_textProductName";
            _textProductName.Size = new Size(121, 23);
            _textProductName.TabIndex = 11;
            // 
            // _numProductCount
            // 
            _numProductCount.Location = new Point(102, 79);
            _numProductCount.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            _numProductCount.Name = "_numProductCount";
            _numProductCount.Size = new Size(121, 23);
            _numProductCount.TabIndex = 12;
            // 
            // _numProductPrice
            // 
            _numProductPrice.DecimalPlaces = 2;
            _numProductPrice.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            _numProductPrice.Location = new Point(102, 113);
            _numProductPrice.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            _numProductPrice.Name = "_numProductPrice";
            _numProductPrice.Size = new Size(120, 23);
            _numProductPrice.TabIndex = 13;
            // 
            // _butClearProduct
            // 
            _butClearProduct.Location = new Point(255, 212);
            _butClearProduct.Name = "_butClearProduct";
            _butClearProduct.Size = new Size(75, 23);
            _butClearProduct.TabIndex = 14;
            _butClearProduct.Text = "Puhasta";
            _butClearProduct.UseVisualStyleBackColor = true;
            // 
            // _butAddProductCategory
            // 
            _butAddProductCategory.Location = new Point(229, 145);
            _butAddProductCategory.Name = "_butAddProductCategory";
            _butAddProductCategory.Size = new Size(125, 23);
            _butAddProductCategory.TabIndex = 15;
            _butAddProductCategory.Text = "Lisa kategooriat";
            _butAddProductCategory.UseVisualStyleBackColor = true;
            _butAddProductCategory.Click += _butAddProductCategory_Click;
            // 
            // _butRemoveProductCategory
            // 
            _butRemoveProductCategory.Location = new Point(229, 174);
            _butRemoveProductCategory.Name = "_butRemoveProductCategory";
            _butRemoveProductCategory.Size = new Size(125, 23);
            _butRemoveProductCategory.TabIndex = 16;
            _butRemoveProductCategory.Text = "Kustuta kategooriat";
            _butRemoveProductCategory.UseVisualStyleBackColor = true;
            _butRemoveProductCategory.Click += _butRemoveProductCategory_Click;
            // 
            // _butFindFile
            // 
            _butFindFile.Location = new Point(576, 212);
            _butFindFile.Name = "_butFindFile";
            _butFindFile.Size = new Size(75, 23);
            _butFindFile.TabIndex = 17;
            _butFindFile.Text = "Otsi fail";
            _butFindFile.UseVisualStyleBackColor = true;
            _butFindFile.Click += _butFindFile_Click;
            // 
            // EShop
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(869, 492);
            Controls.Add(_butFindFile);
            Controls.Add(_butRemoveProductCategory);
            Controls.Add(_butAddProductCategory);
            Controls.Add(_butClearProduct);
            Controls.Add(_numProductPrice);
            Controls.Add(_numProductCount);
            Controls.Add(_textProductName);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label1);
            Controls.Add(_cbProductCategory);
            Controls.Add(_butUpdateProduct);
            Controls.Add(_butRemoveProduct);
            Controls.Add(_butAddProduct);
            Controls.Add(_pbProductImage);
            Controls.Add(_dataGridView1);
            Name = "EShop";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)_dataGridView1).EndInit();
            ((System.ComponentModel.ISupportInitialize)_pbProductImage).EndInit();
            ((System.ComponentModel.ISupportInitialize)_numProductCount).EndInit();
            ((System.ComponentModel.ISupportInitialize)_numProductPrice).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView _dataGridView1;
        private PictureBox _pbProductImage;
        private Button _butAddProduct;
        private Button _butRemoveProduct;
        private Button _butUpdateProduct;
        private ComboBox _cbProductCategory;
        private Label label1;
        private Label label3;
        private Label label4;
        private Label label5;
        private TextBox _textProductName;
        private NumericUpDown _numProductCount;
        private NumericUpDown _numProductPrice;
        private Button _butClearProduct;
        private Button _butAddProductCategory;
        private Button _butRemoveProductCategory;
        private Button _butFindFile;
    }
}