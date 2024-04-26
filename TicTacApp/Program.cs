using GameHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TicTacApp
{
    class Program
    {
        static void Main(string[] args)
        {
            TicTacToeGame tictacgame = new TicTacToeGame();

            var exit = false;

            //пока пользователь не выбрал выход из игры
            while (!exit)
            {
                //инициализируем новую игру
                var game = new Game(3, 3);
                //задаем свои символы для фишек игрока-человека и игрока-компьютера
                PlayersSettings.PlayerHuman.Mark = "*";
                PlayersSettings.PlayerHuman.Color = ConsoleColor.Yellow;
                PlayersSettings.PlayerComputer.Mark = "0";
                // var game = new Game("X O X\r. O .\rO X X");
                //var game = new Game("X X O\r. O .\r. O X");
                //var game = new Game("O O X\r. X .\r. . .");
                Game.MaxDepth =3;
                game.GameTitle = "ИГРА КРЕСТИКИ-НОЛИКИ";

                //задаем правила выигрыша для игры крестики-нолики
                game.SetRules(tictacgame.CheckWinMethod);
                game.CalculateScoreUserDefined += CalculateGameScore;

                //текущий игрок
                var currentPlayer = PlayersSettings.PlayerComputer;

                //пока есть ходы и нет победителя
                while (true)
                {
                    //очищаем консоль от предыдущих данных
                    ///Console.Clear();
                    //рисуем состояние игрового поля
                    game.DrawField();

                    Console.WriteLine($"Ход игрока {currentPlayer}");

                    //тут будем хранить введенные пользователем координаты
                    var row = -1;
                    var column = -1;

                    //если ход пользователя, то просим его ввести координаты
                    if (currentPlayer.Type == PlayerType.Human)
                    {
                        do
                        {
                            Console.WriteLine($"введите адрес, куда хотите поставить метку {currentPlayer.Mark}, номер строки и номер столбца через пробел:");
                            var address = Console.ReadLine();//считываем ввод пользователя
                            var parts = address.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                            row = Convert.ToInt32(parts[0]) - 1;//-1, так как пользователь вводит числа от 1 до ..., а в массиве индексация идет с 0
                            column = Convert.ToInt32(parts[1]) - 1;
                        } while (!game.Validate(row, column));//проверяем правильность введенных данных
                    }
                    //если ход машины, то вычисляем лучший ход
                    else
                    {
                        Console.Write($"Ход компьютера");
                        //3 раза останавливаем поток программы на 500 милисекунд (Thread.Sleep(500)), чтобы фишка  компьютера появлялась не одновременоо с ходом игрока  
                        for (int i = 0; i < 3; i++)
                        {
                            Thread.Sleep(500);
                            Console.Write(".");
                        }
                        Console.WriteLine();
                        //вычисляем ход для компьютера
                        var point = game.ComputerMove(Algorithm.AlphaBetta);
                        row = point.X;
                        column = point.Y;
                        //выводим в консоль разработчика адрес этого хода
                        Debug.WriteLine($"{row + 1}, {column + 1}");
                    }

                    //добавляем текущий ход на доску
                    game.AddMark(currentPlayer.Mark, row, column);

                    //проверяем, есть ли победитель в текущем ходу
                    var winCoordinates = new WonCoordinatesInfo();//координаты
                    if (game.CheckPlayerIsWin(currentPlayer.Mark, out winCoordinates))
                    {
                        ///Console.Clear();
                        Console.WriteLine($"Игра закончена. Выиграл {currentPlayer}");
                        game.DrawField(winCoordinates);//вызываем метод библиотеки для рисования текущего игрового поля с выделенными выигрышными координатами
                        break;
                    }

                    //проверяем, есть ли еще свободные клетки. Если свободных клеток нет, то игра окончена
                    if (!game.HasEmptyCells())
                    {
                        ///Console.Clear();
                        Console.WriteLine($"Игра закончена. Ничья. Ходов больше нет.");
                        game.DrawField();
                        break;//выходим из цикла
                    }

                    //передаем ход следующему игроку
                    currentPlayer = PlayersSettings.GetOpponent(currentPlayer);
                }

                //предлагаем сыграть еще раз
                Console.WriteLine("Сыграть еще раз (+/-)?");
                exit = Console.ReadKey().KeyChar == '-';
            }
        }

        /// <summary>
        /// Оценочная функция игрового поля для игрока
        /// </summary>
        /// <param name="board">Игровое поле</param>
        /// <param name="player">Игрок</param>
        /// <returns></returns>
        private static int CalculateGameScore(string[,] board, Player player)
        {
            //фишка игрока
            var mark = player.Mark;
            //фишка соперника
            var opponentmark = PlayersSettings.GetOpponent(player).Mark;
            //например, в крестиках-ноликах, количество диагоналей, на которых игрок и соперник ещё могут построить линию
            var n = board.GetLength(0);
            var m = board.GetLength(1);

            //смотрим диагональ Слева направо Сверху вниз
            var count1 = 0;
            for (int i = 0; i < n; i++)
            {
                if (board[i, i] == mark || board[i, i] == " ")
                {
                    count1++;
                    continue;
                }

                //если на диагонали лежит хоть одна фишка противника, то на ней уже не выиграть
                if (board[i, i] == opponentmark)
                {
                    count1 = 0;
                    break;
                }
            }

            //смотрим диагонал справа налево сверху вниз
            var count2 = 0;
            var row = 0;
            for (int j = m - 1; j >= 0; j--)
            {
                if (board[row, j] == mark || board[row, j] == " ")
                {
                    count2++;
                    row++;
                    continue;
                }
                //если на диагонали лежит хоть одна фишка противника, то на ней уже не выиграть, 
                if (board[row, j] == opponentmark)
                {
                    //поэтому обнуляем все, что до этого посчитали
                    count2 = 0;
                    break;
                }
            }

            var diagonalscount = count1 + count2;//количество фишек текущего игрока на диагонали и пустых клеток

            //даем доп баллы, если этот игрок имеет фишку по центру игрового поля (откуда можно много сделать)
            var centerX = n / 2;
            var centerY = m / 2;
            if (board[centerX, centerY] == mark)
            {
                diagonalscount += 20;//
            }

            return diagonalscount;
        }
    }
}