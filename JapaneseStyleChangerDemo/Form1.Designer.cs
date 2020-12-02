namespace JapaneseStyleChangerDemo
{
    partial class Form1
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
            this.SourceText = new System.Windows.Forms.TextBox();
            this.TargetText = new System.Windows.Forms.TextBox();
            this.JotaiCheckBox = new System.Windows.Forms.CheckBox();
            this.SpacingCheckBox = new System.Windows.Forms.CheckBox();
            this.MSButton = new System.Windows.Forms.RadioButton();
            this.CGButton = new System.Windows.Forms.RadioButton();
            this.GoButton = new System.Windows.Forms.Button();
            this.Initializing = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel2 = new System.Windows.Forms.Panel();
            this.FullwidthButton = new System.Windows.Forms.RadioButton();
            this.HalfwidthButton = new System.Windows.Forms.RadioButton();
            this.ChangeWidths = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.UseFullwidthAlphabets = new System.Windows.Forms.CheckBox();
            this.UseFullwidthDigits = new System.Windows.Forms.CheckBox();
            this.UseFullwidthSymbols = new System.Windows.Forms.CheckBox();
            this.FullwidthSymbolsList = new System.Windows.Forms.TextBox();
            this.UseIdeographicSpace = new System.Windows.Forms.CheckBox();
            this.UseHalfwidthSymbols = new System.Windows.Forms.CheckBox();
            this.HalfwidthSymbolsList = new System.Windows.Forms.TextBox();
            this.DearuCheckBox = new System.Windows.Forms.CheckBox();
            this.HtmlSyntax = new System.Windows.Forms.CheckBox();
            this.Initializing.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // SourceText
            // 
            this.SourceText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SourceText.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.SourceText.Location = new System.Drawing.Point(0, 0);
            this.SourceText.Multiline = true;
            this.SourceText.Name = "SourceText";
            this.SourceText.Size = new System.Drawing.Size(528, 207);
            this.SourceText.TabIndex = 0;
            this.SourceText.Text = "ここに書き換える文章を入力します。";
            // 
            // TargetText
            // 
            this.TargetText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TargetText.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.TargetText.Location = new System.Drawing.Point(0, 0);
            this.TargetText.Multiline = true;
            this.TargetText.Name = "TargetText";
            this.TargetText.Size = new System.Drawing.Size(528, 206);
            this.TargetText.TabIndex = 1;
            // 
            // JotaiCheckBox
            // 
            this.JotaiCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.JotaiCheckBox.AutoSize = true;
            this.JotaiCheckBox.Checked = true;
            this.JotaiCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.JotaiCheckBox.Location = new System.Drawing.Point(546, 46);
            this.JotaiCheckBox.Name = "JotaiCheckBox";
            this.JotaiCheckBox.Size = new System.Drawing.Size(167, 16);
            this.JotaiCheckBox.TabIndex = 2;
            this.JotaiCheckBox.Text = "Change to jotai (常体) style";
            this.JotaiCheckBox.UseVisualStyleBackColor = true;
            // 
            // SpacingCheckBox
            // 
            this.SpacingCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SpacingCheckBox.AutoSize = true;
            this.SpacingCheckBox.Location = new System.Drawing.Point(546, 93);
            this.SpacingCheckBox.Name = "SpacingCheckBox";
            this.SpacingCheckBox.Size = new System.Drawing.Size(105, 16);
            this.SpacingCheckBox.TabIndex = 3;
            this.SpacingCheckBox.Text = "Change spacing";
            this.SpacingCheckBox.UseVisualStyleBackColor = true;
            // 
            // MSButton
            // 
            this.MSButton.AutoSize = true;
            this.MSButton.Checked = true;
            this.MSButton.Location = new System.Drawing.Point(4, 3);
            this.MSButton.Name = "MSButton";
            this.MSButton.Size = new System.Drawing.Size(68, 16);
            this.MSButton.TabIndex = 4;
            this.MSButton.TabStop = true;
            this.MSButton.Text = "MS style";
            this.MSButton.UseVisualStyleBackColor = true;
            // 
            // CGButton
            // 
            this.CGButton.AutoSize = true;
            this.CGButton.Location = new System.Drawing.Point(4, 25);
            this.CGButton.Name = "CGButton";
            this.CGButton.Size = new System.Drawing.Size(111, 16);
            this.CGButton.TabIndex = 5;
            this.CGButton.Text = "Minimum spacing";
            this.CGButton.UseVisualStyleBackColor = true;
            // 
            // GoButton
            // 
            this.GoButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.GoButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.GoButton.Location = new System.Drawing.Point(546, 12);
            this.GoButton.Name = "GoButton";
            this.GoButton.Size = new System.Drawing.Size(224, 23);
            this.GoButton.TabIndex = 7;
            this.GoButton.Text = "Go";
            this.GoButton.UseVisualStyleBackColor = true;
            this.GoButton.Click += new System.EventHandler(this.GoButton_Click);
            // 
            // Initializing
            // 
            this.Initializing.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Initializing.Controls.Add(this.label2);
            this.Initializing.Location = new System.Drawing.Point(300, 170);
            this.Initializing.Name = "Initializing";
            this.Initializing.Size = new System.Drawing.Size(200, 100);
            this.Initializing.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(71, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "Initializing...";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(12, 12);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.SourceText);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.TargetText);
            this.splitContainer1.Size = new System.Drawing.Size(528, 417);
            this.splitContainer1.SplitterDistance = 207;
            this.splitContainer1.TabIndex = 9;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Controls.Add(this.FullwidthButton);
            this.panel2.Controls.Add(this.HalfwidthButton);
            this.panel2.Location = new System.Drawing.Point(564, 192);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(208, 44);
            this.panel2.TabIndex = 14;
            // 
            // FullwidthButton
            // 
            this.FullwidthButton.AutoSize = true;
            this.FullwidthButton.Location = new System.Drawing.Point(3, 25);
            this.FullwidthButton.Name = "FullwidthButton";
            this.FullwidthButton.Size = new System.Drawing.Size(173, 16);
            this.FullwidthButton.TabIndex = 12;
            this.FullwidthButton.Text = "Prefer fullwidths parentheses";
            this.FullwidthButton.UseVisualStyleBackColor = true;
            // 
            // HalfwidthButton
            // 
            this.HalfwidthButton.AutoSize = true;
            this.HalfwidthButton.Checked = true;
            this.HalfwidthButton.Location = new System.Drawing.Point(3, 3);
            this.HalfwidthButton.Name = "HalfwidthButton";
            this.HalfwidthButton.Size = new System.Drawing.Size(176, 16);
            this.HalfwidthButton.TabIndex = 11;
            this.HalfwidthButton.TabStop = true;
            this.HalfwidthButton.Text = "Prefer halfwidths parentheses";
            this.HalfwidthButton.UseVisualStyleBackColor = true;
            // 
            // ChangeWidths
            // 
            this.ChangeWidths.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ChangeWidths.AutoSize = true;
            this.ChangeWidths.Location = new System.Drawing.Point(546, 170);
            this.ChangeWidths.Name = "ChangeWidths";
            this.ChangeWidths.Size = new System.Drawing.Size(151, 16);
            this.ChangeWidths.TabIndex = 10;
            this.ChangeWidths.Text = "Change character widths";
            this.ChangeWidths.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.MSButton);
            this.panel1.Controls.Add(this.CGButton);
            this.panel1.Location = new System.Drawing.Point(563, 112);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(209, 52);
            this.panel1.TabIndex = 13;
            // 
            // UseFullwidthAlphabets
            // 
            this.UseFullwidthAlphabets.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.UseFullwidthAlphabets.AutoSize = true;
            this.UseFullwidthAlphabets.Location = new System.Drawing.Point(567, 239);
            this.UseFullwidthAlphabets.Name = "UseFullwidthAlphabets";
            this.UseFullwidthAlphabets.Size = new System.Drawing.Size(144, 16);
            this.UseFullwidthAlphabets.TabIndex = 15;
            this.UseFullwidthAlphabets.Text = "Use fullwidth alphabets";
            this.UseFullwidthAlphabets.UseVisualStyleBackColor = true;
            // 
            // UseFullwidthDigits
            // 
            this.UseFullwidthDigits.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.UseFullwidthDigits.AutoSize = true;
            this.UseFullwidthDigits.Location = new System.Drawing.Point(567, 261);
            this.UseFullwidthDigits.Name = "UseFullwidthDigits";
            this.UseFullwidthDigits.Size = new System.Drawing.Size(123, 16);
            this.UseFullwidthDigits.TabIndex = 16;
            this.UseFullwidthDigits.Text = "Use fullwidth digits";
            this.UseFullwidthDigits.UseVisualStyleBackColor = true;
            // 
            // UseFullwidthSymbols
            // 
            this.UseFullwidthSymbols.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.UseFullwidthSymbols.AutoSize = true;
            this.UseFullwidthSymbols.Location = new System.Drawing.Point(567, 305);
            this.UseFullwidthSymbols.Name = "UseFullwidthSymbols";
            this.UseFullwidthSymbols.Size = new System.Drawing.Size(157, 16);
            this.UseFullwidthSymbols.TabIndex = 17;
            this.UseFullwidthSymbols.Text = "Use fullwidth symbols for:";
            this.UseFullwidthSymbols.UseVisualStyleBackColor = true;
            // 
            // FullwidthSymbolsList
            // 
            this.FullwidthSymbolsList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FullwidthSymbolsList.Location = new System.Drawing.Point(588, 327);
            this.FullwidthSymbolsList.Name = "FullwidthSymbolsList";
            this.FullwidthSymbolsList.Size = new System.Drawing.Size(182, 19);
            this.FullwidthSymbolsList.TabIndex = 18;
            // 
            // UseIdeographicSpace
            // 
            this.UseIdeographicSpace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.UseIdeographicSpace.AutoSize = true;
            this.UseIdeographicSpace.Location = new System.Drawing.Point(567, 283);
            this.UseIdeographicSpace.Name = "UseIdeographicSpace";
            this.UseIdeographicSpace.Size = new System.Drawing.Size(214, 16);
            this.UseIdeographicSpace.TabIndex = 19;
            this.UseIdeographicSpace.Text = "Use Ideographic space (全角スペース)";
            this.UseIdeographicSpace.UseVisualStyleBackColor = true;
            // 
            // UseHalfwidthSymbols
            // 
            this.UseHalfwidthSymbols.AutoSize = true;
            this.UseHalfwidthSymbols.Location = new System.Drawing.Point(567, 356);
            this.UseHalfwidthSymbols.Name = "UseHalfwidthSymbols";
            this.UseHalfwidthSymbols.Size = new System.Drawing.Size(169, 16);
            this.UseHalfwidthSymbols.TabIndex = 20;
            this.UseHalfwidthSymbols.Text = "Force halfwidth symbols for:";
            this.UseHalfwidthSymbols.UseVisualStyleBackColor = true;
            // 
            // HalfwidthSymbolsList
            // 
            this.HalfwidthSymbolsList.Location = new System.Drawing.Point(588, 378);
            this.HalfwidthSymbolsList.Name = "HalfwidthSymbolsList";
            this.HalfwidthSymbolsList.Size = new System.Drawing.Size(182, 19);
            this.HalfwidthSymbolsList.TabIndex = 21;
            // 
            // DearuCheckBox
            // 
            this.DearuCheckBox.AutoSize = true;
            this.DearuCheckBox.Location = new System.Drawing.Point(567, 68);
            this.DearuCheckBox.Name = "DearuCheckBox";
            this.DearuCheckBox.Size = new System.Drawing.Size(164, 16);
            this.DearuCheckBox.TabIndex = 22;
            this.DearuCheckBox.Text = "Prefer である ending over だ";
            this.DearuCheckBox.UseVisualStyleBackColor = true;
            // 
            // HtmlSyntax
            // 
            this.HtmlSyntax.AutoSize = true;
            this.HtmlSyntax.Location = new System.Drawing.Point(546, 408);
            this.HtmlSyntax.Name = "HtmlSyntax";
            this.HtmlSyntax.Size = new System.Drawing.Size(148, 16);
            this.HtmlSyntax.TabIndex = 23;
            this.HtmlSyntax.Text = "Recognize HTML syntax";
            this.HtmlSyntax.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AcceptButton = this.GoButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 441);
            this.Controls.Add(this.HtmlSyntax);
            this.Controls.Add(this.DearuCheckBox);
            this.Controls.Add(this.HalfwidthSymbolsList);
            this.Controls.Add(this.UseHalfwidthSymbols);
            this.Controls.Add(this.UseIdeographicSpace);
            this.Controls.Add(this.FullwidthSymbolsList);
            this.Controls.Add(this.Initializing);
            this.Controls.Add(this.UseFullwidthSymbols);
            this.Controls.Add(this.UseFullwidthDigits);
            this.Controls.Add(this.UseFullwidthAlphabets);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.ChangeWidths);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.GoButton);
            this.Controls.Add(this.SpacingCheckBox);
            this.Controls.Add(this.JotaiCheckBox);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(300, 200);
            this.Name = "Form1";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "Japanese Style Changer Demo";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Initializing.ResumeLayout(false);
            this.Initializing.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox SourceText;
        private System.Windows.Forms.TextBox TargetText;
        private System.Windows.Forms.CheckBox JotaiCheckBox;
        private System.Windows.Forms.CheckBox SpacingCheckBox;
        private System.Windows.Forms.RadioButton MSButton;
        private System.Windows.Forms.RadioButton CGButton;
        private System.Windows.Forms.Button GoButton;
        private System.Windows.Forms.Panel Initializing;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox ChangeWidths;
        private System.Windows.Forms.RadioButton HalfwidthButton;
        private System.Windows.Forms.RadioButton FullwidthButton;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.CheckBox UseFullwidthAlphabets;
        private System.Windows.Forms.CheckBox UseFullwidthDigits;
        private System.Windows.Forms.CheckBox UseFullwidthSymbols;
        private System.Windows.Forms.TextBox FullwidthSymbolsList;
        private System.Windows.Forms.CheckBox UseIdeographicSpace;
        private System.Windows.Forms.CheckBox UseHalfwidthSymbols;
        private System.Windows.Forms.TextBox HalfwidthSymbolsList;
        private System.Windows.Forms.CheckBox DearuCheckBox;
        private System.Windows.Forms.CheckBox HtmlSyntax;
    }
}

