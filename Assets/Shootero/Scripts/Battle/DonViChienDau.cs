using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;
using Hiker.GUI.Shootero;
using Hiker.GUI;
using Debug = UnityEngine.Debug;
#if ANTICHEAT
using Int64 = CodeStage.AntiCheat.ObscuredTypes.ObscuredLong;
using Int32 = CodeStage.AntiCheat.ObscuredTypes.ObscuredInt;
using Float = CodeStage.AntiCheat.ObscuredTypes.ObscuredFloat;
using Bool = CodeStage.AntiCheat.ObscuredTypes.ObscuredBool;
#else
using Int64 = System.Int64;
using Int32 = System.Int32;
using Float = System.Single;
using Bool = System.Boolean;
#endif

public class DonViChienDau : MonoBehaviour
{
    #region Serialable Field
    public SungCls shooter;
    public SungCls shooter2;
    public SungCls shooter3;
    public SungCls shooter4;

    public List<SungCls> shooterList;

    public Vector3 OffsetHUD;
    public GameObject visualGO;
    public string UnitName;

    [SerializeField]
    Int32 teamUnit = QuanlyManchoi.EnemyTeam;

    [SerializeField]
    Bool isNoneTargetUnit = false;
    [SerializeField]
    Bool isBossUnit = false;
    [SerializeField]
    Bool isAirUnit = false;
    [SerializeField]
    Bool isMelee = false;
    [SerializeField]
    Bool doNotApplyPopoModifier = false;
    [SerializeField]
    Bool doNotApplyKhongLoModifier = false;
    [SerializeField]
    Bool doNotApplyDeathrateModifier = false;

    public GameObject SpawnObjOnDie = null;
    public int SpawnObjOnDieCount = 0;

    public bool DontHaveDrop = false;

    public GameObject[] HideWhenDie;

    [HideInInspector]
    public Collider m_Collider = null;

    #endregion

    //public UnitStatData StatData;
    public int TeamID { get { return teamUnit; } set { teamUnit = value; } }
    public bool NoneTargetUnit { get { return isNoneTargetUnit; } set { isNoneTargetUnit = value; } }
    public bool IsBoss { get { return isBossUnit; } set { isBossUnit = value; } }
    public bool IsAir { get { return isAirUnit; } set { isAirUnit = value; } }
    public bool IsMelee { get { return isMelee; } }

    public UnitStat OriginStat { get; private set; }
    Int32 mLevel = 1;
    public int Level { get { return mLevel; } set { mLevel = value; } }
    public Int32 Life { get; set; }

    public Int64 STReceived { get; private set; }

    Float mImmunityAfterTakeDMG;
    public float ImmunityAfterTakeDMG { get { return mImmunityAfterTakeDMG; } set { mImmunityAfterTakeDMG = value; } }

    public SkinnedMeshRenderer[] SkinMeshRenderer { get; set; }
    public MeshRenderer[] MeshRenderer { get; set; }

    UnitStat mCurStat;

    Vector3 bodyScale = Vector3.one;

    public float GetCurDPS()
    {
        return mCurStat.DMG * mCurStat.ATK_SPD;
    }
    public float GetElementDMGRate()
    {
        return 1 + mCurStat.ELE_DMG / 100f;
    }

    Int64 mMaxHP;

    HPBar mCurHPBar;
    public HPBar GetCurHPBar() { return mCurHPBar; }
    public void OverrideCurHPBar(HPBar newHpBar) { mCurHPBar = newHpBar; }
    bool mIsInited = false;

    /// <summary>
    /// Bien nay dung de lam eff ngoai target
    /// </summary>
    float mTimeOutOfTarget = 0;
    public bool IsCanTarget() { return mTimeOutOfTarget <= 0; }

    /// <summary>
    /// Bien nay dung de lam eff khong dinh dan (tru field aoe)
    /// </summary>
    float mKhongDinhDan = 0;
    public bool IsCoTheDinhDan() { return mKhongDinhDan <= 0; }

    public Int32 HaveHealOrb { get; set; }

    public bool IsInvulnerable { get { return mInvulnerableTime > 0; } }
    public DonViChienDau UnitGayDmgGanNhat { get; set; }
    /// <summary>
    /// does not take more damage if value > 0
    /// </summary>
    float mInvulnerableTime = 0;
    /// <summary>
    /// default set if time is larger than current value
    /// </summary>
    /// <param name="time"></param>
    /// <param name="isMinTime"></param>
    public void SetInvulnerableTime(float time, bool isMinTime = true)
    {
        if (isMinTime && mInvulnerableTime < time) // only set if time is larger cur
        {
            mInvulnerableTime = time;
            return;
        }

        mInvulnerableTime = time;
    }

    Float unitTimeScale = 1f;
    public float TimeScale { get { return unitTimeScale; }
        set
        {
            unitTimeScale = value;
            GetComponent<BehaviorDesigner.Runtime.BehaviorTree>().ThinkingTimeScale = value;
            UpdateShooterStat();
            UpdateMovementStat();
        }
    }

    float lastTimeGetImpact = 0;
    public void StartVisualImpact(Vector3 bulletPos)
    {
        if (lastTimeGetImpact <= 0)
        {
            // impact on hit
            Vector3 impact_pos = this.transform.position;

            if (m_Collider != null)
                impact_pos = m_Collider.ClosestPointOnBounds(bulletPos);

            GameObject impact_eff = ObjectPoolManager.Spawn("Particle/Other/MonsterImpact");
            //impact_eff.transform.parent = unit.transform;
            impact_eff.transform.position = impact_pos;
            impact_eff.transform.rotation = this.transform.rotation;

            ObjectPoolManager.instance.AutoUnSpawn(impact_eff, 3);
            lastTimeGetImpact = 1;
        }
    }

    List<ParticleChain> listChains = null;
    public void StartLightningChain(Transform preTarget)
    {
        if (listChains == null) listChains = Hiker.Util.ListPool<ParticleChain>.Claim();

        for (int i = listChains.Count - 1; i >= 0; --i)
        {
            var chain = listChains[i];
            if (chain == null ||
                chain.gameObject.activeInHierarchy == false ||
                chain.GetTarget() == null)
            {
                listChains.RemoveAt(i);
                continue;
            }
            if (chain.GetTarget() == preTarget && chain.ChainTime < 0.7f)
            {
                return;
            }
        }

        GameObject l_chain = ObjectPoolManager.Spawn("Particle/PowerUp/ELECTRIC_EFF_Chain");

        l_chain.transform.parent = transform;
        l_chain.transform.position = transform.position;

        var lightningChain = l_chain.GetComponent<ParticleChain>();
        lightningChain.SetTarget(preTarget);

        listChains.Add(lightningChain);
        ObjectPoolManager.instance.AutoUnSpawn(l_chain, 1f);
    }

    public void OverrideChiSoCrit(int critRate, int critDmg)
    {
        mCurStat.CRIT = critRate;
        mCurStat.CRIT_DMG = critDmg;
        UpdateShooterStat();
    }


    public void OverrideChiSoDam(long dmg)
    {
        mCurStat.DMG = dmg;
        UpdateShooterStat();
    }

    public void OverrideChiSoMau(long curHP, long maxHP)
    {
        mCurStat.HP = curHP;
        mMaxHP = maxHP;
        if (mCurHPBar) { mCurHPBar.UpdateHP(); }
    }

    public TuyetKyNhanVat GetKyNang()
    {
        return mKyNang;
    }

    TuyetKyNhanVat mKyNang;
    Animator[] mAnimators;
    public Animator[] Animators { get { return mAnimators; } }
    SungThayThe mSungThayThe;
    public SungThayThe SungPhu { get { return mSungThayThe; } }

    NavMeshAgent mAgent;
    QuanlyHieuung mStatusEffect;
    GameObject mTargetEff;
    List<ChargeStatusEff> mChargeStatusEffs;

    GameObject runingChargeEff;
    GameObject runingChargeMax;

