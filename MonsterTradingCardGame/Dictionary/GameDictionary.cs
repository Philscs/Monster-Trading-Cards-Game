using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame.Dictionary
{
    class GameDictionary
    {
        public Dictionary<string, string> _effectiveGameDictionary = new Dictionary<string, string>();
        public Dictionary<string, string> _notEffectiveGameDictionary = new Dictionary<string, string>();
        public Dictionary<string, string> _notspecialGameDictionary = new Dictionary<string, string>();
        public Dictionary<string, string> _specialGameDictionary = new Dictionary<string, string>();

        public void InitializeAllDictionaries()
        {
            this.InitializeEffectiveDictionary();
            this.InitializeNotEffectiveDictionary();
            this.InitializeNoSpecialtiesDictionary();
            this.InitializeSpecialtiesDictionary();
        }

        private void InitializeEffectiveDictionary()
        {
            this._effectiveGameDictionary.Add("Water", "Fire");
            this._effectiveGameDictionary.Add("Fire", "Normal");
            this._effectiveGameDictionary.Add("Normal", "Water");
        }
        private void InitializeNotEffectiveDictionary()
        {
            this._notEffectiveGameDictionary.Add("Water", "Normal");
            this._notEffectiveGameDictionary.Add("Fire", "Water");
            this._notEffectiveGameDictionary.Add("Normal", "Fire");
        }

        private void InitializeNoSpecialtiesDictionary()
        {
            this._notspecialGameDictionary.Add("Goblin", "Dragon");
            this._notspecialGameDictionary.Add("Orc", "Wizard");
            this._notspecialGameDictionary.Add("Knights", "Water Spell");
            this._notspecialGameDictionary.Add("Spell", "Kraken");
            this._notspecialGameDictionary.Add("Dragon", "Fire Elves");
        }

        private void InitializeSpecialtiesDictionary()
        {
            this._specialGameDictionary.Add("Dragon", "Goblin");
            this._specialGameDictionary.Add("Wizard", "Orc");
            this._specialGameDictionary.Add("Water Spell", "Knights");
            this._specialGameDictionary.Add("Kraken", "Spell"); 
            this._specialGameDictionary.Add("Fire Elves", "Dragon");
        }
    }
}
