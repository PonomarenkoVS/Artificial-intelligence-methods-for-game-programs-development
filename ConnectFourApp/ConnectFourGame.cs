using GameHelper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectFourApp
{
    /// <summary>
    /// Класс с методами для игры 4 в ряд
    /// </summary>
    public class ConnectFourGame
    {
        /// <summary>
        /// Возвращает список возможных ходов
        /// </summary>
        /// <param name="field">Игровое поле</param>
        /// <param name="playerMark">Метка игрока</param>
        /// <returns></returns>
        public static List<Point> GetPossibleMoves(string[,] field, string playerMark)
        {
            var result = new List<Point>();
            var m = field.GetLength(1);
            for (int j = 0; j < m; j++)
            {
                //свободна ли ячейка в первой строке(тоже в метод можно оформить внутри game)
                if (field[0, j] == " ")
                {
                    //то это наш возможный ход
                    result.Add(new Point(0, j));
                }
            }

            return result;
        }

        /// <summary>
        /// Уронить фишку вниз
        /// </summary>
        /// <param name="field">Игровое поле</param>
        /// <param name="step">Последний ход</param>
        /// <param name="playerMark">Метка игрока</param>
        public static void DropChipDown(string[,] field, Point step, string playerMark)
        {
            var n = field.GetLength(0);

            var targetRow = -1;

            //фишка должна упасть
            var column = step.Y;
            //идем вниз (по каждой строке) по столбцу column
            for (int i = 1/*потому что сюда кладем фишку при ходе*/; i < n; i++)
            {
                //свободна ли ячейка (тоже в метод можно оформитьь внутри game)
                if (field[i, column] == " ")
                {
                    //то продолжаем смотреть дальше
                    continue;
                }

                //если дошли до этой строчки, значит нашли верхнюю фишку в столбце column
                //а значит, нужно запоминаем индекс последней свободной ячейки в столбце
                targetRow = i - 1;
                //и выходим из цикла, т.к. нашли что искали
                break;
            }

            if (targetRow != -1)
            {
                //"убираем" фишку с ячейки, куда опустили ее
                field[0, column] = " ";
                //ставим ее в ту ячейку, куда бы она упала вниз
                field[targetRow, column] = playerMark;
            }
            else
            {
                //а если осталась -1, то значит фишка упала в нижнюю ячейку
                //"убираем" фишку с ячейки, куда опустили ее
                field[0, column] = " ";
                //ставим ее в ту ячейку, куда бы она упала вниз
                field[n - 1, column] = playerMark;
            }
        }

        /// <summary>
        /// правила выигрыша для игры 4 в ряд
        /// </summary>
        /// <param name="field">Игровое поле</param>
        /// <param name="playerMark">Метка игрока</param>
        /// <param name="coordinates">Выигрышные координаты</param>
        /// <returns></returns>
        public static bool CheckWinMethod(string[,] field, string playerMark, out WonCoordinatesInfo coordinates)
        {
            //берем размерность игрового поля
            var n = field.GetLength(0);
            var m = field.GetLength(1);
            //список ячеек, куда будем сохранять ячейки, образующие выигрышную комбинацию
            var points  = new List<Point>();

            //есть ли ряд по горизонтали
            var horizontalLine = -1;
            for (int i = 0; i < n; i++)
            {
                var hasLine = true;
                var itemsPerLine = 0;
                for (int j = 0; j < m; j++)
                {
                    if (field[i, j] == playerMark)
                    {
                        points.Add(new Point(i, j));
                        itemsPerLine++;
                        if (itemsPerLine >= 4)
                        {
                            hasLine = true;
                            break;
                        }
                    }
                    else
                    {
                        points.Clear();
                        itemsPerLine = 0;
                        hasLine = false;
                    }
                }
                if (hasLine)
                {
                    horizontalLine = i;
                    break;
                }
            }
            if (horizontalLine != -1)
            {
                coordinates = new WonCoordinatesInfo(points);
                return true;
            }

            //есть ли ряд по вертикали
            var verticalLine = -1;
            for (int j = 0; j < m; j++)
            {
                var hasLine = true;
                var itemsPerLine = 0;
                for (int i = 0; i < n; i++)
                {
                    if (field[i, j] == playerMark)
                    {
                        points.Add(new Point(i, j));
                        itemsPerLine++;
                        if (itemsPerLine >= 4)
                        {
                            hasLine = true;
                            break;
                        }
                    }
                    else
                    {
                        points.Clear();   
                        itemsPerLine = 0;
                        hasLine = false;
                    }
                }
                if (hasLine)
                {
                    verticalLine = j;
                    break;
                }
            }
            if (verticalLine != -1)
            {
                coordinates = new WonCoordinatesInfo(points);
                return true;
            }
            //ниже проверяем, сть ли ряд по диагонали
            var diagonalPoints = new List<Point>();
            var hasDiagonal = false;
            //Диагональ ВПРАВО ВНИЗ
            //проверим, есть ли диагональ ВПРАВО ВНИЗ для каждой ячейки в первом столбце
            for (int i = 0; i < n; i++)
            {
                if (CheckDiagonal(field, playerMark, new Point(i, 0), true, out diagonalPoints))
                {
                    hasDiagonal = true;
                    break;
                }
            }
            //если не нашли диагонали, то 
            if (!hasDiagonal)
            {
                //проверим, есть ли диагональ ВПРАВО ВНИЗ для каждой ячейки в первой строке
                for (int j = 0; j < m; j++)
                {
                    if (CheckDiagonal(field, playerMark, new Point(0, j), true, out diagonalPoints))
                    {
                        hasDiagonal = true;
                        break;
                    }
                }
            }
            if (!hasDiagonal)
            {
                //Диагональ ВЛЕВО ВНИЗ
                //проверим, есть ли диагональ ВЛЕВО ВНИЗ для каждой ячейки в последнем столбце
                for (int i = 0; i < n; i++)
                {
                    if (CheckDiagonal(field, playerMark, new Point(i, m - 1), false, out diagonalPoints))
                    {
                        hasDiagonal = true;
                        break;
                    }
                }
            }
            //если не нашли диагонали, то 
            if (!hasDiagonal)
            {
                //проверим, есть ли диагональ ВЛЕВО ВНИЗ для каждой ячейки в первой строке
                for (int j = 0; j < m; j++)
                {
                    if (CheckDiagonal(field, playerMark, new Point(0, j), false, out diagonalPoints))
                    {
                        hasDiagonal = true;
                        break;
                    }
                }
            }

            coordinates = new WonCoordinatesInfo(diagonalPoints);
            return hasDiagonal;
        }
        /// <summary>
        /// Проверяет, есть ли по диагонали ряд из 4х ячеек одного игрока
        /// </summary>
        /// <param name="field">Игровое поле</param>
        /// <param name="mark">Метка игрока, для которого проверяется есть ли ряд</param>
        /// <param name="start">Начальная точка</param>
        /// <param name="toRight">Направление поиска (диагональ ВПРАВО вниз или ВЛЕВО вниз)</param>
        /// <param name="points">Список ячеек диагонали, на которых есть ряд из 4х фишек игрока</param>
        /// <returns></returns>
        private static bool CheckDiagonal(string[,] field, string mark, Point start, bool toRight, out List<Point> points)
        {
            //размерность поля
            var n = field.GetLength(0);
            var m = field.GetLength(1);
            //начальная ячейка, от которой будет производится поиск
            var startX = start.X;
            var startY = start.Y;
            //счетчик для количества подряд идущих ячеек с фишкой mark
            var itemsInDiagonal = 0;

            var yDirection = toRight ? 1 : -1;//направление поиска диагонали (влево или вправо)

            //пока подряд идущих фишек выбранного игрока mark на диагонали меньше 4х и не все ячейки на диагонали просмотрены
            while (itemsInDiagonal < 4 && startX < n && (toRight && startY < m || !toRight && startY >= 0))
            {
                //есть ли фишка игрока на рассматриваемой ячейке
                if (field[startX, startY] == mark)
                {
                    //если есть увеличиваем счетчик
                    itemsInDiagonal++;
                }
                else
                {
                    //если фишки нет, то обнуляем счетчик
                    itemsInDiagonal = 0;
                }
                //берем координаты следующей ячейки
                startX++;
                startY += yDirection;
            }
            
            //вычисляем список ячеек, образующих ряд из 4х фишек, если такой был найден выше
            points = new List<Point>();
            if (itemsInDiagonal == 4)
            {
                points = GetDiagonalSamePoints(field, mark, start, toRight);
            }
            //возвращаем true, если найден ряд ячеек из 4х фишек
            return itemsInDiagonal == 4;
        }
        /// <summary>
        /// вычисляет список ячеек, образующих ряд из 4х фишек, для диагонали
        /// </summary>
        /// <param name="field">Игровое поле</param>
        /// <param name="mark">Метка игрока, для которого проверяется есть ли ряд</param>
        /// <param name="start">Начальная точка</param>
        /// <param name="toRight">Направление поиска (диагональ ВПРАВО вниз или ВЛЕВО вниз)</param>
        /// <returns></returns>
        private static List<Point> GetDiagonalSamePoints(string[,] field, string mark, Point start, bool toRight = true)
        {
            //список ячеек
            var points = new List<Point>();
            //размерность игрового поля
            var n = field.GetLength(0);
            var m = field.GetLength(1);

            //начальная ячейка, от которой будет производится поиск
            var startX = start.X;
            var startY = start.Y;
            //счетчик для количества подряд идущих ячеек с фишкой mark
            var itemsInDiagonal = 0;

            var yDirection = toRight ? 1 : -1;//направление поиска диагонали (влево или вправо)

            //пока подряд идущих фишек выбранного игрока mark на диагонали меньше 4х и не все ячейки на диагонали просмотрены
            while (itemsInDiagonal < 4 && startX < n && (toRight && startY < m || !toRight && startY >= 0))
            {
                //есть ли фишка игрока на рассматриваемой ячейке
                if (field[startX, startY] == mark)
                {
                    //если есть увеличиваем счетчик
                    itemsInDiagonal++;
                    //сохраняем эту ячейку
                    points.Add(new Point(startX, startY));
                }
                else
                {
                    //очищаем список ячеек
                    points = new List<Point>();
                    //если фишки нет, то обнуляем счетчик
                    itemsInDiagonal = 0;
                }
                //берем координаты следующей ячейки
                startX++;
                startY += yDirection;
            }
            //возвращаем список подряд идущих ячеек с фишкой mark
            return points;
        }
    }
}
