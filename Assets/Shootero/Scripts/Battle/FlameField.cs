using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameField : UnitAura
{
    public static FlameField AddFieldToUnit(DonViChienDau unit)
    {
        var field = unit.gameObject.AddMissingComponent<FlameField>();
        field.listEffect.Clear();

        var lvl = unit.GetBuffCount(BuffType.FLAME_FIELD);
        if (lvl > 0)
        {
            var buffstat = QuanlyNguoichoi.Instance.GetBuffStatByType(BuffType.FLAME_FIELD);
            field.range = buffstat.Params[(lvl - 1) * 3];
            if (unit == QuanlyNguoichoi.Instance.PlayerUnit)
            {
                var listMods = QuanlyNguoichoi.Instance.GetListRuntimeStatMods();
                if (listMods != null)
                {
                    var mod = listMods.Find(e => e.Stat == EStatType.SKILLPLUS && e.Target == "Field");
                    if (mod != null && mod.Val > 0)
                    {
                        field.range += buffstat.Params[(lvl - 1) * 3] * (float)mod.Val / 100f;
                    }
                }
            }
            var battleEff = new EffectConfig
            {
                Type = EffectType.Flame,
                Damage = Mathf.CeilToInt(unit.GetCurDPS() * unit.GetElementDMGRate() * buffstat.Params[1 + (lvl - 1) * 3] / 100),
                Param1 = 1, // tick dot
                Duration = buffstat.Params[2 + (lvl - 1) * 3] // tick dot
            };

            field.listEffect.Add(battleEff);
            field.interval = 0.1f;
        }

        return field;
    }

    protected override void OnAuraExecute()
    {
        if (unit == null || unit.IsAlive() == false) return;
        var count = unit.GetBuffCount(BuffType.FLAME_FIELD);
        if (count == 0)
        {
            enabled = false;
            Destroy(this);
            return;
        }
        //Hiker.GUI.Shootero.ScreenBattle.instance.DisplayTextHud("FlameField", unit);
        var buffStat = QuanlyNguoichoi.Instance.GetBuffStatByType(BuffType.FLAME_FIELD);
        var listEnemies = QuanlyManchoi.FindEnemiesInRange(unit.transform.position, range,null,true);
        if (listEnemies == null) return;
        foreach (var t in listEnemies)
        {
            foreach (var eff in listEffect)
            {
                var applyEff = new BattleEffect(eff);
                if (applyEff.Config.Type == EffectType.Flame)
                {
                    applyEff.Config.Damage = Mathf.CeilToInt(unit.GetCurDPS() * unit.GetElementDMGRate() * buffStat.Params[1 + (count - 1) * 3] / 100);
                }

                if (QuanlyNguoichoi.Instance &&
                    QuanlyNguoichoi.Instance.PlayerUnit)
                {
                    applyEff.ObjSrc = QuanlyNguoichoi.Instance.PlayerUnit.gameObject;

                    var buffCount = QuanlyNguoichoi.Instance.PlayerUnit.GetBuffCount(BuffType.ELEMENT_CRIT);
                    if (buffCount > 0)
                    {
                        UnitStat stats = QuanlyNguoichoi.Instance.PlayerUnit.GetCurStat();
                        applyEff.Config.Crit = stats.CRIT;
                        applyEff.Config.CritDMG = stats.CRIT_DMG;
                        applyEff.Config.isEleCrit = true;
                    }
                    else
                    {
                        applyEff.Config.Crit = 0;
                        applyEff.Config.CritDMG = 0;
                        applyEff.Config.isEleCrit = false;
                    }
                }

                t.GetStatusEff().ApplyEffect(applyEff);
            }
        }
        Hiker.Util.ListPool<DonViChienDau>.Release(listEnemies);
    }
}
