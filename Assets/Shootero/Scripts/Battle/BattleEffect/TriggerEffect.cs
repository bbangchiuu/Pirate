using Hiker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEffect : MonoBehaviour
{
    public EffectConfig Effect;
    public LayerMask TargetMask;

    /// <summary>
    /// Only use for pure damage effect
    /// </summary>
    public bool TriggerOnce = false;
    public float Duration = 0;
    public int SourceTeamID = -1;

    const float TRIGGER_TICK = 0.1f;

    List<DonViChienDau> mTargets = new List<DonViChienDau>();

    public bool IsBossDmage = false;

    float lifeTime = 0;
    float mTimeInterval = -0.0001f;
    protected virtual void Awake()
    {
        lifeTime = 0;
    }

    public virtual void Resets()
    {
        lifeTime = 0;
        mTimeInterval = -0.0001f;
    }

    protected virtual void OnEnable()
    {
        Resets();

        //if (LayersMan.CheckMask(LayersMan.Team1, TargetMask.value))
        //{
        //    TargetMask.value = LayersMan.Team1HitLayerMask;
        //}

        //if (QuanlyNguoichoi.Instance &&
        //    LayersMan.CheckMask(LayersMan.Team1, TargetMask.value))
        //{
        //    Effect.Damage = QuanlyNguoichoi.Instance.GetDMGEnemy();
        //}
    }

    private void Start()
    {

    }

    float curTickTime = TRIGGER_TICK;
    private void Update()
    {

        // Because of unit is disable before exit from trigger then it does not trig the event
        // We must check does target stay in trigger before call EffectOnTick

        if (mTargets == null)
        {
            return;
        }

        //foreach (var t in mTargets)
        //{
        //    if (t == null || t.gameObject == null || t.gameObject.activeInHierarchy == false)
        //    {
        //        mTargets.Remove()
        //    }
        //}

        //mTargets.RemoveAll(e => e == null || (e.gameObject != null && e.gameObject.activeInHierarchy == false));
        curTickTime += Time.deltaTime;
        if (curTickTime >= TRIGGER_TICK)
        {
            float dTime = curTickTime;
            curTickTime = 0;
            if (mTargets.Count > 0)
            {
                switch (Effect.Type)
                {
                    case EffectType.DoT:
                        OnTickDoT(dTime);
                        break;
                    case EffectType.Slow:
                        OnTickSlow(dTime);
                        break;
                    default:
                        break;
                }
            }
        }

        //else
        //{
        //    mTimeInterval = 0;
        //}

        lifeTime += Time.deltaTime;

        if (Duration > 0 && lifeTime >= Duration)
        {
            ObjectPoolManager.Unspawn(gameObject);
        }
    }

    private void OnDisable()
    {
        mTargets.Clear();
    }

    //public void Enable()
    //{
    //    countTime = AimTime;
    //    this.isEnable = true;
    //}



    //public bool isStartFire = true;

    void OnTickSlow(float deltaTime)
    {
        if (Effect.Type == EffectType.Slow && mTimeInterval < 0)
        {
            mTimeInterval = 0.5f;
        }

        mTimeInterval += deltaTime;

        if (mTimeInterval >= 0.5f)
        {
            mTimeInterval = 0;
            for (int i = mTargets.Count - 1; i >= 0; --i)
            {
                var unit = mTargets[i];
                if (unit != null && unit.gameObject != null && unit.gameObject.activeInHierarchy && unit.IsAlive())
                {
                    BattleEffect eff = new BattleEffect(Effect);
                    eff.Config.Damage = 0;
                    eff.Duration = 0.5f;

                    eff.ObjSrc = gameObject;
                    unit.GetStatusEff().ApplyEffect(eff);
                }
                else
                {
                    mTargets.RemoveAt(i);
                }
            }
        }
    }

    void OnTickDoT(float deltaTime)
    {
#if PROFILING
        UnityEngine.Profiling.Profiler.BeginSample("OnTickDoT");
#endif
        if (Effect.Type == EffectType.DoT && mTimeInterval < 0)
        {
            mTimeInterval = Effect.Param1;
        }

        mTimeInterval += deltaTime;

        if (mTimeInterval >= Effect.Param1)
        {
            mTimeInterval = 0;
            StartCoroutine(CoDoT(deltaTime));
        }

#if PROFILING
        UnityEngine.Profiling.Profiler.EndSample();
#endif
    }

    const int NumTargetEffectPerFrame = 3;

    IEnumerator CoDoT(float deltaTime)
    {
        for (int i = mTargets.Count - 1; i >= 0; i -= NumTargetEffectPerFrame)
        {
            int minJ = Mathf.Max(0, i - NumTargetEffectPerFrame);
            for (int j = i; j >= minJ; --j)
            {
                //if (unit.GetDoTEffectDamage() < Effect.Damage)
                if (j < mTargets.Count)
                {
                    var unit = mTargets[j];
                    if (unit != null && unit.gameObject != null && unit.gameObject.activeInHierarchy && unit.IsAlive())
                    {
                        BattleEffect eff = new BattleEffect(Effect);
                        eff.Duration = Effect.Param1;

                        eff.ObjSrc = gameObject;
                        eff.SourceUnitIsBoss = IsBossDmage;
                        //if (GameManager.Instance.ScaleDesignUnitByZombieLevel)
                        //{
                        //    float rate = GameManager.Instance.GetRateScaleByZombieLevel();
                        //    eff.Config.Damage = Effect.Damage * rate;
                        //}
#if PROFILING
                    UnityEngine.Profiling.Profiler.BeginSample("Apply DOT Effect");
#endif
                        unit.GetStatusEff().ApplyEffect(eff);
#if PROFILING
                    UnityEngine.Profiling.Profiler.EndSample();
#endif
                    }
                    else
                    {
                        mTargets.RemoveAt(j);
                    }
                }
            }

            yield return null;
        }
    }

    void OnUnitEnter(DonViChienDau unit)
    {
        //UnityEditor.EditorApplication.isPaused = true;

        if (TriggerOnce && Effect.Type == EffectType.PureDam)
        {
            gameObject.SetActive(false);
        }

        if (unit.TeamID != SourceTeamID)
        {
            mTargets.Add(unit);
            switch (Effect.Type)
            {
                case EffectType.Slow:
                    //unit.ApplySlowEffect(Effect.Param1);
                    BattleEffect eff = new BattleEffect(Effect);
                    eff.Duration = 0.5f;
                    eff.Config.Damage = 0;
                    eff.ObjSrc = gameObject;
                    unit.GetStatusEff().ApplyEffect(eff);

                    break;
                case EffectType.PureDam:
                    //if (unit.IsSolider() && unit.IsDie() == false)
                    //{
                    //    var soldier = unit as SoliderBase;
                    //    if (soldier.IsOnVehicle &&
                    //        soldier.OwnerVehicle.IsInvisible == false)
                    //    {
                    //        break;
                    //    }
                    //}
                    unit.TakeDamage(Effect.Damage, false, null, false, true,false,false,true,false, IsBossDmage);
                    break;
                default:
                    break;
            }
        }
    }

    void OnUnitExit(DonViChienDau unit)
    {
        mTargets.Remove(unit);

        switch (Effect.Type)
        {
            case EffectType.Slow:
                unit.GetStatusEff().UnapplySlowEffect(Effect.Param1);
                break;
            default:
                break;
        }
    }

    protected virtual void OnTriggerEnter(Collider col)
    {
        if (HikerUtils.CheckLayerMask(TargetMask, col.gameObject.layer))
        {
            DonViChienDau unit = col.GetComponent<DonViChienDau>();

            if (unit)
            {
                if (mTargets.Contains(unit) == false)
                {
                    OnUnitEnter(unit);
                }

            }
        }
    }

    protected void OnTriggerExit(Collider col)
    {
        if (HikerUtils.CheckLayerMask(TargetMask, col.gameObject.layer))
        {
            DonViChienDau unit = col.GetComponent<DonViChienDau>();

            if (unit)
            {
                if (mTargets.Contains(unit))
                    OnUnitExit(unit);
            }
        }
    }

    public void SetEffect(EffectConfig eff, LayerMask mask, float duration = 0f, int sourceTeamID = -1)
    {
        Effect = eff.Clone();
        TargetMask = mask;
        Duration = duration;
        SourceTeamID = sourceTeamID;
    }

//#if UNITY_EDITOR
//    public void OnDrawGizmos()
//    {
//        Gizmos.color = new Color(1, 1, 0, 0.2f);

//        Gizmos.matrix = transform.localToWorldMatrix;

//        BoxCollider boxCol = GetComponent<BoxCollider>();
//        if (boxCol != null)
//        {
//            Gizmos.DrawCube(boxCol.center, boxCol.size);
//            return;
//        }

//        SphereCollider sphereCol = GetComponent<SphereCollider>();
//        Gizmos.DrawSphere(sphereCol.center, sphereCol.radius);

//    }
//#endif
}

