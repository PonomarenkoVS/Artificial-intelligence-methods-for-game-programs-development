using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameHelper
{
    /// <summary>
    /// Информация о ячейках игрового поля, которые образуют выигрышную комбинацию
    /// </summary>
    public class WonCoordinatesInfo
    {
        /// <summary>
        /// тип фигуры, которуюобразуют ячейки игрового поля в случае выигрыша любого из игроков
        /// </summary>
        public LineType LineType;
        /// <summary>
        ///  Список ячеек игрового поля, которые образуют выигрышную комбинацию
        /// </summary>
        public List<Point> Points;//заделка на будущее, чтобы выигрышной фигурой могла быть не только прямая, но и любая прямоугольная (а может и не только) фигура

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="points">Список ячеек игровой доски, которые образуют выигрышную комбинацию</param>
        public WonCoordinatesInfo(List<Point> points)
        {
            Points = points;
        }
        /// <summary>
        /// Конструктор. Вычисляет список ячеек игровой доски, которы образуют выигрышную комбинации
        /// </summary>
        /// <param name="start">Начальная точка прямой,на которой лежат "выигрышные" ячейки</param>
        /// <param name="end">Конечная точка прямой,на которой лежат "выигрышные" ячейки</param>
        /// <param name="lineType">Тип фигуры, ячейки на которой образуют выигрышную комбинацию</param>
        /// <exception cref="ArgumentException"></exception>
        public WonCoordinatesInfo(Point start, Point end, LineType lineType)
        {
            //если тип фигуры - пользовательская фигура (а не прямая линия), то мы не можем вычислить автоматически список выигрышных ячеек
            if (lineType == LineType.UserFigure)
            {
                throw new ArgumentException("Данный конструктор не предназначен для пользовательского типа фигуры LineType.UserFigure. Используйте конструктор WonCoordinatesInfo(List<Point> points)");
            }

            LineType = lineType;

            //вычисляем элементы, находящиеся на выигрышной "линии" в зависимости от ее типа, зная адрес начальной и конечной ячейки
            Points = new List<Point>();
            switch (lineType)
            {
                case LineType.Vertical:
                    var y = start.Y;
                    for (int i = start.X; i <= end.X; i++)
                    {
                        Points.Add(new Point(i, y));
                    }
                    break;
                case LineType.Horizontal:
                    var x = start.X;
                    for (int i = start.Y; i <= end.Y; i++)
                    {
                        Points.Add(new Point(x, i));
                    }
                    break;
                case LineType.LeftDiagonal:
                    var x1 = start.X;
                    var y1 = start.Y;

                    var x2 = end.X;
                    var y2 = end.Y;

                    for (int i = x1; i <= x2; i++)
                    {
                        Points.Add(new Point(i, y1++));
                    }
                    break;
                case LineType.RightDiagonal:
                    var y0 = start.Y;

                    for (int i = start.X; i <= end.X; i++)
                    {
                        Points.Add(new Point(i, y0--));
                    }
                    break;
            }
        }

        public WonCoordinatesInfo() { }
    }
}
