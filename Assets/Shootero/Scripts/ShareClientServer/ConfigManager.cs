using Hiker.Networks.Data.Shootero;
using LitJson;
using System.Collections.Generic;
using System.Linq;
using TimeSpan = System.TimeSpan;

public partial class ConfigManager
{
    public const int SecondsEachDay = 3600 * 24;
    public const string DefaultHeroCodeName = "Zora";
    public const int NumBossHunt = 3;

    public static ConfigManager instance;

    public static JsonData otherConfig { get; private set; }
    public static JsonData assetBundleConfig { get; private set; }

    public static Dictionary<string, float> Materials { get; private set; }

    public static Dictionary<string, UnitStat> UnitStats { get; private set; }
    public static Dictionary<string, WeaponStatConfig> WeaponStats { get; private set; }
    public static Dictionary<string, BattleItemStat> BattleItemStats { get; private set; }
    public static BattleItemConfig BattleItemCfg { get; private set; }
    public static Dictionary<string, ItemConfig> ItemStats { get; private set; }
    public static Dictionary<string, CardConfig> CardStats { get; private set; }
    public static Dictionary<string, StatMod[]> ArmoryStats { get; private set; }
    public static Dictionary<string, UpgradeConfig> UpgradeCfg { get; private set; }
    public static Dictionary<string, ChestConfig> ChestCfg { get; private set; }
    public static JsonData cashShopConfig;
    public static JsonData HeroSkills { get; private set; }
    public static HeroUpgradeConfig HeroUpgradeCfg { get; private set; }
    public static LeoThapConfig LeoThapCfg { get; private set; }
    public static LeoThapBattleConfig LeoThapBattleCfg { get; private set; }
    public static ThachDauConfig ThachDauCfg { get; private set; }
    public static ThachDauBattleConfig ThachDauBattleCfg { get; private set; }
    public static SanTheConfig SanTheCfg { get; private set; }
    public static DailyShopConfig DailyShopCfg { get; private set; }
    public static HalloweenEventConfig HalloweenEventConfig { get; private set; }
    public static GiangSinhConfig GiangSinhCfg { get; private set; }
    public static TruongThanhConfig TruongThanhCfg { get; private set; }
    public static TetEventConfig TetEventCfg { get; private set; }

    public static VongQuayConfig VongQuay { get; private set; }

    public static GameConfig gameConfig { get; private set; }
    public static ChapterConfig[] chapterConfigs { get; private set; }
    public static Dictionary<string, FarmConfig> FarmConfigs { get; private set; }

    public static List<string> supportLangues = new List<string>()
    {
            "English",
    };

    public static bool loaded;

    public const string GoldName = "Gold";
    public const string GemName = "Gem";

    public static void LoadConfigs()
    {
        loaded = true;

        otherConfig = ReadConfig("otherConfig");

        if (otherConfig.Contains("languages"))
        {
            supportLangues = JsonMapper.ToObject<List<string>>(otherConfig["languages"].ToJson());
        }
        else
        {
            supportLangues = new List<string>() { "English" };
        }

        UnitStats = ReadUnitStats("UnitStat");

        var heroStats = ReadUnitStats("HeroStat");
        if (heroStats != null)
        {
            foreach (var hero in heroStats)
            {
                if (UnitStats.ContainsKey(hero.Key))
                {
                    UnitStats[hero.Key] = hero.Value;
                }
                else
                {
                    UnitStats.Add(hero.Key, hero.Value);
                }
            }
        }

        WeaponStats = ReadConfig<Dictionary<string, WeaponStatConfig>>("WeaponStat");
        BattleItemStats = ReadConfig<Dictionary<string, BattleItemStat>>("BattleItemStat");
        ItemStats = ReadConfig<Dictionary<string, ItemConfig>>("ItemStat");
        ArmoryStats = ReadConfig<Dictionary<string, StatMod[]>>("ArmoryStat");
        Materials = ReadConfig<Dictionary<string, float>>("Material");
        UpgradeCfg = ReadConfig<Dictionary<string, UpgradeConfig>>("UpgradeConfig");
        ChestCfg = ReadConfig<Dictionary<string, ChestConfig>>("ChestConfig");

        CardStats = ReadConfig<Dictionary<string, CardConfig>>("CardStat");

        VongQuay = ReadConfig<VongQuayConfig>("VongQuay");
        gameConfig = ReadConfig<GameConfig>("GameConfig");
        int numChap = (int)otherConfig["numChapter"];
        chapterConfigs = new ChapterConfig[numChap];
        FarmConfigs = new Dictionary<string, FarmConfig>();
        for (int i = 1; i <= numChap; ++i)
        {
            var chapCfg = ReadConfig<ChapterConfig>("Chapters/Chapter" + i);
            chapterConfigs[i - 1] = chapCfg;
            foreach (var farmName in chapCfg.FarmCfgs)
            {
                var farmCfg = ReadConfig<FarmConfig>("Farms/" + farmName);
                if (FarmConfigs.ContainsKey(farmName))
                {
                    FarmConfigs[farmName] = farmCfg;
                }
                else
                {
                    FarmConfigs.Add(farmName, farmCfg);
                }
            }
        }
        BattleItemCfg = ReadConfig<BattleItemConfig>("BattleItemConfig");
        cashShopConfig = ReadConfig("cashShop");
        HeroSkills = ReadConfig("HeroSkill");
        HeroUpgradeCfg = ReadConfig<HeroUpgradeConfig>("HeroUpgrade");
        assetBundleConfig = ReadConfig("AssetBundlesCfg");
        LeoThapCfg = ReadConfig<LeoThapConfig>("LeoThap");
        LeoThapBattleCfg = ReadConfig<LeoThapBattleConfig>("LeoThapBattle");
        ThachDauCfg = ReadConfig<ThachDauConfig>("ThachDau");
        ThachDauBattleCfg = ReadConfig<ThachDauBattleConfig>("ThachDauBattle");
        SanTheCfg = ReadConfig<SanTheConfig>("CardHunt");
        DailyShopCfg = ReadConfig<DailyShopConfig>("DailyShop");
        HalloweenEventConfig = ReadConfig<HalloweenEventConfig>("HalloweenEvent");
        GiangSinhCfg = ReadConfig<GiangSinhConfig>("GiangSinh");
        TruongThanhCfg = ReadConfig<TruongThanhConfig>("TruongThanh");
        TetEventCfg = ReadConfig<TetEventConfig>("TetEvent");
    }

    public static string RandomMaterial()
    {
        List<float> chances = new List<float>();
        List<string> names = new List<string>();
        foreach (var t in Materials.Keys)
        {
            names.Add(t);
            chances.Add(Materials[t]);
        }

        float total = 0;
        foreach (float c in chances)
        {
            total += c;
        }
#if SERVER_CODE
        float rg = RandomUtils.GetRandomFloat() * total;
#else
        float rg = UnityEngine.Random.Range(0, total);
#endif
        int index = 0;
        float checkChance = 0;
        foreach (float c in chances)
        {
            checkChance += c;
            if (rg < checkChance) return names[index];
            index++;
        }

        return null;
    }

