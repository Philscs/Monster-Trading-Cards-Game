using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardGame.Enum;
using MonsterTradingCardGame.Interfaces;
using MonsterTradingCardGame.PostgreDB;
using BCryptNet = BCrypt.Net.BCrypt;


namespace MonsterTradingCardGame.Classes
{
    public class User : IUser
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
            bool exit = true;
            bool checkChange = false;
            string userInput = null;
            string changeInput = null;
            string check = null;
            do
            {
                userInput = null;
                changeInput = null;
                check = null;
                Console.Clear();
                Console.WriteLine($"Profile-Manager\nYou can change here your User-Data.\nWhat do you wanna change:\n Name\n Password\nQuit\nInput:");
                while (userInput == null)
                {
                    userInput = Console.ReadLine();
                }

                if (userInput.Equals("Name"))
                {
                    while (!checkChange)
                    {
                        Console.Clear();
                        Console.WriteLine($"Change the Name\nInput:");
                        while (changeInput == null)
                        {
                            changeInput = Console.ReadLine();
                        }

                        Console.WriteLine($"You chose '{changeInput}' as your new Name.\nChose [Y|n] to confirm or decline your choice...\nInput:");
                        while (check == null)
                        {
                            check = Console.ReadLine();
                        }

                        if (check.Equals("Y") || check.Equals("y"))
                        {
                            checkChange = true;
                            user.UniqueUsername = changeInput;
                            if (db.UpdateUserData(user))
                            {
                                Console.WriteLine($"You changed your Name to -> {user.UniqueUsername}");
                            }
                            else
                            {
                                Console.WriteLine($"Something went wrong!");
                            }
                            user = db.UpdatedUser(user);
                        }
                    }
                }
                else if (userInput.Equals("Password"))
                {
                    while (!checkChange)
                    {
                        Console.Clear();
                        Console.WriteLine($"Password-Manager\nChange the Password\nInput OLD Password:");
                        var oldPass = string.Empty;
                        ConsoleKey key;
                        do
                        {
                            var keyInfo = Console.ReadKey(intercept: true);
                            key = keyInfo.Key;

                            if (key == ConsoleKey.Backspace && oldPass.Length > 0)
                            {
                                Console.Write("\b \b");
                                oldPass = oldPass[0..^1];
                            }
                            else if (!char.IsControl(keyInfo.KeyChar))
                            {
                                Console.Write("*");
                                oldPass += keyInfo.KeyChar;
                            }
                        } while (key != ConsoleKey.Enter);

                        if (!BCryptNet.Verify(oldPass, user.password))
                        {
                            Console.WriteLine($"\nOld Password was wrong!\nTry again!");
                            Console.ReadLine();
                            continue;
                        }

                        Console.WriteLine($"\nEnter your NEW Password:");
                        var firstPass = string.Empty;
                        do
                        {
                            var keyInfo = Console.ReadKey(intercept: true);
                            key = keyInfo.Key;

                            if (key == ConsoleKey.Backspace && firstPass.Length > 0)
                            {
                                Console.Write("\b \b");
                                firstPass = firstPass[0..^1];
                            }
                            else if (!char.IsControl(keyInfo.KeyChar))
                            {
                                Console.Write("*");
                                firstPass += keyInfo.KeyChar;
                            }
                        } while (key != ConsoleKey.Enter);

                        Console.WriteLine($"\nChose [Y|n] to confirm or decline your choice...\nInput:");
                        while (check == null)
                        {
                            check = Console.ReadLine();
                        }

                        if (check.Equals("Y") || check.Equals("y"))
                        {
                            checkChange = true;
                            user.password = BCryptNet.HashPassword(firstPass);
                            if (db.UpdateUserData(user))
                            {
                                Console.WriteLine($"You successfully changed your Password.");
                            }
                            else
                            {
                                Console.WriteLine($"Something went wrong!");
                            }
                            user = db.UpdatedUser(user);
                        }
                    }
                }
                else if (userInput.Equals("Quit"))
                {
                    Console.Clear();
                    exit = false;
                    Console.WriteLine($"{UniqueUsername} quit Profile-Manager.");
                    return user;
                }
                else
                {
                    Console.WriteLine("Wrong Input!\nPress any key to try again...\n");
                    Console.ReadLine();
                }
                Console.Clear();
                return user;
            } while (exit);
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
            const int cost = 5;
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
                    Console.ReadLine();
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
                        db.ReduceUserCoins(this, cost);
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
                        db.ReduceUserCoins(this, cost);
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
                        db.ReduceUserCoins(this, cost);
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
                        db.ReduceUserCoins(this, cost);
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
                        db.ReduceUserCoins(this, cost);
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
            string userInput = null;
            bool exit = true;

