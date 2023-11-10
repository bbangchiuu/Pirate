using Hiker.GUI.Shootero;
using Hiker.Networks.Data.Shootero;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hiker.GUI;
using UnityEngine.AI;
#if ANTICHEAT
using Int64 = CodeStage.AntiCheat.ObscuredTypes.ObscuredLong;
using Int32 = CodeStage.AntiCheat.ObscuredTypes.ObscuredInt;
using Float = CodeStage.AntiCheat.ObscuredTypes.ObscuredFloat;
using Bool = CodeStage.AntiCheat.ObscuredTypes.ObscuredBool;
using ObString = CodeStage.AntiCheat.ObscuredTypes.ObscuredString;
#else
using Int64 = System.Int64;
using Int32 = System.Int32;
using Float = System.Single;
using Bool = System.Boolean;
using ObString = System.String;
#endif

public class QuanlyNguoichoi : MonoBehaviour
{
    public static QuanlyNguoichoi Instance {
        get {
            if (instance == null)
                instance = FindObjectOfType<QuanlyNguoichoi>();
            return instance;
        }
    }
    static QuanlyNguoichoi instance = null;

    public TargetFollower CamFollower;
    public Transform CamOriginPos;
    public VariableJoystick joystick;
    public NutBamKyNang nutBam;
    public DonViChienDau PlayerUnit { get; set; }
    // POWERUP - VISUAL
    [HideInInspector]
    public TwinShield PlayerShield { get; set; }
    [HideInInspector]
    public GameObject EffectFly { get; set; }
    [HideInInspector]
    public GameObject EffectLinken { get; set; }
    [HideInInspector]
    public GameObject EffectElectricField { get; set; }
    [HideInInspector]
    public GameObject EffectFlameField { get; set; }
    [HideInInspector]
    public GameObject EffectFrozenField { get; set; }

    public bool IsLevelClear { get; private set; }
    public Int32 SoLuotDungKyNang { get; private set; }
    public Int32 LuotReroll { get; private set; }

    // END POWERUP - VISUAL

    #region PlayerInfo
    Int32 playerMat = 0;
    Int64 playerGold = 0;
    Int64 playerExp = 0;
    Int32 playerExpBuff = 0;
    Int32 playerLvl = 1;

    public int PlayerMaterial { get { return playerMat; } set { playerMat = value; } }
    public long PlayerGold { get { return playerGold; } set { playerGold = value; } }
    public long PlayerExp { get { return playerExp; } set { playerExp = value; } }
    public int PlayerLvl { get { return playerLvl; } private set { playerLvl = value; } }
    public int PlayerExpBuff { get { return playerExpBuff; } }
    #endregion

    public int GetTotalExpBuff()
    {
        int totalBuff = QuanlyNguoichoi.Instance.PlayerExpBuff;
        if (PlayerUnit && PlayerUnit.GetBuffCount(BuffType.SMART) > 0)
        {
            var smartcfg = QuanlyNguoichoi.Instance.GetBuffStatByType(BuffType.SMART);
            totalBuff += (int)smartcfg.Params[0];
        }
        return totalBuff;
    }

    float mBattleTime;
    public float BattleTime { get { return mBattleTime; } }
    public float LastLvlBattleTime { get; private set; }

    #region MissionInfo
    Int32 missionID = 0;
    public int MissionID { get { return missionID; } private set { missionID = value; } }
    Int32 segmentIndex = 0;
    public int SegmentIndex { get { return segmentIndex; } private set { segmentIndex = value; } }
    int passedMission = 0;

    Int32 chapterIndex = 0;
    public int ChapterIndex { get { return chapterIndex; } set { chapterIndex = value; } }
    Int32 curDungeonId = 0;
    public int CurDungeonId { get { return curDungeonId; } private set { curDungeonId = value; } }
    List<int> CurrentPath = new List<int>();
    public int GetCurPathLength() { return CurrentPath.Count; }
    List<int> ListNodePaths = new List<int>();

    public Int32 BattleMode { get; set; }

    Int32 mFarmMode = 0;
    public int FarmMode { get { return mFarmMode; } }
    public bool IsFarmMode { get { return mFarmMode > 0 && BattleMode == 1; } }
    public bool IsNormalMode { get { return mFarmMode == 0 && BattleMode == 0; } }
    public bool IsLeoThapMode { get { return LeoThapRankIdx >= 0 && BattleMode == 2; } }
    public Int32 LeoThapRankIdx { get; set; }
    public Int32 LeoThapMod { get; set; }
    public bool IsSanTheMode { get { return BattleMode == 3; } }
    public bool IsThachDauMode { get { return BattleMode == 4; } }

    public Int32 ThachDauRankIdx { get; set; }
    List<string> thachDauScenes = new List<string>();
    public List<string> ThachDauScenes { get { return thachDauScenes;  } }

    public bool IsNienThuMode { get { return BattleMode == 5; } }
    public string NienThuScene { get; set; }
    float mThoiGianNienThu = 0;
    public float ThoiGianNienThu { get { return mThoiGianNienThu; } }
    Int64 mSatThuongNienThu;
    public long NienThuBattlePointSumarize {
        get
        {
            var p = mSatThuongNienThu * ConfigManager.TetEventCfg.HeSoDiemTheoSatThuong / 100;
            if (IsEndGame && ThoiGianNienThu < ConfigManager.TetEventCfg.ThoiGianDanhBoss && PlayerUnit.IsAlive())
            {
                // win
                p += ConfigManager.TetEventCfg.HeSoDiemGietBoss;
            }
            return p;
        }
    }

    string mEnviName;

    public string FarmName { get; set; }

    public bool IsGreedy { get; set; }
    public bool IsEventMap { get; set; }
    public bool IsLoadingMission { get; private set; }

    public ChapterConfig currentChap { get; private set; }
    public FarmConfig farmConfig { get; private set; }
    public NodeMap nodeMapConfig { get; private set; }
    public int BlockIdx { get; private set; }

    List<string> playedScenes = new List<string>();
    public List<string> PlayedScenes { get { return playedScenes; } }
    #endregion

    HashSet<string> killedEnemy = new HashSet<string>();

    public GameObject HealOrbVisual;
    public GameObject ExpDropVisual;
    public GameObject DropGemVisual;
    public GameObject DropGoldVisual;
    public GameObject DropMaterialVisual;

    List<UnitDrop> listDrops = new List<UnitDrop>();

#if DEBUG 
    #region Test

    public bool TestMission = false;
    public bool OverrideStat = false;
    public long OverrideHP = 0;
    public long OverrideDMG = 0;
    [Tooltip("Zora|BeastMaster|Paladin|Graves")]
    public string OverrideHeroName = "Zora";

    #endregion
#endif

    #region PlayerData
    List<BuffStat> listBuffs = new List<BuffStat>();
    public List<BuffStat> ListBuffs { get { return listBuffs; } }
    public GameConfig gameConfig { get; private set; }
    public BattleData battleData { get; set; }
    public bool IsInitedMission { get; private set; }
    public bool CanRevive { get; set; }
    public Int32 LuotHS { get; private set; }

    Dictionary<string, int> mDeTu = new Dictionary<string, int>();

    List<DonViChienDau> listDeTu = new List<DonViChienDau>();

    List<ThongKeST> mTKSatThuong = new List<ThongKeST>();
    List<ThongKeSTNguoiChoi> mListTKTranDanh = new List<ThongKeSTNguoiChoi>();
    public ThongKeSTNguoiChoi TKTranDanh { get; private set; }
#if SERVER_1_3
    public ThongKeSung TKSung { get; private set; }
#endif
    int mThongKeIDNumber = 0;

    const int NumHistoryPos = 10;
    const float IntervalHistory = 0.1f;
    Vector3[] HistoryPos = new Vector3[NumHistoryPos];
    
    public void AddThongKeST(string src, long st, long preMau, long mau, long maxMau)
    {
        ThongKeST tk = new ThongKeST()
        {
            ID = mThongKeIDNumber++,
            Nguon = src,
            ST = st,
            BT = (long)Mathf.Round(BattleTime),
            Mau = mau,
            MaxMau = maxMau,
            PreMau = preMau,

        };
        mTKSatThuong.Add(tk);
    }

    public void ClearOldTK(int number)
    {
        mTKSatThuong.RemoveAll(e => e.ID <= number);
    }

    public void AddTKSTUnitStat(string unitName, UnitStat unitStat, bool isBoss, bool isAir, long takenDmg)
    {
        if (TKTranDanh == null)
        {
            return;
        }

        if (TKTranDanh.UnitStat == null)
        {
            TKTranDanh.UnitStat = new Dictionary<string, UnitStatWrapper>();
            TKTranDanh.Mission = missionID;
            if (IsNormalMode)
            {
                var path = GetPathNodeType();
                TKTranDanh.Path = new List<int>();
                foreach (var t in path)
                {
                    TKTranDanh.Path.Add((int)t);
                }
            }
        }

        if (TKTranDanh.UnitStat.ContainsKey(unitName) == false)
        {
            var stat = unitStat.ToUnitStatWrapper();
            stat.IsBoss = isBoss ? 1 : -1;
            stat.IsAir = isAir ? 1 : -1;
            TKTranDanh.UnitStat.Add(unitName, stat);
        }
        {
            var stat = TKTranDanh.UnitStat[unitName];

            if (stat.STReceived == 0 ||
                stat.STReceived > takenDmg)
            {
                stat.STReceived = takenDmg;
            }
        }
        
    }

    public void AddTKSTNguoiChoi(long dmg)
    {
        if (TKTranDanh == null)
        {
            return;
        }
        TKTranDanh.LuotST++;
        TKTranDanh.STTong += dmg;

        if (TKTranDanh.STLon < dmg)
        {
            TKTranDanh.STLon = dmg;
        }

        if (TKTranDanh.STNho > dmg)
        {
            TKTranDanh.STNho = dmg;
        }
    }
    public void AddTKSTNguoiChoiSCount()
    {
        if (TKTranDanh == null)
        {
            return;
        }
        TKTranDanh.SCount++;
    }
    public void AddTKSTNguoiChoiSBan()
    {
        if (TKTranDanh == null)
        {
            return;
        }
        TKTranDanh.SBan++;
    }

    public void TKSungCount()
    {
#if SERVER_1_3
        if (TKSung != null)
            TKSung.SCount++;
#endif
    }
    public void TKSungDaBan()
    {
#if SERVER_1_3
        if (TKSung != null)
            TKSung.SDaBan++;
#endif
    }

    string vuKhiName = string.Empty;
    public string VKName { get { return vuKhiName; } }

    Dictionary<ObString, Int32> playerLoot = new Dictionary<ObString, Int32>();
    List<ObString> listBossHunted = new List<ObString>();
    List<ObString> listRuongDaMo = new List<ObString>();

    public void ClearGold()
    {
        PlayerGold = 0;
        playerLoot.Remove(CardReward.GOLD_CARD);
    }

    public CardReward GetPlayerLoot()
    {
        CardReward reward = new CardReward();
        foreach (var l in playerLoot)
        {
            reward.Add(l.Key, l.Value);
        }
        return reward;
    }
    #endregion

    public long GetDMGEnemy()
    {
        if (IsFarmMode)
        {
            var segmentIdx = SegmentIndex;

            //int hpBuff = farmConfig.HPBuff;
            long dmg = farmConfig.DMG;

            if (battleData != null)
            {
                var playerOriginStat = UnitStat.FromUnitStat(battleData.OriginStat);
                //hpBuff = (int)Mathf.Round((playerOriginStat.DMG / 100f - 1f) * 100);
                dmg = (long)Mathf.Round((playerOriginStat.HP / 500f) * farmConfig.DMG);
            }
            return farmConfig.GetDMG(dmg, segmentIdx);
        }
        else
        if (IsLeoThapMode)
        {
            var configRank = ConfigManager.LeoThapCfg.GetLeoThapRankCfg(LeoThapRankIdx);

            var dmg = ConfigManager.LeoThapBattleCfg.GetDMG(configRank.BaseStat, MissionID - 1);
            return dmg;
        }
        else
        if (IsSanTheMode)
        {
            var hesoAtk = ConfigManager.SanTheCfg.GetHeSoFinalATK(missionID);

            if (battleData != null && battleData.SanThe != null)
            {
                var sanTheBattleData = battleData.SanThe;

                var dmg = sanTheBattleData.HeSoATK * hesoAtk / 100;
                return dmg;
            }
            else if (PlayerUnit)
            {
                return PlayerUnit.OriginStat.HP / 5 * hesoAtk / 100;
            }
            return 100;
        }
        else
        if (IsThachDauMode)
        {
            var dmg = ConfigManager.ThachDauBattleCfg.GetDMG(MissionID - 1);
            return dmg;
        }
        else
        if (IsNienThuMode)
        {
            var dmg = ConfigManager.TetEventCfg.EnemyBaseDMG;
            return dmg;
        }
        else
        //if (IsNormalMode)
        {
            var dungeonCfg = GetMapDungeonConfig();
            var pathCfg = GetPathNodeType();
            return dungeonCfg.GetDMG(pathCfg);
        }
    }

    TuyetKyNhanVat mTuyetKyMan;
    public void ActivateSkill()
    {
        if (PlayerUnit)
        {
            if (mTuyetKyMan == null || mTuyetKyMan.gameObject != PlayerUnit.gameObject)
                mTuyetKyMan = PlayerUnit.GetKyNang();

            if (mTuyetKyMan && mTuyetKyMan.HaveTuyetKyChuDong())
            {
                var coolDown = mTuyetKyMan.GetTuyetKyCoolDown(0);
                var durability = mTuyetKyMan.GetTuyetKyDurability(0);
                if (coolDown <= 0 && durability > 0)
                {
                    SoLuotDungKyNang++;
                    mTuyetKyMan.Activate(0);

                    UpdatePlayerStat(false);
                }
            }
        }
    }

    public void CountSkillPassive()
    {
        if (PlayerUnit)
        {
            if (mTuyetKyMan == null || mTuyetKyMan.gameObject != PlayerUnit.gameObject)
                mTuyetKyMan = PlayerUnit.GetComponent<TuyetKyNhanVat>();

            if (mTuyetKyMan && mTuyetKyMan.HaveTuyetKyChuDong() == false)
            {
                SoLuotDungKyNang++;

                UpdatePlayerStat(false);
            }
        }
    }

    public void ChonDeTuIR(string deTu, int detuID)
    {
        if (PlayerUnit)
        {
            if (mTuyetKyMan == null || mTuyetKyMan.gameObject != PlayerUnit.gameObject)
                mTuyetKyMan = PlayerUnit.GetComponent<TuyetKyNhanVat>();

            if (mTuyetKyMan && mTuyetKyMan.HaveTuyetKyChuDong() == false)
            {
                var sk = mTuyetKyMan.GetTuyetKy(0) as IronManSkill;
                if (sk != null)
                {
                    SoLuotDungKyNang = detuID;
                    var dt = ThuPhucDeTu(deTu);

                    UpdatePlayerStat(false);
                }
            }
        }
        
    }

    public void RerollSkill()
    {
        QuanlyNguoichoi.Instance.LuotReroll++;
    }

    void LoadMapConfig()
    {
        gameConfig = ConfigManager.gameConfig;
        currentChap = ConfigManager.chapterConfigs[ChapterIndex];
        farmConfig = null;
        if (IsFarmMode)
        {
            if (string.IsNullOrEmpty(FarmName) == false)
            {
                if (ConfigManager.FarmConfigs.ContainsKey(FarmName))
                {
                    farmConfig = ConfigManager.FarmConfigs[FarmName];
                }
            }

            passedMission = 100;
        }
        else if (IsNormalMode)
        {
            nodeMapConfig = Resources.Load<NodeMap>("MapDungeon/Chapter" + (ChapterIndex + 1));
            nodeMapConfig.ClearNodes();
        }
    }

    public MapDungeonConfig GetMapDungeonConfig()
    {
        if (nodeMapConfig == null) LoadMapConfig();
        return nodeMapConfig.GetComponent<MapDungeonConfig>();
    }

    public NodeLevelConfig GetCurrentNodeLevelConfig()
    {
        return nodeMapConfig.points[CurDungeonId].GetComponent<NodeLevelConfig>();
    }

    public List<ENodeType> GetPathNodeType()
    {
        List<ENodeType> result = new List<ENodeType>();
        for (int i = 0; i < CurrentPath.Count; ++i)
        {
            var nodeidx = CurrentPath[i];
            var curNode = nodeMapConfig.points[nodeidx];
            result.Add(curNode.GetComponent<NodeLevelConfig>().LevelType);
        }
        return result;
    }

    private void Awake()
    {
        instance = this;

        gameConfig = ConfigManager.gameConfig;
        currentChap = ConfigManager.chapterConfigs[ChapterIndex];
    }

    GameObject mWeapoonObj;
    void LoadPlayerUnit()
    {
        string heroName = ConfigManager.DefaultHeroCodeName;
        //string vuKhiName = string.Empty;
        if (battleData != null)
        {
            if (string.IsNullOrEmpty(battleData.HeroName) == false)
            {
                heroName = battleData.HeroName;
            }

            vuKhiName = battleData.VuKhiName;
        }
        //else
        {
#if DEBUG
            // change hero Name;
            //heroName = "BeastMaster";
            //heroName = "Paladin";
            //heroName = "Graves";
            if (string.IsNullOrEmpty(OverrideHeroName) == false)
            {
                heroName = OverrideHeroName;
            }
#endif
        }

        if (vuKhiName=="")
            vuKhiName = "VK_Sword";

        //bad sua o day
#if DEBUG
        //vuKhiName = "VK_Cannon";
#endif

        string vuKhiType = "Melee";
        if (string.IsNullOrEmpty(vuKhiName) == false)
        {
            var cfg = ConfigManager.GetItemConfig(vuKhiName);
            if (cfg != null)
            {
                if (cfg.VuKhiType == "Gun")
                {
                    vuKhiType = cfg.VuKhiType;
                }
            }
        }
        string heroPrefabName = heroName + vuKhiType;

        if (PlayerUnit != null)
        {
            PlayerUnit.gameObject.SetActive(false);
            if (ScreenBattle.instance) ScreenBattle.instance.OnUnitDied(PlayerUnit);
            Destroy(PlayerUnit.gameObject);
        }

        PlayerUnit = Instantiate(Resources.Load<DonViChienDau>("AIPrefabs/" + heroPrefabName));

        var playerVisual = Instantiate(Resources.Load<PlayerVisual>("Heroes/" + heroPrefabName), PlayerUnit.visualGO.transform);
        
        GameObject weapon_obj = playerVisual.LoadWeapon(vuKhiName);
        mWeapoonObj = weapon_obj;

        if( weapon_obj.transform.childCount > 0)
        {
            GameObject flash_eff = weapon_obj.transform.GetChild(0).gameObject;
            PlayerUnit.shooter.InitForMainPlayer(flash_eff);
        }

        /*
        PlayerUnit.transform.position = Vector3.zero;
        */
        var ship = GameObject.FindGameObjectWithTag("Ship");
        PlayerUnit.transform.position = ship.transform.position + new Vector3(0, 0.5f, 0);
        CamFollower.transform.position = CamOriginPos.position;
        CamFollower.target = PlayerUnit.transform;
        CamFollower.InitPos();

        PlayerUnit.gameObject.layer = LayersMan.GetDefaultHeroLayer(heroName);
    }

