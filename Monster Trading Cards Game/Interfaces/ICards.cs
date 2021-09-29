namespace Monster_Trading_Cards_Game.Interfaces
{
    public interface ICards
    {
        enum CardElementTypeEnum
        {
            Water,
            Fire,
            Normal
        }
        
        public string CardName{ get; set; }
        public static double CardDamage{ get; set; }
        public CardElementTypeEnum CardElementType { get; set; }
    }
}