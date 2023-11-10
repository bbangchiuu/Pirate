using System.Collections.Generic;

namespace Hiker.Networks.Data.Shootero
{
    using LitJson;
    public class ChestConfig
    {
        public int freeHour;
        public Dictionary<string, int> unlockKey;
        public int[] dropRates;
#if SERVER_CODE
        public int GetNextNumber(string chestName = null)
        {
            int nextNumber = 0;
            for(int i = dropRates.Length - 1; i>= 0; i--)
            {
                if(dropRates[i] > 0)
                {
                    int average = (int)(100f / dropRates[i]);
                    nextNumber = RandomUtils.GetRandomNumber(average - 2, average + 3);
                    break;
                }
            }
            return nextNumber;
        }
#endif
    }

    public class CardReward : Dictionary<string, int>
    {
        public const string GEM_CARD = "Gem";
        public const string GOLD_CARD = "Gold";
        public const string CARD_CHEST = "CardChest";

        public bool IsHaveRes()
        {
            if (ContainsKey(GEM_CARD) && this[GEM_CARD] > 0)
            {
                return true;
            }

            if (ContainsKey(GOLD_CARD) && this[GOLD_CARD] > 0)
            {
                return true;
            }

            return false;
        }

        public bool IsHaveMaterial()
        {
            foreach (var s in Keys)
            {
                if (s.StartsWith("M_"))
                    return true;
            }

            return false;
        }

        public bool IsHaveUnit()
        {
            foreach (var s in Keys)
            {
                if (s.StartsWith("H_"))
                {
                    return true;
                }
            }

            return false;
        }

        
        public static bool IsResourceOrMaterial(string rewardKey)
        {
            if (ConfigManager.Materials.ContainsKey(rewardKey))
            {
                return true;
            }

            if (rewardKey == CardReward.GEM_CARD ||
                rewardKey == CardReward.GOLD_CARD)
            {
                return true;
            }

            return false;
        }

        public static bool IsGetBonusFromPremium(string rewardKey)
        {
            if(rewardKey == "Gold" || rewardKey == "M_VuKhi" || rewardKey == "M_Mu" || rewardKey == "M_Giap" || rewardKey == "M_OffHand")
            {
                return true;
            }
            return false;
        }

        public static bool IsRandomEquipment(string name)
        {
            if (name.StartsWith("RE_"))
            {
                return true;
            }
            return false;
        }

        //public static UnitRarity GetRarityRandom(string name)
        //{
        //    var rarityName = GetUnitNameFromRandomName(name);
        //    var result = UnitRarity.Common;
        //    var names = System.Enum.GetNames(typeof(UnitRarity));

        //    bool canParse = false;
        //    foreach (var n in names)
        //    {
        //        if (rarityName == n)
        //        {
        //            result = (UnitRarity)System.Enum.Parse(typeof(UnitRarity), rarityName);
        //            canParse = true;
        //            break;
        //        }
        //    }

        //    if (canParse == false)
        //    {
        //        return UnitUtils.GetUnitRarity(rarityName);
        //    }
        //    else
        //    {
        //        return result;
        //    }
        //}
#if SERVER_CODE
        public static string GetEquipmentNameFromRandomName(string rdName) // "RE_VK_2"
        {
            string[] strs = rdName.Split('_');
            string slotPrefix = strs[1];
            ERarity rarity = (ERarity)int.Parse(strs[2]);
            if (strs[1] == "XX")
            {
                int rd = RandomUtils.GetRandomNumber(100);
                if (rd < 25)
                    slotPrefix = "VK";
                else if (rd < 50)
                    slotPrefix = "MU";
                else if(rd < 75)
                    slotPrefix = "AG";
                else
                    slotPrefix = "OH";
            }
            return RandomItemByRarity(rarity, slotPrefix);
        }

