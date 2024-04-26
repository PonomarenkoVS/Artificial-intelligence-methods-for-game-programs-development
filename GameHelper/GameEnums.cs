using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameHelper
{
    /// <summary>
    /// Алгоритма поиска оптимального хода
    /// </summary>
    public enum Algorithm
    {
        MonteCarlo,
        MinMax,
        AlphaBetta
    }
    /// <summary>
    /// Тип выигрышной фигуры
    /// </summary>
    public enum LineType
    {
        Vertical,
        Horizontal,
        LeftDiagonal,
        RightDiagonal,
        UserFigure
    }
    /// <summary>
    /// Тип игрока
    /// </summary>
    public enum PlayerType
    {
        Human = 1,//максимизирующий игрок
        Computer = -1//минимизирующий игрок
    }
    /// <summary>
    /// Методология поиска оптимального хода игрока
    /// </summary>
    public enum Methodology
    {
        Maximizing,
        Minimizing
    }
}