    public static Dictionary<string, UnitStat> ReadUnitStats(string configFileName, string path = null)
    {
        string text = ReadConfigString(configFileName, path);
        if (string.IsNullOrEmpty(text)) return null;
        var wrapper = JsonMapper.ToObject<Dictionary<string, UnitStatWrapper>>(text);
        Dictionary<string, UnitStat> result = new Dictionary<string, UnitStat>();
        foreach (var s in wrapper)
        {
            result.Add(s.Key, UnitStat.FromUnitStat(s.Value));
        }

        return result;
    }

    public static JsonData ReadConfig(string configFileName, string path = null)
    {
        string text = ReadConfigString(configFileName, path);
        if (string.IsNullOrEmpty(text)) return null;
        return JsonMapper.ToObject(text);
    }
    public static T ReadConfig<T>(string configFileName, string path = null)
    {
        string text = ReadConfigString(configFileName, path);
        if (string.IsNullOrEmpty(text)) return default(T);
        return JsonMapper.ToObject<T>(text);
    }

    public static UnitStat GetUnitStat(string unitName)
    {
        return UnitStats[unitName];
    }

    #region Items

    public static ItemConfig GetItemConfig(TrangBiData item)
    {
        return GetItemConfig(item.Name);
    }

    public static ItemConfig GetItemConfig(string itemName)
    {
        ItemConfig cfg = null;
        if (ItemStats.ContainsKey(itemName))
        {
            cfg = ItemStats[itemName];
        }
        return cfg;
    }

    public const ERarity MaxRarity = ERarity.Mysthic;
    public const ERarity MinRarity = ERarity.Common;

    public static ERarity GetItemDefaultRarity(string itemName)
    {
        var cfg = GetItemConfig(itemName);
        foreach (var statRarity in cfg.Stats)
        {
            if (System.Enum.TryParse<ERarity>(statRarity.Key, out ERarity rarity))
            {
                return rarity;
            }
        }
        return ERarity.Common;
    }

    #endregion

    #region Armory

    public static StatMod[] GetArmoryStat(string armory, int level)
    {
        var listStatMods = ArmoryStats[armory];
        var result = new StatMod[listStatMods.Length];
        for (int i = 0; i < listStatMods.Length; ++i)
        {
            var mod = listStatMods[i];
            var inc = mod.GetTotalIncByLevel(level);
            mod.Val += inc;
            mod.Inc = null;
            //if (listStatMods[i].Inc != null)
            //{
            //    var c = listStatMods[i].Inc.Length;
            //    mod.Inc = new double[c];
            //    for (int k = 0; k < c; ++k)
            //    {
            //        mod.Inc[k] = listStatMods[i].Inc[k];
            //    }
            //}
            result[i] = mod;
        }
        return result;
    }

    #endregion

    public static CardConfig GetCardConfig(CardData card)
    {
        return GetCardConfig(card.Name);
    }

    public static CardConfig GetCardConfig(string cardName)
    {
        CardConfig cfg = null;
        if (CardStats.ContainsKey(cardName))
        {
            cfg = CardStats[cardName];
        }
        return cfg;
    }

    public static int GetMaxLevelByRarity(string item, ERarity rarity, int star = 0)
    {
        //if (UpgradeCfg.TryGetValue(item, out UpgradeConfig config))
        //{
        //    var lvlReq = config.MaxLevel;
        //    int rarityInt = (int)rarity;
        //    if (lvlReq.Length <= rarityInt)
        //    {
        //        rarityInt = lvlReq.Length - 1;
        //    }
        //    return lvlReq[rarityInt];
        //}
        //else
        //{
        //    return 1;
        //}
        if (otherConfig.Contains("MaxLevelByRarity"))
        {
            var maxLeves = otherConfig["MaxLevelByRarity"];
            int idx = (int)rarity + star;
            if (maxLeves.Count <= idx)
            {
                return (int)maxLeves[maxLeves.Count - 1];
            }
            else
            {
                return (int)maxLeves[idx];
            }
        }
        else
        {
            return 1;
        }
    }

    public static Dictionary<string, int> GetMaterialRecycleFromItems(List<TrangBiData> listTb, IEnumerable<string> listRecycle, bool isCountFirstLevel = true)
    {
        Dictionary<string, int> materials = new Dictionary<string, int>();

        for (int i = listTb.Count - 1; i >= 0; --i)
        {
            var tb = listTb[i];
            if (listRecycle.Contains(tb.ID))
            {
                int startLevel = isCountFirstLevel ? 0 : 1;
                for (int l = startLevel; l < tb.Level; ++l)
                {
                    bool isMaxUpgrade = false;
                    var matReq = ConfigManager.GetItemUpgradeRequirement(tb.Name, l, out isMaxUpgrade);
                    foreach (var m in matReq)
                    {
                        if (ConfigManager.Materials.ContainsKey(m.Res)) // is material
                        {
                            if (materials.ContainsKey(m.Res))
                            {
                                materials[m.Res] += (int)m.Num;
                            }
                            else
                            {
                                materials[m.Res] = (int)m.Num;
                            }
                        }
                    }
                }
            }
        }
        return materials;
    }

    public static UpgradeRequirement[] GetItemUpgradeRequirement(string item, int level, out bool isMaxUpgrade)
    {
        isMaxUpgrade = false;
        ItemConfig itemConfig = GetItemConfig(item);

        var upgradeKey = item;
        if (itemConfig != null) upgradeKey = itemConfig.Slot.ToString();

        if (UpgradeCfg.TryGetValue(upgradeKey, out UpgradeConfig config))
        {
            var lvlReq = config.LevelReq;
            if (lvlReq.Length <= level)
            {
                level = lvlReq.Length - 1;
                isMaxUpgrade = true;
            }

            UpgradeRequirement[] upgradeRequirements = new UpgradeRequirement[lvlReq[level].Requirements.Length];
            double materialScaleByRarity = 1;
            if (itemConfig != null)
            {
                var itemRarity = GetItemDefaultRarity(item);
                materialScaleByRarity = otherConfig["UpgradeMaterialRateByRarity"][(int)itemRarity].ToFloat();
            }

            for (int i = 0; i < lvlReq[level].Requirements.Length; i++)
            {
                upgradeRequirements[i] = new UpgradeRequirement();
                upgradeRequirements[i].Res = lvlReq[level].Requirements[i].Res;
                upgradeRequirements[i].Num = (int)(lvlReq[level].Requirements[i].Num * materialScaleByRarity);
            }
            return upgradeRequirements;
        }
        else
        {
            return null;
        }
    }

