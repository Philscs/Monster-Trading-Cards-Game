using MonsterTradingCardGame.Enum;

namespace MonsterTradingCardGame.Interfaces

{
    public interface ICards
    {
        public string CardName { get; set; }
        public static double CardDamage { get; set; }
        public CardTypesEnum.CardElementTypeEnum CardElementType { get; set; }
    }
}