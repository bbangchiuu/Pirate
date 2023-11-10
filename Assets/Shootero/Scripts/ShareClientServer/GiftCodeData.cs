using DateTime = System.DateTime;

namespace Hiker.Networks.Data.Shootero
{
    public class GiftCodeData
    {
        public string ID;
        public int CodeIndex;
        public GeneralReward Rewards;
        public DateTime StartDate;
        public DateTime EndDate;
        public int Received;
    }
}