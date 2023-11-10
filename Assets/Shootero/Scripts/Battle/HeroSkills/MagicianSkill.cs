using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

public class MagicianSkill : TuyetKy
{
    enum EffectHit
    {
        None = 0,
        Lightning = 1,
        Flame = 2,
        Frozen = 3
    }
    int effect = 0;
    int nextEffect = 0;
    EffectConfig effConfig = new EffectConfig();
    PhapSuVisual visual;

    int firedCount = 0;

    void RandomEffect()
    {
        if (firedCount == 0)
        {
            nextEffect = Random.Range(1, 4);
            //nextEffect = 1;
#if DEBUG
            //Hiker.GUI.Shootero.ScreenBattle.instance.DisplayTextHud(((EffectHit)nextEffect).ToString(), Unit);
#endif
        }
        else
        {
            nextEffect = 0;
        }
        // TODO: doi visual hit ban tiep theo o day
    }

    public override void OnStart()
    {
        base.OnStart();
        visual = Unit.GetComponent<PhapSuVisual>();
        Loai = LoaiTK.BiDong;
        TKName = "MagicianSkill";

        firedCount = 1;
        effect = 0;
        RandomEffect();
    }

    void SetupLightningEffect()
    {
        var buffstat = QuanlyNguoichoi.Instance.GetBuffStatByType(BuffType.ELECTRIC_EFF);

        int eleCritBuffCount = 0;
        if (Unit == QuanlyNguoichoi.Instance.PlayerUnit)
        {
            eleCritBuffCount = QuanlyNguoichoi.Instance.PlayerUnit.GetBuffCount(BuffType.ELEMENT_CRIT);
        }

        if (effConfig.Type != EffectType.Electric)
        {
            effConfig.Type = EffectType.Electric;
            effConfig.Param1 = buffstat.Params[2]; // count unit get
            effConfig.Param2 = buffstat.Params[3]; // range unit lan
            effConfig.Duration = 0.6f;
        }
        var dmg = Mathf.CeilToInt(Unit.GetCurDPS() * Unit.GetElementDMGRate() * buffstat.Params[1] / 100);

        if (eleCritBuffCount > 0)
        {
            UnitStat stats = QuanlyNguoichoi.Instance.PlayerUnit.GetCurStat();
            effConfig.Crit = stats.CRIT;
            effConfig.CritDMG = stats.CRIT_DMG;
            effConfig.isEleCrit = true;
        }
        else
        {
            effConfig.Crit = 0;
            effConfig.CritDMG = 0;
            effConfig.isEleCrit = false;
        }

        effConfig.Damage = dmg;
    }

    void SetupFlameEffect()
    {
        var buffstat = QuanlyNguoichoi.Instance.GetBuffStatByType(BuffType.FLAME_EFF);

        int eleCritBuffCount = 0;
        if (Unit == QuanlyNguoichoi.Instance.PlayerUnit)
        {
            eleCritBuffCount = QuanlyNguoichoi.Instance.PlayerUnit.GetBuffCount(BuffType.ELEMENT_CRIT);
        }

        if (effConfig.Type != EffectType.Flame)
        {
            effConfig.Type = EffectType.Flame;
            effConfig.Param1 = buffstat.Params[1];
            effConfig.Duration = buffstat.Params[2];
            effConfig.Param2 = 0;
        }

        effConfig.Damage = Mathf.CeilToInt(Unit.GetCurStat().DMG * Unit.GetElementDMGRate() * buffstat.Params[0] / 100);

        if (eleCritBuffCount > 0)
        {
            UnitStat stats = QuanlyNguoichoi.Instance.PlayerUnit.GetCurStat();
            effConfig.Crit = stats.CRIT;
            effConfig.CritDMG = stats.CRIT_DMG;
            effConfig.isEleCrit = true;
        }
        else
        {
            effConfig.Crit = 0;
            effConfig.CritDMG = 0;
            effConfig.isEleCrit = false;
        }
    }

    void SetupFrozenEffect()
    {
        var buffstat = QuanlyNguoichoi.Instance.GetBuffStatByType(BuffType.FROZEN_EFF);

        int eleCritBuffCount = 0;
        if (Unit == QuanlyNguoichoi.Instance.PlayerUnit)
        {
            eleCritBuffCount = QuanlyNguoichoi.Instance.PlayerUnit.GetBuffCount(BuffType.ELEMENT_CRIT);
        }

        if (effConfig.Type != EffectType.Frozen)
        {
            effConfig.Type = EffectType.Frozen;
            effConfig.Param1 = buffstat.Params[0];
            effConfig.Duration = buffstat.Params[1];
            effConfig.Damage = 0;
            effConfig.Param2 = 0;
        }

        if (eleCritBuffCount > 0)
        {
            UnitStat stats = QuanlyNguoichoi.Instance.PlayerUnit.GetCurStat();
            effConfig.Crit = stats.CRIT;
            effConfig.CritDMG = stats.CRIT_DMG;
            effConfig.isEleCrit = true;

            effConfig.Damage = Mathf.CeilToInt(Unit.GetCurStat().DMG * Unit.GetElementDMGRate() * buffstat.Params[2] / 100);
        }
        else
        {
            effConfig.Crit = 0;
            effConfig.CritDMG = 0;
            effConfig.isEleCrit = false;
            effConfig.Damage = 0;
        }
    }

    EffectConfig GetEffect(int effect)
    {
        switch (effect)
        {
            case 1:
                SetupLightningEffect();
                return effConfig;
            case 2:
                SetupFlameEffect();
                return effConfig;
            case 3:
                SetupFrozenEffect();
                return effConfig;
            default:
                return null;
        }
    }

    public GameObject GetVisualPrefab()
    {
        switch (effect)
        {
            case 1:
            case 2:
            case 3:
                return visual.MagicParticle[effect - 1];
            default:
                return null;
        }
    }

    public override EffectConfig GetEffectFromSkill()
    {
        return GetEffect(effect);
    }

    public override void OnBeforeUnitFired(Transform target)
    {
        effect = nextEffect;

        base.OnBeforeUnitFired(target);
    }

    public override void OnUnitActiveProj(Vector3 target)
    {
        int pattern = ConfigManager.GetHeroSkillParams(TKName, 0);
        firedCount = (firedCount + 1) % pattern;
        RandomEffect();
    }
}
