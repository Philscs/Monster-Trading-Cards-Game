using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardGame.Enum;
using MonsterTradingCardGame.Interfaces;
using MonsterTradingCardGame.PostgreDB;

namespace MonsterTradingCardGame.Classes
{
    class User : IUser
    {
        private DBConn db = new DBConn();
        public User()
        {

        }
        public User(int userID, string unique, int coins, int userElo, string userPassword, int userWins, int userLoses)
        {
            UserID = userID;
            UniqueUsername = unique;
            Coins = coins;
            UserElo = userElo;
            password = userPassword;
            Wins = userWins;
            Loses = userLoses;
            UserPlayCardStack = GetUserPlayCardStack();
            UserAllCardStack = GetUserAllCardStack();
        }

        public User DeepCopy()
        {
            User other = (User) this.MemberwiseClone();
            other.UniqueUsername = UniqueUsername;
            other.Coins = Coins;
            other.UserElo = UserElo;
            other.password = password;
            other.UserPlayCardStack = new List<Cards>(UserPlayCardStack);
            other.UserAllCardStack = new List<Cards>(UserAllCardStack);
            return other;
        }

        //to Register User
        public User(string unique, string userPassword)
        {
            UserID = 0;
            UniqueUsername = unique;
            Coins = 20;
            UserElo = 1000;
            password = userPassword;
            Wins = 0;
            Loses = 0;
            UserPlayCardStack = null;
            UserAllCardStack = null;
        }

        public int UserID { get; set; }
        public string UniqueUsername { get; set; }
        public int Coins { get; set; }
        public int UserElo { get; set; }
        public string password { get; set; }
        public int Wins { get; set; }
        public int Loses{ get; set; }

        public List<Cards> UserPlayCardStack { get; set; }
        public List<Cards> UserAllCardStack { get; set; }

        private const int IndexSize = 4;
        
        public void ChangeUserPlayCardStack()
        {
            if (UserAllCardStack.Count > 0)
            {
                if (UserPlayCardStack != null)
                {
                    UserPlayCardStack.Clear();
                }
                else
                {
                    UserPlayCardStack = new List<Cards>();
                }

                List<Cards> tmpCardsList = db.GetUserAllCardStack(this);
                int chosenCard;
                int cardCount = 0;
                //get Cards from DB not from TestCards
                while (cardCount < IndexSize)
                {
                    Console.Clear();
                    PrintAllCardDeck(tmpCardsList);
                    Console.Write("Choose Cards by there Index:");
                    chosenCard = -1;
                    while (chosenCard < 0)
                    {
                        string input = Console.ReadLine();
                        Int32.TryParse(input, out chosenCard);
                        if (chosenCard < 0 || chosenCard > tmpCardsList.Count)
                        {
                            Console.WriteLine("Wrong Index!\nTry again...");
                            chosenCard = 0;
                        }
                    }
                    cardCount++;

                    UserPlayCardStack.Add(tmpCardsList.ElementAt(chosenCard));
                    tmpCardsList.RemoveAt(chosenCard);
                    Console.Clear();
                    Console.WriteLine(cardCount < IndexSize ? "Your Deck at the Moment:" : "Your chosen Deck is:");
                    PrintAllCardDeck(UserPlayCardStack);
                    Console.WriteLine("Press any key to continue");
                    Console.ReadLine();
                }

                if (!db.ChangePlayDeck(this))
                {
                    Console.WriteLine("An Error occurred!\nPlease build a new Deck!");
                    UserPlayCardStack.Clear();
                }
            }
            else
            {
                Console.WriteLine($"You have no Cards!\nBuy some in the Shop");
            }
        }
        public List<Cards> GetUserPlayCardStack()
        {
            return db.GetUserPlayCardStack(this);
        } 
        
        public List<Cards> GetUserAllCardStack()
        {
            return db.GetUserAllCardStack(this); ;
        }

        public void PrintAllCardDeck(List<Cards> deck)
        {
            int i = 0;
            foreach (var card in deck)
            {
                Console.WriteLine($"Index {i} -> {card.CardElement} {card.CardName}\n Dmg: {card.CardDamage}");
                i++;
            }

        }

