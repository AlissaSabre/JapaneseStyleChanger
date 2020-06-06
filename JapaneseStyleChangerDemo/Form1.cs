using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using JapaneseStyleChanger;

namespace JapaneseStyleChangerDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            Application.Idle += Application_Idle;
        }

        private TextStyleChanger Changer = new TextStyleChanger();

        private void Application_Idle(object sender, EventArgs e)
        {
            MSButton.Enabled = SpacingCheckBox.Checked;
            CGButton.Enabled = SpacingCheckBox.Checked;
        }

        private async void GoButton_Click(object sender, EventArgs e)
        {
            try
            {
                Changer.ChangeToJotai = JotaiCheckBox.Checked;
                Changer.CombineMode = SpacingCheckBox.Checked
                    ? (MSButton.Checked ? CombineMode.MS : CombineMode.CG)
                    : CombineMode.Default;
                var text = SourceText.Text;
                var result = await Task.Run(() => Changer.ChangeText(text));
                TargetText.Text = result;
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, exception.Message, "Exception - Japanese Style Changer Demo");
            }
        }
    }
}
