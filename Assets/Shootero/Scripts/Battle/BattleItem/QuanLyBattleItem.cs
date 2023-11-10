using Hiker.GUI.Shootero;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuanLyBattleItem : MonoBehaviour
{
    //Inventory
    public List<BattleItem> items;
    public BattleInventory inventory;
    //Artifact
    //public Artifact artifact;

    public static QuanLyBattleItem instance;

    public void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        items = ConfigManager.BattleItemCfg.GetTestItem();

        inventory.InitBattleInventory(items);

        if(QuanlyNguoichoi.Instance != null && QuanlyNguoichoi.Instance.PlayerUnit != null)
        {
            QuanlyNguoichoi.Instance.PlayerUnit.UpdateShooterStat();
        }
    }

    public void AddBattleItem(BattleItem item, string posId = "")
    {
        if (items != null) items.Add(item);
        inventory.SetItemInPosition(posId, item, true);
    }

    public void RemoveItemFromBattle(string posId)
    {
        BattleItem item = items.Find(e => e.posId == posId);
        if (item != null) items.Remove(item);
    }

    public List<WeaponStatConfig> GetWeaponWithBuff()
    {
        if(items == null || items.Count == 0) return null;
        List<WeaponStatConfig> listWeapon = new List<WeaponStatConfig>();
        if (items == null) return null;
        foreach (var item in items)
        {
            if (item.posId.StartsWith("Hand") == false)
            {
                var weaponStatCfg = WeaponStatConfig.FromWeaponStat(item.codeName);
                if (weaponStatCfg != null)
                {
                    WeaponStatConfig weapon = new WeaponStatConfig();
                    weapon.PROJ_N = weaponStatCfg.PROJ_N;
                    weapon.DMG = (long)weaponStatCfg.GetStatLevel(item.tier, EStatType.ATK);
                    weapon.ATK_SPD = (float)weaponStatCfg.GetStatLevel(item.tier, EStatType.ATK_SPD);
                    weapon.PROJ_SPD = weaponStatCfg.PROJ_SPD;
                    weapon.KNOCK_BACK = weaponStatCfg.KNOCK_BACK;
                    weapon.ATK_RANGE = weaponStatCfg.ATK_RANGE;
                    weapon.CRIT = (float)weaponStatCfg.GetStatLevel(item.tier, EStatType.CRIT);
                    weapon.CRIT_DMG = (float)weaponStatCfg.GetStatLevel(item.tier, EStatType.CRIT_DMG);
                    weapon.listBuff = weaponStatCfg.listBuff;

                    if (weapon.listBuff == null) weapon.listBuff = new List<BuffStat>();

                    var buffs = inventory.slotDic[item.posId].GetSlotBuffs();
                    if (buffs.Keys.Count > 0)
                    {
                        foreach (var key in buffs.Keys)
                        {
                            var tier = inventory.itemDic[key].tier;
                            var value = buffs[key].Params[tier];

                            if (weapon.listBuff.Exists(e => e.Type == buffs[key].Type))
                            {
                                weapon.listBuff.Find(e => e.Type == buffs[key].Type).Params[0] += value;
                            }
                            else
                            {
                                weapon.listBuff.Add(buffs[key]);
                            }
                        }
                    }

                    listWeapon.Add(weapon);
                }
            }
        }

        return listWeapon;
    }
}
