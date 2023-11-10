using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricField : UnitAura
{
    public static ElectricField AddFieldToUnit(DonViChienDau unit)
    {
        var field = unit.gameObject.AddMissingComponent<ElectricField>();
        field.listEffect.Clear();

        var lvl = unit.GetBuffCount(BuffType.ELECTRIC_FIELD);
        if (lvl > 0)
        {
            var buffstat = QuanlyNguoichoi.Instance.GetBuffStatByType(BuffType.ELECTRIC_FIELD);
            var rangeParamIndex = (lvl - 1) * 3; // 0 la` chi so range ban dau, tu lvl 1 (moi level them 2 chi so trong param (range, dmg) )
            field.range = buffstat.Params[rangeParamIndex];
            field.interval = 1f;
            if (unit == QuanlyNguoichoi.Instance.PlayerUnit)
            {
                var listMods = QuanlyNguoichoi.Instance.GetListRuntimeStatMods();
                if (listMods != null)
                {
                    var mod = listMods.Find(e => e.Stat == EStatType.SKILLPLUS && e.Target == "Field");
                    if (mod != null && mod.Val > 0)
                    {
                        field.range += buffstat.Params[rangeParamIndex] * (float)mod.Val / 100f;
                    }
                }
            }
            var eff = new EffectConfig()
            {
                Type = EffectType.Electric,
                Damage = Mathf.CeilToInt(buffstat.Params[2 + (lvl - 1) * 3] * unit.GetCurDPS() * unit.GetElementDMGRate() / 100f),
                Param1 = 0, // buffstat.Params[3], // count unit get
                Param2 = 0, // buffstat.Params[4], // range unit lan
                Duration = 0.1f,
            };
            field.listEffect.Add(eff);
        }

        return field;
    }

    protected override void OnAuraExecute()
    {
        if (unit == null || unit.IsAlive() == false) return;
        var count = unit.GetBuffCount(BuffType.ELECTRIC_FIELD);
        if (count == 0)
        {
            enabled = false;
            Destroy(this);
            return;
        }

        //Hiker.GUI.Shootero.ScreenBattle.instance.DisplayTextHud("ElectricField", unit);
        var buffStat = QuanlyNguoichoi.Instance.GetBuffStatByType(BuffType.ELECTRIC_FIELD);
        var listEnemies = QuanlyManchoi.FindEnemiesInRange(unit.transform.position, range,null,true);
        if (listEnemies == null) return;
        
        foreach (var t in listEnemies)
        {
            if (t != null && t.IsAlive())
            {
                var randomRate = buffStat.Params[1 + (count - 1) * 3];
                if (Random.Range(0, 100) < randomRate)
                {
                    foreach (var eff in listEffect)
                    {
                        var applyEff = new BattleEffect(eff);

                        if (applyEff.Config.Type == EffectType.Electric)
                        {
                            applyEff.Config.Damage = Mathf.CeilToInt(buffStat.Params[2 + (count - 1) * 3] * unit.GetCurDPS() * unit.GetElementDMGRate() / 100f);
                        }

                        if (QuanlyNguoichoi.Instance &&
                            QuanlyNguoichoi.Instance.PlayerUnit)
                        {
                            applyEff.ObjSrc = QuanlyNguoichoi.Instance.PlayerUnit.gameObject;

                            var buffCount = QuanlyNguoichoi.Instance.PlayerUnit.GetBuffCount(BuffType.ELEMENT_CRIT);
                            if(buffCount > 0)
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
            }
        }
        Hiker.Util.ListPool<DonViChienDau>.Release(listEnemies);
    }
}