        public void PrintUserInformation()
        {
            Console.Clear();

            Console.WriteLine($"Profile:\n Username: {UniqueUsername}\n Elo:      {UserElo}\n Coins:    {Coins}\n Wins:     {Wins}\n Loses:    {Loses}");
            int percentage;
            if (Wins == 0 && Loses == 0)
            {
                percentage = 0;
            }
            else
            {
                percentage = Wins / (Wins + Loses) * 100;
            }
            Console.WriteLine($" Win-%:    {percentage}");
        }

        public void DeckManager()
        {
            Console.Clear();
            string userInput;
            bool quitManager = false;
            Console.WriteLine($"Welcome to the Deck-Manager from {UniqueUsername}");
            
            while (!quitManager)
            {
                Console.WriteLine($" Print  -> to see your chosen Deck\n Change -> to change your chosen Deck\n All    -> to see all your Cards\n Quit   -> Deck-Manager & go back to the Main-Menu");
                userInput = null;
                Console.Write("Input: ");
                while (userInput == null)
                {
                    userInput = Console.ReadLine();
                }

                if (userInput.Equals("Print"))
                {
                    Console.Clear();
                    if (UserPlayCardStack is {Count: 4})
                    {
                        PrintAllCardDeck(UserPlayCardStack);
                        Console.ReadLine();
                    }
                    else
                    {
                        Console.WriteLine($"You have no valid Deck\nPlease choose a Deck with the 'Change' Command\n");
                        Console.ReadLine();
                    }
                }
                else if (userInput.Equals("Change"))
                {
                    Console.Clear();
                    if (UserAllCardStack != null)
                    {
                        ChangeUserPlayCardStack();
                    }
                    else
                    {
                        Console.WriteLine($"You have no Cards\nPlease buy some Packs in the Shop\n");
                        Console.ReadLine();
                    }
                }
                else if (userInput.Equals("All"))
                {
                    Console.Clear();
                    if (UserAllCardStack != null)
                    {
                        PrintAllCardDeck(UserAllCardStack);
                        Console.ReadLine();
                    }
                    else
                    {
                        Console.WriteLine($"You have no Cards\nPlease buy some Packs in the Shop\n");
                        Console.ReadLine();
                    }
                }
                else if (userInput.Equals("Quit"))
                {
                    Console.Clear();
                    Console.WriteLine($"{UniqueUsername} quit Deck-Manager.");
                    quitManager = true;
                    return;
                }
                else
                {
                    Console.WriteLine("Wrong Input!\nPress any key to try again...\n");
                    Console.ReadLine();
                }
                Console.Clear();
                Console.WriteLine("Deck-Manger");
            }
        }

        public User ChangeUserProfile(User user)
        {
            Console.WriteLine($"Change Data here");
            Console.ReadLine();
            return user;
        }

        public void PrintScoreboard()
        {
            Console.Clear();
            string userInput = null;
            List<(string, int)> score = null;
            Console.WriteLine($"Choose how the Scoreboard should be displayed.");
            Console.WriteLine($" Elo   -> Highest Elo descending\n Games -> Highest Win-% descending");
            do
            {
                Console.WriteLine("Input: ");
                while (userInput == null)
                {
                    userInput = Console.ReadLine();
                }
                if (userInput.Equals("Elo"))
                {
                    score = db.GetScoreByElo();
                    if (score != null)
                    {
                        int i = 1;
                        Console.Clear();
                        Console.WriteLine($"Scoreboard sorted by Elo");
                        foreach (var tuple in score)
                        {
                            Console.WriteLine($" {i}. [User - {tuple.Item1} / Elo - {tuple.Item2}]");
                            i++;
                        }
                    }
                    else
                    {
                        Console.WriteLine(NoSearch());
                    }
                }
                else if (userInput.Equals("Games"))
                {
                    score = db.GetScoreByGames();
                    if (score != null)
                    {
                        int i = 1;
                        Console.Clear();
                        Console.WriteLine($"Scoreboard sorted by Win-%");
                        foreach (var tuple in score)
                        {
                            Console.WriteLine($" {i}. [User - {tuple.Item1} / Win-% - {tuple.Item2}]");
                            i++;
                        }
                    }
                    else
                    {
                        Console.WriteLine(NoSearch());
                    }
                }
                else
                {
                    Console.WriteLine("Wrong Input!\nPress any key to try again...\n");
                }
                Console.ReadLine();
            } while (userInput == null);
        }

