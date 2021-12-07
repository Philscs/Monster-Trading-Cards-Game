using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardGame.Interfaces;
using MonsterTradingCardGame.ToTestFunctions;

namespace MonsterTradingCardGame.Classes
{
    class User : IUser
    {
        public User(string unique, int coins, int userElo)
        {
            UniqueUsername = unique;
            Coins = coins;
            UserElo = userElo;
            UserPlayCardStack = GetUserPlayCardStack();
            UserAllCardStack = GetUserAllCardStack();
        }
        public string UniqueUsername { get; set; }
        public int Coins { get; set; }
        public int UserElo { get; set; }
        public List<ICards> UserPlayCardStack { get; set; }
        public List<ICards> UserAllCardStack { get; set; }

        public void ChangeUserPlayCardStack()
        {
            UserPlayCardStack.Clear();
            Cards cards = new Cards();
            int chosenCard = 0;
            int cardCount = 0;
            //get Cards from DB not from TestCards
            CardTests testcard = new CardTests();
            do
            {
                //print all Cards for the User to choose from
                testcard.PrintAllTestCards();
                while (chosenCard == 0)
                {
                    string input = Console.ReadLine();
                    Int32.TryParse(input, out chosenCard);
                }

                cardCount++;

                cards = testcard.GetCard(chosenCard);
                UserPlayCardStack.Add(cards);
                testcard.RemoveCardFromList(chosenCard);

            } while (cardCount < 5);

        }
        public List<ICards> GetUserPlayCardStack()
        {
            return /*cardstack*/;
        }
        public List<ICards> GetUserAllCardStack()
        {
            return /*cardstack*/;
        }
    }
}
