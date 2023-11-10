using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitDrop
{
    public UnitDropType DropType;
    public GameObject[] Visuals;
}

[System.Serializable]
public class UnitExpDrop : UnitDrop
{
    public long EXP = 0;
}

[System.Serializable]
public class UnitResDrop : UnitDrop
{
    public long Count = 0;
}

[System.Serializable]
public class UnitMaterialDrop : UnitDrop
{
    public string Name;
    public long Count = 0;
}

//[System.Serializable]
//public class UnitBuffDrop : UnitDrop
//{
//    public BuffStat Buff;
//}

[System.Serializable]
public class HealOrb : UnitDrop
{
    public int HPRegen = 0;
}

public enum UnitDropType
{
    Exp,
    HealOrb,
    Gold,
    Gem,
    Material,
}