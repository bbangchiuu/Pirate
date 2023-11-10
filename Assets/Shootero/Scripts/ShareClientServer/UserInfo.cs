using System;
using System.Collections;
using System.Collections.Generic;
using DateTime = System.DateTime;


namespace Hiker.Networks.Data.Shootero
{
    [System.Serializable]
    public class LeoThapGroup
    {
        public string ID;
        public long[] Users;
        public int UserCount;
        public int RankIdx;
        public DateTime STime;
        public DateTime ETime;
        public int Mod;
    }

    [System.Serializable]
    public class GamerLeoThapData
    {
        public string ID;
        public long GID;
        public string GrpID;
        public int LuotChoi;
        public int Level;
        public long ClearedTime;
        public DateTime JoinTime;
        public DateTime LastPlayTime;
        public bool GetRewarded;
        public string Name;
    }

    [System.Serializable]
    public class ThachDauGroup
    {
        public string ID;
        public long[] Users;
        public int UserCount;
        public DateTime STime;
        public DateTime ETime;
        public string[] Levels;
        public int RankIdx;
    }

    [System.Serializable]
    public class GamerThachDauData
    {
        public string ID;
        public long GID;
        public string GrpID;
        public int LuotChoi;
        public int Level;
        public long ClearedTime;
        public DateTime JoinTime;
        public DateTime LastPlayTime;
        public bool GetRewarded;
        public string Name;
    }

    [System.Serializable]
    public class GamerData
    {
        public long GID;
        public string Token;
        public string Name;
        public string Lang;
        public string Platform;
        public long Gold;
        public int DebtGem;
        public int Gem;
        public GTTG TheLuc;
        public GTTG GemFarmEvent;
        public GTTG ReviveAds;
        public string Hero;
        public DateTime RegisterTime;
        public DateTime LastLoginTime;

        public DateTime BanTime;
        public string BanMsg;

        public int PurchaseCount;
        public int PurchaseValue;
        public int GemBought;
        public int LoseBattle;
#if SERVER_1_3
        public int ComeBack;
        public int LanDauHetTheLuc;
#endif
        public long BaseStat;
        public int GemLeoThap;
        public List<string> DisplayName;
        public Dictionary<string, List<string>> reforgeHistory;
        public string PushToken;
        public int HackedBattle;
        public DateTime PremiumEndTime;
        public DateTime LastReceivePremiumTime;
        public TruongThanhGamerData TruongThanhData;

        public string DeviceModel;
        public string OperatingSystem;
        public string GhiChu;
        public int LvlCode;
        public string PkgName;
        public string InstName;
        public string InstMode;
        public string DeviceID;

        public string GetDisplayName()
        {
            if (DisplayName == null || DisplayName.Count == 0)
            {
                return string.Empty;
            }

            return Name;
        }

        private void AddGold(long add, string mota = "")
        {

            this.Gold += add;
            if (this.Gold < 0) this.Gold = 0;
            //if (add > 0)
            //{
            //    if (this.gold > this.limitGold)
            //    {
            //        add -= (this.gold - this.limitGold);
            //        this.gold = this.limitGold;

            //    }
            //}

#if SERVER_CODE
            if (add != 0) GameServer.Database.LogUI.LogToMongo(this.GID, this.Name, ConfigManager.GoldName, mota, this.Gold, add);
#endif
        }

        public void CongGold(long add, string mota = "")
        {
            if (add > 0)
            {
                AddGold(add, mota);
            }
        }

        public void TruGold(long add, string mota = "")
        {
            if (add > 0)
            {
                AddGold(-add, mota);
            }
        }

        private void AddGem(int add, string mota = "")
        {
            this.Gem += add;
#if SERVER_CODE
            if (add != 0) GameServer.Database.LogUI.LogToMongo(this.GID, this.Name, ConfigManager.GemName, mota, this.Gem, add);
#endif
            CheckDebt(mota);
        }

