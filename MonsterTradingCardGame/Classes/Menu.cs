using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonsterTradingCardGame.PostgreDB;
using BCryptNet = BCrypt.Net.BCrypt;

namespace MonsterTradingCardGame.Classes
{
    class Menu
    {
        private string _userInput;
        DBConn db = new DBConn();
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
            //get Username
            Console.WriteLine("Enter Username:");
            while (userName == null)
            {
                userName = Console.ReadLine();
            }
            //get Password from User but only show '*'
            Console.WriteLine("Enter Password:");
            var pass = string.Empty;
            ConsoleKey key;
            do
            {
                var keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    Console.Write("\b \b");
                    pass = pass[0..^1];
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    Console.Write("*");
                    pass += keyInfo.KeyChar;
                }
            } while (key != ConsoleKey.Enter);

            //checks if username is in DB and if it is, check if pwd matches with BCrypt
            User user = db.LoginUser(userName, pass);

            if (user != null)
            {
                Console.WriteLine($"\n{userName} is now logged in.");
                Console.ReadLine();

                return user;
            }
            Console.WriteLine($"\nWrong Credentials to log in, try again!");
            Console.ReadLine();

            return null;
        }

        public void UserDeck(User user)
        {
            if (user != null)
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
            if (user != null)
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
            if (user != null)
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
            //get Username
            Console.WriteLine("Enter Username:");
            while (userName == null)
            {
                userName = Console.ReadLine();
            }
            //get Password from User but only show '*'
            Console.WriteLine("Enter Password:");
            var pass = string.Empty;
            ConsoleKey key;
            do
            {
                var keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    Console.Write("\b \b");
                    pass = pass[0..^1];
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    Console.Write("*");
                    pass += keyInfo.KeyChar;
                }
            } while (key != ConsoleKey.Enter);

            //hash the pwd with BCrypt
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(pass);

            User registerUser = new User(userName, 20, 1000, passwordHash, 0);

            bool reg = db.RegisterUser(registerUser);
            if (reg)
            {
                Console.WriteLine($"\n{userName} is now registered.");
                Console.ReadLine();
                return registerUser;
            }

            Console.WriteLine($"\nRegistration failed, try again with a different Username");
            Console.ReadLine();

            return null;
        }

        public void NoUser()
        {
            Console.WriteLine("No User is logged in, please Login first!");
        }
    }
}
