using Hiker.Networks.Data.Shootero;
using System.Collections;
using System.Collections.Generic;

public class ChapterConfig
{
    public Dictionary<string, CardReward> Chests = new Dictionary<string, CardReward>();
    public string[] FarmCfgs = new string[0];
    public long BaseGold;
    public int BaseMaterial;

    public class SecretShopConfig
    {
        public string Res;
        public float Price;
        public int[] Quantity;
        public string[] Items;
        public int[] Discount;
        public int Rate;
    }
    public SecretShopConfig[] SecretShop;

    public class BossHuntConfig
    {
        public string Avatar;
        public CardReward Reward;
    }
    public Dictionary<string, BossHuntConfig> BossHunt = new Dictionary<string, BossHuntConfig>();
}

public class FarmConfig
{
    public class SegmentConfig
    {
        public int HPScale = 100;
        public int DMGScale = 100;
        public string[] Levels;
    }

    public int HPBuff;
    public long DMG = 10;
    public SegmentConfig[] Segments;
    public string StartLevel;
    public int[] Missions;
    public int[] MissionBoss;
    public long TotalGold;

    public int GetHPBuff(int hpBuff, int segmentIndex)
    {
        //float total = HPBuff;
        //for (int i = 0; i <= segmentIndex; ++i)
        //{
        //    totalBuff += Segments[i].HPBuff;
        //}

        return hpBuff * Segments[segmentIndex].HPScale / 100;
    }

    public long GetDMG(long dmg, int segmentIndex)
    {
        //float totalDMG = DMG;
        //float totalBuff = 0;
        //for (int i = 0; i <= segmentIndex; ++i)
        //{
        //    totalBuff += Segments[i].DMGBuff;
        //}

        return dmg * Segments[segmentIndex].DMGScale / 100;
    }

    public bool IsBossMission(int index, out bool isPrepareBoss)
    {
        isPrepareBoss = false;
        for (int i = 0; i < MissionBoss.Length; ++i)
        {
            if (MissionBoss[i] == index)
            {
                return true;
            }
            else if (MissionBoss[i] == index + 1)
            {
                isPrepareBoss = true;
            }
        }

        return false;
    }

    public int GetTotalCombatMissions()
    {
        int result = 0;
        for (int i = 0; i < Missions.Length; ++i)
        {
            var segmentIdx = Missions[i];
            var segment = Segments[segmentIdx];
            if (segment.Levels.Length > 0 && segment.Levels[0].Contains("Angel") == false)
            {
                result++;
            }
        }

        return result;
    }
}