        public static string RandomItemByRarity(ERarity rarity, string slotPrefix = "XX", List<string> ignoreList  = null)
        {
            List<string> listItems = new List<string>();
            List<float> listChances = new List<float>();
            foreach (var s in ConfigManager.ItemStats)
            {
                var itemName = s.Key;
                var itemRarity = ConfigManager.GetItemDefaultRarity(itemName);
                if (itemRarity == rarity
                    && (slotPrefix == "XX" || itemName.Contains(slotPrefix + "_"))
                    && (ignoreList == null || !ignoreList.Contains(itemName)))
                {
                    listItems.Add(itemName);
                    listChances.Add(s.Value.RandomRate);
                }
            }

            var rIndex = RandomUtils.GetRandomIndexInList(listChances);
            return listItems[rIndex];
        }
#endif

#if SERVER_CODE
        public static CardReward GetRandomChestRewardFromName(ChestData chestData, bool isNoVKEpic = false)
        {
            CardReward rewards = new CardReward();
            ChestConfig chestCfg = ConfigManager.ChestCfg[chestData.Name];
            if(chestData.nextNumber <= 0)
            {
                chestData.nextNumber = chestCfg.GetNextNumber();
            }
            int[] dropRates = chestCfg.dropRates;

            int rd = RandomUtils.GetRandomNumber(100);

            int c = 0;
            int usePseudoRandom = 1;
            for(int i = dropRates.Length - 1; i >= 0; i--)
            {
                if (dropRates[i] == 0)
                    continue;

                bool isUsePR = false;
                int addRate = dropRates[i];
                int diffRate = 0;
                if (usePseudoRandom > 0)
                {
                    int realRate = (int)(100 * RandomUtils.GetPRChanceByTime(addRate, chestData.PseudoRandomCount));

                    //tich luy cho chest Legend
                    if (chestData.Name == "Chest_Legend" && chestData.nextNumber > 0)
                    {
                        if (chestData.PseudoRandomCount >= chestData.nextNumber)
                        {
                            realRate = 100;
                        }
                        else
                        {
                            realRate = 0;
                        }
                    }

                    diffRate = addRate - realRate;
                    addRate = realRate;

                    usePseudoRandom--;
                    isUsePR = true;
                    
                }
                c += addRate;
                if (rd < c)
                {
                    if (isUsePR)
                    {
                        //reset counter
                        chestData.PseudoRandomCount = 0;
                        chestData.nextNumber = chestCfg.GetNextNumber();
                    }
                    string name = "RE_XX_" + i;
                    //do epic dau tien la epic
                    if (i == (int)ERarity.Epic && isNoVKEpic)
                    {
                        name = "RE_VK_" + i;
                    }

                    rewards.AddReward(name, 1);
                    break;
                }

                if (diffRate != 0) c += diffRate;
            }
            return rewards;
        }

        public static CardReward GetRandomCardFromChest(UserInfo uInfo, ChestData chestData, List<string> fromList = null)
        {
            ERarity cardRarity = ERarity.Common;
            ChestConfig chestCfg = ConfigManager.ChestCfg[chestData.Name];
            if (chestData.nextNumber <= 0)
            {
                chestData.nextNumber = chestCfg.GetNextNumber();
            }
            int[] dropRates = chestCfg.dropRates;

            //loc nhung the da dat gioi han
            float[] availChances = new float[dropRates.Length];
            List<string> availFromList = new List<string>();

            if (fromList == null) fromList = new List<string>(ConfigManager.CardStats.Keys);

            for (int i = 0; i < fromList.Count; i++)
            {
                string cardName = fromList[i];
                int haveNum = 0;
                List<CardData> allOfThisCard = uInfo.listCards.FindAll(e => e.Name == cardName);
                if (allOfThisCard != null) haveNum = allOfThisCard.Count;
                int maxDrop = ConfigManager.CardStats[cardName].MaxDrop;
                if(maxDrop == 0 || haveNum < maxDrop)
                {
                    availFromList.Add(cardName);
                    int idx = (int)ConfigManager.CardStats[cardName].Rarity;
                    availChances[idx] = dropRates[idx];
                }
            }

            List<float> listChances = new List<float>();
            if (availFromList.Count > 0)
            {
                listChances.AddRange(availChances);
            }
            else
            {
                listChances.AddRange(System.Array.ConvertAll(dropRates, x => (float)x));
            }

            int lastIdx = listChances.Count - 1;
            //tich luy cho chest Legend
            if (listChances[lastIdx] > 0)
            {
                if (chestData.nextNumber > 0)
                {
                    if (chestData.PseudoRandomCount >= chestData.nextNumber)
                    {
                        cardRarity = ERarity.Legend;
                        //reset counter sau khi drop legend
                        chestData.PseudoRandomCount -= chestData.nextNumber;
                        chestData.nextNumber = chestCfg.GetNextNumber();
                    }
                    else
                    {
                        listChances[lastIdx] = 0;
                    }
                }
            }

            //khong phai legend
            if(cardRarity == ERarity.Common)
            {
                cardRarity = (ERarity)RandomUtils.GetRandomIndexInList(listChances);
            }

            CardReward reward = new CardReward();
            List<string> listCards = ConfigManager.GetListUnitCardByRarity(cardRarity, availFromList);
            if (listCards.Count > 0)
            {
                string rdCard = listCards[RandomUtils.GetRandomNumber(0, listCards.Count)];
                reward.AddReward(rdCard, 1);
            }
            //else
            //{
            //    GameServer.Database.LogUI.LogToMongo(uInfo.GID, "RandomCard", "Card", "listCards = 0, listChance = " + string.Join(",", listChances), 0, (int)cardRarity);
            //    GameServer.Database.LogUI.LogToMongo(uInfo.GID, "RandomCard", "Card", "avaiList = " + string.Join(",", availFromList) + " avaiChance = " + string.Join(",", availChances), 0, (int)cardRarity);
            //    if (fromList != null)
            //    GameServer.Database.LogUI.LogToMongo(uInfo.GID, "RandomCard", "Card", "fromList = " + string.Join(",", fromList), 0, (int)cardRarity);
            //}

            return reward;
        }
#endif