        public void TruGem(int add, string mota = "")
        {
            if (add > 0)
            {
                AddGem(-add, mota);
            }
        }

        public void CongGem(int add, string mota = "")
        {
            if (add > 0)
            {
                AddGem(add, mota);
            }
        }

        private void CheckDebt(string mota)
        {
            if (DebtGem > 0)
            {
                var decVal = System.Math.Min(Gem, DebtGem);

                if (decVal > 0)
                {
                    Gem -= decVal;
                    DebtGem -= decVal;
#if SERVER_CODE
                    GameServer.Database.LogUI.LogToMongo(this.GID, this.Name, "debtGem", "Pay debt gem", DebtGem, -decVal);
#endif
                }
            }
        }
    }

    [System.Serializable]
    public class GTTG
    {
        public int Val;
        public int MaxVal;
        public DateTime LastUpdate;

        public bool IsFull() { return Val >= MaxVal; }

        public void UpdateByTime(DateTime timeUpdate, long cycleInSeconds)
        {
            var ts = timeUpdate - LastUpdate;
            long seconds = (long)ts.TotalSeconds;
            var numCycle = seconds / cycleInSeconds;
            int addCycle = numCycle < MaxVal ? (int)numCycle : MaxVal; // chong tran so do qua lau khong update

            //Val += addCycle;
            //if (Val > MaxVal)
            //{
            //    Val = MaxVal;
            //}

            //khi val > maxVal thi giu nguyen

            if (Val < MaxVal)
            {
                Val += addCycle;
                if (Val > MaxVal) Val = MaxVal;
            }
            LastUpdate = LastUpdate.AddSeconds(numCycle * cycleInSeconds);
        }
    }

    [System.Serializable]
    public class TrangBiData
    {
        public string ID;
        public long GID;
        public string Name;
        public int Level;
        public ERarity Rarity;
        public int Star;
        public List<string> CardSlots;
    }

    [System.Serializable]
    public class CardData
    {
        public string ID;
        public long GID;
        public string Name;
        public ERarity Rarity;
    }

    [System.Serializable]
    public class ArmoryData
    {
        public string ID;
        public long GID;
        public string Name;
        public int Level;
    }

    [System.Serializable]
    public class EquipmentSlot
    {
        public SlotType Slot;
        public string TrangBi;
    }

    [System.Serializable]
    public class HeroData
    {
        public string ID { get; set; }
        public long GID;
        public string Name;
        public int Level;

        public EquipmentSlot[] ListSlots;

        public HeroData()
        {
            ListSlots = new EquipmentSlot[]
            {
                new EquipmentSlot { Slot = SlotType.VuKhi },
                new EquipmentSlot { Slot = SlotType.Mu },
                new EquipmentSlot { Slot = SlotType.Giap },
                new EquipmentSlot { Slot = SlotType.OffHand },
                //new EquipmentSlot { Slot = SlotType.Engine },
                //new EquipmentSlot { Slot = SlotType.Addon },
            };
        }
    }

    public sealed class ThongKeSTNguoiChoi
    {
        public string TenMap;
        public long STNho;
        public long STLon;
        public long STTong;
        public int LuotST;
        public int SCount;
        public int SBan;

        public List<int> Path;
        public int Mission;
        public Dictionary<string, UnitStatWrapper> UnitStat;
    }

    public sealed class ThongKeST
    {
        public int ID;
        public string Nguon;
        public long ST;
        public long PreMau;
        public long Mau;
        public long MaxMau;
        /// <summary>
        /// battle time
        /// </summary>
        public long BT;
    }
#if SERVER_1_3
    public sealed class ThongKeSung
    {
        public int SCount;
        public int SDaBan;
    }
#endif

    public class BattleData
    {
        public string ID;
        public long GID;
        public int ChapIdx;

        /// <summary>
        /// HackCode
        /// 0b0001 cheat mem
        /// 0b0010 cheat hackspeed
        /// 0b0100 cheat time
        /// 0b1000 cheat wallhack
        /// </summary>
        public int CCode;