        public string NoSearch()
        {
            string noSearch = "Your Search was invalid, nothing found!";
            return noSearch;
        }

        public User Shop(User user)
        {
            string checkInput = null;
            string userInput = null;
            bool exit = true;
            do
            {
                userInput = null;
                checkInput = null;
                Console.Clear();
                Console.WriteLine($"Package-Manager\nAll Packages cost 5 Gold and include 5 Cards!\nYou may get Cards you already have...\nChoose a Package:");
                Console.WriteLine($" Water  -> get Monsters of Type Water\n Fire   -> get Monsters of Type Fire\n Normal -> get Monsters of Type Normal\n Rand   -> get Monster of a random Type\n Mixed  -> get a Mix of Monsters and Spells\n Quit   -> quit Package Manager & return to the Shop\nInput:");
                while (userInput == null)
                {
                    userInput = Console.ReadLine();
                }

                if (userInput.Equals("Quit"))
                {
                    Console.Clear();
                    Console.WriteLine($"{UniqueUsername} quit Package-Manager.");
                    Console.ReadLine();
                    exit = true;
                    return user;
                }

                Console.WriteLine($"You chose the {userInput} Package.\nChose [Y|n] to confirm or decline your choice...\nInput:");
                
                while (checkInput == null)
                {
                    checkInput = Console.ReadLine();
                }

                if (!checkInput.Equals("Y") && !checkInput.Equals("y"))
                {
                    Console.WriteLine($"You dismissed the Package!\nPress any Key to try again.");
                    Console.ReadLine();
                    continue;
                }

                if (db.CheckCoins(this) < 5)
                {
                    Console.WriteLine($"Your funds are invalid you have not enough Coins to buy a Package!\nEither Sell Cards or Win Games to get more Coins...");
                    return db.UpdatedUser(this);
                }


                if (userInput.Equals("Water"))
                {
                    if (!db.GetMonsterTypePackages(this, (int) CardTypesEnum.CardElementEnum.Water))
                    {
                        Console.WriteLine($"Something went wrong, try again later!");
                    }
                    else
                    {
                        db.ReduceUserCoins(this);
                        return db.UpdatedUser(this);
                    }
                }
                else if (userInput.Equals("Fire"))
                {
                    if (!db.GetMonsterTypePackages(this, (int)CardTypesEnum.CardElementEnum.Fire))
                    {
                        Console.WriteLine($"Something went wrong, try again later!");
                    }
                    else
                    {
                        db.ReduceUserCoins(this);
                        return db.UpdatedUser(this);
                    }
                }
                else if (userInput.Equals("Normal"))
                {
                    if (!db.GetMonsterTypePackages(this, (int)CardTypesEnum.CardElementEnum.Normal))
                    {
                        Console.WriteLine($"Something went wrong, try again later!");
                    }
                    else
                    {
                        db.ReduceUserCoins(this);
                        return db.UpdatedUser(this);
                    }
                }
                else if (userInput.Equals("Rand"))
                {
                    if (!db.GetRandPackages(this))
                    {
                        Console.WriteLine($"Something went wrong, try again later!");
                    }
                    else
                    {
                        db.ReduceUserCoins(this);
                        return db.UpdatedUser(this);
                    }
                }
                else if (userInput.Equals("Mixed"))
                {
                    if (!db.GetMixedPackages(this))
                    {
                        Console.WriteLine($"Something went wrong, try again later!");
                    }
                    else
                    {
                        db.ReduceUserCoins(this);
                        return db.UpdatedUser(this);
                    }
                }
                else
                {
                    Console.WriteLine("Wrong Input!\nPress any key to try again...\n");
                    Console.ReadLine();
                }
                

            } while (exit);

            return user;
        }
        public User Trade(User user)
        {
            Console.WriteLine($"Trade shit here");
            return user;
        }
    }
}