    public static int GetNumChapter()
    {
        if (otherConfig.Contains("numChapter"))
        {
            int hp = (int)otherConfig["numChapter"];
            return hp;
        }
        else
        {
            return 0;
        }
    }

    public static int GetBaseHPHealOrb()
    {
        if (otherConfig.Contains("HealOrbHP"))
        {
            int hp = (int)otherConfig["HealOrbHP"];
            return hp;
        }
        else
        {
            return 0;
        }
    }

    public static int GetBaseHPLevelUp()
    {
        if (otherConfig.Contains("HPLevelUp"))
        {
            int hp = (int)otherConfig["HPLevelUp"];
            return hp;
        }
        else
        {
            return 0;
        }
    }

    public static int GetCreepNodeHPBuff()
    {
        if (otherConfig.Contains("CreepNodeHPBuff"))
        {
            int hp = (int)otherConfig["CreepNodeHPBuff"];
            return hp;
        }
        else
        {
            return 0;
        }
    }
    public static int GetCreepNodeDMGBuff()
    {
        if (otherConfig.Contains("CreepNodeDMGBuff"))
        {
            int hp = (int)otherConfig["CreepNodeDMGBuff"];
            return hp;
        }
        else
        {
            return 0;
        }
    }
    public static int GetAngelNodeHPBuff()
    {
        if (otherConfig.Contains("AngelNodeHPBuff"))
        {
            int hp = (int)otherConfig["AngelNodeHPBuff"];
            return hp;
        }
        else
        {
            return 0;
        }
    }
    public static int GetAngelNodeDMGBuff()
    {
        if (otherConfig.Contains("AngelNodeDMGBuff"))
        {
            int hp = (int)otherConfig["AngelNodeDMGBuff"];
            return hp;
        }
        else
        {
            return 0;
        }
    }
    public static int GetBossNodeHPBuff()
    {
        if (otherConfig.Contains("BossNodeHPBuff"))
        {
            int hp = (int)otherConfig["BossNodeHPBuff"];
            return hp;
        }
        else
        {
            return 0;
        }
    }
    public static int GetBossNodeDMGBuff()
    {
        if (otherConfig.Contains("BossNodeDMGBuff"))
        {
            int hp = (int)otherConfig["BossNodeDMGBuff"];
            return hp;
        }
        else
        {
            return 0;
        }
    }

    public static int GetExpByCreepNode()
    {
        if (otherConfig.Contains("ExpByCreepNode"))
        {
            int hp = (int)otherConfig["ExpByCreepNode"];
            return hp;
        }
        else
        {
            return 400;
        }
    }
    public static int GetExpByBossNode()
    {
        if (otherConfig.Contains("ExpByBossNode"))
        {
            int hp = (int)otherConfig["ExpByBossNode"];
            return hp;
        }
        else
        {
            return 100;
        }
    }

    public static int GetNumOfHealOrbNodeBoss()
    {
        if (otherConfig.Contains("numOfHealOrbNodeBoss"))
        {
            int num = (int)otherConfig["numOfHealOrbNodeBoss"];
            return num;
        }
        else
        {
            return 0;
        }
    }

    public static int GetNumOfHealOrbNodeQuai()
    {
        if (otherConfig.Contains("numOfHealOrbNodeQuai"))
        {
            int num = (int)otherConfig["numOfHealOrbNodeQuai"];
            return num;
        }
        else
        {
            return 0;
        }
    }

    public static int GetHealOrbDropChance()
    {
        if (otherConfig.Contains("healOrbDropChance"))
        {
            int num = (int)otherConfig["healOrbDropChance"];
            return num;
        }
        else
        {
            return 0;
        }
    }

    public static int GetMaxFarmGemPerDay()
    {
        if (otherConfig.Contains("MaxFarmGemPerDay"))
        {
            int num = (int)otherConfig["MaxFarmGemPerDay"];
            return num;
        }
        else
        {
            return 3;
        }
    }

    public static int GetTheLucCampaign()
    {
        if (otherConfig.Contains("TheLucCampaign"))
        {
            int num = (int)otherConfig["TheLucCampaign"];
            return num;
        }
        else
        {
            return 10;
        }
    }

    public static int GetTheLucVongQuay()
    {
        if (otherConfig.Contains("TheLucVongQuay"))
        {
            int num = (int)otherConfig["TheLucVongQuay"];
            return num;
        }
        else
        {
            return 5;
        }
    }

    public static int GetTheLucFarmMode(int mode)
    {
        if (otherConfig.Contains("TheLucFarmMode"))
        {
            int num = (int)otherConfig["TheLucFarmMode"];

            return num * mode;
        }
        //else
        {
            return 5 * mode;
        }
    }

    public static int GetGamerTheLucMax()
    {
        if (otherConfig.Contains("GamerTheLucMax"))
        {
            int num = (int)otherConfig["GamerTheLucMax"];
            return num;
        }
        else
        {
            return 10;
        }
    }

    public static int GetGamerTheLucRegenSeconds()
    {
        if (otherConfig.Contains("GamerTheLucRegenSeconds"))
        {
            int num = (int)otherConfig["GamerTheLucRegenSeconds"];
            return num;
        }
        else
        {
            return 10 * 60; // default 10 minutes per value
        }
    }

    public static int GetHeSoRewardCampaign()
    {
        if (otherConfig.Contains("HeSoRewardCampaign"))
        {
            int num = (int)otherConfig["HeSoRewardCampaign"];
            return num;
        }
        else
        {
            return 100;
        }
    }

    public static int GetHeSoRewardFarmMode()
    {
        if (otherConfig.Contains("HeSoRewardFarm"))
        {
            int num = (int)otherConfig["HeSoRewardFarm"];
            return num;
        }
        else
        {
            return 100;
        }
    }

    public static int GetHeSoRewardThachDau()
    {
        if (otherConfig.Contains("HeSoRewardThachDau"))
        {
            int num = (int)otherConfig["HeSoRewardThachDau"];
            return num;
        }
        else
        {
            return 100;
        }
    }

    public static int GetHeSoRewardVongQuay()
    {
        if (otherConfig.Contains("HeSoRewardVongQuay"))
        {
            int num = (int)otherConfig["HeSoRewardVongQuay"];
            return num;
        }
        else
        {
            return 100;
        }
    }

    public static int GetTileDropMaterial()
    {
        if (otherConfig.Contains("TileDropMaterial"))
        {
            int num = (int)otherConfig["TileDropMaterial"];
            return num;
        }
        else
        {
            return 0;
        }
    }

    public static int GetDropMaterialMax()
    {
        if (otherConfig.Contains("DropMaterialMax"))
        {
            int num = (int)otherConfig["DropMaterialMax"];
            return num;
        }
        else
        {
            return 0;
        }
    }
    public static int GetDropMaterialMin()
    {
        if (otherConfig.Contains("DropMaterialMin"))
        {
            int num = (int)otherConfig["DropMaterialMin"];
            return num;
        }
        else
        {
            return 0;
        }
    }

