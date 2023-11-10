using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Hiker.Networks.Data.Shootero;

namespace Hiker.GUI.Shootero
{
    using UnityEngine.UI;

    public class PopupShowPhanThuongTop : PopupBase
    {
        public PhanThuongItem[] items;

        public static PopupShowPhanThuongTop instance;

        public static PopupShowPhanThuongTop Create(CardReward rewards)
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupShowPhanThuongTop");
            instance = go.GetComponent<PopupShowPhanThuongTop>();
            instance.Init(rewards);

            return instance;
        }

        public void Init(CardReward rewards)
        {
            var contentEnum = rewards.GetEnumerator();

            for (int i = 0; i < items.Length; ++i)
            {
                if (i < rewards.Count)
                {
                    contentEnum.MoveNext();
                    var curReward = contentEnum.Current;
                    int quantity = curReward.Value;
                    if (quantity < 0)
                    {
                        if (curReward.Key == CardReward.GOLD_CARD)
                            quantity = -quantity * ConfigManager.GetBasedGoldOffer(GameClient.instance.UInfo.GetCurrentChapter());
                        else if (curReward.Key.StartsWith("M_"))
                            quantity = -quantity * ConfigManager.GetBasedMaterialOffer(GameClient.instance.UInfo.GetCurrentChapter());
                    }

                    items[i].gameObject.SetActive(true);
                    items[i].SetItem(curReward.Key, quantity);
                }
                else
                {
                    items[i].gameObject.SetActive(false);
                }
            }
        }
    }

}

