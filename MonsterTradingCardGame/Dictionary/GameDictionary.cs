using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame.Dictionary
{
    class GameDictionary
    {
        public Dictionary<string, string> EffectiveGameDictionary = new Dictionary<string, string>();
        public Dictionary<string, string> NotEffectiveGameDictionary = new Dictionary<string, string>();
        public Dictionary<string, string> NotSpecialGameDictionary = new Dictionary<string, string>();
        public Dictionary<string, string> SpecialGameDictionary = new Dictionary<string, string>();

        public void InitializeAllDictionaries()
        {
            this.InitializeEffectiveDictionary();
            this.InitializeNotEffectiveDictionary();
            this.InitializeNoSpecialtiesDictionary();
            this.InitializeSpecialtiesDictionary();
        }

        private void InitializeEffectiveDictionary()
        {
            this.EffectiveGameDictionary.Add("Water", "Fire");
            this.EffectiveGameDictionary.Add("Fire", "Normal");
            this.EffectiveGameDictionary.Add("Normal", "Water");
        }
        private void InitializeNotEffectiveDictionary()
        {
            this.NotEffectiveGameDictionary.Add("Water", "Normal");
            this.NotEffectiveGameDictionary.Add("Fire", "Water");
            this.NotEffectiveGameDictionary.Add("Normal", "Fire");
        }

        private void InitializeNoSpecialtiesDictionary()
        {
            this.NotSpecialGameDictionary.Add("Goblin", "Dragon");
            this.NotSpecialGameDictionary.Add("Orc", "Wizard");
            this.NotSpecialGameDictionary.Add("Knights", "Water Spell");
            this.NotSpecialGameDictionary.Add("Spell", "Kraken");
            this.NotSpecialGameDictionary.Add("Dragon", "Fire Elves");


            this.NotSpecialGameDictionary.Add("Wizard", "Cock");
        }

        private void InitializeSpecialtiesDictionary()
        {
            this.SpecialGameDictionary.Add("Dragon", "Goblin");
            this.SpecialGameDictionary.Add("Wizard", "Orc");
            this.SpecialGameDictionary.Add("Water Spell", "Knights");
            this.SpecialGameDictionary.Add("Kraken", "Spell"); 
            this.SpecialGameDictionary.Add("Fire Elves", "Dragon");
        }
    }
}
