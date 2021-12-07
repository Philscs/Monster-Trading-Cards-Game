using System.Collections.Generic;
using MonsterTradingCardGame.Enum;

namespace MonsterTradingCardGame.Interfaces

{
    public interface ICards
    {
        public string CardName { get; set; }
        public static double CardDamage { get; set; }
        public CardTypesEnum.CardElementEnum CardElement { get; set; }
        public CardTypesEnum.CardTypeEnum CardType { get; set; }
      

    }
}