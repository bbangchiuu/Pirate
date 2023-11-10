using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public class SungCls : MonoBehaviour
{
    #region SerializeField
    DonViChienDau unit;
    [SerializeField]
    GameObject visualGO;
    [SerializeField]
    SatThuongDT danPrefab;
    [SerializeField]
    Transform[] barrels;
    [SerializeField]
    Float atkSpd = 1;
    [SerializeField]
    Int64 dmg = 10;
    [SerializeField]
    Float projSpd = 10;
    [SerializeField]
    Float aimSpd = 360f;
    [SerializeField]
    Float knockBackDis = 0;
    [SerializeField]
    Float delayActiveDmg = 0;
    [SerializeField]
    Float attackAnimTime = 1f;
    [SerializeField]
    Bool shootAllBarrels = true;
    [SerializeField]
    Bool isMeleeWeapon = false;
    [SerializeField]
    Bool instantiateDanMelee = false;
    [SerializeField]
    ParticleSystem FlashObject = null;
    [SerializeField]
    Transform FlashPos = null;

    [SerializeField]
    Int32[] shootAllBarrelsIgnoreBarrelList;

    [SerializeField]
    bool CanApplyKhongLoModifier = true;
    #endregion

    public WeaponStatConfig weaponCfg { get; set; }
    public bool ShootAllBarrel { get { return shootAllBarrels; } set { shootAllBarrels = value; } }
    public bool isMelee { get { return isMeleeWeapon; } set { isMeleeWeapon = value; } }
    public long DMG { get { return dmg; } set { dmg = value; } }
    public float AtkSpd { get { return atkSpd; } set { atkSpd = value; } }
    public float ProjSpd { get { return projSpd; } set { projSpd = value; } }
    public float CRIT;
    public float CRIT_DMG;
    public float AimRotationSpd { get { return aimSpd; } set { aimSpd = value; } }
    public float KnockBackDistance { get { return knockBackDis; } set { knockBackDis = value; } }
    public float DelayActiveDamage { get { return delayActiveDmg; } set { delayActiveDmg = value; } }
    public float AttackAnimTime { get { return attackAnimTime; } set { attackAnimTime = value; } }

    /// <summary>
    /// Save origin atkspd setup by GD in prefab
    /// </summary>
    public Float OriginAtkSpd { get; private set; }
    public Transform[] Barels { get { return barrels; } }
    public void SetBarels(Transform[] _barrels)
    {
        barrels = _barrels;
    }

    public float AtkSpdAnimScale
    {
        get
        {
            var atkTime = 1f / AtkSpd;
            if (atkTime < AttackAnimTime) // only speed up anim
            {
                return AttackAnimTime / atkTime;
            }
            else
            {
                if (unit.TimeScale < 1f)
                {
                    return unit.TimeScale;
                }
                else
                {
                    return 1;
                }
            }
        }
    }
    public float DelayStartProjectile
    {
        get
        {
            return DelayActiveDamage > 0 ?
                Mathf.Max(0.1f, AtkSpdAnimScale > 1 ? (float)(DelayActiveDamage / AtkSpdAnimScale) : (float)DelayActiveDamage) :
                0;
        }
    }

    private int NextBarrelIdx = 0;
    Float mCoolDown = -10f;
    public bool DelayFireSound = false;

    public bool IgnoreAtkDown = false;

    public bool FireOnOwnerDie = false;
    public bool InitOnAwake = true;

    AudioSource FireSource = null;

    bool getFired = false;

    uint mBarelMask = 0;
    public void SetBarelMask(uint mask)
    {
        mBarelMask = mask;
    }

    private void Awake()
    {
        if (InitOnAwake)
        {
            Init();
        }

        Hiker.HikerUtils.DoAction(this, () =>
        {
            if (QuanlyNguoichoi.Instance != null &&
                unit != QuanlyNguoichoi.Instance.PlayerUnit &&
                unit.TeamID == QuanlyManchoi.EnemyTeam &&
                isMelee == false)
            {
                QuanlyNguoichoi.Instance.TKSungCount();
                QuanlyNguoichoi.Instance.AddTKSTNguoiChoiSCount();
            }
        },
        6f);
    }

    public void Init()
    {
        if (unit == null)
        {
            unit = GetComponentInParent<DonViChienDau>();
        }
        OriginAtkSpd = atkSpd;

        if (FlashObject != null)
        {
            FlashObject.gameObject.SetActive(false);
        }
        mCoolDown = -10f;
        InitFireSoundSource();

        getFired = false;
    }

    void InitFireSoundSource()
    {
        if (FireSource == null)
            FireSource = gameObject.AddComponent<AudioSource>();

        if (QuanlyNguoichoi.Instance && QuanlyNguoichoi.Instance.PlayerUnit == unit)
        {
            FireSource.spatialBlend = 0f;
        }
        else
        {
            FireSource.spatialBlend = 1f;
            FireSource.rolloffMode = AudioRolloffMode.Linear;
            FireSource.minDistance = 4f;
            FireSource.maxDistance = 40f;
            FireSource.volume = 0.7f;
        }
    }

    public void SetFireClip(string fire_sound_name)
    {
        if (FireSource == null)
            InitFireSoundSource();

        if (fire_sound_name == null || fire_sound_name == "")
            return;

        string fire_sound_path = "Sound/SFX/Battle/" + fire_sound_name;

        AudioClip clip = Resources.Load(fire_sound_path) as AudioClip;

        if (clip != null)
            FireSource.clip = clip;
    }

    // Start is called before the first frame update
    void Start()
    {
        mTimeSinceLastFire = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (mCoolDown > -10f)
        {
            mCoolDown -= Time.deltaTime;
        }

        mTimeSinceLastFire += Time.deltaTime;
        //StateHandle(Time.deltaTime);
    }

    float mTimeSinceLastFire = 0;
    public float TimeSceneLastFire { get { return mTimeSinceLastFire; } }

    public void StartCoolDown()
    {
        var atkTime = 1f / AtkSpd;
        //mCoolDown += atkTime;
        mCoolDown = atkTime;
    }

    public bool FireAt(Vector3 targetPos)
    {
        if(FireOnOwnerDie==false)
            if (unit && unit.IsAlive() == false) return false;

        if(weaponCfg != null)
        {
            if(Vector3.Distance(transform.position, targetPos) > weaponCfg.ATK_RANGE)
            {
                return false;
            }
        }
        if (mCoolDown <= 0)
        {
            if (isMelee == false || instantiateDanMelee)
            {
                //var proj = Instantiate(danPrefab);

                var trans = transform;
                if (barrels != null && barrels.Length > 0)
                {
                    if (ShootAllBarrel)
                    {
                        if (shootAllBarrelsIgnoreBarrelList.Length <= 0)
                        {
                            for (int i = 0; i < barrels.Length; ++i)
                            {
                                trans = barrels[i];

                                if (mBarelMask != 0)
                                {
                                    if (((1u << i) & mBarelMask) != 0)
                                    {
                                        continue;
                                    }
                                }

                                var barrelTrans = trans;
                                if (DelayStartProjectile > 0)
                                {
                                    Hiker.HikerUtils.DoAction(this, () => FireByTrans(barrelTrans, targetPos), DelayStartProjectile);
                                }
                                else
                                {
                                    FireByTrans(trans, targetPos);
                                }
                            }
                        }
                        else
                        {
                            int ignore_barrel = shootAllBarrelsIgnoreBarrelList[Random.Range(0, shootAllBarrelsIgnoreBarrelList.Length)];

                            for (int i = 0; i < barrels.Length; ++i)
                            {
                                if (i == ignore_barrel) 
                                    continue;

                                trans = barrels[i];
                                var barrelTrans = trans;
                                if (DelayStartProjectile > 0)
                                {
                                    Hiker.HikerUtils.DoAction(this, () => FireByTrans(barrelTrans, targetPos), DelayStartProjectile);
                                }
                                else
                                {
                                    FireByTrans(trans, targetPos);
                                }
                            }
                        }
                    }
                    else
                    {
                        int barrel_idx = NextBarrelIdx % barrels.Length;
                        NextBarrelIdx++;
                        trans = barrels[barrel_idx];
                        var barrelTrans = trans;
                        if (DelayStartProjectile > 0)
                        {
                            Hiker.HikerUtils.DoAction(this, () => FireByTrans(barrelTrans, targetPos), DelayStartProjectile);
                        }
                        else
                        {
                            FireByTrans(trans, targetPos);
                        }
                    }
                }
                else
                {
                    var barrelTrans = trans;
                    if (DelayStartProjectile > 0)
                    {
                        Hiker.HikerUtils.DoAction(this, () => FireByTrans(barrelTrans, targetPos), DelayStartProjectile);
                    }
                    else
                    {
                        FireByTrans(trans, targetPos);
                    }
                }
            }
            else // Melee
            {
                if (DelayStartProjectile > 0)
                {
                    Hiker.HikerUtils.DoAction(this, () => ActiveMeleeDmgObject(targetPos), DelayStartProjectile);
                }
                else
                {
                    ActiveMeleeDmgObject(targetPos);
                }

                var __atkTime = 1f / AtkSpd;

                Hiker.HikerUtils.DoAction(this, () => DeactiveMeleeDmgObject(), __atkTime * 0.9f);
                
            }

            // Sound
            if (FireSource != null)
            {
                if (DelayFireSound)
                {
                    //FireSource.Play();
                    Hiker.HikerUtils.DoAction(this, () => PlayFireSound(), DelayStartProjectile);
                }
                else
                {
                    PlayFireSound();
                }
            }

            var atkTime = 1f / AtkSpd;
            //mCoolDown += atkTime;
            mCoolDown = atkTime;
            //Debug.Log("mCoolDown= " + mCoolDown);

            if (getFired == false)
            {
                getFired = true;
                // Thong ke ban sung
                if (QuanlyNguoichoi.Instance != null &&
                    unit != QuanlyNguoichoi.Instance.PlayerUnit &&
                    unit.TeamID == QuanlyManchoi.EnemyTeam &&
                    isMelee == false)
                {
                    QuanlyNguoichoi.Instance.AddTKSTNguoiChoiSBan();
                }
            }

            return true;
        }
        return false;
    }

    void PlayFireSound()
    {
        FireSource.Play();
    }

    void ActiveMeleeDmgObject(Vector3 targetPos)
    {
        ActiveDmgObj(danPrefab, targetPos);
    }

    void DeactiveMeleeDmgObject()
    {
        danPrefab.DeactiveDan();
    }

    void ActiveDmgObj(SatThuongDT dmgObj, Vector3 targetPos)
    {
        var dmg = DMG;

        mTimeSinceLastFire = 0;

        if (unit != null)
        {
            if (IgnoreAtkDown == false)
            {
                var atkDown = gameObject.GetComponent<AtkDownEffect>();
                if (atkDown != null)
                {
                    dmg = DMG * atkDown.GetTotalAtkDownPercent() / 100;
                }
            }

            //var crit = unit.GetStat().CRIT;
            //var critDmg = unit.GetStat().CRIT_DMG;
            dmgObj.CRIT = (int)CRIT;
            dmgObj.CRIT_DMG = (int)CRIT_DMG;
            dmgObj.HEADSHOT = 0;
            int headShotCount = unit.GetBuffCount(BuffType.HEAD_SHOT);
            if (headShotCount > 0)
            {
                var buffStat = QuanlyNguoichoi.Instance.GetBuffStatByType(BuffType.HEAD_SHOT);
                dmgObj.HEADSHOT = (int)buffStat.Params[headShotCount - 1];
            }
//            if (crit > 0)
//            {
//                if (Random.Range(0, 100) < crit)
//                {
//                    // have crit
//                    dmg = dmg * critDmg / 100;

////#if UNITY_EDITOR
////                    Debug.Log("Have crit " + dmg);
////#endif
//                }
//            }
        }

        float projSpd = ProjSpd;
        if (unit != QuanlyManchoi.instance.PlayerUnit)
        {
            var player = QuanlyManchoi.instance.PlayerUnit;
            if (player != null && player.GetBuffCount(BuffType.SLOW_PROJECTILE) > 0)
            {
                var cfg = QuanlyNguoichoi.Instance.GetBuffStatByType(BuffType.SLOW_PROJECTILE);
                projSpd = ProjSpd * (1f - cfg.Params[0] / 100f);
            }
        }

        dmgObj.DeactiveRuningCharge();
        if (unit.HaveChargeEff(ChargeEff.RuningCharge))
        {
            var runingChargeBuff = unit.GetBuffCount(BuffType.RUNNING_CHARGE);
            if (runingChargeBuff > 0)
            {
                var chargeRuning = unit.GetChargeEff(ChargeEff.RuningCharge);
                //if (chargeRuning.CountStat >= chargeRuning.MaxCount) // full charge
                if (chargeRuning.CountStat > 0)
                {
                    var rate = chargeRuning.CountStat / chargeRuning.MaxCount;
                    var buffStat = QuanlyNguoichoi.Instance.GetBuffStatByType(BuffType.RUNNING_CHARGE);
                    dmg += (int)(dmg * rate * buffStat.Params[runingChargeBuff + 1] / 100f);
                    dmgObj.ActiveRuningCharge(chargeRuning.CountStat >= chargeRuning.MaxCount);
                }
            }
        }

        dmgObj.SourceUnit = unit;
        dmgObj.sungCls = this;
        dmgObj.ActiveDan(projSpd, dmg, targetPos);
        var khongLo = unit.GetKhongLoHoa();

        DanTo danTo = dmgObj.GetComponent<DanTo>();

        if (danTo == null)
        {
            danTo = dmgObj.gameObject.AddComponent<DanTo>();
            danTo.InitOriginSize(dmgObj.transform.localScale);
        }

        if (khongLo && CanApplyKhongLoModifier && isMelee == false)
        {
            dmgObj.ScaleRate = khongLo.GetTileKhongLo();

            danTo.ApplyScale(new DanToSrc { Src = 1, Scale = khongLo.GetTileKhongLo() });
            //dmgObj.transform.localScale = danPrefab.transform.lossyScale * dmgObj.ScaleRate;
        }
        else
        {
            if (danTo != null)
            {
                danTo.UnApplyScale(1);
            }

            //dmgObj.transform.localScale = danPrefab.transform.lossyScale;
            dmgObj.ScaleRate = 1f;
        }

        CheckDamageEff(dmgObj);
    }

    public void InitForMainPlayer(GameObject flash)
    {
        ParticleSystem flash_eff = flash.GetComponent<ParticleSystem>();
        FlashObject = flash_eff;
    }

    void CheckDamageEff(SatThuongDT proj)
    {
        proj.listEffect.Clear();
        if(weaponCfg != null)
        {
            var dan = proj as DanBay;

            var buffFrozenEff = weaponCfg.GetBuff(BuffType.FROZEN_EFF);
            if(buffFrozenEff.Type == BuffType.FROZEN_EFF)
            {
                var battleEff = new EffectConfig
                {
                    Type = EffectType.Frozen,
                    Param1 = buffFrozenEff.Params[0], // slow percent
                    Duration = buffFrozenEff.Params[1],
                };
                proj.listEffect.Add(battleEff);
            }

            var buffTrackingBullet = weaponCfg.GetBuff(BuffType.TRACKING_BULLET);
            if (buffTrackingBullet.Type == BuffType.TRACKING_BULLET)
            {
                if (unit.LastTarget != null)
                {
                    var targetUnit = unit.LastTarget.gameObject.GetComponent<DonViChienDau>();
                    if (targetUnit)
                    {
                        dan.AddDauDanTamNhiet(targetUnit, buffTrackingBullet);
                    }
                }
            }
            else
            {
                dan.DisableDanTamNhiet();
            }

            var bigBullet = weaponCfg.GetBuff(BuffType.BIG_BULLET);
            var danTo = proj.gameObject.GetComponent<DanTo>();
            if (bigBullet.Type == BuffType.BIG_BULLET)
            {
                if (danTo == null)
                {
                    danTo = proj.gameObject.AddComponent<DanTo>();
                    danTo.InitOriginSize(proj.transform.localScale);
                }

                float scale = bigBullet.Params[0];
                danTo.ApplyScale(new DanToSrc { Src = 2, Scale = scale });
                dan.OverrideDmg(dan.Damage + dan.Damage * (int)bigBullet.Params[1] / 100);
            }
            else
            {
                if (danTo != null)
                {
                    danTo.UnApplyScale(2);
                }
            }

            var bigBulletOverTime = weaponCfg.GetBuff(BuffType.BIG_BULLET_OVER_TIME);
            if (bigBulletOverTime.Type == BuffType.BIG_BULLET_OVER_TIME)
            {
                if (danTo == null)
                {
                    danTo = proj.gameObject.AddComponent<DanTo>();
                    danTo.InitOriginSize(proj.transform.localScale);
                }

                float scale = bigBulletOverTime.Params[0];
                danTo.ApplyScaleTheoTime(scale);
            }
            else
            {
                if (danTo != null)
                {
                    danTo.UnApplyScale(3);
                }
            }
        }

        int eleCritBuffCount = 0;
        if(unit == QuanlyNguoichoi.Instance.PlayerUnit)
        {
            eleCritBuffCount = QuanlyNguoichoi.Instance.PlayerUnit.GetBuffCount(BuffType.ELEMENT_CRIT);
        }
        var electTricCount = unit.GetBuffCount(BuffType.ELECTRIC_EFF);
        if (electTricCount > 0)
        {
            var buffstat = QuanlyNguoichoi.Instance.GetBuffStatByType(BuffType.ELECTRIC_EFF);
            var randomRate = buffstat.Params[0];
            if (randomRate > 0 && Random.Range(0, 100) < randomRate)
            {
                var dmg = Mathf.CeilToInt(unit.GetCurDPS() * unit.GetElementDMGRate() * buffstat.Params[1] / 100);
                var battleEff = new EffectConfig
                {
                    Type = EffectType.Electric,
                    Damage = dmg,
                    Param1 = buffstat.Params[2], // count unit get
                    Param2 = buffstat.Params[3], // range unit lan
                    Duration = 0.6f,
                };
                if(eleCritBuffCount > 0)
                {
                    UnitStat stats = QuanlyNguoichoi.Instance.PlayerUnit.GetCurStat();
                    battleEff.Crit = stats.CRIT;
                    battleEff.CritDMG = stats.CRIT_DMG;
                    battleEff.isEleCrit = true;
                }

                proj.listEffect.Add(battleEff);
#if DEBUG
                Debug.Log("Have Effect " + battleEff.Type.ToString());
#endif
            }
        }

        var flameCount = unit.GetBuffCount(BuffType.FLAME_EFF);
        if (flameCount > 0)
        {
            var buffstat = QuanlyNguoichoi.Instance.GetBuffStatByType(BuffType.FLAME_EFF);
            var battleEff = new EffectConfig
            {
                Type = EffectType.Flame,
                Damage = Mathf.CeilToInt(unit.GetCurStat().DMG * unit.GetElementDMGRate() * buffstat.Params[0] / 100),
                Param1 = buffstat.Params[1], // tick dot
                Duration = buffstat.Params[2],
            };

            if (eleCritBuffCount > 0)
            {
                UnitStat stats = QuanlyNguoichoi.Instance.PlayerUnit.GetCurStat();
                battleEff.Crit = stats.CRIT;
                battleEff.CritDMG = stats.CRIT_DMG;
                battleEff.isEleCrit = true;
            }

            proj.listEffect.Add(battleEff);
        }

        //var frozenCount = unit.GetBuffCount(BuffType.FROZEN_EFF);
        //if (frozenCount > 0)
        //{
        //    var buffstat = QuanlyNguoichoi.Instance.GetBuffStatByType(BuffType.FROZEN_EFF);
        //    var battleEff = new EffectConfig
        //    {
        //        Type = EffectType.Frozen,
        //        Param1 = buffstat.Params[0], // slow percent
        //        Duration = buffstat.Params[1],
        //    };

        //    if (eleCritBuffCount > 0)
        //    {
        //        UnitStat stats = QuanlyNguoichoi.Instance.PlayerUnit.GetCurStat();
        //        battleEff.Crit = stats.CRIT;
        //        battleEff.CritDMG = stats.CRIT_DMG;
        //        battleEff.isEleCrit = true;

        //        battleEff.Damage = Mathf.CeilToInt(unit.GetCurStat().DMG * unit.GetElementDMGRate() * buffstat.Params[2] / 100);
        //    }

        //    proj.listEffect.Add(battleEff);
        //}

        proj.DeactiveSkillProjEff();
        if (unit.GetKyNang())
        {
            var tk = unit.GetKyNang().GetTuyetKy(0);
            if (tk != null)
            {
                var tkEff = tk.GetEffectFromSkill();
                if (tk is MagicianSkill)
                {
                    var magic = tk as MagicianSkill;
                    var projVisual = magic.GetVisualPrefab();
                    if (projVisual)
                    {
                        proj.ActiveSkillProjEff(projVisual);
                    }
                }
#if DEBUG
                if (tkEff != null) Debug.Log("Effect " + tkEff.Type.ToString());
#endif
                if (tkEff != null && proj.listEffect.Exists(e => e.Type == tkEff.Type) == false)
                {
#if DEBUG
                    Debug.Log("Add Effect " + tkEff.Type.ToString());
#endif
                    proj.listEffect.Add(tkEff.Clone());
                }
            }
        }

        if (proj is DanBay)
        {
            var dan = proj as DanBay;
            //var trackingBullet = unit.GetBuffCount(BuffType.TRACKING_BULLET);

            //if (trackingBullet > 0)
            //{
            //    var buffStat = QuanlyNguoichoi.Instance.GetBuffStatByType(BuffType.TRACKING_BULLET);
            //    if (unit.LastTarget != null)
            //    {
            //        var targetUnit = unit.LastTarget.gameObject.GetComponent<DonViChienDau>();
            //        if (targetUnit)
            //        {
            //            dan.AddDauDanTamNhiet(targetUnit);
            //        }
            //    }

            //    if (unit == QuanlyNguoichoi.Instance.PlayerUnit)
            //    {
            //        if (unit.SungPhu != null && unit.SungPhu.sung1 != null)
            //        {
            //            if (this != unit.shooter && this != unit.SungPhu.sung1)
            //            {
            //                dan.OverrideSpd(dan.Speed * buffStat.Params[3] / 100);
            //                var dmg = dan.Damage * buffStat.Params[4] / 100;
            //                dan.OverrideDmg((dmg > 1 || dan.Damage < 1) ? dmg : 1); // minimum dmg = 1
            //            }
            //        }
            //        else if (this != unit.shooter)
            //        {
            //            dan.OverrideSpd(dan.Speed * buffStat.Params[3] / 100);
            //            var dmg = dan.Damage * buffStat.Params[4] / 100;
            //            dan.OverrideDmg((dmg > 1 || dan.Damage < 1) ? dmg : 1); // minimum dmg = 1
            //        }
            //    }
            //}
            //else
            //{
            //    dan.DisableDanTamNhiet();
            //}

            //var bigBullet = unit.GetBuffCount(BuffType.BIG_BULLET);
            //var danTo = proj.gameObject.GetComponent<DanTo>();
            //if (bigBullet > 0)
            //{
            //    var buffStat = QuanlyNguoichoi.Instance.GetBuffStatByType(BuffType.BIG_BULLET);
            //    if (danTo == null)
            //    {
            //        danTo = proj.gameObject.AddComponent<DanTo>();
            //        danTo.InitOriginSize(proj.transform.localScale);
            //    }

            //    float scale = buffStat.Params[0] / 100f;
            //    danTo.ApplyScale(new DanToSrc { Src = 2, Scale = scale });
                
            //    dan.OverrideDmg(dan.Damage + dan.Damage * buffStat.Params[1] / 100);
            //}
            //else 
            //{
            //    if (danTo != null)
            //    {
            //        danTo.UnApplyScale(2);
            //    }
            //}
        }
    }

    public void SetFlashPos(Transform trans)
    {
        FlashPos = trans;
    }

    void FireShotAOE(Transform trans, Vector3 targetPos, Vector3 displacement)
    {
        string vk_proj_name = weaponCfg != null ? weaponCfg.PROJ_N : QuanlyNguoichoi.Instance.VKName;
        var danPrefab = Resources.Load<GameObject>("Particle/PlayerProjectile/" + vk_proj_name);

        var wukongObj = ObjectPoolManager.Spawn(danPrefab, trans.position + displacement, Quaternion.identity);
        wukongObj.transform.eulerAngles = new Vector3(0, trans.eulerAngles.y, 0);
        wukongObj.transform.position = trans.position + displacement;
        //if (target != null)
        //{
        //    var dif = target.position - Unit.transform.position;
        //    dif.y = 0;
        //    if (dif.sqrMagnitude > Vector3.kEpsilonNormalSqrt)
        //    {
        //        wukongObj.transform.forward = dif.normalized;
        //    }
        //}
        wukongObj.transform.localScale = Vector3.one;

        var st = wukongObj.GetComponentInChildren<SatThuongDT>();
        //var dmgScale = ConfigManager.GetHeroSkillParams(TKName, 1);
        //var listMods = QuanlyNguoichoi.Instance.GetListRuntimeStatMods();
        //if (listMods != null)
        //{
        //    var mod = listMods.Find(e => e.Stat == EStatType.HEROPLUS && e.Target == "Wukong");
        //    if (mod != null && mod.Val > 0)
        //    {
        //        dmgScale = (int)Mathf.Round((float)mod.Val);

        //        var scaleObj = ConfigManager.GetWukongIllusionScale();
        //        if (scaleObj > 0)
        //        {
        //            wukongObj.transform.localScale = Vector3.one * (scaleObj / 100f);
        //        }
        //    }
        //}
        st.ActiveDan(0, DMG, Vector3.zero);
        if (KnockBackDistance > 0)
        {
            st.KnockBackDistance = KnockBackDistance;
        }
    }
    SatThuongDT FireShot(Transform trans, Vector3 targetPos, Vector3 displacement)
    {
        if (ProjSpd <= 0)
        {
            if (weaponCfg != null && weaponCfg.CheckHaveBuff(BuffType.SHOT_AOE))
            {
                FireShotAOE(trans, targetPos, displacement);
            }
            return null;
        }
        if (danPrefab == null) Debug.LogFormat("[{0}]Dan prefab is null", unit.UnitName);

        GameObject projObj = null;

        if (unit == QuanlyNguoichoi.Instance.PlayerUnit 
            //&& (this == unit.shooter || this == unit.shooter2 || this == unit.shooter3 || this == unit.shooter4)
            )
        {
            string vk_proj_name = weaponCfg != null ? weaponCfg.PROJ_N : QuanlyNguoichoi.Instance.VKName;
            danPrefab = Resources.Load<SatThuongDT>("Particle/PlayerProjectile/" + vk_proj_name);
            projObj = ObjectPoolManager.Spawn(danPrefab.gameObject, trans.position + displacement, Quaternion.identity);
        }
        else
        {
            projObj = ObjectPoolManager.Spawn(danPrefab.gameObject, trans.position + displacement, Quaternion.identity);
        }

        var proj = projObj.GetComponent<SatThuongDT>(); // Instantiate(danPrefab);
        proj.transform.eulerAngles = new Vector3(0, trans.eulerAngles.y, 0);
        proj.transform.position = trans.position + displacement;
        proj.SourceSungCls = this;

        ActiveDmgObj(proj, targetPos);

        if (KnockBackDistance > 0)
        {
            proj.KnockBackDistance = KnockBackDistance;
        }

        proj.gameObject.SetActive(true);

        if (unit != null && proj is DanBay)
        {
            if (weaponCfg != null)
            {
                var buffRicochet = weaponCfg.GetBuff(BuffType.RICOCHET);
                if (buffRicochet.Type == BuffType.RICOCHET)
                {
                    int countRicoChet = (int)buffRicochet.Params[1];
                    float nhayRange = buffRicochet.Params[2];
                    int dmgPercent = (int)buffRicochet.Params[0];
                    var danBay = proj as DanBay;
                    danBay.AddDanNhay(countRicoChet, nhayRange, dmgPercent);
                }

                var buffPiecing = weaponCfg.GetBuff(BuffType.PIERCING);
                if (buffPiecing.Type == BuffType.PIERCING)
                {
                    var p = proj as DanBay;
                    if (p != null)
                    {
                        p.PiercingCount = (int)buffPiecing.Params[0];
                        p.PiercingDMG = (int)buffPiecing.Params[1];
                    }
                }
                else
                {
                    var p = proj as DanBay;
                    if (p != null)
                    {
                        p.PiercingCount = 0;
                        p.PiercingDMG = 0;
                    }
                }

                var buffAOE = weaponCfg.GetBuff(BuffType.AOE);
                if (buffAOE.Type == BuffType.AOE)
                {
                    var danBay = proj as DanBay;
                    danBay.RangeNo = buffAOE.Params[0];
                }
                else
                {
                    var danBay = proj as DanBay;
                    danBay.RangeNo = 0;
                }
            }

            //int countRicoChet = unit.GetBuffCount(BuffType.RICOCHET);
            //var danBay = proj as DanBay;
            //danBay.AddDanNhay(countRicoChet);

            //if (unit.GetBuffCount(BuffType.PIERCING) > 0)
            //{
            //    var cfg = QuanlyNguoichoi.Instance.GetBuffStatByType(BuffType.PIERCING);
            //    var p = proj as DanBay;
            //    if (p != null)
            //    {
            //        p.PiercingCount = cfg.Params[0];
            //        p.PiercingDMG = cfg.Params[1];
            //    }
            //}
            //else
            //{
            //    var p = proj as DanBay;
            //    if (p != null)
            //    {
            //        p.PiercingCount = 0;
            //        p.PiercingDMG = 0;
            //    }
            //}

            if (unit.GetBuffCount(BuffType.BOUNCING) > 0)
            {
                var cfg = QuanlyNguoichoi.Instance.GetBuffStatByType(BuffType.BOUNCING);
                var p = proj as DanBay;
                if (p != null)
                {
                    p.ReflectCount = (int)cfg.Params[0];
                    p.ReflectDMG = (int)cfg.Params[1];
                    p.ResetReflectCounter();
                }
            }
            //else if (unit == QuanlyNguoichoi.Instance.PlayerUnit)
            //{
            //    // giu nguyen cac tham so duoc setup trong prefab
            //    var p = proj as DanBay;
            //    if (p != null)
            //    {
            //        p.ReflectCount = 0;
            //        p.ReflectDMG = 0;
            //        p.ResetReflectCounter();
            //    }
            //}
        }

        if(FlashObject!=null)
        {
            FlashObject.gameObject.SetActive(true);
            FlashObject.Clear(true);
            if (FlashPos)
            {
                FlashObject.transform.position = FlashPos.position;
                FlashObject.transform.rotation = FlashPos.rotation;
            }
            else if(ShootAllBarrel==false && Barels.Length >1 )
            {
                FlashObject.transform.position = trans.position;
                //FlashObject.transform.rotation = trans.rotation;
            }


            FlashObject.Play(true);
        }

        return proj;
    }

    IEnumerator CoMultiShot(int count, Transform trans, Vector3 pos, Vector3 displacement)
    {
        for (int i = 0; i < count; ++i)
        {
            yield return new WaitForSeconds(0.2f);
            FireShot(trans, pos, displacement);
        }
    }

    void FireByTrans(Transform trans, Vector3 targetPos)
    {
        if (unit != null)
        {
            bool shotSpecial = false;

            int addShot = 0;
            int multiShot = 0;

            int numCheoShot = 0;
            float cheoShotEulerAngles = 0;
            //if (this == unit.shooter ||
            //   (unit.SungPhu !=null && this == unit.SungPhu.sung1) ) // only support addShot && multishot for main shooter
            //{
            //    addShot = unit.GetBuffCount(BuffType.ADD_SHOT);
            //    multiShot = unit.GetBuffCount(BuffType.MULTI_SHOT);

            //    if (unit.SungPhu != null && this == unit.SungPhu.sung1 && unit.UnitName == "Captain")
            //    {
            //        addShot = 0;
            //        multiShot = 0;
            //    }
            //}
            //else // check BACK_SHOT_PLUS
            //if (QuanlyNguoichoi.Instance && unit == QuanlyNguoichoi.Instance.PlayerUnit &&
            //    this == unit.shooter4 ||
            //    (unit.SungPhu != null && this == unit.SungPhu.sung4))
            //{
            //    if (QuanlyNguoichoi.Instance.HaveSkillPlus(BuffType.BACK_SHOT))
            //    {
            //        addShot = 1;
            //        var buff = QuanlyNguoichoi.Instance.GetBuffStatByType(BuffType.ADD_SHOT);
            //        int buffCount = 1;
            //        int atkDown = buff.Params[buffCount - 1];
            //        var atkDownEff = gameObject.AddMissingComponent<AtkDownEffect>();
            //        atkDownEff.AddAtkDownEffect(new AtkDownEffData()
            //        {
            //            Type = buff.Type,
            //            Amount = atkDown
            //        });
            //        IgnoreAtkDown = false;
            //    }
            //}
            if(weaponCfg != null && weaponCfg.listBuff != null)
            {
                for(int i = 0; i < weaponCfg.listBuff.Count; i++)
                {
                    if (weaponCfg.listBuff[i].Type == BuffType.ADD_SHOT)
                    {
                        addShot = (int)weaponCfg.listBuff[i].Params[0];
                        shotSpecial = true;
                    }
                    else if (weaponCfg.listBuff[i].Type == BuffType.MULTI_SHOT)
                    {
                        multiShot = (int)weaponCfg.listBuff[i].Params[0];
                        shotSpecial = true;
                    }
                    else if(weaponCfg.listBuff[i].Type == BuffType.CHEO_SHOT)
                    {
                        cheoShotEulerAngles = weaponCfg.listBuff[i].Params[0];
                        numCheoShot = (int)weaponCfg.listBuff[i].Params[1];
                        shotSpecial = true;
                    }
                }                
            }

            if(shotSpecial == false)
            {
                var proj = FireShot(trans, targetPos, Vector3.zero);
            }

            if (addShot > 0)
            {
                //var displacement = trans.right * 0.4f;
                //var proj = FireShot(trans, targetPos, displacement);
                //var addProj = FireShot(trans, targetPos, -displacement);
                float displace = 1f;
                var displacement = transform.right * displace * addShot / 3;
                for (int i = 0; i < addShot; i++)
                {
                    var proj = FireShot(trans, targetPos, displacement);
                    displacement -= transform.right * displace;
                }
            }

            unit.ResetChargeEff(ChargeEff.RuningCharge);

            if (this == unit.shooter)
            {
                /// only support main shooter
                /// duongrs: now this can be check here to support MagicianSkill 
                /// because main shooter of the heroes has only one barel.
                if (unit.GetKyNang())
                {
                    unit.GetKyNang().OnUnitActiveProj(targetPos);
                }
            }

            //if (multiShot > 0)
            //{
            //    if (addShot > 0)
            //    {
            //        var displacement = trans.right * 0.5f;
            //        StartCoroutine(CoMultiShot(multiShot, trans, targetPos, displacement));
            //        StartCoroutine(CoMultiShot(multiShot, trans, targetPos, -displacement));
            //    }
            //    else
            //    {
            //        StartCoroutine(CoMultiShot(multiShot, trans, targetPos, Vector3.zero));
            //    }
            //}
            if(multiShot > 0)
            {
                StartCoroutine(CoMultiShot(multiShot, trans, targetPos, Vector3.zero));
            }

            if(numCheoShot > 0)
            {
                var degree_step_start = cheoShotEulerAngles / 2;
                var degree_step = cheoShotEulerAngles / Mathf.Max(1, numCheoShot - 1);
                for (int i = 0; i < numCheoShot; i++)
                {
                    var proj = FireShot(trans, targetPos, Vector3.zero);
                    proj.transform.eulerAngles = new Vector3(proj.transform.eulerAngles.x,
                           proj.transform.eulerAngles.y - degree_step_start + degree_step * i,
                           proj.transform.eulerAngles.z);
                }
            }
        }
    }

    public bool IsFireReady()
    {
        return mCoolDown <= 0;
    }

    public void RotateToTarget(Transform target)
    {
        var dir = target.transform.position - transform.position;
        dir.y = 0;
        //dir.z = 0;
        //var angle = Vector2.SignedAngle(Vector2.up, dir);
        //Quaternion quat = Quaternion.Euler(0, 0, angle);
        Quaternion quat = Quaternion.LookRotation(dir.normalized, transform.up);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, quat, AimRotationSpd * Time.deltaTime);
    }

    public bool CanFireDirection(Vector3 target)
    {
        if (QuanlyNguoichoi.Instance == null) return false;

        if (this.unit == QuanlyNguoichoi.Instance.PlayerUnit) return true;

        var dir = target - transform.position;
        dir.y = 0;
        return Vector3.Angle(transform.forward, dir) < 0.1f;
    }

    public void CaculateFinalStat()
    {
        if(weaponCfg == null || weaponCfg.listBuff == null || weaponCfg.listBuff.Count == 0) return;
        for(int i = 0; i < weaponCfg.listBuff.Count; i++)
        {
            var buff = weaponCfg.listBuff[i];
            switch (buff.Type)
            {
                case BuffType.ATK_UP:
                case BuffType.ATK_UP_SMALL:
                    var baseAtk = unit.GetCurStat().DMG + weaponCfg.DMG;
                    DMG = baseAtk + baseAtk * (int)buff.Params[0] / 100;
                    break;
                case BuffType.ATKSPD_UP:
                case BuffType.ATKSPD_UP_SMALL:
                    var baseAtkSpd = unit.GetCurStat().ATK_SPD + weaponCfg.ATK_SPD;
                    AtkSpd = baseAtkSpd + baseAtkSpd * buff.Params[0] / 100;
                    break;
                case BuffType.CRIT_UP:
                case BuffType.CRIT_UP_SMALL:
                    CRIT = unit.GetCurStat().CRIT + weaponCfg.CRIT * buff.Params[0];
                    break;
                case BuffType.CRITDMG_UP:
                    CRIT_DMG = unit.GetCurStat().CRIT_DMG + weaponCfg.CRIT_DMG * buff.Params[0];
                    break;
                case BuffType.RAGE_ATK:
                    break;
                case BuffType.RAGE_ATKSPD:
                    break;
                default:
                    break;
            }
        }        
    }
}
