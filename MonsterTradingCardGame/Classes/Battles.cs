using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardGame.Interfaces;
using MonsterTradingCardGame.Classes;
using MonsterTradingCardGame.Dictionary;
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
                Console.WriteLine($"DMG was {playCard1.CardDamage}, is now {gameCard1Dmg}");
                Console.WriteLine($"DMG was {playCard2.CardDamage}, is now {gameCard2Dmg}");

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

        private double CheckDmgMulti(Cards card1, Cards card2)
        {
            GameDictionary gameDictionary = new GameDictionary();
            gameDictionary.InitializeAllDictionaries();

            double multi = 1;
            if (card1.CardType != CardTypesEnum.CardTypeEnum.Monster ||
                card2.CardType != CardTypesEnum.CardTypeEnum.Monster)
            {
                Console.WriteLine("One Card is a spell");
                if (gameDictionary._effectiveGameDictionary[$"{card1.CardElement}"] == card2.CardElement.ToString())
                {
                    Console.WriteLine($"{card1.CardName} {card1.CardElement}  is effective vs {card2.CardName} {card2.CardElement}");
                    multi = 2;
                }else if (gameDictionary._notEffectiveGameDictionary[$"{card1.CardElement}"] == $"{card2.CardElement}")
                {
                    Console.WriteLine($"{card1.CardName} {card1.CardElement}  is NOT effective vs {card2.CardName} {card2.CardElement}");
                    multi = 0.5;
                }
            }

            if (gameDictionary._notEffectiveGameDictionary.ContainsKey($"{card1.CardName}"))
            {
                Console.WriteLine("One Card has a Special Trait");
                if (gameDictionary._notspecialGameDictionary[$"{card1.CardName}"] == card2.CardName ||
                    gameDictionary._notspecialGameDictionary[$"{card1.CardName}"] ==
                    card2.CardElement + card2.CardName)
                {
                    Console.WriteLine(
                        $"{card1.CardName} {card1.CardElement}  has NO CHANCE vs {card2.CardName} {card2.CardElement}");
                    multi = 0;
                }
            }
            return multi;
        }
    }
}
