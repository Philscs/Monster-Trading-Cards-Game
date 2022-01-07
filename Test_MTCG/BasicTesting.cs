using System.Collections.Generic;
using MonsterTradingCardGame.Classes;
using MonsterTradingCardGame.Enum;
using MonsterTradingCardGame.Interfaces;
using Moq;
using NUnit.Framework;

namespace Test_MTCG
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        [TestCase(TestName = "Test Basic User-functions", Description =
            "Test if Name, Elo and Cards are correct in input and type")]
        public void BasicUserTests()
        {
            var playerAMock = new Mock<IUser>();
            playerAMock.SetupGet(player => player.UniqueUsername).Returns("A");
            playerAMock.SetupGet(player => player.UserElo).Returns(1);
            var playerA = playerAMock.Object;

            Assert.AreEqual(playerA.UniqueUsername, "A");
            Assert.AreEqual(typeof(int), playerA.UserElo.GetType());
            Assert.IsFalse(playerA.UserElo != 1);
        }
        [Test]
        [TestCase(TestName = "Test Functions for List of Cards", Description =
            "Test some Basic Functions from the List<Cards>")]
        public void BasicCardListsTest()
        {
            List<Cards> cardsList = new List<Cards>();
            Cards monsterOne = new Cards(-1, "Gandalf", 222, CardTypesEnum.CardElementEnum.Normal, CardTypesEnum.CardTypeEnum.Monster);
            Cards monsterTwo = new Cards(-1, "Balrog", 111, CardTypesEnum.CardElementEnum.Fire, CardTypesEnum.CardTypeEnum.Monster);
            Cards spellOne = new Cards(-1, "WaterBall", 95, CardTypesEnum.CardElementEnum.Water, CardTypesEnum.CardTypeEnum.Spell);
            Cards spellTwo = new Cards(-1, "StoneDrop", 95, CardTypesEnum.CardElementEnum.Normal, CardTypesEnum.CardTypeEnum.Spell);
            cardsList.Add(monsterOne);
            cardsList.Add(monsterTwo);
            cardsList.Add(spellOne);
            cardsList.Add(spellTwo);

            Assert.AreEqual(cardsList.Count,4);
        }

        [Test]
        [TestCase(TestName = "Check User String Functions", Description =
            "Check if Functions return correct string")]
        public void BasicUserStrings()
        {
            User user = new User();

            Assert.AreEqual(user.NoSearch(), "Your Search was invalid, nothing found!");
        }

        [Test]
        [TestCase(TestName = "Check Menu String Functions", Description =
            "Check if Functions return correct string")]
        public void BasicMenuStrings()
        {
            Menu menu = new Menu();

            Assert.AreEqual(menu.NoUser(), "No User is logged in, please Login first!");
        }

        [Test]
        [TestCase(TestName = "Check effective Dmg Multiplication for SpellNW", Description =
            "Check Dmg Multiplication between Spells between Normal and Water")]
        public void CheckDmgMultiplicationNW()
        {
            Battles battles = new Battles();


            Cards spellOne = new Cards(-1, "Gandalf", 222, CardTypesEnum.CardElementEnum.Normal, CardTypesEnum.CardTypeEnum.Spell);
            Cards spellTwo = new Cards(-1, "Hanfdalf", 222, CardTypesEnum.CardElementEnum.Water, CardTypesEnum.CardTypeEnum.Spell);

            Assert.AreEqual(2,battles.CheckDmgMulti(spellOne, spellTwo));
        }

        [Test]
        [TestCase(TestName = "Check effective Dmg Multiplication for SpellWF", Description =
            "Check Dmg Multiplication between Spells between Water and Fire")]
        public void CheckDmgMultiplicationWF()
        {
            Battles battles = new Battles();


            Cards spellOne = new Cards(-1, "Gandalf", 222, CardTypesEnum.CardElementEnum.Water, CardTypesEnum.CardTypeEnum.Spell);
            Cards spellTwo = new Cards(-1, "Hanfdalf", 222, CardTypesEnum.CardElementEnum.Fire, CardTypesEnum.CardTypeEnum.Spell);

            Assert.AreEqual(2, battles.CheckDmgMulti(spellOne, spellTwo));
        }

        [Test]
        [TestCase(TestName = "Check effective Dmg Multiplication for SpellFN", Description =
            "Check Dmg Multiplication between Spells between Fire and Normal")]
        public void CheckDmgMultiplicationFN()
        {
            Battles battles = new Battles();


            Cards spellOne = new Cards(-1, "Gandalf", 222, CardTypesEnum.CardElementEnum.Fire, CardTypesEnum.CardTypeEnum.Spell);
            Cards spellTwo = new Cards(-1, "Hanfdalf", 222, CardTypesEnum.CardElementEnum.Normal, CardTypesEnum.CardTypeEnum.Spell);

            Assert.AreEqual(2, battles.CheckDmgMulti(spellOne, spellTwo));
        }
        [Test]
        [TestCase(TestName = "Check not effective Dmg Multiplication for SpellNW", Description =
            "Check Dmg Multiplication between Spells between Normal and Water")]
        public void CheckNEDmgMultiplicationNW()
        {
            Battles battles = new Battles();


            Cards spellOne = new Cards(-1, "Gandalf", 222, CardTypesEnum.CardElementEnum.Normal, CardTypesEnum.CardTypeEnum.Spell);
            Cards spellTwo = new Cards(-1, "Hanfdalf", 222, CardTypesEnum.CardElementEnum.Water, CardTypesEnum.CardTypeEnum.Spell);

            Assert.AreEqual(0.5, battles.CheckDmgMulti(spellTwo,spellOne));
        }

        [Test]
        [TestCase(TestName = "Check not effective Dmg Multiplication for SpellWF", Description =
            "Check Dmg Multiplication between Spells between Water and Fire")]
        public void CheckNEDmgMultiplicationWF()
        {
            Battles battles = new Battles();


            Cards spellOne = new Cards(-1, "Gandalf", 222, CardTypesEnum.CardElementEnum.Water, CardTypesEnum.CardTypeEnum.Spell);
            Cards spellTwo = new Cards(-1, "Hanfdalf", 222, CardTypesEnum.CardElementEnum.Fire, CardTypesEnum.CardTypeEnum.Spell);

            Assert.AreEqual(0.5, battles.CheckDmgMulti(spellTwo, spellOne));
        }

        [Test]
        [TestCase(TestName = "Check not effective Dmg Multiplication for SpellFN", Description =
            "Check Dmg Multiplication between Spells between Fire and Normal")]
        public void CheckNEDmgMultiplicationFN()
        {
            Battles battles = new Battles();


            Cards spellOne = new Cards(-1, "Gandalf", 222, CardTypesEnum.CardElementEnum.Fire, CardTypesEnum.CardTypeEnum.Spell);
            Cards spellTwo = new Cards(-1, "Hanfdalf", 222, CardTypesEnum.CardElementEnum.Normal, CardTypesEnum.CardTypeEnum.Spell);

            Assert.AreEqual(0.5, battles.CheckDmgMulti(spellTwo, spellOne));
        }

        [Test]
        [TestCase(TestName = "Check no Dmg Multiplication ", Description =
            "Check Dmg Multiplication between Spells between Fire and Normal")]
        public void CheckNoDmgMultiplication()
        {
            Battles battles = new Battles();


            Cards spellOne = new Cards(-1, "Gandalf", 222, CardTypesEnum.CardElementEnum.Fire, CardTypesEnum.CardTypeEnum.Monster);
            Cards spellTwo = new Cards(-1, "Hanfdalf", 222, CardTypesEnum.CardElementEnum.Normal, CardTypesEnum.CardTypeEnum.Monster);

            Assert.AreEqual(1, battles.CheckDmgMulti(spellTwo, spellOne));
        }

        [Test]
        [TestCase(TestName = "Check special Dmg Multiplication for GD", Description =
            "Check Dmg Multiplication between Spells between Goblin and Dragon")]
        public void CheckSpcDmgMultiplicationGD()
        {
            Battles battles = new Battles();


            Cards spellOne = new Cards(-1, "Goblin", 222, CardTypesEnum.CardElementEnum.Normal, CardTypesEnum.CardTypeEnum.Monster);
            Cards spellTwo = new Cards(-1, "Dragon", 222, CardTypesEnum.CardElementEnum.Water, CardTypesEnum.CardTypeEnum.Monster);

            Assert.AreEqual(0, battles.CheckDmgMulti(spellOne, spellTwo));
        }

        [Test]
        [TestCase(TestName = "Check special Dmg Multiplication for OW", Description =
            "Check Dmg Multiplication between Spells between Orc and Wizard")]
        public void CheckSpcDmgMultiplicationOW()
        {
            Battles battles = new Battles();


            Cards spellOne = new Cards(-1, "Orc", 222, CardTypesEnum.CardElementEnum.Normal, CardTypesEnum.CardTypeEnum.Monster);
            Cards spellTwo = new Cards(-1, "Wizard", 222, CardTypesEnum.CardElementEnum.Water, CardTypesEnum.CardTypeEnum.Monster);

            Assert.AreEqual(0, battles.CheckDmgMulti(spellOne, spellTwo));
        }

        [Test]
        [TestCase(TestName = "Check special Dmg Multiplication for KWS", Description =
            "Check Dmg Multiplication between Spells between Knights and Water Spell")]
        public void CheckSpcDmgMultiplicationKWS()
        {
            Battles battles = new Battles();


            Cards spellOne = new Cards(-1, "Knights", 222, CardTypesEnum.CardElementEnum.Normal, CardTypesEnum.CardTypeEnum.Monster);
            Cards spellTwo = new Cards(-1, "Spell", 222, CardTypesEnum.CardElementEnum.Water, CardTypesEnum.CardTypeEnum.Spell);

            Assert.AreEqual(0, battles.CheckDmgMulti(spellOne, spellTwo));
        }

        [Test]
        [TestCase(TestName = "Check special Dmg Multiplication for SK", Description =
            "Check Dmg Multiplication between Spells between Spell and Kraken")]
        public void CheckSpcDmgMultiplicationSK()
        {
            Battles battles = new Battles();


            Cards spellOne = new Cards(-1, "Kraken", 222, CardTypesEnum.CardElementEnum.Normal, CardTypesEnum.CardTypeEnum.Monster);
            Cards spellTwo = new Cards(-1, "Spell", 222, CardTypesEnum.CardElementEnum.Water, CardTypesEnum.CardTypeEnum.Spell);

            Assert.AreEqual(0, battles.CheckDmgMulti(spellTwo, spellOne));
        }

        [Test]
        [TestCase(TestName = "Check special Dmg Multiplication for DFE", Description =
            "Check Dmg Multiplication between Spells between Dragon and Fire Elves")]
        public void CheckSpcDmgMultiplicationDFE()
        {
            Battles battles = new Battles();


            Cards spellOne = new Cards(-1, "Dragon", 222, CardTypesEnum.CardElementEnum.Normal, CardTypesEnum.CardTypeEnum.Monster);
            Cards spellTwo = new Cards(-1, "Elves", 222, CardTypesEnum.CardElementEnum.Fire, CardTypesEnum.CardTypeEnum.Monster);

            Assert.AreEqual(0, battles.CheckDmgMulti(spellOne, spellTwo));
        }


    }
}