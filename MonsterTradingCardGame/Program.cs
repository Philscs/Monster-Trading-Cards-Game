using System;
using System.Threading;
using MonsterTradingCardGame.Classes;
using MonsterTradingCardGame.Dictionary;
using MonsterTradingCardGame.Enum;

namespace MonsterTradingCardGame
{
    class Program
    {
        static void Main(string[] args)
        {
            
            //create Dictionary for Gamestats
            GameDictionary gameDictionary = new GameDictionary();
            Menu menu = new Menu();

            gameDictionary.InitializeAllDictionaries();

            menu.UserMenu();
        }
    }
}
