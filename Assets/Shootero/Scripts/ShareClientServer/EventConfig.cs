using Hiker.Networks.Data.Shootero;
using System.Collections;
using System.Collections.Generic;
using DateTime = System.DateTime;

public class EventConfig
{

}

public class HalloweenServerData
{
    public DateTime StartTime;
    public DateTime StopDropTime;
    public DateTime EndTime;
}
public class HalloweenEventConfig
{
    public int BaseCandy;
    public int BaseGoldToCandy;
    public int GemToCandy;
}

public partial class GiangSinhGamerData
{
    public long GID;
    public string[] Table;
    public int Act;
    public string State;
    public int Seed;

    public List<int> GiftBox1;
    public List<int> GiftBox2;
    public List<int> GiftBox3;
    public int MaxGift1;
    public int MaxGift2;
    public int MaxGift3;

    public DateTime EndTime;

    public bool IsFull()
    {
        foreach (string cell in Table)
        {
            if (string.IsNullOrEmpty(cell))
            {
                return false;
            }
        }
        return true;
    }
}

public class GiangSinhServerData
{
    public DateTime StartTime;
    public DateTime EndTime;
}

public partial class GiangSinhConfig
{
    public int CostDraw;
    public int MaxLevel;
    public int TableSize;
    public string Material;
    public int MaterialBase;
    public string[] Items;

    public CardReward[] ListGift1;
    public CardReward[] ListGift1Random;
    public CardReward[] ListGift2;
    public CardReward[] ListGift2Random;
    public CardReward[] ListGift3;
    public CardReward[] ListGift3Random;

    public static int GetLevelFromName(string itemName)
    {
        int lvl = int.Parse(itemName.Substring(itemName.Length - 1));
        return lvl;
    }

    public bool ValidateItem(string item)
    {
        bool validName = false;
        foreach (var t in Items)
        {
            if (item.StartsWith(t))
            {
                validName = true;
                break;
            }
        }

        if (validName == false) return false;
        int lvl = GetLevelFromName(item);

        return (lvl > 0 && lvl <= MaxLevel);
    }
}

public class TetEventServerData
{
    public DateTime StartTime;
    public DateTime StopDropTime;
    public DateTime EndTime;
}
public partial class TetEventGamerData
{
    public long GID;

    public int LuotChoi;

    public bool purchased;
    public long currentExp;
    public List<int> receivedRewards;
    public List<int> receivedPremiumRewards;

    public int GetCurrentLevel()
    {
        return (int)System.Math.Floor((double)(1f * currentExp / ConfigManager.TetEventCfg.ExpPerLevel));
    }
}
public class TetEventConfig
{
    public int LuotDanhMotNgay;
    /// <summary>
    /// Thoi gian danh boss theo giay (seconds)
    /// </summary>
    public int ThoiGianDanhBoss;
    public int HeSoDiemTheoSatThuong;
    public long HeSoDiemGietBoss;
    public long EnemyBaseDMG;
    public int LevelStart;
    public long PlayerHPStart;
    public long PlayerDMGStart;
    public string[] LevelByBosses;
    public string StartLevel;
    public int BuyLevelGemCost;
    public int ExpPerLevel;
    public List<CardReward> Rewards;
    public List<CardReward> PremiumRewards;
}