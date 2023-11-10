using Hiker.GUI.Shootero;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenTestBattle : MonoBehaviour
{
    
    public void ShowBattleInventory()
    {
        PopupBattleInventory.Create(QuanLyBattleItem.instance.inventory);
    }
}
