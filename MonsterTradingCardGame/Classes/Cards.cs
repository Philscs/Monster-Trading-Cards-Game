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
        public Cards()
        {

        }
        public Cards(string name, double cardDamage, CardTypesEnum.CardElementEnum element,
            CardTypesEnum.CardTypeEnum type)
        {
            CardName = name;
            CardDamage = cardDamage;
            CardElement = element;
            CardType = type;
        }

        public string CardName { get; set; }
        public static double CardDamage { get; set; }
        public CardTypesEnum.CardElementEnum CardElement { get; set; }
        public CardTypesEnum.CardTypeEnum CardType { get; set; }
    }
}
