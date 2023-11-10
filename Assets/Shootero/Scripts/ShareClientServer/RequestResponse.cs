using System.Collections.Generic;
using DateTime = System.DateTime;

namespace Hiker.Networks.Data.Shootero
{
    public class GetUserInfoRequest : GIDRequest
    {
        public string[] Props;
    }

    public class DoiTenRequest : GIDRequest
    {
        public string newName;
    }

    public class LoginRequest : GIDRequest
    {
        public string DeviceID;
        public string AppleID;
        public string GoogleID;
        public string DeviceModel;
        public string OperatingSystem;
        public string[] Datas;
    }

    public class UserInfoResponse : ExDataBase
    {
        public UserInfo UInfo;
    }

    public class LoginResponse : UserInfoResponse
    {
        public string Token;
        public string BattleID;
    }

    public class StartBattleRequest : GIDRequest
    {
        public string BattleID;
        public int ChapIdx;
        /// <summary>
        /// FarmMode = 1 : Single Rate FarmMode
        /// FarmMode = 2 : Double Rate FarmMode
        /// </summary>
        public int FarmMode;
        public UpdateBattleRequest UpdateReq;
    }

    public class StartBattleResponse : UserInfoResponse
    {
        public BattleData battleData;
    }

    public class StartThachDauBattleResponse : UserInfoResponse
    {
        public BattleData battleData;
        public string[] Levels;
    }

    public class StartNienThuBattleResponse : UserInfoResponse
    {
        public BattleData battleData;
        public string Level;
    }


    public class UpdateBattleRequest : GIDRequest
    {
        public string BattleID;

        /// <summary>
        /// HackCode
        /// 0b0001 cheat mem
        /// 0b0010 cheat hackspeed
        /// 0b0100 cheat time
        /// 0b1000 cheat wallhack
        /// </summary>
        public int CCode;
        /// <summary>
        /// 0 normal, 1 raid mode, 2 leothap mode
        /// </summary>
        public int BattleMode;
        /// <summary>
        /// TimeBattleInSeconds
        /// </summary>
        public long BattleTime;

        public string Envi;

        public int Mat;
        public long Gold;
        public long Exp;
        public int Life;
        public bool IsGreedy;
        //public int PassedMission;
        public UnitStatWrapper PlayerStat;
        public long MaxHP;
        public BuffType[] listBuffs;
        public bool IsEnd;

        public int[] Path;
        public int[] NodePath;
        public int HPCfg;
        public long DMGCfg;

        public int CurDungeonID;
        public int CurMissionID;
        public string[] playedScene;
        public Dictionary<string, int> Loot;
        public int EventIdx;
        public List<string> RuongDaMo;
        public bool IsCompleteMap;
        public List<string> BossHunted;
        public List<ThongKeSTNguoiChoi> TKTranDanh;
        public List<ThongKeST> TKSatThuong;
        public Dictionary<string, int> DeTu;
#if SERVER_1_3
        public ThongKeSung TKSung;
#endif
        public bool CurLvlClear;
        public int HeroSK;
        public int LuotRR; // luot reroll
        public int LuotHS; // luot hoi sinh
        public string[] KilledUnit;
        public string BuffRandomState;
        //public int BuffRandomSeed;
        public string SecKey;
    }

    public class UpdateBattleResponse : UserInfoResponse
    {
        public int TKSTNumber;
    }

    public class AssetBundleConfigResponse : ExDataBase
    {
        public string data;
        public int version;
    }
    //public class ReviveInBattleRequest : GIDRequest
    //{
    //    public string BattleID;
    //}

    public class GetRuongBattleRequest : GIDRequest
    {
        public string BattleID;
        public int ChapIdx;
        public string Ruong;
        public string VerifyKey;
    }

    public class GetRuongBattlResponse : UserInfoResponse
    {
        public GeneralReward reward;
    }

    public class SetHeroEquipmentRequest : GIDRequest
    {
        public string HeroID;
        public EquipmentSlot[] slots;
    }

    public class SetHeroEquipmentResponse : UserInfoResponse
    {

    }

    public class HeroUpgradeRequest : GIDRequest
    {
        public string HeroID;
    }

    public class HeroUpgradeResponse : UserInfoResponse
    {
        public string HeroID;
    }

    public class UpgradeItemRequest : GIDRequest
    {
        public string ItemID;
    }

    public class RecycleItemRequest : GIDRequest
    {
        public string[] ItemIDs;
    }

    public class FuseItemRequest : GIDRequest
    {
        public string ItemID;
        public string[] MaterialIDs;
    }

    public class UpgradeArmoryRequest : GIDRequest
    {
        public string ArmoryName;
    }

    public class OpenChestRequest : GIDRequest
    {
        public string ChestName;
    }

    public class VongQuayResponse : UserInfoResponse
    {
        public int Slot;
        public GeneralReward reward;
    }

