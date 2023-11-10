using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDungeonConfig : MonoBehaviour
{
    public int HPBuff;
    public long DMG = 10;

    //public string StartLevel;
    public long TotalGold;
    public int MaxPathLength = 4;

    public int GetHPBuff(List<ENodeType> nodePaths)
    {
        //int total = HPBuff;
        //int totalScale = 0;
        //for (int i = 0; i < nodePaths.Count - 1; ++i)
        //{
        //    int nodeScale = 0;
        //    var nodeType = nodePaths[i];
        //    if (nodeType == ENodeType.Angel)
        //    {
        //        nodeScale = ConfigManager.GetAngelNodeHPBuff();
        //    }
        //    else if (nodeType == ENodeType.Boss)
        //    {
        //        nodeScale = ConfigManager.GetBossNodeHPBuff();
        //    }
        //    else if (nodeType == ENodeType.Event)
        //    {
        //        nodeScale = ConfigManager.GetEventNodeHPBuff();
        //    }
        //    else if (nodeType == ENodeType.Quai1 || 
        //        nodeType == ENodeType.Quai2) // node quai
        //    {
        //        nodeScale = ConfigManager.GetCreepNodeHPBuff();
        //    }

        //    totalScale += nodeScale;
        //}

        //return total + total * totalScale / 100;

        return ConfigManager.GetHPBuffByMapConfig(nodePaths, HPBuff);
    }

    public long GetDMG(List<ENodeType> nodePaths)
    {
        //int totalScale = 0;
        //for (int i = 0; i < nodePaths.Count - 1; ++i)
        //{
        //    int nodeScale = 0;
        //    var nodeType = nodePaths[i];
        //    if (nodeType == ENodeType.Angel)
        //    {
        //        nodeScale = ConfigManager.GetAngelNodeDMGBuff();
        //    }
        //    else if (nodeType == ENodeType.Boss)
        //    {
        //        nodeScale = ConfigManager.GetBossNodeDMGBuff();
        //    }
        //    else if (nodeType == ENodeType.Event)
        //    {
        //        nodeScale = ConfigManager.GetEventNodeDMGBuff();
        //    }
        //    else if (nodeType == ENodeType.Quai1 || 
        //        nodeType == ENodeType.Quai2) // node quai
        //    {
        //        nodeScale = ConfigManager.GetCreepNodeDMGBuff();
        //    }

        //    totalScale += nodeScale;
        //}
        //return DMG + DMG * totalScale / 100;
        return ConfigManager.GetDMGByMapConfig(nodePaths, DMG);
    }
}
