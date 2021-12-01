using MonsterTradingCardGame.Enum;

namespace MonsterTradingCardGame.Interfaces

{
    public interface ICards
    {
        public static string CardName { get; set; }
        protected static double CardDamage { get; set; }
        protected static CardTypesEnum.CardElementEnum CardElement { get; set; }
        protected static CardTypesEnum.CardTypeEnum CardType { get; set; }
    }
}