    public static long GetTotalGoldCampaign(int chapIdx)
    {
        var chapCfg = chapterConfigs[chapIdx];
        long baseGold = chapCfg.BaseGold;
        return GetHeSoRewardCampaign() * baseGold / 100;
    }
    public static int GetTotalMaterialCampaign(int chapIdx)
    {
        var chapCfg = chapterConfigs[chapIdx];
        var baseMaterial = chapCfg.BaseMaterial;
        return GetHeSoRewardCampaign() * baseMaterial / 100;
    }
    public static long GetTotalGoldFarmMode(int chapIdx)
    {
        var chapCfg = chapterConfigs[chapIdx];
        long baseGold = chapCfg.BaseGold;
        return GetHeSoRewardFarmMode() * baseGold / 100;
    }
    public static long GetTotalGoldThachDau(int chapIdx)
    {
        var chapCfg = chapterConfigs[chapIdx];
        long baseGold = chapCfg.BaseGold;
        return GetHeSoRewardThachDau() * baseGold / 100;
    }
    public static long GetTotalGoldSanThe(int chapIdx)
    {
        var chapCfg = chapterConfigs[chapIdx];
        long baseGold = chapCfg.BaseGold;
        return ConfigManager.SanTheCfg.HeSoRewardGold * baseGold / 100;
    }
    public static int GetTotalMaterialFarmMode(int chapIdx)
    {
        var chapCfg = chapterConfigs[chapIdx];
        var baseMaterial = chapCfg.BaseMaterial;
        return GetHeSoRewardFarmMode() * baseMaterial / 100;
    }
    public static long GetTotalGoldVongQuay(int chapIdx)
    {
        var chapCfg = chapterConfigs[chapIdx];
        long baseGold = chapCfg.BaseGold;
        return GetHeSoRewardVongQuay() * baseGold / 100;
    }
    public static int GetTotalMaterialVongQuay(int chapIdx)
    {
        var chapCfg = chapterConfigs[chapIdx];
        var baseMaterial = chapCfg.BaseMaterial;
        return GetHeSoRewardVongQuay() * baseMaterial / 100;
    }
    public static int GetBasedGoldOffer(int chapIdx)
    {
        int baseNum = 0;
        if (otherConfig.Contains("BasedGoldOffer") && otherConfig["BasedGoldOffer"].Count > chapIdx)
        {
            baseNum = (int)otherConfig["BasedGoldOffer"][chapIdx];
        }
        else
        {
            var chapCfg = chapterConfigs[chapIdx];
            baseNum = (int)chapCfg.BaseGold;
        }
        return baseNum;
    }
    public static int GetBasedMaterialOffer(int chapIdx)
    {
        int baseNum = 0;
        if (otherConfig.Contains("BasedMaterialOffer") && otherConfig["BasedMaterialOffer"].Count > chapIdx)
        {
            baseNum = (int)otherConfig["BasedMaterialOffer"][chapIdx];
        }
        else
        {
            var chapCfg = chapterConfigs[chapIdx];
            baseNum = chapCfg.BaseMaterial;
        }
        return baseNum;
    }

    public static EBattleEventRoom[] GetListBattleEventRoom()
    {
        if (otherConfig.Contains("BattleEvents"))
        {
            var cfg = otherConfig["BattleEvents"];
            int num = 0;
            if (cfg.IsArray)
            {
                num = cfg.Count;

                EBattleEventRoom[] result = new EBattleEventRoom[num];
                for (int i = 0; i < num; ++i)
                {
                    result[i] = (EBattleEventRoom)System.Enum.Parse(typeof(EBattleEventRoom), cfg[i].ToString());
                }
                return result;
            }

            return (EBattleEventRoom[])System.Enum.GetValues(typeof(EBattleEventRoom));
        }
        else
        {
            return (EBattleEventRoom[])System.Enum.GetValues(typeof(EBattleEventRoom));
        }
    }

    public static float[] GetBattleEventRate() // ti le random event room
    {
        bool haveCfg = false;
        var rooms = GetListBattleEventRoom();
        if (otherConfig.Contains("BattleEventRates"))
        {
            var cfg = otherConfig["BattleEventRates"];
            int num = 0;
            if (cfg.IsArray)
            {
                num = cfg.Count;
                if (num == rooms.Length)
                {
                    float[] result = new float[num];
                    for (int i = 0; i < num; ++i)
                    {
                        result[i] = cfg[i].ToFloat();
                    }
                    haveCfg = true;
                    return result;
                }
            }
        }

        //if (haveCfg == false) // default rate
        {
            var rates = new float[rooms.Length];
            for (int i = 0; i < rooms.Length; ++i)
            {
                var r = rooms[i];
                if (r == EBattleEventRoom.Angel)
                {
                    rates[i] = 1;
                }
                else
                {
                    rates[i] = 0;
                }
            }
            return rates;
        }
    }

    public static TimeSpan GetMaxAFKTime()
    {
        int num = 480;
        if (otherConfig.Contains("MaxAFKTimeByMinute"))
        {
            num = (int)otherConfig["MaxAFKTimeByMinute"];
        }
        TimeSpan ts = new TimeSpan(0, num, 0);
        return ts;
    }
    public static int GetWatchAdsTheLuc()
    {
        if (otherConfig.Contains("WatchAdsTheLuc"))
        {
            int num = (int)otherConfig["WatchAdsTheLuc"];
            return num;
        }
        else
        {
            return 5;
        }
    }
    public static int GetFullRefreshTheLucGemCost()
    {
        if (otherConfig.Contains("FullRefreshTheLucGemCost"))
        {
            int num = (int)otherConfig["FullRefreshTheLucGemCost"];
            return num;
        }
        else
        {
            return 50;
        }
    }

    public static int GetMaxAdsPerDay()
    {
        if (otherConfig.Contains("MaxAdsPerDay"))
        {
            int num = (int)otherConfig["MaxAdsPerDay"];
            return num;
        }
        else
        {
            return 15;
        }
    }
    public static int GetMaxAdsVongQuayPerDay()
    {
        if (otherConfig.Contains("MaxAdsVongQuayPerDay"))
        {
            int num = (int)otherConfig["MaxAdsVongQuayPerDay"];
            return num;
        }
        else
        {
            return 7;
        }
    }
    public static int GetMaxAdsThemVangChapterPerDay()
    {
        if (otherConfig.Contains("MaxAdsThemVangChapterPerDay"))
        {
            int num = (int)otherConfig["MaxAdsThemVangChapterPerDay"];
            return num;
        }
        else
        {
            return 3;
        }
    }
    public static int GetWatchAdsThemVangPercent(int chapIdx)
    {
        if (otherConfig.Contains("WatchAdsThemVangPercent"))
        {
            if (chapIdx >= otherConfig["WatchAdsThemVangPercent"].Count) chapIdx = otherConfig["WatchAdsThemVangPercent"].Count - 1;
            int num = (int)otherConfig["WatchAdsThemVangPercent"][chapIdx];
            return num;
        }
        else
        {
            return 100; // default 6 hours
        }
    }
    //public static int GetAFKGoldPerMin(UserInfo _userInfo)
    //{
    //    int num = 0;
    //    var armoryData = _userInfo.ListArmories.Find(e => e.Name == "AfkRewardTalent");

