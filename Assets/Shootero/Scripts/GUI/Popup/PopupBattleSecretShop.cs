using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Hiker.Networks.Data.Shootero;

namespace Hiker.GUI.Shootero
{
    using UnityEngine.UI;

    public class PopupBattleSecretShop : PopupBase
    {
        public Transform itemParent;
        public SecretShopItem itemPrefabs;
        public Button cancelBtn;

        GameObject visual;

        public static PopupBattleSecretShop instance;

        List<SecretShopItemData> listItems;

        public static PopupBattleSecretShop Create(GameObject visual, List<SecretShopItemData> listItems)
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupBattleSecretShop");
            instance = go.GetComponent<PopupBattleSecretShop>();
            instance.Init(listItems);
            instance.visual = visual;
            return instance;
        }

        private void Init(List<SecretShopItemData> listItems)
        {
            this.listItems = listItems;

            foreach (var it in listItems)
            {
                var item = Instantiate(itemPrefabs, itemParent);
                item.SetItem(it);
                item.gameObject.SetActive(true);
            }

            if (ResourceBar.instance)
            {
                ResourceBar.instance.gameObject.SetActive(true);
            }
        }

        private void Update()
        {
            if (ResourceBar.instance && ResourceBar.instance.gameObject.activeSelf == false)
            {
                ResourceBar.instance.gameObject.SetActive(true);
            }
        }

        public void OnTradeInSuccess(string id)
        {
            var item = listItems.Find(e => e.ID == id);
            if (item.Item.StartsWith("Buff_"))
            {
                var buffName = item.Item.Substring(5);
                if (System.Enum.TryParse(buffName, out BuffType buffType))
                {
                    if (QuanlyNguoichoi.Instance)
                    {
                        QuanlyNguoichoi.Instance.GetBuff(buffType);
                    }
                }
            }

            OnCloseBtnClick();
        }

        protected override void OnCleanUp()
        {
            if (visual)
            {
                visual.gameObject.SetActive(false);
            }

            QuanlyManchoi.instance.OnLevelClear();

            if (ResourceBar.instance)
            {
                ResourceBar.instance.gameObject.SetActive(false);
            }
        }
    }

}

