using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame.Classes
{
    class Menu
    {
        private string _userInput;
        public void UserMenu()
        {
            bool exit = true;
            do
            {
                Console.Clear();
                Console.WriteLine($"Login\nDeck\nBattle\nProfile\nShop\n");
                Console.Write("Input: ");

                while (_userInput == null)
                {
                    _userInput = Console.ReadLine();
                }

                if (_userInput.Equals("Login"))
                {
                    Console.WriteLine("User chose not implemented Function Login!");
                    exit = false;
                }else if (_userInput.Equals("Deck"))
                {
                    Console.WriteLine("User chose not implemented Function Deck!");
                    exit = false;
                }
                else if (_userInput.Equals("Battle"))
                {
                    Console.WriteLine("User chose not implemented Function Battle!");
                    exit = false;
                }
                else if (_userInput.Equals("Profile"))
                {
                    Console.WriteLine("User chose not implemented Function Profile!");
                    exit = false;
                }
                else if (_userInput.Equals("Shop"))
                {
                    Console.WriteLine("User chose not implemented Function Shop!");
                    exit = false;
                }
                else
                {
                    _userInput = null;
                }
            } while (exit);
        }
    }
}