        public int GetGem() { return ContainsKey(GEM_CARD) ? this[GEM_CARD] : 0; }

        public int GetGold() { return ContainsKey(GOLD_CARD) ? this[GOLD_CARD] : 0; }

        //public int GetCargo() { return ContainsKey(CARGO_CARD) ? this[CARGO_CARD] : 0; }

        public void AddGem(int gem)
        {
            AddReward(GEM_CARD, gem);
        }

        public void AddGold(int gold)
        {
            AddReward(GOLD_CARD, gold);
        }

        //public void AddCargo(int cargo)
        //{
        //    AddReward(CARGO_CARD, cargo);
        //}

        //public void AddCargoBase(int cargoBase)
        //{
        //    AddReward(BASE_CARGO_CARD, cargoBase);
        //}

        //public void AddGoldBase(int goldbase)
        //{
        //    AddReward(BASE_GOLD_CARD, goldbase);
        //}

        //public void AddCargoBase(float baseCargoRate)
        //{
        //    AddReward(BASE_CARGO_CARD, (int)(baseCargoRate * 100));
        //}

        //public void AddGoldBase(float baseCargoRate)
        //{
        //    AddReward(BASE_GOLD_CARD, (int)(baseCargoRate * 100));
        //}

        //public int GetCargoFromBase(int baseCargo)
        //{
        //    if (ContainsKey(BASE_CARGO_CARD))
        //    {
        //        return (int)(baseCargo / 100f * this[BASE_CARGO_CARD]);
        //    }

        //    return 0;
        //}

        //public int GetGoldFromBase(int baseCargo)
        //{
        //    if (ContainsKey(BASE_GOLD_CARD))
        //    {
        //        return (int)(baseCargo / 100f * this[BASE_GOLD_CARD]);
        //    }

        //    return 0;
        //}

        public void AddReward(string key, int quantity)
        {
            if (ContainsKey(key))
            {
                this[key] += quantity;
            }
            else
            {
                this[key] = quantity;
            }
        }

        public virtual void ToJsonData(JsonData data)
        {
            foreach (var k in Keys)
            {
                //if (this[k] > 0)
                {
                    data[k] = this[k];
                }
            }
        }
        public JsonData ToJsonData()
        {
            JsonData result = new JsonData();
            ToJsonData(result);

            return result;
        }

        public void MergeReward(CardReward reward)
        {
            foreach (var c in reward.Keys)
            {
                if (ContainsKey(c))
                {
                    this[c] += reward[c];
                }
                else
                {
                    this.Add(c, reward[c]);
                }
            }
        }

        public CardReward(CardReward reward)
        {
            this.MergeReward(reward);
        }

        public CardReward(JsonData json)
        {
            foreach (string s in json.GetKeys())
            {
                if (json[s].IsInt)
                {
                    this.Add(s, (int)json[s]);
                }
            }
        }
        public CardReward() { }

        //public static CardReward FromOldRewardData(RewardData reward)
        //{
        //    CardReward result = new CardReward();
        //    if (reward.GetGold() > 0)
        //    {
        //        result.AddGold(reward.GetGold());
        //    }

        //    if (reward.GetGem() > 0)
        //    {
        //        result.AddGem(reward.GetGem());
        //    }

        //    if (reward.GetCargo() > 0)
        //    {
        //        result.AddCargo(reward.GetCargo());
        //    }

        //    if (reward.materials != null)
        //    {
        //        foreach (var s in reward.materials)
        //        {
        //            result.Add(s.Key, s.Value);
        //        }
        //    }

        //    return result;
        //}

        //public static implicit operator CardReward(RewardData data)
        //{
        //    return CardReward.FromOldRewardData(data);
        //}
    }
    public class ChestReward : CardReward
    {
        public Dictionary<string, string> Titles;
        public string Name;

        public override void ToJsonData(JsonData result)
        {
            result["Name"] = Name;

            base.ToJsonData(result);

            if (Titles != null && Titles.Count > 0)
            {
                var jsonTitles = new JsonData();
                foreach (var s in Titles.Keys)
                {
                    jsonTitles[s] = Titles[s];
                }
                result["Titles"] = jsonTitles;
            }
        }

