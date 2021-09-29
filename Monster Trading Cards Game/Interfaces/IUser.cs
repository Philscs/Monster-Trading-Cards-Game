using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Monster_Trading_Cards_Game.Interfaces
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