            do
            {
                userInput = null;
                Console.Clear();
                Console.WriteLine($"Trade-Manager\nYou can offer a Trade for a Card which is not in your Deck or check out the Trades of other User");


                Console.WriteLine("Choose:\n Make   -> create a new Trade-offer for a Card or Coins\n Show   -> list the Offers other Users created and get yourself some new Cards\n Quit   -> quit the Trade & go back to the shop\nInput:");
                while (userInput == null)
                {
                    userInput = Console.ReadLine();
                }

                if (userInput.Equals("Quit"))
                {
                    Console.Clear();
                    Console.WriteLine($"{UniqueUsername} quit Trade-Manager.");
                    Console.ReadLine();
                    exit = true;
                    return user;
                }

                if (userInput.Equals("Make"))
                {
                    user = MakeTrade(this);
                }
                else if (userInput.Equals("Show"))
                {
                    user = ShowTrades(this);
                }
                else
                {
                    Console.WriteLine("Wrong Input!\nPress any key to try again...\n");
                    Console.ReadLine();
                    continue;
                }

                user = db.UpdatedUser(this);
            } while (exit);
            return user;
        }

        public User MakeTrade(User user)
        {
            string userInput = null;
            bool exit = true;

            List<Cards> tradeableCardsList = db.TradeAbleCards(this);
            if (tradeableCardsList.Count <= 0)
            {
                Console.WriteLine($"You have no Cards to Trade!");
                Console.ReadLine();
                return user;
            }
            do
            {
                userInput = null;
                Console.Clear();
                Console.WriteLine($"Trade-Offer-Manager\nYou can offer a Trade for a Card which is not in your Play-Deck");

                Console.WriteLine($"Choose a Card to trade by its Index!");
                PrintAllCardDeck(tradeableCardsList);
                Console.Write("Index:");
                string chosenIndex = Console.ReadLine();
                int chosenCard = -1;
                int cardID = -1;
                if (Int32.TryParse(chosenIndex, out chosenCard))
                {
                    if (chosenCard < 0 || chosenCard > tradeableCardsList.Count)
                    {
                        Console.WriteLine("Wrong Index!\nTry again...");
                        chosenCard = -1;
                    }

                    cardID = tradeableCardsList[chosenCard].CardID;
                }
                else
                {
                    Console.WriteLine($"Only Numeric Inputs in the Range of the indices");
                    Console.ReadLine();
                    continue;
                }

                Console.WriteLine("Choose what do you want to trade your card for\n Spell  -> a Spell Card where you can ask for a min Dmg\n Monster-> a Monster Card where you can ask for a min Dmg\n Coins  -> a chosen amount of Coins\n Quit   -> quit the Trade & go back to the shop\nInput:");
                while (userInput == null)
                {
                    userInput = Console.ReadLine();
                }

                if (userInput.Equals("Quit"))
                {
                    Console.Clear();
                    Console.WriteLine($"{UniqueUsername} quit Trade-Offer-Manager.");
                    Console.ReadLine();
                    exit = true;
                    return user;
                }

                int tradeType = -1;
                int toTradeAmt = -1;
                if (userInput.Equals("Spell"))
                {
                    tradeType = (int)CardTypesEnum.TradeTypeEnum.Spell;
                }
                else if (userInput.Equals("Monster"))
                {
                    tradeType = (int)CardTypesEnum.TradeTypeEnum.Monster;
                }
                else if (userInput.Equals("Coins"))
                {
                    tradeType = (int)CardTypesEnum.TradeTypeEnum.Coins;
                }
                else
                {
                    Console.WriteLine("Wrong Input!\nPress any key to try again...\n");
                    Console.ReadLine();
                    continue;
                }

                if (userInput.Equals("Spell") || userInput.Equals("Monster"))
                {
                    Console.WriteLine($"What is the minimal Damage a Card has to have for the Trade to be successful");
                }
                else
                {
                    Console.WriteLine($"What is the Price of the Card in Coins you want for the Trade to be successful");
                }

                
                while (toTradeAmt <= 0)
                {
                    Console.Write($"Input:");
                    Int32.TryParse(Console.ReadLine(), out toTradeAmt);
                    if (toTradeAmt <= 0)
                    {
                        Console.WriteLine($"Your Value is invalid!\nTry again with a numeric Value > 0...\n");
                    }
                }

                if (chosenCard >= 0 && tradeType is >= 0 and <= 3 && toTradeAmt > 0)
                {
                    db.UpdateTrade(this, cardID, true);
                    db.CreateCardTrade(this, cardID, tradeType, toTradeAmt);
                    exit = false;
                    break;
                }

                user = db.UpdatedUser(this);
            } while (exit);
            return user;
        }
        
        public User ShowTrades(User user)
        {
            user = db.UpdatedUser(this);
            bool exit = true;

            List<TradeHelper> tradeableCardsList = db.GetAllTrades(this);
            if (tradeableCardsList.Count <= 0)
            {
                Console.WriteLine($"There are NO Trades available!");
                Console.ReadLine();
                return user;
            }
            do
            {
                Console.Clear();
                Console.WriteLine($"All-Trades\nCheck out the Trades of other User");

                Console.WriteLine($"Choose a Card to trade by its Index!");
                int index = 0;
                foreach (var helper in tradeableCardsList)
                {
                    Console.WriteLine($"Index {index} -> {helper.TradeCard.CardType.ToString()}: {helper.TradeCard.CardElement.ToString()} {helper.TradeCard.CardName} is trade-able for " + (((int)helper.TradeType < 2) ? $"a {helper.TradeType.ToString()} with at least {helper.TradeAmnt} Damage." : $"{helper.TradeAmnt} {helper.TradeType.ToString()}."));
                    index++;
                }
                Console.Write("Index:");
                string chosenIndex = Console.ReadLine();
                int chosenCard = -1;
                if (Int32.TryParse(chosenIndex, out chosenCard))
                {
                    if (chosenCard < 0 || chosenCard > tradeableCardsList.Count)
                    {
                        Console.WriteLine("Wrong Index!\nTry again...");
                        chosenCard = -1;
                        continue;
                    }
                }
                else
                {
                    Console.WriteLine($"Only Numeric Inputs in the Range of the indices");
                    Console.ReadLine();
                    continue;
                }

                string checkInput = null;
                Console.WriteLine($"You chose the Card {tradeableCardsList[chosenCard].TradeCard.CardName} at Index {chosenCard}.\nChose [Y|n] to confirm or decline your choice...\nInput:");
                while (checkInput == null)
                {
                    checkInput = Console.ReadLine();
                }

                if (!checkInput.Equals("Y") && !checkInput.Equals("y"))
                {
                    Console.WriteLine($"You dismissed the Trade!");
                    Console.ReadLine();
                    exit = true;
                    return user;
                }

                if ((int) tradeableCardsList[chosenCard].TradeType == 2)
                {
                    if (db.CheckCoins(this) > tradeableCardsList[chosenCard].TradeAmnt)
                    {
                        Console.WriteLine($"You have {db.CheckCoins(this)} Coins, do you want to buy the {tradeableCardsList[chosenCard].TradeCard.CardType.ToString()} {tradeableCardsList[chosenCard].TradeCard.CardElement.ToString()} {tradeableCardsList[chosenCard].TradeCard.CardName} for {tradeableCardsList[chosenCard].TradeAmnt} Coins?\nChose [Y|n] to confirm or decline your choice...\nInput:");
                        string confirm = null;
                        while (confirm == null)
                        {
                            confirm = Console.ReadLine();
                        }

                        if (!confirm.Equals("Y") && !confirm.Equals("y"))
                        {
                            Console.WriteLine($"You dismissed the Trade!");
                            Console.ReadLine();
                            exit = true;
                            return user;
                        }
                       
                        db.ConfirmedCoinTrade(this, tradeableCardsList[chosenCard].TradeUser.UserID, tradeableCardsList[chosenCard].TradeCard.CardID);
                        db.ReduceUserCoins(this, tradeableCardsList[chosenCard].TradeAmnt);
                        db.IncreaseUserCoins(tradeableCardsList[chosenCard].TradeUser, tradeableCardsList[chosenCard].TradeAmnt);
                        db.DeleteTradeOffer(tradeableCardsList[chosenCard]);
                        user = db.UpdatedUser(this);
                        return user;
                    }
                    else
                    {
                        Console.WriteLine($"You do not have enough Coins ​​for that offer!");
                        Console.ReadLine();
                        return user;
                    }
                }


                if((int)tradeableCardsList[chosenCard].TradeType == 0 || (int)tradeableCardsList[chosenCard].TradeType == 1)
                {
                    List<Cards> toTradeCards = db.GetToTradeCards(tradeableCardsList[chosenCard], this);
                    if (toTradeCards.Count <= 0)
                    {
                        Console.WriteLine($"You do not have a card with the appropriate values ​​for the offer!");
                        Console.ReadLine();
                        return user;
                    }

                    PrintAllCardDeck(toTradeCards);

                    int chosenTradeCard = -1;
                    while (chosenTradeCard == -1)
                    {
                        Console.Write("Index:");
                        string choseTradeCardInput = Console.ReadLine();
                        if (Int32.TryParse(choseTradeCardInput, out chosenTradeCard))
                        {
                            if (chosenCard < 0 || chosenCard > tradeableCardsList.Count)
                            {
                                Console.WriteLine("Wrong Index!\nTry again...");
                                Console.ReadLine();
                                chosenCard = -1;
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Only Numeric Inputs in the Range of the indices");
                            Console.ReadLine();
                        }
                    }

                    Console.WriteLine($"You have chosen a {toTradeCards[chosenTradeCard].CardElement.ToString()} {toTradeCards[chosenTradeCard].CardName} with {toTradeCards[chosenTradeCard].CardDamage} Damage, for a Trade where {tradeableCardsList[chosenCard].TradeAmnt} was the required Damage.\nChose [Y|n] to confirm or decline your choice...\nInput:");
                    string confirm = null;
                    while (confirm == null)
                    {
                        confirm = Console.ReadLine();
                    }

                    if (!confirm.Equals("Y") && !confirm.Equals("y"))
                    {
                        Console.WriteLine($"You dismissed the Trade!");
                        Console.ReadLine();
                        exit = true;
                        return user;
                    }

                    db.ConfirmedCardTrade(this, tradeableCardsList[chosenCard].TradeUser, toTradeCards[chosenTradeCard].CardID, tradeableCardsList[chosenCard].TradeCard.CardID);
                    db.DeleteTradeOffer(tradeableCardsList[chosenCard]);
                    user = db.UpdatedUser(this);
                    return user;
                }

                user = db.UpdatedUser(this);
            } while (exit);
            return user;

        }
    }
}
