using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardGame.Interfaces;
using MonsterTradingCardGame.ToTestFunctions;

namespace MonsterTradingCardGame.Classes
{
    class User : IUser
    {
        public User()
        {

        }
        public User(string unique, int coins, int userElo, string userPassword)
        {
            UniqueUsername = unique;
            Coins = coins;
            UserElo = userElo;
            password = userPassword;
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
        //only for testing
        //Register User
        public User(string unique, int coins, int userElo,string userPassword, int Register)
        {
            UniqueUsername = unique;
            Coins = coins;
            UserElo = userElo;
            password = userPassword;
            UserPlayCardStack = null;
            UserAllCardStack = null;

        }

        //(AI User)
        public User(string name)
        {
            UniqueUsername = name;
            Coins = 0;
            UserElo = 1000;
            UserPlayCardStack = GetAiCardStack();
            UserAllCardStack = GetUserAllCardStack();
        }
        public string UniqueUsername { get; set; }
        public int Coins { get; set; }
        public int UserElo { get; set; }
        public string password { get; set; }

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
            CardTests cardTests = new CardTests();

            cardTests.InitializeCards();

            List<Cards> tempList = new List<Cards>
            {
                cardTests.GetCard(1),
                cardTests.GetCard(23),
                cardTests.GetCard(20),
                cardTests.GetCard(9)
            };
            return tempList;
        } 
        //only for Testing, that AI Player has Cards!!
        public List<Cards> GetAiCardStack()
        {
            CardTests cardTests = new CardTests();

            cardTests.InitializeCards();

            List<Cards> tempList = new List<Cards>
            {
                cardTests.GetCard(23),
                cardTests.GetCard(22),
                cardTests.GetCard(13),
                cardTests.GetCard(11)
            };
            return tempList;
        }
        public List<Cards> GetUserAllCardStack()
        {
            CardTests cardTests = new CardTests();

            cardTests.InitializeCards();

            return cardTests.TestCardsList;
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
            Console.WriteLine($"{UniqueUsername} is logged in with an Elo of {UserElo}...");
            Console.WriteLine($"{UniqueUsername} has {Coins} Coins...");
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
