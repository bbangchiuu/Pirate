using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;
    using UnityEngine.UI;
    public class PopupOpenChest : PopupBase
    {
        public Text lbItemName;
        public Text lbDesc;
        
        public ItemAvatar avatar;
        public Text lbRarity;

        public MaterialAvatar matAva;

        public Transform GemAvatar;
        public Transform GoldAvatar;

        public static PopupOpenChest instance;

        CardReward mReward;

        string[] rewardsItems;
        int mCurIndex = 0;
        int mCurQuantityNum = 0;

        public static PopupOpenChest Create(CardReward itemData)
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupOpenChest");
            instance = go.GetComponent<PopupOpenChest>();
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

            GemAvatar.gameObject.SetActive(itemName == CardReward.GEM_CARD);
            GoldAvatar.gameObject.SetActive(itemName == CardReward.GOLD_CARD);

            mCurQuantityNum = 0;

            if (itemName == CardReward.GEM_CARD)
            {
                matAva.gameObject.SetActive(false);
                avatar.gameObject.SetActive(false);
                lbRarity.text = string.Format(Localization.Get("QuantityLabel"), quantity);
            }
            else if (itemName == CardReward.GOLD_CARD)
            {
                matAva.gameObject.SetActive(false);
                avatar.gameObject.SetActive(false);
                lbRarity.text = string.Format(Localization.Get("QuantityLabel"), quantity);
            }
            else if (ConfigManager.Materials.ContainsKey(itemName))
            {
                mCurQuantityNum = 0;
                matAva.gameObject.SetActive(true);
                avatar.gameObject.SetActive(false);
                matAva.SetItem(itemName, quantity);

                lbRarity.text = string.Format(Localization.Get("QuantityLabel"), quantity);
            }
            else
            {
                mCurQuantityNum = quantity;
                matAva.gameObject.SetActive(false);
                avatar.gameObject.SetActive(true);
                var rarity = ConfigManager.GetItemDefaultRarity(itemName);
                int level = 1;
                avatar.SetItem(itemName, rarity, level);
                lbRarity.text = Localization.Get(string.Format("Rariry_" + rarity.ToString()));
            }

            lbItemName.text = ConfigManager.GetDisplayPhanThuong(itemName);
            lbDesc.text = ConfigManager.GetMoTaPhanThuong(itemName);

            transform.localScale = new Vector3(0, 1, 1);
            var tween = TweenScale.Begin(gameObject, 0.15f, Vector3.one);
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