    //    if (armoryData != null)
    //    {
    //        return GetErrorResponse(response, ERROR_CODE.DISPLAY_MESSAGE,
    //            Localization.Get("InvalidArmory", req.lang));

    //    }
    //    return num;
    //}
    //public static float GetAFKMaterialPerMin(int chapIndex)
    //{
    //    float num = 0;
    //    if (otherConfig.Contains("AFKMaterialPerMinRate"))
    //    {
    //        num = otherConfig["AFKMaterialPerMinRate"].ToFloat();
    //    }
    //    var chapCfg = chapterConfigs[chapIndex];
    //    long baseMaterial = chapCfg.BaseMaterial;
    //    num *= baseMaterial;
    //    return num;
    //}

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

    public static Dictionary<string, ChapterConfig.BossHuntConfig> GetTotalBossHuntConfigs()
    {
        Dictionary<string, ChapterConfig.BossHuntConfig> result = new Dictionary<string, ChapterConfig.BossHuntConfig>();
        for (int i = 0; i < chapterConfigs.Length; ++i)
        {
            var chapCfg = chapterConfigs[i];
            if (chapCfg.BossHunt.Count > 0)
            {
                foreach (var bossName in chapCfg.BossHunt.Keys)
                {
                    result.Add(bossName, chapCfg.BossHunt[bossName]);
                }
            }
        }
        return result;
    }

    public static int GetGemRevive()
    {
        if (otherConfig.Contains("GemRevive"))
        {
            int num = (int)otherConfig["GemRevive"];
            return num;
        }
        else
        {
            return 30;
        }
    }

    public static int GetReviveCountDown()
    {
        if (otherConfig.Contains("ReviveCountDown"))
        {
            int num = (int)otherConfig["ReviveCountDown"];
            return num;
        }
        else
        {
            return 5;
        }
    }
    public static int GetAdReviveMaxPerDay()
    {
        if (otherConfig.Contains("AdReviveMaxPerDay"))
        {
            int num = (int)otherConfig["AdReviveMaxPerDay"];
            return num;
        }
        else
        {
            return 3;
        }
    }
    public static int GetAdReviveMaxRate()
    {
        if (otherConfig.Contains("AdReviveRate"))
        {
            int num = (int)otherConfig["AdReviveRate"];
            return num;
        }
        else
        {
            return 50;
        }
    }

    public static int GetFreeGemAdsMaxTime()
    {
        if (otherConfig.Contains("FreeGemAdsMaxTime"))
        {
            int num = (int)otherConfig["FreeGemAdsMaxTime"];
            return num;
        }
        else
        {
            return 4;
        }
    }

    public static int GetFreeGemNumber()
    {
        if (otherConfig.Contains("FreeGemNumber"))
        {
            int num = (int)otherConfig["FreeGemNumber"];
            return num;
        }
        else
        {
            return 25;
        }
    }

    public static int GetFreeGemAdsRegenSeconds()
    {
        if (otherConfig.Contains("FreeGemAdsRegenSeconds"))
        {
            int num = (int)otherConfig["FreeGemAdsRegenSeconds"];
            return num;
        }
        else
        {
            return 6 * 24 * 60; // default 6 hours
        }
    }

    public static JsonData GetHeroSkillCfg(string skillName)
    {
        if (HeroSkills.Contains(skillName))
        {
            return HeroSkills[skillName];
        }
        return null;
    }

    public static int GetHeroSkillParams(string skillName, int paramIdx)
    {
        var skillCfg = GetHeroSkillCfg(skillName);
        var paramList = skillCfg["Params"];
        return (int)paramList[paramIdx];
    }

    public static bool IsHeroUnit(string hName)
    {
        var heroes = otherConfig["Heroes"];
        for (int i = 0; i < heroes.Count; ++i)
        {
            if (heroes[i].ToString() == hName)
            {
                return true;
            }
        }
        return false;
    }

    public static int GetChapterUnlockHero()
    {
        if (otherConfig.Contains("ChapterUnlockHero"))
        {
            int num = (int)otherConfig["ChapterUnlockHero"];
            return num;
        }
        else
        {
            return 2; // default unlock at chapter 3
        }
    }

    public static int GetHeroUpgradeGoldCost(int crLevel)
    {
        int idx = crLevel - 1;
        int num = 999999;
        if (idx < HeroUpgradeCfg.GoldCost.Length)
        {
            num = HeroUpgradeCfg.GoldCost[idx];
        }
        return num;
    }
    public static int GetHeroUpgradeHeroStoneCost(int crLevel)
    {
        int idx = crLevel - 1;
        int num = 999999;
        if (idx < HeroUpgradeCfg.HeroStoneCost.Length)
        {
            num = HeroUpgradeCfg.HeroStoneCost[idx];
        }
        return num;
    }
    public static int GetHeroLevelMax(string heroName)
    {
        int levelMax = 1;
        if (HeroUpgradeCfg.Heroes.ContainsKey(heroName))
        {
            levelMax = HeroUpgradeCfg.Heroes[heroName].LevelMax;
        }
        return levelMax;
    }

    public static int GetMaxStarMythicItem()
    {
        if (otherConfig.Contains("MaxStarMythicItem"))
        {
            int num = (int)otherConfig["MaxStarMythicItem"];
            return num;
        }
        else
        {
            return 3;
        }
    }

