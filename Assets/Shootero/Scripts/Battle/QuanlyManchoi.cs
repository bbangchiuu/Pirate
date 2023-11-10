using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hiker.GUI.Shootero;
#if ANTICHEAT
using Int64 = CodeStage.AntiCheat.ObscuredTypes.ObscuredLong;
using Int32 = CodeStage.AntiCheat.ObscuredTypes.ObscuredInt;
using Float = CodeStage.AntiCheat.ObscuredTypes.ObscuredFloat;
using Bool = CodeStage.AntiCheat.ObscuredTypes.ObscuredBool;
using OString = CodeStage.AntiCheat.ObscuredTypes.ObscuredString;

#else
using Int64 = System.Int64;
using Int32 = System.Int32;
using Float = System.Single;
using Bool = System.Boolean;
#endif

public class QuanlyManchoi : MonoBehaviour
{
    public static QuanlyManchoi instance;

    public const int EnemyTeam = 2;
    public const int PlayerTeam = 1;
    public const float UnitCellSize = 2f;

    #region SerializeField
    [SerializeField]
    Int32 numHealOrb = 0;

    [SerializeField]
    Bool isEventLevel = false;

    public DonViChienDau PlayerUnit { get; set; }
    #endregion

    bool initedHealOrb = false;
    Int32 mEnemyLevel = 1;
    public int EnemyLevel { get { return mEnemyLevel; } private set { mEnemyLevel = value; } }
    Int64 mTotalExp = 100;
    public long TotalExp { get { return mTotalExp; } set { mTotalExp = value; } }

    Int64 remainEXP;
    Int64 levelGold = 100;
    public long LevelGold { get { return levelGold; } set { levelGold = value; } }

    Int64 levelMat = 1;
    public long LevelMat { get { return levelMat; } set { levelMat = value; } }
    public int NumHealOrb { get { return numHealOrb; } set { numHealOrb = value; } }

    public bool IsEventLevel { get { return isEventLevel; } set { isEventLevel = value; } }

    List<DonViChienDau> mAllies = new List<DonViChienDau>();
    List<DonViChienDau> mListEnemy = new List<DonViChienDau>();

    private WaveController m_WaveController = null;
    public WaveController WaveCtrler { get { return m_WaveController; } }

    public void SetWaveController(WaveController o)
    {
        m_WaveController = o;
    }

    private void Awake()
    {
        instance = this;
        EnemyLevel = 1; // khong con level quai dung` buff chi so trong Chapter Config

        ObjectPoolManager.Init();
        //if (ObjectPoolManager.instance == null)
        //{
        //    ObjectPoolManager.instance = gameObject.AddMissingComponent<ObjectPoolManager>();
        //}
    }

    private void Start()
    {
#if ANTICHEAT
        if (CodeStage.AntiCheat.Detectors.ObscuredCheatingDetector.Instance &&
            CodeStage.AntiCheat.Detectors.ObscuredCheatingDetector.Instance.IsRunning == false
            )
        {
            CodeStage.AntiCheat.Detectors.ObscuredCheatingDetector.StartDetection();
        }
        if (CodeStage.AntiCheat.Detectors.SpeedHackDetector.Instance &&
            CodeStage.AntiCheat.Detectors.SpeedHackDetector.Instance.IsRunning == false
            )
        {
            CodeStage.AntiCheat.Detectors.SpeedHackDetector.StartDetection();
        }
        if (CodeStage.AntiCheat.Detectors.TimeCheatingDetector.Instance &&
            CodeStage.AntiCheat.Detectors.TimeCheatingDetector.Instance.IsRunning == false
            )
        {
            CodeStage.AntiCheat.Detectors.TimeCheatingDetector.StartDetection();
        }
        if (CodeStage.AntiCheat.Detectors.WallHackDetector.Instance &&
            CodeStage.AntiCheat.Detectors.WallHackDetector.Instance.IsRunning == false
            )
        {
            CodeStage.AntiCheat.Detectors.WallHackDetector.StartDetection();
        }
#endif
#if UNITY_EDITOR
        // for debug only
        //Time.timeScale = 0.1f;
#endif
        MeshRenderer[] renderer_list = this.GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < renderer_list.Length; i++)
        {
            renderer_list[i].enabled = false;
        }

