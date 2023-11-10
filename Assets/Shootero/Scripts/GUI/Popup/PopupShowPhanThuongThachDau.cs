using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Hiker.Networks.Data.Shootero;

namespace Hiker.GUI.Shootero
{
    using UnityEngine.UI;

    public class PopupShowPhanThuongThachDau : PopupBase
    {
        public int itemEachRow = 3;
        public PhanThuongItem[] items;
        public Image hightlight;
        public ScrollRect scroll;

        public static PopupShowPhanThuongThachDau instance;

        public static PopupShowPhanThuongThachDau Create(int numPlayer)
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupShowPhanThuongThachDau");
            instance = go.GetComponent<PopupShowPhanThuongThachDau>();
            instance.Init(numPlayer);

            return instance;
        }

        public void Init(int numPlayer)
        {
            //int index = (5 - numPlayer) * itemEachRow + (pos - 1);

            //Vector3 posHightLight = Vector3.zero;
            
            for (int i = 0; i < items.Length; ++i)
            {
                var r = i / itemEachRow;
                var c = i % itemEachRow;

                var group = 5 - r;
                var reward = ConfigManager.ThachDauCfg.GetTopRewards(group, c);
                if (reward == null)
                {
                    items[i].transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    var cursor = reward.GetEnumerator();
                    if (cursor.MoveNext())
                    {
                        items[i].transform.parent.gameObject.SetActive(true);
                        items[i].SetItem(cursor.Current.Key, cursor.Current.Value);
                    }
                    else
                    {
                        items[i].transform.parent.gameObject.SetActive(false);
                    }
                }

                if (group == numPlayer && items[i].transform.parent.gameObject.activeSelf)
                {
                    var groupTrans = items[i].transform.parent.parent as RectTransform;
                    hightlight.gameObject.SetActive(true);
                    hightlight.rectTransform.position = groupTrans.position;
                    hightlight.rectTransform.SetParent(items[i].transform.parent);
                    if (scroll)
                        scroll.verticalNormalizedPosition = 0.5f;

                    Hiker.HikerUtils.DoAction(this, () =>
                    {
                        if (scroll)
                            scroll.verticalNormalizedPosition = 0.5f;
                        hightlight.rectTransform.position = groupTrans.position;
                        //hightlight.rectTransform.SetParent(items[i].transform.parent);
                    }, 0.1f);
                }
            }

            //if (pos <= itemEachRow && index < items.Length)
            //{
            //    var it = items[index];
            //    if (it.gameObject.activeSelf)
            //    {
            //        hightlight.gameObject.SetActive(true);
            //        //var posTrans = it.transform.parent as RectTransform;
            //        hightlight.rectTransform.position = it.transform.parent.position;

            //        Hiker.HikerUtils.DoAction(this, () =>
            //        {
            //            hightlight.rectTransform.position = it.transform.parent.position;
            //        }, 0.1f);
            //    }
            //    else
            //    {
            //        hightlight.gameObject.SetActive(false);
            //    }
            //}
            //else
            //{
            //    hightlight.gameObject.SetActive(false);
            //}

            //var contentEnum = rewards.GetEnumerator();

            //for (int i = 0; i < items.Length; ++i)
            //{
            //    if (i < rewards.Count)
            //    {
            //        contentEnum.MoveNext();
            //        var curReward = contentEnum.Current;
            //        int quantity = curReward.Value;
            //        if (quantity < 0)
            //        {
            //            if (curReward.Key == CardReward.GOLD_CARD)
            //                quantity = -quantity * ConfigManager.GetBasedGoldOffer(GameClient.instance.UInfo.GetCurrentChapter());
            //            else if (curReward.Key.StartsWith("M_"))
            //                quantity = -quantity * ConfigManager.GetBasedMaterialOffer(GameClient.instance.UInfo.GetCurrentChapter());
            //        }

            //        items[i].gameObject.SetActive(true);
            //        items[i].SetItem(curReward.Key, quantity);
            //    }
            //    else
            //    {
            //        items[i].gameObject.SetActive(false);
            //    }
            //}
        }
    }

}