    void DisablePlayerUnit()
    {
        PlayerUnit.GetComponent<PlayerMovement>().enabled = false;
        PlayerUnit.enabled = false;
        var behavior = PlayerUnit.GetComponent<BehaviorDesigner.Runtime.BehaviorTree>();
        behavior.DisableBehavior();
        if (mWeapoonObj)
        {
            mWeapoonObj.gameObject.SetActive(false);
        }
    }

    void EnablePlayerUnit()
    {
        PlayerMovement playerMovement = PlayerUnit.GetComponent<PlayerMovement>();
        playerMovement.enabled = true;
        PlayerUnit.enabled = true;
        if (!playerMovement.characterController.isOnNavMesh)
        {
            playerMovement.characterController.Warp(PlayerUnit.transform.position);
        }
        var behavior = PlayerUnit.GetComponent<BehaviorDesigner.Runtime.BehaviorTree>();
        behavior.EnableBehavior();

        if (mWeapoonObj)
        {
            mWeapoonObj.gameObject.SetActive(true);
        }

        // force bat tu trong 0.5 giay
        if (PlayerUnit) PlayerUnit.SetInvulnerableTime(0.5f, false);
    }

    public int BuffRandomSeed { get; private set; }
    public Random.State BuffRandomStated { get; private set; }

    public static string GetStringObsPref(string key, string defaultVal)
    {
        try
        {
            return CodeStage.AntiCheat.Storage.ObscuredPrefs.GetString(key, defaultVal);
        }
        catch (System.Exception e)
        {
            return defaultVal;
        }
    }
    public static int GetIntObsPref(string key, int defaultVal)
    {
        try
        {
            return CodeStage.AntiCheat.Storage.ObscuredPrefs.GetInt(key, defaultVal);
        }
        catch (System.Exception e)
        {
            return defaultVal;
        }
    }

    //void SaveBuffRandomState(string battleID, int playMode)
    //{
    //    //if (string.IsNullOrEmpty(battleID) == false)
    //    //{
    //    //    CodeStage.AntiCheat.Storage.ObscuredPrefs.SetString("LastBattleIDBuff", battleID);
    //    //    CodeStage.AntiCheat.Storage.ObscuredPrefs.SetInt("BuffRandomSeed", BuffRandomSeed);
    //    //    CodeStage.AntiCheat.Storage.ObscuredPrefs.SetString("BuffRandomStated", JsonUtility.ToJson(BuffRandomStated));
    //    //    CodeStage.AntiCheat.Storage.ObscuredPrefs.Save();
    //    //}
    //}

    void LoadBuffRandomState(int seed, string state)
    {
        BuffRandomSeed = seed;
        if (string.IsNullOrEmpty(state))
        {
            var curState = Random.state;
            Random.InitState(seed);
            BuffRandomStated = Random.state;
            Random.state = curState;
        }
        else
        {
            BuffRandomStated = JsonUtility.FromJson<Random.State>(state);
        }
    }

    public int GetRandomRollingBuff(int count)
    {
        var curState = Random.state;
        Random.InitState(BuffRandomSeed);
        Random.state = BuffRandomStated;
        int result = Random.Range(0, count);
        BuffRandomStated = Random.state;
        Random.state = curState;
        return result;
    }

    private void OnEnable()
    {
        StopAllCoroutines();

        ResetCheckHack();

        ResetHistory();
        listDrops.Clear();
        mEnviName = string.Empty;
        CanRevive = true;
        LuotHS = 0;
        PlayerLvl = 1;
        PlayerExp = 0;
        PlayerGold = 0;
        PlayerMaterial = 0;
        IsEndGame = false;
        IsInitedMission = false;
        SoLuotDungKyNang = 0;
        LuotReroll = 0;

        lastTimeUpdatePlayerStat = -10;
        mBattleTime = 0;
        LastLvlBattleTime = mBattleTime;

        foreach (var s in listDeTu)
        {
            if (s != null)
            {
                s.gameObject.SetActive(false);
                Destroy(s.gameObject);
            }
        }
        listDeTu.Clear();

        ObjectPoolManager.Init();

        ObjectPoolManager.instance.PreCachePool(HealOrbVisual, 1);

        ObjectPoolManager.instance.PreCachePool(ExpDropVisual, 20);
        ObjectPoolManager.instance.PreCachePool(DropGemVisual, 10);
        ObjectPoolManager.instance.PreCachePool(DropGoldVisual, 20);
        ObjectPoolManager.instance.PreCachePool(DropMaterialVisual, 10);
        StartCoroutine(ObjectPoolManager.instance.PreCachePool("Particle/PowerUp/RUNNING_CHARGE_Proj", 3));
        StartCoroutine(ObjectPoolManager.instance.PreCachePool("Particle/PowerUp/RUNNING_CHARGE_ProjMax", 3));
        StartCoroutine(ObjectPoolManager.instance.PreCachePool("Particle/Other/MonsterDie", 3));

        ListBuffs.Clear();
        CurrentPath.Clear();
        PlayedScenes.Clear();
        killedEnemy.Clear();

        LoadPlayerUnit();

        DisablePlayerUnit();

        curDungeonId = 0;
        missionID = 0;
        playerLoot.Clear();
        listRuongDaMo.Clear();
        listBossHunted.Clear();

        playerExpBuff = 0;

        mThongKeIDNumber = 0;
        mListTKTranDanh.Clear();
        mTKSatThuong.Clear();
        TKTranDanh = null;
#if SERVER_1_3
        TKSung = null;
#endif
        mDeTu.Clear();

        if (QuanlyNguoichoi.Instance.EffectLinken)
            QuanlyNguoichoi.Instance.EffectLinken.SetActive(false);

        if (string.IsNullOrEmpty(FarmName))
        {
            mFarmMode = 0;
        }
        else
        {
            mFarmMode = 1;
        }

        if (battleData != null)
        {
            mEnviName = battleData.Envi;
            if (battleData.Path != null)
                CurrentPath.AddRange(battleData.Path);

            if (battleData.NodePath != null)
            {
                ListNodePaths.AddRange(battleData.NodePath);
            }

            curDungeonId = battleData.CurDungeonID;
            missionID = battleData.CurMissionID;
            passedMission = battleData.PassedMission;

            mFarmMode = battleData.FarmRate;

            if (battleData.PlayedScene != null)
                PlayedScenes.AddRange(battleData.PlayedScene);

            IsGreedy = battleData.IsGreedy;
            PlayerGold = battleData.Gold;
            PlayerMaterial = battleData.Mat;
            if (battleData.Loot != null)
            {
                foreach (var loot in battleData.Loot)
                {
                    playerLoot.Add(loot.Key, loot.Value);
                }
            }

            if (battleData.RuongDaMo != null)
            {
                foreach (var r in battleData.RuongDaMo)
                {
                    listRuongDaMo.Add(r);
                }
            }

            if (battleData.BossHunted != null)
            {
                foreach (var r in battleData.BossHunted)
                {
                    listBossHunted.Add(r);
                }
            }

            CanRevive = battleData.CanRevive(battleData.LuotHS);
            LuotHS = battleData.LuotHS;

            foreach (var s in battleData.ListMods)
            {
                if (s.Stat == EStatType.EXP)
                {
                    playerExpBuff += (int)s.Val;
                }
            }

            if (battleData.TKSatThuong != null)
                mTKSatThuong = battleData.TKSatThuong;
            if (mTKSatThuong != null && mTKSatThuong.Count > 0)
            {
                foreach (var t in mTKSatThuong)
                {
                    if (mThongKeIDNumber <= t.ID)
                    {
                        mThongKeIDNumber = t.ID + 1;
                    }
                }
                //mThongKeIDNumber = mTKSatThuong[mTKSatThuong.Count - 1].ID+1;
            }
            if (battleData.TKTranDanh != null)
                mListTKTranDanh = battleData.TKTranDanh;
            if (mListTKTranDanh != null && mListTKTranDanh.Count > 0)
            {
                TKTranDanh = mListTKTranDanh[mListTKTranDanh.Count - 1];
                if (battleData.CurLvlClear == false && PlayedScenes.Count > 0)
                {
                    string preSceneName = PlayedScenes[PlayedScenes.Count - 1];
                    /// duongrs
                    /// reset dem sung ban neu continue battle -> chong viec choi cung luc ban hack
                    /// tren thiet bi thu 2
                    if (string.IsNullOrEmpty(preSceneName) == false &&
                        TKTranDanh.TenMap.StartsWith(preSceneName))
                    {
                        TKTranDanh.SCount = 0;
                        TKTranDanh.SBan = 0;
                    }
                }
            }
#if SERVER_1_3
            TKSung = battleData.TKSung;
#endif
            LoadBuffRandomState(battleData.BuffRandomSeed, battleData.BuffRandomState);
            SoLuotDungKyNang = battleData.HeroSK;
            LuotReroll = battleData.LuotRR;
            mBattleTime = battleData.BattleTime;
            LastLvlBattleTime = mBattleTime;

            if (battleData.KilledUnit != null)
            {
                killedEnemy.UnionWith(battleData.KilledUnit);
            }
        }
        else
        {
            LoadBuffRandomState(Random.Range(0, int.MaxValue), string.Empty);
        }
#if SERVER_1_3
        if (TKSung == null)
        {
            TKSung = new ThongKeSung();
        }
#endif
        mThoiGianNienThu = 0;
        mSatThuongNienThu = 0;
        Hiker.GUI.GUIManager.Instance.SetScreen("Battle");

        LoadMapConfig();
#if UNITY_EDITOR
        if (TestMission)
        {
            StartTestMission();
        }
        else
#endif
        {
            if (IsFarmMode)
            {
                string preSceneName = null;
                if (PlayedScenes.Count > 0)
                {
                    preSceneName = PlayedScenes[PlayedScenes.Count - 1];
                }
                LoadMissionFarmMode(MissionID, preSceneName, false);
                if (battleData != null)
                {
                    IsLevelClear = battleData.CurLvlClear;
                }

                if (MissionID == 0 && IsLevelClear == false)
                {
                    AnalyticsManager.LogEvent("START_FARM_MODE");
                }
                
            }
            else if (IsLeoThapMode)
            {
                string preSceneName = null;
                if (PlayedScenes.Count > 0)
                {
                    preSceneName = PlayedScenes[PlayedScenes.Count - 1];
                }
                LoadMissionLeoThap(MissionID, preSceneName, false);
                if (battleData != null)
                {
                    IsLevelClear = battleData.CurLvlClear;
                }
                if (MissionID == 0 && IsLevelClear == false)
                {
                    AnalyticsManager.LogEvent("START_LEOTHAP_MODE");
                }
            }
            else if (IsSanTheMode)
            {
                string preSceneName = null;
                if (PlayedScenes.Count > 0)
                {
                    preSceneName = PlayedScenes[PlayedScenes.Count - 1];
                }
                LoadMissionSanThe(MissionID, preSceneName, false);
                if (battleData != null)
                {
                    IsLevelClear = battleData.CurLvlClear;
                }
                if (MissionID == 0 && IsLevelClear == false)
                {
                    AnalyticsManager.LogEvent("START_SANTHE_MODE");
                }
            }
            else if (IsThachDauMode)
            {
                string preSceneName = null;
                if (PlayedScenes.Count > 0)
                {
                    preSceneName = PlayedScenes[PlayedScenes.Count - 1];
                }
                LoadMissionThachDau(MissionID, preSceneName, false);
                if (battleData != null)
                {
                    IsLevelClear = battleData.CurLvlClear;
                }
                if (MissionID == 0 && IsLevelClear == false)
                {
                    AnalyticsManager.LogEvent("START_THACHDAU_MODE");
                }
            }
            else if (IsNienThuMode)
            {
                LoadMissionNienThu(MissionID, false);
                if (battleData != null)
                {
                    IsLevelClear = battleData.CurLvlClear;
                }
                if (MissionID == 0 && IsLevelClear == false)
                {
                    AnalyticsManager.LogEvent("START_NIENTHU_MODE");
                }
            }
            else
            {
                GoToDungeon(curDungeonId, missionID, false);
                if (battleData != null)
                {
                    IsLevelClear = battleData.CurLvlClear;
                }

                if (curDungeonId == 0 && missionID == 0 &&
                    IsLevelClear == false)
                {
                    AnalyticsManager.LogEvent("START_CHAPTER_" + chapterIndex,
                        new AnalyticsParameter("LEVEL", CurDungeonId));

                    AnalyticsManager.LogEvent(string.Format("GO_CHAP{0}_D{1}", QuanlyNguoichoi.Instance.ChapterIndex, QuanlyNguoichoi.Instance.GetCurPathLength()));
                }

                
            }
        }

        ScreenBattle.instance.UpdatePlayerExp();
        ScreenBattle.instance.UpdatePlayerGold();
    }

    public bool HaveSkillPlus(BuffType skill)
    {
        var listMods = GetListRuntimeStatMods();
        if (listMods != null)
        {
            var mod = listMods.Find(e => e.Stat == EStatType.SKILLPLUS && e.Target == skill.ToString());
            return mod != null;
        }
        return false;
    }

    public bool HaveHeroPlus(string heroName)
    {
        var listMods = GetListRuntimeStatMods();
        if (listMods != null)
        {
            var mod = listMods.Find(e => e.Stat == EStatType.HEROPLUS && e.Target == heroName);
            return mod != null;
        }
        return false;
    }

    public List<StatModWraper> GetListRuntimeStatMods()
    {
        List<StatModWraper> listResult = null;
        if (battleData != null)
        {
            listResult = battleData.ListMods;
        }

#if UNITY_EDITOR
        if (listResult == null)
        {
            listResult = new List<StatModWraper>();

            // cheat test mod here
            listResult.AddRange(new StatModWraper[]
            {
            //new StatModWraper {
            //    Stat = EStatType.SKILLPLUS,
            //    Target = BuffType.BACK_SHOT.ToString()
            //},
            //new StatModWraper {
            //    Stat = EStatType.SKILLPLUS,
            //    Target = "Field",
            //    Val = 20
            //},
            //new StatModWraper {
            //    Stat = EStatType.HEROPLUS,
            //    Target = "BeastMaster"
            //},
            //new StatModWraper {
            //    Stat = EStatType.HEROPLUS,
            //    Target = "Thor",
            //    Val = 6,
            //},
            //new StatModWraper {
            //    Stat = EStatType.HEROPLUS,
            //    Target = "Wukong",
            //    Val = 180,
            //},
            //new StatModWraper {
            //    Stat = EStatType.HEROPLUS,
            //    Target = "Magician"
            //},
            //new StatModWraper {
            //    Stat = EStatType.REGEN,
            //    Val = 200
            //},
            //new StatModWraper {
            //    Stat = EStatType.BLESSINGPLUS,
            //    Val = 5
            //}
            //});
            //new StatModWraper {
            //    Stat = EStatType.DMG_ON_FREEZE,
            //    Val = 40
            //}
            });
        }
#endif
        return listResult;
    }

#if UNITY_EDITOR
    void StartTestMission()
    {
        EnablePlayerUnit();

        if (GateVisualController.instance)
            GateVisualController.instance.SetMission(0, false);

        IncreaseGioiHanBuff(10000);
        //PopupRollingBuff.Create();

        //bad sua o day
#if DEBUG
        //QuanlyNguoichoi.instance.GetBuff(BuffType.MULTI_SHOT);
        //QuanlyNguoichoi.instance.GetBuff(BuffType.ADD_SHOT);
        //QuanlyNguoichoi.instance.GetBuff(BuffType.CHEO_SHOT);
        //QuanlyNguoichoi.instance.GetBuff(BuffType.TRACKING_BULLET);
        //QuanlyNguoichoi.instance.GetBuff(BuffType.SIDE_SHOT);
        //QuanlyNguoichoi.instance.GetBuff(BuffType.BACK_SHOT);
        //QuanlyNguoichoi.instance.GetBuff(BuffType.PIERCING);
        //QuanlyNguoichoi.instance.GetBuff(BuffType.BOUNCING);
        //QuanlyNguoichoi.instance.GetBuff(BuffType.BIG_BULLET);

        //QuanlyNguoichoi.instance.GetBuff(BuffType.RICOCHET);
        //QuanlyNguoichoi.instance.GetBuff(BuffType.SLOW_PROJECTILE);

        //QuanlyNguoichoi.instance.GetBuff(BuffType.ELEMENT_CRIT);
        //QuanlyNguoichoi.instance.GetBuff(BuffType.LIFE_LEECH);
        //QuanlyNguoichoi.instance.GetBuff(BuffType.FLAME_FIELD);
        //QuanlyNguoichoi.instance.GetBuff(BuffType.FLAME_FIELD);
        //QuanlyNguoichoi.instance.GetBuff(BuffType.FROZEN_FIELD);
        //QuanlyNguoichoi.instance.GetBuff(BuffType.FROZEN_FIELD);
        //QuanlyNguoichoi.instance.GetBuff(BuffType.ELECTRIC_FIELD);
        //QuanlyNguoichoi.instance.GetBuff(BuffType.LINKEN);

        //QuanlyNguoichoi.instance.GetBuff(BuffType.CRIT_UP);
        //QuanlyNguoichoi.instance.GetBuff(BuffType.CRIT_UP);
        //QuanlyNguoichoi.instance.GetBuff(BuffType.CRIT_UP);
        //QuanlyNguoichoi.instance.GetBuff(BuffType.ELECTRIC_EFF);
        //QuanlyNguoichoi.instance.GetBuff(BuffType.FLY);
        //QuanlyNguoichoi.instance.GetBuff(BuffType.FROZEN_EFF);
        //QuanlyNguoichoi.instance.GetBuff(BuffType.FLAME_EFF);
        //QuanlyNguoichoi.instance.GetBuff(BuffType.DWARF);
        //QuanlyNguoichoi.instance.GetBuff(BuffType.SHIELD);

        //QuanlyNguoichoi.instance.GetBuff(BuffType.RUNNING_CHARGE);
        //QuanlyNguoichoi.instance.GetBuff(BuffType.GIANT);
#endif
        if (PlayerUnit.UnitName == "IronMan")
        {
            if (SoLuotDungKyNang == 0)
            {
                PopupChonDeTuIR.Create();
            }
        }
    }
#endif


    float countTimeHistory = 0;
    int historyIdx = 0;
    int fillHistory = 0;
    void UpdateHistoryPos()
    {
        if (fillHistory < NumHistoryPos)
        {
            fillHistory++;
        }

        HistoryPos[historyIdx] = PlayerUnit.transform.position;

        historyIdx++;
        if (historyIdx >= NumHistoryPos)
        {
            historyIdx -= NumHistoryPos;
        }
    }

