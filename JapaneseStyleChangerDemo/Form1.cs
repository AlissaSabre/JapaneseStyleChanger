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
            Busy = true;
            Task.Run(() =>
            {
                Exception exception = null;
                try
                {
                    Changer = new TextStyleChanger();
                }
                catch (Exception e)
                {
                    exception = e;
                }
                BeginInvoke((Action)(() =>
                {
                    if (exception == null)
                    {
                        Initializing.Visible = false;
                        Busy = false;
                    }
                    else
                    {
                        MessageBox.Show(this, exception.Message, "Exception - Japanese Style Changer Demo");
                        Close();
                    }
                }));
            });
        }

        private TextStyleChanger Changer;

        private void Application_Idle(object sender, EventArgs e)
        {
            DearuCheckBox.Enabled = JotaiCheckBox.Checked;

            MSButton.Enabled = SpacingCheckBox.Checked;
            CGButton.Enabled = SpacingCheckBox.Checked;

            HalfwidthButton.Enabled = ChangeWidths.Checked;
            FullwidthButton.Enabled = ChangeWidths.Checked;
            UseFullwidthAlphabets.Enabled = ChangeWidths.Checked;
            UseFullwidthDigits.Enabled = ChangeWidths.Checked;
            UseIdeographicSpace.Enabled = ChangeWidths.Checked;
            UseFullwidthSymbols.Enabled = ChangeWidths.Checked;
            UseHalfwidthSymbols.Enabled = ChangeWidths.Checked;

            FullwidthSymbolsList.Enabled = ChangeWidths.Checked && UseFullwidthSymbols.Checked;
            HalfwidthSymbolsList.Enabled = ChangeWidths.Checked && UseHalfwidthSymbols.Checked;
        }

        private bool _Busy;

        private bool Busy
        {
            get { return _Busy; }
            set
            {
                _Busy = value;
                UseWaitCursor = value;
                Cursor = value ? Cursors.WaitCursor : Cursors.Default;
                GoButton.Enabled = !value;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FullwidthSymbolsList.Text = string.Concat(TokenCombiner.OtherAsciiSymbols.Except(new[] { '\u0020' }));
        }

        private async void GoButton_Click(object sender, EventArgs e)
        {
            Busy = true;
            try
            {
                Changer.ChangeToJotai = JotaiCheckBox.Checked;
                Changer.JotaiPreferences = DearuCheckBox.Checked ? JotaiPreferences.PreferDearu : JotaiPreferences.None;
                Changer.CombineMode = SpacingCheckBox.Checked
                    ? (MSButton.Checked ? CombineMode.MS : CombineMode.CG)
                    : CombineMode.Default;

                var wp = WidthPreferences.None;
                if (ChangeWidths.Checked)
                {
                    if (HalfwidthButton.Checked) wp |= WidthPreferences.HalfwidthParentheses;
                    if (FullwidthButton.Checked) wp |= WidthPreferences.FullwidthParentheses;
                    if (UseFullwidthAlphabets.Checked) wp |= WidthPreferences.FullwidthAlphabets;
                    if (UseFullwidthDigits.Checked) wp |= WidthPreferences.FullwidthDigits;
                    if (UseIdeographicSpace.Checked) wp |= WidthPreferences.CustomFullwidthSet;
                    if (UseFullwidthSymbols.Checked) wp |= WidthPreferences.CustomFullwidthSet;
                    if (UseHalfwidthSymbols.Checked) wp |= WidthPreferences.CustomHalfwidthSet;
                }
                Changer.WidthPreferences = wp;

                string fws = string.Empty;
                if (UseFullwidthSymbols.Checked) fws = FullwidthSymbolsList.Text;
                if (UseIdeographicSpace.Checked) fws += '\u0020';
                Changer.CustomFullwidthSet = fws;

                Changer.CustomHalfwidthSet = HalfwidthSymbolsList.Text;

                var text = SourceText.Text;
                var result = await Task.Run(() => Changer.ChangeText(text));
                TargetText.Text = result;
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, exception.Message, "Exception - Japanese Style Changer Demo");
            }
            Busy = false;
        }
    }
}
