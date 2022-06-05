using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace WarPoint
{

    public partial class Form1 : Form
    {
        public MenuForm menufrm;

        public Field field; //игровое поле

        //игровые настройки
        public Tuple<int, int, int, bool> fieldSettings;
        public Tuple<bool, int> timerSettings;
        public Tuple<bool, int> immWinSettings;
        public Tuple<bool, int> MaxPtsWinSettings;

        const int pointRad = 10; //радиус точек, в пикселях
        static Point startPosNet = new Point(0, 29); //стартовая позиция сетки, в пикселях
        static int startPosDelta = Field.CELLSIZE / 2; //стартовая позиция сетки для курсора, в пикселях

        static Color blueSIC = Color.FromArgb(0, 80, 255); //цвет синей полоски
        static Color redSIC = Color.FromArgb(235, 0, 80); //цвет красной полоски
        Cell nowSide;

        public bool canPlay = true; //может ли игрок ставить точки
        public int timeToEndGame; //таймер

        Results childForm;
        Pen pen = new Pen(Color.Red);
        SolidBrush brush = new SolidBrush(Color.Red);

        public Form1(Tuple<int, int, int, bool> FS, Tuple<bool, int> TS, Tuple<bool, int> IWS, Tuple<bool, int> MPWS)
        {
            InitializeComponent();
            fieldSettings = FS;
            timerSettings = TS;
            immWinSettings = IWS;
            MaxPtsWinSettings = MPWS;
            InitGame();
        }

        private void InitGame()
        {
            if (childForm != null) childForm.Close();
            field = new Field(fieldSettings.Item1, fieldSettings.Item2, fieldSettings.Item3, fieldSettings.Item4);
            nowSide = Cell.Blue;
            this.Width = field.WIDTH;
            this.Height = field.HEIGHT;
            this.MaximumSize = new Size(this.Width, this.Height);
            sideIndicator.Size = new Size(field.WIDTH - 35, 23);
            sideIndicator.Location = new Point(10, field.HEIGHT - sideIndicator.Height * 3);
            sideIndicator.BackColor = blueSIC;

            timer1.Enabled = timerSettings.Item1;
            timeToEndGame = timerSettings.Item2;
            label1.Visible = timerSettings.Item1;
            label1.Location = new Point(this.Width / 2 - 50, sideIndicator.Location.Y - label1.Height);

            TimerCheckWin.Enabled = true;
            canPlay = true;

            Invalidate();
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (!canPlay) return;
            int mouseX = e.X;
            int mouseY = e.Y;
            int X = (mouseX - startPosDelta - startPosNet.X) / Field.CELLSIZE;
            int Y = (mouseY - startPosDelta - startPosNet.Y) / Field.CELLSIZE;
            Point newPoint = new Point(X, Y);
            if (field[newPoint] != Cell.Empty) return;
            field.SetNewPoint(newPoint, nowSide);
            nowSide = field.ChangeSide(nowSide);
            sideIndicator.BackColor = nowSide == Cell.Blue ? blueSIC : redSIC;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawNet(e.Graphics); //рисуем сетку
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

            //рисуем замкнутые области
            foreach (Tuple<Cell, HashSet<Point>> item in field.GameData)
            {
                Cell sideP = item.Item1;
                HashSet<Point> area = item.Item2;
                foreach (Point p in area) //выключаем точки внутри контуров
                {
                    int X = p.X * Field.CELLSIZE + Field.CELLSIZE + startPosNet.X;
                    int Y = p.Y * Field.CELLSIZE + Field.CELLSIZE + startPosNet.Y;
                    field[p] = field.DeactivateCell(field[p], nowSide);
                    //e.Graphics.FillEllipse(new SolidBrush(Color.Green), X - 10, Y - 10, 20, 20); //DEBUG
                }

                Point[] contour = field.GetContour(area).ToArray();

                //рисуем контур
                pen.Color = (Cell.Blue == sideP ? Color.Blue : Color.Red);
                pen.Width = 2;
                brush.Color = (Cell.Blue == sideP ? Color.Blue : Color.Red);
                brush.Color = Color.FromArgb(100, brush.Color.R, brush.Color.G, brush.Color.B);
                
                for (int i = 0; i < contour.Count(); i++)
                {
                    int X = contour[i].X * Field.CELLSIZE + Field.CELLSIZE + startPosNet.X;
                    int Y = contour[i].Y * Field.CELLSIZE + Field.CELLSIZE + startPosNet.Y;
                    contour[i] = new Point(X, Y);
                }
                
                e.Graphics.FillPolygon(brush, contour.ToArray());
                e.Graphics.DrawPolygon(pen, contour.ToArray());
                
            }
            

            //рисуем точки
            brush.Color = Color.White;
            for (int x = 0; x < field.SIZEX; x++)
                for (int y = 0; y < field.SIZEY; y++)
                {
                    Point p = new Point(x, y);
                    Cell cell = field[p];
                    if (cell != Cell.Empty)
                    {
                        brush.Color = (Cell.Blue == cell ? Color.Blue : Cell.Red == cell? Color.Red: Color.Purple);
                        e.Graphics.FillEllipse(brush, x * Field.CELLSIZE + Field.CELLSIZE - pointRad / 2 + startPosNet.X, 
                                                      y * Field.CELLSIZE + Field.CELLSIZE - pointRad / 2 + startPosNet.Y, pointRad, pointRad);
                    }
                }
        }

        private void DrawNet(Graphics g)
        {
            pen.Color = Color.Gray;
            pen.Width = 1;
            for (int x = 0; x < field.SIZEX * Field.CELLSIZE; x += Field.CELLSIZE)
                g.DrawLine(pen, x + Field.CELLSIZE + startPosNet.X, 0, x + Field.CELLSIZE + startPosNet.X, (field.SIZEY + 1) * Field.CELLSIZE);

            for (int y = 0; y < field.SIZEY * Field.CELLSIZE; y += Field.CELLSIZE)
                g.DrawLine(pen, 0, y + Field.CELLSIZE + startPosNet.Y, (field.SIZEX + 1) * Field.CELLSIZE, y + Field.CELLSIZE + startPosNet.Y);
        }

        private void goToMenuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure to go to menu? All game progress will be deleted!", "Go to menu", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No) return;
            menufrm.Show();
            this.Close();
        }

        private void restartGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure to restart game? All game progress will be deleted!", "Restart game", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No) return;
            InitGame();
        }

        private void surrenderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure to surrender? You will loose the game!", "Surrender", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
            ShowResult(field.ChangeSide(nowSide));
        }

        public void timer1_Tick(object sender, EventArgs e)
        {
            if (timeToEndGame != 0)
            {
                timeToEndGame--;
                int min = (timeToEndGame / 60), sec = (timeToEndGame - timeToEndGame / 60 * 60);
                string minZeroAdd = min < 10 ? "0" : "", secZeroAdd = sec < 10 ? "0" : "";

                label1.Text = minZeroAdd + min + ":" + secZeroAdd + sec;
            }
            else
            {
                if (field.capturedPointsBlue < field.capturedPointsRed) ShowResult(Cell.Blue);
                else if (field.capturedPointsBlue > field.capturedPointsRed) ShowResult(Cell.Red);
                else ShowResult(Cell.Empty);
            }
        }

        public void CheckWin()
        {
            bool immW = immWinSettings.Item1 && (field.capturedPointsBlue >= immWinSettings.Item2 || field.capturedPointsRed >= immWinSettings.Item2) ;
            bool maxPt = MaxPtsWinSettings.Item1 && Math.Abs(field.capturedPointsBlue - field.capturedPointsRed) >= MaxPtsWinSettings.Item2;
            if (immW || maxPt)
            {
                TimerCheckWin.Enabled = false;
                canPlay = false;
                ShowResult((field.capturedPointsBlue < field.capturedPointsRed) ? Cell.Blue : Cell.Red);
            }
            
        }

        public void ShowResult(Cell whoWin)
        {
            timer1.Enabled = false;
            childForm = new Results(Cell.Blue == whoWin ? 1: Cell.Red == whoWin ? -1: 0);
            this.Width = childForm.Width;
            this.Height = childForm.Height;
            childForm.game = this;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            this.Controls.Add(childForm);
            childForm.BringToFront();
            childForm.InitComponents();
            childForm.Show();
        }

        private void TimerCheckWin_Tick(object sender, EventArgs e)
        {
            CheckWin();
        }
    }
}
