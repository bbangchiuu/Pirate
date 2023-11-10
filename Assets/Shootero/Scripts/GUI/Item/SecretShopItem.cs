using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using UnityEngine.UI;
    using Hiker.Networks.Data.Shootero;

    public class SecretShopItem : MonoBehaviour
    {
        public ItemAvatar item;
        public Text goldText;
        public Text gemText;
        public MaterialAvatar mat;
        public BuffIcon buff;

        public Text discountText;
        public Text gemLabel;
        public Text goldLabel;
        public Transform grpGem;
        public Transform grpGold;

        public Button btnTradeIn;

        SecretShopItemData data;

        [GUIDelegate]
        public void OnBtnClick()
        {
            if (data != null)
            {
                if(data.GemCost > GameClient.instance.UInfo.Gamer.Gem)
                {
                    PopupBuyGem.Create();
                    return;
                }
                GameClient.instance.RequestBuyInSecretShop(data.ID);
            }
        }

        public void SetItem(SecretShopItemData data)
        {
            item.gameObject.SetActive(false);
            goldText.transform.parent.gameObject.SetActive(false);
            gemText.transform.parent.gameObject.SetActive(false);
            mat.gameObject.SetActive(false);
            buff.gameObject.SetActive(false);

            this.data = data;

            discountText.text = "-" + data.Discount + "%";
            if (data.GemCost > 0)
            {
                grpGem.gameObject.SetActive(true);
                gemLabel.text = string.Format("{0}", data.GemCost);
            }
            else
            {
                grpGem.gameObject.SetActive(false);
            }

            if (data.GoldCost > 0)
            {
                grpGold.gameObject.SetActive(true);
                goldLabel.text = string.Format("{0}", data.GoldCost);
            }
            else
            {
                grpGold.gameObject.SetActive(false);
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
        }
    }
}