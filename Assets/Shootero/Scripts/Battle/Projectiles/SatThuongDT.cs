using Hiker.GUI.Shootero;
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

public class SatThuongDT : MonoBehaviour
{
    #region Serialize Field
    [SerializeField]
    Float lifeTimeDuration = 10f;
    [SerializeField]
    Float knockBackDis = 0;
    #endregion

    public float LifeTimeDuration { get { return lifeTimeDuration; } set { lifeTimeDuration = value; } }
    public float LifeTime { get; protected set; }
    public float KnockBackDistance { get { return knockBackDis; } set { knockBackDis = value; } }
    public long Damage { get; protected set; }
    public int CRIT { get; set; }
    public int CRIT_DMG { get; set; }
    public int HEADSHOT { get; set; }
    public float ScaleRate { get; set; }
    public DonViChienDau SourceUnit { get; set; }
    public SungCls sungCls { get; set; }
    public long PostDmg { get; protected set; }
    public bool IsHS { get; protected set; }
    public bool IsCrit { get; protected set; }

    GameObject runingChargeProjEff;
    GameObject runingChargeProjMax;

    GameObject skillProjEff;

    [HideInInspector]
    public List<EffectConfig> listEffect = new List<EffectConfig>();

    protected bool mIsActive = false;

    int lastFrameCollision = 0;
    [HideInInspector] public GameObject lastObjCollision = null;
    protected bool cothongke = true;

    public SungCls SourceSungCls { get; set; }

    protected virtual void OnEnable()
    {
        
    }
    protected void ApplyScaleDanTo(GameObject projObj)
    {
        DanTo danTo = gameObject.GetComponent<DanTo>();

        if (danTo)
        {
            var danToProj = projObj.GetComponent<DanTo>();
            if (danToProj == null)
            {
                danToProj = projObj.AddComponent<DanTo>();
                danToProj.InitOriginSize(projObj.transform.localScale);
            }
            danToProj.ApplyScale(new DanToSrc { Src = 3, Scale = danTo.TileScale });
        }
        else
        {
            var danToProj = projObj.GetComponent<DanTo>();
            if (danToProj)
            {
                danToProj.UnApplyScale(3);
            }
        }
    }

    protected virtual void Update()
    {

    }

    public void ActiveSkillProjEff(GameObject prefab)
    {
        if (prefab == null) return;
        if (skillProjEff != null && skillProjEff.name != prefab.name)
        {
            ObjectPoolManager.Unspawn(skillProjEff);
            skillProjEff = null;
        }

        if (skillProjEff == null)
        {
            skillProjEff = ObjectPoolManager.Spawn(prefab, Vector3.zero, Quaternion.identity, transform);
        }
    }
    public void DeactiveSkillProjEff()
    {
        if (skillProjEff)
        {
            skillProjEff.gameObject.SetActive(false);
        }
    }

