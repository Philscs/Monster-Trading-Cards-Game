using System.Collections.Generic;

namespace MonsterTradingCardGame.Interfaces
{
    public interface IUser
    {
        public string UniqueUsername { get; set; }
        public int Coins { get; set; }
        public int UserElo { get; set; }
        public List<ICards> UserPlayCardStack { get; set; }
        public List<ICards> UserAllCardStack { get; set; }
        //History

    }
}