        public ChestReward(JsonData json) : base(json)
        {
            this.Name = json["Name"].ToString();
            if (json.Contains("Titles"))
            {
                var jsonTitles = json["Titles"];

                if (jsonTitles != null && jsonTitles.Count > 0)
                {
                    this.Titles = new Dictionary<string, string>();
                    foreach (string s in jsonTitles.GetKeys())
                    {
                        this.Titles.Add(s, jsonTitles[s].ToString());
                    }
                }
            }
        }
        public ChestReward() { }

        public ChestReward(ChestReward reward) : base(reward)
        {
            if (reward.Titles != null)
            {
                this.Titles = new Dictionary<string, string>(reward.Titles);
            }
            this.Name = reward.Name;
        }
    }

    public class GeneralReward : CardReward
    {
        //public const string SOURCE_LEAGUE_UP = "LeagueUp";
        //public const string SOURCE_BATTLE_PASS = "BattlePass";
        //public const string SOURCE_CHEST_AMBUSH = "ChestAmbush";
        //public const string SOURCE_BATTLE = "Battle";
        //public const string SOURCE_BATTLE_ADS = "Battle_Ads";
        //public const string SOURCE_CAMPAIGN_ADS = "CAMPAIGN_ADS";
        //public const string SOURCE_IN_TOWN = "IN_TOWN";

        public ChestReward[] Chests;
        public Dictionary<string, string> Titles; // Custom Titles from Server, support multi-languages
        public string Name; // client se hien thi theo Localization.Get("Name")
        public string Source;

        public int GetTotalGem()
        {
            int result = GetGem();

            if (Chests != null)
            {
                foreach (var s in Chests)
                {
                    result += s.GetGem();
                }
            }

            return result;
        }

        //public int GetTotalCargo()
        //{
        //    int result = GetCargo();

        //    if (Chests != null)
        //    {
        //        foreach (var s in Chests)
        //        {
        //            result += s.GetCargo();
        //        }
        //    }

        //    return result;
        //}

        public int GetTotalGold()
        {
            int result = GetGold();

            if (Chests != null)
            {
                foreach (var s in Chests)
                {
                    result += s.GetGold();
                }
            }

            return result;
        }
        public CardReward ShortenToCardReward()
        {
            CardReward newReward = new CardReward();
            if (Count > 0)
            {
                newReward.MergeReward(this);
            }

            if (Chests != null)
            {
                foreach (var chest in Chests)
                {
                    if (chest != null && chest.Count > 0)
                    {
                        newReward.MergeReward(chest);
                    }
                }
            }

            return newReward;
        }

        public override void ToJsonData(JsonData json)
        {
            base.ToJsonData(json);

            if (Chests != null && Chests.Length > 0)
            {
                var s = new JsonData();
                foreach (var c in Chests)
                {
                    s.Add(c.ToJsonData());
                }

                json["Chests"] = s;
            }

            if (string.IsNullOrEmpty(Source) == false)
            {
                json["Source"] = Source;
            }

            if (string.IsNullOrEmpty(Name) == false)
            {
                json["Name"] = Name;
            }

            if (Titles != null && Titles.Count > 0)
            {
                var jsonTitles = new JsonData();
                foreach (var s in Titles.Keys)
                {
                    jsonTitles[s] = Titles[s];
                }
                json["Titles"] = jsonTitles;
            }
        }

        public GeneralReward(JsonData json) : base(json)
        {
            if (json.Contains("Chests"))
            {
                var chestsJson = json["Chests"];
                if (chestsJson != null && chestsJson.Count > 0)
                {
                    this.Chests = new ChestReward[chestsJson.Count];

                    for (int i = 0; i < chestsJson.Count; ++i)
                    {
                        this.Chests[i] = new ChestReward(chestsJson[i]);
                    }
                }
            }

            if (json.Contains("Source"))
            {
                Source = json["Source"].ToString();
            }

            if (json.Contains("Name"))
            {
                Name = json["Name"].ToString();
            }

            if (json.Contains("Titles"))
            {
                var jsonTitles = json["Titles"];
                foreach (string t in jsonTitles.GetKeys())
                {
                    Titles.Add(t, jsonTitles[t].ToString());
                }
            }
        }

        public GeneralReward(ChestReward chest)
        {
            Chests = new ChestReward[] { chest };
        }

        public GeneralReward() { }

        public GeneralReward(CardReward reward) : base(reward)
        {
        }

        public GeneralReward(GeneralReward reward) : base(reward)
        {
            this.Name = reward.Name;
            this.Source = reward.Source;
            if (reward.Titles != null)
                this.Titles = new Dictionary<string, string>(reward.Titles);

            if (reward.Chests != null)
            {
                this.Chests = new ChestReward[reward.Chests.Length];

                for (int i = 0; i < reward.Chests.Length; ++i)
                {
                    this.Chests[i] = new ChestReward(reward.Chests[i]);
                }
            }
        }
    }
}
