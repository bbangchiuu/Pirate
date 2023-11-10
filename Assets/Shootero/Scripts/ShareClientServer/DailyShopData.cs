namespace Hiker.Networks.Data.Shootero
{
    using System;
    using System.Collections.Generic;

    public class DailyShopItemData
    {
        public string Item;
        public int ItemCount;
        public int GemCost; // Gem
        public int GoldCost; // Gold
        public int Discount;
        public int BuyCount;

        public int GetCandyCost(int chapIdx)
        {
            int CandyCost = 999999;
            if(GemCost > 0)
            {
                CandyCost = GemCost * ConfigManager.HalloweenEventConfig.GemToCandy;
            }
            else if(GoldCost > 0)
            {
                ChapterConfig chap = ConfigManager.chapterConfigs[ConfigManager.chapterConfigs.Length - 1];
                if(ConfigManager.chapterConfigs.Length > chapIdx)
                {
                    chap = ConfigManager.chapterConfigs[chapIdx];
                }
                CandyCost = (int)(GoldCost * ConfigManager.HalloweenEventConfig.BaseGoldToCandy / chap.BaseGold );
            }

            return CandyCost;
        }
    }

    public class DailyShopData
    {
        public string ID;
        public long GID;
        public DateTime ResetTime;
        public List<DailyShopItemData> ListItems;
        public int WatchAdsCount;
    }

    public class DailyShopItemConfig
    {
        public int GemCost; // Gem
        public int GoldCost; // Gold
        public string[] Items;
        public int Quantity;
        public int[] Discount;
        public int Rate;
        public int UnlockChapter;
        
    }
    public class DailyShopConfig
    {
        public int MaxWatchAds;
        public int RefreshHours;
        public int MaxItem;
        public DailyShopItemConfig[] ShopItems;
    }
}
