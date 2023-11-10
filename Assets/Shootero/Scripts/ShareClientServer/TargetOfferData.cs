namespace Hiker.Networks.Data.Shootero
{
    using System;
    using System.Collections.Generic;
    
    public class TargetOfferData
    {
        public string ID;
        public long GID;
        
        public DateTime LastTimePurchaseHero;
        public Dictionary<string, int> ListPlayHeroes;
        public int PurchaseCount;
        public int OfferCount;
        
        public List<string> ListHeroOffers;
        public int CurrentHeroOfferIndex;
        public int PurchasedHeroOfferCount;
        public DateTime DelayAfterLevelUpHero;

        public DateTime LastTimeShowOffer;
        public List<TargetOfferDataItem> ListOffers;
    }
    public class TargetOfferDataItem
    {
        public string PackageName;
        public DateTime StartTime;
        public DateTime EndTime;
    }

    public class TargetOfferConfig
    {
        public Dictionary<string, OfferStoreConfig> ListOffers;
    }
}
