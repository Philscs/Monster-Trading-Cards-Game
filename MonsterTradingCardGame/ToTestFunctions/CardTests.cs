using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardGame.Classes;
using MonsterTradingCardGame.Enum;
using MonsterTradingCardGame.Interfaces;

namespace MonsterTradingCardGame.ToTestFunctions
{
    class CardTests : Cards
    {
        public List<Cards> TestCardsList = new List<Cards>();

        public CardTests() : base()
        {

        }
        public CardTests(string name, double cardDamage, CardTypesEnum.CardElementEnum element,
            CardTypesEnum.CardTypeEnum type) : base(name, cardDamage, element, type)
        {

        }

        public void RemoveCardFromList(int index)
        {
            TestCardsList.RemoveAt(index);
        }

        public void PrintAllTestCards()
        {
            int i = 0;
            foreach (var card in TestCardsList)
            {
                Console.WriteLine($"Index{i} -> {card.CardElement} {card.CardName}");
                i++;
            }
        }

        public Cards GetCard(int cardNumber)
        {
            return TestCardsList.ElementAt(cardNumber);
        }

        public void InitializeCards()
        {
            CardTests card1 = new CardTests("Goblin", 75, CardTypesEnum.CardElementEnum.Water,
                CardTypesEnum.CardTypeEnum.Monster);
            TestCardsList.Add(card1);
            CardTests card2 = new CardTests("Goblin", 100, CardTypesEnum.CardElementEnum.Fire,
                CardTypesEnum.CardTypeEnum.Monster);
            TestCardsList.Add(card2);
            CardTests card3 = new CardTests("Goblin", 110, CardTypesEnum.CardElementEnum.Normal,
                CardTypesEnum.CardTypeEnum.Monster);
            TestCardsList.Add(card3);
            CardTests card4 = new CardTests("Troll", 93, CardTypesEnum.CardElementEnum.Water,
                CardTypesEnum.CardTypeEnum.Monster);
            TestCardsList.Add(card4);
            CardTests card5 = new CardTests("Troll", 84, CardTypesEnum.CardElementEnum.Fire,
                CardTypesEnum.CardTypeEnum.Monster);
            TestCardsList.Add(card5);
            CardTests card6 = new CardTests("Troll", 18, CardTypesEnum.CardElementEnum.Normal,
                CardTypesEnum.CardTypeEnum.Monster);
            TestCardsList.Add(card6);
            CardTests card7 = new CardTests("Dragon", 99, CardTypesEnum.CardElementEnum.Water,
                CardTypesEnum.CardTypeEnum.Monster);
            TestCardsList.Add(card7);
            CardTests card8 = new CardTests("Dragon", 115, CardTypesEnum.CardElementEnum.Fire,
                CardTypesEnum.CardTypeEnum.Monster);
            TestCardsList.Add(card8);
            CardTests card9 = new CardTests("Dragon", 103, CardTypesEnum.CardElementEnum.Normal,
                CardTypesEnum.CardTypeEnum.Monster);
            TestCardsList.Add(card9);
            CardTests card10 = new CardTests("Wizard", 88, CardTypesEnum.CardElementEnum.Water,
                CardTypesEnum.CardTypeEnum.Monster);
            TestCardsList.Add(card10);
            CardTests card11 = new CardTests("Wizard", 99, CardTypesEnum.CardElementEnum.Fire,
                CardTypesEnum.CardTypeEnum.Monster);
            TestCardsList.Add(card11);
            CardTests card12 = new CardTests("Wizard", 111, CardTypesEnum.CardElementEnum.Normal,
                CardTypesEnum.CardTypeEnum.Monster);
            TestCardsList.Add(card12);
            CardTests card13 = new CardTests("Orc", 98, CardTypesEnum.CardElementEnum.Water,
                CardTypesEnum.CardTypeEnum.Monster);
            TestCardsList.Add(card13);
            CardTests card14 = new CardTests("Orc", 56, CardTypesEnum.CardElementEnum.Fire,
                CardTypesEnum.CardTypeEnum.Monster);
            TestCardsList.Add(card14);
            CardTests card15 = new CardTests("Orc", 107, CardTypesEnum.CardElementEnum.Normal,
                CardTypesEnum.CardTypeEnum.Monster);
            TestCardsList.Add(card15);
            CardTests card16 = new CardTests("Elves", 123, CardTypesEnum.CardElementEnum.Water,
                CardTypesEnum.CardTypeEnum.Monster);
            TestCardsList.Add(card16);
            CardTests card17 = new CardTests("Elves", 154, CardTypesEnum.CardElementEnum.Fire,
                CardTypesEnum.CardTypeEnum.Monster);
            TestCardsList.Add(card17);
            CardTests card18 = new CardTests("Elves", 187, CardTypesEnum.CardElementEnum.Normal,
                CardTypesEnum.CardTypeEnum.Monster);
            TestCardsList.Add(card18);
            CardTests card19 = new CardTests("Kraken", 210, CardTypesEnum.CardElementEnum.Water,
                CardTypesEnum.CardTypeEnum.Monster);
            TestCardsList.Add(card19);
            CardTests card20 = new CardTests("Knight", 195, CardTypesEnum.CardElementEnum.Normal,
                CardTypesEnum.CardTypeEnum.Monster);
            TestCardsList.Add(card20);
            CardTests card21 = new CardTests("UndeadKnight", 666, CardTypesEnum.CardElementEnum.Fire,
                CardTypesEnum.CardTypeEnum.Monster);
            TestCardsList.Add(card21);
            CardTests card22 = new CardTests("Spell", 100, CardTypesEnum.CardElementEnum.Water,
                CardTypesEnum.CardTypeEnum.Spell);
            TestCardsList.Add(card22);
            CardTests card23 = new CardTests("Spell", 100, CardTypesEnum.CardElementEnum.Fire,
                CardTypesEnum.CardTypeEnum.Spell);
            TestCardsList.Add(card23);
            CardTests card24 = new CardTests("Spell", 100, CardTypesEnum.CardElementEnum.Normal,
                CardTypesEnum.CardTypeEnum.Spell);
            TestCardsList.Add(card24);
        }
    }
}
