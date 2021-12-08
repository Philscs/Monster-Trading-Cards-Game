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
            User user = new User();
            bool exit = true;
            do
            {
                _userInput = null;
                Console.Clear();
                Console.WriteLine("Welcome to the Monster-Game!");
                Console.WriteLine($" Login\n Deck\n Battle\n Profile\n Shop\n Register\n Quit\n");
                Console.Write("Input: ");

                while (_userInput == null)
                {
                    _userInput = Console.ReadLine();
                }

                if (_userInput.Equals("Login"))
                {
                    user = UserLogin();
                }
                else if (_userInput.Equals("Deck"))
                {
                    UserDeck(user);
                }
                else if (_userInput.Equals("Battle"))
                {
                    UserBattle(user);
                    Console.ReadLine();

                }
                else if (_userInput.Equals("Profile"))
                {
                    UserProfile(user);
                }
                else if (_userInput.Equals("Shop"))
                {
                    UserShop();
                }
                else if (_userInput.Equals("Register"))
                {
                    user = UserRegister();

                }
                else if (_userInput.Equals("Quit"))
                {
                    Console.WriteLine("We hope you enjoyed your game!");
                    Console.ReadLine();
                    exit = false;
                }
                else
                {
                    Console.WriteLine("Wrong Input!\nPress any key to try again...\n");
                    Console.ReadLine();
                }
            } while (exit);

        }

        public User UserLogin()
        {
            string userName = null;
            Console.WriteLine("Enter Username:");
            while (userName == null)
            {
                userName = Console.ReadLine();
            }
            User loginUser = new User(userName, 88, 1337);
            Console.WriteLine($"{userName} is now logged in.");
            Console.ReadLine();


            return loginUser;
        }

        public void UserDeck(User user)
        {
            if (user.UniqueUsername != null)
            {
                user.DeckManager();
            }
            else
            {
                NoUser();
            }
            Console.ReadLine();
        }

        public void UserBattle(User user)
        {
            if (user.UniqueUsername != null)
            {
                User aiUser = new User("AI");
                Battles battles = new Battles();
                if (user.UserPlayCardStack is {Count: 4})
                {
                    battles.Battle(user, aiUser);
                }
                else
                {
                    Console.WriteLine("Insufficient Play-Card-Stack!\nPlease change your Deck.");
                }
            }
            else
            {
                NoUser();
            }
        }

        public void UserProfile(User user)
        {
            if (user.UniqueUsername != null)
            {
                user.PrintUserInformation();
            }
            else
            {
                NoUser();
            }
            Console.ReadLine();
        }

        public void UserShop()
        {
            Console.WriteLine($"Stay tuned, our Shop will come soon...");
            Console.ReadLine();
        }

        public User UserRegister()
        {
            string userName = null;
            Console.WriteLine("Enter Username:");
            while (userName == null)
            {
                userName = Console.ReadLine();
            }
            User registerUser = new User(userName, 20, 1000, 0);
            Console.WriteLine($"{userName} is now registered.");
            Console.ReadLine();

            return registerUser;

        }

        public void NoUser()
        {
            Console.WriteLine("No User is logged in, please Login first!");
        }
    }
}
