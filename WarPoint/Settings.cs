using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace WarPoint
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
            LoadSettings();
        }
        private void checkBoxTimerOn_CheckedChanged(object sender, EventArgs e)
        {
            TimerNumUpDown.Enabled = checkBoxTimerOn.Checked;
        }

        private void checkBoxImmediateWin_CheckedChanged(object sender, EventArgs e)
        {
            ImmediateWinNumUpDown.Enabled = checkBoxImmediateWin.Checked;
        }
        private void checkBoxMaxPointsWin_CheckedChanged(object sender, EventArgs e)
        {
            MaxPtsWinNumUpDown.Enabled = checkBoxMaxPointsWin.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void LoadSettings()
        {
            StreamReader sr = new StreamReader("Settings.txt");

            HorizontalDotsNumUpDown.Value = Convert.ToInt32(sr.ReadLine());
            VerticalDotsNumUpDown.Value = Convert.ToInt32(sr.ReadLine());
            checkBoxPrePlayedField.Checked = Convert.ToBoolean(sr.ReadLine());
            checkBoxTimerOn.Checked = Convert.ToBoolean(sr.ReadLine());
            TimerNumUpDown.Value = Convert.ToInt32(sr.ReadLine());
            checkBoxImmediateWin.Checked = Convert.ToBoolean(sr.ReadLine());
            ImmediateWinNumUpDown.Value = Convert.ToInt32(sr.ReadLine());
            checkBoxMaxPointsWin.Checked = Convert.ToBoolean(sr.ReadLine());
            MaxPtsWinNumUpDown.Value = Convert.ToInt32(sr.ReadLine());
            sr.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StreamWriter sw = new StreamWriter("Settings.txt");
            sw.WriteLine(HorizontalDotsNumUpDown.Value);
            sw.WriteLine(VerticalDotsNumUpDown.Value);
            sw.WriteLine(checkBoxPrePlayedField.Checked);
            sw.WriteLine(checkBoxTimerOn.Checked);
            sw.WriteLine(TimerNumUpDown.Value);
            sw.WriteLine(checkBoxImmediateWin.Checked);
            sw.WriteLine(ImmediateWinNumUpDown.Value);
            sw.WriteLine(checkBoxMaxPointsWin.Checked);
            sw.WriteLine(MaxPtsWinNumUpDown.Value);
            sw.Close();

            MessageBox.Show("Settings Saved!", "Success save", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure to set default settings?", "Default Settings", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

            StreamWriter sw = new StreamWriter("Settings.txt");
            StreamReader sr = new StreamReader("DefaultSettings.txt");
            sw.Write(sr.ReadToEnd());
            sw.Close();
            sr.Close();
            LoadSettings();
            MessageBox.Show("Default Settings Saved!", "Success save", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
