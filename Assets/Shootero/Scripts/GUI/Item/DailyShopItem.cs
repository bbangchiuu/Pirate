using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using UnityEngine.UI;
    using Hiker.Networks.Data.Shootero;

    public class DailyShopItem : MonoBehaviour
    {
        public ItemAvatar item;
        public Text goldText;
        public Text gemText;
        public MaterialAvatar mat;
        public BuffIcon buff;

        public Text discountText;
        public Text gemLabel;
        public Text goldLabel;
        public Text candyLabel;
        public Transform grpGem;
        public Transform grpGold;
        public Transform grpCandy;

        public Button btnTradeIn;
        public Text lbPurchased;

        DailyShopItemData data;
        int mItemIdx;
        System.DateTime mCrResetTime;

        [GUIDelegate]
        public void OnBtnBuyClick()
        {
            if (data != null)
            {
                if(grpCandy.gameObject.activeSelf == false)
                    GameClient.instance.RequestBuyDailyShopItem(mItemIdx, mCrResetTime);
                else
                    GameClient.instance.RequestBuyDailyShopItemByCandy(mItemIdx, mCrResetTime);
            }
        }

        public void SetItem(DailyShopItemData data, int idx, System.DateTime crResetTime, bool isInHalloween = false)
        {
            item.gameObject.SetActive(false);
            goldText.transform.parent.gameObject.SetActive(false);
            gemText.transform.parent.gameObject.SetActive(false);
            mat.gameObject.SetActive(false);
            buff.gameObject.SetActive(false);

            this.data = data;
            mItemIdx = idx;
            mCrResetTime = crResetTime;

            discountText.text = "-" + data.Discount + "%";
            if (isInHalloween)
            {
                int CandyCost = data.GetCandyCost(GameClient.instance.UInfo.GetCurrentChapter());
                candyLabel.text = string.Format("{0}", CandyCost);
                btnTradeIn.interactable = PopupDailyShop.instance != null && PopupDailyShop.instance.HaveCandy >= CandyCost;

                grpCandy.gameObject.SetActive(true);
                grpGem.gameObject.SetActive(false);
                grpGold.gameObject.SetActive(false);
            }
            else
            {
                grpCandy.gameObject.SetActive(false);
                if (data.GemCost > 0)
                {
                    grpGem.gameObject.SetActive(true);
                    gemLabel.text = string.Format("{0}", data.GemCost);
                    btnTradeIn.interactable = (GameClient.instance.UInfo.Gamer.Gem >= data.GemCost);
                }
                else
                {
                    grpGem.gameObject.SetActive(false);
                }

                if (data.GoldCost > 0)
                {
                    grpGold.gameObject.SetActive(true);
                    goldLabel.text = string.Format("{0}", data.GoldCost);
                    btnTradeIn.interactable = (GameClient.instance.UInfo.Gamer.Gold >= data.GoldCost);
                }
                else
                {
                    grpGold.gameObject.SetActive(false);
                }
            }

            if (data.Item.StartsWith("Buff_"))
            {
                var buffName = data.Item.Substring(5);

                if (System.Enum.TryParse(buffName, out BuffType buffType))
                {
                    buff.gameObject.SetActive(true);
                    buff.SetBuffType(buffType);
                }
                else
                {
                    buff.gameObject.SetActive(false);
                }
            }
            else if (data.Item.StartsWith("M_"))
            {
                mat.SetItem(data.Item, data.ItemCount);
                mat.gameObject.SetActive(true);
            }
            else if (data.Item == "Gold")
            {
                goldText.text = data.ItemCount + "";
                goldText.transform.parent.gameObject.SetActive(true);
            }
            else if (data.Item == "Gem")
            {
                gemText.text = data.ItemCount + "";
                gemText.transform.parent.gameObject.SetActive(true);
            }
            else
            {
                item.SetItem(data.Item, ConfigManager.GetItemDefaultRarity(data.Item), data.ItemCount);

                item.gameObject.SetActive(true);
            }
            lbPurchased.gameObject.SetActive(data.BuyCount > 0);
            btnTradeIn.gameObject.SetActive(data.BuyCount <= 0);
        }
    }
}