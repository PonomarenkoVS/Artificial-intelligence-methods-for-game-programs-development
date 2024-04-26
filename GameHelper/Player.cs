using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameHelper
{
    /// <summary>
    /// Игрок
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Имя игрока
        /// </summary>
        public string Name;

        /// <summary>
        /// Фишка игрока (фигура)
        /// </summary>
        public string Mark;

        /// <summary>
        ///Цвет фишка игроки (фигуры). По умолчанию белый
        /// </summary>
        public ConsoleColor Color = ConsoleColor.White;

        /// <summary>
        /// Тип игрока
        /// </summary>
        public readonly PlayerType Type;

        /// <summary>
        /// Коснтруктор класса Игрок
        /// </summary>
        /// <param name="name">Имя</param>
        /// <param name="chip">Фишка</param>
        /// <param name="type">Тип игрока</param>
        public Player(string name, string chip, PlayerType type)
        {
            Name = name;
            Mark = chip;
            Type = type;
        }

        /// <summary>
        /// Строковое представление информации об игроке
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Name} ({Type}-{Mark})";
        }
    }
}