    Transform lastTargetUnit = null;
    public Transform LastTarget { get { return lastTargetUnit; } }
    public GameObject SetTarget(Transform targetUnit)
    {
        if (targetUnit == lastTargetUnit) return null;

        if (targetUnit != null)
        {
            if (this == QuanlyNguoichoi.Instance.PlayerUnit)
            {
                if (mTargetEff == null)
                {
                    mTargetEff = ObjectPoolManager.Spawn("Particle/Other/TargetEff");
                }

                if (mTargetEff)
                {
                    mTargetEff.transform.SetParent(null);
                    mTargetEff.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                    mTargetEff.transform.SetParent(targetUnit.transform);
                    var customPivot = targetUnit.Find("pivot");
                    if (customPivot)
                    {
                        mTargetEff.transform.position = customPivot.position;
                    }
                    else
                    {
                        mTargetEff.transform.localPosition = Vector3.zero;
                    }
                    mTargetEff.transform.localPosition += Vector3.up * 0.1f;

                    mTargetEff.transform.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            if (this == QuanlyNguoichoi.Instance.PlayerUnit)
            {
                if (mTargetEff)
                {
                    mTargetEff.transform.SetParent(null);
                    ObjectPoolManager.Unspawn(mTargetEff);
                    //mTargetEff.gameObject.SetActive(false);
                }
            }
        }

        lastTargetUnit = targetUnit;

        return mTargetEff;
    }

    public float DMGTakenLastFrame { get; private set; }

    List<BuffStat> listBuffs = new List<BuffStat>();
    public event System.Action<long> OnUnitTakenDMG;
    public event System.Action<long> OnUnitDinhDan;
    public bool IsInited { get { return mIsInited; } }


    public List<string> listWeapon = new List<string>();
    public QuanlyHieuung GetStatusEff()
    {
        if (mStatusEffect == null)
        {
            mStatusEffect = gameObject.AddMissingComponent<QuanlyHieuung>();
        }

        return mStatusEffect;
    }

    Color m_CurrentRendererColor = new Color(1,1,1,0);

    Color m_OriginRendererColor = new Color(1, 1, 1, 0);
    private Queue<ScreenBattle.DisplayHUDCommand> queueHUDs = new Queue<ScreenBattle.DisplayHUDCommand>();

    float hudCoolDown = 0;
    private void ProcessHud()
    {
        if (hudCoolDown > 0)
        {
            hudCoolDown -= Time.unscaledDeltaTime;
        }
        else
        {
            if (queueHUDs.Count > 0)
            {
                var command = queueHUDs.Dequeue();
                var originText = command.Text;
                //int count = 1;
                //while (command.canMerge && queueHUDs.Count > 0)
                //{
                //    var nextCommand = queueHUDs.Peek();
                //    if (nextCommand.canMerge && nextCommand.frame == command.frame && nextCommand.Text == originText)
                //    {
                //        count++;
                //        queueHUDs.Dequeue();
                //    }
                //    else
                //    {
                //        command.canMerge = false;
                //    }
                //}

                //if (count > 1)
                //{
                //    command.Text = originText + " x " + count;
                //}

                ScreenBattle.instance.DisplayHUD(command);
                hudCoolDown = 0.065f;
            }
        }
    }

    public void QueueHudDisplay(ScreenBattle.DisplayHUDCommand hudCommand)
    {
        queueHUDs.Enqueue(hudCommand);
    }

    public int GetCurrentHUDQueue()
    {
        return queueHUDs.Count;
    }

    public NavMeshAgent GetAgent()
    {
        return mAgent;
    }


    public bool Warp(Vector3 pos)
    {
        if (mAgent && mAgent.isOnNavMesh)
        {
            return mAgent.Warp(pos);
        }
        else
        {
            transform.position = pos;
            if (mAgent)
            {
                mAgent.enabled = false;
                mAgent.enabled = true;
            }

            return true;
        }
    }

    public void BackToOriginRendererColor()
    {
        SetRendererColor(m_OriginRendererColor, true);
    }

    public void SetRendererColor(Color c, bool saveColor = true)
    {
        if(saveColor)
            m_CurrentRendererColor = c;

        for(int m=0;m< SkinMeshRenderer.Length;m++)
        {
            SkinnedMeshRenderer mesh = SkinMeshRenderer[m];
            if (mesh != null && mesh.materials != null)
            {
                for (int i = 0; i < mesh.materials.Length; i++)
                {
                    var mat = mesh.materials[i];
                    if (mat != null && mat.HasProperty("_Color"))
                    {
                        mat.SetColor("_Color", c);
                    }
                }
            }
        }

        for (int m = 0; m < MeshRenderer.Length; m++)
        {
            MeshRenderer mesh = MeshRenderer[m];

            if (mesh != null && mesh.materials != null)
            {
                for (int i = 0; i < mesh.materials.Length; i++)
                {
                    var mat = mesh.materials[i];
                    if (mat != null && mat.HasProperty("_Color"))
                    {
                        mesh.materials[i].SetColor("_Color", c);
                    }
                }
            }
            
        }
    }

    IEnumerator OnDamageEff()
    {
        if (m_CurrentRendererColor== m_OriginRendererColor)
        {
            SetRendererColor(new Color(255f / 255f, 0f, 0f / 255f, 75f / 255f), false);

            yield return new WaitForSeconds(0.2f);

            SetRendererColor(m_CurrentRendererColor, false);
        }
    }

    public void InitMeshVisual()
    {
        SkinMeshRenderer = GetComponentsInChildren<SkinnedMeshRenderer>();
        MeshRenderer = GetComponentsInChildren<MeshRenderer>();

        if (SkinMeshRenderer.Length > 0)
        {
            for (int i = 0; i < SkinMeshRenderer.Length; ++i)
            {
                var renderer = SkinMeshRenderer[i];
                if (renderer)
                {
                    bool getColor = false;
                    foreach (var mat in renderer.materials)
                    {
                        if (mat != null && mat.HasProperty("_Color"))
                        {
                            m_OriginRendererColor = mat.GetColor("_Color");
                            getColor = true;
                            break;
                        }
                    }
                    if (getColor) break;
                }
            }
            //m_OriginRendererColor = SkinMeshRenderer[0].materials[0].GetColor("_Color");
        }
    }

    AudioSource HitSoundSource;
    //AudioSource DieSoundSource;

    DonViChienDau[] mChildren;

    private void Awake()
    {
        GetChargeStatusEffs();

        mAnimators = GetComponentsInChildren<Animator>();
        mAgent = GetComponentInChildren<NavMeshAgent>();
        mStatusEffect = gameObject.AddMissingComponent<QuanlyHieuung>();
        mKyNang = GetComponent<TuyetKyNhanVat>();
        mSungThayThe = GetComponentInChildren<SungThayThe>();

        InitMeshVisual();

        mChildren = GetComponentsInChildren<DonViChienDau>();

        var behaviorTree = GetComponent<BehaviorDesigner.Runtime.BehaviorTree>();
        if (behaviorTree)
        {
            behaviorTree.DisableBehavior();
        }

        mIsInited = false;

        m_Collider = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        if (mIsInited == false)
            Init();

        if (mAnimators == null || mAnimators.Length == 0)
        {
            mAnimators = GetComponentsInChildren<Animator>();
        }
    }

    private void OnDisable()
    {
#if UNITY_EDITOR
        //Debug.Log(UnitName + " ondisale");
#endif
    }

    void InitStatPlayerUnit()
    {
        ImmunityAfterTakeDMG = 0.98f; // cac unit co' atkspd = 1 khong bi miss dmg
        if (QuanlyNguoichoi.Instance.battleData != null)
        {
            if (QuanlyNguoichoi.Instance.battleData.Buffs != null)
            {
                QuanlyNguoichoi.Instance.IncreaseGioiHanBuff(QuanlyNguoichoi.Instance.battleData.Buffs.Count);

                foreach (var buff in QuanlyNguoichoi.Instance.battleData.Buffs) // khoi phuc list buff tu battle data truoc
                {
                    QuanlyNguoichoi.Instance.GetBuff(buff, false);
                }
            }

            if (QuanlyNguoichoi.Instance.battleData.DeTu != null)
            {
                foreach (var s in QuanlyNguoichoi.Instance.battleData.DeTu)
                {
                    for (int i = 0; i < s.Value; ++i)
                    {
                        QuanlyNguoichoi.Instance.ThuPhucDeTu(s.Key);
                    }
                }
            }

            OriginStat = UnitStat.FromUnitStat(QuanlyNguoichoi.Instance.battleData.OriginStat);
            mCurStat = UnitStat.FromUnitStat(QuanlyNguoichoi.Instance.battleData.PlayerStat);
            Life = QuanlyNguoichoi.Instance.battleData.Life;
            mMaxHP = OriginStat.HP;

            if (QuanlyNguoichoi.Instance.battleData.MaxHP > 0)
            {
                mMaxHP = QuanlyNguoichoi.Instance.battleData.MaxHP;
            }
        }

#if UNITY_EDITOR
        var player = QuanlyNguoichoi.Instance;
        if (player && player.OverrideStat)
        {
            var overrideStat = OriginStat;

            overrideStat.HP = player.OverrideHP;
            overrideStat.DMG = player.OverrideDMG;

            OriginStat = overrideStat;
            mCurStat.HP = overrideStat.HP;
            mMaxHP = OriginStat.HP;
            mCurStat.DMG = overrideStat.DMG;
        }
#endif
    }

    void InitStatEnemyUnit()
    {
        if (QuanlyNguoichoi.Instance.IsFarmMode)
        {
            var farmConfig = QuanlyNguoichoi.Instance.farmConfig;
            var segmentIdx = QuanlyNguoichoi.Instance.SegmentIndex;

            //int hpBuff = farmConfig.HPBuff;
            //long dmg = farmConfig.DMG;

            //if (QuanlyNguoichoi.Instance.battleData != null)
            //{
            //    var playerOriginStat = UnitStat.FromUnitStat(QuanlyNguoichoi.Instance.battleData.OriginStat);
            //    var originDmg = ConfigManager.NormalizePlayerDMG(playerOriginStat.DMG, playerOriginStat.ATK_SPD);
            //    hpBuff = (int)Mathf.Round((originDmg / 100f - 1f) * 100);
            //    dmg = (long)Mathf.Round((playerOriginStat.HP / 500f) * farmConfig.DMG);
            //}

            //var buffStat = OriginStat;
            //buffStat.HP = OriginStat.HP + OriginStat.HP * farmConfig.GetHPBuff(hpBuff, segmentIdx) / 100;
            //buffStat.DMG = farmConfig.GetDMG(dmg, segmentIdx);
            //buffStat.COLLISION_DMG = buffStat.DMG;
            OriginStat = ConfigManager.InitStatEnemyUnit_FarmMode(OriginStat, farmConfig, segmentIdx, QuanlyNguoichoi.Instance.battleData);
        }
        else if (QuanlyNguoichoi.Instance.IsLeoThapMode)
        {
            //var buffStat = OriginStat;
            //var configRank = ConfigManager.LeoThapCfg.GetLeoThapRankCfg(QuanlyNguoichoi.Instance.LeoThapRankIdx);

            //var dmg = ConfigManager.LeoThapBattleCfg.GetDMG(configRank.BaseStat,
            //    QuanlyNguoichoi.Instance.MissionID - 1);
            //var hpBuff = ConfigManager.LeoThapBattleCfg.GetHPBuff(configRank.BaseStat,
            //    QuanlyNguoichoi.Instance.MissionID - 1);
            //var hp = OriginStat.HP + OriginStat.HP * hpBuff / 100;
            //buffStat.HP = hp;
            //buffStat.DMG = dmg;
            //buffStat.COLLISION_DMG = buffStat.DMG;
            //OriginStat = buffStat;
            OriginStat = ConfigManager.InitStatEnemyUnit_LeoThap(OriginStat,
                QuanlyNguoichoi.Instance.LeoThapRankIdx,
                QuanlyNguoichoi.Instance.MissionID,
                QuanlyNguoichoi.Instance.battleData);
        }
        else if (QuanlyNguoichoi.Instance.IsSanTheMode)
        {
            //var buffStat = OriginStat;
            //if (QuanlyNguoichoi.Instance.battleData != null && QuanlyNguoichoi.Instance.battleData.SanThe != null)
            //{
            //    var sanTheBattleData = QuanlyNguoichoi.Instance.battleData.SanThe;

            //    var dmg = sanTheBattleData.HeSoATK * ConfigManager.SanTheCfg.GetHeSoFinalATK(QuanlyNguoichoi.Instance.MissionID) / 100;
            //    var hp = OriginStat.HP * sanTheBattleData.HeSoHP / 100 * ConfigManager.SanTheCfg.GetHeSoFinalHP(QuanlyNguoichoi.Instance.MissionID) / 100;
            //    if (IsBoss)
            //    {
            //        hp = hp * ConfigManager.SanTheCfg.HeSoHPBoss / 100;
            //    }
            //    else
            //    {
            //        hp = hp * ConfigManager.SanTheCfg.HeSoHPQuai / 100;
            //    }

            //    buffStat.HP = hp;
            //    buffStat.DMG = dmg;
            //    buffStat.COLLISION_DMG = buffStat.DMG;
            //}

            OriginStat = ConfigManager.InitStatEnemyUnit_SanThe(OriginStat,
                QuanlyNguoichoi.Instance.MissionID,
                QuanlyNguoichoi.Instance.battleData,
                IsBoss);
        }
        else if (QuanlyNguoichoi.Instance.IsThachDauMode)
        {
            //var buffStat = OriginStat;

            //var dmg = ConfigManager.ThachDauBattleCfg.GetDMG(QuanlyNguoichoi.Instance.MissionID - 1);
            //var hpBuff = ConfigManager.ThachDauBattleCfg.GetHPBuff(QuanlyNguoichoi.Instance.MissionID - 1);
            //var hp = OriginStat.HP + OriginStat.HP * hpBuff / 100;
            //buffStat.HP = hp;
            //buffStat.DMG = dmg;
            //buffStat.COLLISION_DMG = buffStat.DMG;
            OriginStat = ConfigManager.InitStatEnemyUnit_ThachDau(OriginStat,
                QuanlyNguoichoi.Instance.MissionID, QuanlyNguoichoi.Instance.battleData);
        }
        else if (QuanlyNguoichoi.Instance.IsNienThuMode)
        {
            //var buffStat = OriginStat;

            //var dmg = ConfigManager.TetEventCfg.EnemyBaseDMG;
            //var hp = ConfigManager.TetEventCfg.EnemyBaseDMG * OriginStat.HP / OriginStat.DMG;
            //buffStat.HP = hp;
            //buffStat.DMG = dmg;
            //buffStat.COLLISION_DMG = buffStat.DMG;
            OriginStat = ConfigManager.InitStatEnemyUnit_NienThu(OriginStat, QuanlyNguoichoi.Instance.battleData);
        }
        else
        {
            //var chapterCfg = PlayerManager.Instance.currentChap;
            //var segmentIdx = PlayerManager.Instance.SegmentIndex;

            var dungeonCfg = QuanlyNguoichoi.Instance.GetMapDungeonConfig();
            var pathCfg = QuanlyNguoichoi.Instance.GetPathNodeType();

            //var buffStat = OriginStat;
            ////buffStat.HP = OriginStat.HP + OriginStat.HP * chapterCfg.GetHPBuff(segmentIdx) / 100;
            //buffStat.HP = OriginStat.HP + OriginStat.HP * dungeonCfg.GetHPBuff(pathCfg) / 100;
            ////buffStat.DMG = chapterCfg.GetDMG(segmentIdx);
            //buffStat.DMG = dungeonCfg.GetDMG(pathCfg);

            //buffStat.COLLISION_DMG = buffStat.DMG;
            int buffStatHP = 0;
            if (QuanlyManchoi.instance.WaveCtrler != null
              && QuanlyManchoi.instance.WaveCtrler.Waves.Length > 0
              && QuanlyManchoi.instance.WaveCtrler.WavesHPRatio != null
              && QuanlyManchoi.instance.WaveCtrler.CurrentWave - 1 < QuanlyManchoi.instance.WaveCtrler.WavesHPRatio.Length
              && QuanlyManchoi.instance.WaveCtrler.CurrentWave - 1 > 0)
            {
                buffStatHP = QuanlyManchoi.instance.WaveCtrler.WavesHPRatio[QuanlyManchoi.instance.WaveCtrler.CurrentWave - 1];
            }

            OriginStat = ConfigManager.InitStatEnemyUnit_Normal(OriginStat, pathCfg, dungeonCfg.HPBuff, dungeonCfg.DMG, QuanlyNguoichoi.Instance.battleData, buffStatHP);
        }


    }

    void Init()
    {
        bodyScale = transform.localScale;

        queueHUDs.Clear();
        hudCoolDown = 0;
        curLeech = 0;

        if (QuanlyNguoichoi.Instance == null) return;
        if (QuanlyNguoichoi.Instance.PlayerUnit == null) return;

        if (mIsInited) return;
        if (QuanlyManchoi.instance == null) return;
        if (QuanlyNguoichoi.Instance == null) return;

        if (TeamID == QuanlyManchoi.EnemyTeam &&
            QuanlyNguoichoi.Instance.IsLevelClear)
        {
            // auto disable enemy if level is cleared before
            gameObject.SetActive(false);
            return;
        }

        if (QuanlyManchoi.instance)
        {
            if (TeamID == QuanlyManchoi.EnemyTeam)
            {
                Level = QuanlyManchoi.instance.EnemyLevel;
            }

            if (NoneTargetUnit == false)
                QuanlyManchoi.instance.AddUnit(this);
        }

        UnitStat config_stat = ConfigManager.GetUnitStat(UnitName);

        OriginStat = UnitStatUtils.GetStatAtLevel(config_stat, Level);
        STReceived = 0;
        Life = 1;

        if (TeamID == QuanlyManchoi.EnemyTeam) // update stat by buff from chapter config
        {
            InitStatEnemyUnit();
        }

        mCurStat = OriginStat;
        mMaxHP = OriginStat.HP;

        if (this == QuanlyNguoichoi.Instance.PlayerUnit)
        {
            InitStatPlayerUnit();
        }

        if (ScreenBattle.instance &&
            mCurHPBar == null &&
            NoneTargetUnit == false)
        {
            mCurHPBar = ScreenBattle.instance.CreateHPBar(this);
        }

        if (mCurHPBar) mCurHPBar.UpdateHP();

        UpdateShooterStat();
        UpdateMovementStat();

        mIsInited = true;
        DMGTakenLastFrame = 0;
        mInvulnerableTime = 0;
        if (mCurHPBar) mCurHPBar.UpdateHP();

        BehaviorDesigner.Runtime.BehaviorTree behaviorTree = GetComponent<BehaviorDesigner.Runtime.BehaviorTree>();
        if (behaviorTree)
        {
            behaviorTree.EnableBehavior();
        }

        // Init Sound
        HitSoundSource = gameObject.AddComponent<AudioSource>();
        HitSoundSource.spatialBlend = 1f;
        HitSoundSource.rolloffMode = AudioRolloffMode.Linear;
        HitSoundSource.minDistance = 4;
        HitSoundSource.maxDistance = 40;


        string hit_sound_name = config_stat.HitSound;
        if (hit_sound_name == null || hit_sound_name == "")
            hit_sound_name = "hit_body";
        string hit_sound_path = "Sound/SFX/Battle/" + hit_sound_name;
        HitSoundSource.clip = Resources.Load(hit_sound_path) as AudioClip;


        if (QuanlyNguoichoi.Instance.PlayerUnit != this)
        {
            if (shooter != null)
                shooter.SetFireClip(config_stat.Fire1Sound);

            if (shooter2 != null)
                shooter2.SetFireClip(config_stat.Fire2Sound);

            if (shooter3 != null)
                shooter3.SetFireClip(config_stat.Fire3Sound);

            if (shooter4 != null)
                shooter4.SetFireClip(config_stat.Fire4Sound);
        }

        if (mCurStat.HP <= 0)
        {
            Hiker.HikerUtils.DoAction(this, OnDie, 0.1f, true);
        }

        CheckLeoThapModifier();

        if (QuanlyNguoichoi.Instance)
        {
            QuanlyNguoichoi.Instance.OnEnemyInit(this);
        }


        if (UnitName == "IronManRocket")
        {
            var ricochetStat = QuanlyNguoichoi.Instance.GetBuffStatByType(BuffType.RICOCHET);
            GetBuff(ricochetStat);
            GetBuff(ricochetStat);
        }
    }

    void CheckLeoThapModifier()
    {
        if (this == QuanlyNguoichoi.Instance.PlayerUnit)
        {
            switch (QuanlyNguoichoi.Instance.LeoThapMod)
            {
                case 2: // HutMau
                    mMaCaRongModifier = gameObject.AddMissingComponent<MaCaRongModifier>();
                    break;
                case 4: // flash
                    {
                        mFlashModifier = gameObject.AddMissingComponent<FlashModifier>();
                    }
                    break;
                case 5: // infernal
                    {
                        mInfernalModifier = gameObject.AddMissingComponent<InfernalModifier>();
                    }
                    break;
                default:
                    if (mMaCaRongModifier == null)
                        mMaCaRongModifier = gameObject.GetComponent<MaCaRongModifier>();
                    if (mMaCaRongModifier)
                    {
                        Destroy(mMaCaRongModifier);
                        mMaCaRongModifier = null;
                    }

                    if (mFlashModifier == null)
                        mFlashModifier = gameObject.GetComponent<FlashModifier>();
                    if (mFlashModifier)
                    {
                        Destroy(mFlashModifier);
                        mFlashModifier = null;
                    }

                    if (mInfernalModifier == null)
                        mInfernalModifier = gameObject.GetComponent<InfernalModifier>();
                    if (mInfernalModifier)
                    {
                        Destroy(mInfernalModifier);
                        mInfernalModifier = null;
                    }
                    break;

            }
        }
        else if (this.TeamID != QuanlyManchoi.PlayerTeam)
        {
            switch (QuanlyNguoichoi.Instance.LeoThapMod)
            {
                case 1: // KhongLoHoa
                    if (doNotApplyKhongLoModifier == false)
                    {
                        mKhongLoHoaModifier = gameObject.AddMissingComponent<KhongLoHoaModifier>();
                    }
                    break;
                case 3: // Popo
                    if (IsBoss == false && doNotApplyPopoModifier == false)
                    {
                        mPopoModifier = gameObject.AddMissingComponent<PopoModifier>();
                    }
                    break;
                case 4: // flash
                    {
                        mFlashModifier = gameObject.AddMissingComponent<FlashModifier>();
                        ApplyFlashModifier();
                    }
                    break;
                case 6: // deathrattle
                    {
                        if (IsBoss == false && doNotApplyDeathrateModifier == false)
                        {
                            mDeathrattleModifier = gameObject.AddMissingComponent<DeathrattleModifier>();
                        }
                    }
                    break;
                default:
#if UNITY_EDITOR
                    if (QuanlyNguoichoi.Instance.TestMission)
                    {
                        if (mKhongLoHoaModifier == null)
                            mKhongLoHoaModifier = gameObject.GetComponent<KhongLoHoaModifier>();
                        
                        if (mPopoModifier == null)
                            mPopoModifier = gameObject.GetComponent<PopoModifier>();
                        
                        if (mFlashModifier == null)
                            mFlashModifier = gameObject.GetComponent<FlashModifier>();
                        
                        if (mDeathrattleModifier == null)
                            mDeathrattleModifier = gameObject.GetComponent<DeathrattleModifier>();
                        
                        break;
                    }
#endif
                    if (mKhongLoHoaModifier == null)
                        mKhongLoHoaModifier = gameObject.GetComponent<KhongLoHoaModifier>();
                    if (mKhongLoHoaModifier)
                    {
                        Destroy(mKhongLoHoaModifier);
                        mKhongLoHoaModifier = null;
                    }

                    if (mPopoModifier == null)
                        mPopoModifier = gameObject.GetComponent<PopoModifier>();
                    if (mPopoModifier)
                    {
                        Destroy(mPopoModifier);
                        mPopoModifier = null;
                    }

                    if (mFlashModifier == null)
                        mFlashModifier = gameObject.GetComponent<FlashModifier>();
                    if (mFlashModifier)
                    {
                        Destroy(mFlashModifier);
                        mFlashModifier = null;
                    }

                    if (mDeathrattleModifier == null)
                        mDeathrattleModifier = gameObject.GetComponent<DeathrattleModifier>();
                    if (mDeathrattleModifier)
                    {
                        Destroy(mDeathrattleModifier);
                        mDeathrattleModifier = null;
                    }

                    break;

            }
        }
    }

    public void ApplyFlashModifier()
    {
        if (mFlashModifier != null)
        {
            var newStat = OriginStat;
            newStat.SPD = mFlashModifier.GetBuffSpeed(OriginStat.SPD);
            newStat.PROJ_SPD = mFlashModifier.GetBuffSpeed(OriginStat.PROJ_SPD);
            OriginStat = newStat;

            newStat = mCurStat;
            newStat.SPD = mFlashModifier.GetBuffSpeed(mCurStat.SPD);
            newStat.PROJ_SPD = mFlashModifier.GetBuffSpeed(mCurStat.PROJ_SPD);
            mCurStat = newStat;

            UpdateMovementStat();
            UpdateShooterStat();
        }
    }

    public void UpdateMovementStat()
    {
        if (mAgent == null)
        {
            mAgent = GetComponent<NavMeshAgent>();
        }

        if (mAgent)
        {
            mAgent.speed = mCurStat.SPD * TimeScale;
        }
    }

    public void UpdateShooterStat()
    {
        if (shooter)
        {
            long dmg = mCurStat.DMG + (OriginStat.DMG * RageBuffAtk / 100);
            float akSpd = mCurStat.ATK_SPD + (OriginStat.ATK_SPD * RageBuffAtkSpd / 100);

            shooter.DMG = dmg;
            shooter.AtkSpd = akSpd * TimeScale;
            shooter.ProjSpd = mCurStat.PROJ_SPD;
            shooter.KnockBackDistance = mCurStat.KNOCK_BACK;
        }

        if (shooter2)
        {
            shooter2.DMG = mCurStat.DMG;
            shooter2.AtkSpd = shooter2.OriginAtkSpd * TimeScale;
        }

        if (shooter3)
        {
            shooter3.DMG = mCurStat.DMG;
            shooter3.AtkSpd = shooter3.OriginAtkSpd * TimeScale;
        }

        if (shooter4)
        {
            shooter4.DMG = mCurStat.DMG;
            shooter4.AtkSpd = shooter4.OriginAtkSpd * TimeScale;
        }

        // override setting shooter only for player unit
        if (this == QuanlyNguoichoi.Instance.PlayerUnit)
        {
            var listBattleItem = QuanLyBattleItem.instance != null ? QuanLyBattleItem.instance.GetWeaponWithBuff() : null;
            if (shooter)
            {
                if(listBattleItem != null && listBattleItem.Count > 0)
                {
                    var weaponStatCfg = listBattleItem[0];
                    if (weaponStatCfg != null)
                    {
                        shooter.weaponCfg = weaponStatCfg;
                        shooter.DMG = mCurStat.DMG + weaponStatCfg.DMG;
                        shooter.AtkSpd = (mCurStat.ATK_SPD + weaponStatCfg.ATK_SPD) * TimeScale;
                        shooter.ProjSpd = mCurStat.PROJ_SPD + weaponStatCfg.PROJ_SPD;
                        shooter.KnockBackDistance = mCurStat.KNOCK_BACK + weaponStatCfg.KNOCK_BACK;
                        shooter.CRIT = mCurStat.CRIT + weaponStatCfg.CRIT;
                        shooter.CRIT_DMG = mCurStat.CRIT_DMG + weaponStatCfg.CRIT_DMG;
                        shooter.CaculateFinalStat();
                        listBattleItem.RemoveAt(0);
                    }
                }
                else if(listWeapon.Count > 0)
                {
                    var weaponStatCfg = WeaponStatConfig.FromWeaponStat(listWeapon[0]);
                    if(weaponStatCfg != null)
                    {
                        shooter.weaponCfg = weaponStatCfg;
                        shooter.DMG = mCurStat.DMG + weaponStatCfg.DMG;
                        shooter.AtkSpd = (mCurStat.ATK_SPD + weaponStatCfg.ATK_SPD) * TimeScale;
                        shooter.ProjSpd = mCurStat.PROJ_SPD + weaponStatCfg.PROJ_SPD;
                        shooter.KnockBackDistance = mCurStat.KNOCK_BACK + weaponStatCfg.KNOCK_BACK;
                        shooter.CRIT = mCurStat.CRIT + weaponStatCfg.CRIT;
                        shooter.CRIT_DMG = mCurStat.CRIT_DMG + weaponStatCfg.CRIT_DMG;
                        shooter.CaculateFinalStat();
                    }
                }
            }
            for(int i = 0; i < shooterList.Count; i++)
            {
                if (shooterList[i] != null)
                {
                    bool activeShoot = false;
                    if (listBattleItem != null && listBattleItem.Count > 0)
                    {
                        for (int j = 0; j < listBattleItem.Count; j++)
                        {
                            var weaponStatCfg = listBattleItem[j];
                            if (weaponStatCfg != null)
                            {
                                shooterList[i].weaponCfg = weaponStatCfg;
                                shooterList[i].DMG = mCurStat.DMG + weaponStatCfg.DMG;
                                shooterList[i].AtkSpd = (mCurStat.ATK_SPD + weaponStatCfg.ATK_SPD) * TimeScale;
                                shooterList[i].ProjSpd = mCurStat.PROJ_SPD + weaponStatCfg.PROJ_SPD;
                                shooterList[i].KnockBackDistance = mCurStat.KNOCK_BACK + weaponStatCfg.KNOCK_BACK;
                                shooterList[i].CRIT = mCurStat.CRIT + weaponStatCfg.CRIT;
                                shooterList[i].CRIT_DMG = mCurStat.CRIT_DMG + weaponStatCfg.CRIT_DMG;
                                shooterList[i].CaculateFinalStat();

                                shooterList[i].AttackAnimTime = shooter.AttackAnimTime;
                                shooterList[i].DelayActiveDamage = shooter.DelayActiveDamage;
                                activeShoot = true;
                                listBattleItem.RemoveAt(j);
                                break;
                            }
                        }
                    }
                    else if (listWeapon.Count > i + 1)
                    {
                        var weaponStatCfg = WeaponStatConfig.FromWeaponStat(listWeapon[i + 1]);
                        if (weaponStatCfg != null)
                        {
                            shooterList[i].weaponCfg = weaponStatCfg;
                            shooterList[i].DMG = mCurStat.DMG + weaponStatCfg.DMG;
                            shooterList[i].AtkSpd = (mCurStat.ATK_SPD + weaponStatCfg.ATK_SPD) * TimeScale;
                            shooterList[i].ProjSpd = mCurStat.PROJ_SPD + weaponStatCfg.PROJ_SPD;
                            shooterList[i].KnockBackDistance = mCurStat.KNOCK_BACK + weaponStatCfg.KNOCK_BACK;
                            shooterList[i].CRIT = mCurStat.CRIT + weaponStatCfg.CRIT;
                            shooterList[i].CRIT_DMG = mCurStat.CRIT_DMG + weaponStatCfg.CRIT_DMG;
                            shooterList[i].CaculateFinalStat();

                            shooterList[i].AttackAnimTime = shooter.AttackAnimTime;
                            shooterList[i].DelayActiveDamage = shooter.DelayActiveDamage;
                            activeShoot = true;
                        }
                    }
                    shooterList[i].gameObject.SetActive(activeShoot);
                }
            }
            //if (shooter2)
            //{
            //    bool activeShoot = false;
            //    if (listWeapon.Count > 1)
            //    {
            //        var weaponStatCfg = WeaponStatConfig.FromWeaponStat(listWeapon[1]);
            //        if (weaponStatCfg != null)
            //        {
            //            shooter2.weaponCfg = weaponStatCfg;
            //            shooter2.DMG = weaponStatCfg.DMG;
            //            shooter2.AtkSpd = weaponStatCfg.ATK_SPD * TimeScale;
            //            shooter2.ProjSpd = weaponStatCfg.PROJ_SPD;
            //            shooter2.KnockBackDistance = weaponStatCfg.KNOCK_BACK;

            //            shooter2.AttackAnimTime = shooter.AttackAnimTime;
            //            shooter2.DelayActiveDamage = shooter.DelayActiveDamage;

            //            activeShoot = true;
            //        }
            //    }
            //    shooter2.gameObject.SetActive(activeShoot);
            //}
            //if (shooter3)
            //{
            //    bool activeShoot = false;
            //    if (listWeapon.Count > 2)
            //    {
            //        var weaponStatCfg = WeaponStatConfig.FromWeaponStat(listWeapon[2]);
            //        if (weaponStatCfg != null)
            //        {
            //            shooter3.weaponCfg = weaponStatCfg;
            //            shooter3.DMG = weaponStatCfg.DMG;
            //            shooter3.AtkSpd = weaponStatCfg.ATK_SPD * TimeScale;
            //            shooter3.ProjSpd = weaponStatCfg.PROJ_SPD;
            //            shooter3.KnockBackDistance = weaponStatCfg.KNOCK_BACK;

            //            shooter3.AttackAnimTime = shooter.AttackAnimTime;
            //            shooter3.DelayActiveDamage = shooter.DelayActiveDamage;
            //            activeShoot = true;
            //        }
            //    }
            //    shooter3.gameObject.SetActive(activeShoot);
            //}
            //if (shooter4)
            //{
            //    bool activeShoot = false;
            //    if (listWeapon.Count > 4)
            //    {
            //        var weaponStatCfg = WeaponStatConfig.FromWeaponStat(listWeapon[4]);
            //        if (weaponStatCfg != null)
            //        {
            //            shooter4.weaponCfg = weaponStatCfg;
            //            shooter4.DMG = weaponStatCfg.DMG;
            //            shooter4.AtkSpd = weaponStatCfg.ATK_SPD * TimeScale;
            //            shooter4.ProjSpd = weaponStatCfg.PROJ_SPD;
            //            shooter4.KnockBackDistance = weaponStatCfg.KNOCK_BACK;

            //            shooter4.AttackAnimTime = shooter.AttackAnimTime;
            //            shooter4.DelayActiveDamage = shooter.DelayActiveDamage;
            //            activeShoot = true;
            //        }
            //    }
            //    shooter4.gameObject.SetActive(activeShoot);
            //}
        }
    }

    private void Start()
    {
        if (ScreenBattle.instance &&
            mCurHPBar == null &&
            NoneTargetUnit == false)
        {
            mCurHPBar = ScreenBattle.instance.CreateHPBar(this);
        }
    }

    float dieTime = 0;
    static readonly int speedID = Animator.StringToHash("speed");
    private void Update()
    {
        if (mIsInited == false && dieTime <= 0)
        {
            Init();
            return;
        }

        if (lastTimeGetImpact > 0)
        {
            lastTimeGetImpact -= Time.deltaTime;
        }

        ProcessHud();

        if (mInvulnerableTime > 0)
        {
            mInvulnerableTime -= Time.deltaTime;
        }

        if (mAnimators != null && mAnimators.Length > 0)
        {
            if (mAgent)
            {
                for (int i = 0; i < mAnimators.Length; ++i)
                {
                    var mAnimator = mAnimators[i];
                    mAnimator.SetFloat(speedID, mAgent.velocity.magnitude);
                    //mAnimator.SetFloat("speed", 1);
                }
            }
        }

        if (mTimeOutOfTarget > 0)
        {
            mTimeOutOfTarget -= Time.deltaTime;
        }
        if (mKhongDinhDan > 0)
        {
            mKhongDinhDan -= Time.deltaTime;
        }

        UpdateChargeEff(Time.deltaTime);

        if (QuanlyNguoichoi.Instance.PlayerUnit == this)
        {
            if (timeReactiveLinken > 0)
            {
                timeReactiveLinken -= Time.deltaTime;
                if (timeReactiveLinken <= 0)
                {
                    var linkenCount = GetBuffCount(BuffType.LINKEN);

                    if (linkenCount > 0)
                    {
                        //if (GetStatusEff().IsHaveActiveEffect(EffectType.Linken) == false)
                        {
                            GetStatusEff().ApplyLinkenEffect();
                        }
                    }
                }
            }
        }
        
    }

    void UpdateChargeEff(float deltaTime)
    {
        bool haveRuningCharge = false;
        GetChargeStatusEffs();
        for (int i = mChargeStatusEffs.Count - 1; i >= 0; --i)
        {
            var eff = mChargeStatusEffs[i];
            switch (eff.ID)
            {
                case (int)ChargeEff.RuningCharge:
                    haveRuningCharge = true;
                    if (eff.CountStat < eff.MaxCount)
                    {
                        var buffStat = QuanlyNguoichoi.Instance.GetBuffStatByType(BuffType.RUNNING_CHARGE);
                        if (shooter &&
                            //shooter.IsFireReady() &&
                            shooter.TimeSceneLastFire > buffStat.Params[0] / 100f)
                        {
                            eff.CountStat += deltaTime;
#if DEBUG
                            //Debug.Log(eff.CountStat);
#endif
                            mChargeStatusEffs[i] = eff;
                        }
                    }
                    if (eff.CountStat > 0 && eff.CountStat < eff.MaxCount)
                    {
                        if (runingChargeEff == null)
                        {
                            runingChargeEff = ObjectPoolManager.Spawn("Particle/PowerUp/RUNNING_CHARGE_Charging",
                                Vector3.zero,
                                Quaternion.identity,
                                visualGO.transform);
                        }
                        if (runingChargeEff)
                        {
                            runingChargeEff.gameObject.SetActive(true);
                        }
                        if (runingChargeMax)
                        {
                            runingChargeMax.gameObject.SetActive(false);
                        }
                    }
                    else if (eff.CountStat >= eff.MaxCount)
                    {
                        if (runingChargeMax == null)
                        {
                            runingChargeMax = ObjectPoolManager.Spawn("Particle/PowerUp/RUNNING_CHARGE_ChargeMax",
                                Vector3.zero,
                                Quaternion.identity,
                                visualGO.transform);
                        }
                        if (runingChargeEff)
                        {
                            runingChargeEff.gameObject.SetActive(false);
                        }
                        if (runingChargeMax)
                        {
                            runingChargeMax.gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        if (runingChargeEff)
                        {
                            runingChargeEff.gameObject.SetActive(false);
                        }
                        if (runingChargeMax)
                        {
                            runingChargeMax.gameObject.SetActive(false);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        if (haveRuningCharge == false)
        {
            if (runingChargeEff)
            {
                runingChargeEff.gameObject.SetActive(false);
            }
            if (runingChargeMax)
            {
                runingChargeMax.gameObject.SetActive(false);
            }
        }
    }

    public void ResetChargeEff(ChargeEff eff)
    {
        GetChargeStatusEffs();
        for (int i = mChargeStatusEffs.Count - 1; i >= 0; --i)
        {
            var effObj = mChargeStatusEffs[i];

            if (effObj.ID == (int)eff)
            {
                effObj.CountStat = 0;
                mChargeStatusEffs[i] = effObj;
                return;
            }
        }

        if (eff == ChargeEff.RuningCharge)
        {
            if (runingChargeEff)
            {
                runingChargeEff.gameObject.SetActive(false);
            }
            if (runingChargeMax)
            {
                runingChargeMax.gameObject.SetActive(false);
            }
        }
    }

    public bool HaveChargeEff(ChargeEff eff)
    {
        GetChargeStatusEffs();
        return mChargeStatusEffs.Exists(e => e.ID == (int)eff);
    }

    public ChargeStatusEff GetChargeEff(ChargeEff eff)
    {
        GetChargeStatusEffs();
        var effObj = mChargeStatusEffs.Find(e => e.ID == (int)eff);
        return effObj;
    }

    public void RemoveChargeEff(ChargeEff eff)
    {
        GetChargeStatusEffs();
        for (int i = mChargeStatusEffs.Count - 1; i >= 0; --i)
        {
            if (mChargeStatusEffs[i].ID == (int)eff)
            {
                mChargeStatusEffs.RemoveAt(i);
                return;
            }
        }
    }

    public void AddChargeEff(ChargeEff eff, float maxCount)
    {
        GetChargeStatusEffs();
        if (mChargeStatusEffs.Exists(e => e.ID == (int)eff))
        {
            return;
        }
        var effObj = new ChargeStatusEff()
        {
            ID = (int)eff,
            CountStat = 0,
            MaxCount = maxCount
        };
        mChargeStatusEffs.Add(effObj);
    }

    private void LateUpdate()
    {
        DMGTakenLastFrame = 0;
    }

    List<ChargeStatusEff> GetChargeStatusEffs()
    {
        if (mChargeStatusEffs == null) mChargeStatusEffs = Hiker.Util.ListPool<ChargeStatusEff>.Claim();
        return mChargeStatusEffs;
    }

    private void OnDestroy()
    {
        //if (mCurHPBar != null) { mCurHPBar.gameObject.SetActive(false);  }
        if (mChargeStatusEffs != null)
        {
            Hiker.Util.ListPool<ChargeStatusEff>.Release(mChargeStatusEffs);
            mChargeStatusEffs = null;
        }

        if (listChains != null)
        {
            Hiker.Util.ListPool<ParticleChain>.Release(listChains);
            listChains = null;
        }
    }

    public void SetOutOfTarget(float t)
    {
        mTimeOutOfTarget = t;
    }

    public void SetKhongDinhDan(float t)
    {
        mKhongDinhDan = t;
    }

    public UnitStat GetCurStat()
    {
        return mCurStat;
    }

    public void UpdateMoveSpeedToCurStat(float speed)
    {
        mCurStat.SPD = speed;

        UpdateMovementStat();
    }


    public long GetCurHP()
    {
        return mCurStat.HP;
    }

    public long GetMaxHP()
    {
        return mMaxHP;
    }

    public UnitStat GetStat()
    {
        return mCurStat;
    }

    public bool IsAlive()
    {
        return mCurStat.HP > 0;
    }

    public void CancelShooter()
    {
        if (shooter)
        {
            shooter.StopAllCoroutines();
        }
        //if (shooter2)
        //{
        //    shooter2.StopAllCoroutines();
        //}
        //if (shooter3)
        //{
        //    shooter3.StopAllCoroutines();
        //}
        //if (shooter4)
        //{
        //    shooter4.StopAllCoroutines();
        //}

        for(int i = 0; i < shooterList.Count; i++)
        {
            shooterList[i].StopAllCoroutines();
        }

        if (SungPhu)
        {
            SungPhu.CancelShooter();
        }
    }

    public bool FireAt(Vector3 target, bool shouldPlayAtkAnim = true)
    {
        bool isFired = false;

        if (mKyNang)
        {
            mKyNang.OnBeforeUnitFired(lastTargetUnit);
        }

        if (mSungThayThe != null && mSungThayThe.IsActive)
        {
            isFired = mSungThayThe.FireAt(target, shouldPlayAtkAnim);
            if (isFired)
            {
                if (shooter) shooter.StartCoolDown(); // make sure main shooter be count as fired
            }
        }
        else
        {
            if (shooter && shooter.gameObject.activeSelf)
            {
                isFired = shooter.FireAt(target);
            }

            for(int i = 0; i < shooterList.Count; i++)
            {
                if (shooterList[i].gameObject.activeSelf)
                {
                    shooterList[i].FireAt(target);
                }
            }            

            //if (GetBuffCount(BuffType.SIDE_SHOT) > 0)
            //{
            //    if (shooter2)
            //    {
            //        shooter2.DMG = shooter.DMG;
            //        shooter2.ProjSpd = shooter.ProjSpd;
            //        shooter2.AtkSpd = shooter.AtkSpd;
            //        shooter2.AttackAnimTime = shooter.AttackAnimTime;
            //        shooter2.DelayActiveDamage = shooter.DelayActiveDamage;
            //        shooter2.KnockBackDistance = shooter.KnockBackDistance;
            //        shooter2.FireAt(target);
            //    }
            //}

            //if (GetBuffCount(BuffType.CHEO_SHOT) > 0)
            //{
            //    if (shooter3)
            //    {
            //        shooter3.DMG = shooter.DMG;
            //        shooter3.ProjSpd = shooter.ProjSpd;
            //        shooter3.AtkSpd = shooter.AtkSpd;
            //        shooter3.AttackAnimTime = shooter.AttackAnimTime;
            //        shooter3.DelayActiveDamage = shooter.DelayActiveDamage;
            //        shooter3.KnockBackDistance = shooter.KnockBackDistance;
            //        shooter3.FireAt(target);
            //    }
            //}

            //if (GetBuffCount(BuffType.BACK_SHOT) > 0)
            //{
            //    if (shooter4)
            //    {
            //        shooter4.DMG = shooter.DMG;
            //        shooter4.ProjSpd = shooter.ProjSpd;
            //        shooter4.AtkSpd = shooter.AtkSpd;
            //        shooter4.AttackAnimTime = shooter.AttackAnimTime;
            //        shooter4.DelayActiveDamage = shooter.DelayActiveDamage;
            //        shooter4.KnockBackDistance = shooter.KnockBackDistance;
            //        shooter4.FireAt(target);
            //    }
            //}

            if (isFired && shouldPlayAtkAnim)
            {
                if (mAnimators != null && mAnimators.Length > 0)
                {
                    //if (mAgent)
                    {
                        for (int i = 0; i < mAnimators.Length; ++i)
                        {
                            var mAnimator = mAnimators[i];
                            mAnimator.SetTrigger("atk");
                            //Debug.Log(gameObject.name + "SetTriggerAtk " + Time.time);
                            mAnimator.SetFloat("atkSpd", shooter.AtkSpdAnimScale);
                        }
                    }
                }
            }
        }

        if (mKyNang && isFired)
        {
            mKyNang.OnUnitFired(lastTargetUnit);
        }

        return isFired;
    }

    public void FireAt2(Vector3 target, bool shouldPlayAtkAnim = true)
    {
        bool isFired = false;
        if (shooter2) { isFired = shooter2.FireAt(target); }

        if (isFired && shouldPlayAtkAnim)
        {
            if (mAnimators != null && mAnimators.Length > 0)
            {
                //if (mAgent)
                {
                    for (int i = 0; i < mAnimators.Length; ++i)
                    {
                        var mAnimator = mAnimators[i];
                        mAnimator.SetTrigger("atk2");
                        mAnimator.SetFloat("atkSpd", shooter2.AtkSpdAnimScale);
                    }
                }
            }
        }
    }

    public void FireAt3(Vector3 target, bool shouldPlayAtkAnim = true)
    {
        bool isFired = false;
        if (shooter3) { isFired = shooter3.FireAt(target); }

        if (isFired && shouldPlayAtkAnim)
        {
            if (mAnimators != null && mAnimators.Length > 0)
            {
                //if (mAgent)
                {
                    for (int i = 0; i < mAnimators.Length; ++i)
                    {
                        var mAnimator = mAnimators[i];
                        mAnimator.SetTrigger("atk3");
                        mAnimator.SetFloat("atkSpd", shooter3.AtkSpdAnimScale);
                    }
                }
            }
        }
    }

    public void FireAt4(Vector3 target, bool shouldPlayAtkAnim = true)
    {
        bool isFired = false;
        if (shooter4) { isFired = shooter4.FireAt(target); }

        if (isFired && shouldPlayAtkAnim)
        {
            if (mAnimators != null && mAnimators.Length > 0)
            {
                //if (mAgent)
                {
                    for (int i = 0; i < mAnimators.Length; ++i)
                    {
                        var mAnimator = mAnimators[i];
                        mAnimator.SetTrigger("atk4");
                        mAnimator.SetFloat("atkSpd", shooter4.AtkSpdAnimScale);
                    }
                }
            }
        }
    }

    public void KnockBack(Vector3 distance)
    {
        var knockBackRes = GetStat().KNOCK_BACK_RES;
        if (knockBackRes >= 100)
        {
            return;
        }
        if (knockBackRes > 0)
        {
            distance -= distance * knockBackRes / 100f;
        }

        if (mAgent && mAgent.isActiveAndEnabled)
        {
            if (mAgent.Raycast(transform.position + distance, out NavMeshHit hit))
            {
                mAgent.Warp(hit.position);
                mAgent.nextPosition = hit.position;
            }
            else
            {
                var nextPos = transform.position + distance;
                mAgent.Warp(nextPos);
                mAgent.nextPosition = nextPos;
            }
        }
        else
        {
            transform.Translate(distance, Space.World);
        }

        mStatusEffect.ApplyEffect(new BattleEffect(new EffectConfig() { Type = EffectType.Root, Duration = 0.2f }));
    }

    public void SetStun(bool isStun)
    {
        var behavior = GetComponent<BehaviorDesigner.Runtime.BehaviorTree>();
        if (isStun)
        {
            if (behavior)
            {
                behavior.DisableBehavior();
            }

            if (mAgent)
            {
                mAgent.isStopped = true;
            }
        }
        else
        {
            if (behavior)
            {
                behavior.EnableBehavior();
            }

            if (mAgent)
            {
                mAgent.isStopped = false;
            }
        }
    }

    public void SetRoot(bool isStun)
    {
        var behavior = GetComponent<BehaviorDesigner.Runtime.BehaviorTree>();
        if (isStun)
        {
            //if (behavior)
            //{
            //    behavior.DisableBehavior();
            //}

            if (mAgent && mAgent.isOnNavMesh)
            {
                mAgent.isStopped = true;
            }
        }
        else
        {
            //if (behavior)
            //{
            //    behavior.EnableBehavior();
            //}

            if (mAgent && mAgent.isOnNavMesh)
            {
                mAgent.isStopped = false;
            }
        }
    }

    void OnChangedHP(float changedHP)
    {
        if (IsAlive() == false) return;

        CheckRageAtk();
        CheckRageAtkSpd();
    }

    public void RegenHP(long hp, bool displayRegen, string nguon)
    {
        if (IsAlive() == false) return;

        if (QuanlyNguoichoi.Instance &&
            this == QuanlyNguoichoi.Instance.PlayerUnit)
        {
            var listMods = QuanlyNguoichoi.Instance.GetListRuntimeStatMods();
            if (listMods != null)
            {
                int buff = 0;
                int scale = 0;
                long add = 0;
                foreach (var m in listMods)
                {
                    if (m.Stat == EStatType.HEALING)
                    {
                        if (m.Mod == EStatModType.ADD)
                        {
                            add += (long)Mathf.Round((float)m.Val);
                        }
                        else if (m.Mod == EStatModType.MUL)
                        {
                            scale += (int)Mathf.Round((float)m.Val);
                        }
                        else if (m.Mod == EStatModType.BUFF)
                        {
                            buff += (int)Mathf.Round((float)m.Val);
                        }
                    }
                }

                if (add > 0)
                {
                    hp += add;
                }
                if (scale > 0)
                {
                    var scaleHP = hp * scale / 100;
                    if (scaleHP > 0)
                    {
                        hp = scaleHP;
                    }
                    else if (hp > 0)
                    {
                        hp = 1;
                    }
                }
                if (buff > 0)
                {
                    hp += hp * buff / 100;
                }
            }
        }

        if (hp > 0)
        {
            if (displayRegen)
                ScreenBattle.instance.DisplayRegenHP(hp, this);

            long lastHP = mCurStat.HP;

            mCurStat.HP += hp;
            if (mCurStat.HP > mMaxHP)
            {
                mCurStat.HP = mMaxHP;
            }

            if (QuanlyNguoichoi.Instance &&
                this == QuanlyNguoichoi.Instance.PlayerUnit)
            {
                if (nguon != "+Hut")
                {
                    if (curLeech > 0)
                    {
                        QuanlyNguoichoi.Instance.AddThongKeST("+Hut", curLeech, preMauLeech, curMauLeech, mMaxHP);
                        curLeech = 0;
                        preMauLeech = 0;
                        curMauLeech = 0;
                    }

                    if (mCurStat.HP != lastHP)
                    {
                        QuanlyNguoichoi.Instance.AddThongKeST(nguon, hp, lastHP, mCurStat.HP, mMaxHP);
                    }
                }
                else
                {
                    if (mCurStat.HP != lastHP)
                    {
                        curLeech += hp;
                    }
                }
            }

            if (mCurStat.HP != lastHP)
            {
                OnChangedHP(mCurStat.HP - lastHP);
            }
        }

        if (mCurHPBar) mCurHPBar.UpdateHP();

        if (QuanlyNguoichoi.Instance &&
            QuanlyNguoichoi.Instance.PlayerUnit == this &&
            GameClient.instance &&
            GameClient.instance.OfflineMode == false &&
            displayRegen)
        {
            QuanlyNguoichoi.Instance.UpdatePlayerStat(true);
        }
    }

    long curLeech = 0;
    long preMauLeech = 0;
    long curMauLeech = 0;
    public void LifeLeech(long dmg)
    {
        var lifeLeechCount = GetBuffCount(BuffType.LIFE_LEECH);
        if (lifeLeechCount > 0)
        {
            if (curLeech == 0)
            {
                preMauLeech = mCurStat.HP;
            }
            var buffStat = QuanlyNguoichoi.Instance.GetBuffStatByType(BuffType.LIFE_LEECH);
            var regen = dmg * buffStat.Params[lifeLeechCount - 1] / 100;
            if (dmg > 0 && regen < 1) regen = 1;
            RegenHP((int)regen, false, "+Hut");
            curMauLeech = mCurStat.HP;
        }
    }

    public void PlaySound(string path)
    {
        AudioClip clip = Resources.Load(path) as AudioClip;

        if (clip != null)
        {
            //AudioSource.PlayClipAtPoint(clip, pos, vol);

        }
    }

    public void PlayHitSound()
    {
        if (HitSoundSource)
            HitSoundSource.Play();
    }

    float timeReactiveLinken = 0f;

    [Beebyte.Obfuscator.ObfuscateLiterals]
    public long TakeDamage(long dmg,
        bool displayCrit = false,
        DonViChienDau sourceUnit = null,
        bool playHitSound = true,
        bool showHUD = true,
        bool xuyenInvulnerable = false,
        bool xuyenLinken = false,
        bool thongKe = true,
        bool isHeadShot = false,
        bool sourceUnitIsBoss = false,
        bool sourceUnitIsAir = false,
        bool skipSourceUnitType = false,
        bool isDanCuaNguoiChoi = false,
        bool isEleCrit = false)
    {
        var takenDmg = 0L;
        if (IsAlive() == false) return takenDmg;
        if (UnitName == "ChimBM") return takenDmg;

        if (dmg > 0)
        {
            if (xuyenInvulnerable == false && mInvulnerableTime > 0)
            {
//#if DEBUG
//                Debug.Log("ImmumTime " + dmg);
//#endif
                dmg = 0;
                //if (thongKe)
                {
                    if (QuanlyNguoichoi.Instance && this == QuanlyNguoichoi.Instance.PlayerUnit)
                    {
                        if (mInvulnerableTime > 3 && QuanlyNguoichoi.Instance.IsLevelClear == false)
                        {
                            // TODO: thoi gian bat tu qua dai`
                            QuanlyNguoichoi.Instance.AddThongKeST("BatTu", 0, mCurStat.HP, mCurStat.HP, mMaxHP);
                        }
                    }
                }
                return takenDmg;
            }

            if (playHitSound)
                PlayHitSound();
        }

        int addBaseDMG = 0;

        if (QuanlyNguoichoi.Instance && QuanlyNguoichoi.Instance.PlayerUnit == this)
        {
            if (this.GetStatusEff().IsHaveActiveEffect(EffectType.Linken) && xuyenLinken == false)
            {
                this.GetStatusEff().RemoveLinkenEffect();
                int buffCount = this.GetBuffCount(BuffType.LINKEN);
                if (buffCount > 0)
                {
                    //ScreenBattle.instance.DisplayTextHud("LINKEN", unit);
                    
                    var buffStat = QuanlyNguoichoi.Instance.GetBuffStatByType(BuffType.LINKEN);

                    timeReactiveLinken = buffStat.Params[buffCount - 1];
                }

                //if (thongKe)
                {
                    if (this == QuanlyNguoichoi.Instance.PlayerUnit)
                    {
                        QuanlyNguoichoi.Instance.AddThongKeST("Linken", 0, mCurStat.HP, mCurStat.HP, mMaxHP);
                    }
                }
                SetInvulnerableTime(0.3f);
                return takenDmg;
            }
            var battleData = QuanlyNguoichoi.Instance.battleData;
            if (battleData != null &&
                battleData.ListMods != null)
            {
                int def = 0;

                bool _srcIsBoss = sourceUnitIsBoss;
                bool _srcIsAir = sourceUnitIsAir;
                if (sourceUnit != null)
                {
                    _srcIsBoss = sourceUnit.IsBoss;
                    _srcIsAir = sourceUnit.IsAir;
                }


                foreach (var m in battleData.ListMods)
                {
                    if (m.Stat == EStatType.DEF_ON)
                    {
                        if (m.Target == "Boss" && _srcIsBoss)
                        {
                            def += (int)m.Val;
                        }
                        else
                        if (m.Target == "Creep" && _srcIsBoss == false)
                        {
                            def += (int)m.Val;
                        }
                        else
                        if (m.Target == "Air" && _srcIsAir)
                        {
                            def += (int)m.Val;
                        }
                        else
                        if (m.Target == "Ground" && _srcIsAir == false)
                        {
                            def += (int)m.Val;
                        }
                        else if (m.Target == "All")
                        {
                            def += (int)m.Val;
                        }
                    }
                }

                if (skipSourceUnitType)
                {
                    def = 0;
                }

                if (def > 0)
                {
                    dmg -= def * dmg / 100;
                    if (dmg < 0)
                    {
                        dmg = 0;
                    }
                }
            }
        }
        else if (sourceUnit != null && QuanlyNguoichoi.Instance && QuanlyNguoichoi.Instance.PlayerUnit == sourceUnit)
        {
            var battleData = QuanlyNguoichoi.Instance.battleData;
            if (battleData != null &&
                battleData.ListMods != null)
            {
                int buff = 0;
                foreach (var m in battleData.ListMods)
                {
                    if (m.Stat == EStatType.ATK_ON)
                    {
                        if (m.Target == "Boss" && sourceUnit != null && this.IsBoss)
                        {
                            buff += (int)m.Val;
                        }
                        else
                        if (m.Target == "Creep" && sourceUnit != null && this.IsBoss == false)
                        {
                            buff += (int)m.Val;
                        }
                        else
                        if (m.Target == "Air" && sourceUnit != null && this.IsAir)
                        {
                            buff += (int)m.Val;
                        }
                        else
                        if (m.Target == "Ground" && sourceUnit != null && this.IsAir == false)
                        {
                            buff += (int)m.Val;
                        }
                    }

                    if (isDanCuaNguoiChoi &&
                        m.Stat == EStatType.DMG_ON_FREEZE)
                    {
                        // duongrs changed
                        // apply dmg_on_freeze ngay tu hit ban dau tien neu co eff frozen_eff
                        if (GetStatusEff().IsHaveActiveEffect(EffectType.Frozen) ||
                            sourceUnit.GetBuffCount(BuffType.FROZEN_EFF) > 0)
                        {
                            if (displayCrit) // duongrs dmg_on_free apply ca CRIT neu co
                            {
                                addBaseDMG += ((int)m.Val) * sourceUnit.GetCurStat().CRIT_DMG / 100;
                            }
                            else
                            {
                                addBaseDMG += (int)m.Val;
                            }
                        }
                    }
                }
                if (buff > 0)
                {
                    dmg += buff * dmg / 100;
                }
            }
        }

        if (addBaseDMG > 0 && sourceUnit != null)
        {
            dmg += addBaseDMG * sourceUnit.GetCurStat().DMG / 100;
        }

        if (dmg > 0 && showHUD)
        {
            if (ScreenBattle.instance)
                ScreenBattle.instance.DisplayDmgHud(dmg, displayCrit, this, isEleCrit);
        }

        var nonCapDmg = dmg;
        if (dmg > mCurStat.HP) // cap dmg to use eff like lifeleech
        {
            dmg = mCurStat.HP;
        }
        long preMau = mCurStat.HP;
        mCurStat.HP -= dmg;
        STReceived += dmg;

        //if (thongKe)
        {
            if (QuanlyNguoichoi.Instance && this == QuanlyNguoichoi.Instance.PlayerUnit)
            {
                if (curLeech > 0)
                {
                    QuanlyNguoichoi.Instance.AddThongKeST("+Hut", curLeech, preMauLeech, curMauLeech, mMaxHP);
                    curLeech = 0;
                    preMauLeech = 0;
                    curMauLeech = 0;
                }
                QuanlyNguoichoi.Instance.AddThongKeST("N", nonCapDmg, preMau, mCurStat.HP, mMaxHP);
            }
            else if (QuanlyNguoichoi.Instance && sourceUnit && thongKe)
            {
                if (sourceUnit == QuanlyNguoichoi.Instance.PlayerUnit ||
                    sourceUnit.TeamID == QuanlyManchoi.PlayerTeam)
                {
                    QuanlyNguoichoi.Instance.AddTKSTNguoiChoi(nonCapDmg);
                }
            }
        }
        
        if (mCurStat.HP <= 0)
        {
            if (TeamID == QuanlyManchoi.EnemyTeam)
            {
                QuanlyNguoichoi.Instance.AddTKSTUnitStat(UnitName, OriginStat, IsBoss, IsAir, STReceived);
            }
        }

        if (sourceUnit &&
            sourceUnit.IsAlive() &&
            isHeadShot == false)
        {
            sourceUnit.LifeLeech(dmg);
        }

        if (dmg > 0)
        {
            if (ImmunityAfterTakeDMG > 0)
            {
                SetInvulnerableTime(ImmunityAfterTakeDMG);
            }
        }

        // hit eff
        if (QuanlyNguoichoi.Instance && QuanlyNguoichoi.Instance.PlayerUnit != this)
        {
            StartCoroutine(OnDamageEff());
        }

        OnChangedHP(dmg);

        DMGTakenLastFrame += dmg;
        OnUnitTakenDMG?.Invoke(dmg);

        if (mCurHPBar) mCurHPBar.UpdateHP();

        if (mCurStat.HP <= 0)
        {
            Life--;

            if (mKyNang)
            {
                mKyNang.OnUnitDie();
            }

            if (Life > 0)
            {
                var behaviour = GetComponent<BehaviorDesigner.Runtime.BehaviorTree>();
                if (behaviour)
                    behaviour.DisableBehavior();

                if (ScreenBattle.instance)
                {
                    ScreenBattle.instance.OnUnitDied(this);
                }
                foreach (var animator in mAnimators)
                {
                    if (animator)
                    {
                        animator.SetTrigger("die");
                    }
                }

                if (this.UnitName == "NecromancerSkeleton")
                {
                    Hiker.HikerUtils.DoAction(this, SkeletonReviveNow, 0.6f); 
                }
                else // PhuongTD : Main
                    Hiker.HikerUtils.DoAction(this, ReviveNow, 2f); // auto revive after 2 seconds
            }
            else
            {
                OnDie();
            }
        }

        if (QuanlyNguoichoi.Instance &&
            QuanlyNguoichoi.Instance.PlayerUnit == this &&
            GameClient.instance &&
            GameClient.instance.OfflineMode == false)
        {
            if (thongKe)
            {
                QuanlyNguoichoi.Instance.UpdatePlayerStat(true);
            }
            else if (mCurStat.HP <= 0)
            {
                QuanlyNguoichoi.Instance.UpdatePlayerStat(false);
            }
        }

        if (dmg > 0 && mCurStat.HP > 0 && sourceUnit != null)
        {
            OnGetDmgFromUnit(sourceUnit);
        }
        takenDmg = dmg;
        return takenDmg;
    }

    void OnGetDmgFromUnit(DonViChienDau unit)
    {
        UnitGayDmgGanNhat = unit;
    }

    public void OnPostDinhDan(long takenDmg)
    {
        OnUnitDinhDan?.Invoke(takenDmg);
    }

    KhongLoHoaModifier mKhongLoHoaModifier;
    public KhongLoHoaModifier GetKhongLoHoa()
    {
        return mKhongLoHoaModifier;
    }

    MaCaRongModifier mMaCaRongModifier;
    public MaCaRongModifier GetMaCaRongModifier()
    {
        return mMaCaRongModifier;
    }

    PopoModifier mPopoModifier;
    public PopoModifier GetPopoModifier()
    {
        return mPopoModifier;
    }

    FlashModifier mFlashModifier;
    public FlashModifier GetFlashModifier()
    {
        return mFlashModifier;
    }

    InfernalModifier mInfernalModifier;
    public InfernalModifier GetInfernalModifier()
    {
        return mInfernalModifier;
    }

    DeathrattleModifier mDeathrattleModifier;
    public DeathrattleModifier GetDeathrattleModifier()
    {
        return mDeathrattleModifier;
    }


    [Beebyte.Obfuscator.ObfuscateLiterals]
    public void ReviveNow()
    {
        foreach (var animator in mAnimators)
        {
            if (animator)
            {
                animator.SetTrigger("revive");
            }
        }

        Hiker.HikerUtils.DoAction(this, () =>
        {
            mIsInited = true;
            m_Collider.enabled = true;
            ScreenBattle.instance.DisplayTextHud(Localization.Get("ReviveLabel"), this);
            if (Life <= 0)
            {
                Life = 1;
            }
            STReceived = 0;
            mCurStat.HP = mMaxHP;

            if (QuanlyNguoichoi.Instance)
            {
                if (curLeech > 0)
                {
                    QuanlyNguoichoi.Instance.AddThongKeST("+Hut", curLeech, preMauLeech, curMauLeech, mMaxHP);
                    curLeech = 0;
                    preMauLeech = 0;
                    curMauLeech = 0;
                }
                QuanlyNguoichoi.Instance.AddThongKeST("+HoiSinh", mMaxHP, 0, mCurStat.HP, mMaxHP);
            }

            OnChangedHP(mMaxHP);
            if (ScreenBattle.instance)
            {
                ScreenBattle.instance.OnUnitRevive(this);
            }

            if (QuanlyNguoichoi.Instance &&
                QuanlyNguoichoi.Instance.PlayerUnit == this &&
                GameClient.instance &&
                GameClient.instance.OfflineMode == false)
            {
                QuanlyNguoichoi.Instance.UpdatePlayerStat();
            }

            var behaviour = GetComponent<BehaviorDesigner.Runtime.BehaviorTree>();
            behaviour.EnableBehavior();

            if (mCurHPBar) mCurHPBar.UpdateHP();

            // reactive linken when revive
            var linkenCount = GetBuffCount(BuffType.LINKEN);

            if (linkenCount > 0)
            {
                if (GetStatusEff().IsHaveActiveEffect(EffectType.Linken) == false)
                {
                    GetStatusEff().ApplyLinkenEffect();
                }
            }

            if (mKyNang)
            {
                mKyNang.OnUnitHoiSinh();
            }

        }, 0.25f);

        SetInvulnerableTime(2f);
    }

    [Beebyte.Obfuscator.ObfuscateLiterals]
    public void SkeletonReviveNow()
    {
        foreach (var animator in mAnimators)
        {
            if (animator)
            {
                animator.SetTrigger("revive");
            }
        }

        Hiker.HikerUtils.DoAction(this, () =>
        {
            mIsInited = true;
            m_Collider.enabled = true;

            mCurStat.HP = mMaxHP;

            //if (ScreenBattle.instance)
            //{
            //    ScreenBattle.instance.OnUnitRevive(this);
            //}
            
            var behaviour = GetComponent<BehaviorDesigner.Runtime.BehaviorTree>();
            behaviour.EnableBehavior();

            if (mCurHPBar) mCurHPBar.UpdateHP();
            
        }, 0.4f);

        SetInvulnerableTime(0.4f);
    }

    public void OnEvadeBullet()
    {
        SetInvulnerableTime(0.5f);
        ScreenBattle.instance.DisplayTextHud(Localization.Get("EvasionLabel"), this);
    }

    public void SetupVisualGO(GameObject go)
    {
        if (visualGO)
        {
            visualGO.SetActive(false);
        }

        if (mAnimators != null)
        {
            foreach (var anim in mAnimators)
            {
                if (anim && anim.gameObject.activeInHierarchy)
                {
                    anim.gameObject.SetActive(false);
                }
            }
        }

        visualGO = go;

        go.SetActive(true);
        mAnimators = visualGO.GetComponentsInChildren<Animator>();

        InitMeshVisual();
    }

    public void HideVisual()
    {
        //if (visualGO)
        //{
        //    visualGO.SetActive(false);
        //}
        //else
        {
            foreach (var anim in mAnimators)
            {
                anim.gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < HideWhenDie.Length; i++)
        {
            HideWhenDie[i].SetActive(false);
        }
    }

    void OnDie()
    {
        if (runingChargeEff)
        {
            runingChargeEff.gameObject.SetActive(false);
            ObjectPoolManager.Unspawn(runingChargeEff);
            runingChargeEff = null;
        }
        if (runingChargeMax)
        {
            runingChargeMax.gameObject.SetActive(false);
            ObjectPoolManager.Unspawn(runingChargeMax);
            runingChargeMax = null;
        }

        if (m_Collider)
            m_Collider.enabled = false;

        foreach (var animator in mAnimators)
        {
            if (animator)
            {
                animator.SetTrigger("die");
            }
        }

#if UNITY_EDITOR
        //Debug.Log(gameObject.name + " die");
#endif
        if (mChildren != null)
        {
            foreach (var unit in mChildren)
            {
                if (unit != this)
                {
                    unit.gameObject.SetActive(false);
                }
            }
        }

        var behaviour = GetComponent<BehaviorDesigner.Runtime.BehaviorTree>();
        if (behaviour)
            behaviour.DisableBehavior();

        var collisiionDmgs = GetComponentsInChildren<GayDamKhiVaCham>();
        foreach (var co in collisiionDmgs)
        {
            if (co)
            {
                co.gameObject.SetActive(false);
            }
        }

        if (shooter)
        {
            if (shooter.FireOnOwnerDie == true)
                shooter.FireAt(Vector3.zero);

            shooter.StopAllCoroutines();
        }

        if (shooter2)
        {
            if (shooter2.FireOnOwnerDie == true)
                shooter2.FireAt(Vector3.zero);

            shooter2.StopAllCoroutines();
        }

        if (shooter3)
        {
            if (shooter3.FireOnOwnerDie == true)
                shooter3.FireAt(Vector3.zero);

            shooter3.StopAllCoroutines();
        }

        if (shooter4)
        {
            if (shooter4.FireOnOwnerDie == true)
                shooter4.FireAt(Vector3.zero);

            shooter4.StopAllCoroutines();
        }

        for (int i = 0; i < shooterList.Count; i++)
        {
            if (shooterList[i].FireOnOwnerDie == true)
                shooterList[i].FireAt(Vector3.zero);
            shooterList[i].StopAllCoroutines();
        }

        if (mAgent)
        {
            if (mAgent.enabled && mAgent.isOnNavMesh)
            {
                mAgent.isStopped = true;
            }
            mAgent.enabled = false;
        }

        if (ScreenBattle.instance)
        {
            ScreenBattle.instance.OnUnitDied(this);
        }

        if( SpawnObjOnDie!=null && SpawnObjOnDieCount > 0)
        {
            for(int i=0;i< SpawnObjOnDieCount;i++)
            {
                var go = GameObject.Instantiate(SpawnObjOnDie);
                go.transform.position = this.transform.position;
                go.transform.rotation = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up);
                go.SetActive(true);
            }
        }

        QuanlyManchoi.instance.OnUnitDie(this);

        mIsInited = false;
        dieTime = 3f;

        if(isBossUnit)
        {
            GameObject impact_eff = ObjectPoolManager.Spawn("Particle/Other/BossDie");
            //impact_eff.transform.parent = unit.transform;
            impact_eff.transform.position = this.transform.position /*+ this.transform.forward * 0.5f*/;
            impact_eff.transform.rotation = this.transform.rotation;

            ObjectPoolManager.instance.AutoUnSpawn(impact_eff, 3);
        }
        else
        {
            GameObject impact_eff = ObjectPoolManager.Spawn("Particle/Other/MonsterDie");
            //impact_eff.transform.parent = unit.transform;
            impact_eff.transform.position = this.transform.position /*+ this.transform.forward * 0.5f*/;
            impact_eff.transform.rotation = this.transform.rotation;

            ObjectPoolManager.instance.AutoUnSpawn(impact_eff, 3);
        }

        if (mDeathrattleModifier)
        {
            mDeathrattleModifier.OnUnitDie();
        }

        // PhuongTD : destroy necromancer skeletond
        if (UnitName == "NecromancerSkeleton")
        {
            QuanlyManchoi.instance.DestroyEnemy(this);
        }
    }

    public int GetBuffCount(BuffType t)
    {
        int count = 0;
        for (int i = listBuffs.Count - 1; i >= 0; --i)
        {
            if (listBuffs[i].Type == t)
            {
                count++;
            }
        }
        return count;
    }

    void OnBodyScaleChange()
    {
        var countGiant = GetBuffCount(BuffType.GIANT);
        var countDwarf = GetBuffCount(BuffType.DWARF);
        var dif = countGiant - countDwarf;
        Vector3 finalScale = bodyScale;
        if (dif > 0)
        {
            var buff = QuanlyNguoichoi.Instance.GetBuffStatByType(BuffType.GIANT);
            finalScale = bodyScale * buff.Params[1] / 100f;
        }
        else if (dif < 0)
        {
            var buff = QuanlyNguoichoi.Instance.GetBuffStatByType(BuffType.DWARF);
            finalScale = bodyScale * buff.Params[1] / 100f;
        }

        transform.localScale = finalScale;
    }

    public void GetBuff(BuffStat buff)
    {
        int buffCount = GetBuffCount(buff.Type);
        if (buff.MaxCount > 0 && buffCount >= buff.MaxCount)
        {
#if UNITY_EDITOR
            Debug.Log("Max Buff " + buff.Type.ToString());
#endif
            return;
        }

        listBuffs.Add(buff);
        buffCount++;
        switch (buff.Type)
        {
            case BuffType.RUNNING_CHARGE:
                AddChargeEff(ChargeEff.RuningCharge, buff.Params[1] / 100f);
                break;
            case BuffType.GIANT:
                BuffAtkUp((int)buff.Params[0]);
                OnBodyScaleChange();
                break;
            case BuffType.DWARF:
                BuffCritUp((int)buff.Params[0]);
                OnBodyScaleChange();
                break;
            case BuffType.HEAL:
                GetHealByPercent((int)buff.Params[0], "+Heal");
                break;
            case BuffType.HP_UP:
                BuffHPUp((int)buff.Params[0]);
                break;
            case BuffType.ATK_UP:
            case BuffType.ATK_UP_SMALL:
                BuffAtkUp((int)buff.Params[0]);
                break;
            case BuffType.ATKSPD_UP:
            case BuffType.ATKSPD_UP_SMALL:
                BuffAtkSpdUp((int)buff.Params[0]);
                break;
            case BuffType.CRIT_UP:
            case BuffType.CRIT_UP_SMALL:
                BuffCritUp((int)buff.Params[0]);
                break;
            case BuffType.CRITDMG_UP:
                BuffCritDMGUp((int)buff.Params[0]);
                break;
            case BuffType.MULTI_SHOT:
                BuffMultiShot(buff, shooter);
                break;
            case BuffType.ADD_SHOT:
                BuffAddShot(buff, shooter);
                break;
            case BuffType.FLY:
                gameObject.layer = LayersMan.Team1_Flying;
                if (visualGO != null) visualGO.transform.localPosition = Vector3.up * 1.2f;
                //if (shooter != null && shooter.visualGO != null)
                //{
                //    shooter.visualGO.transform.localPosition += Vector3.up * 2f;
                //}
                break;
            case BuffType.LINKEN:
                if (buffCount > 0)
                {
                    BattleEffect battleEff = new BattleEffect(new EffectConfig
                    {
                        Type = EffectType.Linken,
                        Duration = 1000000
                    });
                    var buffStat = QuanlyNguoichoi.Instance.GetBuffStatByType(BuffType.LINKEN);
                    GetStatusEff().ApplyEffect(battleEff); // Apply Linken
                }
                break;
            case BuffType.EVASION:
                mCurStat.EVASION += (int)buff.Params[0];
                break;
            case BuffType.LIFE:
                Life++;
                break;
            case BuffType.ELECTRIC_FIELD:
                ElectricField.AddFieldToUnit(this);
                break;
            case BuffType.FLAME_FIELD:
                FlameField.AddFieldToUnit(this);
                break;
            case BuffType.FROZEN_FIELD:
                FrozenField.AddFieldToUnit(this);
                break;
            case BuffType.RAGE_ATK:
                CheckRageAtk();
                break;
            case BuffType.RAGE_ATKSPD:
                CheckRageAtkSpd();
                break;
            default:
                break;
        }
    }

    public void RemoveBuff(BuffType buffType)
    {
        bool removed = false;
        BuffStat removedBuff = new BuffStat();
        for (int i = listBuffs.Count - 1; i >= 0; --i)
        {
            var b = listBuffs[i];
            if (b.Type == buffType)
            {
                removedBuff = b;
                removed = true;
                listBuffs.RemoveAt(i);
                break;
            }
        }

        if (removed == false)
        {
            return;
        }

        int buffCount = GetBuffCount(buffType);

        switch (buffType)
        {
            case BuffType.RUNNING_CHARGE:
                if (buffCount == 0)
                {
                    RemoveChargeEff(ChargeEff.RuningCharge);
                }
                break;
            case BuffType.GIANT:
                DeBuffAtkUp((int)removedBuff.Params[0]);
                OnBodyScaleChange();
                break;
            case BuffType.DWARF:
                DeBuffCritUp((int)removedBuff.Params[0]);
                OnBodyScaleChange();
                break;
            case BuffType.HEAL:
                //GetHealByPercent(buff.Params[0]);
                break;
            case BuffType.HP_UP:
                DebuffHPUp((int)removedBuff.Params[0]);
                break;
            case BuffType.ATK_UP:
            case BuffType.ATK_UP_SMALL:
                DeBuffAtkUp((int)removedBuff.Params[0]);
                break;
            case BuffType.ATKSPD_UP:
            case BuffType.ATKSPD_UP_SMALL:
                DeBuffAtkSpdUp((int)removedBuff.Params[0]);
                break;
            case BuffType.CRIT_UP:
            case BuffType.CRIT_UP_SMALL:
                DeBuffCritUp((int)removedBuff.Params[0]);
                break;
            case BuffType.CRITDMG_UP:
                DeBuffCritDMGUp((int)removedBuff.Params[0]);
                break;
            case BuffType.MULTI_SHOT:
                DeBuffMultiShot(removedBuff, shooter);
                break;
            case BuffType.ADD_SHOT:
                DeBuffAddShot(removedBuff, shooter);
                break;
            case BuffType.FLY:
                gameObject.layer = LayersMan.GetDefaultHeroLayer(UnitName);
                if (visualGO != null) visualGO.transform.localPosition = Vector3.zero;
                //if (shooter != null && shooter.visualGO != null)
                //{
                //    shooter.visualGO.transform.localPosition -= Vector3.up * 2f;
                //}
                break;
            case BuffType.LINKEN:
                if (buffCount == 0)
                {
                    GetStatusEff().RemoveLinkenEffect(); // Apply Linken
                }
                break;
            case BuffType.EVASION:
                mCurStat.EVASION -= (int)removedBuff.Params[0];
                break;
            case BuffType.LIFE:
                Life--;
                break;
            case BuffType.ELECTRIC_FIELD:
                {
                    if (buffCount > 0)
                    {
                        ElectricField.AddFieldToUnit(this); // re init
                    }
                    else
                    {
                        var field = GetComponent<ElectricField>();
                        if (field)
                        {
                            field.enabled = false;
                            Destroy(field);
                        }
                    }
                }
                break;
            case BuffType.FLAME_FIELD:
                {
                    if (buffCount > 0)
                    {
                        FlameField.AddFieldToUnit(this); // re init
                    }
                    else
                    {
                        var field = GetComponent<FlameField>();
                        if (field)
                        {
                            field.enabled = false;
                            Destroy(field);
                        }
                    }
                }
                break;
            case BuffType.FROZEN_FIELD:
                {
                    if (buffCount > 0)
                    {
                        FrozenField.AddFieldToUnit(this); // re init
                    }
                    else
                    {
                        var field = GetComponent<FrozenField>();
                        if (field)
                        {
                            field.enabled = false;
                            Destroy(field);
                        }
                    }
                }
                break;
            case BuffType.RAGE_ATK:
                CheckRageAtk();
                break;
            case BuffType.RAGE_ATKSPD:
                CheckRageAtkSpd();
                break;
            default:
                break;
        }
    }

    public int RageBuffAtk { get; protected set; }
    public int RageBuffAtkSpd { get; protected set; }

    void CheckRageAtk()
    {
        var rageAtkCount = GetBuffCount(BuffType.RAGE_ATK);
        if (rageAtkCount > 0)
        {
            var buffStat = QuanlyNguoichoi.Instance.GetBuffStatByType(BuffType.RAGE_ATK);
            int hpPercent = (int)buffStat.Params[0];
            int curPercent = (int)(mCurStat.HP * 100 / mMaxHP);
            int dif = hpPercent - curPercent;

            var lastRageBuff = RageBuffAtk;

            if (dif > 0)
            {
                RageBuffAtk = dif * (int)buffStat.Params[1];

                //GetStatusEff().ApplyEffect(new BattleEffect(new EffectConfig
                //{
                //    Type = EffectType.RageAtk,
                //    Duration = 10000,
                //    Param1 = buffStat.Params[1]
                //}));
            }
            else
            {
                RageBuffAtk = 0;
                //GetStatusEff().RemoveRageAtk();
            }

            if (lastRageBuff != RageBuffAtk)
            {
                UpdateShooterStat();
            }
        }
        else if (RageBuffAtk != 0)
        {
            RageBuffAtk = 0;
            UpdateShooterStat();
        }
    }

    void CheckRageAtkSpd()
    {
        var rageAtkSpdCount = GetBuffCount(BuffType.RAGE_ATKSPD);
        if (rageAtkSpdCount > 0)
        {
            var buffStat = QuanlyNguoichoi.Instance.GetBuffStatByType(BuffType.RAGE_ATKSPD);
            int hpPercent = (int)buffStat.Params[0];
            int curPercent = (int)(mCurStat.HP * 100 / mMaxHP);
            int dif = hpPercent - curPercent;

            var lastRageBuff = RageBuffAtkSpd;

            if (dif > 0)
            {
                RageBuffAtkSpd = dif * (int)buffStat.Params[1];

                //GetStatusEff().ApplyEffect(new BattleEffect(new EffectConfig
                //{
                //    Type = EffectType.RageAtk,
                //    Duration = 10000,
                //    Param1 = buffStat.Params[1]
                //}));
            }
            else
            {
                RageBuffAtkSpd = 0;
                //GetStatusEff().RemoveRageAtk();
            }

            if (lastRageBuff != RageBuffAtkSpd)
            {
                UpdateShooterStat();
            }
            //if (mCurStat.HP * 100 <= mMaxHP * hpPercent)
            //{
            //    GetStatusEff().ApplyEffect(new BattleEffect(new EffectConfig
            //    {
            //        Type = EffectType.RageAtkSpd,
            //        Duration = 10000,
            //        Param1 = buffStat.Params[1]
            //    }));
            //}
            //else
            //{
            //    GetStatusEff().RemoveRageAtkSpd();
            //}
        }
        else if (RageBuffAtkSpd != 0)
        {
            RageBuffAtkSpd = 0;
            UpdateShooterStat();
        }
    }

    void GetHealByPercent(int percent, string nguon)
    {
        long healAmount = OriginStat.HP * percent / 100;
        RegenHP(healAmount, true, nguon);
    }

    public void DebuffHPUp(int percent)
    {
        if (percent <= 0) return;

        long ammount = OriginStat.HP * percent / 100;
        DecHPDown(ammount);
    }


    public void BuffHPUp(int percent)
    {
        if (percent <= 0) return;

        long ammount = OriginStat.HP * percent / 100;

        IncHPUp(ammount);
    }

    public void DecHPDown(long ammount)
    {
        if (ammount <= 0) return;

        long oldHP = mCurStat.HP;
        long oldMaxHP = mMaxHP;

        mMaxHP -= ammount;

        if (mCurStat.HP > mMaxHP)
        {
            mCurStat.HP = mMaxHP;
        }

        if (mCurHPBar != null)
        {
            mCurHPBar.UpdateHP();
        }
    }
    public void IncHPUp(long ammount)
    {
        if (ammount <= 0) return;

        long oldHP = mCurStat.HP;
        long oldMaxHP = mMaxHP;

        mMaxHP += ammount;
        mCurStat.HP += ammount;// oldHP * mMaxHP / oldMaxHP;

        if (mCurHPBar != null)
        {
            mCurHPBar.UpdateHP();
        }
    }

    public void IncATKUp(long ammount)
    {
        if (ammount <= 0) return;

        mCurStat.DMG += ammount;

        UpdateShooterStat();
    }

    public void DecATKDown(long ammount)
    {
        if (ammount <= 0) return;

        mCurStat.DMG -= ammount;
        if (mCurStat.DMG < 0)
        {
            mCurStat.DMG = 0;
        }

        UpdateShooterStat();
    }

    public void DeBuffAtkUp(int percent)
    {
        if (percent <= 0) return;

        long ammount = OriginStat.DMG * percent / 100;
        DecATKDown(ammount);
    }

    public void BuffAtkUp(int percent)
    {
        if (percent <= 0) return;

        long ammount = OriginStat.DMG * percent / 100;
        IncATKUp(ammount);
    }

    public void DeBuffAtkSpdUp(int percent)
    {
        if (percent <= 0) return;

        float ammount = OriginStat.ATK_SPD * percent / 100f;
        mCurStat.ATK_SPD -= ammount;

        if (mCurStat.ATK_SPD < 0.0001f) mCurStat.ATK_SPD = 0.0001f;

        UpdateShooterStat();
    }

    public void BuffAtkSpdUp(int percent)
    {
        if (percent <= 0) return;

        float ammount = OriginStat.ATK_SPD * percent / 100f;
        mCurStat.ATK_SPD += ammount;

        UpdateShooterStat();
    }

    void DeBuffCritUp(int percent)
    {
        if (percent <= 0) return;

        mCurStat.CRIT -= percent;
    }

    void BuffCritUp(int percent)
    {
        if (percent <= 0) return;

        mCurStat.CRIT += percent;
    }

    void DeBuffCritDMGUp(int percent)
    {
        if (percent <= 0) return;

        mCurStat.CRIT_DMG -= percent;
    }

    void BuffCritDMGUp(int percent)
    {
        if (percent <= 0) return;

        mCurStat.CRIT_DMG += percent;
    }

    //public void SlowProjSpd(float slowPercent)
    //{
    //    if (slowPercent > 0)
    //    {
    //        mCurStat.PROJ_SPD -= slowPercent * OriginStat.PROJ_SPD / 100;
    //    }
    //}
    //public void BuffProjSpd(float buffPercent)
    //{
    //    if (buffPercent > 0)
    //    {
    //        mCurStat.PROJ_SPD += buffPercent * OriginStat.PROJ_SPD / 100;
    //    }
    //}

    //public void SlowAtkSpd(float slowPercent)
    //{
    //    if (slowPercent > 0)
    //    {
    //        mCurStat.ATK_SPD -= slowPercent * OriginStat.PROJ_SPD / 100;
    //    }
    //}
    //public void BuffAtkSpd(float buffPercent)
    //{
    //    if (buffPercent > 0)
    //    {
    //        mCurStat.PROJ_SPD += buffPercent * OriginStat.PROJ_SPD / 100;
    //    }
    //}

    void DeBuffMultiShot(BuffStat buff, SungCls shooter)
    {
        int buffCount = GetBuffCount(buff.Type);
        var atkDownEff = shooter.gameObject.AddMissingComponent<AtkDownEffect>();

        if (buffCount > 0)
        {
            int atkDown = (int)buff.Params[buffCount - 1];
            
            atkDownEff.AddAtkDownEffect(new AtkDownEffData()
            {
                Type = buff.Type,
                Amount = atkDown
            });
        }
        else
        {
            atkDownEff.RemoveAtkDownEffect(buff.Type);
        }
    }


    void BuffMultiShot(BuffStat buff, SungCls shooter)
    {
        int buffCount = GetBuffCount(buff.Type);
        if (buffCount > 0)
        {
            int atkDown = (int)buff.Params[buffCount - 1];
            var atkDownEff = shooter.gameObject.AddMissingComponent<AtkDownEffect>();
            atkDownEff.AddAtkDownEffect(new AtkDownEffData()
            {
                Type = buff.Type,
                Amount = atkDown
            });
        }
    }

    void DeBuffAddShot(BuffStat buff, SungCls shooter)
    {
        int buffCount = GetBuffCount(buff.Type);
        var atkDownEff = shooter.gameObject.AddMissingComponent<AtkDownEffect>();

        if (buffCount > 0)
        {
            int atkDown = (int)buff.Params[buffCount - 1];

            atkDownEff.AddAtkDownEffect(new AtkDownEffData()
            {
                Type = buff.Type,
                Amount = atkDown
            });
        }
        else
        {
            atkDownEff.RemoveAtkDownEffect(buff.Type);
        }
    }

    void BuffAddShot(BuffStat buff, SungCls shooter)
    {
        int buffCount = GetBuffCount(buff.Type);
        if (buffCount > 0)
        {
            int atkDown = (int)buff.Params[buffCount - 1];
            var atkDownEff = shooter.gameObject.AddMissingComponent<AtkDownEffect>();
            atkDownEff.AddAtkDownEffect(new AtkDownEffData()
            {
                Type = buff.Type,
                Amount = atkDown
            });
        }
    }
}
