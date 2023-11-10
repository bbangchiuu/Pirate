using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hiker.GUI.Shootero;

public class DonViNguoiChoi : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "GateCollider")
        {
            if (QuanlyNguoichoi.Instance.CurDungeonId == 0 && QuanlyNguoichoi.Instance.MissionID == 0)
            {
                // bai start khong chekc dieu kien loading
            }
            else
            {
                if (QuanlyNguoichoi.Instance && QuanlyNguoichoi.Instance.IsLoadingMission) return;
                if (QuanlyManchoi.instance != null && QuanlyManchoi.instance.IsLevelClear() == false) return;
                // neu cua dang dong -> khong di qua
                if (GateController.instance && GateController.instance.gameObject.activeInHierarchy) return;
                if (QuanlyNguoichoi.Instance && QuanlyNguoichoi.Instance.IsLevelClear == false) return;
            }
#if DEBUG
            Debug.Log("Player Pos " + transform.position.ToString());
            Debug.Log("GateCollider from scene " + other.gameObject.scene.name);
#endif
            other.gameObject.SetActive(false);
            if (QuanlyNguoichoi.Instance)
                QuanlyNguoichoi.Instance.OnPlayerExit();
        }
        else if (other.gameObject.name == "Angel")
        {
            other.enabled = false;
            if (QuanlyNguoichoi.Instance)
                QuanlyNguoichoi.Instance.OnPlayerGetAngel(other.gameObject);
        }
        else if (other.gameObject.name == "BlackSmith")
        {
            other.enabled = false;

            if (QuanlyNguoichoi.Instance)
            {
                QuanlyNguoichoi.Instance.OnPlayerGetBlackSmith(other.gameObject);
            }
        }
        else if (other.gameObject.name == "Devil")
        {
            other.enabled = false;

            if (QuanlyNguoichoi.Instance)
            {
                QuanlyNguoichoi.Instance.OnPlayerGetDevil(other.gameObject);
            }
        }
        else if (other.gameObject.name == "Trap")
        {
            other.enabled = false;

            if (QuanlyNguoichoi.Instance)
            {
                QuanlyNguoichoi.Instance.OnPlayerGetTrap(other.gameObject);
            }
        }
        else if (other.gameObject.name == "GreedyGoblin")
        {
            other.enabled = false;

            if (QuanlyNguoichoi.Instance)
            {
                QuanlyNguoichoi.Instance.OnPlayerGetGreedyGoblin(other.gameObject);
            }
        }
        else if (other.gameObject.name == "SecretShop")
        {
            other.enabled = false;

            if (QuanlyNguoichoi.Instance)
            {
                QuanlyNguoichoi.Instance.OnPlayerGetSecretShop(other.gameObject);
            }
        }
        else if (other.gameObject.name == "ExpMaster")
        {
            other.enabled = false;
            if (QuanlyNguoichoi.Instance)
            {
                QuanlyNguoichoi.Instance.OnPlayerGetExpMaster(other.gameObject);
            }
        }
        else if (other.gameObject.name == "RangeMinion")
        {
            other.enabled = false;
            if (QuanlyNguoichoi.Instance)
            {
                QuanlyNguoichoi.Instance.OnPlayerGetRangeMinion(other.gameObject);
            }
        }
        else if (other.gameObject.name == "StatMaster")
        {
            other.enabled = false;
            if (QuanlyNguoichoi.Instance)
            {
                QuanlyNguoichoi.Instance.OnPlayerGetStatMaster(other.gameObject);
            }
        }
        else if (other.gameObject.name == "KhongLoModifier")
        {
            other.enabled = false;
            PopupKhongLoModifier.Create(other.gameObject);
        }
        else if (other.gameObject.name == "MaCaRongModifier")
        {
            other.enabled = false;
            PopupMaCaRongModifier.Create(other.gameObject);
        }
        else if (other.gameObject.name == "PopoModifier")
        {
            other.enabled = false;
            PopupPopoModifier.Create(other.gameObject);
        }
        else if (other.gameObject.name == "FlashModifier")
        {
            other.enabled = false;
            PopupFlashModifier.Create(other.gameObject);
        }
        else if (other.gameObject.name == "InfernalModifier")
        {
            other.enabled = false;
            PopupInfernalModifier.Create(other.gameObject);
        }
        else if (other.gameObject.name == "DeathrattleModifier")
        {
            other.enabled = false;
            PopupDeathrattleModifier.Create(other.gameObject);
        }
        else if (other.gameObject.name == "CardHuntBoss")
        {
            other.enabled = false;
            PopupCardHuntBoss.Create(other.gameObject);
        }
        else if (other.gameObject.name == "CardHuntExp")
        {
            other.enabled = false;
            PopupCardHuntExp.Create(other.gameObject);
        }
        else if (other.gameObject.name == "NienThuExp")
        {
            other.enabled = false;
            PopupCardHuntExp.Create(other.gameObject);
        }
        else
        {
            var chest = other.gameObject.GetComponent<BattleChest>();
            if (chest != null)
            {
                other.enabled = false;

                if (QuanlyNguoichoi.Instance)
                    QuanlyNguoichoi.Instance.OnPlayerGetChest(chest);
            }

            var npc = other.gameObject.GetComponent<FightingNPC>();
            if (npc != null)
            {
                other.enabled = false;
                if (QuanlyNguoichoi.Instance)
                {
                    QuanlyNguoichoi.Instance.OnPlayerGetFightingNPC(npc);
                }
            }
        }
    }
}
