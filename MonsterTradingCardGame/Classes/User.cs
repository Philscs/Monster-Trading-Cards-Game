using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            other.UniqueUsername = String.Copy(UniqueUsername);
            other.Coins = Coins;
            other.UserElo = UserElo;
            other.password = password;
            other.UserPlayCardStack = new List<Cards>(UserPlayCardStack);
            other.UserAllCardStack = new List<Cards>(UserAllCardStack);
            return other;
        }

        //Register User
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
            List<Cards> tmpCardsList = new List<Cards>();
            tmpCardsList = UserAllCardStack;
            UserPlayCardStack.Clear();
            int chosenCard;
            int cardCount = 0;
            //get Cards from DB not from TestCards
            while (cardCount < IndexSize)
            {
                PrintUserAllCardDeck();
                Console.Write("Choose Cards by there Index:");
                chosenCard = 0;
                while (chosenCard == 0)
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
                Console.WriteLine(chosenCard < IndexSize ? "Your Deck at the Moment:" : "Your chosen Deck is:");
                PrintUserPlayCardDeck();
                Console.WriteLine("Press any key to continue");
                Console.ReadLine();
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

        public void PrintUserPlayCardDeck()
        {
            int i = 0;
            foreach (var card in UserPlayCardStack)
            {
                Console.WriteLine($"Index {i} -> {card.CardElement} {card.CardName}\n Dmg: {card.CardDamage}");
                i++;
            }

        }

        public void PrintUserAllCardDeck()
        {
            int i = 0;
            foreach (var card in UserAllCardStack)
            {
                Console.WriteLine($"Index {i} -> {card.CardElement} {card.CardName}\n Dmg: {card.CardDamage}");
                i++;
            }

        }

        public void PrintUserInformation()
        {
            Console.Clear();

            Console.WriteLine($"Profile:\n Username: {UniqueUsername}\n Elo:      {UserElo}\n Coins:    {Coins}\n Wins:     {Wins}\n Loses:    {Loses}\n Win-%:    {(Loses > 0 ? Wins/Loses : 0)}");
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
                    if (UserPlayCardStack != null)
                    {
                        PrintUserPlayCardDeck();
                        Console.ReadLine();
                    }
                    else
                    {
                        Console.WriteLine($"You have no Deck\nPlease choose a Deck with the 'Change' Command\n");
                        Console.ReadLine();
                    }
                }
                else if (userInput.Equals("Change"))
                {
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
                    if (UserAllCardStack != null)
                    {
                        PrintUserAllCardDeck();
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
    }
}
