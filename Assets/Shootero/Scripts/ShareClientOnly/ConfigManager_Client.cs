using Hiker.GUI;
using Hiker.Networks.Data.Shootero;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ConfigManager
{
    public static string language;
    public static void InitLanguage()
    {
        language = GetDefautLangue();
        Localization.language = language;//Localization.knownLanguages[languageIdx];
    }

    public static string GetDefautLangue()
    {
        string langue = PlayerPrefs.GetString("language", Application.systemLanguage.ToString());

        // PhuongTD : convert ChineseSimplified+ChineseTraditional to Chinese
        if (langue.Contains("Chinese"))
            langue = "Chinese";

        //langue supported
        if (ConfigManager.supportLangues.Contains(langue))
        {
            return langue;
        }
        else if (ConfigManager.loaded == false && // chua load config thi cho phep doc ngon ngu hien tai dang co
            Localization.knownLanguages != null &&
            System.Array.Exists(Localization.knownLanguages, e => e == langue))
        {
            return langue;
        }

        return ConfigManager.supportLangues[0]; //otherwise return english
    }

    public static void SetLanguage(string language)
    {
        if (supportLangues.Contains(language) == false)
        {
            return;
        }
        PlayerPrefs.SetString("language", language);
        GameClient.instance.RestartGame();
    }
    public static int GetDevilMaxHPTradeOff()
    {
        if (otherConfig.Contains("DevilMaxHPTradeOff"))
        {
            int hp = (int)otherConfig["DevilMaxHPTradeOff"];
            return hp;
        }
        else
        {
            return 25;
        }
    }
    public static int GetTrapHPPercent()
    {
        if (otherConfig.Contains("TrapHPPercent"))
        {
            int hp = (int)otherConfig["TrapHPPercent"];
            return hp;
        }
        else
        {
            return 25;
        }
    }
    public static int GetTrapGoldPercent()
    {
        if (otherConfig.Contains("TrapGoldPercent"))
        {
            int hp = (int)otherConfig["TrapGoldPercent"];
            return hp;
        }
        else
        {
            return 25;
        }
    }
    /// <summary>
    ///  power-up, gold, hp
    /// </summary>
    /// <returns>power-up, gold, hp</returns>
    public static int[] GetTrapRate()
    {
        if (otherConfig.Contains("TrapRate"))
        {
            var cfg = otherConfig["TrapRate"];
            int[] result = new int[cfg.Count];
            for (int i = 0; i < cfg.Count; ++i)
            {
                result[i] = cfg[i].ToInt();
            }
            return result;
        }
        else
        {
            return new int[] { 30, 30, 40 }; // power-up, gold, hp
        }
    }

    // moved to ConfigManager.cs
    //public static long GetKinhNghiemSuPhuTruyenCong()
    //{
    //    if (otherConfig.Contains("ExpMaster"))
    //    {
    //        var cfg = otherConfig["ExpMaster"];
            
    //        return cfg.ToLong();
    //    }
    //    else
    //    {
    //        return 200;
    //    }
    //}

    public static int GetATKRateThoNgoc()
    {
        if (otherConfig.Contains("ATKRateThoNgoc"))
        {
            var cfg = otherConfig["ATKRateThoNgoc"];

            return cfg.ToInt();
        }
        else
        {
            return 100;
        }
    }

    public static int GetATKRateMedic()
    {
        if (otherConfig.Contains("ATKRateMedic"))
        {
            var cfg = otherConfig["ATKRateMedic"];

            return cfg.ToInt();
        }
        else
        {
            return 50;
        }
    }

    public static int GetATKRateFalcon()
    {
        if (otherConfig.Contains("ATKRateFalcon"))
        {
            var cfg = otherConfig["ATKRateFalcon"];

            return cfg.ToInt();
        }
        else
        {
            return 50;
        }
    }

    public static int GetATKRateRocket()
    {
        if (otherConfig.Contains("ATKRateRocket"))
        {
            var cfg = otherConfig["ATKRateRocket"];

            return cfg.ToInt();
        }
        else
        {
            return 100;
        }
    }

    public static Vector3 GetHeroViewRotation(string heroName)
    {
        if (otherConfig.Contains("HeroViewRotation"))
        {
            Dictionary<string, Vector3> dic = JsonMapper.ToObject<Dictionary<string, Vector3>>(otherConfig["HeroViewRotation"].ToJson());
            if (dic.ContainsKey(heroName))
                return dic[heroName];
            else
                return Vector3.zero;
        }
        else
        {
            return Vector3.zero;
        }
    }

    public static Vector3 GetHeroViewScale(string heroName)
    {
        if (otherConfig.Contains("HeroViewScale"))
        {
            Dictionary<string, Vector3> dic = JsonMapper.ToObject<Dictionary<string, Vector3>>(otherConfig["HeroViewScale"].ToJson());
            if (dic.ContainsKey(heroName))
                return dic[heroName];
            else
                return Vector3.one;
        }
        else
        {
            return Vector3.one;
        }
    }

    public static Vector3 GetHeroViewOffset(string heroName)
    {
        if (otherConfig.Contains("HeroViewOffset"))
        {
            Dictionary<string, Vector3> dic = JsonMapper.ToObject<Dictionary<string, Vector3>>(otherConfig["HeroViewOffset"].ToJson());
            if (dic.ContainsKey(heroName))
                return dic[heroName];
            else
                return Vector3.zero;
        }
        else
        {
            return Vector3.zero;
        }
    }

    public static int GetMaxEvasion()
    {
        if (otherConfig.Contains("MaxEvasion"))
        {
            int hp = (int)otherConfig["MaxEvasion"];
            return hp;
        }
        else
        {
            return 70;
        }
    }

    public static int GetMaxHUDQueue()
    {
        if (otherConfig.Contains("MaxHUDQueue"))
        {
            int hp = (int)otherConfig["MaxHUDQueue"];
            return hp;
        }
        else
        {
            return 10;
        }
    }

    public static string GetStatDesc(StatMod mod, int level = 0)
    {
        if (mod.Stat == EStatType.HEROPLUS)
        {
            return string.Format(Localization.Get("Stat_HeroUpgrade_Desc"), Localization.Get(mod.Target+ "_Name"), Localization.Get("Stat_HeroUpgrade_" + mod.Target) );
        }
        else if (mod.Stat == EStatType.SKILLREROLL)
        {
            return Localization.Get("Stat_SKILLREROLL");
        }
        else if (mod.Stat == EStatType.SKILLPLUS)
        {
            return string.Format(Localization.Get("Stat_SkillUpgrade_Desc"), Localization.Get("Buff_"+mod.Target), Localization.Get("Stat_SkillUpgrade_" + mod.Target));
        }
        else if (mod.Stat == EStatType.BLESSINGPLUS)
        {
            return Localization.Get("Stat_BLESSINGPLUS");
        }
        else if (mod.Stat == EStatType.DETU)
        {
            return Localization.Get("Stat_DETU_" + mod.Target);
        }
        else if (mod.Stat == EStatType.REGEN)
        {
            return string.Format( Localization.Get("Stat_REGEN") , mod.Val);
        }

        if ( mod.Stat== EStatType.STARTSKILL || mod.Stat == EStatType.AFKREWARD )
        {
            return Localization.Get("Stat_" + mod.Stat.ToString());
        }

        if (mod.Stat == EStatType.GETSKILL)
        {
            string op_desc = "";
            //if (mod.Val > 0)
            //{
            //    op_desc = string.Format(Localization.Get("Stat_GETSKILL_" + mod.Target + "_" + mod.Val));
            //}
            //else
            //{
            //    op_desc = string.Format(Localization.Get("Stat_GETSKILL_" + mod.Target));
            //}

            //if (op_desc.StartsWith("Stat_GETSKILL_")) // dont have custom desc
            {
                string buff_name = "Buff_" + mod.Target;
                if (mod.Val > 0)
                {
                    op_desc = string.Format(Localization.Get("Option_Skill_lv"), Localization.Get(buff_name), mod.Val);
                }
                else
                {
                    op_desc = string.Format(Localization.Get("Option_Skill"), Localization.Get(buff_name));
                }
            }

            return op_desc;
        }
        else
        if (mod.Stat == EStatType.ATK_ON)
        {
            return string.Format("{0} + {1}%",
                string.Format(Localization.Get("Stat_ATK_ON"),
                Localization.Get(mod.Target)), mod.Val + mod.GetTotalIncByLevel(level));
        }
        else
        if (mod.Stat == EStatType.DEF_ON)
        {
            return string.Format("{0} + {1}%",
                string.Format(Localization.Get("Stat_DEF_ON"),
                Localization.Get(mod.Target)), mod.Val + mod.GetTotalIncByLevel(level));
        }

        if (mod.Mod == EStatModType.ADD)
        {
            if (mod.Stat == EStatType.CRIT)
            {
                return string.Format("{0} + {1}%",
                    Localization.Get("Stat_" + mod.Stat.ToString()),
                    mod.Val + mod.GetTotalIncByLevel(level));
            }
            else if (mod.Stat == EStatType.CRIT_DMG)
            {
                return string.Format("{0} + {1}%",
                    Localization.Get("Stat_" + mod.Stat.ToString()),
                    mod.Val + mod.GetTotalIncByLevel(level));
            }
            else if (mod.Stat == EStatType.ELE_DMG)
            {
                return string.Format("{0} + {1}%",
                    Localization.Get("Stat_" + mod.Stat.ToString()),
                    mod.Val + mod.GetTotalIncByLevel(level));
            }
            else if (mod.Stat == EStatType.DMG_ON_FREEZE)
            {
                return string.Format(Localization.Get("Stat_" + mod.Stat.ToString()),
                    mod.Val + mod.GetTotalIncByLevel(level));
            }

            //if (level > 0)
            //{
            //    return string.Format("{0} + {1}(+{2})",
            //        Localization.Get("Stat_" + mod.Stat.ToString()),
            //        mod.Val, mod.GetIncByLevel(level));
            //}
            //else
            {
                return string.Format("{0} + {1}",
                    Localization.Get("Stat_" + mod.Stat.ToString()),
                    mod.Val + mod.GetTotalIncByLevel(level));
            }
        }
        else if (mod.Mod == EStatModType.BUFF)
        {
            return string.Format("{0} + {1}%",
                    Localization.Get("Stat_" + mod.Stat.ToString()),
                    mod.Val + mod.GetTotalIncByLevel(level));
        }
        else if (mod.Mod == EStatModType.MUL)
        {
            return string.Format("{0} * {1}%",
                    Localization.Get("Stat_" + mod.Stat.ToString()),
                    mod.Val + mod.GetTotalIncByLevel(level));
        }
        else
        {
            return string.Empty;
        }
    }

    //public static void ReadConfigString(string configFileName, System.Action<string> onCompleted)
    //{
    //    if (configFileName.EndsWith(".txt") == false)
    //    {
    //        configFileName += ".txt";
    //    }
    //    Addressables.LoadAssetAsync<TextAsset>(configFileName).Completed += (re) =>
    //    {
    //        if (re.Result != null)
    //        {
    //            onCompleted(re.Result.text);
    //        }
    //        else
    //        {
    //            onCompleted(string.Empty);
    //        }
    //    };

    //    //if (textAsset == null)
    //    //{
    //    //    return string.Empty;
    //    //}
    //    //return textAsset.text;
    //}

    public static string ReadConfigString(string configFileName, string path = null)
    {
        TextAsset textAsset = null;
        if (LoaderBundleManager.instance != null) textAsset = LoaderBundleManager.instance.GetAssetFileConfig(configFileName);

        if (path == null)
        {
            path = string.Format("Configs_{0}/", GameClient.GameVersion);
            //path = string.Format("Configs/");
        }

        //Path = path;

        if (textAsset == null) textAsset = Resources.Load(path + configFileName) as TextAsset;

        if (textAsset == null)
        {
            return string.Empty;
        }
        return textAsset.text;
    }

    //public static void ReadUnitStats(string configFileName, System.Action<Dictionary<string, UnitStat>> onCompleted)
    //{
    //    ReadConfigString(configFileName, (text) =>
    //    {
    //        if (string.IsNullOrEmpty(text))
    //        {
    //            onCompleted(null);
    //        }
    //        else
    //        {
    //            var wrapper = JsonMapper.ToObject<Dictionary<string, UnitStatWrapper>>(text);
    //            Dictionary<string, UnitStat> result = new Dictionary<string, UnitStat>();
    //            foreach (var s in wrapper)
    //            {
    //                result.Add(s.Key, UnitStat.FromUnitStat(s.Value));
    //            }
    //            onCompleted(result);
    //        }
    //    });
    //}
    //public static void ReadConfig(string configFileName, System.Action<JsonData> onCompleted)
    //{
    //    ReadConfigString(configFileName, (text) =>
    //    {
    //        if (string.IsNullOrEmpty(text))
    //        {
    //            onCompleted(null);
    //            return;
    //        }
    //        onCompleted(JsonMapper.ToObject(text));
    //    });

    //}
    //public static void ReadConfig<T>(string configFileName, System.Action<T> onCompleted)
    //{
    //    ReadConfigString(configFileName, (text) =>
    //    {
    //        if (string.IsNullOrEmpty(text))
    //        {
    //            onCompleted(default(T));
    //            return;
    //        }
    //        onCompleted(JsonMapper.ToObject<T>(text));
    //    });

    //}
    //public static void ReadConfig<T>(string configFileName, object dat, System.Action<T, object> onCompleted)
    //{
    //    ReadConfigString(configFileName, (text) =>
    //    {
    //        if (string.IsNullOrEmpty(text))
    //        {
    //            onCompleted(default(T), dat);
    //            return;
    //        }
    //        onCompleted(JsonMapper.ToObject<T>(text), dat);
    //    });

    //}

    public static JsonData battleABConfig { get; private set; }
    public static void LoadConfigs_Client()
    {
        loaded = true;
        LoadConfigs();
        battleABConfig = ReadConfig("battle");

        LoadLocalization_Client();
        InitLanguage();
        //ReadConfig("otherConfig", (re) =>
        //{
        //    otherConfig = re;



        //    ReadUnitStats("UnitStat", (re2) =>
        //    {
        //        UnitStats = re2;
        //        ReadUnitStats("HeroStat", (heroStats) =>
        //        {
        //            if (heroStats != null)
        //            {
        //                foreach (var hero in heroStats)
        //                {
        //                    if (UnitStats.ContainsKey(hero.Key))
        //                    {
        //                        UnitStats[hero.Key] = hero.Value;
        //                    }
        //                    else
        //                    {
        //                        UnitStats.Add(hero.Key, hero.Value);
        //                    }
        //                }
        //            }
        //        });
        //    });
        //    ReadConfig<Dictionary<string, ItemConfig>>("ItemStat", (re2) =>
        //    {
        //        ItemStats = re2;
        //    });
        //    ReadConfig<Dictionary<string, StatMod[]>>("ArmoryStat", (re2) =>
        //    {
        //        ArmoryStats = re2;
        //    });
        //    ReadConfig<Dictionary<string, float>>("Material", (re2) =>
        //    {
        //        Materials = re2;
        //    });
        //    ReadConfig<Dictionary<string, UpgradeConfig>>("UpgradeConfig", (re2) =>
        //    {
        //        UpgradeCfg = re2;
        //    });
        //    ReadConfig<Dictionary<string, ChestConfig>>("ChestConfig", (re2) =>
        //    {
        //        ChestCfg = re2;
        //    });
        //    ReadConfig<VongQuayConfig>("VongQuay", (re2) =>
        //    {
        //        VongQuay = re2;
        //    });
        //    ReadConfig<GameConfig>("GameConfig", (re2) =>
        //    {
        //        gameConfig = re2;
        //    });

        //    int numChap = (int)otherConfig["numChapter"];
        //    chapterConfigs = new ChapterConfig[numChap];
        //    FarmConfigs = new Dictionary<string, FarmConfig>();
        //    for (int i = 1; i <= numChap; ++i)
        //    {
        //        ReadConfig<ChapterConfig>("Chapters/Chapter" + i, i - 1, (chapCfg, index) =>
        //        {
        //            chapterConfigs[(int)index] = chapCfg;
        //            foreach (var farmName in chapCfg.FarmCfgs)
        //            {
        //                ReadConfig<FarmConfig>("Farms/" + farmName, farmName, (farmCfg, data) =>
        //                {
        //                    string fName = (string)data;
        //                    if (FarmConfigs.ContainsKey(fName))
        //                    {
        //                        FarmConfigs[fName] = farmCfg;
        //                    }
        //                    else
        //                    {
        //                        FarmConfigs.Add(fName, farmCfg);
        //                    }
        //                });
        //            }
        //        });
        //    }

        //    ReadConfig("cashShop", (re2) =>
        //    {
        //        cashShopConfig = re2;
        //    });



        //});

        //foreach (var t in ItemStats.Keys)
        //{
        //    if (UpgradeCfg.ContainsKey(t) == false)
        //    {
        //        Debug.LogWarning("Missing UpgradeConfig " + t);
        //    }
        //}
    }
    public static void LoadLocalization_Client()
    {
        ResourceLoadLocalization("Localization");
        //ResourceLoadLocalization("Localization/Localization_BR");
        //ResourceLoadLocalization("Localization/Localization_RU");
        //ResourceLoadLocalization("GoogleDrive/BBLocalization/Localization_CD");
        //ResourceLoadLocalization("GoogleDrive/BBLocalization/buildingsLocalization");
        //ResourceLoadLocalization("GoogleDrive/BBLocalization/tradingsLocalization");
        //ResourceLoadLocalization("GoogleDrive/BBLocalization/abilitiesLocalization");
        //ResourceLoadLocalization("GoogleDrive/BBLocalization/unitsLocalization");
        //ResourceLoadLocalization("GoogleDrive/BBLocalization/achievementsLocalization");
        //ResourceLoadLocalization("GoogleDrive/BBLocalization/locationLocalization");
        //ResourceLoadLocalization("GoogleDrive/BBLocalization/clanWarLocalization");
        //ResourceLoadLocalization("GoogleDrive/BBLocalization/materialsLocalization");
        //ResourceLoadLocalization("GoogleDrive/BBLocalization/encyclopediaLocalization");
        //ResourceLoadLocalization("GoogleDrive/BBLocalization/campaignsLocalization");
    }
    private static void ResourceLoadLocalization(string resPath)
    {
        TextAsset textAsset = null;
        if (LoaderBundleManager.instance != null) textAsset = LoaderBundleManager.instance.GetAssetFileConfig(resPath);

        if (textAsset == null) textAsset = Resources.Load<TextAsset>(resPath);
        //if (resPath.EndsWith(".csv") == false)
        //{
        //    resPath += ".csv";
        //}
        //Addressables.LoadAssetAsync<TextAsset>(resPath).Completed += (re) => {
        //    if (re.Result != null)
        //    {
        //    }
        //    onCompleted?.Invoke();
        //};
        
        Localization.LoadCSV(textAsset.bytes, true);
        Resources.UnloadAsset(textAsset);
    }

    public static string GetMoTaPhanThuong(string itemName)
    {
        if (itemName.StartsWith("H_"))
        {
            return Localization.Get("RewardHeroDesc");
        }
        else
        {
            return Localization.Get(itemName + "_Desc");
        }
    }

    public static string GetDisplayPhanThuong(string itemName)
    {
        if (itemName == Hiker.Networks.Data.Shootero.CardReward.GEM_CARD)
        {
            return GetDisplayItem(itemName);
        }
        else
        if (itemName == Hiker.Networks.Data.Shootero.CardReward.GOLD_CARD)
        {
            return GetDisplayItem(itemName);
        }
        else
        if (ConfigManager.Materials.ContainsKey(itemName))
        {
            return GetDisplayMaterial(itemName);
        }
        else if (itemName.StartsWith("H_"))
        {
            return GetDisplayHero(itemName.Substring(2));
        }
        else
        {
            return GetDisplayItem(itemName);
        }
    }

    public static string GetDisplayHero(string codeName)
    {
        return Localization.Get(codeName + "_Name");
    }
    public static string GetDisplayItem(string codeName)
    {
        return Localization.Get(codeName + "_Display");
    }
    public static string GetDisplayMaterial(string codeName)
    {
        return Localization.Get(codeName + "_Name");
    }

    public static int GetChapUnlockCommunity()
    {
        if (otherConfig.Contains("ChapUnlockCommunity"))
        {
            int num = (int)otherConfig["ChapUnlockCommunity"];
            return num;
        }
        else
        {
            return 0; // default alway unlock
        }
    }

    public static int GetChapUnlockAdvanceStat()
    {
        if (otherConfig.Contains("ChapUnlockAdvanceStat"))
        {
            int num = (int)otherConfig["ChapUnlockAdvanceStat"];
            return num;
        }
        else
        {
            return 0; // default alway unlock
        }
    }

    public static int GetPaladinSkillUpgradeCooldown()
    {
        if (otherConfig.Contains("PaladinSkillUpgradeCooldown"))
        {
            int num = (int)otherConfig["PaladinSkillUpgradeCooldown"];
            return num;
        }
        else
        {
            return 0; // default won't change cooldown
        }
    }

    public static int GetWukongIllusionScale()
    {
        if (otherConfig.Contains("WukongIllusionScale"))
        {
            int num = (int)otherConfig["WukongIllusionScale"];
            return num;
        }
        else
        {
            return 0; // default won't scale
        }
    }

    public static string GetBossAvatar(string unitName)
    {
        var stat = GetUnitStat(unitName);
        if (string.IsNullOrEmpty(stat.Ava))
        {
            return unitName;
        }
        return stat.Ava;
    }
    public static string GetBossName(string unitName)
    {
        var cardKey = ConfigManager.GetCardDropFromUnit(unitName, (int)ERarity.Legend);
        if (string.IsNullOrEmpty(cardKey)) return string.Empty;

        return Localization.Get(string.Format("{0}_Display", cardKey));
    }
    public static EStatType[] GetAdvanceStatShow()
    {
        if (otherConfig.Contains("AdvanceStatShow") == false) return null;
        var json = otherConfig["AdvanceStatShow"];
        if (json.IsArray == false) return null;
        
        var result = new EStatType[json.Count];
        for (int i = 0; i < json.Count; ++i)
        {
            result[i] = (EStatType)System.Enum.Parse(typeof(EStatType), json[i].ToString());
        }
        return result;
    }

    public static bool IsHeroSkillSwipe(string skillName)
    {
        var skillCfg = GetHeroSkillCfg(skillName);
        return skillCfg.Contains("Swipe") && skillCfg["Swipe"].ToBoolean();
    }

    public static float GetThoiGianBatCheatVideoQuangCao()
    {
        if (otherConfig.Contains("ThoiGianBatCheatQCVideo") == false) return 10;
        var json = otherConfig["ThoiGianBatCheatQCVideo"].ToFloat();
        return json;
    }

    public static float GetThoiGianBatCheatVideoQuangCaoInters()
    {
        if (otherConfig.Contains("ThoiGianBatCheatQCInter") == false) return 10;
        var json = otherConfig["ThoiGianBatCheatQCInter"].ToFloat();
        return json;
    }

    static SpriteCollection spIcon;
    public static Sprite GetIconSprite(string name)
    {
        if (spIcon == null)
        {
            spIcon = Resources.Load<SpriteCollection>("IconSprite");
        }
        if (spIcon)
        {
            return spIcon.GetSprite(name);
        }
        return null;
    }
}
