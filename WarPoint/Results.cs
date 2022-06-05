using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WarPoint
{
    public partial class Results : Form
    {
        public Form1 game;
        int whoWin;

        public Results(int side)
        {
            InitializeComponent();
            whoWin = side;
        }

        public void InitComponents()
        {
            
            listBox1.Items.Add("Points: " + game.field.BluePoints.ToString());
            listBox1.Items.Add("Captured Points: " + game.field.capturedPointsRed.ToString());

            listBox2.Items.Add("Points: " + game.field.RedPoints.ToString());
            listBox2.Items.Add("Captured Points: " + game.field.capturedPointsBlue.ToString());

            listBox3.Items.Add("Winner: " + (whoWin == 1? "Blue side": (whoWin == -1? "Red side": "Draw")));
            listBox3.Items.Add("Points: " + (game.field.BluePoints + game.field.RedPoints).ToString());
            listBox3.Items.Add("Captured Points: " + (game.field.capturedPointsBlue + game.field.capturedPointsRed).ToString());

            if (game.timerSettings.Item1) listBox3.Items.Add("Game Time: " + ((game.timerSettings.Item2 - game.timeToEndGame) / 60).ToString() + ":"
                                                + ((game.timerSettings.Item2 - game.timeToEndGame) - (game.timerSettings.Item2 - game.timeToEndGame) / 60 * 60).ToString());

            if (game.timerSettings.Item1 || game.immWinSettings.Item1 || game.MaxPtsWinSettings.Item1) listBox3.Items.Add("Game Modes: ");
            if (game.timerSettings.Item1) listBox3.Items.Add("\tTime");
            if (game.immWinSettings.Item1) listBox3.Items.Add("\tMax Points");
            if (game.MaxPtsWinSettings.Item1) listBox3.Items.Add("\tSuperiority");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Exit();
        }
        public void Exit()
        {
            game.Width = game.field.WIDTH;
            game.Height = game.field.HEIGHT;
            this.Close();
        }
    }
}
