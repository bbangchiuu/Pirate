[System.Serializable]
public struct UpgradeRequirement
{
    public string Res;
    public long Num;
}

[System.Serializable]
public struct UpgradeConfig
{
    ///// <summary>
    ///// Max Level By Rariry
    ///// </summary>
    //public int[] MaxLevel;
    public ItemUpgrade[] LevelReq;
}
[System.Serializable]
public struct ItemUpgrade
{
    public UpgradeRequirement[] Requirements;
}