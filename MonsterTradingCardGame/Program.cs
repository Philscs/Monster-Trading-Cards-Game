using System;
using System.Threading;
using MonsterTradingCardGame.Classes;
using MonsterTradingCardGame.Dictionary;
using MonsterTradingCardGame.Enum;
using MonsterTradingCardGame.ToTestFunctions;

namespace MonsterTradingCardGame
{
    class Program
    {
        static void Main(string[] args)
        {
            
            //create Dictionary for Gamestats
            GameDictionary gameDictionary = new GameDictionary();
            gameDictionary.InitializeAllDictionaries();
            //Console.WriteLine(gameDictionary.CheckEffectiveGameDictionary("Water","Fire"));

            Console.WriteLine("Card Tests");
            CardTests cardTests = new CardTests();
            cardTests.InitializeCards();
            //cardTests.PrintAllTestCards();

            Cards card = new Cards();
            card = cardTests.GetCard(0);
            //Console.WriteLine($"{card.CardElement} {card.CardName}");

            //Console.WriteLine(card1.CardName);
            Menu menu = new Menu();
           
            menu.UserMenu();
           
        }
    }
}
