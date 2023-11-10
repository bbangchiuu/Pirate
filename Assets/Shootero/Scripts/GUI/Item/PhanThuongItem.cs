using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;

    public class PhanThuongItem : MonoBehaviour
    {
        public ItemAvatar itemAva;
        public HeroAvatar heroAva;
        public MaterialAvatar matAva;
        public Transform cardChest;
        public Text lbCardChest;
        public Transform GemGrp;
        public Text lbGem;
        public Transform GoldGrp;
        public Text lbGold;
        public CardAvatar cardAvatar;

        public void SetItem(string item, long quantity)
        {
            if (quantity < 0)
            {
                if (item == CardReward.GOLD_CARD)
                    quantity = -quantity * ConfigManager.GetBasedGoldOffer(GameClient.instance.UInfo.GetCurrentChapter());
                else if (item.StartsWith("M_"))
                    quantity = -quantity * ConfigManager.GetBasedMaterialOffer(GameClient.instance.UInfo.GetCurrentChapter());
            }

            if (cardAvatar) cardAvatar.gameObject.SetActive(false);

            if (item == CardReward.CARD_CHEST)
            {
                if(cardChest) cardChest.gameObject.SetActive(true);
                heroAva.gameObject.SetActive(false);
                GemGrp.gameObject.SetActive(false);
                GoldGrp.gameObject.SetActive(false);
                itemAva.gameObject.SetActive(false);
                matAva.gameObject.SetActive(false);
                lbCardChest.text = string.Format(Localization.Get("ShortQuantityLabel"), quantity);
            }
            else if (item == CardReward.GEM_CARD)
            {
                if (cardChest) cardChest.gameObject.SetActive(false);
                heroAva.gameObject.SetActive(false);
                GemGrp.gameObject.SetActive(true);
                GoldGrp.gameObject.SetActive(false);
                itemAva.gameObject.SetActive(false);
                matAva.gameObject.SetActive(false);
                lbGem.text = string.Format(Localization.Get("ShortQuantityLabel"), quantity);
            }
            else if (item == CardReward.GOLD_CARD)
            {
                if (cardChest) cardChest.gameObject.SetActive(false);
                heroAva.gameObject.SetActive(false);
                GemGrp.gameObject.SetActive(false);
                GoldGrp.gameObject.SetActive(true);
                itemAva.gameObject.SetActive(false);
                matAva.gameObject.SetActive(false);
                lbGold.text = string.Format(Localization.Get("ShortQuantityLabel"), quantity);
            }
            else if (ConfigManager.ItemStats.ContainsKey(item))
            {
                if (cardChest) cardChest.gameObject.SetActive(false);
                heroAva.gameObject.SetActive(false);
                GemGrp.gameObject.SetActive(false);
                GoldGrp.gameObject.SetActive(false);
                itemAva.gameObject.SetActive(true);
                matAva.gameObject.SetActive(false);
                itemAva.SetItem(item, ConfigManager.GetItemDefaultRarity(item), 1);
            }
            else if (item.StartsWith("H_"))
            {
                if (cardChest) cardChest.gameObject.SetActive(false);
                heroAva.gameObject.SetActive(true);
                GemGrp.gameObject.SetActive(false);
                GoldGrp.gameObject.SetActive(false);
                itemAva.gameObject.SetActive(false);
                matAva.gameObject.SetActive(false);
                heroAva.SetItem(item.Substring(2));
                heroAva.IsSelected = false;
                heroAva.IsLocked = false;
                heroAva.IsEquiped = false;
            }
            else if (item.StartsWith("C_") && cardAvatar)
            {
                if (cardChest) cardChest.gameObject.SetActive(false);
                heroAva.gameObject.SetActive(false);
                GemGrp.gameObject.SetActive(false);
                GoldGrp.gameObject.SetActive(false);
                itemAva.gameObject.SetActive(false);
                matAva.gameObject.SetActive(false);

                cardAvatar.gameObject.SetActive(true);
                var cardName = item;
                CardConfig cardCfg = ConfigManager.CardStats[cardName];
                cardAvatar.SetItem(cardName, cardCfg.Rarity);
                var itemBtn = cardAvatar.GetComponent<Button>();
                itemBtn.onClick.RemoveAllListeners();
                itemBtn.onClick.AddListener(() => { PopupCardInfo.CreateViewOnly(cardName); });
            }
            else
            {
                if (cardChest) cardChest.gameObject.SetActive(false);
                heroAva.gameObject.SetActive(false);
                GemGrp.gameObject.SetActive(false);
                GoldGrp.gameObject.SetActive(false);
                itemAva.gameObject.SetActive(false);
                matAva.gameObject.SetActive(true);
                matAva.SetItem(item, quantity);
            }
        }
    }
}