        /// <summary>
        /// 0 = Normal
        /// 1 = FarmMode
        /// 2 = LeoThap
        /// 3 = SanThe
        /// 4 = ThachDau
        /// 5 = NienThu
        /// </summary>
        public int BattleMode;

        #region FarmMode
        public int PassedMission;
        public string FarmCfg;
        public int FarmRate;
        #endregion

        #region LeoThapMode
        public int RankIdx = -1;
        public long BattleTime;
        public string Envi;
        public string GrpLeoThap;
        #endregion

        #region CardHuntMode
        public SanTheBattleData SanThe;
        #endregion

        #region ThachDauMode
        public string GrpThachDau;
        #endregion

        public string[] KilledUnit;

        public string HeroName;
        public string VuKhiName;
        public bool IsGreedy;
        public int LuotHS;

        public UnitStatWrapper OriginStat;
        public UnitStatWrapper PlayerStat;
        public long MaxHP;
        public List<StatModWraper> ListMods;
        public List<BuffType> Buffs;
        public List<BuffType> TKBuffs;
        public int Life = 1;
        public bool IsEnd;
        public int Mat;
        public long Gold;
        public long Exp;
        public List<ThongKeSTNguoiChoi> TKTranDanh;
        public List<ThongKeST> TKSatThuong;
#if SERVER_1_3
        public ThongKeSung TKSung;
#endif
        public DateTime ExpireTime;
        public DateTime StartTime;

        public List<int> Path;
        public List<int> NodePath;
        public long MapDmgConfig;
        public int MapHPConfig;

        public int CurMissionID;
        public int CurDungeonID;
        public List<string> PlayedScene;
        public Dictionary<string, int> Loot;
        public List<StartBattleEventResponse> ListRoomEvents;
        public int BattleEventIndex;
        public List<string> RuongDaMo;
        public List<string> BossHunted;
        public bool CurLvlClear;
        public int HeroSK;
        public int LuotRR;

        public Dictionary<string, int> DeTu;

        public string BuffRandomState;
        public int BuffRandomSeed;

        public int LeoThapMod;

        public bool IsNormalBattle()
        {
            return RankIdx < 0 && string.IsNullOrEmpty(FarmCfg) && SanThe == null && BattleMode == 0;
        }
        public bool IsFarmModeBattle()
        {
            return string.IsNullOrEmpty(FarmCfg) == false && BattleMode == 1;
        }
        public bool IsLeoThapBattle()
        {
            return RankIdx >= 0 && string.IsNullOrEmpty(GrpLeoThap) == false && BattleMode == 2;
        }
        public bool IsSanTheBattle()
        {
            return SanThe != null && BattleMode == 3;
        }

        public bool IsThachDauBattle()
        {
            return BattleMode == 4;
        }

        public bool IsNienThuBattle()
        {
            return BattleMode == 5;
        }

        public bool CanRevive(int luotHS)
        {
            int maxLuot = 1;
            if (IsSanTheBattle())
            {
                maxLuot = ConfigManager.SanTheCfg.LuotHoiSinh;
            }
            return luotHS < maxLuot;
        }
    }

    [System.Serializable]
    public class ChapterData
    {
        public string ID { get; set; }
        public long GID;
        public int ChapIdx;
        public int NumBattle;
        public int LoseBattle;
        //public int CurMission;
        //public int DungeonID;
        public bool IsComplete = false;
        public List<int> Revealed = new List<int>();
        public List<string> RuongOpened = new List<string>();
    }

    [System.Serializable]
    public class BossHuntData
    {
        public string ID { get; set; }
        public long GID;
        public string Name;
        /// <summary>
        /// 0 is open mission
        /// 1 is Success
        /// 2 is Claimed
        /// </summary>
        public int Status;
    }

