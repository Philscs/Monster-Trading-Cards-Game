using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardGame.Interfaces;
using MonsterTradingCardGame.Classes;

namespace MonsterTradingCardGame.Classes
{
    class Battles : IBattles
    {
       public void Battle(IUser user1, IUser user2)
        {
            Console.WriteLine($"implement fight between{user1.UniqueUsername} with Deck");
            user1.PrintUserPlayCardDeck();
            Console.WriteLine($"and {user2.UniqueUsername} with Deck");
            user2.PrintUserPlayCardDeck();

        }
    }
}