    void ResetHistory()
    {
        fillHistory = 0;
        historyIdx = 0;
        countTimeHistory = 0;
    }

    public Vector3 GetHistoryPos(float time)
    {
        int idx = historyIdx;
        int countLast = 0;
        while (time >= IntervalHistory && countLast < fillHistory)
        {
            countLast++;
            time -= IntervalHistory;
            if (--idx < 0)
            {
                idx += NumHistoryPos;
            }
        }

        if (countLast == 0 || fillHistory == 0)
        {
            if (PlayerUnit && PlayerUnit.gameObject.activeSelf)
            {
                return PlayerUnit.transform.position;
            }
        }

        return HistoryPos[idx];
    }

    private void Update()
    {
        if (PlayerUnit != null && PlayerUnit.gameObject.activeSelf)
        {
            countTimeHistory += Time.deltaTime;
            if (countTimeHistory >= IntervalHistory)
            {
                countTimeHistory -= IntervalHistory;
                UpdateHistoryPos();
            }
        }

        // CHEAT 
#if UNITY_EDITOR
        if ( Input.GetKeyDown(KeyCode.F1))
        {
            GoToNextMission();
        }
#endif
        // end

        //if (LevelController.instance == null)
        //{
        //    PlayerUnit.gameObject.SetActive(false);
        //}
        //else
        //{
        //    PlayerUnit.gameObject.SetActive(true);
        //}

        if (IsLevelClear == false && IsEndGame == false)
        {
            mBattleTime += Time.deltaTime;

            if (IsNienThuMode)
            {
                mThoiGianNienThu += Time.deltaTime;

                if (mThoiGianNienThu > ConfigManager.TetEventCfg.ThoiGianDanhBoss)
                {
                    ScreenBattle.PauseGame(true);
                    StartCoroutine(CoEndGame(false));
                }
            }
        }
    }

    //public void OnRoundSanTheClear()
    //{
    //    GoToNextSanTheRound();
    //}

    public void OnEnemyInit(DonViChienDau enemyUnit)
    {
        if (IsNienThuMode && enemyUnit.IsBoss)
        {
            enemyUnit.OnUnitTakenDMG += UnitBossNienThu_OnUnitTakenDMG;
        }
    }

    private void UnitBossNienThu_OnUnitTakenDMG(long obj)
    {
        mSatThuongNienThu += obj;
    }

    public void OnLevelClear()
    {
        if (PlayerUnit) PlayerUnit.SetInvulnerableTime(10000f);
        StartCoroutine(CoProcessDrop(false));
    }

    public void OnEnemyDie(DonViChienDau unit)
    {
        killedEnemy.Add(unit.UnitName);
    }

    bool IsEndNode()
    {
        var curNode = GetCurrentNodeLevelConfig();
        return curNode.Levels.Length - 1 <= MissionID;
    }

    bool IsEndChapter()
    {
        var node = nodeMapConfig.points[CurDungeonId];
        if (node.GetComponent<NodeLevelConfig>().LevelType == ENodeType.Boss &&
            node.connections.Count == 0)
        {
            return IsEndNode();
        }

        return false;
    }

    public bool IsMissionEndGame()
    {
        if (IsNormalMode && IsEndChapter())
        {
            return true;
        }
        else if (IsFarmMode && farmConfig.Missions.Length == MissionID)
        {
            return true;
        }
        else if (IsLeoThapMode)
        {
            int blockIdx = ConfigManager.LeoThapBattleCfg.GetBlockIdxFromMission(MissionID - 1);
            var maxNumBlock = ConfigManager.LeoThapBattleCfg.BlockArray.Length;
            if (blockIdx == maxNumBlock - 1)
            {
                var missionInBlockIdx = (MissionID - 1) % ConfigManager.LeoThapBattleCfg.BlockLevel.Length;
                if (missionInBlockIdx == (ConfigManager.LeoThapBattleCfg.BlockLevel.Length - 1))
                {
                    return true;
                }
            }
        }
        else if (IsThachDauMode)
        {
            string rankName = "Rank" + (ThachDauRankIdx + 1);
            var blocks = ConfigManager.ThachDauBattleCfg.BlockArrayByRank[rankName];
            int blockIdx = ConfigManager.ThachDauBattleCfg.GetBlockIdxFromMission(MissionID - 1);
            var maxNumBlock = blocks.Length;
            if (blockIdx == maxNumBlock - 1)
            {
                return true;
            }
        }
        else if (IsSanTheMode)
        {
            if (MissionID == 4)
            {
                return true;
            }
        }
        else if (IsNienThuMode)
        {
            if (missionID > 0)
            {
                return true;
            }
        }
        return false;
    }

    bool procesingDrop = false;

    public IEnumerator CoProcessDrop(bool cleanUpOnly)
    {
        int lastLevel = PlayerLvl;

        if (procesingDrop == false)
        {
            procesingDrop = true;

            var listMods = GetListRuntimeStatMods();
            if (listMods != null)
            {
                var modRegen = listMods.Find(e => e.Stat == EStatType.REGEN);
                if (modRegen != null && modRegen.Val > 0)
                {
                    PlayerUnit.RegenHP((long)Mathf.Round((float)modRegen.Val), true, "+Regen");
                    yield return new WaitForSeconds(0.5f);
                }
            }

            if (PlayerUnit.IsAlive() && PlayerUnit.UnitName == "IronMan")
            {
                var remainHPToFull = PlayerUnit.GetMaxHP() - PlayerUnit.GetCurHP();
                if (remainHPToFull > 0)
                {
                    var skill = PlayerUnit.GetKyNang().GetTuyetKy(0) as IronManSkill;
                    if (skill != null && skill.DeTu == "IronManMedic")
                    {
                        PlayerUnit.RegenHP(remainHPToFull, true, "+MedicFull");
                    }
                }
            }

            int numEXPCount = 0;
            long totalExp = 0;
            
            for (int i = 0; i < listDrops.Count; ++i)
            {
                if (listDrops[i] != null && listDrops[i].DropType == UnitDropType.Exp)
                {
                    var expDrop = listDrops[i] as UnitExpDrop;
                    totalExp += expDrop.EXP;
                    numEXPCount++;
                }
            }

            if (listDrops.Count > 0)
            {
                long remainGold = QuanlyManchoi.instance.LevelGold;
                if (IsGreedy || numEXPCount == 0)
                {
                    remainGold = 0;
                }

                long gold = numEXPCount > 0 ? remainGold / numEXPCount : 0;

                int maxCount = 3; // so luot gom resource drop toi da

                var waveController = QuanlyManchoi.instance.WaveCtrler;
                if (waveController && waveController.Waves.Length > 1)
                {
                    maxCount = Mathf.Max(maxCount, waveController.Waves.Length * 2);
                }

                int countPerOnce = listDrops.Count / maxCount;
                int frameIndex = 0;
                for (int i = listDrops.Count - 1; i >= 0; --i)
                {
                    ConsumDrop(listDrops[i]);

                    PlayerGold += gold;
                    remainGold -= gold;
                    ScreenBattle.instance.UpdatePlayerGold();
                    if (frameIndex == countPerOnce)
                    {
                        frameIndex = 0;
                        yield return new WaitForSecondsRealtime(0.15f);

                        //if (IsMissionEndGame())
                        //{
                        //}
                        //else
                        {
                            //if (lastLevel < PlayerLvl)
                            //{
                            //    IncreaseGioiHanBuff(PlayerLvl - lastLevel);
                            //}

                            while (lastLevel < PlayerLvl)
                            {
                                if (QuanlyNguoichoi.instance.PlayerUnit.IsAlive() ||
                                    QuanlyNguoichoi.instance.PlayerUnit.Life > 0)
                                {
                                    PopupRollingBuff.Create();
                                    while (PopupRollingBuff.instance) // PAUSE TO WAIT USER PICK BUFF BEFORE CONTINUE
                                    {
                                        yield return new WaitForSecondsRealtime(0.15f);
                                    }
                                }
                                lastLevel++;
                            }
                        }
                    }
                    else
                    {
                        frameIndex++;
                    }
                }
                PlayerGold += remainGold;
                ScreenBattle.instance.UpdatePlayerGold();
            }

            yield return new WaitForSecondsRealtime(0.5f);
            ScreenBattle.instance.UpdatePlayerExp();

            //if (IsMissionEndGame())
            //{
            //}
            //else
            {
                //if (lastLevel < PlayerLvl)
                //{
                //    IncreaseGioiHanBuff(PlayerLvl - lastLevel);
                //}
                while (lastLevel < PlayerLvl) // dam bao la` khong bi sot luot len level
                {
                    if (QuanlyNguoichoi.instance.PlayerUnit.IsAlive() ||
                        QuanlyNguoichoi.instance.PlayerUnit.Life > 0)
                    {
                        PopupRollingBuff.Create();
                        while (PopupRollingBuff.instance) // PAUSE TO WAIT USER PICK BUFF BEFORE CONTINUE
                        {
                            yield return new WaitForSecondsRealtime(0.15f);
                        }
                    }
                    lastLevel++;
                }
            }

            listDrops.Clear();
            ScreenBattle.instance.UpdatePlayerGold();
        }

        if (IsEndGame == false)
        {
            if (IsNormalMode)
            {
                var curNodeLevelCfg = GetCurrentNodeLevelConfig();
                var chapcfg = ConfigManager.chapterConfigs[chapterIndex];

                if (string.IsNullOrEmpty(curNodeLevelCfg.BossCfg) == false && curNodeLevelCfg.LevelType == ENodeType.Boss)
                {
                    listBossHunted.Add(curNodeLevelCfg.BossCfg);
                }

                if (string.IsNullOrEmpty(curNodeLevelCfg.RuongConfig) == false &&
                    chapcfg.Chests.ContainsKey(curNodeLevelCfg.RuongConfig))
                {
                    var chestVisual = GameObject.FindObjectOfType<BattleChest>();
                    if (chestVisual == null)
                    {
                        if (GameClient.instance.OfflineMode == false)
                        {
                            var chapData = GameClient.instance.UInfo.ListChapters.Find(e => e.ChapIdx == ChapterIndex);
                            if (chapData != null && chapData.RuongOpened.Contains(curNodeLevelCfg.RuongConfig) == false)
                            {
                                var chestPrefab = Resources.Load<GameObject>("LevelDesign/Chest");
                                var originPos = chestPrefab.transform.position;
                                var chestPos = GateController.instance.transform.position + Vector3.back * 6;
                                chestPos.y = originPos.y;
                                var chest = Instantiate(chestPrefab, chestPos + Vector3.up * 30, Quaternion.LookRotation(Vector3.back));
                                var tween = TweenPosition.Begin(chest.gameObject, 0.5f, chestPos);
                                tween.method = UITweener.Method.BounceIn;
                                chest.name = "Chest";
                                yield break;
                            }
                        }
                    }
                }
            }

            if (cleanUpOnly == false)
            {
                IsLoadingMission = false;
                IsLevelClear = true;

                LastLvlBattleTime = mBattleTime;

                UpdateBattleProgress();
                //SaveBuffRandomState(battleData != null ? battleData.ID : string.Empty);
            }

            if (GateController.instance)
            {
                GateController.instance.gameObject.SetActive(false);
            }

            if (GateVisualController.instance)
            {
                GateVisualController.instance.OpenGate();
            }

            // reactive linken when map is cleared
            var linkenCount = PlayerUnit.GetBuffCount(BuffType.LINKEN);

            if (linkenCount > 0)
            {
                if (PlayerUnit.GetStatusEff().IsHaveActiveEffect(EffectType.Linken) == false)
                {
                    PlayerUnit.GetStatusEff().ApplyLinkenEffect();
                }
            }
        }
    }

    void ActivateConsumeDropMovement(GameObject visual)
    {
        if (visual != null)
        {
            var dropMv = visual.AddMissingComponent<UnitDropMovement>();
            if (dropMv != null)
            {
                dropMv.maxHeight = 10f;
                dropMv.RandomRadius = 0f;
                dropMv.Activate(60, PlayerUnit.transform.position);
            }
        }
    }

    void DeactiveDropVisual(UnitDrop drop)
    {
        if (drop != null && drop.Visuals != null)
        {
            for (int i = 0; i < drop.Visuals.Length; ++i)
            {
                var visual = drop.Visuals[i];
                if (visual != null)
                {
                    visual.SetActive(false);
                    ObjectPoolManager.Unspawn(visual);
                }
            }
        }
    }

    public void ConsumDrop(UnitDrop drop)
    {
        if (PlayerUnit == null || drop == null) return;

        switch (drop.DropType)
        {
            case UnitDropType.Exp:
                {
                    for (int i = 0; i < drop.Visuals.Length; ++i)
                    {
                        var visual = drop.Visuals[i];
                        ActivateConsumeDropMovement(visual);
                    }
                    Hiker.HikerUtils.DoAction(this,
                        () => {
                            if (drop == null) return;
                            var expDrop = drop as UnitExpDrop;
                            GetExp(expDrop.EXP);
                            DeactiveDropVisual(drop);
                        },
                        0.25f, true);
                }
                break;
            case UnitDropType.HealOrb:
                {
                    var visual = drop.Visuals[0];
                    ActivateConsumeDropMovement(visual);
                    Hiker.HikerUtils.DoAction(this,
                        () => {
                            if (drop == null) return;
                            var healOrb = drop as HealOrb;
                            GetRegen(healOrb.HPRegen, "+Orb");
                            DeactiveDropVisual(drop);
                        },
                        0.25f, true);
                }
                break;
            case UnitDropType.Gem:
                {
                    var visual = drop.Visuals[0];
                    ActivateConsumeDropMovement(visual);
                    var resDrop = drop as UnitResDrop;
                    //playerLoot.Add(CardReward.GEM_CARD, (int)resDrop.Count);
                    if (playerLoot.ContainsKey(CardReward.GEM_CARD))
                    {
                        playerLoot[CardReward.GEM_CARD] += (int)resDrop.Count;
                    }
                    else
                    {
                        playerLoot.Add(CardReward.GEM_CARD, (int)resDrop.Count);
                    }
                    Hiker.HikerUtils.DoAction(this,
                        () => {
                            if (drop == null) return;
                            DeactiveDropVisual(drop);
                        },
                        0.25f, true);
                }
                break;
            case UnitDropType.Gold:
                {
                    var visual = drop.Visuals[0];
                    ActivateConsumeDropMovement(visual);
                    var resDrop = drop as UnitResDrop;
                    //playerLoot.Add(CardReward.GOLD_CARD, (int)resDrop.Count);
                    if (IsGreedy == false)
                    {
                        if (playerLoot.ContainsKey(CardReward.GOLD_CARD))
                        {
                            playerLoot[CardReward.GOLD_CARD] += (int)resDrop.Count;
                        }
                        else
                        {
                            playerLoot.Add(CardReward.GOLD_CARD, (int)resDrop.Count);
                        }
                    }

                    Hiker.HikerUtils.DoAction(this,
                        () => {
                            if (drop == null) return;
                            DeactiveDropVisual(drop);
                        },
                        0.25f, true);
                }
                break;
            case UnitDropType.Material:
                {
                    var visual = drop.Visuals[0];
                    ActivateConsumeDropMovement(visual);
                    var matDrop = drop as UnitMaterialDrop;
                    //playerLoot.Add(matDrop.Name, (int)matDrop.Count);
                    if (playerLoot.ContainsKey(matDrop.Name))
                    {
                        playerLoot[matDrop.Name] += (int)matDrop.Count;
                    }
                    else
                    {
                        playerLoot.Add(matDrop.Name, (int)matDrop.Count);
                    }

                    Hiker.HikerUtils.DoAction(this,
                        () => {
                            if (drop == null) return;
                            DeactiveDropVisual(drop);
                        },
                        0.25f, true);
                }
                break;
            default:
                break;
        }
    }

    void GetExp(long exp, bool skipExpAtStart = true)
    {
        if (MissionID == 0 && CurDungeonId == 0 && skipExpAtStart)
        {
            // khong duoc nhan exp o bai start.
            return; 
        }
        // chuyen phan buff exp cua nguoi choi duoc buff truoc o moi khi load level (Level Controller)
        //int totalBuff = playerExpBuff;
        //if (PlayerUnit.GetBuffCount(BuffType.SMART) > 0)
        //{
        //    var smartcfg = PlayerManager.instance.GetBuffStatByType(BuffType.SMART);
        //    totalBuff += smartcfg.Params[0];
        //}
        //var expBuff = exp * totalBuff / 100;
        ////if (expBuff <= 0) expBuff = 1;
        //exp += expBuff;

        PlayerExp += exp;
#if DEBUG
        Debug.Log("GetExp " + exp);
#endif
        var lastLevelExp = QuanlyNguoichoi.instance.gameConfig.GetTotalExpAtLevel(PlayerLvl - 1);
        var nextLevelExp = QuanlyNguoichoi.instance.gameConfig.GetTotalExpAtLevel(PlayerLvl);
        while (PlayerExp >= nextLevelExp && PlayerLvl < QuanlyNguoichoi.instance.gameConfig.LevelExp.Length)
        {
            PlayerLvl++;
#if DEBUG
            Debug.Log("Increase Level " + PlayerLvl);
#endif
            IncreaseGioiHanBuff(1);

            GUIManager.Instance.PlaySound("Sound/SFX/Battle/levelup", this.PlayerUnit.transform.position,1);

            lastLevelExp = QuanlyNguoichoi.instance.gameConfig.GetTotalExpAtLevel(PlayerLvl - 1);
            nextLevelExp = QuanlyNguoichoi.instance.gameConfig.GetTotalExpAtLevel(PlayerLvl);

            //var battleData = PlayerManager.Instance.battleData;
            if (battleData != null &&
                battleData.ListMods != null)
            {
                int baseHP = ConfigManager.GetBaseHPLevelUp();
                float buff = 0;
                float scale = 0;
                foreach (var m in battleData.ListMods)
                {
                    if (m.Stat == EStatType.HPLEVELUP)
                    {
                        if (m.Mod == EStatModType.ADD)
                        {
                            baseHP += (int)m.Val;
                        }
                        else if (m.Mod == EStatModType.MUL)
                        {
                            scale += (float)m.Val;
                        }
                        else if (m.Mod == EStatModType.BUFF)
                        {
                            buff += (float)m.Val;
                        }
                    }
                }

                if (scale > 0)
                {
                    baseHP = (int)(baseHP * scale / 100);
                }
                if (buff > 0)
                {
                    baseHP += (int)(baseHP * buff / 100);
                }

                GetRegen(baseHP, "+LvlUp");
            }
        }

        ScreenBattle.instance.UpdatePlayerExp();
    }

    void GetRegen(int hpRegen, string nguon)
    {
        PlayerUnit.RegenHP(hpRegen, true, nguon);
    }

