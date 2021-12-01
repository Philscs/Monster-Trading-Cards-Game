using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardGame.Enum;
using MonsterTradingCardGame.Interfaces;

namespace MonsterTradingCardGame.Classes
{
    class Cards : ICards
    {
        public static string CardName { get; set; }
        protected static double CardDamage { get; set; }
        protected static CardTypesEnum.CardElementEnum CardElement { get; set; }
        protected static CardTypesEnum.CardTypeEnum CardType { get; set; }

        public int CheckEffectiveness(string element)
        {

            return 0;
        }
    }
}
