using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;
    using UnityEngine.UI;
    public class PopupChestInfo : PopupBase
    {
        public Text lbChestDesc;
        public Image[] chestAvatars;

        public static PopupChestInfo instance;

        public static PopupChestInfo Create(string chestName)
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupChestInfo");
            instance = go.GetComponent<PopupChestInfo>();
            instance.Init(chestName);
            return instance;
        }

        private void Init(string chestName)
        {
            //for (int i = 0; i < lbChestDesc.Length; ++i)
            {
                //var lbDesc = lbChestDesc[i];
                //var chestName = chestNames[i];
                var chestCfg = ConfigManager.ChestCfg[chestName];
                List<int> drops = new List<int>();
                for (int d = chestCfg.dropRates.Length - 1; d >= 0; --d)
                {
                    if (chestCfg.dropRates[d] > 0)
                    {
                        drops.Add(chestCfg.dropRates[d]);
                    }
                }
                string localize = Localization.Get("ChestDesc_" + chestName);
                var args = new object[drops.Count];
                for (int i = 0; i < drops.Count; ++i)
                {
                    args[i] = drops[i];
                }

                lbChestDesc.text = string.Format(localize, args);
            }
            foreach (var t in chestAvatars)
            {
                if (t != null)
                {
                    t.gameObject.SetActive(t.name == chestName);
                }
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
        }
    }
}