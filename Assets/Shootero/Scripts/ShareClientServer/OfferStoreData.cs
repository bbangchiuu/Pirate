using DateTime = System.DateTime;
using System.Collections.Generic;

namespace Hiker.Networks.Data.Shootero
{
    public enum OfferType
    {
        GemOffer = 0,
        DailyGemPack = 1,
        FlashSale = 2,
        DailyOffer = 3,
        WeeklyOffer = 4,
        MonthlyOffer = 5,
        LimitedTime = 6,
        HeroOffer = 7,
        PremiumPack = 8,
        TargetOffer = 9,
        TruongThanhOffer = 10,
        TetEventOffer = 11,
    }

    public class OfferStoreData
    {
        public string ID;
        public long GID;
        public string PackageName;
        public short BonusRate;
        public int BuyCount;
        public int StockCount;
        //public int Cost;
        //public int Value;
        public OfferType Type;
        public int AllTimeBuyCount;
        public DateTime ExpireTime;

        public int GetCost() { return GetConfig().Cost; }

        public OfferStoreConfig config;

        public OfferStoreConfig GetConfig()
        {
            if (config != null)
            {
                return config;
            }
            else
            {
                return ConfigManager.GetOfferConfig(PackageName, Type);
            }
        }
    }

    public class OfferStoreConfig
    {
        public GeneralReward Content;
        public int MinuteOnStock;
        public string StoreId;
        public Dictionary<string, string> Names;
        public Dictionary<string, string> Promo;
        public int PromoSale;
        public ERarity PromoRarity;
        public string Price;
        public int Cost;
        public short Bonus;
        public int Stock;
        public int RequireHQLvl;
        public bool IsNonConsum;

        /// <summary>
        /// Dieu kien kich hoat FlashSale Offer
        /// Dictionary<FlashSaleConditionType, Param> 
        /// </summary>
        public Dictionary<string, string> FlashSaleCondition;
        public enum FlashSaleConditionType
        {
            CLEAR_CHAP,
            LOSE_BATTLE
        }

        public string GetName(string languague, string packageName)
        {
            if (Names != null && Names.Count > 0)
            {
                if (Names.ContainsKey(languague))
                {
                    return Names[languague];
                }
                else
                {
                    // return first available
                    var enumerator = Names.GetEnumerator();
                    enumerator.MoveNext(); // fix not show first line but null string
                    return enumerator.Current.Value;
                }
            }

#if SERVER_CODE
            return Localization.Get(string.Format("Offer_{0}_name", packageName), languague);
#else
            return Localization.Get(string.Format("Offer_{0}_name", packageName));
#endif
        }

        public string GetPromo(string languague)
        {
            if (Promo != null && Promo.Count > 0)
            {
                if (Promo.ContainsKey(languague))
                {
                    return Promo[languague];
                }
                else
                {
                    // return first available
                    var enumerator = Names.GetEnumerator();
                    enumerator.MoveNext(); // fix not show first line but null string
                    return enumerator.Current.Value;
                }
            }

            return string.Empty;
        }
    }

}