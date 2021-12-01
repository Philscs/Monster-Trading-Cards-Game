using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame.Dictionary
{
    class GameDictionary
    {
        private Dictionary<string, string> _effectiveGameDictionary = new Dictionary<string, string>();
        private Dictionary<string, string> _notEffectiveGameDictionary = new Dictionary<string, string>();
        private Dictionary<string, string> _specialGameDictionary = new Dictionary<string, string>();

        public void InitializeAllDictionaries()
        {
            this.InitializeEffectiveDictionary();
            this.InitializeNotEffectiveDictionary();
            this.InitializeSpecialtiesDictionary();
        }

        private void InitializeEffectiveDictionary()
        {
            //TypeDic
            this._effectiveGameDictionary.Add("Water", "Fire");
            this._effectiveGameDictionary.Add("Fire", "Normal");
            this._effectiveGameDictionary.Add("Normal", "Water");

            //MonsterDic
        }
        private void InitializeNotEffectiveDictionary()
        {
            this._notEffectiveGameDictionary.Add("Water", "Normal");
            this._notEffectiveGameDictionary.Add("Fire", "Water");
            this._notEffectiveGameDictionary.Add("Normal", "Fire");
        }

        private void InitializeSpecialtiesDictionary()
        {
            this._specialGameDictionary.Add("Dragon", "Goblin");
        }

        public int CheckEffectiveGameDictionary(string element, string toCheck)
        {
            if(!String.Equals(this._effectiveGameDictionary[element], (toCheck)))
            {
                return 0;
            }

            return 1;
        }
    }
}