        if (isEventLevel == false)
        {
            remainEXP = TotalExp;
            long missionGold = 0;
            //if (PlayerManager.Instance && PlayerManager.Instance.currentChap != null)
            //{
            //    missionGold = PlayerManager.Instance.currentChap.TotalGold / PlayerManager.Instance.currentChap.GetTotalCombatMissions();
            //}
            if (QuanlyNguoichoi.Instance)
            {
                if (QuanlyNguoichoi.Instance.IsFarmMode)
                {
                    if (QuanlyNguoichoi.Instance.farmConfig != null)
                    {
                        missionGold = ConfigManager.GetTotalGoldFarmMode(QuanlyNguoichoi.Instance.ChapterIndex) /
                            QuanlyNguoichoi.Instance.farmConfig.GetTotalCombatMissions();

                        //var isBoss = PlayerManager.Instance.farmConfig.IsBossMission(PlayerManager.Instance.MissionID, out bool prepareBoss);
                        //if (isBoss)
                        //{
                        //    remainEXP = ConfigManager.GetExpByBossNode();
                        //}
                        if (WaveCtrler != null || WaveController.instance != null)
                        {
                            remainEXP = ConfigManager.GetExpByCreepNode();
                        }

                        var totalMat = 0;
                        var remainMat = 0;
                        if (QuanlyNguoichoi.Instance)
                        {
                            totalMat = ConfigManager.GetTotalMaterialFarmMode(QuanlyNguoichoi.Instance.ChapterIndex);
                            remainMat = Mathf.Max(totalMat - QuanlyNguoichoi.Instance.PlayerMaterial, 0);
                        }
                        if (remainMat > 0)
                        {
                            var tileDrop = ConfigManager.GetTileDropMaterial();
                            if (Random.Range(0, 100) < tileDrop)
                            {
                                var minRand = Mathf.Min(remainMat, ConfigManager.GetDropMaterialMin());
                                var maxRand = Mathf.Min(remainMat + 1, ConfigManager.GetDropMaterialMax() + 1);
                                LevelMat = Random.Range(minRand, maxRand);
                            }
                            else
                            {
                                LevelMat = 0;
                            }
                        }
                        else
                        {
                            LevelMat = 0;
                        }
                    }
                }
                else if (QuanlyNguoichoi.Instance.IsLeoThapMode)
                {
                    int missionIdx = QuanlyNguoichoi.Instance.MissionID - 1;

                    if (missionIdx < 0)
                    {
                        missionGold = 0;
                        LevelMat = 0;
                        remainEXP = 0;
                    }
                    else
                    {
                        var rankCfg = ConfigManager.LeoThapCfg.GetLeoThapRankCfg(
                            QuanlyNguoichoi.Instance.LeoThapRankIdx);

                        var numBlock = ConfigManager.LeoThapBattleCfg.BlockArray.Length;
                        int blockGetMaxPercent = ConfigManager.LeoThapBattleCfg.LevelNhanFullPT;
                        if (blockGetMaxPercent <= 0) blockGetMaxPercent = 40;

                        int blockGetMax = blockGetMaxPercent * numBlock / 100;
                        var blockIdx = ConfigManager.LeoThapBattleCfg.GetBlockIdxFromMission(missionIdx);
                        int countGoldMission = 0;
                        int countHeroStoneMission = 0;
                        foreach (var s in ConfigManager.LeoThapBattleCfg.Block)
                        {
                            if (s != "Event")
                            {
                                countGoldMission++;
                            }
                            if (s == "Boss")
                            {
                                countHeroStoneMission++;
                            }
                        }

                        int levelGetMax = countGoldMission * blockGetMax;
                        int levelGetStoneMax = countHeroStoneMission * blockGetMax;

                        var goldPerMission = rankCfg.Gold / levelGetMax;

                        if (ConfigManager.LeoThapBattleCfg.IsBossMission(missionIdx, out bool isPrepareBoss))
                        {
                            LevelMat = rankCfg.HeroStone / levelGetStoneMax;
                        }
                        else
                        {
                            levelMat = 0;
                        }
                        missionGold = goldPerMission * Random.Range(90, 110) / 100;
                        int missionInBlockIdx = missionIdx % ConfigManager.LeoThapBattleCfg.BlockLevelExp.Length;
                        remainEXP = ConfigManager.LeoThapBattleCfg.BlockLevelExp[missionInBlockIdx];
                    }
                }
                else if (QuanlyNguoichoi.Instance.IsThachDauMode)
                {
                    int missionIdx = QuanlyNguoichoi.Instance.MissionID - 1;

                    if (missionIdx < 0)
                    {
                        missionGold = 0;
                        LevelMat = 0;
                        remainEXP = 0;
                    }
                    else
                    {
                        LevelMat = 0;
                        var blocks = ConfigManager.ThachDauBattleCfg.BlockArrayByRank["Rank" + (QuanlyNguoichoi.Instance.ThachDauRankIdx + 1)];
                        var goldPerMission = ConfigManager.GetTotalGoldThachDau(QuanlyNguoichoi.Instance.ChapterIndex) / blocks.Length;
                            
                        missionGold = goldPerMission * Random.Range(90, 110) / 100;
                        
                        remainEXP = ConfigManager.ThachDauBattleCfg.LevelExp;
                    }
                }
                else if (QuanlyNguoichoi.Instance.IsNormalMode)
                {
                    var mapCfg = QuanlyNguoichoi.Instance.GetMapDungeonConfig();
                    var nodeCfg = QuanlyNguoichoi.Instance.GetCurrentNodeLevelConfig();

                    if (mapCfg != null)
                    {
                        missionGold = ConfigManager.GetTotalGoldCampaign(
                            QuanlyNguoichoi.Instance.ChapterIndex) / mapCfg.MaxPathLength;
                        if (missionGold > 0)
                        {
                            if (nodeCfg != null)
                            {
                                missionGold = missionGold / nodeCfg.Levels.Length;
                            }

                            //TODO: pseudo random here
                            missionGold += Random.Range(-10, 10) * missionGold / 100;
                        }
                    }

                    if (nodeCfg != null && nodeCfg.LevelType == ENodeType.Boss)
                    {
                        remainEXP = ConfigManager.GetExpByBossNode() / nodeCfg.Levels.Length;
                    }
                    else if (nodeCfg != null && nodeCfg.LevelType != ENodeType.Start && nodeCfg.LevelType != ENodeType.Angel)
                    {
                        remainEXP = ConfigManager.GetExpByCreepNode() / nodeCfg.Levels.Length;
                    }
                    else
                    {
                        remainEXP = 0;
                    }
#if DEBUG
                    if (QuanlyNguoichoi.Instance.TestMission)
                        remainEXP = 400;
#endif
                    var totalMat = 0;
                    var remainMat = 0;
                    if (QuanlyNguoichoi.Instance)
                    {
                        totalMat = ConfigManager.GetTotalMaterialCampaign(QuanlyNguoichoi.Instance.ChapterIndex); ;
                        remainMat = Mathf.Max(totalMat - QuanlyNguoichoi.Instance.PlayerMaterial, 0);
                    }
                    if (remainMat > 0)
                    {
                        var tileDrop = ConfigManager.GetTileDropMaterial();
                        if (Random.Range(0, 100) < tileDrop)
                        {
                            var minRand = Mathf.Min(remainMat, ConfigManager.GetDropMaterialMin());
                            var maxRand = Mathf.Min(remainMat + 1, ConfigManager.GetDropMaterialMax() + 1);
                            LevelMat = Random.Range(minRand, maxRand);
                        }
                        else
                        {
                            LevelMat = 0;
                        }
                    }
                    else
                    {
                        LevelMat = 0;
                    }
                }
                else if (QuanlyNguoichoi.Instance.IsSanTheMode)
                {
                    int numRound = ConfigManager.SanTheCfg.LevelAtRound.Length;
                    if (numRound > 0)
                    {
                        missionGold = ConfigManager.GetTotalGoldSanThe(QuanlyNguoichoi.Instance.ChapterIndex) / numRound;
                        if (missionGold > 0)
                        {
                            missionGold += Random.Range(-10, 10) * missionGold / 100;
                        }
                    }
                    var curLevel = QuanlyNguoichoi.Instance.PlayerLvl;
                    int missionId = QuanlyNguoichoi.Instance.MissionID;
                    var levelAtRound = 0;
                    if (numRound > 0 && missionId < numRound)
                    {
                        levelAtRound = ConfigManager.SanTheCfg.LevelAtRound[missionId];
                    }
                    if (levelAtRound > 0)
                    {
                        remainEXP = QuanlyNguoichoi.Instance.gameConfig.GetTotalExpAtLevel(curLevel - 1 + levelAtRound) - QuanlyNguoichoi.Instance.PlayerExp;
                    }
                    else
                    {
                        remainEXP = 0;
                    }
                    LevelMat = 0;
                }
                else if (QuanlyNguoichoi.Instance.IsNienThuMode)
                {
                    remainEXP = 0;
                    LevelGold = 0;
                    LevelMat = 0;
                }
            }

            //LevelGold = missionGold * TotalExp / 100;
            LevelGold = missionGold;
        }
        else
        {
            remainEXP = 0;
            LevelGold = 0;
            LevelMat = 0;
        }

