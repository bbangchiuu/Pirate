using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEffect
{
    public EffectConfig Config;
    public float Duration { get; set; }
    public DonViChienDau OwnerUnit { get; set; }
    public SungCls WeaponSrc { get; set; }
    public GameObject ObjSrc { get; set; }

    public bool IsActive { get; set; }

    public bool SourceUnitIsBoss = false;

    public BattleEffect(EffectConfig cfg)
    {
        Config = cfg.Clone();
        Duration = cfg.Duration;
        mTimeInterval = 0;
    }

    public EffectType Type { get { return Config.Type; } }

    float mTimeInterval = 0;

    public void OnTick(float deltaTime)
    {
        if (IsActive)
        {
            Duration -= deltaTime;
            mTimeInterval += deltaTime;

            if (OwnerUnit != null && OwnerUnit.IsAlive())
            {
                switch (Type)
                {
                    case EffectType.Slow:
                        OnSlowTick(deltaTime);
                        break;
                    case EffectType.Frozen:
                        if(Config.Damage > 0)
                        {
                            OnDoTTickFrozen(deltaTime);
                        }
                        if (Duration <= 0)
                        {
                            IsActive = false;
                            OwnerUnit.GetStatusEff().UnapplyFronzenEffect(Config.Param1);
                        }
                        break;
                    case EffectType.Flame:
                    case EffectType.DoT:
                        OnDoTTick(deltaTime);
                        break;
                    case EffectType.Stun:
                        break;
                    case EffectType.Pierce:
                        break;
                }
            }
        }
    }

    void OnSlowTick(float deltaTime)
    {
        if (Duration <= 0)
        {
            IsActive = false;
            OwnerUnit.GetStatusEff().UnapplySlowEffect(Config.Param1);
        }
    }

    void OnDoTTick(float deltaTime)
    {
        if (Duration > 0)
        {
            if (mTimeInterval > Config.Param1)
            {
                DonViChienDau sourceUnit = null;
                if (ObjSrc != null)
                {
                    sourceUnit = ObjSrc.GetComponent<DonViChienDau>();
                }

                long dmg = Config.Damage;
                bool isCrit = false;
                if(Config.Crit > 0)
                {
                    int rd = Random.Range(0, 100);
                    if (rd < Config.Crit)
                    {
                        isCrit = true;
                        dmg = (long)(dmg * Config.CritDMG / 100);
                    }
                }

                if (isCrit == false)
                {
                    OwnerUnit.TakeDamage(dmg,
                        false, // is crit
                        sourceUnit, // source unit
                        false, // play hit sound
                        true); // show HUD
                }
                else
                {
                    OwnerUnit.TakeDamage(dmg,
                        isCrit, // is crit
                        sourceUnit, // source unit
                        false, // play hit sound
                        true, // show hud
                        false, // xuyen bat tu
                        false, // xuyen linken
                        true, // thong ke
                        false,  // isHeadshot
                        false, // sourceUnitIsBoss
                        false, // sourceUnitIsAir
                        false, // skipSourceUnitType
                        false, //isDanCuaNguoiChoi
                        Config.isEleCrit //isEleCrit
                        );
                }

                mTimeInterval = 0;
            }
        }
    }

    void OnDoTTickFrozen(float deltaTime)
    {
        if (Duration > 0)
        {
            //fixed 1s for frozen
            if (mTimeInterval > 1)
            {
                DonViChienDau sourceUnit = null;
                if (ObjSrc != null)
                {
                    sourceUnit = ObjSrc.GetComponent<DonViChienDau>();
                }

                long dmg = Config.Damage;
                bool isCrit = false;
                if (Config.Crit > 0)
                {
                    int rd = Random.Range(0, 100);
                    if (rd < Config.Crit)
                    {
                        isCrit = true;
                        dmg = (long)(dmg * Config.CritDMG / 100);
                    }
                }

                if (isCrit == false)
                {
                    OwnerUnit.TakeDamage(dmg,
                        false, // is crit
                        sourceUnit, // source unit
                        false, // play hit sound
                        true); // show HUD
                }
                else
                {
                    OwnerUnit.TakeDamage(dmg,
                        isCrit, // is crit
                        sourceUnit, // source unit
                        false, // play hit sound
                        true, // show hud
                        false, // xuyen bat tu
                        false, // xuyen linken
                        true, // thong ke
                        false,  // isHeadshot
                        false, // sourceUnitIsBoss
                        false, // sourceUnitIsAir
                        false, // skipSourceUnitType
                        false, //isDanCuaNguoiChoi
                        Config.isEleCrit //isEleCrit
                        );
                }

                mTimeInterval = 0;
            }
        }
    }
}

public enum EffectType
{
    Stun,
    Pierce,
    Slow,
    DoT, // Damage over Time
    PureDam,
    Crit,
    Root, // cant move but still think
    Linken, // skip 1 damageObject
    Electric, // shock enemy portion dmg
    Flame, // damgage over time 
    Frozen, // slow time
    RageAtk,
    RageAtkSpd,
    BatTu,
    BuffAtk,
}

[System.Serializable]
public class EffectConfig
{
    public EffectType Type;
    public float Duration { get; set; }
    public long Damage;
    public float Param1;
    public float Param2;
    public float Param3;
    public float Crit = 0;
    public float CritDMG = 0;
    public bool isEleCrit = false;
    public EffectConfig Clone()
    {
        return new EffectConfig()
        {
            Type = this.Type,
            Duration = this.Duration,
            Damage = this.Damage,
            Param1 = this.Param1,
            Param2 = this.Param2,
            Param3 = this.Param3,
            Crit = this.Crit,
            CritDMG = this.CritDMG,
            isEleCrit = this.isEleCrit,
        };
    }
}
