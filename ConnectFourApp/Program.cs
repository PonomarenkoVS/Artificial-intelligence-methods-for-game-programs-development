using GameHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConnectFourApp
{
    class Program
    {
        //здесь приведен код для игры 4 в ряд  с использованием универсальной библиотеки
        static void Main(string[] args)
        {
            //инициализируем новую игру
            //var game = new Game("O O . . .\rX O . . .\rO X . . .\rO X X . .\rO X X X O");
            var game = new Game(5, 5);
            game.GameTitle = "4 В РЯД";
            Game.MaxDepth = 20;//ограничиваем глубину поиска
            //задаем свои символы для фишек игрока-человека и игрока-компьютера
            PlayersSettings.PlayerHuman.Mark = "*";
            PlayersSettings.PlayerHuman.Color = ConsoleColor.Yellow;
            PlayersSettings.PlayerComputer.Mark = "0";
            PlayersSettings.PlayerComputer.Color = ConsoleColor.Blue;

            //задаем правила для игры 4 в ряд: условия выигрыша,
            //передаем функцию, которая определяет возможные ходы
            game.SetRules(winRules: ConnectFourGame.CheckWinMethod, getPossibleMoves: ConnectFourGame.GetPossibleMoves);
            //подписываемся на событие после хода игрока - будем выполнять функцию "падения" фишки вниз после хода игрока
            game.OnMakeStep += ConnectFourGame.DropChipDown;

            var exit = false;

            //пока пользователь не выбрал выход из игры
            while (!exit)
            {
                //очищаем доску от предыдущей игры
                game.Clear();

                //текущий игрок
                var currentPlayer = PlayersSettings.PlayerComputer;

                //пока есть ходы и нет победителя
                while (true)
                {
                    //очищаем консоль от предыдущих данных
                    Console.Clear();
                    //рисуем состояние игрового поля
                    game.DrawField();

                    Console.WriteLine($"Ход игрока {currentPlayer}");

                    var row = 0;//всегда для игры 4 в ряд
                    var column = -1;
                    //если текущий ход пользователя, то просим его ввести координаты
                    if (currentPlayer.Type == PlayerType.Human)
                    {
                        do
                        {
                            Console.WriteLine($"введите номер столбца, куда хотите поставить фишку {currentPlayer.Mark}:");
                            var address = Console.ReadLine();//считываем ввод пользователя
                            column = Convert.ToInt32(address) - 1;
                        } while (!game.Validate(0, column));//проверяем правильность введенных данных
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
                        var point = game.ComputerMove();
                        row = point.X;
                        column = point.Y;
                        //выводим в консоль разработчика адрес этого хода
                        Debug.WriteLine($"{row + 1}, {column + 1}");
                    }

                    //добавляем текущий ход на доску
                    game.AddMark(currentPlayer.Mark, row, column);

                    //проверяем, есть ли победитель в текущем ходу
                    var winCoordinates = new WonCoordinatesInfo();
                    if (game.CheckPlayerIsWin(currentPlayer.Mark, out winCoordinates))
                    {
                        Console.Clear();
                        Console.WriteLine($"Игра закончена. Выиграл {currentPlayer}");
                        game.DrawField(winCoordinates);//выводим игровое поле с выделенной выигрышной линией
                        break;//выходим из цикла while (true)
                    }

                    //проверяем, есть ли еще свободные клетки
                    if (!game.HasEmptyCells())
                    {
                        Console.Clear();
                        Console.WriteLine($"Игра закончена. Ничья. Ходов больше нет.");
                        game.DrawField();
                        break;//выходим из цикла while (true)
                    }

                    //передаем ход следующему игроку
                    currentPlayer = PlayersSettings.GetOpponent(currentPlayer);
                }

                //предлагаем сыграть еще раз
                Console.WriteLine("Сыграть еще раз (+/-)?");
                exit = Console.ReadKey().KeyChar == '-';
            }
        }
    }
}
