using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardGame.Interfaces;
using MonsterTradingCardGame.Classes;
using MonsterTradingCardGame.Dictionary;
using MonsterTradingCardGame.Enum;
using MonsterTradingCardGame.PostgreDB;

namespace MonsterTradingCardGame.Classes
{
    public class Battles : IBattles
    {
        private readonly DBConn db = new();
        public void Battle(User user1, User user2)
        {
            bool endOfBattle = false;
            int rounds = 0;
            var random = new Random();

            User gameUser1 = user1.DeepCopy();
            User gameUser2 = user2.DeepCopy();

            Cards playCard1;
            Cards playCard2;

            int gameCardInt1;
            double gameCard1Dmg;
            int gameCardInt2;
            double gameCard2Dmg;

            bool Arch1 = true;
            bool Arch2 = true;

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

                double ArchMulti = 1.5;
                //Unique Feature 'Archangel'
                if (gameUser1.UserPlayCardStack.Count == 1 && Arch1)
                {
                    Console.WriteLine($"{gameUser1.UniqueUsername} triggered 'Archangel' his Card deals 50% more Damage this Round...");
                    if (playCard1.CardDamage > gameCard1Dmg)
                    {
                        gameCard1Dmg = playCard1.CardDamage * ArchMulti;
                    }
                    else
                    {
                        gameCard1Dmg *= ArchMulti;
                    }
                    Arch1 = false;
                }
                if (gameUser2.UserPlayCardStack.Count == 1 && Arch2)
                {
                    Console.WriteLine($"{gameUser2.UniqueUsername} triggered 'Archangel' his Card deals 50% more Damage this Round...");
                    if (playCard2.CardDamage > gameCard2Dmg)
                    {
                        gameCard2Dmg = playCard2.CardDamage * ArchMulti;
                    }
                    else
                    {
                        gameCard2Dmg *= ArchMulti;
                    }
                    Arch2 = false;
                }

                Console.WriteLine($"DMG was {playCard1.CardDamage}, is now {gameCard1Dmg}");
                Console.WriteLine($"DMG was {playCard2.CardDamage}, is now {gameCard2Dmg}");

                if (gameCard1Dmg > gameCard2Dmg)
                {
                    Console.WriteLine($"{gameUser1.UniqueUsername} won that round with {playCard1.CardName} and got {playCard2.CardName}");
                    gameUser1.UserPlayCardStack.Add(gameUser2.UserPlayCardStack.ElementAt(gameCardInt2));
                    gameUser2.UserPlayCardStack.RemoveAt(gameCardInt2);
                }
                else if (gameCard1Dmg < gameCard2Dmg)
                {
                    Console.WriteLine($"{gameUser2.UniqueUsername} won that round with {playCard2.CardName} and got {playCard1.CardName}");
                    gameUser2.UserPlayCardStack.Add(gameUser1.UserPlayCardStack.ElementAt(gameCardInt1));
                    gameUser1.UserPlayCardStack.RemoveAt(gameCardInt1);
                }
                else
                {
                    Console.WriteLine("Draw\n Cards have same dmg");
                }

                rounds++;

                Console.WriteLine($"{gameUser1.UniqueUsername} has {gameUser1.UserPlayCardStack.Count} Cards in the Play-Deck.\n{gameUser2.UniqueUsername} has {gameUser2.UserPlayCardStack.Count} Cards in the Play-Deck\n");
                Console.WriteLine($"Rounds played: {rounds}\n");

                Console.WriteLine($"_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_\n\n");

                if (rounds >= 100)
                {
                    EndOfGameHeader();
                    Console.WriteLine($"The Game Ended as a Draw after 100 Rounds\nNo one loses or gains Elo!");
                    Console.ReadLine();
                    endOfBattle = true;
                    return;
                }

                if (gameUser1.UserPlayCardStack.Count > 0 && gameUser2.UserPlayCardStack.Count > 0) continue;

                if (gameUser1.UserPlayCardStack.Count > 0)
                {
                    BattleWinning(gameUser1, gameUser2, rounds);
                }
                else if (gameUser2.UserPlayCardStack.Count > 0)
                {
                    BattleWinning(gameUser2, gameUser1, rounds);
                }
                else
                {
                    Console.WriteLine($"An Error occurred in the Battle between {gameUser1.UniqueUsername} and {gameUser2.UniqueUsername} in round {rounds}.\nNO Elo is gained nor lost in this round!");
                }
                endOfBattle = true;
                return;
            }
        }

        private void BattleWinning(User winner, User loser, int rounds)
        {
            int winCoins = 1;
            EndOfGameHeader();
            Console.WriteLine($"{winner.UniqueUsername} has won the Battle in {rounds} rounds vs {loser.UniqueUsername}!\n{winner.UniqueUsername} gained +3 Elo, while {loser.UniqueUsername} lost -5 Elo.");
            db.UpdateWin(winner);
            db.IncreaseUserCoins(winner, winCoins);
            db.UpdateLos(loser);
        }

        public double CheckDmgMulti(Cards card1, Cards card2)
        {
            GameDictionary gameDictionary = new GameDictionary();
            gameDictionary.InitializeAllDictionaries();

            double multi = 1;
            if (card1.CardType != CardTypesEnum.CardTypeEnum.Monster ||
                card2.CardType != CardTypesEnum.CardTypeEnum.Monster)
            {
                if (gameDictionary.EffectiveGameDictionary[$"{card1.CardElement}"] == card2.CardElement.ToString())
                {
                    Console.WriteLine($"{card1.CardName} {card1.CardElement} is effective vs {card2.CardName} {card2.CardElement}");
                    multi = 2;
                }else if (gameDictionary.NotEffectiveGameDictionary[$"{card1.CardElement}"] == $"{card2.CardElement}")
                {
                    Console.WriteLine($"{card1.CardName} {card1.CardElement} is NOT effective vs {card2.CardName} {card2.CardElement}");
                    multi = 0.5;
                }
            }

            if (gameDictionary.NotSpecialGameDictionary.ContainsKey($"{card1.CardName}"))
            {
                if (gameDictionary.NotSpecialGameDictionary[$"{card1.CardName}"] == card2.CardName ||
                    gameDictionary.NotSpecialGameDictionary[$"{card1.CardName}"] ==
                    card2.CardElement + card2.CardName)
                {
                    Console.WriteLine(
                        $"{card1.CardName} {card1.CardElement} has NO CHANCE vs {card2.CardName} {card2.CardElement}");
                    multi = 0;
                }
            }

            return multi;
        }

        private static void EndOfGameHeader()
        {
            Console.WriteLine("  ........          ....         ....         ....  ............. ");
            Console.WriteLine(" ..........        ......        .....       .....  ............. ");
            Console.WriteLine("....              ...  ...       .......   .......  ....          ");
            Console.WriteLine("....             ...    ...      .... ... ... ....  .........     ");
            Console.WriteLine("....    .....   ............     ....  .....  ....  .........     ");
            Console.WriteLine("....     ...   ..............    ....   ...   ....  ....          ");
            Console.WriteLine(" ..........   ...          ...   ....         ....  ............. ");
            Console.WriteLine("  ........   ....          ....  ....         ....  ............. ");
            Console.WriteLine("\n\n");
        }
    }
}