    public UnitDrop HaveADrop(UnitDrop drop)
    {
        if (drop.DropType == UnitDropType.Gem)
        {
            var resDrop = drop as UnitResDrop;
            resDrop.Visuals = new GameObject[1];
            var visualObj = ObjectPoolManager.Spawn(DropGemVisual);
            resDrop.Visuals[0] = visualObj;

            //if (playerLoot.ContainsKey(CardReward.GEM_CARD))
            //{
            //    playerLoot[CardReward.GEM_CARD] += (int)resDrop.Count;
            //}
            //else
            //{
            //    playerLoot.Add(CardReward.GEM_CARD, (int)resDrop.Count);
            //}

            listDrops.Add(drop);
        }
        else
        if (drop.DropType == UnitDropType.Gold)
        {
            var resDrop = drop as UnitResDrop;

            //resDrop.Visuals = new GameObject[1];
            //var visualObj = ObjectPoolManager.Spawn(DropGoldVisual);

            resDrop.Visuals = new GameObject[Random.Range(3, 6)];
            for (int i = 0; i < resDrop.Visuals.Length; ++i)
            {
                var visualObj = ObjectPoolManager.Spawn(DropGoldVisual);
                resDrop.Visuals[i] = visualObj;
            }

            //resDrop.Visuals[0] = visualObj;

            //if (IsGreedy == false)
            //{
            //    if (playerLoot.ContainsKey(CardReward.GOLD_CARD))
            //    {
            //        playerLoot[CardReward.GOLD_CARD] += (int)resDrop.Count;
            //    }
            //    else
            //    {
            //        playerLoot.Add(CardReward.GOLD_CARD, (int)resDrop.Count);
            //    }
            //}

            listDrops.Add(drop);
        }
        else
        if (drop.DropType == UnitDropType.Material)
        {
            var matDrop = drop as UnitMaterialDrop;
            matDrop.Visuals = new GameObject[1];
            var visualObj = ObjectPoolManager.Spawn(DropMaterialVisual);
            var visual = visualObj.GetComponent<DropMaterialVisual>();
            visual.SetMaterial(matDrop.Name);
            matDrop.Visuals[0] = visualObj;

            //if (playerLoot.ContainsKey(matDrop.Name))
            //{
            //    playerLoot[matDrop.Name] += (int)matDrop.Count;
            //}
            //else
            //{
            //    playerLoot.Add(matDrop.Name, (int)matDrop.Count);
            //}

            listDrops.Add(drop);
        }
        else
        if (drop.DropType == UnitDropType.HealOrb)
        {
            var healOrb = drop as HealOrb;
            healOrb.Visuals = new GameObject[1];
            var visualObj = ObjectPoolManager.Spawn(HealOrbVisual);
            healOrb.Visuals[0] = visualObj;
        }
        else
        {
            if (drop.DropType == UnitDropType.Exp)
            {
                drop.Visuals = new GameObject[Random.Range(3, 6)];
                for (int i = 0; i < drop.Visuals.Length; ++i)
                {
                    var visualObj = ObjectPoolManager.Spawn(ExpDropVisual);
                    drop.Visuals[i] = visualObj;
                }
            }
            listDrops.Add(drop);
        }
        return drop;
    }

    public void GoToDungeon(int dungeonId, int missionID = 0, bool updateBattleData = true)
    {
#if UNITY_EDITOR
        Debug.LogFormat("GoToDungeon {0}-{1}", ChapterIndex + 1, dungeonId);
#endif
        this.CurDungeonId = dungeonId;
        if (this.CurrentPath.Count == 0 ||
            this.CurrentPath[this.CurrentPath.Count - 1] != dungeonId)
        {
            this.CurrentPath.Add(dungeonId);

            if (battleData != null && battleData.IsNormalBattle())
            {
                var nodeLevelCfg = GetCurrentNodeLevelConfig();

                this.ListNodePaths.Add((int)nodeLevelCfg.LevelType);
            }
        }
        MissionID = missionID;

        LoadMission(MissionID, updateBattleData);

        ScreenBattle.PauseGame(false);
    }

    float lastTimeUpdatePlayerStat = -10;
    public void UpdatePlayerStat(bool checkTime = false)
    {
        var lastTimeCheck = lastTimeUpdatePlayerStat;
        lastTimeUpdatePlayerStat = Time.timeSinceLevelLoad;

        UpdateBattleRequest lastUpdate = GameClient.instance.GetLastBattleRequest(BattleMode);

        if (lastUpdate == null)
        {
            UpdateBattleProgress();
        }
        else
        {
            lastUpdate.PlayerStat = PlayerUnit.GetCurStat().ToUnitStatWrapper();
            lastUpdate.Life = PlayerUnit.Life;
            lastUpdate.LuotHS = LuotHS;
            lastUpdate.CurLvlClear = IsLevelClear;
            lastUpdate.HeroSK = SoLuotDungKyNang;
            lastUpdate.BuffRandomState = JsonUtility.ToJson(BuffRandomStated);
            if (BattleMode == 2)
            {
                lastUpdate.BattleTime = (long)System.Math.Floor(LastLvlBattleTime);
            }
            else
            if (BattleMode == 4)
            {
                lastUpdate.BattleTime = (long)System.Math.Floor(LastLvlBattleTime);
            }
            else
            {
                lastUpdate.BattleTime = (long)System.Math.Floor(mBattleTime);
            }
            lastUpdate.CCode = GetCCode();

            lastUpdate.MaxHP = PlayerUnit.GetMaxHP();

            if (checkTime && lastTimeUpdatePlayerStat - lastTimeCheck < 10)
            {
                var reqJson = LitJson.JsonMapper.ToJson(lastUpdate);
                GameClient.instance.SaveLastBattleRequest(lastUpdate, reqJson);
                return;
            }
            else
            {
                GameClient.instance.RequestUpdateBattle(lastUpdate);
            }
        }
    }

    void UpdateProgressCampaign()
    {
        if (PlayerUnit != null && PlayerUnit.IsInited && battleData != null)
        {
            UpdateBattleRequest req = new UpdateBattleRequest();
            req.BattleID = battleData.ID;
            req.PlayerStat = PlayerUnit.GetCurStat().ToUnitStatWrapper();
            req.listBuffs = new BuffType[ListBuffs.Count];
            for (int i = 0; i < ListBuffs.Count; ++i)
            {
                req.listBuffs[i] = ListBuffs[i].Type;
            }
            req.Mat = playerMat;
            req.Gold = PlayerGold;
            req.Exp = PlayerExp;
            req.Life = PlayerUnit.Life;
            
            req.Path = CurrentPath.ToArray();
            req.NodePath = ListNodePaths.ToArray();
            var mapCfg = GetMapDungeonConfig();
            req.HPCfg = mapCfg.HPBuff;
            req.DMGCfg = mapCfg.DMG;

            req.CurMissionID = missionID;
            req.CurDungeonID = curDungeonId;
            req.playedScene = playedScenes.ToArray();
            req.TKSatThuong = mTKSatThuong;
            req.TKTranDanh = mListTKTranDanh;
#if SERVER_1_3
            req.TKSung = TKSung;
#endif
            req.CurLvlClear = IsLevelClear;
            req.HeroSK = SoLuotDungKyNang;
            req.LuotRR = LuotReroll;

            req.IsEnd = false;
            req.Loot = new Dictionary<string, int>();
            req.EventIdx = battleData.BattleEventIndex;
            foreach (var t in playerLoot)
            {
                req.Loot.Add(t.Key, t.Value);
            }
            req.RuongDaMo = new List<string>();
            foreach (var t in listRuongDaMo)
            {
                req.RuongDaMo.Add(t);
            }
            req.BossHunted = new List<string>();
            foreach (var t in listBossHunted)
            {
                req.BossHunted.Add(t);
            }
            req.IsGreedy = IsGreedy;
            req.LuotHS = LuotHS;
            req.BattleMode = 0;
            req.DeTu = mDeTu;
            req.MaxHP = PlayerUnit.GetMaxHP();
            req.BuffRandomState = JsonUtility.ToJson(BuffRandomStated);
            GameClient.instance.RequestUpdateBattle(req);
        }
    }

    void UpdateProgressFarmMode()
    {
        if (PlayerUnit != null && PlayerUnit.IsInited && battleData != null)
        {
            UpdateBattleRequest req = new UpdateBattleRequest();
            req.BattleID = battleData.ID;
            req.PlayerStat = PlayerUnit.GetCurStat().ToUnitStatWrapper();
            req.listBuffs = new BuffType[ListBuffs.Count];
            for (int i = 0; i < ListBuffs.Count; ++i)
            {
                req.listBuffs[i] = ListBuffs[i].Type;
            }
            req.Mat = playerMat;
            req.Gold = PlayerGold;
            req.Exp = PlayerExp;
            req.Life = PlayerUnit.Life;
            req.TKSatThuong = mTKSatThuong;
            req.TKTranDanh = mListTKTranDanh;
#if SERVER_1_3
            req.TKSung = TKSung;
#endif
            req.CurLvlClear = IsLevelClear;
            req.HeroSK = SoLuotDungKyNang;
            req.LuotRR = LuotReroll;

            req.CurMissionID = missionID;
            req.CurDungeonID = curDungeonId;
            //req.PassedMission = newMission - 1;
            req.playedScene = playedScenes.ToArray();
            req.IsEnd = false;
            req.Loot = new Dictionary<string, int>();
            req.EventIdx = battleData.BattleEventIndex;
            foreach (var t in playerLoot)
            {
                req.Loot.Add(t.Key, t.Value);
            }
            req.RuongDaMo = new List<string>();
            foreach (var t in listRuongDaMo)
            {
                req.RuongDaMo.Add(t);
            }
            req.BossHunted = new List<string>();
            foreach (var t in listBossHunted)
            {
                req.BossHunted.Add(t);
            }
            req.IsGreedy = IsGreedy;
            req.LuotHS = LuotHS;
            req.BattleMode = 1;
            req.DeTu = mDeTu;
            req.MaxHP = PlayerUnit.GetMaxHP();
            req.BuffRandomState = JsonUtility.ToJson(BuffRandomStated);
            GameClient.instance.RequestUpdateBattle(req);
        }
    }

    void UpdateProgressLeoThapMode()
    {
        if (PlayerUnit != null && PlayerUnit.IsInited && battleData != null)
        {
            UpdateBattleRequest req = new UpdateBattleRequest();
            req.BattleID = battleData.ID;
            req.PlayerStat = PlayerUnit.GetCurStat().ToUnitStatWrapper();
            req.listBuffs = new BuffType[ListBuffs.Count];
            for (int i = 0; i < ListBuffs.Count; ++i)
            {
                req.listBuffs[i] = ListBuffs[i].Type;
            }
            req.Mat = playerMat;
            req.Gold = PlayerGold;
            req.Exp = PlayerExp;
            req.Life = PlayerUnit.Life;
            req.TKSatThuong = mTKSatThuong;
            req.TKTranDanh = mListTKTranDanh;
#if SERVER_1_3
            req.TKSung = TKSung;
#endif
            req.CurLvlClear = IsLevelClear;
            req.HeroSK = SoLuotDungKyNang;
            req.LuotRR = LuotReroll;

            req.CurMissionID = missionID;
            req.CurDungeonID = curDungeonId;
            //req.PassedMission = newMission - 1;
            req.playedScene = playedScenes.ToArray();
            req.IsEnd = false;
            req.Loot = new Dictionary<string, int>();
            req.EventIdx = battleData.BattleEventIndex;
            foreach (var t in playerLoot)
            {
                req.Loot.Add(t.Key, t.Value);
            }
            req.RuongDaMo = new List<string>();
            foreach (var t in listRuongDaMo)
            {
                req.RuongDaMo.Add(t);
            }
            req.BossHunted = new List<string>();
            foreach (var t in listBossHunted)
            {
                req.BossHunted.Add(t);
            }
            req.IsGreedy = IsGreedy;
            req.LuotHS = LuotHS;

            req.BattleMode = 2;
            req.BattleTime = (long)System.Math.Floor(LastLvlBattleTime);
            req.CCode = GetCCode();
            req.Envi = mEnviName;
            req.DeTu = mDeTu;
            req.MaxHP = PlayerUnit.GetMaxHP();
            req.BuffRandomState = JsonUtility.ToJson(BuffRandomStated);
            GameClient.instance.RequestUpdateBattle(req);
        }
    }
    
    void UpdateProgressSanTheMode()
    {
        if (PlayerUnit != null && PlayerUnit.IsInited && battleData != null)
        {
            UpdateBattleRequest req = new UpdateBattleRequest();
            req.BattleID = battleData.ID;
            req.PlayerStat = PlayerUnit.GetCurStat().ToUnitStatWrapper();
            req.listBuffs = new BuffType[ListBuffs.Count];
            for (int i = 0; i < ListBuffs.Count; ++i)
            {
                req.listBuffs[i] = ListBuffs[i].Type;
            }
            req.Mat = playerMat;
            req.Gold = PlayerGold;
            req.Exp = PlayerExp;
            req.Life = PlayerUnit.Life;
            req.TKSatThuong = mTKSatThuong;
            req.TKTranDanh = mListTKTranDanh;
#if SERVER_1_3
            req.TKSung = TKSung;
#endif
            req.CurLvlClear = IsLevelClear;
            req.HeroSK = SoLuotDungKyNang;
            req.LuotRR = LuotReroll;
            req.KilledUnit = new string[killedEnemy.Count];
            killedEnemy.CopyTo(req.KilledUnit);

            req.CurMissionID = missionID;
            req.CurDungeonID = curDungeonId;
            //req.PassedMission = newMission - 1;
            req.playedScene = playedScenes.ToArray();
            req.IsEnd = false;
            req.Loot = new Dictionary<string, int>();
            req.EventIdx = battleData.BattleEventIndex;
            foreach (var t in playerLoot)
            {
                req.Loot.Add(t.Key, t.Value);
            }
            req.RuongDaMo = new List<string>();
            foreach (var t in listRuongDaMo)
            {
                req.RuongDaMo.Add(t);
            }
            req.BossHunted = new List<string>();
            foreach (var t in listBossHunted)
            {
                req.BossHunted.Add(t);
            }
            req.IsGreedy = IsGreedy;
            req.LuotHS = LuotHS;

            req.BattleMode = 3;
            req.BattleTime = (long)System.Math.Floor(LastLvlBattleTime);
            req.CCode = GetCCode();
            req.Envi = mEnviName;
            req.DeTu = mDeTu;
            req.MaxHP = PlayerUnit.GetMaxHP();
            req.BuffRandomState = JsonUtility.ToJson(BuffRandomStated);
            GameClient.instance.RequestUpdateBattle(req);
        }
    }

    void UpdateProgressThachDauMode()
    {
        if (PlayerUnit != null && PlayerUnit.IsInited && battleData != null)
        {
            UpdateBattleRequest req = new UpdateBattleRequest();
            req.BattleID = battleData.ID;
            req.PlayerStat = PlayerUnit.GetCurStat().ToUnitStatWrapper();
            req.listBuffs = new BuffType[ListBuffs.Count];
            for (int i = 0; i < ListBuffs.Count; ++i)
            {
                req.listBuffs[i] = ListBuffs[i].Type;
            }
            req.Mat = playerMat;
            req.Gold = PlayerGold;
            req.Exp = PlayerExp;
            req.Life = PlayerUnit.Life;
            req.TKSatThuong = mTKSatThuong;
            req.TKTranDanh = mListTKTranDanh;
#if SERVER_1_3
            req.TKSung = TKSung;
#endif
            req.CurLvlClear = IsLevelClear;
            req.HeroSK = SoLuotDungKyNang;
            req.LuotRR = LuotReroll;

            req.CurMissionID = missionID;
            req.CurDungeonID = curDungeonId;
            //req.PassedMission = newMission - 1;
            req.playedScene = playedScenes.ToArray();
            req.IsEnd = false;
            req.Loot = new Dictionary<string, int>();
            req.EventIdx = battleData.BattleEventIndex;
            foreach (var t in playerLoot)
            {
                req.Loot.Add(t.Key, t.Value);
            }
            req.RuongDaMo = new List<string>();
            foreach (var t in listRuongDaMo)
            {
                req.RuongDaMo.Add(t);
            }
            req.BossHunted = new List<string>();
            foreach (var t in listBossHunted)
            {
                req.BossHunted.Add(t);
            }
            req.IsGreedy = IsGreedy;
            req.LuotHS = LuotHS;

            req.BattleMode = 4;
            req.BattleTime = (long)System.Math.Floor(LastLvlBattleTime);
            req.CCode = GetCCode();
            req.Envi = mEnviName;
            req.DeTu = mDeTu;
            req.MaxHP = PlayerUnit.GetMaxHP();
            req.BuffRandomState = JsonUtility.ToJson(BuffRandomStated);
            GameClient.instance.RequestUpdateBattle(req);
        }
    }

    void UpdateProgressNienThuMode()
    {
        if (PlayerUnit != null && PlayerUnit.IsInited && battleData != null)
        {
            UpdateBattleRequest req = new UpdateBattleRequest();
            req.BattleID = battleData.ID;
            req.PlayerStat = PlayerUnit.GetCurStat().ToUnitStatWrapper();
            req.listBuffs = new BuffType[ListBuffs.Count];
            for (int i = 0; i < ListBuffs.Count; ++i)
            {
                req.listBuffs[i] = ListBuffs[i].Type;
            }
            req.Mat = playerMat;
            req.Gold = PlayerGold;
            req.Exp = PlayerExp;
            req.Life = PlayerUnit.Life;
            req.TKSatThuong = mTKSatThuong;
            req.TKTranDanh = mListTKTranDanh;
#if SERVER_1_3
            req.TKSung = TKSung;
#endif
            req.CurLvlClear = IsLevelClear;
            req.HeroSK = SoLuotDungKyNang;
            req.LuotRR = LuotReroll;

            req.CurMissionID = missionID;
            req.CurDungeonID = curDungeonId;
            //req.PassedMission = newMission - 1;
            req.playedScene = playedScenes.ToArray();
            req.IsEnd = false;
            req.Loot = new Dictionary<string, int>();
            req.EventIdx = battleData.BattleEventIndex;
            foreach (var t in playerLoot)
            {
                req.Loot.Add(t.Key, t.Value);
            }
            req.RuongDaMo = new List<string>();
            foreach (var t in listRuongDaMo)
            {
                req.RuongDaMo.Add(t);
            }
            req.BossHunted = new List<string>();
            foreach (var t in listBossHunted)
            {
                req.BossHunted.Add(t);
            }
            req.IsGreedy = IsGreedy;
            req.LuotHS = LuotHS;

            req.BattleMode = 5;
            req.BattleTime = (long)System.Math.Floor(LastLvlBattleTime);
            req.CCode = GetCCode();
            req.Envi = mEnviName;
            req.DeTu = mDeTu;
            req.MaxHP = PlayerUnit.GetMaxHP();
            req.BuffRandomState = JsonUtility.ToJson(BuffRandomStated);
            GameClient.instance.RequestUpdateBattle(req);
        }
    }