    public static int GetNumberOfMythicItemBySlot(string slot)
    {
        if (otherConfig.Contains("NumberOfMythicItemBySlot"))
        {
            int num = (int)otherConfig["NumberOfMythicItemBySlot"][slot];
            return num;
        }
        else
        {
            return 0;
        }
    }
    public static int GetExtraAFKMin()
    {
        if (otherConfig.Contains("ExtraAFKMin"))
        {
            int num = (int)otherConfig["ExtraAFKMin"];
            return num;
        }
        else
        {
            return 360;
        }
    }
    public static int GetExtraAFKMaxTime()
    {
        if (otherConfig.Contains("ExtraAFKMaxTime"))
        {
            int num = (int)otherConfig["ExtraAFKMaxTime"];
            return num;
        }
        else
        {
            return 2;
        }
    }
    public static int GetExtraAFKGemCost()
    {
        if (otherConfig.Contains("ExtraAFKGemCost"))
        {
            int num = (int)otherConfig["ExtraAFKGemCost"];
            return num;
        }
        else
        {
            return 50;
        }
    }
    public static int GetExtraAFKWatchAdsDelayMin()
    {
        if (otherConfig.Contains("ExtraAFKWatchAdsDelayMin"))
        {
            int num = (int)otherConfig["ExtraAFKWatchAdsDelayMin"];
            return num;
        }
        else
        {
            return 360;
        }
    }
    public static int GetDailyGoldAdsMax()
    {
        if (otherConfig.Contains("DailyGoldAdsMax"))
        {
            int num = (int)otherConfig["DailyGoldAdsMax"];
            return num;
        }
        else
        {
            return 2;
        }
    }
    public static int GetDailyGoldAdsBaseNumber()
    {
        if (otherConfig.Contains("DailyGoldAdsBaseNumber"))
        {
            int num = (int)otherConfig["DailyGoldAdsBaseNumber"];
            return num;
        }
        else
        {
            return 2;
        }
    }
    public static int GetDailyMatAdsMax()
    {
        if (otherConfig.Contains("DailyMatAdsMax"))
        {
            int num = (int)otherConfig["DailyMatAdsMax"];
            return num;
        }
        else
        {
            return 2;
        }
    }
    public static int GetDailyMatAdsBaseNumber()
    {
        if (otherConfig.Contains("DailyMatAdsBaseNumber"))
        {
            int num = (int)otherConfig["DailyMatAdsBaseNumber"];
            return num;
        }
        else
        {
            return 2;
        }
    }
    public static int GetDailyItemAdsMax()
    {
        if (otherConfig.Contains("DailyItemAdsMax"))
        {
            int num = (int)otherConfig["DailyItemAdsMax"];
            return num;
        }
        else
        {
            return 3;
        }
    }
    public static string GetDailyItemAdsName()
    {
        if (otherConfig.Contains("DailyItemAdsName"))
        {
            string n = (string)otherConfig["DailyItemAdsName"];
            return n;
        }
        else
        {
            return "M_VeThachDau";
        }
    }
    public static int GetTongHopQuangCaoChapterUnlock()
    {
        if (otherConfig.Contains("TongHopQuangCaoChapterUnlock"))
        {
            int num = (int)otherConfig["TongHopQuangCaoChapterUnlock"];
            return num;
        }
        else
        {
            return 2; // default unlock at chapter 4
        }
    }


    public static List<StatMod> GetHeroStats(string heroName, int level)
    {
        List<StatMod> listStatMods = new List<StatMod>();
        //basic stats
        for (int i = 0; i < ConfigManager.HeroUpgradeCfg.Heroes[heroName].BasicStats.Length; i++)
        {
            StatMod mod = ConfigManager.HeroUpgradeCfg.Heroes[heroName].BasicStats[i];
            mod.Val += mod.GetTotalIncByLevel(level);
            mod.Inc = null;
            listStatMods.Add(mod);
        }
        //advanced stats
        for (int i = 0; i < ConfigManager.HeroUpgradeCfg.Heroes[heroName].AdvancedStats.Length; i++)
        {
            if (ConfigManager.HeroUpgradeCfg.Heroes[heroName].AdvancedStats[i].unlockLevel > level) continue;
            StatMod mod = ConfigManager.HeroUpgradeCfg.Heroes[heroName].AdvancedStats[i].Stat;
            mod.Val += mod.GetTotalIncByLevel(level);
            mod.Inc = null;
            listStatMods.Add(mod);
        }

        return listStatMods;
    }

    public static List<string> dailyMaterialOffers = new List<string> { "Daily_Offer_VuKhi", "Daily_Offer_Mu", "Daily_Offer_Giap", "Daily_Offer_OffHand" };
    public static string GetBonusDailyMaterialPackageName()
    {
#if SERVER_CODE
        var ServerTime = TimeUtils.Now;
#else
        var ServerTime = GameClient.instance.ServerTime;
#endif
        return dailyMaterialOffers[ServerTime.DayOfYear % dailyMaterialOffers.Count];
    }

    public static short GetBonusDailyMaterial()
    {
        if (otherConfig.Contains("BonusDailyMaterial"))
        {
            short num = (short)otherConfig["BonusDailyMaterial"];
            return num;
        }
        else
        {
            return 25;
        }
    }

    public static int GetPremiumDailyGem()
    {
        if (otherConfig.Contains("PremiumDailyGem"))
        {
            int num = (int)otherConfig["PremiumDailyGem"];
            return num;
        }
        else
        {
            return 200;
        }
    }
    public static int GetPremiumDailyTheLuc()
    {
        if (otherConfig.Contains("PremiumDailyTheLuc"))
        {
            int num = (int)otherConfig["PremiumDailyTheLuc"];
            return num;
        }
        else
        {
            return 40;
        }
    }
    public static int GetPremiumBonusResource()
    {
        if (otherConfig.Contains("PremiumBonusResource"))
        {
            int num = (int)otherConfig["PremiumBonusResource"];
            return num;
        }
        else
        {
            return 50;
        }
    }
    public static int GetPremiumDays(string packageName)
    {
        string key = "PremiumDays_" + packageName;
        if (otherConfig.Contains(key))
        {
            int num = (int)otherConfig[key];
            return num;
        }
        else
        {
            return 3;
        }
    }
    public static int GetPremiumChapterUnlock()
    {
        if (otherConfig.Contains("PremiumChapterUnlock"))
        {
            int num = (int)otherConfig["PremiumChapterUnlock"];
            return num;
        }
        else
        {
            return 3; // default unlock at chapter 4
        }
    }
    public static int GetMaxCardSlotByRarity(ERarity rarity)
    {
        if (otherConfig.Contains("MaxCardSlotByRarity"))
        {
            int num = (int)otherConfig["MaxCardSlotByRarity"][(int)rarity];
            return num;
        }
        else
        {
            return 0;
        }
    }
    public static int GetRecycleCardDustByRarity(ERarity rarity)
    {
        if (otherConfig.Contains("RecycleCardDustByRarity"))
        {
            int num = (int)otherConfig["RecycleCardDustByRarity"][(int)rarity];
            return num;
        }
        else
        {
            return 0;
        }
    }
    public static string GetTrangBiPrefixBySlotType(SlotType slotType)
    {
        string prefix = "";
        switch (slotType)
        {
            case SlotType.VuKhi:
                prefix = "VK_";
                break;
            case SlotType.Mu:
                prefix = "MU_";
                break;
            case SlotType.Giap:
                prefix = "AG_";
                break;
            case SlotType.OffHand:
                prefix = "OH_";
                break;

        }
        return prefix;
    }
#if SERVER_CODE
    public static List<DailyShopItemData> GetNewDailyShopItems(int chapterIdx)
    {
        if (chapterIdx < ConfigManager.GetDailyShopChapterUnlock())
        {
            chapterIdx = ConfigManager.GetDailyShopChapterUnlock();
        }
        List<DailyShopItemData> listItems = new List<DailyShopItemData>();
        for (int i = 0; i < DailyShopCfg.ShopItems.Length; i++)
        {
            if (DailyShopCfg.ShopItems[i].UnlockChapter > chapterIdx) continue;
            int rdNum = RandomUtils.GetRandomNumber(100);
            if (rdNum < DailyShopCfg.ShopItems[i].Rate)
            {
                int goldCostEach = DailyShopCfg.ShopItems[i].GoldCost;
                if (goldCostEach < 0) goldCostEach = (int)(-goldCostEach * ConfigManager.chapterConfigs[chapterIdx].BaseGold / 100f);
                //add to list
                DailyShopItemData newItem = new DailyShopItemData
                {
                    Item = DailyShopCfg.ShopItems[i].Items[RandomUtils.GetRandomNumber(DailyShopCfg.ShopItems[i].Items.Length)],
                    Discount = RandomUtils.GetRandomNumber(DailyShopCfg.ShopItems[i].Discount[0], DailyShopCfg.ShopItems[i].Discount[1] + 1),
                    BuyCount = 0
                };

                if (DailyShopCfg.ShopItems[i].Quantity >= 0)
                {
                    newItem.ItemCount = DailyShopCfg.ShopItems[i].Quantity;
                    newItem.GemCost = (int)(DailyShopCfg.ShopItems[i].GemCost * (100 - newItem.Discount) / 100f);
                    newItem.GoldCost = (int)(newItem.ItemCount * goldCostEach * (100 - newItem.Discount) / 100f);
                }
                //theo base
                else
                {
                    if (newItem.Item == "Gold")
                    {
                        newItem.ItemCount = (int)(-DailyShopCfg.ShopItems[i].Quantity * ConfigManager.chapterConfigs[chapterIdx].BaseGold);
                    }
                    //material
                    else
                    {
                        newItem.ItemCount = (int)(-DailyShopCfg.ShopItems[i].Quantity * ConfigManager.chapterConfigs[chapterIdx].BaseMaterial);
                    }
                    newItem.GemCost = (int)(DailyShopCfg.ShopItems[i].GemCost * (100 - newItem.Discount) / 100f);
                    newItem.GoldCost = (int)(newItem.ItemCount * DailyShopCfg.ShopItems[i].GoldCost * (100 - newItem.Discount) / 100f);
                }

                listItems.Add(newItem);
                if (listItems.Count >= DailyShopCfg.MaxItem) break;
            }
        }
        return listItems;
    }
#endif

