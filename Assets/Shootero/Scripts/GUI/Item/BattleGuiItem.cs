using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleGuiItem : MonoBehaviour
{
    public BattleItem item;

    public void Init(BattleItem battleItem)
    {
        item = battleItem;
    }
}
