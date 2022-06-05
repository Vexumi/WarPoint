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
    public partial class MenuForm : Form
    {
        public Form1 frm1;

        public MenuForm()
        {
            InitializeComponent();
            this.MaximumSize = new Size(this.Width, this.Height);
        }


        private void buttonStart_Click(object sender, EventArgs e)
        {
            int deltaY;
            StreamReader sr = new StreamReader(@"Settings.txt");
            int xDots = Convert.ToInt32(sr.ReadLine().Replace("\n", ""));
            int yDots = Convert.ToInt32(sr.ReadLine().Replace("\n", ""));
            bool usePrePlayedField = Convert.ToBoolean(sr.ReadLine().Replace("\n", ""));
            bool useTimer = Convert.ToBoolean(sr.ReadLine().Replace("\n", ""));
            int timerTime = Convert.ToInt32(sr.ReadLine().Replace("\n", ""));
            bool useImmWin = Convert.ToBoolean(sr.ReadLine().Replace("\n", ""));
            int immWinPoints = Convert.ToInt32(sr.ReadLine());
            bool MaxPtsWinOn = Convert.ToBoolean(sr.ReadLine());
            int MaxPointWin = Convert.ToInt32(sr.ReadLine());
            sr.Close();

            if (useTimer) deltaY = 150;
            else deltaY = 100;

            Tuple<int, int, int, bool> fieldSettings = new Tuple<int, int, int, bool> (xDots, yDots, deltaY, usePrePlayedField);
            Tuple<bool, int> timerSettings = new Tuple<bool, int>(useTimer, timerTime);
            Tuple<bool, int> immWinSettings = new Tuple<bool, int>(useImmWin, immWinPoints);
            Tuple<bool, int> MaxPtsWinSettings = new Tuple<bool, int>(MaxPtsWinOn, MaxPointWin);

            frm1 = new Form1(fieldSettings, timerSettings, immWinSettings, MaxPtsWinSettings);
            frm1.Show();
            frm1.menufrm = this;
            this.Hide();
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure to exit?", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
            if (activeForm != null) activeForm.Close();
            Application.Exit();
        }


        private Form activeForm = null;
        private void openChildForm(Form childForm)
        {
            if (activeForm != null)
                activeForm.Close();
            activeForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            panelChildForm.Controls.Add(childForm);
            childForm.BringToFront();
            childForm.Show();
        }

        private void buttonSettings_Click(object sender, EventArgs e)
        {
            openChildForm(new Settings());
        }

        private void buttonHelp_Click(object sender, EventArgs e)
        {
            openChildForm(new Help());
        }
    }
}
