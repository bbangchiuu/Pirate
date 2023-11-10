using Hiker.Networks.Data.Shootero;
using System.Collections;
using System.Collections.Generic;
using DateTime = System.DateTime;

public class ThachDauConfig
{
    public int GemThachDau;
    public int SoGioDienRa = 24; // 24h = 1 day
    public string DateTimeStart;
    public int HourStart = 0;
    public int MaxPlayerInGrp = 5;
    public int LockJoinTime = 2; // 2h before expire time
    public int LockPlayTime = 1; // 1h before expire time
    public int[] PlayCostPerDays;
    /// <summary>
    /// chapter number from 1 -> 20 -> xxx
    /// </summary>
    public int[] RankByChapter;

    public int ChapterUnlock = 2;
    public CardReward[] TopRewards1;
    public CardReward[] TopRewards2;
    public CardReward[] TopRewards3;
    public CardReward[] TopRewards4;
    public CardReward[] TopRewards5;

    public CardReward GetTopRewards(int numPlayerInGroup, int posIdx)
    {
        switch (numPlayerInGroup)
        {
            case 1:
                if (posIdx < TopRewards1.Length) return TopRewards1[posIdx];
                else return null;
            case 2:
                if (posIdx < TopRewards2.Length) return TopRewards2[posIdx];
                else return null;
            case 3:
                if (posIdx < TopRewards3.Length) return TopRewards3[posIdx];
                else return null;
            case 4:
                if (posIdx < TopRewards4.Length) return TopRewards4[posIdx];
                else return null;
            case 5:
                if (posIdx < TopRewards5.Length) return TopRewards5[posIdx];
                else return null;
            default:
                break;
        }
        return null;
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

    public int GetCostJoinThachDau()
    {
        return GemThachDau;
    }

    public int GetCostStartBattle(int luotChoi)
    {
        return luotChoi < PlayCostPerDays.Length ? PlayCostPerDays[luotChoi] : PlayCostPerDays[PlayCostPerDays.Length - 1];
    }

    public int GetRankIdxByChapter(int curChapIdx)
    {
        int rankIdx = -1;
        for (int i = 0; i < RankByChapter.Length; ++i)
        {
            if (RankByChapter[i] < (curChapIdx + 1))
            {
                rankIdx = i + 1;
            }
            else
            {
                break;
            }
        }

        if (rankIdx < 0)
        {
            rankIdx = 0;
        }
        return rankIdx;
    }
}

public class ThachDauBattleConfig
{
    public class BlockRandomPool
    {
        public string[] LevelsBoss;
    }

    public long PlayerHPStart;
    public long PlayerDMGStart;

    public int BaseDMG;
    public BlockRandomPool[] Pools;
    public string StartLevel;
    public int LevelExp;

    public int BlockATKBuff;
    public int BlockHPBuff;
    public int NumBlockToBuff;
    public Dictionary<string, int[]> BlockArrayByRank;

    public long GetHPBuff(int missionIdx)
    {
        int blockIdx = GetBlockIdxFromMission(missionIdx);

        long hesoBuff = 100;
        var numBuff = blockIdx / NumBlockToBuff;
        for (int i = 0; i < numBuff; ++i)
        {
            hesoBuff += hesoBuff * BlockHPBuff / 100;
        }

        return BaseDMG * hesoBuff / 100;
    }

    public long GetDMG(int missionIdx)
    {
        int blockIdx = GetBlockIdxFromMission(missionIdx);

        long hesoBuff = 100;
        var numBuff = blockIdx / NumBlockToBuff;
        for (int i = 0; i < numBuff; ++i)
        {
            hesoBuff += hesoBuff * BlockATKBuff / 100;
        }

        return BaseDMG * hesoBuff / 100;
    }

    public int GetBlockIdxFromMission(int missionIdx)
    {
        return missionIdx;
    }

    public long GetTotalExpFromMission(int missionIdx)
    {
        long re = 0;
        for (int i = 0; i < missionIdx; ++i)
        {
            re += LevelExp;
        }
        return re;
    }

    public bool IsBossMission(int index)
    {
        return true;
    }

    public int GetTotalLevelFromMission(int mission)
    {
        return mission;
    }
}