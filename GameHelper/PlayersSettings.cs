using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameHelper
{

    /// <summary>
    /// Настройки игроков
    /// </summary>
    public static class PlayersSettings
    {
        /// <summary>
        /// Игрок 1
        /// </summary>
        public static Player PlayerHuman;

        /// <summary>
        /// Игрок 2
        /// </summary>
        public static Player PlayerComputer;

        /// <summary>
        /// Коснтруктор 
        /// </summary>
        static PlayersSettings()
        {
            PlayerHuman = new Player("HUMAN", "X", PlayerType.Human);
            PlayerComputer = new Player("COMPUTER", "O", PlayerType.Computer);
        }

        /// <summary>
        /// Возвращает соперника для игрока player
        /// </summary>
        /// <param name="player">Игрок</param>
        /// <returns></returns>
        public static Player GetOpponent(Player player)
        {
            return player == PlayerHuman ? PlayerComputer : PlayerHuman;
        }

        /// <summary>
        /// Возвращает игрока по его фишке chip
        /// </summary>
        /// <param name="chip">Фишка</param>
        /// <returns></returns>
        public static Player GetPlayer(string chip)
        {
            if (PlayerHuman.Mark == chip)
                return PlayerHuman;
            if (PlayerComputer.Mark == chip)
                return PlayerComputer;

            return null;
        }
    }
}