        if (remainEXP > 0 && QuanlyNguoichoi.Instance)
        {
            int totalBuff = QuanlyNguoichoi.Instance.GetTotalExpBuff();
            var playerUnit = PlayerUnit;
            if (PlayerUnit == null && QuanlyNguoichoi.Instance.PlayerUnit != null)
            {
                playerUnit = QuanlyNguoichoi.Instance.PlayerUnit;
                PlayerUnit = playerUnit;
            }

            var expBuff = remainEXP * totalBuff / 100;
            if (expBuff <= 0 && totalBuff > 0)
            {
                expBuff = 1;
            }
            remainEXP += expBuff;
        }

        if (QuanlyNguoichoi.Instance &&
            QuanlyNguoichoi.Instance.IsLevelClear &&
            QuanlyNguoichoi.Instance.battleData != null &&
            QuanlyNguoichoi.Instance.battleData.CurLvlClear)
        {
            OnLevelClear();
        }
    }

    public void AddUnit(DonViChienDau unit)
    {
        if (unit.TeamID == EnemyTeam)
        {
            if (mListEnemy.Contains(unit) == false)
            {
                mListEnemy.Add(unit);
            }
        }
        else if (unit.TeamID == PlayerTeam)
        {
            if (PlayerUnit == null && unit.GetComponent<PlayerMovement>() != null)
            {
                PlayerUnit = unit;
            }
            else if (unit != PlayerUnit)
            {
                if (mAllies.Contains(unit) == false)
                    mAllies.Add(unit);
            }
        }
    }

    public static DonViChienDau FindClosestEnemy(Vector3 centerPos, float maxRange = 0, List<DonViChienDau> exceptions = null)
    {
        if (instance == null) return null;
        if (instance.PlayerUnit == null) return null;
        if (instance.mListEnemy.Count == 0) return null;

        DonViChienDau result = null;
        float closestDis = 0;
        for (int i = instance.mListEnemy.Count - 1; i >= 0; --i)
        {
            var enemy = instance.mListEnemy[i];
            if (exceptions != null && exceptions.Contains(enemy))
            {
                continue;
            }

            if (enemy.IsAlive() &&
                enemy.IsCanTarget())
            {
                var sqDis = Vector3.SqrMagnitude(enemy.transform.position - centerPos);
                if (maxRange > 0 && sqDis > maxRange * maxRange)
                {
                    continue;
                }

                if (result == null || sqDis < closestDis)
                {
                    result = enemy;
                    closestDis = sqDis;
                }
            }
        }

        return result;
    }

    public static List<DonViChienDau> FindEnemiesInRange(Vector3 centerPos, float maxRange, List<DonViChienDau> exceptions = null, bool includeCancelTarget = false)
    {
        if (instance == null) return null;
        if (instance.PlayerUnit == null) return null;
        if (instance.mListEnemy.Count == 0) return null;

        List<DonViChienDau> result = Hiker.Util.ListPool<DonViChienDau>.Claim();
        //float closestDis = 0;
        for (int i = instance.mListEnemy.Count - 1; i >= 0; --i)
        {
            var enemy = instance.mListEnemy[i];
            if (exceptions != null && exceptions.Contains(enemy))
            {
                continue;
            }

            if (enemy.IsAlive() &&
                (enemy.IsCanTarget() || includeCancelTarget) )
            {
                var sqDis = Vector3.SqrMagnitude(enemy.transform.position - centerPos);
                if (maxRange > 0 && sqDis > maxRange * maxRange)
                {
                    continue;
                }

                result.Add(enemy);
            }
        }

        return result;
    }

    public static DonViChienDau FindClosestEnemy()
    {
        if (instance == null) return null;
        if (instance.PlayerUnit == null) return null;
        if (instance.mListEnemy.Count == 0) return null;

        DonViChienDau result = null;
        float closestDis = 0;
        for (int i = instance.mListEnemy.Count - 1; i >= 0; --i)
        {
            var enemy = instance.mListEnemy[i];
            if (enemy.IsAlive() &&
                enemy.IsCanTarget())
            {
                var sqDis = Vector3.SqrMagnitude(enemy.transform.position - instance.PlayerUnit.transform.position);
                if (result == null || sqDis < closestDis)
                {
                    result = enemy;
                    closestDis = sqDis;
                }
            }
        }

        return result;
    }

    public static DonViChienDau FindClosestEnemyCanFire(Vector3 playerPos)
    {
        if (instance == null) return null;
        if (instance.PlayerUnit == null) return null;
        if (instance.mListEnemy.Count == 0) return null;

        DonViChienDau result = null;
        float closestDis = 0;
        float closestDisCanFire = 0;
        DonViChienDau closestUnits = null;
        for (int i = instance.mListEnemy.Count - 1; i >= 0; --i)
        {
            var enemy = instance.mListEnemy[i];
            if (enemy.IsAlive() &&
                enemy.IsCanTarget())
            {
                var enemyPos = enemy.transform.position;
                var dir = enemyPos - playerPos;
                var sqDis = dir.magnitude;
                if (closestUnits == null || sqDis < closestDis)
                {
                    closestUnits = enemy;
                    closestDis = sqDis;
                }

                //if (Physics.Raycast(playerPos + Vector3.up * 1f, dir, sqDis, LayerMask.GetMask("Default", "Obstacle")))
                //{
                    
                //}
                //else
                {
                    if (result == null || sqDis < closestDisCanFire)
                    {
                        // gioi han tam danh cua user
                        if (sqDis <= instance.PlayerUnit.GetCurStat().ATK_RANGE)
                        {
                            result = enemy;
                            closestDisCanFire = sqDis;
                        }
                    }
                }
            }
        }

        if (result == null)
        {
            if (closestDis <= instance.PlayerUnit.GetCurStat().ATK_RANGE)
            {
                result = closestUnits;
            }
        }

        return result;
    }

    public void DestroyEnemy(DonViChienDau unit)
    {
        unit.HideVisual();
        if (this == null)
        {
            unit.gameObject.SetActive(false);
        }
        else
        {
            Hiker.HikerUtils.DoAction(this, () => {
                if (unit && unit.gameObject)
                    unit.gameObject.SetActive(false);
            },
            1f,
            true);
        }
        
    }

    void OnEnemyDied(DonViChienDau unit)
    {
        if (PlayerUnit && PlayerUnit.GetKyNang() != null)
        {
            PlayerUnit.GetKyNang().OnEnemyDie(unit);
        }

        if (QuanlyNguoichoi.Instance)
        {
            QuanlyNguoichoi.Instance.OnEnemyDie(unit);
        }

        //if (IsEventLevel == false && unit.DontHaveDrop == false)
        //{
        //    int unit_drop_exp_count = 0;

        //    for (int e = 0; e < mListEnemy.Count; e++)
        //    {
        //        if (mListEnemy[e] != null && mListEnemy[e].DontHaveDrop == false)
        //            unit_drop_exp_count++;
        //    }

        //    if (remainEXP > 0 && unit_drop_exp_count > 0)
        //    {
        //        long exp = remainEXP / unit_drop_exp_count;

        //        if (QuanlyManchoi.instance.m_WaveController != null
        //            && QuanlyManchoi.instance.m_WaveController.Waves.Length > 0)
        //        {
        //            long waveEXP = remainEXP / (QuanlyManchoi.instance.m_WaveController.Waves.Length - QuanlyManchoi.instance.m_WaveController.CurrentWave + 1);
        //            waveEXP = waveEXP / unit_drop_exp_count;
        //            if (QuanlyManchoi.instance.m_WaveController.CurrentWave == QuanlyManchoi.instance.m_WaveController.Waves.Length
        //                && unit_drop_exp_count == 1)
        //            {
        //                exp = remainEXP;
        //            }
        //            else
        //            {
        //                exp = waveEXP;
        //            }
        //        }

        //        remainEXP -= exp;

        //        if (remainEXP < 0)
        //            remainEXP = 0;

        //        var drop = new UnitExpDrop() { DropType = UnitDropType.Exp, EXP = exp };
        //        QuanlyNguoichoi.Instance.HaveADrop(drop);

        //        for (int i = 0; i < drop.Visuals.Length; ++i) // roi exp ra dat
        //        {
        //            drop.Visuals[i].transform.position = unit.transform.position;
        //            drop.Visuals[i].SetActive(true);
        //            var dropMv = drop.Visuals[i].GetComponent<UnitDropMovement>();
        //            dropMv.maxHeight = 4f;
        //            dropMv.RandomRadius = 3f;
        //            dropMv.Activate(12, unit.transform.position);
        //        }
        //    }

        //    #region Xu ly HealthOrb
        //    InitHealthOrb();

        //    while (unit.HaveHealOrb > 0)
        //    {
        //        var heal = new HealOrb()
        //        {
        //            DropType = UnitDropType.HealOrb,
        //            HPRegen = (int)(ConfigManager.GetBaseHPHealOrb() * PlayerManager.Instance.PlayerUnit.GetMaxHP() / 100)
        //            HPRegen = (int)(ConfigManager.GetBaseHPHealOrb())
        //        };
        //        var battleData = QuanlyNguoichoi.Instance.battleData;
        //        if (battleData != null &&
        //            battleData.ListMods != null)
        //        {
        //            float buff = 0;
        //            float scale = 0;
        //            foreach (var m in battleData.ListMods)
        //            {
        //                if (m.Stat == EStatType.HEALORB)
        //                {
        //                    if (m.Mod == EStatModType.ADD)
        //                    {
        //                        heal.HPRegen += (int)m.Val;
        //                    }
        //                    else if (m.Mod == EStatModType.MUL)
        //                    {
        //                        scale += (float)m.Val;
        //                    }
        //                    else if (m.Mod == EStatModType.BUFF)
        //                    {
        //                        buff += (float)m.Val;
        //                    }
        //                }
        //            }

        //            if (scale > 0)
        //            {
        //                heal.HPRegen = (int)(heal.HPRegen * scale / 100);
        //            }
        //            if (buff > 0)
        //            {
        //                heal.HPRegen += (int)(heal.HPRegen * buff / 100);
        //            }
        //        }

        //        QuanlyNguoichoi.Instance.HaveADrop(heal);
        //        heal.Visuals[0].transform.position = unit.transform.position;
        //        heal.Visuals[0].SetActive(true);
        //        unit.HaveHealOrb--;

        //        var dropMv = heal.Visuals[0].AddMissingComponent<UnitDropMovement>();
        //        dropMv.maxHeight = 4f;
        //        dropMv.Activate(12, unit.transform.position);
        //        Hiker.HikerUtils.DoAction(this,
        //            () =>
        //            {
        //                if (QuanlyNguoichoi.Instance != null && heal != null)
        //                    QuanlyNguoichoi.Instance.ConsumDrop(heal);
        //            },
        //            0.3f);
        //    }

        //    if (LevelMat > 0 && unit_drop_exp_count > 0)
        //    {
        //        var rate = 100f / unit_drop_exp_count;
        //        if (Random.Range(0, 100) < rate)
        //        {
        //            string matName = ConfigManager.RandomMaterial();
        //            if (QuanlyNguoichoi.Instance.IsLeoThapMode)
        //            {
        //                matName = "M_HeroStone";
        //            }
        //            var drop = new UnitMaterialDrop() { DropType = UnitDropType.Material };
        //            drop.Name = matName;
        //            drop.Count = LevelMat;
        //            LevelMat = 0;
        //            QuanlyNguoichoi.Instance.PlayerMaterial += (int)drop.Count;

        //            QuanlyNguoichoi.Instance.HaveADrop(drop);
        //            drop.Visuals[0].transform.position = unit.transform.position;
        //            drop.Visuals[0].SetActive(true);

        //            var dropMv = drop.Visuals[0].AddMissingComponent<UnitDropMovement>();
        //            dropMv.maxHeight = 4f;
        //            dropMv.Activate(12, unit.transform.position);
        //            Hiker.HikerUtils.DoAction(this, () => PlayerManager.Instance.ConsumDrop(drop), 0.3f);
        //        }
        //    }
        //    #endregion
        //}
        //else if (BattleEventController.instance)
        //{
        //    if (BattleEventController.instance.NumUserGem > 0)
        //    {
        //        var drop = new UnitResDrop() { DropType = UnitDropType.Gem };

        //        var gem = BattleEventController.instance.NumUserGem / mListEnemy.Count;
        //        if (gem < 0) gem = 1;
        //        drop.Count = gem;
        //        BattleEventController.instance.NumUserGem -= gem;

        //        QuanlyNguoichoi.Instance.HaveADrop(drop);

        //        drop.Visuals[0].transform.position = unit.transform.position;
        //        drop.Visuals[0].SetActive(true);

        //        var dropMv = drop.Visuals[0].AddMissingComponent<UnitDropMovement>();
        //        dropMv.maxHeight = 4f;
        //        dropMv.RandomRadius = 2;
        //        dropMv.Activate(12, unit.transform.position);
        //        Hiker.HikerUtils.DoAction(this, () => PlayerManager.Instance.ConsumDrop(drop), 0.3f, true);
        //    }

        //    if (BattleEventController.instance.NumUserGold > 0)
        //    {
        //        var drop = new UnitResDrop() { DropType = UnitDropType.Gold };
        //        var gold = BattleEventController.instance.NumUserGold / mListEnemy.Count;

        //        if (gold < 0)
        //        {
        //            gold = 1;
        //        }
        //        drop.Count = gold;
        //        BattleEventController.instance.NumUserGold -= gold;
        //        QuanlyNguoichoi.Instance.HaveADrop(drop);

        //        for (int i = 0; i < drop.Visuals.Length; ++i) // roi exp ra dat
        //        {
        //            drop.Visuals[i].transform.position = unit.transform.position;
        //            drop.Visuals[i].SetActive(true);
        //            var dropMv = drop.Visuals[i].AddMissingComponent<UnitDropMovement>();
        //            dropMv.maxHeight = 3f;
        //            dropMv.RandomRadius = 2;
        //            dropMv.Activate(12, unit.transform.position);
        //        }

        //        drop.Visuals[0].transform.position = unit.transform.position;
        //        drop.Visuals[0].SetActive(true);

        //        var dropMv = drop.Visuals[0].AddMissingComponent<UnitDropMovement>();
        //        dropMv.maxHeight = 4f;
        //        dropMv.RandomRadius = 2;
        //        dropMv.Activate(12, unit.transform.position);
        //        Hiker.HikerUtils.DoAction(this, () => PlayerManager.Instance.ConsumDrop(drop), 0.3f, true);
        //    }

        //    if (BattleEventController.instance.MaterialData != null &&
        //        BattleEventController.instance.MaterialData.Count > 0)
        //    {
        //        var rate = BattleEventController.instance.MaterialData.Count * 100f / mListEnemy.Count;
        //        while (rate > 0)
        //        {
        //            if (Random.Range(0, 100) < rate)
        //            {
        //                var index = BattleEventController.instance.MaterialData.Count - 1;
        //                var cur = BattleEventController.instance.MaterialData[index];

        //                var drop = new UnitMaterialDrop() { DropType = UnitDropType.Material };
        //                drop.Name = cur.Key;
        //                drop.Count = cur.Value;
        //                BattleEventController.instance.MaterialData.RemoveAt(index);

        //                QuanlyNguoichoi.Instance.HaveADrop(drop);

        //                drop.Visuals[0].transform.position = unit.transform.position;
        //                drop.Visuals[0].SetActive(true);

        //                var dropMv = drop.Visuals[0].AddMissingComponent<UnitDropMovement>();
        //                dropMv.maxHeight = 4f;
        //                dropMv.RandomRadius = 2;
        //                dropMv.Activate(12, unit.transform.position);
        //                Hiker.HikerUtils.DoAction(this, () => PlayerManager.Instance.ConsumDrop(drop), 0.3f, true);
        //            }
        //            rate -= 100f;
        //        }
        //    }
        //}

        DestroyEnemy(unit);

        //Destroy(unit.gameObject, 3f);
        mListEnemy.Remove(unit);
        if (mListEnemy.Count == 0)
        {
            OnLastEnemyDied();
        }
    }

    public static void RemoveEnemyUnit(DonViChienDau unit)
    {
        if (instance)
        {
            instance.mListEnemy.Remove(unit);
        }

        unit.TakeDamage(unit.GetMaxHP(),
            false, // display crit
            null, // source unit
            false, // playHitSound
            false, // showHUD
            true, // xuyen Invuneble
            true, // xuyen linken
            false, // thongke
            false, // isHeadShot
            false, // isSourceUnitBoss
            false, // isSourceUnitAir
            true
            );

        if (instance)
        {
            instance.DestroyEnemy(unit);
        }
    }

    public void OnUnitDie(DonViChienDau unit)
    {
        if (unit != null && unit == PlayerUnit)
        {
            OnPlayerDied();
        }

        if (IsLevelClear()) return;

        if (unit != null && unit.TeamID == EnemyTeam && mListEnemy.Contains(unit))
        {
            OnEnemyDied(unit);
        }
    }

    Coroutine delayLevelClear;
    void OnLastEnemyDied()
    {
        if( m_WaveController==null) // normal level
        {
            //if (QuanlyNguoichoi.Instance && QuanlyNguoichoi.Instance.IsSanTheMode)
            //{
            //    QuanlyNguoichoi.Instance.OnRoundSanTheClear();
            //}
            //else
            {
                if (delayLevelClear != null)
                {
                    StopCoroutine(delayLevelClear);
                }

                delayLevelClear = StartCoroutine(DelayLevelClear());
            }
#if UNITY_EDITOR
            // duongrs test bug player die after kill last units
            //Hiker.HikerUtils.DoAction(this, () => { PlayerUnit.TakeDamage(100000, false, null); }, 0.46f);
#endif
        }
        else // wave level
        {
            bool checkOpenPopup = m_WaveController.CheckHasNextWave();
            if (checkOpenPopup)
            {
                var popup = PopupBattleInventory.Create(QuanLyBattleItem.instance.inventory, true);
                popup.onPopupClosed.AddListener(() =>
                {
                    bool isHasWave = m_WaveController.SpawnCurrentWave();

                    if (!isHasWave)
                    {
                        if (delayLevelClear != null)
                        {
                            StopCoroutine(delayLevelClear);
                        }

                        delayLevelClear = StartCoroutine(DelayLevelClear());
                    }
                });
            }
        }
    }

    public IEnumerator DelayLevelClear()
    {
        yield return new WaitForSeconds(0.5f);

        if (mListEnemy.Count == 0)
        {
            // khi user da chet -> dang cho hoi sinh
            while (PlayerUnit && PlayerUnit.IsAlive() == false)
            {
                yield return null;
            }
            OnLevelClear();
        }
    }

    void OnPlayerDied()
    {
        //if (delayLevelClear != null)
        //{
        //    StopCoroutine(delayLevelClear);
        //}
        QuanlyNguoichoi.Instance.OnPlayerDied();
    }

    void InitHealthOrb()
    {
        if (initedHealOrb || NumHealOrb <= 0) return;

        //List<int> listUnitHaveOrb = new List<int>();

        for (int i = 0; i < NumHealOrb; ++i)
        {
            int rIndex = Random.Range(0, mListEnemy.Count);
            mListEnemy[rIndex].HaveHealOrb++;
            //listUnitHaveOrb.Add(rIndex);
        }

        //for (int i = 0; i < listUnitHaveOrb.Count; ++i)
        //{
        //    var index = listUnitHaveOrb[i];
            
        //}

        initedHealOrb = true;
    }

    public bool IsLevelClear()
    {
        return mListEnemy.Count == 0;
    }

    public void OnLevelClear()
    {
        if (QuanlyNguoichoi.Instance)
        {
            QuanlyNguoichoi.Instance.OnLevelClear();
        }
    }

    public void OnEndGame()
    {
        for (int i = 0; i < mListEnemy.Count; ++i)
        {
            if (mListEnemy[i].IsAlive())
            {
                // clear HUD UI
                if (ScreenBattle.instance) ScreenBattle.instance.OnUnitDied(mListEnemy[i]);
            }
        }
    }

    public string WaterSpritePath = "Chap1/water_c1";
}
