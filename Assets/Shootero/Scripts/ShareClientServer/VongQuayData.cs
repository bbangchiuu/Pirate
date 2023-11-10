public enum EVongQuaySlot
{
    GOLD1,
    GOLD2,
    GOLD3,
    ITEM,
    MATERIAL,
    GEM
}

[System.Serializable]
public class VongQuayConfig
{
    public EVongQuaySlot[] SlotConfig = new EVongQuaySlot[]
    {
        EVongQuaySlot.GOLD1,
        EVongQuaySlot.ITEM,
        EVongQuaySlot.GOLD2,
        EVongQuaySlot.MATERIAL,
        EVongQuaySlot.GOLD3,
        EVongQuaySlot.GEM,
    };

    [System.Serializable]
    public struct VongQuayChapterCfg
    {
        public int[] BaseGold;
        public string[] Items;
        public int[] MaterialCount;
        public int[] Gem;
        public int[] SlotRates;
    }

    public VongQuayChapterCfg[] Chapters;
}