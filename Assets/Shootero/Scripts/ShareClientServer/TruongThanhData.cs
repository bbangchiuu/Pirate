namespace Hiker.Networks.Data.Shootero
{
    using System;
    using System.Collections.Generic;

    public class TruongThanhGamerData
    {
        public DateTime startDate;
        public bool purchased;
        public List<int> receivedChapters;
    }

    public class TruongThanhConfig
    {
        public int ChapterUnlock;
        public int ExpiredHours;
        public int[] GemRewards;
    }
}
