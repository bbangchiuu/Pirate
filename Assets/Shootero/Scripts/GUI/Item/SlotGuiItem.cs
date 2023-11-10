using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SlotGuiItem : MonoBehaviour
{
    public SlotInventory slot;
    public BattleItem item;

    public Image shapeIcon;
    public Image itemIcon;

    public TMPro.TMP_Text lvText;
    public TMPro.TMP_Text nameText;

    public BuffGuiIcon[] buffIcons;

    public void Init(SlotInventory slotInventory)
    {
        this.slot = slotInventory;
    }

    public void SetItem(BattleItem battleItem = null)
    {
        item = battleItem;
        if (battleItem == null) 
        {
            shapeIcon.gameObject.SetActive(false);
            itemIcon.gameObject.SetActive(false);
            lvText.gameObject.SetActive(false);
            nameText.gameObject.SetActive(false);
        } 
        else
        {
            if (battleItem.codeName.StartsWith("Charm"))
            {
                shapeIcon.sprite = ConfigManager.GetIconSprite("icon_" + battleItem.stat.shape);
                itemIcon.sprite = ConfigManager.GetIconSprite("icon_" + battleItem.stat.buffStat.Type.ToString());
                shapeIcon.gameObject.SetActive(true);
            }
            else
            {
                shapeIcon.gameObject.SetActive(false);
                itemIcon.sprite = ConfigManager.GetIconSprite("icon_" + battleItem.codeName);
            }
            itemIcon.gameObject.SetActive(true);
            lvText.text = "Lv " + battleItem.tier.ToString();
            lvText.gameObject.SetActive(true);
            nameText.text = battleItem.codeName;
            nameText.gameObject.SetActive(true);
        }
    }

    public void SetBuff()
    {
        for(int i = 0; i < buffIcons.Length; ++i)
        {
            buffIcons[i].gameObject.SetActive(false);
        }
        int j = 0;
        foreach (var buff in slot.GetSlotBuffs().Keys)
        {
            if(j < buffIcons.Length)
            {
                buffIcons[j].SetIcon(ConfigManager.GetIconSprite("icon_" + slot.GetSlotBuffs()[buff].Type.ToString()));
                buffIcons[j].gameObject.SetActive(true);
                j++;
            }
        }
    }
}