    void UpdateBattleProgress()
    {
        if (IsLeoThapMode)
        {
            UpdateProgressLeoThapMode();
        }
        else if (IsFarmMode)
        {
            UpdateProgressFarmMode();
        }
        else if (IsSanTheMode)
        {
            UpdateProgressSanTheMode();
        }
        else if (IsThachDauMode)
        {
            UpdateProgressThachDauMode();
        }
        else if (IsNormalMode)
        {
            UpdateProgressCampaign();
        }
        else if (IsNienThuMode)
        {
            UpdateProgressNienThuMode();
        }
    }

    public void LoadMission(int missionID, bool updateBattleData = true)
    {
#if DEBUG
        Debug.Log(string.Format("LOAD Chapter {0} Level {1} Mission {2}", chapterIndex, CurDungeonId, missionID));
#endif
        var curLevelNodeCfg = nodeMapConfig.points[CurDungeonId].GetComponent<NodeLevelConfig>();
        var sceneName = curLevelNodeCfg.Levels[MissionID];

        PlayerUnit.GetComponent<PlayerMovement>().enabled = false;

        RepostionHeroAndDeTu();

        IsLevelClear = false;
        LoadMission(sceneName);

        if (CurDungeonId > 0 || missionID > 0)
        {
            if (GameClient.instance.OfflineMode == false && updateBattleData)
            {
                UpdateProgressCampaign();

                //SaveBuffRandomState(battleData != null ? battleData.ID : string.Empty);
            }
        }
        else
        {
            //if (GameClient.instance.OfflineMode == false)
            //{
            //    SaveBuffRandomState(battleData != null ? battleData.ID : string.Empty);
            //}
        }
    }

    public void LoadMission(string mission)
    {
        StartCoroutine(CoLoadMission(mission));

        ScreenBattle.instance.FadeOutSkillDesc();
    }

    public void LoadMissionFarmMode(int newMission, string sceneName = null, bool updateBattleData = true)
    {
        RepostionHeroAndDeTu();
        IsLevelClear = false;
        StartCoroutine(CoLoadMissionFarmMode(newMission, sceneName, updateBattleData));
    }

    void RepostionHeroAndDeTu()
    {
        PlayerUnit.transform.position = Vector3.zero;
        PlayerUnit.transform.rotation = Quaternion.identity;

        for (int i = 0; i < listDeTu.Count; ++i)
        {
            if (listDeTu[i])
            {
                Vector2 posR = Random.insideUnitCircle * 3f;
                Vector3 pos = new Vector3(posR.x, 0, posR.y);

                listDeTu[i].Warp(pos);
            }
        }

        if (PlayerUnit.UnitName == "BeastMaster")
        {
            var tk = PlayerUnit.GetComponent<TuyetKyNhanVat>();
            if (tk != null)
            {
                var tk2 = tk.GetTuyetKy(0);
                if (tk2 is BeastMasterSkill)
                {
                    var skill = tk2 as BeastMasterSkill;
                    skill.TeleportBackToUnit();
                }
            }
        }

        ResetHistory();
        UpdateHistoryPos();
    }

    public void LoadMissionLeoThap(int newMission, string sceneName = null, bool updateBattleData = true)
    {
        RepostionHeroAndDeTu();
        IsLevelClear = false;
        StartCoroutine(CoLoadMissionLeoThapMode(newMission, sceneName, updateBattleData));
    }

    public void LoadMissionThachDau(int newMission, string sceneName = null, bool updateBattleData = true)
    {
        RepostionHeroAndDeTu();
        IsLevelClear = false;
        StartCoroutine(CoLoadMissionThachDauMode(newMission, sceneName, updateBattleData));
    }

    public void LoadMissionNienThu(int newMission, bool updateBattleData = true)
    {
        RepostionHeroAndDeTu();
        IsLevelClear = false;
        StartCoroutine(CoLoadMissionNienThuMode(newMission, updateBattleData));
    }

    public void LoadMissionSanThe(int newMission, string sceneName = null, bool updateBattleData = true)
    {
        RepostionHeroAndDeTu();
        IsLevelClear = false;
        StartCoroutine(CoLoadMissionSanTheMode(newMission, sceneName, updateBattleData));
    }

    string GetSceneNameByMissionThachDau(int newMission)
    {
        return ThachDauScenes[newMission - 1];
    }

    string GetSceneNameByMissionLeothap(int newMission)
    {
        var missionIdx = newMission - 1;
        var blockIdx = ConfigManager.LeoThapBattleCfg.GetBlockIdxFromMission(missionIdx);
        var missionInBlock = ConfigManager.LeoThapBattleCfg.GetMissionIdxInBlock(missionIdx);
        var missionBlockType = ConfigManager.LeoThapBattleCfg.Block[missionInBlock];

        if (missionBlockType == "Event")
        {
            return ConfigManager.LeoThapBattleCfg.EventLevel;
        }
        else
        {
            List<string> randomPools = new List<string>();
            var pools = ConfigManager.LeoThapBattleCfg.Pools[ConfigManager.LeoThapBattleCfg.BlockArray[blockIdx]];

            var sceneName = string.Empty;
            string[] scenePool;

            if (missionBlockType == "Boss")
            {
                scenePool = pools.LevelsBoss;
            }
            else
            {
                scenePool = pools.LevelsQuai;
            }

            for (int i = 0; i < scenePool.Length; ++i)
            {
                var s = scenePool[i];
                if (playedScenes.Contains(s) == false)
                {
                    randomPools.Add(s);
                }
            }

            int sceneRandIndex = Random.Range(0, randomPools.Count);

            if (randomPools.Count > 0)
                sceneName = randomPools[sceneRandIndex];
            else
                sceneName = scenePool[0];

            return sceneName;
        }
    }

    string GetSceneNameByMissionIndex(int newMission, out int segmentIndex)
    {
        string sceneName = string.Empty;
        int sceneIndex = newMission - 1;
        var seqScene = farmConfig.Missions[sceneIndex];
        int seqStartIndex = 0;
        int countCurSeqScenes = 0;

        // tim` mission index dau tien cua segment hien tai
        for (int i = sceneIndex; i >= 0; --i)
        {
            var lastseq = farmConfig.Missions[i];
            if (lastseq != seqScene)
            {
                seqStartIndex = i + 1;
                break;
            }
        }
        // dem so mission cua segment hien tai
        for (int i = seqStartIndex; i < farmConfig.Missions.Length; ++i)
        {
            var lastseq = farmConfig.Missions[i];
            if (lastseq == seqScene)
            {
                countCurSeqScenes++;
            }
            else
            {
                break;
            }
        }

        int passedNum = passedMission - seqStartIndex;

        if (passedNum < countCurSeqScenes) // chua hoan thanh segment
        {
            // lay mission theo dung' trinh tu
            sceneName = farmConfig.Segments[seqScene].Levels[sceneIndex - seqStartIndex];
        }
        else
        {
            List<string> randomPools = new List<string>();

            for (int i = 0; i < farmConfig.Segments[seqScene].Levels.Length; ++i)
            {
                var s = farmConfig.Segments[seqScene].Levels[i];
                if (PlayedScenes.Contains(s) == false)
                {
                    randomPools.Add(s);
                }
            }

            int sceneRandIndex = Random.Range(0, randomPools.Count);

            if (randomPools.Count > 0)
                sceneName = randomPools[sceneRandIndex];
            else
                sceneName = farmConfig.Segments[seqScene].Levels[0];

        }
        segmentIndex = seqScene;
        return sceneName;
    }

    IEnumerator CoLoadMission(string newMission)
    {
        IsLoadingMission = true;
        DisablePlayerUnit();

        for (int i = 1; i < UnityEngine.SceneManagement.SceneManager.sceneCount; ++i)
        {
            var level = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
            yield return UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(level);
        }

        string sceneName = newMission;
        if (PlayedScenes.Count == 0 ||
            PlayedScenes[PlayedScenes.Count - 1] != sceneName)
        {
            PlayedScenes.Add(sceneName);
        }

        ScreenBattle.instance.lbMapName.text = sceneName;
        ScreenBattle.instance.lbMapName.gameObject.SetActive(true);

        yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(
            sceneName,
            UnityEngine.SceneManagement.LoadSceneMode.Additive);
        var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(1);
        UnityEngine.SceneManagement.SceneManager.SetActiveScene(scene);
        RepostionHeroAndDeTu();
        ScreenBattle.instance.lbMapName.gameObject.SetActive(false);

        while (QuanlyManchoi.instance == null)
        {
            yield return null;
        }

        QuanlyManchoi.instance.AddUnit(PlayerUnit);

        EnablePlayerUnit();

        string tenMap = sceneName + "_" + curDungeonId;
        if (TKTranDanh == null || TKTranDanh.TenMap != tenMap)
        {
            TKTranDanh = new ThongKeSTNguoiChoi()
            {
                TenMap = tenMap,
                STLon = 0,
                STNho = 0,
                STTong = 0,
                LuotST = 0,
                SBan = 0,
                SCount = 0,
            };
            mListTKTranDanh.Add(TKTranDanh);
        }

        // Boss mission
        bool isLastLevel = false;
        bool isPrepareLastLevel = false;
        var nodeLevelCfg = nodeMapConfig.points[CurDungeonId].GetComponent<NodeLevelConfig>();

        if (nodeLevelCfg.Levels.Length > 0 &&
            nodeLevelCfg.Levels[nodeLevelCfg.Levels.Length - 1] == sceneName)
        {
            isLastLevel = true;
            int numOfHealOrb = 0;
            if(nodeLevelCfg.LevelType == ENodeType.Boss)
            {
                numOfHealOrb = ConfigManager.GetNumOfHealOrbNodeBoss();
            }
            else if (nodeLevelCfg.LevelType == ENodeType.Quai1
                || nodeLevelCfg.LevelType == ENodeType.Quai2)
            {
                numOfHealOrb = ConfigManager.GetNumOfHealOrbNodeQuai();
            }
            QuanlyManchoi.instance.NumHealOrb = numOfHealOrb;
        }
        else
        {
            int healOrbDropChance = ConfigManager.GetHealOrbDropChance();

            if (Random.Range(0, 100) < healOrbDropChance)
            {
                QuanlyManchoi.instance.NumHealOrb = 1;
            }
        }

        if (GateVisualController.instance)
            GateVisualController.instance.SetMission(MissionID, isPrepareLastLevel);

        //if (MissionID == 0 && CurDungeonId == 0)
        if (
#if UNITY_EDITOR
            TestMission == false &&
#endif
            IsInitedMission == false)
        {
            yield return StartCoroutine(CoStartMission());
        }

        IsLoadingMission = false;
        procesingDrop = false;

        if (IsNormalMode)
        {
            var curNodeLevelCfg = GetCurrentNodeLevelConfig();
            var chapcfg = ConfigManager.chapterConfigs[ChapterIndex];
            var ruongCfg = curNodeLevelCfg.RuongConfig;

            if (string.IsNullOrEmpty(ruongCfg) == false &&
                chapcfg.Chests.ContainsKey(ruongCfg))
            {
                bool openRuong = false;
                if (GameClient.instance.OfflineMode == false)
                {
                    var chapData = GameClient.instance.UInfo.ListChapters.Find(e => e.ChapIdx == ChapterIndex);
                    if (chapData != null && chapData.RuongOpened.Contains(ruongCfg))
                    {
                        openRuong = true;
                    }
                }

                if (openRuong)
                {
                    BattleChest chest = GameObject.FindObjectOfType<BattleChest>();
                    if (chest != null)
                    {
                        OnPlayerGetChest(chest);
                    }
                    //if (listRuongDaMo.Contains(ruongCfg) == false)
                    //{
                    //    listRuongDaMo.Add(ruongCfg);
                    //    var chestCfg = chapcfg.Chests[ruongCfg];

                    //    Hiker.HikerUtils.DoAction(this, () =>
                    //    {
                    //        Hiker.GUI.Shootero.PopupOpenChest.Create(chestCfg);
                    //    }, 0.2f, true);
                    //}
                }
            }
        }
    }

    public void ReviveNow()
    {
        if (PlayerUnit.GetCurHP() <= 0)
        {
            PlayerUnit.ReviveNow();
        }
    }

    IEnumerator CoLoadMissionFarmMode(int newMission, string preCfgSceneName, bool updateBattleData)
    {
        IsLoadingMission = true;

        DisablePlayerUnit();
        for (int i = 1; i < UnityEngine.SceneManagement.SceneManager.sceneCount; ++i)
        {
            var level = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
            yield return UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(level);
        }
        MissionID = newMission;

        var sceneName = farmConfig.StartLevel;

        if (newMission > 0)
        {
            if (string.IsNullOrEmpty(preCfgSceneName))
            {
                sceneName = GetSceneNameByMissionIndex(newMission, out int segmentIndex);
            }
            else
            {
                sceneName = preCfgSceneName;
            }

            this.SegmentIndex = segmentIndex;
        }

        Debug.Log("Load level : " + sceneName);
        ScreenBattle.instance.lbMapName.text = sceneName;
        ScreenBattle.instance.lbMapName.gameObject.SetActive(true);

        if (PlayedScenes.Count == 0 ||
            PlayedScenes[PlayedScenes.Count - 1] != sceneName)
        {
            PlayedScenes.Add(sceneName);
        }

        if (newMission > 0)
        {
            if (GameClient.instance.OfflineMode == false && updateBattleData)
            {
                UpdateProgressFarmMode();

                //SaveBuffRandomState(battleData != null ? battleData.ID : string.Empty);
            }
        }

        yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(
            sceneName,
            UnityEngine.SceneManagement.LoadSceneMode.Additive);
        var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(1);
        UnityEngine.SceneManagement.SceneManager.SetActiveScene(scene);

        RepostionHeroAndDeTu();

        ScreenBattle.instance.lbMapName.gameObject.SetActive(false);

        ScreenBattle.PauseGame(true);

        while (QuanlyManchoi.instance == null)
        {
            yield return null;
        }

        QuanlyManchoi.instance.AddUnit(PlayerUnit);
        ScreenBattle.PauseGame(false);

        EnablePlayerUnit();

        string tenMap = sceneName + "_" + newMission;
        if (TKTranDanh == null || TKTranDanh.TenMap != tenMap)
        {
            TKTranDanh = new ThongKeSTNguoiChoi()
            {
                TenMap = tenMap,
                STLon = 0,
                STNho = 0,
                STTong = 0,
                LuotST = 0,
                SBan = 0,
                SCount = 0,
            };
            mListTKTranDanh.Add(TKTranDanh);
        }

        // Boss mission
        if (farmConfig.IsBossMission(MissionID - 1, out bool isPrepareBoss))
        {
            QuanlyManchoi.instance.NumHealOrb = 3;

        }
        else if (isPrepareBoss)
        {
            QuanlyManchoi.instance.NumHealOrb = 3;
        }
        else
        {
            if (Random.Range(0, 100) < 20)
            {
                QuanlyManchoi.instance.NumHealOrb = 1;
            }
        }

        if (GateVisualController.instance)
            GateVisualController.instance.SetMission(MissionID, isPrepareBoss);

        //if (battleData != null && MissionID == 0)
        if (
#if UNITY_EDITOR
            TestMission == false &&
#endif
            IsInitedMission == false)
        {
            yield return StartCoroutine(CoStartMission());
        }

        IsLoadingMission = false;
        procesingDrop = false;
    }

    void SwapEnvi()
    {
        var enviCol = Resources.Load<EnviSceneColletion>("EnviSceneCollection");
        GameObject prefab = null;
        
        if (string.IsNullOrEmpty(mEnviName) == false)
        {
            foreach (var p in enviCol.enviPrefabs)
            {
                if (mEnviName == p.name)
                {
                    prefab = p;
                    break;
                }
            }
        }

        if (prefab == null)
        {
            mEnviName = string.Empty;
        }

        if (string.IsNullOrEmpty(mEnviName))
        {
            prefab = enviCol.enviPrefabs[Random.Range(0, enviCol.enviPrefabs.Length)];
            mEnviName = prefab.name;
        }

        ReplaceScene(prefab);
    }

    void ReplaceScene(GameObject enviPrefab)
    {
        var gameDesign = GameObject.Find("GAMEDESIGN");

        GameObject oldObj = null;
        for (int i = 0; i < gameDesign.transform.childCount; ++i)
        {
            var child = gameDesign.transform.GetChild(i);
            if (child.name.StartsWith("Envi_C"))
            {
                oldObj = child.gameObject;
                break;
            }
        }
        if (oldObj == null)
        {
            Debug.Log("old envi not found");
            return;
        }

        var newEnvi = Instantiate<GameObject>(enviPrefab, gameDesign.transform);
        newEnvi.transform.position = oldObj.transform.position;

        var newTop = newEnvi.transform.Find("Border/top");
        var oldTop = oldObj.transform.Find("Border/top");
        if (newTop != null && oldTop != null)
        {
            newTop.transform.position = oldTop.transform.position;
        }

        var floor = newEnvi.transform.Find("Floor");
        for (int i = 0; i < floor.childCount; ++i) // enable all floor
        {
            var child = floor.GetChild(i);
            child.gameObject.SetActive(true);
        }

        oldObj.gameObject.SetActive(false);
        Destroy(oldObj);

        ReplaceWaterVisual(enviPrefab.name);
    }

    static List<SpriteRenderer> listWaterRenderers = new List<SpriteRenderer>();
    void ReplaceWaterVisual(string enviName)
    {
        int cNum = int.Parse(enviName.Substring(6));
        string waterColName = "water_c" + cNum;
        var waterCol = Resources.Load<SpriteCollection>("EnviWater/" + waterColName);
        if (waterCol != null)
        {
            GetListWaters(listWaterRenderers);
            foreach (var sp in listWaterRenderers)
            {
                if (sp != null && sp.sprite != null)
                {
                    sp.sprite = waterCol.GetSprite(sp.sprite.name);
                }
            }
        }
    }
    void GetListWaters(List<SpriteRenderer> listWaterRenderers)
    {
        listWaterRenderers.Clear();
        var bObj = GameObject.Find("GAMEDESIGN/Block");
        if (bObj == null)
        {
            Debug.Log("BLOCK GRP NOT FOUND");
            return;
        }
        Transform grpBlocks = bObj.transform;
        for (int i = 0; i < grpBlocks.childCount; i++)
        {
            Transform child = grpBlocks.GetChild(i);
            if (!child.gameObject.name.StartsWith("Water")
                || !child.gameObject.activeSelf) continue;

            listWaterRenderers.AddRange(child.GetComponentsInChildren<SpriteRenderer>());
        }
    }

