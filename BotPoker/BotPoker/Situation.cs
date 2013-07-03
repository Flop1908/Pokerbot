using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotPoker
{
    /// <summary>
    /// Class for linking the information in the game and the decisional engine
    /// </summary>
    class Situation
    {

        /// <summary>
        /// The number of the turn we are
        /// </summary>
        public int turnNumber;
        private int _turnNumber { get { return turnNumber; } set { turnNumber = value; } }

        /// <summary>
        /// the number of round we are on a turn
        /// </summary>
        public int roundNumber;
        private int _roundNumber { get { return roundNumber; } set { roundNumber = value; } }

        /// <summary>
        /// the type of the round we are, blind or flop or turn or river
        /// </summary>
        public string typeOfRound;
        private string _typeRound
        {
            get { return typeOfRound; }
            set
            {
                //on ne doit avoir que l'une des valeurs suivantes
                if (!String.IsNullOrWhiteSpace(value))
                    if (value.Equals("blind") ||
                        value.Equals("flop") ||
                        value.Equals("turn") ||
                        value.Equals("river"))
                        typeOfRound = value;
            }
        }

        /// <summary>
        /// List containing the two cards of the player
        /// </summary>
        public List<PokerCard> playerCards;
        private List<PokerCard> _playerCards { get { return playerCards; } set { playerCards = value; } }

        /// <summary>
        /// List containing the community cards of the turn
        /// </summary>
        public List<PokerCard> communityCards;
        private List<PokerCard> _communityCards { get { return communityCards; } set { communityCards = value; } }

        /// <summary>
        /// how much there is in the pot
        /// </summary>
        public int potMoney;
        private int _potMoney { get { return potMoney; } set { potMoney = value; } }

        /// <summary>
        /// how we bet so far
        /// </summary>
        public int bettedMoney;
        private int _bettedMoney { get { return bettedMoney; } set { bettedMoney = value; } }

        /// <summary>
        /// how much we have left for betting
        /// </summary>
        public int moneyLeft;
        private int _moneyLeft { get { return moneyLeft; } set { moneyLeft = value; } }

        /// <summary>
        /// how many player still in the game
        /// </summary>
        public int playerLeft;
        private int _playerLeft { get { return playerLeft; } set { playerLeft = value; } }


    }
}
