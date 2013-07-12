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
        /// Value of the card from 2 to 14(Ace)
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
                    _cardValue = 14;
                    break;
                default:
                    int newCardValue;
                    if (!int.TryParse(input.Substring(0, input.Length - 1), out newCardValue) || newCardValue < 1 || newCardValue > 10)
                        throw new ArgumentException();
                    //special ace
                    if (newCardValue == 1)
                        newCardValue = 14;
                    _cardValue = newCardValue;
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
            string encodedCard="";
            switch (_Suit)
            {
                //Trefle
                case SuitType.Clubs:
                    encodedCard = "c";
                    break;
                //Pique
                case SuitType.Spades:
                    encodedCard = "s";
                    break;
                //Coeur
                case SuitType.Hearts:
                    encodedCard = "h";
                    break;
                //Carreaux
                case SuitType.Diamonds:
                    encodedCard = "d";
                    break;
            }
            return encodedCard;
        }

        /// <summary>
        /// return the PokerCard string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string output = "";
            if (_cardValue >= 10)
            {
                switch (_cardValue)
                {
                    case 10:
                        //output += "Ten";
                        output += "T";
                        break;
                    case 11:
                        //output += "Jack";
                        output += "J";
                        break;
                    case 12:
                        //output += "Queen";
                        output += "Q";
                        break;
                    case 13:
                        //output += "King";
                        output += "K";
                        break;
                    case 14:
                        //output += "Ace";
                        output += "A";
                        break;
                }
            }
            else
            {
                output += _cardValue;
            }
            //output += " of " + System.Enum.GetName(typeof(SuitType), _Suit);
            output += encode();
            return output;
        }
    }
}
