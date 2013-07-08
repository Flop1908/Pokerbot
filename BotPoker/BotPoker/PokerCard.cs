using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BotPoker
{
    /// <summary>
    /// Class for the cards
    /// </summary>
    class PokerCard
    {
        /// <summary>
        /// Value of the card from 1 to 13
        /// </summary>
        public int cardValue;
        private int _cardValue { get { return cardValue; } set { cardValue = value; } }

        /// <summary>
        /// Enumeration for the four types of a card
        /// </summary>
        public enum SuitType
        {
            Clubs, Spades, Hearts, Diamonds
        }

        /// <summary>
        /// the type of the card using SuitType
        /// </summary>
        public SuitType _Suit { get; private set; }

        /// <summary>
        /// constructor with two param
        /// </summary>
        /// <param name="value">the value of the card from 1 to 13</param>
        /// <param name="suit">the type of the card using SuitType</param>
        public PokerCard(int value, SuitType suit)
        {
            _Suit = suit;
            _cardValue = value;
        }

        /// <summary>
        /// constructor with a string with the value first and the type in last (Ex:11H, or jh)
        /// </summary>
        /// <param name="input"></param>
        public PokerCard(String input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentNullException();
            if (input.Length < 2 || input.Length > 3)
                throw new ArgumentException();
            switch (input[input.Length - 1])
            {
                case 'H':
                case 'h':
                case '♥':
                    _Suit = SuitType.Hearts;
                    break;
                case 'D':
                case 'd':
                case '♦':
                    _Suit = SuitType.Diamonds;
                    break;
                case 'S':
                case 's':
                case '♠':
                    _Suit = SuitType.Spades;
                    break;
                case 'C':
                case 'c':
                case '♣':
                    _Suit = SuitType.Clubs;
                    break;
                default:
                    throw new ArgumentException();
            }
            switch (input[0])
            {
                case 'J':
                case 'j':
                    _cardValue = 11;
                    break;
                case 'Q':
                case 'q':
                    _cardValue = 12;
                    break;
                case 'K':
                case 'k':
                    _cardValue = 13;
                    break;
                case 'A':
                case 'a':
                    _cardValue = 1;
                    break;
                default:
                    //if(!int.TryParse(input.Substring(0, input.Length - 1), out _cardValue) || _cardValue < 2 || _cardValue > 10)
                    throw new ArgumentException();
                    break;
            }
        }
        /// <summary>
        /// function for returning a new PokerCard given a string
        /// </summary>
        /// <param name="cardString"></param>
        /// <returns></returns>
        public static PokerCard Parse(string cardString)
        {
            return new PokerCard(cardString);
        }

        public string encode()
        {
            string encodedCard = "";
            switch (_Suit)
            {
                //Trefle
                case SuitType.Clubs:
                    encodedCard += 'c';
                    break;
                //Pique
                case SuitType.Spades:
                    encodedCard += 's';
                    break;
                //Coeur
                case SuitType.Hearts:
                    encodedCard += 'h';
                    break;
                //Carreaux
                case SuitType.Diamonds:
                    encodedCard += 'd';
                    break;
            }
            encodedCard += (char)_cardValue;
            return encodedCard;
        }

        /// <summary>
        /// return the PokerCard string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string output = "";
            if (_cardValue > 10)
            {
                switch (_cardValue)
                {
                    case 11:
                        output += "Jack";
                        break;
                    case 12:
                        output += "Queen";
                        break;
                    case 13:
                        output += "King";
                        break;
                    case 14:
                        output += "Ace";
                        break;
                }
            }
            else
            {
                output += _cardValue;
            }
            output += " of " + System.Enum.GetName(typeof(SuitType), _Suit);
            return output;
        }
    }
}
