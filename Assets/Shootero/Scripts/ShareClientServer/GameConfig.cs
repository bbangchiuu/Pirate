using System.Collections;
using System.Collections.Generic;
//using UnityEngine;

public class GameConfig
{
    public BuffStat[] Buffs;
    public long[] LevelExp;

    public long GetTotalExpAtLevel(int level)
    {
        long totalExp = 0;
        for (int i = 1; i <= level; ++i)
        {
            totalExp += LevelExp[i];
        }

        return totalExp;
    }

    public int GetLevelByExp(long exp)
    {
        int playerLvl = 1;
        var lastLevelExp = GetTotalExpAtLevel(playerLvl - 1);
        var nextLevelExp = GetTotalExpAtLevel(playerLvl);
        while (exp >= nextLevelExp && playerLvl < LevelExp.Length)
        {
            playerLvl++;
            lastLevelExp = GetTotalExpAtLevel(playerLvl - 1);
            nextLevelExp = GetTotalExpAtLevel(playerLvl);
        }
        return playerLvl;
    }
}
