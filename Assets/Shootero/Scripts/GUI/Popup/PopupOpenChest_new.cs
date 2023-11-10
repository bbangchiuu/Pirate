using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;
    using UnityEngine.UI;
    public class PopupOpenChest_new : PopupBase
    {
        public Text lbItemName;
        public Text lbDesc;
        public Text lbRarity;

        public PhanThuongItem phanThuongItem;
        public TweenScale twScalePhanThuong;

        public GameObject[] ItemEff;

        public static PopupOpenChest_new instance;

        CardReward mReward;

        string[] rewardsItems;
        int mCurIndex = 0;
        int mCurQuantityNum = 0;

        public static PopupOpenChest_new Create(CardReward itemData)
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupOpenChest_new");
            instance = go.GetComponent<PopupOpenChest_new>();
            instance.Init(itemData);
            return instance;
        }

        private void Init(CardReward reward)
        {
            mReward = reward;
            rewardsItems = new string[reward.Keys.Count];
            int i = 0;
            foreach (var s in reward.Keys)
            {
                rewardsItems[i++] = s;
            }
            if (rewardsItems.Length > 0)
            {
                ShowReward(0, 0);
            }
            else
            {
                OnTouchClick();
            }
        }

        void ShowReward(int index, int quantity)
        {
            mCurIndex = index;

            var itemName = rewardsItems[index];

            if (quantity == 0)
            {
                quantity = mReward[itemName];
            }
            if (quantity == 0)
            {
                OnTouchClick();
                return;
            }

            if (itemName == CardReward.GEM_CARD)
            {
                lbRarity.text = string.Format(Localization.Get("QuantityLabel"), quantity);
            }
            else if (itemName == CardReward.GOLD_CARD)
            {
                lbRarity.text = string.Format(Localization.Get("QuantityLabel"), quantity);
            }
            else if (ConfigManager.Materials.ContainsKey(itemName))
            {
                lbRarity.text = string.Format(Localization.Get("QuantityLabel"), quantity);
            }
            else if (itemName.StartsWith("H_"))
            {
                lbRarity.text = string.Empty;
            }
            else if (itemName.StartsWith("C_"))
            {
                var cfgCfg = ConfigManager.GetCardConfig(itemName);
                lbRarity.text = Localization.Get(string.Format("Rariry_" + cfgCfg.Rarity.ToString()));
            }
            else
            {
                var rarity = ConfigManager.GetItemDefaultRarity(itemName);
                lbRarity.text = Localization.Get(string.Format("Rariry_" + rarity.ToString()));
            }


            //EZCameraShake.CameraShaker.Instance.ShakeOnce(1f, 1f, 0.1f, 0.5f);
            twScalePhanThuong.ResetToBeginning();
            twScalePhanThuong.enabled = true;
            twScalePhanThuong.PlayForward();
            phanThuongItem.SetItem(itemName, quantity);

            lbItemName.text = ConfigManager.GetDisplayPhanThuong(itemName);
            lbDesc.text = ConfigManager.GetMoTaPhanThuong(itemName);

            for (int i = 0; i < ItemEff.Length; i++)
            {
                if(ItemEff[i]!=null)
                    ItemEff[i].SetActive(false);
            }


            if (itemName == CardReward.GEM_CARD)
            {
                if (ItemEff[(int)(ERarity.Common)] != null)
                {
                    ItemEff[(int)(ERarity.Common)].SetActive(true);
                }
            }
            else if (itemName == CardReward.GOLD_CARD)
            {
                if (ItemEff[(int)(ERarity.Common)] != null)
                {
                    ItemEff[(int)(ERarity.Common)].SetActive(true);
                }
            }
            else if (ConfigManager.ItemStats.ContainsKey(itemName))
            {
                ERarity rarity = ConfigManager.GetItemDefaultRarity(itemName);
                if (ItemEff[(int)(rarity)] != null)
                {
                    ItemEff[(int)(rarity)].SetActive(true);
                }
            }
            else
            {
                if (ItemEff[(int)(ERarity.Common)] != null)
                {
                    ItemEff[(int)(ERarity.Common)].SetActive(true);
                }
            }
            
            //transform.localScale = new Vector3(0, 1, 1);
            //var tween = TweenScale.Begin(gameObject, 0.15f, Vector3.one);
        }

        [GUIDelegate]
        public void OnTouchClick()
        {
            int remain = mCurQuantityNum - 1;
            if (remain > 0)
            {
                ShowReward(mCurIndex, remain);
            }
            else
            {
                if (mCurIndex < rewardsItems.Length - 1)
                {
                    ShowReward(mCurIndex + 1, 0);
                }
                else
                {
                    OnCloseBtnClick();
                }
            }
        }

        [GUIDelegate]
        public override void OnCloseBtnClick()
        {
            base.OnCloseBtnClick();

            HikerUtils.DoAction(GameClient.instance, () => GameClient.instance.CheckRewardInfo(), 0.5f, true);
        }
    }
}