    [System.Serializable]
    public class MaterialData
    {
        public string ID { get; set; }
        public long GID;
        public string Name;
        public int Num;
    }

    public enum ChestType
    {
        Rare,
        Epic,
        Legend,
        None = 100
    }

    [System.Serializable]
    public class ChestData
    {
        public string ID { get; set; }
        public long GID;
        public string Name;
        public DateTime LastOpened;
        public int OpenCount;
        public int nextNumber;
        public int PseudoRandomCount;
    }

    [System.Serializable]
    public class AdsData
    {
        public string ID { get; set; }
        public long GID;
        public List<string> listAdsIDs;
        public List<string> listAdsGemIDs;
        public List<string> listAdsVongQuayIDs;
        public List<string> listAdsThemVangChapterIDs;
        public List<string> listAdsExtraAFKIDs;
        public List<string> listAdsDailyGoldIDs;
        public List<string> listAdsDailyMatIDs;
        public List<string> listAdsDailyItemIDs;
        public DateTime nextResetTime;
    }

    [System.Serializable]
    public class AFKRewardData
    {
        public string ID { get; set; }
        public long GID;
        public DateTime lastClaimTime;
        public DateTime lastDropMaterialTime;
        public CardReward afkRewards;
#if SERVER_CODE
        public void UpdateAFKRewardData(UserInfo _userInfo)
        {
            TimeSpan maxAFKTime = ConfigManager.GetMaxAFKTime();
            DateTime afkEndTime = this.lastClaimTime.Add(maxAFKTime);
            if (afkEndTime > TimeUtils.Now) afkEndTime = TimeUtils.Now;

            int crGold = this.afkRewards.GetGold();
            int totalAFKGold = (int)((afkEndTime - lastClaimTime).TotalMinutes * _userInfo.GetAFKGoldPerMin());
            afkRewards.AddGold(totalAFKGold - crGold);

            double afkMaterialPerMin = _userInfo.GetAFKMaterialPerMin();
            double afkMaterialPoint = (afkEndTime - lastDropMaterialTime).TotalMinutes * afkMaterialPerMin;
            double numOfMat = Math.Floor(afkMaterialPoint);
            //add mat by number
            for (int i = 0; i < numOfMat; i++)
            {
                var item = ConfigManager.RandomMaterial();
                afkRewards.AddReward(item, 1);
            }
            if (afkMaterialPerMin > 0)
            {
                double addedMinutes = numOfMat / afkMaterialPerMin;
                lastDropMaterialTime = lastDropMaterialTime.AddMinutes(addedMinutes);
            }
        }
#endif
    }

    //[System.Serializable]
    //public class MailData
    //{
    //    public string ID { get; set; }
    //    public long GID;
    //    public DateTime receiveTime;
    //    public bool isNew;
    //    public string content;
    //    public GeneralReward rewards;
    //    public bool isClaimed;

    //}

    [System.Serializable]
    public class UserInfo
    {
        public static readonly string[] ALL_PROPS = new string[]
        {
            "gamer",
            "heroes",
            "armories",
            "chapters",
            "trangbi",
            "materials",
            "chests",
            "offers",
            "afkreward",
            "mail",
            "bosshunt",
            "ads",
            "leothap",
            "card",
            "santhe",
            "dailyshop",
            "targetoffers",
            "thachdau",
        };

        public static readonly string[] BATTLE_PROPS = new string[]
        {
            "gamer",
            "trangbi",
            "materials",
            "heroes",
            "armories",
            "chapters",
            "card",
        };

