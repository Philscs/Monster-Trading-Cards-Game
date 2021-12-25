using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardGame.Enum;
using MonsterTradingCardGame.Interfaces;

namespace MonsterTradingCardGame.Classes
{
    public class Cards : Interfaces.ICards
    {
        public Cards()
        {

        }
        public Cards(int cardID, string name, double cardDamage, CardTypesEnum.CardElementEnum element,
            CardTypesEnum.CardTypeEnum type)
        {
            CardID = cardID;
            CardName = name;
            CardDamage = cardDamage;
            CardElement = element;
            CardType = type;
        }

        public int CardID { get; set; }
        public string CardName { get; set; }
        public double CardDamage { get; set; }
        public CardTypesEnum.CardElementEnum CardElement { get; set; }
        public CardTypesEnum.CardTypeEnum CardType { get; set; }
    }
}
