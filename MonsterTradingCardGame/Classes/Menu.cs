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
            User user = null;
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
                    user = UserBattle(user);
                    Console.ReadLine();

                }
                else if (_userInput.Equals("Profile"))
                {
                    UserProfile(user);
                }
                else if (_userInput.Equals("Shop"))
                {
                    user = UserShop(user);
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

        public User UserBattle(User user)
        {
            if (user != null)
            {
                if (user.UserPlayCardStack is {Count: 4})
                {
                    User enemyUser = db.GetBattleEnemy(user);
                    Battles battles = new Battles();
                    if (enemyUser != null && enemyUser.UserPlayCardStack.Count == 4 && user.UserID != enemyUser.UserID)
                    {
                        battles.Battle(user, enemyUser);
                        user = db.UpdatedUser(user);

                        if (user == null)
                        {
                            Console.WriteLine($"An Error occurred, please contact the Support or try to Login again!");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"No viable Enemy to Battle was found, try again another time...");
                    }
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

            return user;
        }

        public void UserProfile(User user)
        {
            if (user != null)
            {
                bool exit = true;
                do
                {
                    _userInput = null;
                    Console.Clear();
                    Console.WriteLine($"Profile-Manager\n Show   -> shows User Profile\n Change -> change Profile Data \n Score  -> shows global Scoreboard\n Quit   -> quit Profile-Manager & go back to the Main-Menu\n");
                    Console.Write("Input: ");

                    while (_userInput == null)
                    {
                        _userInput = Console.ReadLine();
                    }

                    if (_userInput.Equals("Show"))
                    {
                        user.PrintUserInformation();
                        Console.ReadLine();
                    }
                    else if (_userInput.Equals("Change"))
                    {
                        user.ChangeUserProfile(user);
                    } 
                    else if (_userInput.Equals("Score"))
                    {
                        user.PrintScoreboard();
                    }
                    else if (_userInput.Equals("Quit"))
                    {
                        Console.Clear();
                        Console.WriteLine($"{user.UniqueUsername} quit Profile-Manager");
                        Console.ReadLine();
                        exit = false;
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Wrong Input!\nPress any key to try again...\n");
                        Console.ReadLine();
                    }
                } while (exit);
            }
            else
            {
                NoUser();
            }
            Console.ReadLine();
        }

        public User UserShop(User user)
        {
            if (user != null)
            {
                bool exit = true;
                do
                {
                    _userInput = null;
                    Console.Clear();
                    Console.WriteLine($"Welcome to our Shop\n Buy    -> Buy Card-packages\n Trade  -> create new Trades & trade with others\n Quit   -> quit the Shop & go back to the Main-Menu\n");
                    Console.Write("Input: ");

                    while (_userInput == null)
                    {
                        _userInput = Console.ReadLine();
                    }

                    if (_userInput.Equals("Buy"))
                    {
                        user = user.Shop(user);
                    }
                    else if (_userInput.Equals("Trade"))
                    {
                        user = user.Trade(user);
                    }
                    else if (_userInput.Equals("Quit"))
                    {
                        Console.Clear();
                        Console.WriteLine($"{user.UniqueUsername} left the Shop");
                        Console.ReadLine();
                        exit = false;
                        return user;
                    }
                    else
                    {
                        Console.WriteLine("Wrong Input!\nPress any key to try again...\n");
                        Console.ReadLine();
                    }
                } while (exit);
            }
            else
            {
                NoUser();
            }
            Console.ReadLine();
            return null;
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

            User registerUser = new User(userName, passwordHash);
            User registered = db.RegisterUser(registerUser);
            
            if (registered != null && registered.UserID != 0)
            {
                Console.WriteLine($"\n{registered.UniqueUsername} is now registered.");
                //Console.WriteLine($"with id: {registered.UserID}");
                Console.ReadLine();
                return registered;
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
