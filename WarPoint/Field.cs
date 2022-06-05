using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace WarPoint
{
    public class Field
    {
        public int WIDTH, HEIGHT; // размер игрового поля в пикселях
        public int SIZEX, SIZEY; // кол-во ячеек по горизонтали и вертикали
        public const int CELLSIZE = 30; // размер ячейки в пикселях
        public Cell[,] cells; // тут хранятся все ячейки в двумерном массиве
        public List<Tuple<Cell, HashSet<Point>>> GameData = new List<Tuple<Cell, HashSet<Point>>>(); // тут хранятся занятые точки и области

        public int capturedPointsRed = 0; // сколько точек захватил красный игрок
        public int capturedPointsBlue = 0; // сколько точек захватил синий игрок
        public int RedPoints = 0; // сколько точек поставил красный игрок
        public int BluePoints = 0; // сколько точек поставил синий игрок

        public Field(int x, int y, int yDeltaPix, bool usePrePlayedField) // инициализация класса
        {
            WIDTH = x * CELLSIZE + CELLSIZE;
            HEIGHT = y * CELLSIZE + CELLSIZE + yDeltaPix;
            SIZEX = x;
            SIZEY = y;
            cells = new Cell[SIZEX, SIZEY]; //заполнение пустыми ячейками
            for (int i = 0; i < SIZEX; i++)
                for (int j = 0; j < SIZEY; j++)
                    cells[i, j] = Cell.Empty;

            if (usePrePlayedField)
            {
                int i = SIZEX / 2 - 1;
                int j = SIZEY / 2 - 1;
                cells[i, j] = Cell.Blue;
                cells[i + 1, j] = Cell.Red;
                cells[i, j + 1] = Cell.Red;
                cells[i + 1, j + 1] = Cell.Blue;
                RedPoints += 2;
                BluePoints += 2;
            }
        }


        public Cell this[Point p]
        {
            get
            {
                if (p.X < 0 || p.X >= SIZEX || p.Y < 0 || p.Y >= SIZEY)
                    return Cell.OutOfField;
                return cells[p.X, p.Y];
            }

            set { cells[p.X, p.Y] = value; }
        }


        public Cell ChangeSide(Cell c) // nado li??
        {
            return c == Cell.Blue ? Cell.Red: c == Cell.Red ? Cell.Blue : Cell.OutOfField;
        }

        public void SetNewPoint(Point pt, Cell side)
        {
            if (side == Cell.Blue) BluePoints++; //увеличиваем количество поставленных точек в счетчиках
            else RedPoints++;
            this[pt] = side;
            foreach (HashSet<Point> usedPts in GetClosedArea(pt)) 
                GameData.Add(new Tuple<Cell, HashSet<Point>>(side, usedPts)); //сохраняем данные о точках
        }

        private IEnumerable<HashSet<Point>> GetClosedArea(Point pt)
        {
            Cell side = this[pt];
            foreach (Point n in GetNeighbors4(pt))
                if (this[n] == ChangeSide(side) && side != Cell.BlueOff && side != Cell.RedOff)
                {
                    HashSet<Point> lst = FindClosedArea(n, side);
                    if (lst != null) yield return lst;
                }
        }
        public IEnumerable<Point> GetNeighbors4(Point p) //соседи по вертикали и горизонали
        {
            yield return new Point(p.X - 1, p.Y);
            yield return new Point(p.X, p.Y - 1);
            yield return new Point(p.X + 1, p.Y);
            yield return new Point(p.X, p.Y + 1);
        }
        public IEnumerable<Point> GetNeighbors8(Point p) //соседи по горизонтали, вертикали и по диагоналям
        {
            yield return new Point(p.X - 1, p.Y);
            yield return new Point(p.X - 1, p.Y - 1);
            yield return new Point(p.X, p.Y - 1);
            yield return new Point(p.X + 1, p.Y - 1);
            yield return new Point(p.X + 1, p.Y);
            yield return new Point(p.X + 1, p.Y + 1);
            yield return new Point(p.X, p.Y + 1);
            yield return new Point(p.X - 1, p.Y + 1);
        }


        private HashSet<Point> FindClosedArea(Point pt, Cell side)
        {
            Stack<Point> ptsToCheck = new Stack<Point>(); // Стэк
            HashSet<Point> ptsVisited = new HashSet<Point>(); // Множество
            ptsToCheck.Push(pt);
            ptsVisited.Add(pt);
            while (ptsToCheck.Count > 0)
            {
                var p = ptsToCheck.Pop();
                var sideP = this[p];
                //если вышли за пределы поля - значит область не замкнута и возвращаем null
                if (sideP == Cell.OutOfField)
                    return null;
                //рекурсивно перебираем соседей
                foreach (Point n in GetNeighbors4(p))
                    if (this[n] != side && !ptsVisited.Contains(n))
                    {
                        ptsToCheck.Push(n); 
                        ptsVisited.Add(n);
                    }
            }
            return ptsVisited;
        }

        public IEnumerable<Point> GetContour(HashSet<Point> usedPts)
        {
            //ищем точку из контура
            Point startPoint = new Point();
            foreach (Point p in usedPts)
                foreach (Point np in GetNeighbors4(p))
                    if (!usedPts.Contains(np))
                    {
                        startPoint = np;
                        goto next;
                    }

            next:

            //обход по часовой стрелке вдоль области
            yield return startPoint;
            Point pt = GetNext(startPoint, usedPts);
            while (pt != startPoint)
            {
                yield return pt;
                pt = GetNext(pt, usedPts);
            }
        }

        private Point GetNext(Point p, HashSet<Point> usedPts)
        {
            List<Point> neibs = GetNeighbors8(p).ToList();
            List<Point> pts = new List<Point>(neibs);
            pts.AddRange(neibs);
            for (int i = 0; i < pts.Count - 1; i++)
                if (!usedPts.Contains(pts[i]) && usedPts.Contains(pts[i + 1]))
                    return pts[i];

            return Point.Empty; //Need test. Before:(throw new Exception("Error");)
        }
        public Cell DeactivateCell(Cell c, Cell Pside)
        {
            if (c == Cell.Blue || c == Cell.Red || c == Cell.Empty)
            {
                if (Pside == Cell.Blue) capturedPointsBlue++;
                else capturedPointsRed++;
            }
            return ((c == Cell.Blue) ? Cell.BlueOff : (c == Cell.Red) ? Cell.RedOff : Cell.EmptyOff);
        }
    }


    public enum Cell
    {
        Empty = 1,
        Red = 2,
        Blue = 3,
        EmptyOff = -1,
        RedOff = -2,
        BlueOff = -3,
        OutOfField = 0
    }
}