    IEnumerator CoLoadMissionLeoThapMode(int newMission, string preCfgSceneName, bool updateBattleData)
    {
        IsLoadingMission = true;

        DisablePlayerUnit();
        for (int i = 1; i < UnityEngine.SceneManagement.SceneManager.sceneCount; ++i)
        {
            var level = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
            yield return UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(level);
        }
        MissionID = newMission;

        var sceneName = ConfigManager.LeoThapBattleCfg.StartLevel;

        if (newMission > 0)
        {
            if (string.IsNullOrEmpty(preCfgSceneName))
            {
                sceneName = GetSceneNameByMissionLeothap(newMission);
            }
            else
            {
                sceneName = preCfgSceneName;
            }

            this.BlockIdx = ConfigManager.LeoThapBattleCfg.GetBlockIdxFromMission(newMission);
        }

        Debug.Log("Load level : " + sceneName);
        ScreenBattle.instance.lbMapName.text = sceneName;
        ScreenBattle.instance.lbMapName.gameObject.SetActive(true);

        if (PlayedScenes.Count == 0 ||
            PlayedScenes[PlayedScenes.Count - 1] != sceneName)
        {
            PlayedScenes.Add(sceneName);
        }

        if (newMission > 0)
        {
            if (GameClient.instance.OfflineMode == false && updateBattleData)
            {
                UpdateProgressLeoThapMode();

                //SaveBuffRandomState(battleData != null ? battleData.ID : string.Empty);
            }
        }

        yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(
            sceneName,
            UnityEngine.SceneManagement.LoadSceneMode.Additive);
        var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(1);
        UnityEngine.SceneManagement.SceneManager.SetActiveScene(scene);
        RepostionHeroAndDeTu();
        ScreenBattle.instance.lbMapName.gameObject.SetActive(false);

        SwapEnvi();

        ScreenBattle.PauseGame(true);

        while (QuanlyManchoi.instance == null)
        {
            yield return null;
        }

        QuanlyManchoi.instance.AddUnit(PlayerUnit);
        ScreenBattle.PauseGame(false);

        EnablePlayerUnit();

        string tenMap = sceneName + "_" + newMission;
        if (TKTranDanh == null || TKTranDanh.TenMap != tenMap)
        {
            TKTranDanh = new ThongKeSTNguoiChoi()
            {
                TenMap = tenMap,
                STLon = 0,
                STNho = 0,
                STTong = 0,
                LuotST = 0,
                SBan = 0,
                SCount = 0,
            };
            mListTKTranDanh.Add(TKTranDanh);
        }

        bool isPrepareBoss = false;
        if (missionID > 0)
        {
            // Boss mission
            if (ConfigManager.LeoThapBattleCfg.IsBossMission(MissionID - 1, out isPrepareBoss))
            {
                QuanlyManchoi.instance.NumHealOrb = 3;

            }
            else if (isPrepareBoss)
            {
                QuanlyManchoi.instance.NumHealOrb = 3;
            }
            else
            {
                if (Random.Range(0, 100) < 20)
                {
                    QuanlyManchoi.instance.NumHealOrb = 1;
                }
            }
        }

        if (GateVisualController.instance)
            GateVisualController.instance.SetMission(MissionID, isPrepareBoss);

        //if (battleData != null && MissionID == 0)
        if (
#if UNITY_EDITOR
            TestMission == false &&
#endif
            IsInitedMission == false)
        {
            yield return StartCoroutine(CoStartMission());
        }

        IsLoadingMission = false;
        procesingDrop = false;
        if (ScreenBattle.instance && IsLeoThapMode && MissionID > 0)
        {
            var idx = (MissionID - 1) % ConfigManager.LeoThapBattleCfg.BlockLevel.Length;
            if (ConfigManager.LeoThapBattleCfg.BlockLevel[idx] > 0)
            {
                ScreenBattle.instance.ShowMessage2(
                    string.Format(Localization.Get("LeoThapLevel"),
                    ConfigManager.LeoThapBattleCfg.GetTotalLevelFromMission(MissionID)));
            }

            if (LeoThapMod == 2 && PlayerUnit.GetMaCaRongModifier())
            {
                PlayerUnit.GetMaCaRongModifier().MaCaRongCurse();
            }
            else if (LeoThapMod == 5 && PlayerUnit.GetInfernalModifier())
            {
                // roi ca o bai quai' va` bai BOSS
                if (ConfigManager.LeoThapBattleCfg.BlockLevel[idx] > 0
                    //&& (idx + 1) < ConfigManager.LeoThapBattleCfg.BlockLevel.Length
                    )
                {
                    PlayerUnit.GetInfernalModifier().InfernalSkill();
                }
            }
        }
    }

    IEnumerator CoLoadMissionNienThuMode(int newMission, bool updateBattleData)
    {
        IsLoadingMission = true;

        DisablePlayerUnit();
        for (int i = 1; i < UnityEngine.SceneManagement.SceneManager.sceneCount; ++i)
        {
            var level = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
            yield return UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(level);
        };

        MissionID = newMission;

        var sceneName = ConfigManager.TetEventCfg.StartLevel;

        if (newMission > 0)
        {
            sceneName = NienThuScene;
        }

        Debug.Log("Load level : " + sceneName);
        ScreenBattle.instance.lbMapName.text = sceneName;
        ScreenBattle.instance.lbMapName.gameObject.SetActive(true);

        if (PlayedScenes.Count == 0 ||
            PlayedScenes[PlayedScenes.Count - 1] != sceneName)
        {
            PlayedScenes.Add(sceneName);
        }

        if (newMission > 0)
        {
            if (GameClient.instance.OfflineMode == false && updateBattleData)
            {
                UpdateProgressNienThuMode();

                //SaveBuffRandomState(battleData != null ? battleData.ID : string.Empty);
            }
        }
        mThoiGianNienThu = 0;
        yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(
            sceneName,
            UnityEngine.SceneManagement.LoadSceneMode.Additive);
        var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(1);
        UnityEngine.SceneManagement.SceneManager.SetActiveScene(scene);
        RepostionHeroAndDeTu();
        ScreenBattle.instance.lbMapName.gameObject.SetActive(false);

        //SwapEnvi();
        mThoiGianNienThu = 0;
        ScreenBattle.PauseGame(true);

        while (QuanlyManchoi.instance == null)
        {
            mThoiGianNienThu = 0;
            yield return null;
        }

        QuanlyManchoi.instance.AddUnit(PlayerUnit);
        ScreenBattle.PauseGame(false);

        EnablePlayerUnit();

        string tenMap = sceneName + "_" + newMission;
        if (TKTranDanh == null || TKTranDanh.TenMap != tenMap)
        {
            TKTranDanh = new ThongKeSTNguoiChoi()
            {
                TenMap = tenMap,
                STLon = 0,
                STNho = 0,
                STTong = 0,
                LuotST = 0,
                SBan = 0,
                SCount = 0,
            };
            mListTKTranDanh.Add(TKTranDanh);
        }

        bool isPrepareBoss = false;
        if (missionID > 0)
        {
            // Boss mission
            QuanlyManchoi.instance.NumHealOrb = 3;
        }

        if (GateVisualController.instance)
            GateVisualController.instance.SetMission(MissionID, isPrepareBoss);

        //if (battleData != null && MissionID == 0)
        if (
#if UNITY_EDITOR
            TestMission == false &&
#endif
            IsInitedMission == false)
        {
            yield return StartCoroutine(CoStartMission());
        }

        mThoiGianNienThu = 0;
        IsLoadingMission = false;
        procesingDrop = false;
        //if (ScreenBattle.instance && IsThachDauMode && MissionID > 0)
        //{
        //    ScreenBattle.instance.ShowMessage2(
        //        string.Format(Localization.Get("ThachDauLevel"),
        //        ConfigManager.ThachDauBattleCfg.GetTotalLevelFromMission(MissionID)));
        //}
    }

    IEnumerator CoLoadMissionThachDauMode(int newMission, string preCfgSceneName, bool updateBattleData)
    {
        IsLoadingMission = true;

        DisablePlayerUnit();
        for (int i = 1; i < UnityEngine.SceneManagement.SceneManager.sceneCount; ++i)
        {
            var level = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
            yield return UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(level);
        }
        MissionID = newMission;

        var sceneName = ConfigManager.ThachDauBattleCfg.StartLevel;

        if (newMission > 0)
        {
            if (string.IsNullOrEmpty(preCfgSceneName))
            {
                sceneName = GetSceneNameByMissionThachDau(newMission);
            }
            else
            {
                sceneName = preCfgSceneName;
            }

            this.BlockIdx = ConfigManager.ThachDauBattleCfg.GetBlockIdxFromMission(newMission);
        }

        Debug.Log("Load level : " + sceneName);
        ScreenBattle.instance.lbMapName.text = sceneName;
        ScreenBattle.instance.lbMapName.gameObject.SetActive(true);

        if (PlayedScenes.Count == 0 ||
            PlayedScenes[PlayedScenes.Count - 1] != sceneName)
        {
            PlayedScenes.Add(sceneName);
        }

        if (newMission > 0)
        {
            if (GameClient.instance.OfflineMode == false && updateBattleData)
            {
                UpdateProgressThachDauMode();

                //SaveBuffRandomState(battleData != null ? battleData.ID : string.Empty);
            }
        }

        yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(
            sceneName,
            UnityEngine.SceneManagement.LoadSceneMode.Additive);
        var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(1);
        UnityEngine.SceneManagement.SceneManager.SetActiveScene(scene);
        RepostionHeroAndDeTu();
        ScreenBattle.instance.lbMapName.gameObject.SetActive(false);

        //SwapEnvi();

        ScreenBattle.PauseGame(true);

        while (QuanlyManchoi.instance == null)
        {
            yield return null;
        }

        QuanlyManchoi.instance.AddUnit(PlayerUnit);
        ScreenBattle.PauseGame(false);

        EnablePlayerUnit();

        string tenMap = sceneName + "_" + newMission;
        if (TKTranDanh == null || TKTranDanh.TenMap != tenMap)
        {
            TKTranDanh = new ThongKeSTNguoiChoi()
            {
                TenMap = tenMap,
                STLon = 0,
                STNho = 0,
                STTong = 0,
                LuotST = 0,
                SBan = 0,
                SCount = 0,
            };
            mListTKTranDanh.Add(TKTranDanh);
        }

        bool isPrepareBoss = false;
        if (missionID > 0)
        {
            // Boss mission
            QuanlyManchoi.instance.NumHealOrb = 3;
        }

        if (GateVisualController.instance)
            GateVisualController.instance.SetMission(MissionID, isPrepareBoss);

        //if (battleData != null && MissionID == 0)
        if (
#if UNITY_EDITOR
            TestMission == false &&
#endif
            IsInitedMission == false)
        {
            yield return StartCoroutine(CoStartMission());
        }

        IsLoadingMission = false;
        procesingDrop = false;
        if (ScreenBattle.instance && IsThachDauMode && MissionID > 0)
        {
            ScreenBattle.instance.ShowMessage2(
                string.Format(Localization.Get("ThachDauLevel"),
                ConfigManager.ThachDauBattleCfg.GetTotalLevelFromMission(MissionID)));
        }
    }

    IEnumerator CoLoadMissionSanTheMode(int newMission, string preCfgSceneName, bool updateBattleData)
    {
        IsLoadingMission = true;

        DisablePlayerUnit();
        for (int i = 1; i < UnityEngine.SceneManagement.SceneManager.sceneCount; ++i)
        {
            var level = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
            yield return UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(level);
        }
        MissionID = newMission;

        var sceneName = ConfigManager.SanTheCfg.StartLevel;

        if (newMission > 0)
        {
            sceneName = ConfigManager.SanTheCfg.MainLevel;
        }

        Debug.Log("Load level : " + sceneName);
        ScreenBattle.instance.lbMapName.text = sceneName;
        ScreenBattle.instance.lbMapName.gameObject.SetActive(true);

        if (PlayedScenes.Count == 0 ||
            PlayedScenes[PlayedScenes.Count - 1] != sceneName)
        {
            PlayedScenes.Add(sceneName);
        }

        if (newMission > 0)
        {
            if (GameClient.instance.OfflineMode == false && updateBattleData)
            {
                UpdateProgressSanTheMode();

                //SaveBuffRandomState(battleData != null ? battleData.ID : string.Empty);
            }
        }

        yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(
            sceneName,
            UnityEngine.SceneManagement.LoadSceneMode.Additive);
        var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(1);
        UnityEngine.SceneManagement.SceneManager.SetActiveScene(scene);
        RepostionHeroAndDeTu();
        ScreenBattle.instance.lbMapName.gameObject.SetActive(false);

        //SwapEnvi();

        ScreenBattle.PauseGame(true);

        while (QuanlyManchoi.instance == null)
        {
            yield return null;
        }

        if (missionID > 0)
        {
            while (QuanlyManChoiSanThe.instance == null)
            {
                yield return null;
            }
        }

        QuanlyManchoi.instance.AddUnit(PlayerUnit);
        ScreenBattle.PauseGame(false);

        EnablePlayerUnit();

        string tenMap = sceneName + "_" + newMission;
        if (TKTranDanh == null || TKTranDanh.TenMap != tenMap)
        {
            TKTranDanh = new ThongKeSTNguoiChoi()
            {
                TenMap = tenMap,
                STLon = 0,
                STNho = 0,
                STTong = 0,
                LuotST = 0,
                SBan = 0,
                SCount = 0,
            };
            mListTKTranDanh.Add(TKTranDanh);
        }

        bool isPrepareBoss = false;
        if (missionID > 0)
        {
            isPrepareBoss = (missionID == 3);
            //// Boss mission
            if (missionID == ConfigManager.SanTheCfg.LevelAtRound.Length)
            {
                QuanlyManchoi.instance.NumHealOrb = 3;
            }
            else if (isPrepareBoss)
            {
                QuanlyManchoi.instance.NumHealOrb = 3;
            }
            else
            {
                if (Random.Range(0, 100) < 20)
                {
                    QuanlyManchoi.instance.NumHealOrb = 1;
                }
            }

            //QuanlyManchoi.instance.NumHealOrb = 3;

            if (IsLevelClear == false)
            {
                if (missionID < 4)
                {
                    ScreenBattle.instance.ShowMessage2(
                        string.Format(Localization.Get("SanTheLevel"), MissionID));
                }
                else
                {
                    ScreenBattle.instance.ShowMessage2(Localization.Get("SanTheFinalBoss"));
                }

                if (ConfigManager.SanTheCfg.DelaySpawnFirstRound > 0)
                {
                    Hiker.HikerUtils.DoAction(this,
                        () =>
                        {
                            QuanlyManChoiSanThe.instance.SpawnRound(missionID);
                        },
                        ConfigManager.SanTheCfg.DelaySpawnFirstRound);
                }
                else
                {
                    QuanlyManChoiSanThe.instance.SpawnRound(missionID);
                }
            }
            else
            {
                ScreenBattle.instance.ShowMessage2(
                    string.Format(Localization.Get("SanTheLevelCleared"), MissionID));
            }
        }

        if (GateVisualController.instance)
            GateVisualController.instance.SetMission(MissionID, isPrepareBoss);

        //if (battleData != null && MissionID == 0)
        if (
#if UNITY_EDITOR
            TestMission == false &&
#endif
            IsInitedMission == false)
        {
            yield return StartCoroutine(CoStartMission());
        }

        IsLoadingMission = false;
        procesingDrop = false;
        //if (ScreenBattle.instance && IsLeoThapMode && MissionID > 0)
        //{
        //    var idx = (MissionID - 1) % ConfigManager.LeoThapBattleCfg.BlockLevel.Length;
        //    if (ConfigManager.LeoThapBattleCfg.BlockLevel[idx] > 0)
        //    {
        //        ScreenBattle.instance.ShowMessage2(
        //            string.Format(Localization.Get("LeoThapLevel"),
        //            ConfigManager.LeoThapBattleCfg.GetTotalLevelFromMission(MissionID)));
        //    }

        //    if (LeoThapMod == 2 && PlayerUnit.GetMaCaRongModifier())
        //    {
        //        PlayerUnit.GetMaCaRongModifier().MaCaRongCurse();
        //    }
        //    else if (LeoThapMod == 5 && PlayerUnit.GetInfernalModifier())
        //    {
        //        // roi ca o bai quai' va` bai BOSS
        //        if (ConfigManager.LeoThapBattleCfg.BlockLevel[idx] > 0
        //            //&& (idx + 1) < ConfigManager.LeoThapBattleCfg.BlockLevel.Length
        //            )
        //        {
        //            PlayerUnit.GetInfernalModifier().InfernalSkill();
        //        }
        //    }
        //}
    }

    //void GoToNextSanTheRound()
    //{
    //    if (MissionID < 4)
    //    {
    //        UpdateBattleProgress();
    //        StartCoroutine(CoProcessDrop(false));

    //        Hiker.HikerUtils.DoAction(this, () =>
    //        {
    //            missionID++;

    //            QuanlyManChoiSanThe.instance.SpawnRound(missionID);
    //            UpdateBattleProgress();
    //        }, 3f);
    //    }
    //    else if (QuanlyManchoi.instance)
    //    {
    //        QuanlyManchoi.instance.OnLevelClear();
    //    }
    //}

    IEnumerator CoStartMission()
    {
        if (PlayerUnit.UnitName == "IronMan")
        {
            if (SoLuotDungKyNang == 0)
            {
                PopupChonDeTuIR.Create();
                yield return new WaitUntil(() => PopupChonDeTuIR.instance == null);
            }
        }
        InitMissionStart();
    }

    void InitMissionStart()
    {
        //SaveBuffRandomState(battleData != null ? battleData.ID : string.Empty);

        PlayerExp = battleData == null ? 0 : battleData.Exp;
        PlayerGold = battleData == null ? 0 : battleData.Gold;
        PlayerLvl = 1;
        var lastLevelExp = QuanlyNguoichoi.instance.gameConfig.GetTotalExpAtLevel(PlayerLvl - 1);
        var nextLevelExp = QuanlyNguoichoi.instance.gameConfig.GetTotalExpAtLevel(PlayerLvl);
        while (PlayerExp >= nextLevelExp && PlayerLvl < QuanlyNguoichoi.instance.gameConfig.LevelExp.Length)
        {
            PlayerLvl++;
            lastLevelExp = QuanlyNguoichoi.instance.gameConfig.GetTotalExpAtLevel(PlayerLvl - 1);
            nextLevelExp = QuanlyNguoichoi.instance.gameConfig.GetTotalExpAtLevel(PlayerLvl);
        }

        ScreenBattle.instance.UpdatePlayerExp();
        ScreenBattle.instance.UpdatePlayerGold();

#if DEBUG
        Debug.Log("Init Exp " + PlayerExp);
        Debug.Log("Init Level " + PlayerLvl);
#endif

        //playerLoot.Clear();

        //if (battleData != null && battleData.Loot != null)
        //{
        //    foreach (var l in battleData.Loot)
        //    {
        //        playerLoot.Add(l.Key, l.Value);
        //    }
        //}

        if (MissionID == 0 && CurDungeonId == 0)
        {
            if (IsLevelClear == false)
            {
                if (battleData != null &&
                    battleData.ListMods != null)
                {
                    int startSkill = 0;
                    var skills = battleData.ListMods.FindAll(e => e.Stat == EStatType.GETSKILL);
                    int blessingPlus = 0;
                    foreach (var s in battleData.ListMods)
                    {
                        if (s.Stat == EStatType.GETSKILL)
                        {
                            var buff = (BuffType)System.Enum.Parse(typeof(BuffType), s.Target);
                            if (s.Val > 0)
                            {
                                IncreaseGioiHanBuff((int)(s.Val));
                                for (int i = 0; i < s.Val; i++)
                                {
                                    GetBuff(buff, false);
                                }
                            }
                            else
                            {
                                IncreaseGioiHanBuff(1);
                                GetBuff(buff, false);
                            }
                        }
                        else if (s.Stat == EStatType.STARTSKILL)
                        {
                            startSkill = Mathf.Max((int)s.Val, startSkill);
                        }
                        else if (s.Stat == EStatType.DETU)
                        {
                            QuanlyNguoichoi.Instance.ThuPhucDeTu(s.Target);
                        }
                        else if (s.Stat == EStatType.BLESSINGPLUS)
                        {
                            var blessVal = (int)Mathf.Round((float)s.Val);
                            if (blessVal > 0 && blessingPlus < blessVal)
                            {
                                blessingPlus = blessVal;
                            }
                        }
                        //else if (s.Stat == EStatType.EXP)
                        //{
                        //    playerExpBuff += (int)s.Val;
                        //}
                    }

                    if (startSkill > 0)
                    {
                        IncreaseGioiHanBuff(1);

                        if (blessingPlus > 0)
                        {
                            PopupRollingBuff.Create(blessingPlus);
                        }
                        else
                        {
                            PopupRollingBuff.Create();
                        }
                    }
                }
            }

            SpawnNPCLeoThap();
            SpawnBossSanThe();
            SpawnExpNienThu();
        }

        IsInitedMission = true;
    }

