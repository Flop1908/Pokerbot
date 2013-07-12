using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HoldemHand;

namespace BotPoker
{
    class Calculon
    {
        /// <summary>
        /// Fonction wich determine what the bot should do
        /// </summary>
        /// <param name="_situation"></param>
        /// <returns>fold or raise or check</returns>
        public static string elCalculator (Situation _situation)
        {
            double odds = 0;

            //less money than we can bet
            //if (_situation.minimumBet > _situation.moneyLeft)
               // return "fold";

            //can i afford to bet easy
            //odds += addMoneyLeftVSMinimumBet(_situation.minimumBet, _situation.moneyLeft);

            //play more agressive with less player
            //odds += addPlayerLeftOdds(_situation.playerLeft); 

            //odds of having a winning hand
            odds += addCardsOdds(_situation.playerCards, _situation.communityCards);


            return oddsDecision(odds);
            
        }

        private static double addMoneyLeftVSMinimumBet(int minimum, int moneyIHave)
        {
            if (moneyIHave>0)
            {
                return 0;
            }
            else
            {
                return (minimum / moneyIHave);
            }
        }

        private static double addPlayerLeftOdds(int p)
        {
            return 10 - p;
        }

        private static double addCardsOdds(List<PokerCard> listPlayerCards, List<PokerCard> listCommunityCards)
        {
            double returnOdds = 0;
            //ulong playerMask = Hand.ParseHand("as ks"); // Player Pocket Cards
            //string des cartes du joueur

            string playerCards = "";
            foreach (PokerCard currentCard in listPlayerCards)
            {
                playerCards += currentCard.ToString();
                playerCards += " ";
            }
            ulong playerMask = Hand.ParseHand(playerCards);

            //ulong board = Hand.ParseHand("Ts Qs 2d");   // Partial Board
            //string des cartes du milieu

            string boardCards = "";
            foreach (PokerCard currentCard in listCommunityCards)
            {
                boardCards += currentCard.ToString().Trim();
                boardCards += " ";
            }
            ulong board = Hand.ParseHand(boardCards);


            // Calculate values for each hand type
            double[] playerWins = { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
            double[] opponentWins = { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
            // Count of total hands examined.
            long count = 0;

            // Iterate through all possible opponent hands
            foreach (ulong opponentMask in Hand.Hands(0UL,
                                 board | playerMask, 2))
            {
                // Iterate through all possible boards
                foreach (ulong boardMask in Hand.Hands(board,
                               opponentMask | playerMask, 5))
                {
                    // Create a hand value for each player
                    uint playerHandValue =
                           Hand.Evaluate(boardMask | playerMask, 7);
                    uint opponentHandValue =
                           Hand.Evaluate(boardMask | opponentMask, 7);

                    // Calculate Winners
                    if (playerHandValue > opponentHandValue)
                    {
                        // Player Win
                        playerWins[Hand.HandType(playerHandValue)] += 1.0;
                    }
                    else if (playerHandValue < opponentHandValue)
                    {
                        // Opponent Win
                        opponentWins[Hand.HandType(opponentHandValue)] += 1.0;
                    }
                    else if (playerHandValue == opponentHandValue)
                    {
                        // Give half credit for ties.
                        playerWins[Hand.HandType(playerHandValue)] += 0.5;
                        opponentWins[Hand.HandType(opponentHandValue)] += 0.5;
                    }
                    count++;
                }
            }

            // Print results
            /*
            Console.WriteLine("Player Results");
            Console.WriteLine("High Card:\t{0:0.0}%",
              playerWins[(int)Hand.HandTypes.HighCard] / ((double)count) * 100.0);
            Console.WriteLine("Pair:\t{0:0.0}%",
              playerWins[(int)Hand.HandTypes.Pair] / ((double)count) * 100.0);
            Console.WriteLine("Two Pair:\t{0:0.0}%",
              playerWins[(int)Hand.HandTypes.TwoPair] / ((double)count) * 100.0);
            Console.WriteLine("Three of Kind:\t{0:0.0}%",
              playerWins[(int)Hand.HandTypes.Trips] / ((double)count) * 100.0);
            Console.WriteLine("Straight:\t{0:0.0}%",
              playerWins[(int)Hand.HandTypes.Straight] / ((double)count) * 100.0);
            Console.WriteLine("Flush:\t{0:0.0}%",
              playerWins[(int)Hand.HandTypes.Flush] / ((double)count) * 100.0);
            Console.WriteLine("Fullhouse:\t{0:0.0}%",
              playerWins[(int)Hand.HandTypes.FullHouse] / ((double)count) * 100.0);
            Console.WriteLine("Four of a Kind:\t{0:0.0}%",
              playerWins[(int)Hand.HandTypes.FourOfAKind] / ((double)count) * 100.0);
            Console.WriteLine("Straight Flush:\t{0:0.0}%",
              playerWins[(int)Hand.HandTypes.StraightFlush] / ((double)count) * 100.0);
            */

            //power ratio
            returnOdds += playerWins[(int)Hand.HandTypes.HighCard] / ((double)count) * 100.0;
            returnOdds *= 0.2;
            returnOdds += playerWins[(int)Hand.HandTypes.Pair] / ((double)count) * 100.0;
            returnOdds *= 0.5;
            returnOdds += playerWins[(int)Hand.HandTypes.TwoPair] / ((double)count) * 100.0;
            returnOdds *= 0.8;
            //brelan
            returnOdds += playerWins[(int)Hand.HandTypes.Trips] / ((double)count) * 100.0;
            returnOdds *= 1;
            returnOdds += playerWins[(int)Hand.HandTypes.Straight] / ((double)count) * 100.0;
            returnOdds *= 2;
            returnOdds += playerWins[(int)Hand.HandTypes.Flush] / ((double)count) * 100.0;
            returnOdds *= 4;
            returnOdds += playerWins[(int)Hand.HandTypes.FullHouse] / ((double)count) * 100.0;
            returnOdds *= 5;
            returnOdds += playerWins[(int)Hand.HandTypes.FourOfAKind] / ((double)count) * 100.0;
            returnOdds *= 6;
            returnOdds += playerWins[(int)Hand.HandTypes.StraightFlush] / ((double)count) * 100.0;
            returnOdds *= 10;

            return returnOdds;
        }

        public static string oddsDecision(double odds)
        {
            if (odds > 50) return "raise";
            else if (odds > 20) return "check";
            else return "fold";
        }
    }
}
