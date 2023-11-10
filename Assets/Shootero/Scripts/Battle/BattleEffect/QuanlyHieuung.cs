using Hiker.GUI.Shootero;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(DonViChienDau))]
public class QuanlyHieuung : MonoBehaviour
{
    /// <summary>
    /// value < 1 => slow down
    /// value > 1 = speed up
    /// </summary>
    float speedEff = 1; 

    List<BattleEffect> mListEffect = new List<BattleEffect>();
    DonViChienDau unit;
    NavMeshAgent navAgent;

    private void Awake()
    {
        unit = GetComponent<DonViChienDau>();
        navAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (unit == null || unit.IsAlive() == false) return;

        EffectCycle(Time.deltaTime);
    }

    public void ApplySlowEffect(float slowPercent)
    {
        slowPercent = Mathf.Clamp(slowPercent, 1f, 99f);

        ApplySpeedEffect(-slowPercent / 100f);
    }

    public void UnapplySlowEffect(float slowPercent)
    {
        slowPercent = Mathf.Clamp(slowPercent, 1f, 99f);

        ApplySpeedEffect(slowPercent / (1 - slowPercent / 100f) / 100f);
    }
    /// <summary>
    ///
    /// </summary>
    /// <param name="_speedEffDif"> negative when slow down, positive when speed up </param>
    public virtual void ApplySpeedEffect(float _speedEffDif)
    {
        speedEff *= (1 + _speedEffDif);

        UpdateNavAgentSpeed();
    }

    public virtual void ApplyFronzenEffect(float slowPercent)
    {
        //ScreenBattle.instance.DisplayTextHud("GetFrozen", unit);

        unit.SetRendererColor(new Color(0, 0.675f, 1, 0.56f));        

        unit.TimeScale -= slowPercent / 100f;
    }

    public virtual void UnapplyFronzenEffect(float slowPercent)
    {
        unit.BackToOriginRendererColor();

        unit.TimeScale += slowPercent / 100f;
    }

    void ApplyRageAtk(float buff)
    {
        ScreenBattle.instance.DisplayTextHud("RageAtk Active", unit);
        unit.BuffAtkUp((int)buff);
    }

    void UnapplyRageAtk(float buff)
    {
        ScreenBattle.instance.DisplayTextHud("RageAtk Deactive", unit);
        unit.BuffAtkUp(-(int)buff);
    }

    void ApplyRageAtkSpd(float buff)
    {
        ScreenBattle.instance.DisplayTextHud("RageAtkSpd Active", unit);
        unit.BuffAtkSpdUp((int)buff);
    }

    void UnapplyRageAtkSpd(float buff)
    {
        ScreenBattle.instance.DisplayTextHud("RageAtkSpd Deactive", unit);
        unit.BuffAtkSpdUp(-(int)buff);
    }

    public bool IsHaveActiveEffect(EffectType effType)
    {
        return  mListEffect.Exists(e => e.Type == effType && e.IsActive && e.Duration > 0);
    }

    public void RemoveLinkenEffect()
    {
        if (QuanlyNguoichoi.Instance.EffectLinken)
            QuanlyNguoichoi.Instance.EffectLinken.SetActive(false);

        mListEffect.RemoveAll(e => e.Type == EffectType.Linken);
    }
    public void ApplyLinkenEffect()
    {
        BattleEffect battleEff = new BattleEffect(new EffectConfig
        {
            Type = EffectType.Linken,
            Duration = 1000000
        });
        ApplyEffect(battleEff);
    }

    public void RemoveRageAtk()
    {
        var effs = mListEffect.FindAll(e => e.Type == EffectType.RageAtk);
        foreach (var eff in effs)
        {
            UnapplyRageAtk(eff.Config.Param1);
        }
    }

    public void RemoveRageAtkSpd()
    {
        var effs = mListEffect.FindAll(e => e.Type == EffectType.RageAtkSpd);
        foreach (var eff in effs)
        {
            UnapplyRageAtkSpd(eff.Config.Param1);
        }
    }

    GameObject burnEff = null;