    public static int GetDailyShopChapterUnlock()
    {
        if (otherConfig.Contains("DailyShopChapterUnlock"))
        {
            int num = (int)otherConfig["DailyShopChapterUnlock"];
            return num;
        }
        else
        {
            return 3; // default unlock at chapter 4
        }
    }

    public static int GetRankIdxFromBaseStat(long baseStat)
    {
        for (int i = 0; i < LeoThapCfg.BaseStat.Length; ++i)
        {
            if (LeoThapCfg.BaseStat[i] > baseStat)
            {
                return i >= 1 ? i - 1 : 0;
            }
        }

        return 0;
    }

    public static bool ValidateUserName(string name)
    {
        name = name.Trim();
        if (string.IsNullOrEmpty(name) || name.Length > 15)
            return false;

        return System.Text.RegularExpressions.Regex.IsMatch(name, @"\b(\w+ *)+");
    }

    public static int GetAtkSpdNormalize()
    {
        var itemCfg = ConfigManager.GetItemConfig("VK_Sword");
        var advStat = itemCfg.GetAdvanceStat(1, (int)ERarity.Common);
        int atkSpd = 170;
        for (int i = 0; i < advStat.Length; ++i)
        {
            var statMod = advStat[i];
            if (statMod.Stat == EStatType.ATK_SPD &&
                statMod.Mod == EStatModType.ADD)
            {
                atkSpd = (int)statMod.Val;
            }
        }
        return atkSpd;
    }

    public static long NormalizePlayerDMG(long originDMG, float originATKSPD)
    {
        var atkSpd = GetAtkSpdNormalize();
        var originDPS = originDMG * originATKSPD;
        var refDMG = (long)System.Math.Round(originDPS / atkSpd * 100);
        return refDMG;
    }

    public static string GetCardDropFromUnit(string unit, int cardRarity)
    {
        foreach (var k in ConfigManager.CardStats)
        {
            if ((int)k.Value.Rarity == cardRarity &&
                k.Value.UnitsDrop != null &&
                k.Value.UnitsDrop.Contains(unit))
            {
                return k.Key;
            }
        }

        return string.Empty;
    }

    public static string GetCardDropFromUnit(string unit)
    {
        foreach (var k in ConfigManager.CardStats)
        {
            if (//(int)k.Value.Rarity == cardRarity &&
                k.Value.UnitsDrop != null &&
                k.Value.UnitsDrop.Contains(unit))
            {
                return k.Key;
            }
        }

        return string.Empty;
    }

    public static long GetKinhNghiemSuPhuTruyenCong()
    {
        if (otherConfig.Contains("ExpMaster"))
        {
            var cfg = otherConfig["ExpMaster"];

            return cfg.ToLong();
        }
        else
        {
            return 200;
        }
    }

    public static long GetATKBonusStatMaster(long baseATK)
    {
        int heSo = 20;

        if (otherConfig.Contains("ATKStatMaster"))
        {
            var cfg = otherConfig["ATKStatMaster"];

            heSo = (int)cfg.ToLong();
        }

        return baseATK * heSo / 100;
    }

    public static long GetHPBonusStatMaster(long baseATK)
    {
        int heSo = 500;
        if (otherConfig.Contains("HPRateStatMaster"))
        {
            var cfg = otherConfig["HPRateStatMaster"];

            heSo = (int)cfg.ToLong();
        }

        var atkBonus = GetATKBonusStatMaster(baseATK);
        return atkBonus * heSo / 100;
    }

    public static UnitStat InitStatEnemyUnit_FarmMode(UnitStat originStat, FarmConfig farmConfig, int segmentIdx, BattleData battleData)
    {
        int hpBuff = farmConfig.HPBuff;
        long dmg = farmConfig.DMG;

        if (battleData != null)
        {
            var playerOriginStat = UnitStat.FromUnitStat(battleData.OriginStat);
            var originDmg = ConfigManager.NormalizePlayerDMG(playerOriginStat.DMG, playerOriginStat.ATK_SPD);
            hpBuff = (int)System.Math.Round((originDmg / 100f - 1f) * 100);
            dmg = (long)System.Math.Round((playerOriginStat.HP / 500f) * farmConfig.DMG);
        }

        var buffStat = originStat;
        buffStat.HP = originStat.HP + originStat.HP * farmConfig.GetHPBuff(hpBuff, segmentIdx) / 100;
        buffStat.DMG = farmConfig.GetDMG(dmg, segmentIdx);
        buffStat.COLLISION_DMG = buffStat.DMG;

        return buffStat;
    }

