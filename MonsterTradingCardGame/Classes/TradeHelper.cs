using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardGame.Enum;

namespace MonsterTradingCardGame.Classes
{
    class TradeHelper
    {
        public TradeHelper(User user, Cards card, CardTypesEnum.TradeTypeEnum type, int amnt)
        {
            TradeUser = user;
            TradeCard = card;
            TradeType = type;
            TradeAmnt = amnt;
        }

        public User TradeUser { get; set; }
        public Cards TradeCard { get; set; }
        public CardTypesEnum.TradeTypeEnum TradeType { get; set; }
        public int TradeAmnt { get; set; }
    }
}
