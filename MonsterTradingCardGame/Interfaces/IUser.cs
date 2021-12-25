using System.Collections.Generic;
using MonsterTradingCardGame.Classes;

namespace MonsterTradingCardGame.Interfaces
{
    public interface IUser
    {
        
        public string UniqueUsername { get; set; }
        public int Coins { get; set; }
        public int UserElo { get; set; }
        public string password { get; set; }
        public List<Cards> UserPlayCardStack { get; set; }
        public List<Cards> UserAllCardStack { get; set; }

        public void ChangeUserPlayCardStack();
        public List<Cards> GetUserPlayCardStack();
        public List<Cards> GetUserAllCardStack();
        public void PrintUserPlayCardDeck();
        public void PrintUserInformation();
    }
}