    public static UnitStat InitStatEnemyUnit_LeoThap(UnitStat originStat, int leoThapRankIdx, int missionID, BattleData battleData)
    {
        var buffStat = originStat;
        var configRank = ConfigManager.LeoThapCfg.GetLeoThapRankCfg(leoThapRankIdx);

        var dmg = ConfigManager.LeoThapBattleCfg.GetDMG(configRank.BaseStat,
            missionID - 1);
        var hpBuff = ConfigManager.LeoThapBattleCfg.GetHPBuff(configRank.BaseStat,
            missionID - 1);
        var hp = originStat.HP + originStat.HP * hpBuff / 100;
        buffStat.HP = hp;
        buffStat.DMG = dmg;
        buffStat.COLLISION_DMG = buffStat.DMG;

        return buffStat;
    }

    public static UnitStat InitStatEnemyUnit_SanThe(UnitStat originStat, int missionID, BattleData battleData, bool isBoss)
    {
        var buffStat = originStat;
        if (battleData != null && battleData.SanThe != null)
        {
            var sanTheBattleData = battleData.SanThe;

            var dmg = sanTheBattleData.HeSoATK * ConfigManager.SanTheCfg.GetHeSoFinalATK(missionID) / 100;
            var hp = originStat.HP * sanTheBattleData.HeSoHP / 100 * ConfigManager.SanTheCfg.GetHeSoFinalHP(missionID) / 100;

            if (isBoss)
            {
                hp = hp * ConfigManager.SanTheCfg.HeSoHPBoss / 100;
            }
            else
            {
                hp = hp * ConfigManager.SanTheCfg.HeSoHPQuai / 100;
            }

            buffStat.HP = hp;
            buffStat.DMG = dmg;
            buffStat.COLLISION_DMG = buffStat.DMG;
        }

        return buffStat;
    }

    public static UnitStat InitStatEnemyUnit_ThachDau(UnitStat originStat, int missionID, BattleData battleData)
    {
        var buffStat = originStat;

        var dmg = ConfigManager.ThachDauBattleCfg.GetDMG(missionID - 1);
        var hpBuff = ConfigManager.ThachDauBattleCfg.GetHPBuff(missionID - 1);
        var hp = originStat.HP + originStat.HP * hpBuff / 100;
        buffStat.HP = hp;
        buffStat.DMG = dmg;
        buffStat.COLLISION_DMG = buffStat.DMG;

        return buffStat;
    }

    public static UnitStat InitStatEnemyUnit_NienThu(UnitStat originStat, BattleData battleData)
    {
        var buffStat = originStat;

        var dmg = ConfigManager.TetEventCfg.EnemyBaseDMG;
        var hp = ConfigManager.TetEventCfg.EnemyBaseDMG * originStat.HP / originStat.DMG;
        buffStat.HP = hp;
        buffStat.DMG = dmg;
        buffStat.COLLISION_DMG = buffStat.DMG;

        return buffStat;
    }

    public static UnitStat InitStatEnemyUnit_Normal(UnitStat originStat, List<ENodeType> pathCfg, int hpBuff, long dmgCfg, BattleData battleData, int hpBuffWave = 0)
    {
        var buffStat = originStat;

        buffStat.HP = (int)((originStat.HP + originStat.HP * ConfigManager.GetHPBuffByMapConfig(pathCfg, hpBuff) / 100) * (1 + hpBuffWave / 100f));
        buffStat.DMG = ConfigManager.GetDMGByMapConfig(pathCfg, dmgCfg);

        buffStat.COLLISION_DMG = buffStat.DMG;

        return buffStat;
    }

    public static int GetHPBuffByMapConfig(List<ENodeType> nodePaths, int hpBuff)
    {
        int total = hpBuff;
        int totalScale = 0;
        for (int i = 0; i < nodePaths.Count - 1; ++i)
        {
            int nodeScale = 0;
            var nodeType = nodePaths[i];
            if (nodeType == ENodeType.Angel)
            {
                nodeScale = ConfigManager.GetAngelNodeHPBuff();
            }
            else if (nodeType == ENodeType.Boss)
            {
                nodeScale = ConfigManager.GetBossNodeHPBuff();
            }
            else if (nodeType == ENodeType.Event)
            {
                nodeScale = ConfigManager.GetEventNodeHPBuff();
            }
            else if (nodeType == ENodeType.Quai1 ||
                nodeType == ENodeType.Quai2) // node quai
            {
                nodeScale = ConfigManager.GetCreepNodeHPBuff();
            }

            totalScale += nodeScale;
        }

        return total + total * totalScale / 100;
    }

    public static long GetDMGByMapConfig(List<ENodeType> nodePaths, long dmg)
    {
        int totalScale = 0;
        for (int i = 0; i < nodePaths.Count - 1; ++i)
        {
            int nodeScale = 0;
            var nodeType = nodePaths[i];
            if (nodeType == ENodeType.Angel)
            {
                nodeScale = ConfigManager.GetAngelNodeDMGBuff();
            }
            else if (nodeType == ENodeType.Boss)
            {
                nodeScale = ConfigManager.GetBossNodeDMGBuff();
            }
            else if (nodeType == ENodeType.Event)
            {
                nodeScale = ConfigManager.GetEventNodeDMGBuff();
            }
            else if (nodeType == ENodeType.Quai1 ||
                nodeType == ENodeType.Quai2) // node quai
            {
                nodeScale = ConfigManager.GetCreepNodeDMGBuff();
            }

            totalScale += nodeScale;
        }
        return dmg + dmg * totalScale / 100;
    }


    public static int GetEventNodeDMGBuff()
    {
        if (otherConfig.Contains("EventNodeDMGBuff"))
        {
            int hp = (int)otherConfig["EventNodeDMGBuff"];
            return hp;
        }
        else
        {
            return 0;
        }
    }

    public static int GetEventNodeHPBuff()
    {
        if (otherConfig.Contains("EventNodeHPBuff"))
        {
            int hp = (int)otherConfig["EventNodeHPBuff"];
            return hp;
        }
        else
        {
            return 0;
        }
    }
}

public enum EBattleEventRoom
{
    Angel,          // 0
    GoldFarm,       // 1
    MatFarm,        // 2
    GemFarm,        // 3
    GreedyGoblin,   // 4
    SecretShop,     // 5
    BlackSmith,     // 6
    Devil,          // 7
    Fighting,       // 8
    Trap,           // 9
    ExpMaster,      // 10
    RangeMinion,    // 11
    StatMaster      // 12
}

public enum ENodeType
{
    Start,
    Boss,
    Angel,
    Ruong,
    Quai1,
    Quai2,
    Event,
}