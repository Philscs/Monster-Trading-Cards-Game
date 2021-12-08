using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardGame.Interfaces;
using MonsterTradingCardGame.Classes;
using MonsterTradingCardGame.Enum;

namespace MonsterTradingCardGame.Classes
{
    class Battles : IBattles
    {
        public void Battle(User user1, User user2)
        {
            Console.Clear();
            bool endOfBattle = false;
            int rounds = 0;
            var random = new Random();

            User gameUser1 = user1.DeepCopy();
            User gameUser2 = user2.DeepCopy();

            Cards playCard1 = new Cards();
            Cards playCard2 = new Cards();

            int gameCardInt1;
            double gameCard1Dmg;
            int gameCardInt2;
            double gameCard2Dmg;

            while (!endOfBattle)
            {
                gameCardInt1 = random.Next(gameUser1.UserPlayCardStack.Count);
                gameCardInt2 = random.Next(gameUser2.UserPlayCardStack.Count);
                playCard1 = gameUser1.UserPlayCardStack.ElementAt(gameCardInt1);
                playCard2 = gameUser2.UserPlayCardStack.ElementAt(gameCardInt2);

                Console.WriteLine($"Card1\n Name: {playCard1.CardName}\n Ele: {playCard1.CardElement}\n DMG: {playCard1.CardDamage}\n Type: {playCard1.CardType}\n");
                Console.WriteLine($"Card2\n Name: {playCard2.CardName}\n Ele: {playCard2.CardElement}\n DMG: {playCard2.CardDamage}\n Type: {playCard2.CardType}\n");

                //checks multiplier for cards DMG
                gameCard1Dmg = playCard1.CardDamage * CheckDmgMulti(playCard1, playCard2);
                gameCard2Dmg = playCard2.CardDamage * CheckDmgMulti(playCard2, playCard1);

                if (gameCard1Dmg > gameCard2Dmg)
                {
                    Console.WriteLine("Card 1 won");
                    gameUser1.UserPlayCardStack.Add(gameUser2.UserPlayCardStack.ElementAt(gameCardInt2));
                    gameUser2.UserPlayCardStack.RemoveAt(gameCardInt2);
                }
                else if (gameCard1Dmg < gameCard2Dmg)
                {
                    Console.WriteLine("Card 2 won");
                    gameUser2.UserPlayCardStack.Add(gameUser1.UserPlayCardStack.ElementAt(gameCardInt1));
                    gameUser1.UserPlayCardStack.RemoveAt(gameCardInt1);
                }
                else
                {
                    Console.WriteLine("Draw\n Cards have same dmg");
                }

                rounds++;
                if (rounds >= 100)
                {
                    endOfBattle = true;
                }
                else
                {
                    Console.WriteLine($"Rounds played: {rounds}");
                }

                if (gameUser1.UserPlayCardStack.Count > 0 && gameUser2.UserPlayCardStack.Count > 0) continue;
                Console.WriteLine($"Play Card Deck is empty\n{gameUser1.UniqueUsername} has {gameUser1.UserPlayCardStack.Count} left\n{gameUser2.UniqueUsername} has {gameUser2.UserPlayCardStack.Count} left\n");

                Console.Write(gameUser1.UserPlayCardStack.Count > 0 ? $"{gameUser1.UniqueUsername}" : $"{gameUser2.UniqueUsername}");
                Console.WriteLine($" has won the Battle in {rounds} rounds!");
                endOfBattle = true;
                return;
            }
            


        }

        private int CheckDmgMulti(Cards card1, Cards card2)
        {
            int multi = 1;
            if (card1.CardType != CardTypesEnum.CardTypeEnum.Monster &&
                card2.CardType != CardTypesEnum.CardTypeEnum.Monster)
            {
                if (expr)
                {
                    
                }
            }

            return multi;
        }
    }
}