        public long GID;
        public long ServerTimeTick;
        public GamerData Gamer;
        public List<ChapterData> ListChapters;
        public List<TrangBiData> ListTrangBi;
        public List<ArmoryData> ListArmories;
        public List<HeroData> ListHeroes;
        public List<MaterialData> ListMaterials;
        public List<ChestData> ListChests;
        public List<OfferStoreData> ListOffers;
        public List<MailData> ListMails;
        public List<BossHuntData> ListBossHunts;
        public List<CardData> listCards;
        public AFKRewardData afkRewardData;
        public AdsData adsData;
        public DailyShopData dailyShopData;
        public HalloweenServerData halloweenServerData;
        public TetEventGamerData tetEventGamerData;
        public TetEventServerData tetEventServerData;
        public GiangSinhServerData giangSinhSrvData;
        public GiangSinhGamerData giangSinhData;
        public TargetOfferData targetOfferData;

        public int LuotLeoThap;
        public int LeoThapRankIdx;
        public string BattleLeoThap;

        public string BattleSanThe;
        public int LuotSanThe;

        public int LuotThachDau;
        public string BattleThachDau;

        public string GhiChuServer;

        /// <summary>
        /// List Reward to display at client
        /// </summary>
        public List<GeneralReward> Rewards = new List<GeneralReward>();

        public int GetCurrentChapter()
        {
            int result = 0;
            if (ListChapters != null)
            {
                for (int i = ListChapters.Count - 1; i >= 0; --i)
                {
                    var chap = ListChapters[i];

                    //if (chap.CurMission >= ConfigManager.chapterConfigs[chap.ChapIdx].Missions.Length) // complete chap
                    if (chap.IsComplete && chap.ChapIdx >= result)
                    {
                        result = chap.ChapIdx + 1;
                    }
                }
            }
            if (result >= ConfigManager.GetNumChapter())
            {
                result = ConfigManager.GetNumChapter() - 1;
            }

            return result;
        }

        public int GetTotalBattle()
        {
            int total = 0;
            foreach (var chap in ListChapters)
            {
                total += chap.NumBattle;
            }
            return total;
        }

        public bool IsChapterComplete(int chapterIdx)
        {
            if (chapterIdx >= ListChapters.Count)
            {
                return false;
            }

            return ListChapters[chapterIdx].IsComplete;
            //int passedMission = GetPassedMission(chapterIdx);
            //return passedMission >= ConfigManager.chapterConfigs[chapterIdx].Missions.Length;
        }

        public bool IsChapterUnLock(int chapterIdx)
        {
            if (chapterIdx <= 0) return true;

            return IsChapterComplete(chapterIdx - 1);
        }

        public string CheckUpgradeItem(TrangBiData tb, string lang, Dictionary<string, int> listModifiedMaterial, out long changedGold, out int changedGem)
        {
            changedGold = 0;
            changedGem = 0;
            listModifiedMaterial.Clear();

            if (tb == null)
            {
                return Localization.Get("InvalidTrangBi", lang);
            }

            var maxLevel = ConfigManager.GetMaxLevelByRarity(tb.Name, tb.Rarity, tb.Star);
            if (tb.Level >= maxLevel)
            {
                return Localization.Get("MaxLevel", lang);
            }

            bool isMaxUpgrade = false;
            var requirements = ConfigManager.GetItemUpgradeRequirement(tb.Name, tb.Level, out isMaxUpgrade);
            if (isMaxUpgrade)
            {
                return Localization.Get("MaxLevel", lang);
            }

            for (int i = 0; i < requirements.Length; ++i)
            {
                var r = requirements[i];
                if (r.Num <= 0) continue;

                if (r.Res == "Gold")
                {
                    if (Gamer.Gold < r.Num)
                    {
                        return Localization.Get("NotEnoughGold", lang);
                    }
                    changedGold += r.Num;
                }
                else if (r.Res == "Gem")
                {
                    if (Gamer.Gem < r.Num)
                    {
                        return Localization.Get("NotEnoughGem", lang);
                    }
                    changedGem += (int)r.Num;
                }
                else
                {
                    int haveNum = 0;
                    if (ListMaterials != null)
                    {
                        var material = ListMaterials.Find(e => e.Name == r.Res);
                        if (material != null)
                        {
                            haveNum = material.Num;
                        }
                    }

                    if (haveNum < r.Num)
                    {
                        return Localization.Get("NotEnoughMaterial", lang);
                    }

                    if (listModifiedMaterial.ContainsKey(r.Res))
                    {
                        listModifiedMaterial[r.Res] += (int)r.Num;
                    }
                    else
                    {
                        listModifiedMaterial.Add(r.Res, (int)r.Num);
                    }
                }
            }
            return string.Empty;
        }