    public void ActiveRuningCharge(bool isFull)
    {
        if (isFull == false)
        {
            if (runingChargeProjEff == null)
            {
                runingChargeProjEff = ObjectPoolManager.Spawn("Particle/PowerUp/RUNNING_CHARGE_Proj",
                    Vector3.zero,
                    Quaternion.identity,
                    transform);
            }
            if (runingChargeProjEff)
            {
                runingChargeProjEff.gameObject.SetActive(true);
            }
            if (runingChargeProjMax)
            {
                runingChargeProjMax.gameObject.SetActive(false);
            }
        }
        else
        {
            if (runingChargeProjMax == null)
            {
                runingChargeProjMax = ObjectPoolManager.Spawn("Particle/PowerUp/RUNNING_CHARGE_ProjMax",
                    Vector3.zero,
                    Quaternion.identity,
                    transform);
            }
            if (runingChargeProjEff)
            {
                runingChargeProjEff.gameObject.SetActive(false);
            }
            if (runingChargeProjMax)
            {
                runingChargeProjMax.gameObject.SetActive(true);
            }
        }
    }
    public void DeactiveRuningCharge()
    {
        if (runingChargeProjEff)
        {
            runingChargeProjEff.gameObject.SetActive(false);
        }
        if (runingChargeProjMax)
        {
            runingChargeProjMax.gameObject.SetActive(false);
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.gameObject == lastObjCollision)
        {
            lastObjCollision = null;
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (mIsActive)
        {
            var frameCount = Time.frameCount;
            var colObj = other.gameObject;
            if (lastObjCollision == colObj) return;
            lastFrameCollision = frameCount;
            lastObjCollision = colObj;
            //Debug.Log("frameCount = " + frameCount);
            //Debug.Log("obj = " + colObj.name);
            OnCollisionOther(colObj);
        }
    }

    protected virtual void OnReflectDMG(DonViChienDau unit)
    {

    }

    [Beebyte.Obfuscator.ObfuscateLiterals]
    protected virtual bool OnCatchTarget(DonViChienDau unit)
    {
        if (unit.IsAlive() == false) return false;

        bool reflect = false;
        if (unit.GetStat().EVASION > 0 && unit.IsInvulnerable == false) // chi tinh ne khi unit khong duoc bat bat tu
        {
            reflect = Random.Range(0, 100) < unit.GetStat().EVASION;
            OnReflectDMG(unit);
        }

        if (reflect == false)
        {
            bool isHeadShot = Random.Range(0, 100) < HEADSHOT;
            if (unit.IsBoss)
            {
                isHeadShot = false;
            }
            
            IsHS = isHeadShot;
            if (isHeadShot)
            {
                var takenDmg = unit.TakeDamage(
                    unit.GetMaxHP(), // dmg
                    true, // display crit
                    SourceUnit,
                    true, // play hit sound
                    false, // show hud
                    false, // xuyen bat tu
                    false, // xuyen linken
                    false, // thong ke
                    true  // isHeadshot
                    );
                
                PostDmg = takenDmg;

                if (takenDmg > 0)
                {
                    unit.OnPostDinhDan(takenDmg);
                }

                ScreenBattle.instance.DisplayTextHud(Localization.Get("HeadShotLabel"), unit);
            }
            else
            {
                bool isCrit = Random.Range(0, 100) < CRIT;
                var dmg = isCrit ? Damage * CRIT_DMG / 100 : Damage;

                bool isDanCuaNguoiChoi = false;
                if (SourceUnit != null && SourceUnit == QuanlyNguoichoi.Instance.PlayerUnit && this is DanBay)
                    isDanCuaNguoiChoi = true;
                var takenDmg = unit.TakeDamage(dmg,
                    isCrit,
                    SourceUnit,
                    true, // play hit sound
                    true, // show hud
                    false, // xuyen bat tu
                    false, // xuyen linken
                    cothongke, // thong ke
                    false,  // isHeadshot
                    false, // sourceUnitIsBoss
                    false, // sourceUnitIsAir
                    false, // skipSourceUnitType
                    isDanCuaNguoiChoi
                    );
                IsCrit = isCrit;
                PostDmg = takenDmg;
                if (takenDmg > 0)
                {
                    unit.OnPostDinhDan(takenDmg);
                }
                if (KnockBackDistance > 0)
                {
                    var vec = transform.forward;
                    vec.y = 0;
                    unit.KnockBack(vec.normalized * KnockBackDistance);
                    KnockBackDistance = 0;
                }

                foreach (var eff in listEffect)
                {
                    unit.GetStatusEff().ApplyEffect(new BattleEffect(eff));
                }
            }
        }
        else
        {
#if DEBUG
            //Debug.Log("EVASION");
#endif
            if (unit == QuanlyNguoichoi.Instance.PlayerUnit)
            {
                QuanlyNguoichoi.Instance.AddThongKeST("EVADE", 0,
                    QuanlyNguoichoi.Instance.PlayerUnit.GetCurHP(),
                    QuanlyNguoichoi.Instance.PlayerUnit.GetCurHP(),
                    QuanlyNguoichoi.Instance.PlayerUnit.GetMaxHP());
            }
            unit.OnEvadeBullet();
            return false;
        }

        return true;
    }

    protected virtual void OnDeactiveDan()
    {
        mIsActive = false;
        DeactiveRuningCharge();
        if (runingChargeProjEff)
        {
            ObjectPoolManager.Unspawn(runingChargeProjEff);
            runingChargeProjEff = null;
        }
        if (runingChargeProjMax)
        {
            ObjectPoolManager.Unspawn(runingChargeProjMax);
            runingChargeProjMax = null;
        }
        if (skillProjEff)
        {
            ObjectPoolManager.Unspawn(skillProjEff);
            skillProjEff = null;
        }
    }

    public virtual void DeactiveDan()
    {
        OnDeactiveDan();
    }

    protected virtual bool OnHitObstacle(GameObject obstacle)
    {
        if (LayersMan.CheckMask(obstacle.layer, LayersMan.Team1HitLayerMask))
        { // deactive when hit team1 object
            return true;
        }
        return false; // mac dinh damage object xuyen obstacle
    }

    protected virtual bool OnCollisionOther(GameObject other)
    {
#if PROFILING
        UnityEngine.Profiling.Profiler.BeginSample("OnCollisionOther - GetUnit");
#endif
        var unit = other.GetComponentInParent<DonViChienDau>();
#if PROFILING
        UnityEngine.Profiling.Profiler.EndSample();
#endif
#if PROFILING
        UnityEngine.Profiling.Profiler.BeginSample("OnCollisionOther - Remain");
#endif
        if (unit != null)
        {
            if (unit.IsAlive() == false)
            {
#if PROFILING
        UnityEngine.Profiling.Profiler.EndSample();
#endif
                return false;
            }
            if (unit.IsCoTheDinhDan() == false)
            {
#if PROFILING
        UnityEngine.Profiling.Profiler.EndSample();
#endif
                return false;
            }

            var re = OnCatchTarget(unit);
            if (re)
            {
                OnDeactiveDan();
            }
#if PROFILING
        UnityEngine.Profiling.Profiler.EndSample();
#endif
            return re;
        }
        else
        {
            if (OnHitObstacle(other))
            {
                OnDeactiveDan();
#if PROFILING
        UnityEngine.Profiling.Profiler.EndSample();
#endif
                return true;
            }
        }

#if PROFILING
        UnityEngine.Profiling.Profiler.EndSample();
#endif
        return false;
    }

    public virtual void ActiveDan(float speed, long dmg, Vector3 target)
    {
        cothongke = true;
        this.Damage = dmg;
        this.LifeTime = lifeTimeDuration;
        mIsActive = true;
        gameObject.SetActive(true);

        lastObjCollision = null;

        ScaleRate = 1f;
    }
}
