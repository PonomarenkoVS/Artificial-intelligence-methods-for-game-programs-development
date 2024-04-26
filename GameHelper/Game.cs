using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace GameHelper
{
    
    public class Node
    {

        public Game GameRules;
        public string [,] CurrField;
        public int NumGames;        
        public int WonGames;        
        public List<Point> PossMoves;        
        public List<Node> NextMoves;
        public Player CurrPlayer;
        public bool CurrPlayerWonFlag;

        public int GamesPlayedNow;
        public int GamesWonNow;
        public int GamesOpponentWon;


        public Node(Game game, string[,] currentBoard, Player currentPlayer) 
        {
            GameRules = game;
            CurrField = (string[,])currentBoard.Clone();
            NumGames = 0; 
            WonGames = 0;
            PossMoves = GameRules.GetPossibleMoves(currentBoard, currentPlayer.Mark);
            CurrPlayer = new Player(currentPlayer.Name, currentPlayer.Mark, currentPlayer.Type);
            NextMoves = new List<Node>();
            CurrPlayerWonFlag = false;

            GamesPlayedNow = 0;
            GamesWonNow = 0;
        }

        public string Simulation(string[,] field, string playerMark)
        {
            var currField = (string[,])field.Clone();
            List<Point> nextMoves = GameRules.GetPossibleMoves(field, playerMark);
            if (nextMoves.Count == 0) 
            {
                return "TIE";
            }
            Random rnd = new Random();
            var num = rnd.Next(nextMoves.Count);
            currField[nextMoves[num].X, nextMoves[num].Y] = playerMark;
            if (GameRules.OnMakeStep != null)
                GameRules.OnMakeStep(currField, nextMoves[num], playerMark);
            var line = new WonCoordinatesInfo();
            var isWonMove = GameRules.CheckPlayerIsWin(currField, playerMark, out line);
            if (isWonMove)
            {
                return playerMark;
            }
            return Simulation(currField, PlayersSettings.GetOpponent(PlayersSettings.GetPlayer(playerMark)).Mark);

        }

        public void GetChildStats(Node child)
        {
            GamesPlayedNow = child.GamesPlayedNow;
            GamesWonNow = child.GamesOpponentWon;
            GamesOpponentWon = child.GamesWonNow;
            child.GamesPlayedNow = 0;
            child.GamesOpponentWon = 0;
            child.GamesWonNow = 0;

            NumGames += GamesPlayedNow;
            WonGames += GamesWonNow;
        }
        
        public void Step()
        {

            ///Console.WriteLine("STEP");

            var line = new WonCoordinatesInfo();
            var isWonMove = GameRules.CheckPlayerIsWin(CurrField, CurrPlayer.Mark, out line);
            if (isWonMove)
            {
                GamesPlayedNow = 2;
                NumGames += 2;
                GamesWonNow = 2;
                WonGames += 2;
                return;
            }

            else if (PossMoves.Count == 0)
            {
                GamesPlayedNow = 2;
                NumGames += 2;
                GamesWonNow = 1;
                GamesOpponentWon = 1;
                WonGames += 1;
                return;
            }

            else if (NumGames == 0)
            {

                ///Console.WriteLine("SIMULATION");

                GamesPlayedNow = 2;
                NumGames += 2;
                var result = Simulation(CurrField, PlayersSettings.GetOpponent(PlayersSettings.GetPlayer(CurrPlayer.Mark)).Mark);
                if (result == CurrPlayer.Mark)
                {
                    GamesWonNow = 2;
                    WonGames += 2;
                }
                else if (result == "TIE")
                {
                    GamesWonNow = 1;
                    GamesOpponentWon = 1;
                    WonGames += 1;
                }
                else
                {
                    GamesOpponentWon = 2;
                }
                return;
            }

            else
            {
                if (NextMoves.Count == 0)
                {
                    ///Console.WriteLine("GENERATED FIRST NODE");

                    var currField = (string[,])CurrField.Clone();
                    NextMoves.Add(new Node(GameRules, currField, PlayersSettings.GetOpponent(PlayersSettings.GetPlayer(CurrPlayer.Mark))));
                    NextMoves[0].Step();
                    GetChildStats(NextMoves[0]);
                }
                else
                {
                    int allGames = 0;
                    foreach (var childNode in NextMoves)
                    {
                        allGames += childNode.NumGames;
                    }
                    int bestMove = 0;
                    double bestScore = 0;
                    for (int i = 0; i < NextMoves.Count; i++)
                    {
                        var score = ((double)NextMoves[i].WonGames / (double)NextMoves[i].NumGames) + Math.Sqrt(2) * Math.Sqrt(Math.Log(allGames) / (double)NextMoves[i].NumGames);
                        if (score > bestScore)
                        {
                            bestScore = score;
                            bestMove = i;
                        }
                    }
                    if ((NextMoves.Count < PossMoves.Count) & (bestScore < Math.Sqrt(2) * Math.Sqrt(Math.Log(allGames))))
                    {

                        ///Console.WriteLine("GENERATED NEW NODE");

                        var currField = (string[,])CurrField.Clone();
                        currField[PossMoves[NextMoves.Count].X, PossMoves[NextMoves.Count].Y] = PlayersSettings.GetOpponent(PlayersSettings.GetPlayer(CurrPlayer.Mark)).Mark;
                        NextMoves.Add(new Node(GameRules, currField, PlayersSettings.GetOpponent(PlayersSettings.GetPlayer(CurrPlayer.Mark))));
                        NextMoves[NextMoves.Count - 1].Step();
                        GetChildStats(NextMoves[NextMoves.Count - 1]);
                        return;
                    }
                    else
                    {

                        ///Console.WriteLine("ADVANCED TO EXPLORED NODE");

                        NextMoves[bestMove].Step();
                        GetChildStats(NextMoves[bestMove]);
                        return;
                    }
                }
            }

        }

        public Point ChooseMove()
        {
            int bestMove = 0;
            int bestScore = 0;
            for (int i = 0; i < NextMoves.Count; i++)
            {
                Console.WriteLine($"{NextMoves.Count}");
                Console.Write("Оценки ходов:");
                Console.Write($"{NextMoves[i].WonGames}, {NextMoves[i].NumGames}, {NextMoves[i].WonGames / NextMoves[i].NumGames}");
                Console.WriteLine();
                if (NextMoves[i].NumGames > bestScore)
                {
                    bestScore = NextMoves[i].NumGames;
                    bestMove = i;
                }
            }
            return PossMoves[bestMove];
        }

    }
    
    
    /// <summary>
    /// Игра двух игроков на прямоугльном поле
    /// </summary>
    
    public class Game
    {
        /// <summary>
        /// Размерность игрового поля по вертикали (количество строк)
        /// </summary>
        public int N;

        /// <summary>
        /// Размерность игрового поля по горизонтали (количество столбцов)
        /// </summary>
        public int M;

        /// <summary>
        /// Игровое поле Прямоугольная доска (представление в виде таблицы)
        /// </summary>
        public string[,] Field;

        /// <summary>
        /// Глубина поиска при подсчете игровых позиций, по умолчанию не ограничена
        /// </summary>
        public static int MaxDepth = 3; ///int.MaxValue;//глубина поиска не ограничена

        /// <summary>
        /// Наименование игры (для вывода в консоль)
        /// </summary>
        public string GameTitle = "Заголовок игры";

        /// <summary>
        /// Конструктор игры. Инициализирует новый экземпляр игры на прямоугольной доске [n,m] для двух игроков
        /// </summary>
        /// <param name="n">количество строк на игровом поле (доске)</param>
        /// <param name="m">количество столбцов на игровом поле (доске)</param>
        public Game(int n, int m)
        {
            N = n;
            M = m;

            InitializeField();
        }

        /// <summary>
        /// Конструктор игры. Инициализирует новый экземпляр игры на прямоугольной доске для двух игроков
        /// </summary>
        /// <param name="data">Игровое поле в текстовом виде. В формате "O X .\r. . .\r. O .", где знак "." означает пустую ячейку игрового поля, "X" - ячейки, которые занял игрок X, "O" - ячейки, которые занял игрок O</param>
        public Game(string data)
        {
            var lines = data.Split(new string[1] { "\r" }, StringSplitOptions.RemoveEmptyEntries);
            N = lines.Length;
            M = lines[0].Split(new string[1] { " " }, StringSplitOptions.RemoveEmptyEntries).Length;
            Field = new string[N, M];

            for (int i = 0; i < lines.Length; i++)
            {
                var parts = lines[i].Split(new string[1] { " " }, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < parts.Length; j++)
                {
                    Field[i, j] = parts[j] == "." ? " " : parts[j];
                }
            }
        }

        /// <summary>
        /// Инициализирует пустое игровое поле
        /// </summary>
        private void InitializeField()
        {
            Field = new string[N, M];
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    Field[i, j] = " ";
                }
            }
        }

        /// <summary>
        /// Очищает игровое поле
        /// </summary>
        public void Clear()
        {
            InitializeField();
        }

        /// <summary>
        /// Проверяет правильность заданных координат (на возможность положить фишку)
        /// </summary>
        /// <param name="row">номер строки</param>
        /// <param name="column">номер столбца</param>
        /// <returns></returns>
        public bool Validate(int row, int column)
        {
            //можно положить фишку только в ячейку, чьи координаты входят в размерность игрового поля
            return row >= 0 && column >= 0 && row <= N && column <= M;
        }

        /// <summary>
        /// Ход игрока с маркером mark в позицию игрового поля [row,column]
        /// </summary>
        /// <param name="mark">Метка игрока</param>
        /// <param name="row">Индекс строки игрового поля</param>
        /// <param name="column">Индекс столбца игрового поля</param>
        public void AddMark(string mark, int row, int column)
        {
            //проверка правильности хода
            if (!Validate(row, column))
            {
                throw new ArgumentException("Один из введенных агрументов [row,column] имеет неверное значение");
            }

            //ставим метку игрока в выбранную позицию на игровое поле
            Field[row, column] = mark;

            //вызываем события, которые должны быть выполнены после хода игрока
            if (OnMakeStep != null)
            {
                OnMakeStep(this.Field, new Point(row, column), mark);
            }
        }

        /// <summary>
        /// Проверяет, есть ли пустые клетки на игровом поле текущей игры
        /// </summary>
        /// <returns></returns>
        public bool HasEmptyCells()
        {
            return HasEmptyCells(Field);
        }

        /// <summary>
        /// Проверяет, есть ли пустые клетки на заданном игровом поле field
        /// </summary>
        /// <param name="field">Игровое поле</param>
        /// <returns></returns>
        private static bool HasEmptyCells(string[,] field)
        {
            var n = field.GetLength(0);
            var m = field.GetLength(1);
            var result = false;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    if (field[i, j] == " ")
                        return true;
                }
            }
            return result;
        }

        /// <summary>
        /// Возвращает список пустых клеток игрового поля (их координат)
        /// </summary>
        /// <param name="field">Игровое поле</param>
        /// <returns>Список пустых клеток</returns>
        private static List<Point> GetEmptyCells(string[,] field)
        {
            var n = field.GetLength(0);
            var m = field.GetLength(1);
            var result = new List<Point>();

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    if (field[i, j] == " ")
                        result.Add(new Point(i, j));
                }
            }

            return result;
        }

        /// <summary>
        /// Делегат для передачи метода, заданного пользователем, который возвращает список возможных ходов
        /// </summary>
        /// <param name="field">Игровове поле</param>
        /// <param name="playerMark">Метка игрока</param>
        /// <returns></returns>
        public delegate List<Point> GetPossibleMovesDelegate(string[,] field, string playerMark);
        /// <summary>
        /// Метод для поиска возможных ходов
        /// </summary>
        public GetPossibleMovesDelegate GetPossibleMovesMethod;

        /// <summary>
        /// Возвращает список возможных ходов (клеток), куда еще может походить игрок
        /// </summary>
        /// <param name="field">Игровое поле</param>
        /// <param name="playerMark">Метка игрока</param>
        /// <returns></returns>
        public List<Point> GetPossibleMoves(string[,] field, string playerMark)
        {
            //если пользователь не задал свой метод для определения возможных ходов,
            if (GetPossibleMovesMethod == null)
            {
                //то возвращаем список всех пустых клеток на игровом поле
                return GetEmptyCells(field);
            }

            //иначе вызываем пользовательский метод для определения возможных ходов
            return GetPossibleMovesMethod(field, playerMark);
        }

        /// <summary>
        /// Делегат для метода проверки выигрыша игрока
        /// </summary>
        /// <param name="field">Игровое поле</param>
        /// <param name="playerMark">Метка игрока</param>
        /// <param name="wonCoordinates">Координаты с выигрышной комбинацией на поле</param>
        /// <returns></returns>
        public delegate bool CheckPlayerIsWinDelegate(string[,] field, string playerMark, out WonCoordinatesInfo wonCoordinates);
        /// <summary>
        /// Правила выигрыша игрока в игре
        /// </summary>
        private CheckPlayerIsWinDelegate CheckPlayerIsWinUserDefined;

        /// <summary>
        /// Делегат для подсчета игровых позиций
        /// </summary>
        /// <param name="currentBoard">Игрвоое поле</param>
        /// <param name="player">Игрок</param>
        /// <returns></returns>
        public delegate int CalculateScoreDelegate(string[,] currentBoard, Player player);
        /// <summary>
        /// Подсчет игровых позиций
        /// </summary>
        public CalculateScoreDelegate CalculateScoreUserDefined;

        /// <summary>
        /// Делегат для задания пользователем действий, которые должны быть выполнены сразу после хода игрока
        /// </summary>
        /// <param name="field">Игровое поле</param>
        /// <param name="step">Последний ход</param>
        /// <param name="playerMark">Игрок, сделавший последний ход</param>
        public delegate void OnMakeStepDelegate(string[,] field, Point step, string playerMark);
        /// <summary>
        /// Автоматические вещи, которые после хода происходят - падение фишек вниз или др
        /// </summary>
        public OnMakeStepDelegate OnMakeStep;

        /// <summary>
        /// Задает правила игры (так как библиотека не знает для какой конкретно игры ее будут использовать)
        /// </summary>
        /// <param name="winRules">Условия выигрыша</param>
        /// <param name="getPossibleMoves">Функция, определяющая список возможных ходов (необязательное)</param>
        public void SetRules(CheckPlayerIsWinDelegate winRules, GetPossibleMovesDelegate getPossibleMoves = null)
        {
            CheckPlayerIsWinUserDefined += winRules;

            if (getPossibleMoves != null)
            {
                GetPossibleMovesMethod += getPossibleMoves;
            }
        }

        /// <summary>
        /// Проверяет, выиграл ли игрок playerMark на заданном игровом поле
        /// </summary>
        /// <param name="field">Игровое поле</param>
        /// <param name="playerMark">Метка текущего игрока, для которого происходит подсчет</param>
        /// <param name="wonCoordinates">Выигрышные позиции</param>
        /// <returns></returns>
        public bool CheckPlayerIsWin(string[,] field, string playerMark, out WonCoordinatesInfo wonCoordinates)
        {
            return CheckPlayerIsWinUserDefined(field, playerMark, out wonCoordinates);
        }

        /// <summary>
        /// Проверяет, выиграл ли игрок playerMark на поле текущей игры
        /// </summary>
        /// <param name="playerMark">Метка текущего игрока, для которого происходит подсчет</param>
        /// <param name="wonCoordinates">Выигрышные позиции</param>
        /// <returns></returns>
        public bool CheckPlayerIsWin(string playerMark, out WonCoordinatesInfo wonCoordinates)
        {
            return CheckPlayerIsWinUserDefined(this.Field, playerMark, out wonCoordinates);
        }

        /// <summary>
        /// Выводит игровое поле в консоль (со всеми метками всех игроков)
        /// </summary>
        public void DrawField(WonCoordinatesInfo coordinatesInfo = null)
        {
            var defaultForeColor = ConsoleColor.White;
            Console.WriteLine(GameTitle);
            Console.Write($" |\t");

            for (int j = 0; j < M; j++)
            {
                Console.Write($"{j + 1}\t|\t");
            }

            Console.WriteLine();
            Console.WriteLine("-" + new string('-', 16 * M));

            for (int i = 0; i < N; i++)
            {
                Console.Write($"{i + 1}|\t");
                for (int j = 0; j < M; j++)
                {
                    if (coordinatesInfo != null && coordinatesInfo.Points.Any(x => x.X == i && x.Y == j))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(Field[i, j]);
                        Console.ForegroundColor = defaultForeColor;
                    }
                    else if (Field[i, j] != " ")
                    {
                        var player = PlayersSettings.GetPlayer(Field[i, j]);
                        Console.ForegroundColor = player.Color;
                        Console.Write(Field[i, j]);
                        Console.ForegroundColor = defaultForeColor;
                    }
                    else
                    {
                        Console.Write(Field[i, j]);
                    }
                    Console.Write("\t|\t");
                }
                Console.WriteLine();
                Console.WriteLine("-" + new string('-', 16 * M));
            }
        }

        /// <summary>
        /// Выводит заданное игровое поле в консоль
        /// </summary>
        /// <param name="data">Игровое поле</param>
        /// <returns></returns>
        public static string DrawField(string[,] data)
        {
            var M = data.GetLength(1);
            var N = data.GetLength(0);

            var result = new StringBuilder();
            result.Append($" |\t");

            for (int j = 0; j < M; j++)
            {
                result.Append($"{j + 1}\t|\t");
            }

            result.AppendLine();
            result.AppendLine("------------------------------------------------");


            for (int i = 0; i < N; i++)
            {
                result.Append($"{i + 1}|\t");
                for (int j = 0; j < M; j++)
                {
                    result.Append(data[i, j] + "\t|\t");
                }
                result.AppendLine();
                result.AppendLine("------------------------------------------------");
            }

            return result.ToString();
        }

        /// <summary>
        /// Возвращает оптимальный ход, подсчитанный компьютером по заданному алгоритму
        /// </summary>
        /// <param name="algorithm">Алгоритм подсчета. По умолчанию  Algorithm.AlphaBetta</param>
        /// <returns>Оптимальный ход</returns>
        public Point ComputerMove(Algorithm algorithm = Algorithm.AlphaBetta)
        {
            var result = new Point();
            var newBoard = (string[,])Field.Clone();
            //запускаем таймер для замера времени поиска оптимального хода
            Stopwatch stopwatch = Stopwatch.StartNew();
            //если выбран алгоритм минимакс с альфа-бета отсечением
            if (algorithm == Algorithm.AlphaBetta)
            {
                //то вычисляем лучший ход с помощью минимакса с альфа-бета отсечения
                int alpha = -100;
                int beta = 100;
                var move = FindBestMoveMinMaxAlphaBeta(newBoard, MaxDepth, PlayersSettings.PlayerComputer, alpha, beta);
                Debug.WriteLine("cost=" + move.Item1);
                result = move.Item2;
            }
            //если алгоритм минимакс
            else if (algorithm == Algorithm.MonteCarlo)
            {
                //вычисляем лучший ход с помощью минимакса
                ///var move = FindBestMoveMinMax(newBoard, PlayerType.Computer, Methodology.Minimizing);
                ///Debug.WriteLine("cost=" + move.Item1);
                ///result = move.Item2;

                result = FindBestMoveMonteCarloAlgorithm(newBoard, 800, PlayersSettings.PlayerHuman);

            }
            //останавливаем таймер
            stopwatch.Stop();
            //выводим в консоль разработчика время выполнения поиска оптимального хода
            Debug.WriteLine(TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds));
            //возвращаем лучший ход
            return result;
        }

        /// <summary>
        /// Возвращает оптимальный ход и его оценку, подсчитанный алгоритмом Минимакс
        /// </summary>
        /// <param name="currentBoard">игровое поле</param>
        /// <param name="player">текущий игрок</param>
        /// <param name="methodology">стратегия игрока</param>
        /// <param name="depth">глубина поиска</param>
        /// <returns></returns>
        public Tuple<int, Point> FindBestMoveMinMax(string[,] currentBoard, PlayerType player, Methodology methodology, int depth = 0)
        {
            //если достигли ограничения на глубину поиска, выходим с 0 стоимостью
            if (depth > MaxDepth)
            {
                return new Tuple<int, Point>(0, new Point());
            }
            //метка текщего игрока
            var currentPlayerMark = player == PlayerType.Computer ? PlayersSettings.PlayerComputer.Mark : PlayersSettings.PlayerHuman.Mark;
            //предыдщий игрок, сделавший ход
            var previousPlayer = player == PlayerType.Computer ? PlayerType.Human : PlayerType.Computer;

            var line = new WonCoordinatesInfo();
            var isWonMove = CheckPlayerIsWin(currentBoard, previousPlayer == PlayerType.Computer ? PlayersSettings.PlayerComputer.Mark : PlayersSettings.PlayerHuman.Mark, out line);
            //если текущий игрок, сделавший текущий ход, выиграл, это Лист дерева
            if (isWonMove)
            {
                return new Tuple<int, Point>(5 * (int)previousPlayer, new Point());
            }
            //если шагов больше нет (НИЧЬЯ), это тоже лист дерева
            if (!HasEmptyCells(currentBoard))
            {
                return new Tuple<int, Point>(2, new Point());
            }

            //возможные ходы (пустые клетки)
            var possibleMoves = GetPossibleMoves(currentBoard, currentPlayerMark);

            var record = methodology == Methodology.Maximizing ? -1000 : 1000;//рекорд текущего шага (в зависимости от стратегии игрока)
            var recordMove = new Point();//переменная для хранения лучшего шага
            //идем по всем возможным ходам
            for (int i = 0; i < possibleMoves.Count; i++)
            {
                var move = possibleMoves[i];
                //так как в голове у компьютера происходит этот ход, то он "в уме" делает новую доску с игрой
                var newBoard = (string[,])currentBoard.Clone();
                newBoard[move.X, move.Y] = currentPlayerMark;//делаем ход
                //если пользователь задал, что надо выполнить какие-либо действия с игровым полем, то выполняем
                if (OnMakeStep != null)
                    OnMakeStep(newBoard, move, currentPlayerMark);
                //оценка хода
                var s = FindBestMoveMinMax(newBoard,
                    player == PlayerType.Computer ? PlayerType.Human : PlayerType.Computer,
                    methodology == Methodology.Maximizing ? Methodology.Minimizing : Methodology.Maximizing,
                    depth + 1);
                //если игрок максимизирующий и оценка текущего хода больше наилучшей, найденной ранее
                //или если игрок минимизирующий и оценка текущего хода меньше наилучшей, найденной ранее
                if (methodology == Methodology.Maximizing && s.Item1 > record
                    || methodology == Methodology.Minimizing && s.Item1 < record)
                {
                    //то запоминаем этот ход и оценку как наилучшие
                    record = s.Item1;
                    recordMove = move;
                }
            }
            //возвращаем наилучший ход и его оценку
            return new Tuple<int, Point>(record, recordMove);
        }


        /// <summary>
        /// Возвращает оптимальный ход и его оценку, подсчитанный алгоритмом Минимакс с Альфа-бета-отсечением
        /// </summary>
        /// <param name="currentBoard">игровое поле</param>
        /// <param name="depth">глубина поиска</param>
        /// <param name="player">текущий игрок</param>
        /// <param name="alpha">начало промежутка</param>
        /// <param name="beta">конец промежутка</param>
        /// <returns></returns>
        public Tuple<int, Point> FindBestMoveMinMaxAlphaBeta(string[,] currentBoard, int depth, Player player, int alpha, int beta)
        {
            //фишка текущего игрока
            var currentPlayerMark = player.Mark;
            //предыдущий игрок
            var previousPlayer = PlayersSettings.GetOpponent(player);

            //переменные для хранения лучшего хода и его оценки
            int record;
            int bestX = -1;
            int bestY = -1;

            //проверяем, а выиграл ли игрок, сделавший только что ход
            var isWonMove = CheckPlayerIsWin(currentBoard, previousPlayer.Mark, out WonCoordinatesInfo line);
            //если он выиграл
            if (isWonMove)
            {
                var winner = previousPlayer;
                //начисляем баллы
                record = winner.Type == PlayerType.Computer ? depth : -depth;// 10 - depth : depth - 10;//depth тут для того, чтобы выигрыш весил больше, если он достигается как можно раньше
                return new Tuple<int, Point>(record, new Point(bestX, bestY));
            }

            //список возможных ходов
            var nextMoves = GetPossibleMoves(currentBoard, currentPlayerMark);
            //если ходов больше нет, то это ничья. Начисляем 2 балла 
            if (nextMoves.Count == 0)
            {
                //возвращаем оценку
                return new Tuple<int, Point>(2, new Point(bestX, bestY));
            }

            //если глубина поиска достигнута
            if (depth == 0)
            {
                //если оценочная процедура не задана пользователем
                if (CalculateScoreUserDefined == null)
                {
                    //то оцениваем как
                    return new Tuple<int, Point>(2, new Point(bestX, bestY));
                }
                //то оцениваем игровые позиции (при помощи процедуры, заданной пользователем)
                var score = -1 * CalculateScoreUserDefined(currentBoard, player);//оцениваем сколько предыдущий игрок за свой ход получил "баллов"
                return new Tuple<int, Point>(score, new Point(bestX, bestY));
            }

            //идем по каждому возможному ходу
            foreach (var move in nextMoves)
            {
                //делаем ход
                var newBoard = (string[,])currentBoard.Clone();
                newBoard[move.X, move.Y] = currentPlayerMark;
                //если пользователь задал, что надо выполнить какие-либо действия с игровым полем, то выполняем
                if (OnMakeStep != null)
                    OnMakeStep(newBoard, move, currentPlayerMark);

                //если текущий игрок Computer
                if (player.Type == PlayerType.Computer)
                {
                    //вычисляем для него оценку (рекурсивно вызываем себя)
                    record = FindBestMoveMinMaxAlphaBeta(newBoard, depth - 1, PlayersSettings.PlayerHuman, alpha, beta).Item1;
                    //обновляем лучшую оценку
                    if (record > alpha)
                    {
                        alpha = record;
                        bestX = move.X;
                        bestY = move.Y;
                    }
                }
                else
                {
                    //вычисляем оценку  (рекурсивно вызываем себя)
                    record = FindBestMoveMinMaxAlphaBeta(newBoard, depth - 1, PlayersSettings.PlayerComputer, alpha, beta).Item1;
                    //обновляем лучшую оценку
                    if (record < beta)
                    {
                        beta = record;
                        bestX = move.X;
                        bestY = move.Y;
                    }
                }
                if (alpha >= beta)
                    break;
            }

            //возвращаем наилучшй ход и его оценку
            return new Tuple<int, Point>((player.Type == PlayerType.Computer) ? alpha : beta, new Point(bestX, bestY));
        }


        public Point FindBestMoveMonteCarloAlgorithm(string[,] currentBoard, int dur, Player player)
        {
            var root = new Node(this, currentBoard, player);
            for (int i = 0; i < dur; i++)
            {
                root.Step();
            }
            return root.ChooseMove();
        }
    }
}
