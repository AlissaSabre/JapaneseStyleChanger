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
            this.label1 = new System.Windows.Forms.Label();
            this.GoButton = new System.Windows.Forms.Button();
            this.Initializing = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.Initializing.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // SourceText
            // 
            this.SourceText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SourceText.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.SourceText.Location = new System.Drawing.Point(0, 0);
            this.SourceText.Multiline = true;
            this.SourceText.Name = "SourceText";
            this.SourceText.Size = new System.Drawing.Size(568, 208);
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
            this.TargetText.Size = new System.Drawing.Size(568, 205);
            this.TargetText.TabIndex = 1;
            // 
            // JotaiCheckBox
            // 
            this.JotaiCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.JotaiCheckBox.AutoSize = true;
            this.JotaiCheckBox.Checked = true;
            this.JotaiCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.JotaiCheckBox.Location = new System.Drawing.Point(603, 91);
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
            this.SpacingCheckBox.Location = new System.Drawing.Point(603, 122);
            this.SpacingCheckBox.Name = "SpacingCheckBox";
            this.SpacingCheckBox.Size = new System.Drawing.Size(105, 16);
            this.SpacingCheckBox.TabIndex = 3;
            this.SpacingCheckBox.Text = "Change spacing";
            this.SpacingCheckBox.UseVisualStyleBackColor = true;
            // 
            // MSButton
            // 
            this.MSButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MSButton.AutoSize = true;
            this.MSButton.Checked = true;
            this.MSButton.Location = new System.Drawing.Point(620, 144);
            this.MSButton.Name = "MSButton";
            this.MSButton.Size = new System.Drawing.Size(68, 16);
            this.MSButton.TabIndex = 4;
            this.MSButton.TabStop = true;
            this.MSButton.Text = "MS style";
            this.MSButton.UseVisualStyleBackColor = true;
            // 
            // CGButton
            // 
            this.CGButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CGButton.AutoSize = true;
            this.CGButton.Location = new System.Drawing.Point(620, 166);
            this.CGButton.Name = "CGButton";
            this.CGButton.Size = new System.Drawing.Size(111, 16);
            this.CGButton.TabIndex = 5;
            this.CGButton.Text = "Minimum spacing";
            this.CGButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(601, 63);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "Options";
            // 
            // GoButton
            // 
            this.GoButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.GoButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.GoButton.Location = new System.Drawing.Point(603, 12);
            this.GoButton.Name = "GoButton";
            this.GoButton.Size = new System.Drawing.Size(169, 23);
            this.GoButton.TabIndex = 7;
            this.GoButton.Text = "Go";
            this.GoButton.UseVisualStyleBackColor = true;
            this.GoButton.Click += new System.EventHandler(this.GoButton_Click);
            // 
            // Initializing
            // 
            this.Initializing.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
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
            this.splitContainer1.Size = new System.Drawing.Size(568, 417);
            this.splitContainer1.SplitterDistance = 208;
            this.splitContainer1.TabIndex = 9;
            // 
            // Form1
            // 
            this.AcceptButton = this.GoButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 441);
            this.Controls.Add(this.Initializing);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.GoButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CGButton);
            this.Controls.Add(this.MSButton);
            this.Controls.Add(this.SpacingCheckBox);
            this.Controls.Add(this.JotaiCheckBox);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(300, 200);
            this.Name = "Form1";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "Japanese Style Changer Demo";
            this.Initializing.ResumeLayout(false);
            this.Initializing.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
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
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button GoButton;
        private System.Windows.Forms.Panel Initializing;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}