    void SpawnBossSanThe()
    {
        if (IsSanTheMode)
        {
            var npc = Instantiate(Resources.Load<GameObject>("LevelDesign/CardHuntBoss"));
            npc.transform.position = Vector3.forward * 10f;
            npc.transform.eulerAngles = Vector3.up * 180f;
            npc.name = "CardHuntBoss";

            var visualParent = npc.transform.Find("BossVisual");
            if (visualParent == null)
            {
                visualParent = npc.transform;
            }

            var bossName = battleData.SanThe.SpawnInfo.Wave4[0];
            var bossPrefab = Resources.Load<DonViChienDau>("AIPrefabs/" + bossName);
            bossPrefab.gameObject.SetActive(false);
            var bossUnit = Instantiate(bossPrefab, visualParent);
            bossPrefab.gameObject.SetActive(true);
            bossUnit.transform.localPosition = Vector3.zero;
            bossUnit.transform.localRotation = Quaternion.identity;
            if (bossUnit.visualGO == null)
            {
                var unitVisual = bossUnit.gameObject.GetComponentInChildren<Animator>();
                if (unitVisual != null)
                {
                    bossUnit.visualGO = unitVisual.gameObject;
                }
            }
            if (bossUnit.visualGO != null)
            {
                bossUnit.visualGO.transform.SetParent(visualParent);
            }
        }
    }
    void SpawnExpSanThe()
    {
        if (IsSanTheMode)
        {
            var npc = Instantiate(Resources.Load<GameObject>("LevelDesign/CardHuntExp"));
            npc.transform.position = Vector3.forward * 15f;
            npc.transform.eulerAngles = Vector3.up * 180f;
            npc.name = "CardHuntExp";
        }
    }

    void SpawnExpNienThu()
    {
        if (IsNienThuMode)
        {
            var npc = Instantiate(Resources.Load<GameObject>("LevelDesign/CardHuntExp"));
            npc.transform.position = Vector3.forward * 15f;
            npc.transform.eulerAngles = Vector3.up * 180f;
            npc.name = "NienThuExp";
        }
    }