    public class StartBattleEventResponse : ExDataBase
    {
        public EBattleEventRoom room;
        public List<KeyValueReward> Rewards;
        public List<SecretShopItemData> listItems;
    }

    public class KeyValueReward
    {
        public string k;
        public int v;
        public KeyValueReward() { }
        public KeyValueReward(string key, int value) { k = key; v = value; }
    }

    public class BuySecretShopRequest : GIDRequest
    {
        public string ItemID;
    }

    public class GetSecretShopResponse : ExDataBase
    {
        public List<SecretShopItemData> listItems;
    }

    public class PurchaseRequestBase : GIDRequest
    {
        public DateTime puchaseTime;
        public string receipt;
        public string transID;
        public string password;//password for editor fix purchase bug
        public string missingId;
        public string orderId;
        public string packageName;
    }

    public class PurchaseResponse : UserInfoResponse
    {
        public string transID { get; set; }
        public bool checkMail; // check mail to receive full reward from purchase
        public bool isRestorePurchase = false;
        //public GetShopInfoResponse shopInfo;
    }

    public class ClaimBossHuntRequest : GIDRequest
    {
        public string Boss;
    }
    public class GetAFKRewardResponse : UserInfoResponse
    {
        public AFKRewardData afkRewardData;
    }

    public class ClaimAFKRewardResquest : GIDRequest
    {
        public AFKRewardData afkRewardData;
    }
    public class CloseMailBoxRequest : GIDRequest
    {
        public List<MailData> listMails;
    }
    public class ClaimMailRewardRequest : GIDRequest
    {
        public MailData claimMailData;
    }
    public class WatchAdsRequest : GIDRequest
    {
        public string adsId;
    }
    public class ForgeMysthicItemRequest : GIDRequest
    {
        public string selectedSlot;
        public string[] ItemIDs;
    }
    public class ReforgeMysthicItemRequest : GIDRequest
    {
        public string itemID;
        public string matItemID;
    }
    public class ReforgeMysthicItemResponse : UserInfoResponse
    {
        public string itemID;
    }
    public class ChangeStyleMysthicItemRequest : GIDRequest
    {
        public string itemID;
        public string newStyleName;
    }
    public class ChangeStyleMysthicItemResponse : UserInfoResponse
    {
        public string itemID;
    }
    public class UpStarMysthicItemRequest : GIDRequest
    {
        public string itemID;
        public string matItemID;
    }
    public class UpStarMysthicItemResponse : UserInfoResponse
    {
        public string itemID;
    }

    public class WatchAdsThemVangRequest : GIDRequest
    {
        public string adsId;
        public int chapIdx;
    }
    public class WatchAdsThemVangResponse : UserInfoResponse
    {
        public int soLuongVangThem;
    }

    public class LeoThapDataResponse : UserInfoResponse
    {
        public LeoThapGroup curGrp;
        public LeoThapGroup lastGrp;
        public List<GamerLeoThapData> curTop;
        public List<GamerLeoThapData> lastTop;
        public string BattleID;
    }

    public class ThachDauDataResponse : UserInfoResponse
    {
        public ThachDauGroup curGrp;
        public ThachDauGroup lastGrp;
        public List<GamerThachDauData> curTop;
        public List<GamerThachDauData> lastTop;
        public string BattleID;
    }

    public class SanTheDataResponse : UserInfoResponse
    {
        public string BossName;
        public int LuotSanThe;
        public int LuotReset;
        public bool IsWin;
        public DateTime ResetTime;
        public string BattleID;
        public string[] listCards;
    }

    public class AddCardSlotRequest : GIDRequest
    {
        public string itemID;
        public string matItemID;
    }
    public class UpdateCardSlotRequest : GIDRequest
    {
        public string itemID;
        public List<string> newCardSlots;
    }
    public class RecycleCardRequest : GIDRequest
    {
        public string[] cardIDs;
    }
    public class GetLastBattleLeoThapRewardResponse : UserInfoResponse
    {
        public CardReward lastBattleRewards;
        public int lastBattleFloor;
    }
    public class UseGiftCodeRequest : GIDRequest
    {
        public string giftcode;
    }
    public class BuyDailyShopItemRequest : GIDRequest
    {
        public int itemIdx;
        public DateTime crResetTime;
    }

    public class GiangSinhRequest : GIDRequest
    {
        public int Act;
        public string State;
        public string[] Table;
    }

    public class GiangSinhDrawRequest : GiangSinhRequest
    {
        public int Slot;
        public string Item;
        public string Key;
    }

    public class GiangSinhMergeRequest : GiangSinhRequest
    {
        public int Slot1;
        public int Slot2;
        public string Item;
        public string Key;
    }

    public class GiangSinhActiveItemRequest : GiangSinhRequest
    {
        public int Slot;
        public string Key;
    }

    public class GiangSinhOpenItemRequest : GIDRequest
    {
        public int Gift;
    }

    public class ClaimTruongThanhChapterRequest : GIDRequest
    {
        public int chapIdx;
    }
}

