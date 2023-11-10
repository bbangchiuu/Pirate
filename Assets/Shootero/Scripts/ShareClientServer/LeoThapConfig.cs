using Hiker.Networks.Data.Shootero;
using System.Collections;
using System.Collections.Generic;
using DateTime = System.DateTime;

public class LeoThapConfig
{
    public int GemLeoThap;
    public int SoGioDienRa = 72; // 72h = 3 days
    public string DateTimeStart;
    public int HourStart = 0;
    public int MaxPlayerInGrp = 10;
    public int LockJoinTime = 3; // 3h before expire time
    public int LockPlayTime = 1; // 1h before expire time
    public int[] ModifierPattern = new int[] { 0, 1, 1 };
    public int[] ModifierHeSoRandom = new int[] { 0, 1 };
    public int RankEnableMod = 4;
    public int TileKhongLo = 150;
    public int HeSoMauDracula = 100;
    public int HeSoFlash = 30;

    public int[] BaseStat;
    public int[] RewardRate;
    public long[] Gold;
    public int[] HeroStone;

    public int TheLucReq = 10; // the luc cho moi luot choi free (cost = 0)
    public int[] PlayCostPerDays = new int[] { 0, 50, 100, 150, 200 };
    public int ChapterUnlock = 2;
    public CardReward[] TopRewards;

    public CardReward GetTopRewards(int posIdx, int rankIdx)
    {
        var reward = new CardReward(TopRewards[posIdx]);
        var enumerator = TopRewards[posIdx].GetEnumerator();
        while (enumerator.MoveNext())
        {
            var s = enumerator.Current;
            if (s.Key == "M_HeroStone")
            {
                reward[s.Key] = s.Value * RewardRate[rankIdx] / 100;
            }
        }
        return reward;
    }

    public LeoThapRankCfg GetLeoThapRankCfg(int idx)
    {
        return new LeoThapRankCfg
        {
            BaseStat = BaseStat[idx],
            RewardRate = RewardRate[idx],
            Gold = Gold[idx],
            HeroStone = HeroStone[idx],
        };
    }

    public DateTime GetTimeStartGroup(DateTime now)
    {
        var originTime = TimeUtils.ConvertUtcToServerTimeZone(
            DateTime.Parse(DateTimeStart, 
                           System.Globalization.CultureInfo.InvariantCulture,
                           System.Globalization.DateTimeStyles.AssumeUniversal)
            );
        var ts = now - originTime;
        var cycle = System.Convert.ToInt64(System.Math.Floor(ts.TotalHours / SoGioDienRa));
        var sTime = originTime.AddHours(cycle * SoGioDienRa);
        return sTime;
    }

    public int GetCostStartBattle(int luotChoi)
    {
        return luotChoi < PlayCostPerDays.Length ? PlayCostPerDays[luotChoi] : PlayCostPerDays[PlayCostPerDays.Length - 1];
    }
}

public struct LeoThapRankCfg
{
    public int BaseStat; // BaseStat requirement
    public int RewardRate; // 100% percent
    public long Gold;
    public int HeroStone;
}

public class LeoThapBattleConfig
{
    public class BlockRandomPool
    {
        public string[] LevelsQuai;
        public string[] LevelsBoss;
    }

    public int ATKRate;
    public BlockRandomPool[] Pools;
    public string StartLevel;
    public string EventLevel;
    public string[] Block;
    public int[] BlockLevel;
    public int[] BlockLevelExp;
    public int LevelNhanFullPT;

    public int BlockATKBuff;
    public int BlockHPBuff;
    public int NumBlockToBuff;
    public int[] BlockArray;

    public long GetHPBuff(long baseStat, int missionIdx)
    {
        var baseDmg = GetBaseDMG(baseStat, missionIdx);
        int blockIdx = GetBlockIdxFromMission(missionIdx);

        long hesoBuff = 100;
        var numBuff = blockIdx / NumBlockToBuff;
        for (int i = 0; i < numBuff; ++i)
        {
            hesoBuff += hesoBuff * BlockHPBuff / 100;
        }

        return baseDmg * hesoBuff / 100;
    }

    public long GetBaseDMG(long baseStat, int missionIdx)
    {
        if (baseStat < 300)
        {
            baseStat = 300;
        }

        var baseDmg = baseStat * ATKRate / 100;
        return baseDmg;
    }

    public long GetDMG(long baseStat, int missionIdx)
    {
        var baseDmg = GetBaseDMG(baseStat, missionIdx);
        int blockIdx = GetBlockIdxFromMission(missionIdx);

        long hesoBuff = 100;
        var numBuff = blockIdx / NumBlockToBuff;
        for (int i = 0; i < numBuff; ++i)
        {
            hesoBuff += hesoBuff * BlockATKBuff / 100;
        }

        return baseDmg * hesoBuff / 100;
    }

    public int GetBlockIdxFromMission(int missionIdx)
    {
        return missionIdx / Block.Length;
    }

    public int GetMissionIdxInBlock(int missionIdx)
    {
        return missionIdx % Block.Length;
    }

    public long GetTotalExpFromMission(int missionIdx)
    {
        long re = 0;
        for (int i = 0; i < missionIdx; ++i)
        {
            var idx = i % BlockLevelExp.Length;
            re += BlockLevelExp[idx];
        }
        return re;
    }

    public bool IsBossMission(int index, out bool isPrepareBoss)
    {
        isPrepareBoss = false;

        var idxInBlock = GetMissionIdxInBlock(index);
        if (Block[idxInBlock] == "Boss")
        {
            return true;
        }
        else if (Block[GetMissionIdxInBlock(index + 1)] == "Boss")
        {
            isPrepareBoss = true;
        }

        return false;
    }

    public int GetTotalLevelFromMission(int mission)
    {
        var numBlock = (mission - 1) / Block.Length;
        var idxInBlock = (mission - 1) % Block.Length;
        int level = 0;
        int numLevelPerBlock = 0;
        for (int i = 0; i < Block.Length; ++i)
        {
            if (i <= idxInBlock)
            {
                level += BlockLevel[i];
            }

            numLevelPerBlock += BlockLevel[i];
        }

        level += numBlock * numLevelPerBlock;

        return level;
    }
}