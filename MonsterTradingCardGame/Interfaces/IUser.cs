using System.Collections.Generic;

namespace MonsterTradingCardGame.Interfaces
{
    public interface IUser
    {
        public string UniqueUsername { get; set; }
        public int Coins { get; set; }
        public int UserElo { get; set; }
        public List<ICardStack> UserCardStack { get; set; }
        //History

    }
}