    public virtual void ApplyEffect(BattleEffect effect)
    {
        if (unit == null || unit.IsAlive() == false) return;

        effect.IsActive = true;

#if PROFILING
        UnityEngine.Profiling.Profiler.BeginSample("UnitBase.ApplyEffect");
#endif
        if (effect.Type == EffectType.Linken)
        {
            if (QuanlyNguoichoi.Instance && QuanlyNguoichoi.Instance.EffectLinken)
                QuanlyNguoichoi.Instance.EffectLinken.SetActive(true);

            if (mListEffect.Exists(e => e.Type == EffectType.Linken &&
                                        e.Duration > 0))
            {
                // duongrs changed on 20180208
                return;  // khong stack slow
            }
            else
            {
                //ScreenBattle.instance.DisplayTextHud("LINKEN ACTIVE", unit);
                //if (QuanlyNguoichoi.Instance && QuanlyNguoichoi.Instance.EffectLinken)
                //    QuanlyNguoichoi.Instance.EffectLinken.SetActive(true);

                mListEffect.RemoveAll(e => e.Type == EffectType.Linken);
                mListEffect.Add(effect);
            }
        }
        else
        if (effect.Type == EffectType.RageAtk)
        {
            if (mListEffect.Exists(e => e.Type == EffectType.RageAtk &&
                                        e.Config.Param1 >= effect.Config.Param1 &&
                                        e.Duration > 0))
            {
                // duongrs changed on 20180208
                return; // khong stack DoT
            }
            else
            {
                foreach (var eff in mListEffect)
                {
                    if (eff.Type == effect.Type && eff.Duration > 0)
                    {
                        UnapplyRageAtk(eff.Config.Param1);
                    }
                }
                mListEffect.RemoveAll(e => e.Type == EffectType.RageAtk);
                mListEffect.Add(effect);
                ApplyRageAtk(effect.Config.Param1);
            }
        }
        else
        if (effect.Type == EffectType.RageAtkSpd)
        {
            if (mListEffect.Exists(e => e.Type == EffectType.RageAtkSpd &&
                                        e.Config.Param1 >= effect.Config.Param1 &&
                                        e.Duration > 0))
            {
                // duongrs changed on 20180208
                return; // khong stack DoT
            }
            else
            {
                foreach (var eff in mListEffect)
                {
                    if (eff.Type == effect.Type && eff.Duration > 0)
                    {
                        UnapplyRageAtkSpd(eff.Config.Param1);
                    }
                }
                mListEffect.RemoveAll(e => e.Type == EffectType.RageAtkSpd);
                mListEffect.Add(effect);
                ApplyRageAtkSpd(effect.Config.Param1);
            }
        }
        else
        if (effect.Type == EffectType.Electric)
        {
            var count = (int)Mathf.Round(effect.Config.Param1);
            if (count > 0)
            {
                var targetUnit = unit;
                List<DonViChienDau> otherUnits = new List<DonViChienDau>() { unit };

                for (int i = 1; i < count; ++i)
                {
                    Transform previous_target = targetUnit.transform;

                    targetUnit = QuanlyManchoi.FindClosestEnemy(targetUnit.transform.position,
                        effect.Config.Param2,
                        otherUnits);
                    if (targetUnit != null)
                    {
                        var cloneEff = effect.Config.Clone();
                        cloneEff.Param1 = 0;
                        var newEff = new BattleEffect(cloneEff);
                        newEff.ObjSrc = effect.ObjSrc;

                        targetUnit.GetStatusEff().ApplyEffect(newEff);
                        otherUnits.Add(targetUnit);

                        targetUnit.StartLightningChain(previous_target);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            

            if( effect.Config.Param2==0)
            {
                // electric field 
                var targetUnit = unit;
                targetUnit.StartLightningChain(QuanlyNguoichoi.Instance.PlayerUnit.transform);
            }

            //ScreenBattle.instance.DisplayTextHud("Electric", unit);
        }
        else
        if (effect.Type == EffectType.Frozen)
        {
            if (unit.IsBoss)
            {
                effect.Config.Param1 *= 0.5f;
            }

            var curEff = mListEffect.Find(e => e.Type == effect.Type && e.Duration > 0);
            if (curEff != null)
            {
                if (curEff.Config.Param1 > effect.Config.Param1)
                {
                    // khong stack slow
                    return;
                }
                else if (curEff.Config.Param1 == effect.Config.Param1 &&
                    curEff.Duration < effect.Duration)
                {
                    curEff.Duration = effect.Duration;
                    return;
                }
                else
                {
                    foreach (var eff in mListEffect)
                    {
                        if (eff.Type == effect.Type && eff.Duration > 0)
                        {
                            UnapplyFronzenEffect(eff.Config.Param1);
                        }
                    }

                    mListEffect.RemoveAll(e => e.Type == effect.Type);
                    mListEffect.Add(effect);

                    ApplyFronzenEffect(effect.Config.Param1);
                }
            }
            else
            {
                mListEffect.Add(effect);

                ApplyFronzenEffect(effect.Config.Param1);
            }
        }
        else
        if (effect.Type == EffectType.Flame)
        {
            var curEff = mListEffect.Find(e => e.Type == EffectType.Flame &&
                                       //e.Config.Damage > effect.Config.Damage &&
                                       e.Duration > 0);
            if (curEff != null)
            {
                if (curEff.Config.Damage > effect.Config.Damage)
                {
                    // khong stack slow
                    return;
                }
                else if (curEff.Config.Damage == effect.Config.Damage &&
                    curEff.Duration < effect.Duration)
                {
                    curEff.Duration = effect.Duration;
                    return;
                }
                else
                {
                    mListEffect.RemoveAll(e => e.Type == EffectType.Flame);
                    mListEffect.Add(effect);
                    if (burnEff == null)
                    {
                        burnEff = ObjectPoolManager.Spawn("Particle/PowerUp/FLAME_EFF");
                        burnEff.transform.parent = unit.transform;
                        burnEff.transform.position = unit.transform.position;
                    }
                }
            }
            else
            {
                //ScreenBattle.instance.DisplayTextHud("GetFlame", unit);
                if (burnEff == null)
                {
                    burnEff = ObjectPoolManager.Spawn("Particle/PowerUp/FLAME_EFF");
                    burnEff.transform.parent = unit.transform;
                    burnEff.transform.position = unit.transform.position;
                }

                //mListEffect.RemoveAll(e => e.Type == EffectType.Flame);
                mListEffect.Add(effect);
            }
        }
        else
        if (effect.Type == EffectType.Slow)
        {
            if (mListEffect.Exists(e => e.Type == effect.Type &&
                                        e.Config.Param1 > effect.Config.Param1 &&
                                        e.Duration > 0))
            {
                // duongrs changed on 20180208
                //return;  // khong stack slow
            }
            else
            {
                foreach (var eff in mListEffect)
                {
                    if (eff.Type == EffectType.Slow && eff.Duration > 0)
                    {
                        UnapplyFronzenEffect(eff.Config.Param1);
                    }
                }

                mListEffect.RemoveAll(e => e.Type == EffectType.Slow);
                mListEffect.Add(effect);

                ApplySlowEffect(effect.Config.Param1);
            }
        }
        else if (effect.Type == EffectType.DoT)
        {
            if (mListEffect.Exists(e => e.Type == EffectType.DoT &&
                                        e.Config.Damage >= effect.Config.Damage &&
                                        e.Duration > 0))
            {
                // duongrs changed on 20180208
                //return; // khong stack DoT
            }
            else
            {
                mListEffect.RemoveAll(e => e.Type == EffectType.DoT);
                mListEffect.Add(effect);

                if (effect.Config.Param3 > 0)
                {
                    EffectConfig eCfg = new EffectConfig()
                    {
                        Type = EffectType.Slow,
                        Duration = effect.Duration,
                        Param1 = effect.Config.Param3
                    };

                    BattleEffect eSlow = new BattleEffect(eCfg);

                    ApplyEffect(eSlow);
                }
            }
        }
        else if (effect.Type == EffectType.BuffAtk)
        {
            mListEffect.Add(effect);
            unit.BuffAtkUp((int)Mathf.Round(effect.Config.Param1));
        }
        else
        {
            mListEffect.Add(effect);
        }

#if PROFILING
        UnityEngine.Profiling.Profiler.EndSample();
#endif
        if (effect.Type == EffectType.Stun)
        {
            //SetState(UnitState.STUN);

            //if (effect.Config.Damage > 0)
            //{
            //    unitHealth.FinalDamage(effect.Config.Damage);
            //}
            //PlayScreen.instance.AddPoolText(Localization.Get("StunLabel"), transform);
            unit.SetStun(true);
        }
        else if (effect.Type == EffectType.Root)
        {
            unit.SetRoot(true);
        }
        else if (effect.Type == EffectType.PureDam)
        {
            //if (effect.Config.Damage > 0)
            //{
            //    unitHealth.FinalDamage(effect.Config.Damage);
            //}
        }
        else if (effect.Type == EffectType.Crit)
        {
            //if (effect.Config.Damage > 0)
            //{
            //    unitHealth.FinalDamage(effect.Config.Damage);
            //}
        }

        effect.OwnerUnit = unit;

        // tru damage tu effect
        if (effect.Config.Damage > 0)
        {
            long finalDamage = effect.Config.Damage;
            //if (IsZombie())
            //{
            //    ZombieBase zombie = (ZombieBase)this;
            //    if (zombie && zombie.ZombieType == ZombieType.ZomRat && zombie.MaxReceiveDamage > 0)
            //    {
            //        if (effect.Type == EffectType.DoT)
            //        {
            //            ZombieRatManager zombieRatManager = gameObject.GetComponent<ZombieRatManager>();
            //            if (zombieRatManager && zombieRatManager.multilpleDoTDamage > 0)
            //                finalDamage *= zombieRatManager.multilpleDoTDamage;
            //        }
            //        else //such as : Slow
            //        {
            //            if (finalDamage > zombie.MaxReceiveDamage)
            //                finalDamage = zombie.MaxReceiveDamage;
            //        }
            //    }
            //}

            //if (effect.WeaponSrc != null && effect.WeaponSrc.IsGun())
            //{
            //    if (unitHealth.BlockBulletDamage > 0)
            //    {
            //        finalDamage -= unitHealth.BlockBulletDamage;

            //        if (finalDamage <= 0)
            //            finalDamage = 1;
            //    }

            //    if (unitHealth.BulletProof > 0)
            //    {
            //        finalDamage *= (100 - unitHealth.BulletProof) / 100f;
            //    }
            //}

            //PlayScreen.instance.AddPoolText(((int)finalDamage).ToString(), transform);
            //if (effect.Type == EffectType.Crit)
            //{
            //    unitHealth.FinalDamage(finalDamage, true,
            //        effect.WeaponSrc != null ? effect.WeaponSrc.myUnitBase : null,
            //        effect.WeaponSrc != null ? effect.WeaponSrc.wInfo.HudCfgIdx : 0,
            //        PoolTextItem.EPoolTextSize.Big);
            //}
            //else
            {
                DonViChienDau sourceUnit = null;
                if (effect.ObjSrc != null)
                {
                    sourceUnit = effect.ObjSrc.GetComponent<DonViChienDau>();
                }

                bool isCrit = false;
                if(effect.Config.Crit > 0)
                {
                    int rd = Random.Range(0, 100);
                    if (rd < effect.Config.Crit)
                    {
                        isCrit = true;
                        finalDamage = (long)(finalDamage * effect.Config.CritDMG / 100);
                    }
                }
                if (isCrit == false)
                {
                    unit.TakeDamage(finalDamage,
                        false, // display crit
                        sourceUnit, // source unit
                        false, // play hit sound
                        true, // show hud
                        false, // xuyen bat tu
                        false, // xuyen linken
                        true, // thong ke
                        false, // headshot ?
                        effect.SourceUnitIsBoss);
                }
                else
                {
                    unit.TakeDamage(finalDamage,
                            isCrit, // is crit
                            sourceUnit, // source unit
                            false, // play hit sound
                            true, // show hud
                            false, // xuyen bat tu
                            false, // xuyen linken
                            true, // thong ke
                            false,  // isHeadshot
                            effect.SourceUnitIsBoss, // sourceUnitIsBoss
                            false, // sourceUnitIsAir
                            false, // skipSourceUnitType
                            false, //isDanCuaNguoiChoi
                            effect.Config.isEleCrit //isEleCrit
                            );
                }
            }
        }
    }

    protected virtual void EffectCycle(float deltaTime)
    {
        for (int i = 0; i < mListEffect.Count; ++i)
        {
            if (mListEffect[i] != null)
                mListEffect[i].OnTick(deltaTime);
        }

        bool removeStunEff = false;
        bool removeRootEff = false;

        for (int i = mListEffect.Count - 1; i >= 0; --i)
        {
            var eff = mListEffect[i];
            if (eff == null)
            {
                mListEffect.RemoveAt(i);
            }
            else if (eff.Duration <= 0)
            {
                if (eff.Type == EffectType.Stun)
                {
                    removeStunEff = true;
                }
                else if (eff.Type == EffectType.Root)
                {
                    removeRootEff = true;
                }
                else if (eff.Type == EffectType.RageAtk)
                {
                    UnapplyRageAtk(eff.Config.Param1);
                }
                else if (eff.Type == EffectType.RageAtkSpd)
                {
                    UnapplyRageAtkSpd(eff.Config.Param1);
                }
                else if (eff.Type == EffectType.Flame)
                {
                    if (burnEff)
                    {
                        ObjectPoolManager.Unspawn(burnEff);
                        burnEff = null;
                    }
                }
                else if (eff.Type == EffectType.BuffAtk)
                {
                    unit.DeBuffAtkUp((int)Mathf.Round(eff.Config.Param1));
                }

                mListEffect.RemoveAt(i);
            }
        }
        if (removeStunEff)
        {
            if (mListEffect.Exists(e => e.Type == EffectType.Stun) == false)
            {
                if (unit)
                    unit.SetStun(false);
            }
        }
        if (removeRootEff)
        {
            if (mListEffect.Exists(e => e.Type == EffectType.Root) == false)
            {
                if (unit)
                    unit.SetRoot(false);
            }
        }
    }

    protected virtual void UpdateNavAgentSpeed()
    {
        if (navAgent == null || !navAgent.isActiveAndEnabled) return;

        navAgent.speed = unit.GetStat().SPD * speedEff;
    }
}
