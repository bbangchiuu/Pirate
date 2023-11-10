using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Hiker.Networks.Data.Shootero;

namespace Hiker.GUI.Shootero
{
    using UnityEngine.UI;

    public class PopupBossHunt : PopupBase
    {
        public BossHuntItem[] Items;
        public static PopupBossHunt instance;

        public static PopupBossHunt Create()
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupBossHunt");
            instance = go.GetComponent<PopupBossHunt>();
            instance.Init();

            return instance;
        }

        public void Init()
        {
            //var listBossHunt = GameClient.instance.UInfo.ListBossHunts.FindAll(e => e.Status != 2);
            //listBossHunt.Sort((e1, e2) => e2.Status.CompareTo(e1.Status));
            List<BossHuntData> listBossHunt = GameClient.instance.UInfo.ListBossHunts.FindAll(e => e.Status == 1);
            List<BossHuntData> listBostOpen = GameClient.instance.UInfo.ListBossHunts.FindAll(e => e.Status == 0);

            listBossHunt.AddRange(listBostOpen);

            for (int i = 0, count = 0; i < Items.Length; ++i)
            {
                var item = Items[i];
                if (item != null)
                {
                    bool isset = false;
                    while (count < listBossHunt.Count)
                    {
                        if (item.SetItem(listBossHunt[count++]))
                        {
                            isset = true;
                            break;
                        }
                    }
                    item.gameObject.SetActive(isset);
                }
            }
        }
    }

}

