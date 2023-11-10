using Hiker.Networks.Data.Shootero;
using System.Collections;
using System.Collections.Generic;
using DateTime = System.DateTime;

public class SanTheConfig
{
    public int ChapterUnlock;
    public int TheLucReq;
    public int LuotRequireAds;
    public int LuotHoiSinh;
    public int[] CostResetLuotChoi = new int[] { 100 };
    public int[] PlayCostPerDays = new int[] { 0, 200, 250, 300, 400 };
    public int[] LevelAtRound = new int[] { 3, 2, 2, 2 };
    public int HeSoHPQuai;
    public int HeSoHPBoss;
    public int[] HeSoFinalHP;
    public int[] HeSoFinalATK;
    public float DelaySpawnFirstRound;
    public int HeSoRewardGold;
    public string StartLevel;
    public string MainLevel;
    public string[] ListBoss;

    public int GetCostStartBattle(int luotChoi)
    {
        return luotChoi < PlayCostPerDays.Length ? PlayCostPerDays[luotChoi] : PlayCostPerDays[PlayCostPerDays.Length - 1];
    }

    public int GetCostReset(int luotReset)
    {
        return luotReset < CostResetLuotChoi.Length ? CostResetLuotChoi[luotReset] : CostResetLuotChoi[CostResetLuotChoi.Length - 1];
    }

    public int GetHeSoFinalATK(int missionID)
    {
        var heso = 100;
        if (missionID < 1)
        {
            heso = ConfigManager.SanTheCfg.HeSoFinalATK[0];
        }
        else if (missionID > ConfigManager.SanTheCfg.HeSoFinalATK.Length)
        {
            heso = ConfigManager.SanTheCfg.HeSoFinalATK[ConfigManager.SanTheCfg.HeSoFinalATK.Length - 1];
        }
        else
        {
            heso = ConfigManager.SanTheCfg.HeSoFinalATK[missionID - 1];
        }
        return heso;
    }

    public int GetHeSoFinalHP(int missionID)
    {
        var heso = 100;
        if (missionID < 1)
        {
            heso = ConfigManager.SanTheCfg.HeSoFinalHP[0];
        }
        else if (missionID > ConfigManager.SanTheCfg.HeSoFinalHP.Length)
        {
            heso = ConfigManager.SanTheCfg.HeSoFinalHP[ConfigManager.SanTheCfg.HeSoFinalHP.Length - 1];
        }
        else
        {
            heso = ConfigManager.SanTheCfg.HeSoFinalHP[missionID - 1];
        }
        return heso;
    }
}


public class SanTheSpawnInfo
{
    public string[] Wave1;
    public string[] Wave2;
    public string[] Wave3;
    public string[] Wave4;
}

public class SanTheServerData
{
    public SanTheSpawnInfo SpawnInfo;
    public DateTime SanTheResetTime;
}

public class SanTheBattleData
{
    public SanTheSpawnInfo SpawnInfo;
    public long HeSoHP;
    public long HeSoATK;
}