        //public int GetPassedMission(int chapter)
        //{
        //    int result = 0;
        //    if (ListChapters != null && ListChapters.Count > chapter)
        //    {
        //        return ListChapters[chapter].CurMission;
        //    }
        //    return result;
        //}

        public float GetAFKGoldPerMin()
        {
            float num = 0;

            var armoryData = ListArmories.Find(e => e.Name == "AfkRewardTalent");
            if (armoryData != null)
            {
                var armStats = ConfigManager.GetArmoryStat(armoryData.Name, armoryData.Level);
                num = (float)(armStats[0].Val / (24f * 60f));
            }

            return num;
        }
        public float GetAFKMaterialPerMin()
        {
            float num = 0;

            var armoryData = ListArmories.Find(e => e.Name == "AfkRewardTalent");
            if (armoryData != null)
            {
                var armStats = ConfigManager.GetArmoryStat(armoryData.Name, armoryData.Level);
                num = (float)(armStats[1].Val / (24f * 60f));
            }

            return num;
        }

        public void Update(UserInfo other)
        {
            if (other != null)
            {
                if (other.Gamer != null)
                {
                    this.Gamer = other.Gamer;
                }

                if (other.ListTrangBi != null)
                {
                    this.ListTrangBi = other.ListTrangBi;
                }

                if (other.ListArmories != null)
                {
                    this.ListArmories = other.ListArmories;
                }

                if (other.ListMaterials != null)
                {
                    this.ListMaterials = other.ListMaterials;
                }

                if (other.ListChests != null)
                {
                    this.ListChests = other.ListChests;
                }

                if (other.ListHeroes != null)
                {
                    this.ListHeroes = other.ListHeroes;
                }

                if (other.ListChapters != null)
                {
                    this.ListChapters = other.ListChapters;
                }

                if (other.ListOffers != null)
                {
                    this.ListOffers = other.ListOffers;
                }

                if (other.ListBossHunts != null)
                {
                    this.ListBossHunts = other.ListBossHunts;
                }

                if (other.ListMails != null)
                {
                    if (this.ListMails == null)
                    {
                        this.ListMails = new List<MailData>();
                    }

                    this.ListMails = other.ListMails;
                }

                if (other.listCards != null)
                {
                    this.listCards = other.listCards;
                }

                if (other.Rewards.Count > 0)
                {
                    this.Rewards.AddRange(other.Rewards);
                }
                if (other.afkRewardData != null)
                {
                    this.afkRewardData = other.afkRewardData;
                }
                if (other.adsData != null)
                {
                    this.adsData = other.adsData;
                }
                if (other.GhiChuServer != null)
                {
                    this.GhiChuServer = other.GhiChuServer;
                }
                if (other.dailyShopData != null)
                {
                    this.dailyShopData = other.dailyShopData;
                }
                if (other.halloweenServerData != null)
                {
                    this.halloweenServerData = other.halloweenServerData;
                }

                if (other.giangSinhData != null)
                {
                    this.giangSinhData = other.giangSinhData;
                }
                if (other.giangSinhSrvData != null)
                {
                    this.giangSinhSrvData = other.giangSinhSrvData;
                }
                if (other.targetOfferData != null)
                {
                    this.targetOfferData = other.targetOfferData;
                }

                if (other.tetEventGamerData != null)
                {
                    this.tetEventGamerData = other.tetEventGamerData;
                }
                if (other.tetEventServerData != null)
                {
                    this.tetEventServerData = other.tetEventServerData;
                }
            }
        }
    }

}
