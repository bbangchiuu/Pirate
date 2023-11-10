using System.Collections;
using System.Collections.Generic;
using LitJson;
using System;

using Hiker.Networks.Data.Shootero;

public partial class DailyShopUtils
{
    public static string GetProductPackage(string productId)
    {
        return string.Format("{0}.{1}", ConfigManager.cashShopConfig["PrefixIAP"], productId);
    }

    public static Dictionary<string, OfferStoreConfig> GetGemOfferConfigs()
    {
        var config = ConfigManager.cashShopConfig["GemOffer"];

        var result = JsonMapper.ToObject<Dictionary<string, OfferStoreConfig>>(config.ToJson());
        return result;
    }

    public static Dictionary<string, OfferStoreConfig> GetDailyGemPackConfigs()
    {
        var config = ConfigManager.cashShopConfig["DailyGemPack"];

        var result = JsonMapper.ToObject<Dictionary<string, OfferStoreConfig>>(config.ToJson());
        return result;
    }

    public static Dictionary<string, OfferStoreConfig> GetFlashSaleOfferConfigs()
    {
        var config = ConfigManager.cashShopConfig["FlashSale"];

        var result = JsonMapper.ToObject<Dictionary<string, OfferStoreConfig>>(config.ToJson());
        return result;
    }

    public static Dictionary<string, OfferStoreConfig> GetDailyOfferConfigs()
    {
        var config = ConfigManager.cashShopConfig["DailyOffer"];

        var result = JsonMapper.ToObject<Dictionary<string, OfferStoreConfig>>(config.ToJson());
        return result;
    }
    public static Dictionary<string, OfferStoreConfig> GetWeeklyOfferConfigs()
    {
        var config = ConfigManager.cashShopConfig["WeeklyOffer"];

        var result = JsonMapper.ToObject<Dictionary<string, OfferStoreConfig>>(config.ToJson());
        return result;
    }
    public static Dictionary<string, OfferStoreConfig> GetMonthlyOfferConfigs()
    {
        var config = ConfigManager.cashShopConfig["MonthlyOffer"];

        var result = JsonMapper.ToObject<Dictionary<string, OfferStoreConfig>>(config.ToJson());
        return result;
    }

    public static Dictionary<string, OfferStoreConfig> GetLimitedTimeOfferConfig()
    {
        if (ConfigManager.cashShopConfig.Contains("LimitedTime") == false)
            return null;

        var config = ConfigManager.cashShopConfig["LimitedTime"];
        try
        {
            var result = JsonMapper.ToObject<Dictionary<string, OfferStoreConfig>>(config.ToJson());
            return result;
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public static Dictionary<string, OfferStoreConfig> GetHeroOfferConfig()
    {
        if (ConfigManager.cashShopConfig.Contains("HeroOffer") == false)
            return null;

        var config = ConfigManager.cashShopConfig["HeroOffer"];
        try
        {
            var result = JsonMapper.ToObject<Dictionary<string, OfferStoreConfig>>(config.ToJson());
            return result;
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public static Dictionary<string, OfferStoreConfig> GetPremiumPackConfigs()
    {
        var config = ConfigManager.cashShopConfig["PremiumPack"];

        var result = JsonMapper.ToObject<Dictionary<string, OfferStoreConfig>>(config.ToJson());
        return result;
    }

    public static OfferStoreConfig GetOfferConfig(string name, OfferType type)
    {
        string typeStr = type.ToString();
        if (ConfigManager.cashShopConfig.Contains(typeStr) == false)
            return null;

        var config = ConfigManager.cashShopConfig[typeStr];

        if (config.Contains(name))
        {
            return JsonMapper.ToObject<OfferStoreConfig>(config[name].ToJson());
        }

        return null;
    }
}