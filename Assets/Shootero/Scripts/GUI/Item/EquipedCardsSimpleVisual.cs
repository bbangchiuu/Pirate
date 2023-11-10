using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipedCardsSimpleVisual : MonoBehaviour
{
    public List<Image> iconSlots;
    Color[] slotColors =
    {
        new Color(0.18f, 0.18f, 0.18f),
        new Color(0.77f, 0.77f, 0.77f),
        new Color(0.384f, 1f, 0.212f),
        new Color(0.18f, 0.788f, 1f),
        new Color(0.976f, 0f, 1f),
        new Color(1f, 0.835f, 0f)
    };
    public void ShowItemSlots(List<int> slotStatus)
    {
        for(int i= 0; i < iconSlots.Count; i++)
        {
            if (i < slotStatus.Count)
            {
                if (slotStatus[i] > 0)
                {
                    iconSlots[i].color = slotColors[slotStatus[i] + 1];
                    iconSlots[i].gameObject.SetActive(true);
                }
                else
                {
                    iconSlots[i].gameObject.SetActive(false);
                }
            }
            else
                iconSlots[i].gameObject.SetActive(false);
        }
    }
}
