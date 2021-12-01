using System;
using MonsterTradingCardGame.Dictionary;

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

            //UserLogin
            Console.WriteLine("Implement Login here");

            //UserMenu
            Console.WriteLine("Implement Menu for User here");

            //Battle
            Console.WriteLine("Implement Battle here");

        }
    }
}