    void SpawnNPCLeoThap()
    {
        if (IsLeoThapMode)
        {
            switch (LeoThapMod)
            {
                case 1: // khong lo
                    {
                        var npc = Instantiate(Resources.Load<GameObject>("LevelDesign/KhongLoModifier"));
                        npc.transform.position = Vector3.forward * 15f;
                        npc.transform.eulerAngles = Vector3.up * 180f;
                        npc.name = "KhongLoModifier";
                    }
                    break;
                case 2: // macarong
                    {
                        var npc = Instantiate(Resources.Load<GameObject>("LevelDesign/MaCaRongModifier"));
                        npc.transform.position = Vector3.forward * 15f;
                        npc.transform.eulerAngles = Vector3.up * 180f;
                        npc.name = "MaCaRongModifier";
                    }
                    break;
                case 3: // popo
                    {
                        var npc = Instantiate(Resources.Load<GameObject>("LevelDesign/PopoModifier"));
                        npc.transform.position = Vector3.forward * 15f;
                        npc.transform.eulerAngles = Vector3.up * 180f;
                        npc.name = "PopoModifier";
                    }
                    break;
                case 4: // flash
                    {
                        var npc = Instantiate(Resources.Load<GameObject>("LevelDesign/FlashModifier"));
                        npc.transform.position = Vector3.forward * 15f;
                        npc.transform.eulerAngles = Vector3.up * 180f;
                        npc.name = "FlashModifier";
                    }
                    break;
                case 5: // infernal
                    {
                        var npc = Instantiate(Resources.Load<GameObject>("LevelDesign/InfernalModifier"));
                        npc.transform.position = Vector3.forward * 15f;
                        npc.transform.eulerAngles = Vector3.up * 180f;
                        npc.name = "InfernalModifier";
                    }
                    break;
                case 6: // deathrattle
                    {
                        var npc = Instantiate(Resources.Load<GameObject>("LevelDesign/DeathrattleModifier"));
                        npc.transform.position = Vector3.forward * 15f;
                        npc.transform.eulerAngles = Vector3.up * 180f;
                        npc.name = "DeathrattleModifier";
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public void GoToNextMission()
    {
        if (IsNormalMode)
        {
            MissionID++;
            var curLevelNodeCfg = nodeMapConfig.points[CurDungeonId].GetComponent<NodeLevelConfig>();
            if (MissionID >= curLevelNodeCfg.Levels.Length)
            {
                PopupDungeonSelect.Create(ChapterIndex, CurDungeonId, CurrentPath);
            }
            else
            {
                LoadMission(MissionID);
            }
        }
        else if (IsFarmMode)
        {
            LoadMissionFarmMode(MissionID + 1);
        }
        else if (IsLeoThapMode)
        {
            if (missionID > 0 &&
                ConfigManager.LeoThapBattleCfg.IsBossMission(missionID - 1, out bool prepare))
            {
                mEnviName = string.Empty; // clear oldenvi to swap new random
            }
            LoadMissionLeoThap(MissionID + 1);
        }
        else if (IsThachDauMode)
        {
            LoadMissionThachDau(MissionID + 1);
        }
        else if (IsNienThuMode)
        {
            LoadMissionNienThu(MissionID + 1);
        }
        else if (IsSanTheMode)
        {
            //if (missionID == 0)
            {
                LoadMissionSanThe(MissionID + 1);
            }
        }
    }

    public void OnPlayerExit()
    {
        if (IsMissionEndGame())
        {
            if (GameClient.instance && GameClient.instance.OfflineMode == false)
            {
                StartCoroutine(CoEndGame(true));
            }
        }
        else
        {
            if (PlayerUnit && PlayerUnit.GetCurHP() > 0)
            {
                GoToNextMission();
            }
        }

        if( ScreenBattle.instance.lblWave!=null)
            ScreenBattle.instance.lblWave.text = string.Empty;

    }

    public long GetTotalGoldHaving()
    {
        var playerLoot = QuanlyNguoichoi.Instance.GetPlayerLoot();
        var totalGold = QuanlyNguoichoi.Instance.PlayerGold + playerLoot.GetGold();
        return totalGold;
    }
    public int GetResourceRate()
    {
        var rate = 1;
        if (QuanlyNguoichoi.Instance.FarmMode > 1)
        {
            rate = QuanlyNguoichoi.Instance.FarmMode;
        }
        return rate;
    }
    public long GetTotalGoldByRate()
    {
        var rate = GetResourceRate();
        var totalGold = GetTotalGoldHaving();
        return totalGold * rate;
    }
    void TruGold(long goldMatDi)
    {
        if (goldMatDi > 0)
        {
            if (PlayerGold >= goldMatDi)
            {
                PlayerGold -= goldMatDi;
            }
            else
            {
                var goldLoot = playerLoot[CardReward.GOLD_CARD];
                goldMatDi -= PlayerGold;
                PlayerGold = 0;
                if (goldMatDi > 0)
                {
                    if (goldMatDi <= goldLoot)
                    {
                        playerLoot[CardReward.GOLD_CARD] -= (int)goldMatDi;
                    }
                    else
                    {
                        playerLoot[CardReward.GOLD_CARD] = 0;
                    }
                }
            }
        }
    }

    public void OnPlayerGetTrap(GameObject visual)
    {
        int rand = 0;
        long gold = 0;
        long hp = 0;
        BuffType buff = BuffType.HEAL;
        int[] TrapRate = ConfigManager.GetTrapRate();

        if (chapterIndex <= 1) // chap 2 tro xuong chi lay gold
        {
            TrapRate[0] = 0; // power rate = 0
            TrapRate[2] = 0; // hp rate = 0
        }
        else if (chapterIndex == 2) // chap 3,4 -> chi lay gold hoac hp
        {
            TrapRate[0] = 0;
        }
        else if (chapterIndex == 3)
        {
            TrapRate[0] = 0;
        }

        int totalRate = 0;
        for (int i = 0; i < TrapRate.Length; ++i)
        {
            totalRate += TrapRate[i];
        }
        rand = Random.Range(0, totalRate);

        if (rand < TrapRate[0])
        {
            // layPowerUp
            var listRandomBuff = new List<int>();

            for (int i = ListBuffs.Count - 1; i >= 0; i--)
            {
                var t = ListBuffs[i];

                int buff_level = QuanlyNguoichoi.Instance.PlayerUnit.GetBuffCount(t.Type);

                if (t.Type != BuffType.HEAL && t.Type != BuffType.LIFE && buff_level <=1)
                {
                    listRandomBuff.Add(i);
                }
            }

            if (listRandomBuff.Count > 0)
            {
                var r = Random.Range(0, listRandomBuff.Count);
                var buffIndex = listRandomBuff[r];
                BuffStat buffStat = new BuffStat();
                if (RemoveBuff(buffIndex, ref buffStat))
                {
                    buff = buffStat.Type;
                }
                PopupTrap.Create(visual, buff, hp, gold);
                return;
            }
            else
            {
                rand = Random.Range(0, TrapRate[1] + TrapRate[2]);
            }
        }
        else
        {
            rand -= TrapRate[0];
        }

        if (rand < TrapRate[1])
        {
            // lay gold
            var totalGold = GetTotalGoldHaving();
            gold = totalGold * ConfigManager.GetTrapGoldPercent() / 100;
            if (gold > 0 || TrapRate[2] <= 0) // khi khong co rate cho hp -> chi lay gold
            {
                TruGold(gold);
                // force visual display lay gold = 100 khi gold = 0
                PopupTrap.Create(visual, buff, hp, (gold <= 0 ? 100 : gold) * GetResourceRate());
                return;
            }
        }

        // lay hp
        hp = PlayerUnit.GetCurHP() * ConfigManager.GetTrapHPPercent() / 100;
        PlayerUnit.TakeDamage(hp, false);
        PopupTrap.Create(visual, buff, hp, gold);
        return;
        //PopupTrap.Create(visual, buff, hp, gold);
        //LevelController.instance.OnLevelClear();
    }

    public void OnPlayerGetExpMaster(GameObject visual)
    {
        long kinhNghiem = ConfigManager.GetKinhNghiemSuPhuTruyenCong();
        var popup = PopupSuPhuTruyenCong.Create(visual, kinhNghiem);
        popup.onPopupClosed.AddListener(() =>
            {
                int lastLevel = PlayerLvl;
                var buff = GetTotalExpBuff();
                if (buff > 0)
                {
                    kinhNghiem += (kinhNghiem * buff / 100);
                }
                GetExp(kinhNghiem);

                StartCoroutine(CoProcessGetExp(lastLevel, () => {
                    if (QuanlyManchoi.instance)
                    {
                        QuanlyManchoi.instance.OnLevelClear();
                    }
                }));
            });
    }

    public void OnPlayerGetCardHuntBoss(GameObject visual)
    {
        if (visual)
        {
            visual.gameObject.SetActive(false);
        }

        SpawnExpSanThe();
    }

    public void OnPlayerGetCardHuntExp(GameObject visual)
    {
        if (visual)
        {
            visual.gameObject.SetActive(false);
        }
        int lastLevel = PlayerLvl;
        int round = missionID;
        int level = ConfigManager.SanTheCfg.LevelAtRound[missionID];
        var expToLevel = gameConfig.GetTotalExpAtLevel(level);
        var remainExp = expToLevel - playerExp;
        GetExp(remainExp, false);
        StartCoroutine(CoProcessGetExp(lastLevel, null));
    }

    public void OnPlayerGetNienThuExp(GameObject visual)
    {
        if (visual)
        {
            visual.gameObject.SetActive(false);
        }
        int lastLevel = PlayerLvl;
        int round = missionID;
        int level = ConfigManager.TetEventCfg.LevelStart;
        var expToLevel = gameConfig.GetTotalExpAtLevel(level);
        var remainExp = expToLevel - playerExp;
        GetExp(remainExp, false);
        StartCoroutine(CoProcessGetExp(lastLevel, null));
    }

    IEnumerator CoProcessGetExp(int lastLevel, System.Action finishAction)
    {
        //mGioiHanBuff = PlayerLvl - lastLevel;
        while (lastLevel < PlayerLvl)
        {
            if (QuanlyNguoichoi.instance.PlayerUnit.IsAlive() ||
                QuanlyNguoichoi.instance.PlayerUnit.Life > 0)
            {
                PopupRollingBuff.Create();
                while (PopupRollingBuff.instance) // PAUSE TO WAIT USER PICK BUFF BEFORE CONTINUE
                {
                    yield return new WaitForSecondsRealtime(0.15f);
                }
            }
            lastLevel++;
        }
        finishAction?.Invoke();
    }

    public void OnPlayerGetRangeMinion(GameObject visual)
    {
        //PopupGreedyGoblin.Create(visual);
        PopupRangeMinion.Create(visual);
    }

    public void OnPlayerGetStatMaster(GameObject visual)
    {
        long baseATK = 100;
        if (IsLeoThapMode)
        {
            var cfg = ConfigManager.LeoThapCfg.GetLeoThapRankCfg(LeoThapRankIdx);
            baseATK = cfg.BaseStat;
            if (baseATK < 300)
            {
                baseATK = 300;
            }
        }

        long atkBonus = ConfigManager.GetATKBonusStatMaster(baseATK);
        long hpBonus = ConfigManager.GetHPBonusStatMaster(baseATK);
        PopupThienThanChucPhuc.Create(visual, atkBonus, hpBonus);
    }

    public void OnPlayerGetSecretShop(GameObject visual)
    {
        if (BattleEventController.instance &&
            BattleEventController.instance.Response != null &&
            BattleEventController.instance.Response.listItems != null)
        {
            PopupBattleSecretShop.Create(visual, BattleEventController.instance.Response.listItems);
        }

        //LevelController.instance.OnLevelClear();
    }

    public void OnPlayerGetBlackSmith(GameObject visual)
    {
        PopupBlackSmith.Create(visual);
        //LevelController.instance.OnLevelClear();
    }

    public void OnPlayerGetDevil(GameObject visual)
    {
        PopupDevilBuff.Create(visual);
        //LevelController.instance.OnLevelClear();
    }

    public void OnPlayerGetAngel(GameObject visual)
    {
        IncreaseGioiHanBuff(1);
        PopupAngelBuff.Create(visual);
        //LevelController.instance.OnLevelClear();
    }

    public void OnPlayerGetGreedyGoblin(GameObject visual)
    {
        PopupGreedyGoblin.Create(visual);
        //LevelController.instance.OnLevelClear();
    }

    public void OnPlayerGetFightingNPC(FightingNPC npcVisual)
    {
        PopupFightingNPC.Create(npcVisual, npcVisual.gameObject.name);
        //LevelController.instance.OnLevelClear();
    }

    public void OnPlayerGetChest(BattleChest visual)
    {
        visual.Open();

        //var battleChest = Instantiate(Resources.Load<BattleChest>("Prefabs/Chests/BattleChest"));
        //battleChest.transform.position = PlayerUnit.transform.position + Vector3.forward * 2f;
        //battleChest.transform.forward = Vector3.back;
        bool showChestReward = false;
        CardReward chestCfg = null;
        if (IsNormalMode)
        {
            var curNodeLevelCfg = GetCurrentNodeLevelConfig();
            var chapcfg = ConfigManager.chapterConfigs[ChapterIndex];
            var ruongCfg = curNodeLevelCfg.RuongConfig;

            if (string.IsNullOrEmpty(ruongCfg) == false &&
                chapcfg.Chests.ContainsKey(ruongCfg))
            {
                bool openRuong = false;
                if (GameClient.instance.OfflineMode == false)
                {
                    var chapData = GameClient.instance.UInfo.ListChapters.Find(e => e.ChapIdx == ChapterIndex);
                    if (chapData != null && chapData.RuongOpened.Contains(ruongCfg))
                    {
                        openRuong = true;
                    }
                }

                if (openRuong == false)
                {
                    if (listRuongDaMo.Contains(ruongCfg) == false)
                    {
                        listRuongDaMo.Add(ruongCfg);
                        chestCfg = chapcfg.Chests[ruongCfg];
                        showChestReward = true;
                    }
                }
            }
        }

        if (showChestReward && chestCfg != null)
        {
            Hiker.HikerUtils.DoAction(this, () =>
            {
                QuanlyManchoi.instance.OnLevelClear();
                Hiker.GUI.Shootero.PopupOpenChest_new.Create(chestCfg);
            }, 0.5f, true);
        }
        else
        {
            QuanlyManchoi.instance.OnLevelClear();
        }
    }

    public DonViChienDau ThuPhucDeTu(string deTu)
    {
        var prefab = Resources.Load<DonViChienDau>("AIPrefabs/" + deTu);

        if (prefab == null || prefab.TeamID != QuanlyManchoi.PlayerTeam)
        {
            return null;
        }

        if (prefab.GetComponent<PlayerMovement>() != null)
        {
            return null;
        }

        var mainScene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(0);
        var vec2 = Random.insideUnitCircle * 3f;
        var pos = PlayerUnit.transform.position + new Vector3(vec2.x, 0, vec2.y);

        var newDeTu = Instantiate(prefab, pos, Quaternion.identity);
        if (newDeTu == null) return null;

        UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(newDeTu.gameObject, mainScene);

        listDeTu.Add(newDeTu);

        if (mDeTu.ContainsKey(deTu))
        {
            mDeTu[deTu] += 1;
        }
        else
        {
            mDeTu.Add(deTu, 1);
        }

        return newDeTu;
    }


    public void NecromancerSpawnSkeleton(int life_time)
    {
        var prefab = Resources.Load<DonViChienDau>("AIPrefabs/NecromancerSkeleton");

        if (prefab == null || prefab.TeamID != QuanlyManchoi.PlayerTeam)
        {
            return;
        }

        if (prefab.GetComponent<PlayerMovement>() != null)
        {
            return;
        }

        //var mainScene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(0);
        var vec2 = Random.insideUnitCircle * 10f;
        var pos = PlayerUnit.transform.position + new Vector3(vec2.x, 0, vec2.y);
        
        var newDeTu = Instantiate(prefab, pos, Quaternion.identity);

        TimerToDestroy _timerToDestroy = newDeTu.gameObject.GetComponent<TimerToDestroy>();
        if (_timerToDestroy != null)
            _timerToDestroy.TimeToDestroy = life_time;

    }

    int mGioiHanBuff = 0;
    public void IncreaseGioiHanBuff(int g)
    {
        mGioiHanBuff += g;
    }

    public void GetBuff(BuffType type, bool showMessage = true)
    {
        if (mGioiHanBuff > 0)
        {
            mGioiHanBuff--;
        }
        else
        {
            return;
        }

        var buff = GetBuffStatByType(type);
        int curCount = PlayerUnit.GetBuffCount(type);
        if (buff.MaxCount > 0 && curCount >= buff.MaxCount)
        {
            return;
        }

        GetBuff(buff);
        if (showMessage)
        {
            Hiker.HikerUtils.DoAction(this,
            () => { ScreenBattle.instance.ShowMessage(Localization.Get(string.Format("Buff_{0}_Desc", type.ToString()))); },
            0.15f);
        }
    }

    public void GetBuff(BuffStat buff)
    {
        ListBuffs.Add(buff);

        if (buff.Type == BuffType.HEAL)
        {
            GameObject heal_eff = Instantiate<GameObject>(Resources.Load<GameObject>("Particle/PowerUp/HEAL"),PlayerUnit.transform);
            heal_eff.transform.localPosition = Vector3.zero;
            GameObject.Destroy(heal_eff, 3);
        }

        PlayerUnit.GetBuff(buff);

        UpdatePowerUpObj(buff);
    }

    void UpdatePowerUpObj(BuffStat buff)
    {
        var t = buff.Type;
        var count = PlayerUnit.GetBuffCount(t);
        if (count <= 0)
        {
            ClearPowerupObj(t);
        }
        else
        {
            switch (t)
            {
                case BuffType.SHIELD:
                    {
                        if (PlayerShield == null)
                        {
                            PlayerShield = Instantiate<TwinShield>(Resources.Load<TwinShield>("TwinShield"));
                            DontDestroyOnLoad(PlayerShield.gameObject);
                        }

                        if (PlayerShield)
                        {
                            PlayerShield.gameObject.SetActive(true);
                        }
                        break;
                    }
                case BuffType.FLY:
                    {
                        if (EffectFly == null)
                        {
                            EffectFly = Instantiate<GameObject>(Resources.Load<GameObject>("Particle/PowerUp/FLY"));
                            DontDestroyOnLoad(EffectFly);
                        }

                        EffectFly.gameObject.SetActive(true);
                        EffectFly.transform.position = PlayerUnit.transform.position;
                        TargetFollower follower = EffectFly.GetComponent<TargetFollower>();
                        follower.target = PlayerUnit.transform;
                        follower.InitPos();
                        break;
                    }
                case BuffType.LINKEN:
                    {
                        if (EffectLinken == null)
                        {
                            EffectLinken = Instantiate<GameObject>(Resources.Load<GameObject>("Particle/PowerUp/LINKEN"));
                            DontDestroyOnLoad(EffectLinken);
                        }

                        EffectLinken.gameObject.SetActive(true);
                        EffectLinken.transform.position = PlayerUnit.transform.position;
                        TargetFollower follower = EffectLinken.GetComponent<TargetFollower>();
                        follower.target = PlayerUnit.visualGO.transform;
                        follower.InitPos();
                        break;
                    }

                case BuffType.ELECTRIC_FIELD:
                    {
                        int level = ListBuffs.FindAll(e => e.Type == BuffType.ELECTRIC_FIELD).Count;
                        var field = PlayerUnit.GetComponent<ElectricField>();
                        var range = (field != null ? field.GetRange() : (float)buff.Params[(level - 1) * 3]) / buff.Params[0];
                        string effObjName = "ELECTRIC_FIELD_" + Mathf.RoundToInt(range * 100);
                        if (EffectElectricField && EffectElectricField.name != effObjName)
                        {
                            EffectElectricField.gameObject.SetActive(false);
                            Destroy(EffectElectricField.gameObject);
                            EffectElectricField = null;
                        }

                        if (EffectElectricField == null)
                        {
                            EffectElectricField = Instantiate<GameObject>(Resources.Load<GameObject>("Particle/PowerUp/ELECTRIC_FIELD"));
                            DontDestroyOnLoad(EffectElectricField);

                            EffectElectricField.name = effObjName;
                            var multi = EffectElectricField.AddMissingComponent<UnityStandardAssets.Effects.ParticleSystemMultiplier>();

                            multi.multiplier = range;
                        }

                        EffectElectricField.gameObject.SetActive(true);
                        EffectElectricField.transform.position = PlayerUnit.transform.position;

                        TargetFollower follower = EffectElectricField.GetComponent<TargetFollower>();
                        follower.target = PlayerUnit.transform;
                        follower.InitPos();
                        break;
                    }
                case BuffType.FLAME_FIELD:
                    {
                        int level = ListBuffs.FindAll(e => e.Type == BuffType.FLAME_FIELD).Count;
                        var field = PlayerUnit.GetComponent<FlameField>();
                        var range = (field != null ? field.GetRange() : (float)buff.Params[(level - 1) * 3]) / buff.Params[0];
                        string effObjName = "FLAME_FIELD_" + Mathf.RoundToInt(range * 100);
                        if (EffectFlameField && EffectFlameField.name != effObjName)
                        {
                            EffectFlameField.gameObject.SetActive(false);
                            Destroy(EffectFlameField.gameObject);
                            EffectFlameField = null;
                        }

                        if (EffectFlameField == null)
                        {
                            EffectFlameField = Instantiate<GameObject>(Resources.Load<GameObject>("Particle/PowerUp/FLAME_FIELD"));
                            DontDestroyOnLoad(EffectFlameField);
                            EffectFlameField.name = effObjName;

                            var multi = EffectFlameField.AddMissingComponent<UnityStandardAssets.Effects.ParticleSystemMultiplier>();

                            multi.multiplier = range;
                        }

                        EffectFlameField.gameObject.SetActive(true);
                        EffectFlameField.transform.position = PlayerUnit.transform.position;

                        TargetFollower follower = EffectFlameField.GetComponent<TargetFollower>();
                        follower.target = PlayerUnit.transform;
                        follower.InitPos();
                        break;
                    }

                case BuffType.FROZEN_FIELD:
                    {
                        int level = ListBuffs.FindAll(e => e.Type == BuffType.FROZEN_FIELD).Count;
                        var field = PlayerUnit.GetComponent<FrozenField>();
                        var range = (field != null ? field.GetRange() : (float)buff.Params[(level - 1) * 3]) / buff.Params[0];
                        string effObjName = "FROZEN_FIELD_" + Mathf.RoundToInt(range * 100);

                        if (EffectFrozenField && EffectFrozenField.name != effObjName)
                        {
                            EffectFrozenField.gameObject.SetActive(false);
                            Destroy(EffectFrozenField.gameObject);
                            EffectFrozenField = null;
                        }

                        if (EffectFrozenField == null)
                        {
                            EffectFrozenField = Instantiate<GameObject>(Resources.Load<GameObject>("Particle/PowerUp/FROZEN_FIELD"));
                            DontDestroyOnLoad(EffectFrozenField);
                            EffectFrozenField.name = effObjName;

                            var multi = EffectFrozenField.AddMissingComponent<UnityStandardAssets.Effects.ParticleSystemMultiplier>();

                            multi.multiplier = range;
                        }

                        EffectFrozenField.gameObject.SetActive(true);
                        EffectFrozenField.transform.position = PlayerUnit.transform.position;
                        TargetFollower follower = EffectFrozenField.GetComponent<TargetFollower>();
                        follower.target = PlayerUnit.transform;
                        follower.InitPos();
                        break;
                    }

                default:
                    break;
            }
        }
    }

    public void ClearAllPowerUpObj()
    {
        for(int i=0; i < (int)(BuffType.END_OF_BUFF_TYPE);i++ )
        {
            ClearPowerupObj((BuffType)(i));
        }
    }

    public void ClearPowerupObj(BuffType t)
    {
        switch(t)
        {
            case BuffType.SHIELD:
                {
                    if(PlayerShield)
                        PlayerShield.gameObject.SetActive(false);
                    break;
                }
            case BuffType.FLY:
                {
                    if (EffectFly)
                        EffectFly.gameObject.SetActive(false);
                    break;
                }
            case BuffType.LINKEN:
                {
                    if (EffectLinken)
                        EffectLinken.gameObject.SetActive(false);
                    break;
                }

             case BuffType.ELECTRIC_FIELD:
                {
                    if (EffectElectricField)
                        EffectElectricField.gameObject.SetActive(false);
                    break;
                }
            case BuffType.FLAME_FIELD:
                {
                    if (EffectFlameField)
                        EffectFlameField.gameObject.SetActive(false);
                    break;
                }

            case BuffType.FROZEN_FIELD:
                {
                    if (EffectFrozenField)
                        EffectFrozenField.gameObject.SetActive(false);
                    break;
                }

            default:
                break;
        }
    }

    public bool RemoveBuff(int index, ref BuffStat buffStat)
    {
        if (index < 0 || index >= ListBuffs.Count) return false;
        buffStat = ListBuffs[index];
        ListBuffs.RemoveAt(index);
        PlayerUnit.RemoveBuff(buffStat.Type);

        UpdatePowerUpObj(buffStat);

        return true;
    }

    public BuffStat GetBuffStatByType(BuffType t)
    {
        for (int i = 0; i < gameConfig.Buffs.Length; ++i)
        {
            if (gameConfig.Buffs[i].Type == t)
            {
                return gameConfig.Buffs[i];
            }
        }

        return gameConfig.Buffs[0];
    }

    IEnumerator CoProcessPlayerDie()
    {
        if (CanRevive)
        {
            yield return new WaitForSeconds(2f);
            LuotHS++;
            QuanlyNguoichoi.Instance.CanRevive = false;
            if (battleData != null)
            {
                QuanlyNguoichoi.Instance.CanRevive = battleData.CanRevive(LuotHS);
            }
            PopupRequestHoiSinh.Create();
        }
        else
        {
            if (GameClient.instance && GameClient.instance.OfflineMode == false)
            {
                yield return StartCoroutine(CoEndGame(false));
            }
        }
    }

    public void OnPlayerRetreat()
    {
        CanRevive = false;
        QuanlyNguoichoi.Instance.PlayerUnit.Life = 0;
        var dmg = long.MaxValue;// QuanlyNguoichoi.Instance.PlayerUnit.GetMaxHP() + 1;
        QuanlyNguoichoi.Instance.PlayerUnit.TakeDamage(dmg,
            false, // display crit
            null, // source unit
            false, // play hit sound
            false, // show hud
            true, // xuyen bat tu
            true, // xuyen linken
            false, // thong ke,
            false,
            false,
            false,
            true
            );
    }

    public void OnPlayerDied()
    {
        UpdatePlayerStat();
        StartCoroutine(CoProcessPlayerDie());
    }

    public bool LastWinOrLose { get; private set; }
    public bool IsCompleteMap { get; private set; }
    public bool IsEndGame { get; private set; }

    public IEnumerator CoBackToMain(bool isRestart = false)
    {
        if (QuanlyManchoi.instance)
            QuanlyManchoi.instance.OnEndGame();

        for (int i = 1; i < UnityEngine.SceneManagement.SceneManager.sceneCount; ++i)
        {
            var level = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
            yield return UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(level);
        }

        GUIManager.Instance.SetScreen("Main");
        //if (PlayerUnit)
        //{
        //    PlayerUnit.gameObject.SetActive(false);
        //}

        if (IsThachDauMode && isRestart == false)
        {
            GameClient.instance.RequestGetThachDauData();
        }
        else if (IsLeoThapMode && isRestart == false)
        {
            GameClient.instance.RequestGetLeoThapData();
        }

        QuanlyNguoichoi.instance.gameObject.SetActive(false);
    }

    public IEnumerator CoCleanUp()
    {
        yield return StartCoroutine(CoProcessDrop(true));
        ClearAllPowerUpObj();
        PopupRollingBuff.Dismiss();

        if (PlayerUnit && PlayerUnit.UnitName == "BeastMaster")
        {
            var tk = PlayerUnit.GetComponent<TuyetKyNhanVat>();
            var chimTk = tk.GetTuyetKy(0) as BeastMasterSkill;
            if (chimTk != null)
            {
                chimTk.CleanUpObj();
            }
        }
        DisablePlayerUnit();
    }

    IEnumerator CoEndGame(bool winOrLose)
    {
        if (IsEndGame) yield break;
        IsEndGame = true;
        
        yield return StartCoroutine(CoCleanUp());

        LastWinOrLose = winOrLose;
        IsCompleteMap = false;
        if (IsNormalMode)
        {
            if (LastWinOrLose &&
               nodeMapConfig.points[CurDungeonId].connections.Count == 0 &&
               GetCurrentNodeLevelConfig().LevelType == ENodeType.Boss)
            {
                IsCompleteMap = true;
            }
        }
        else if (IsFarmMode)
        {
#if DEBUG
            Debug.Log(string.Format("END Chapter {0} Level {1}", chapterIndex, missionID));
#endif
            AnalyticsManager.LogEvent("END_FARM_MODE",
                new AnalyticsParameter("MISSION", missionID));
        }
        else if (IsLeoThapMode)
        {
            AnalyticsManager.LogEvent("END_LEOTHAP_MODE",
                new AnalyticsParameter("MISSION", missionID));
        }
        else if (IsThachDauMode)
        {
            AnalyticsManager.LogEvent("END_THACHDAU_MODE",
                new AnalyticsParameter("MISSION", missionID));
        }

        if (GameClient.instance.OfflineMode == false)
        {
            UpdateBattleRequest req = new UpdateBattleRequest();
            req.BattleID = battleData.ID;
            req.PlayerStat = PlayerUnit.GetCurStat().ToUnitStatWrapper();
            req.listBuffs = new BuffType[ListBuffs.Count];
            req.CurMissionID = missionID;
            req.CurDungeonID = curDungeonId;
            //req.PassedMission = PassedMission;
            for (int i = 0; i < ListBuffs.Count; ++i)
            {
                req.listBuffs[i] = ListBuffs[i].Type;
            }

            req.Mat = playerMat;
            req.Gold = PlayerGold;
            req.Exp = PlayerExp;
            
            req.Path = CurrentPath.ToArray(); //winOrLose ? MissionID : MissionID - 1;
            req.NodePath = ListNodePaths.ToArray();

            if (IsNormalMode)
            {
                var mapCfg = GetMapDungeonConfig();
                req.HPCfg = mapCfg.HPBuff;
                req.DMGCfg = mapCfg.DMG;
            }

            req.playedScene = playedScenes.ToArray();
            req.IsEnd = true;
            req.IsCompleteMap = IsCompleteMap;
            req.Loot = new Dictionary<string, int>();
            req.EventIdx = battleData.BattleEventIndex;
            foreach (var t in playerLoot)
            {
                req.Loot.Add(t.Key, t.Value);
            }
            req.RuongDaMo = new List<string>();
            foreach (var t in listRuongDaMo)
            {
                req.RuongDaMo.Add(t);
            }
            req.BossHunted = new List<string>();
            foreach (var t in listBossHunted)
            {
                req.BossHunted.Add(t);
            }
            req.IsGreedy = IsGreedy;
            req.LuotHS = LuotHS;
            req.HeroSK = SoLuotDungKyNang;
            req.LuotRR = LuotReroll;
            req.TKSatThuong = mTKSatThuong;
            req.TKTranDanh = mListTKTranDanh;
#if SERVER_1_3
            req.TKSung = TKSung;
#endif
            req.BattleMode = BattleMode;
            if (req.BattleMode == 2)
            {
                req.BattleTime = (long)System.Math.Floor(LastLvlBattleTime);
            }
            else if (req.BattleMode == 3)
            {
                req.KilledUnit = new string[killedEnemy.Count];
                killedEnemy.CopyTo(req.KilledUnit);
            }
            else if (req.BattleMode == 4)
            {
                req.BattleTime = (long)System.Math.Floor(LastLvlBattleTime);
            }
            else if (req.BattleMode == 5)
            {
                req.BattleTime = (long)System.Math.Floor(mThoiGianNienThu);
                req.Gold = NienThuBattlePointSumarize;
            }
            else
            {
                req.BattleTime = (long)System.Math.Floor(mBattleTime);
            }

            req.CCode = GetCCode();

            req.CurLvlClear = IsLevelClear;

            GameClient.instance.RequestEndBattle(req);
            //float totalSeconds = 0;
            //while (req.IsEnd)
            //{
            //    yield return new WaitForSecondsRealtime(0.1f);
            //    totalSeconds += 0.1f;
            //    if (totalSeconds > 5f)
            //    {
            //        break;
            //    }
            //}

            //PlayerPrefs.SetInt("EndChapter2Rating", 0);
            //IsCompleteMap = true;
            if (IsNormalMode && IsCompleteMap &&
                (PlayerPrefs.GetInt("EndChapter2Rating", 0) == 0) &&
                GameClient.instance.UInfo.GetCurrentChapter() >= 1)
            {
                PlayerPrefs.SetInt("EndChapter2Rating", 1);
                PopupRating.Create(() => {
                    PopupEndBattle.Create(LastWinOrLose);
                });
            }
            else {
                yield return new WaitForSecondsRealtime(winOrLose ? 1f : 2f);
                PopupEndBattle.Create(LastWinOrLose);
            }
        }
#if UNITY_EDITOR
        else
        {
            Application.Quit();
        }
#endif
        battleData = null;
    }

    public void GetRuongDaMo(List<string> listRuong)
    {
        listRuong.Clear();

        foreach (var t in listRuongDaMo)
        {
            listRuong.Add(t);
        }
    }

    public bool IsMoRuong(string ruong)
    {
        return listRuongDaMo.Contains(ruong);
    }

    #region HackDetectorCallBack

    public int GetCCode()
    {
        return mCCode;
    }

    void ResetCheckHack()
    {
        mCCode = 0;
    }

    Int32 mCCode = 0;

    [GUIDelegate]
    public void OnBtnBatGianLanBoNho()
    {
        mCCode |= 0b0001;
    }

    [GUIDelegate]
    public void OnBtnBatGianLanTocDo()
    {
        mCCode |= 0b0010;
    }

    [GUIDelegate]
    public void OnBtnBatGianLanThoiGian()
    {
        mCCode |= 0b0100;
    }

    [GUIDelegate]
    public void OnBtnBatGianLanPhycis()
    {
        mCCode |= 0b1000;
    }

    #endregion
}
