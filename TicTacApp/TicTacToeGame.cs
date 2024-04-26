using GameHelper;
using System.Drawing;

namespace TicTacApp
{
    /// <summary>
    /// Класс с методами для игры в крестики-нолики
    /// </summary>
    public class TicTacToeGame
    {
        /// <summary>
        /// Провеяет, выиграл ли игрок с  меткой playerMark на текущец доске field
        /// </summary>
        /// <param name="field">Игровое поле</param>
        /// <param name="playerMark">Метка игрока</param>
        /// <param name="coordinates">Выигрышные координаты</param>
        /// <returns></returns>
        public bool CheckWinMethod(string[,] field, string playerMark, out WonCoordinatesInfo coordinates)
        {
            return CheckWonLine(field, playerMark, out coordinates);
        }

        /// <summary>
        /// Правила выигрыша для игры крестики-нолики
        /// </summary>
        /// <param name="field">Игровое поле</param>
        /// <param name="playerMark">Метка игрока</param>
        /// <param name="wonCoordinates">Выигрышные координаты</param>
        /// <returns></returns>
        private bool CheckWonLine(string[,] field, string playerMark, out WonCoordinatesInfo wonCoordinates)
        {
            //размерность игрового поля
            var n = field.GetLength(0);
            var m = field.GetLength(1);
            //проверяем, есть ли выигрыш на горизонтальных линиях
            var horizontalLine = -1;
            for (int i = 0; i < n; i++)
            {
                var hasLine = true;
                for (int j = 0; j < m; j++)
                {
                    if (field[i, j] != playerMark)
                    {
                        hasLine = false;
                        break;
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
                wonCoordinates = new WonCoordinatesInfo(new Point(horizontalLine, 0), new Point(horizontalLine, m - 1), LineType.Horizontal);
                return true;
            }

            //проверяем, есть ли выигрыш на вертикальных линиях
            var verticalLine = -1;
            for (int j = 0; j < m; j++)
            {
                var hasLine = true;
                for (int i = 0; i < n; i++)
                {
                    if (field[i, j] != playerMark)
                    {
                        hasLine = false;
                        break;
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
                wonCoordinates = new WonCoordinatesInfo(new Point(0, verticalLine), new Point(n - 1, verticalLine), LineType.Vertical);
                return true;
            }

            //проверяем, есть ли выигрыш на диагоналях
            var hasDiagonal = true;
            for (int i = 0; i < n; i++)
            {
                if (field[i, i] != playerMark)
                {
                    hasDiagonal = false;
                }
            }
            if (hasDiagonal)
            {
                wonCoordinates = new WonCoordinatesInfo(new Point(0, 0), new Point(n - 1, m - 1), LineType.LeftDiagonal);
                return true;
            }

            hasDiagonal = true;
            for (int i = 0; i < n; i++)
            {
                if (field[i, m - i - 1] != playerMark)
                {
                    hasDiagonal = false;
                }
            }
            if (hasDiagonal)
            {
                wonCoordinates = new WonCoordinatesInfo(new Point(0, m - 1), new Point(n - 1, 0), LineType.RightDiagonal);
                return true;
            }

            //если дошли до этого кода, значит выигрышной комбинации не найдено
            wonCoordinates = new WonCoordinatesInfo();
            return false;
        }
    }
}
