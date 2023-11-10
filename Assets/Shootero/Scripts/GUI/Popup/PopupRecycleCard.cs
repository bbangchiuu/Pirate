using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;
    using UnityEngine.UI;

    public class PopupRecycleCard : PopupBase
    {
        public Transform matParent;
        public MaterialAvatar matPrefab;

        public Transform itemParent;
        public CardAvatar itemPrefab;
        public Button btnFuse;

        public GameObject emptyItems;
        public GameObject emptyMaterials;

        public static PopupRecycleCard instance;

        List<MaterialAvatar> mListMaterials = new List<MaterialAvatar>();

        List<CardAvatar> mListItems = new List<CardAvatar>();
        List<CardAvatar> selectedFusingItems = new List<CardAvatar>();
        bool activeResourceBar = false;

        int SortByRarity(CardAvatar it1, CardAvatar it2)
        {
            if (it1 == null || it2 == null) return 0;
            var tb1 = GameClient.instance.UInfo.listCards.Find(e => e.ID == it1.TBId);
            var tb2 = GameClient.instance.UInfo.listCards.Find(e => e.ID == it2.TBId);
            if (tb1 == null || tb2 == null) return 0;

            var cfg1 = ConfigManager.CardStats[it1.TBName];
            var cfg2 = ConfigManager.CardStats[it2.TBName];
            var dif = (int)cfg2.Rarity - (int)cfg1.Rarity;
            if (dif == 0)
            {
                if (cfg1.Slot == cfg2.Slot)
                {
                    var n = tb1.Name.CompareTo(tb2.Name);
                    if (n == 0)
                    {
                        return tb1.ID.CompareTo(tb2.ID);
                    }
                    else
                    {
                        return n;
                    }
                }

                return cfg1.Slot - cfg2.Slot;
            }
            return dif;
        }

        private void SortItems()
        {
            mListItems.Sort(SortByRarity);

            for (int i = 0; i < mListItems.Count; ++i)
            {
                mListItems[i].transform.SetSiblingIndex(i);
            }
        }

        public static PopupRecycleCard Create()
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupRecycleCard");
            instance = go.GetComponent<PopupRecycleCard>();
            instance.Init();
            return instance;
        }

        private void InitItems()
        {
            //if (mInitedItems) return;
            if (GameClient.instance == null ||
                GameClient.instance.UInfo == null ||
                GameClient.instance.UInfo.listCards == null)
            {
                return;
            }

            foreach (var itemData in GameClient.instance.UInfo.listCards)
            {
                if (itemData != null)
                {
                    if (mListItems.Exists(e => e.TBId == itemData.ID) == false)
                    {
                        var itemAvatar = Instantiate(itemPrefab, itemParent);
                        itemAvatar.SetItem(itemData.Name, itemData.Rarity, itemData.ID);
                        itemAvatar.IsEquiped = IsEquipedCard(itemData.ID);
                        var itemBtn = itemAvatar.GetComponent<Button>();
                        itemBtn.onClick.AddListener(delegate { OnItemClick(itemAvatar); });
                        itemAvatar.gameObject.SetActive(true);
                        mListItems.Add(itemAvatar);
                    }
                }
            }
            
            itemPrefab.gameObject.SetActive(false);
            
        }

        bool IsEquipedCard(string cardID)
        {
            UserInfo uInfo = GameClient.instance.UInfo;
            var heroData = uInfo.ListHeroes.Find(e => e.Name == uInfo.Gamer.Hero);
            if (heroData != null)
            {
                foreach (var slot in heroData.ListSlots)
                {
                    if (string.IsNullOrEmpty(slot.TrangBi) == false)
                    {
                        TrangBiData tbData = uInfo.ListTrangBi.Find(e => e.ID == slot.TrangBi);
                        if (tbData != null && tbData.CardSlots != null)
                        {
                            if (tbData.CardSlots.Contains(cardID)) return true;
                        }
                    }
                }
            }
            return false;
        }

        void OnUnSelectFusedItems(CardAvatar item)
        {
            selectedFusingItems.Remove(item);

            UpdateItemSelectState();
        }

        void UpdateItemSelectState()
        {
            for (int i = 0; i < mListItems.Count; ++i)
            {
                var item = mListItems[i];
                if (item == null) continue;

                if (selectedFusingItems.Contains(item))
                {
                    item.IsSelected = true;
                }
                else
                {
                    item.IsSelected = false;
                }
            }

            if (selectedFusingItems.Count > 0)
            {
                btnFuse.interactable = true;
            }
            else
            {
                btnFuse.interactable = false;
            }

            InitListMaterials();
        }

        void InitListMaterials()
        {
            selectedFusingItems.RemoveAll(e => !mListItems.Contains(e));

            int count = 0;
            if (selectedFusingItems.Count > 0)
            {
                Dictionary<string, int> materials = new Dictionary<string, int>();
                materials.Add("M_CardDust", 0);
                for (int i = 0; i < selectedFusingItems.Count; i++)
                {
                    materials["M_CardDust"] += ConfigManager.GetRecycleCardDustByRarity(selectedFusingItems[i].TBRarity);
                }
                foreach (var m in materials)
                {
                    if (count < mListMaterials.Count)
                    {
                        mListMaterials[count].gameObject.SetActive(true);
                        mListMaterials[count].SetItem(m.Key, m.Value);
                    }
                    else
                    {
                        var matAvatar = Instantiate(matPrefab, matParent);
                        matAvatar.SetItem(m.Key, m.Value);
                        //var itemBtn = itemAvatar.GetComponent<Button>();
                        //itemBtn.onClick.AddListener(() => OnItemClick(itemAvatar));
                        matAvatar.gameObject.SetActive(true);
                        //mDicItems.Add(itemData.ID, itemAvatar);
                        mListMaterials.Add(matAvatar);
                    }
                    count++;
                }
            }

            for (int i = mListMaterials.Count - 1; i >= count; --i)
            {
                mListMaterials[i].gameObject.SetActive(false);
            }
            
            matPrefab.gameObject.SetActive(false);
            emptyMaterials.SetActive(count <= 0);
        }

        void OnItemClick(CardAvatar selectedItem)
        {
            if (selectedItem.IsLocked) return;
            if (selectedItem.IsSelected)
            {
                if (selectedFusingItems.Contains(selectedItem))
                {
                    OnUnSelectFusedItems(selectedItem);
                    return;
                }
            }

            SelectFusingSlot(selectedItem);
        }

        void SelectFusingSlot(CardAvatar item)
        {
            if (item.IsLocked) return;

            if (selectedFusingItems.Contains(item) == false)
                selectedFusingItems.Add(item);

            UpdateItemSelectState();
        }

        public void Init()
        {
            activeResourceBar = true;
            if (ResourceBar.instance)
            {
                activeResourceBar = ResourceBar.instance.gameObject.activeSelf;
                ResourceBar.instance.gameObject.SetActive(false);
            }
            InitItems();

            // update trang bi avatar info
            for (int i = mListItems.Count - 1; i >= 0; --i)
            {
                var item = mListItems[i];
                if (item == null) continue;
                var itemData = GameClient.instance.UInfo.listCards.Find(e => e.ID == item.TBId);
                if (itemData != null)
                {
                    item.SetItem(itemData.Name, itemData.Rarity, itemData.ID);
                }
                else
                {
                    mListItems.Remove(item);
                    Destroy(item.gameObject);
                }
            }

            SortItems();

            UpdateItemSelectState();

            emptyItems.SetActive(mListItems.Count == 0);
        }

        //bool IsEquipedTrangBi(string itemID)
        //{
        //    var uInfo = GameClient.instance.UInfo;
        //    var heroData = uInfo.ListHeroes.Find(e => e.Name == uInfo.Gamer.Hero);
        //    if (heroData != null)
        //    {
        //        foreach(var slot in heroData.ListSlots)
        //        {
        //            if (string.IsNullOrEmpty(slot.TrangBi) == false && slot.TrangBi == itemID)
        //            {
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}

        [GUIDelegate]
        public void OnFusingBtnClick()
        {
            selectedFusingItems.RemoveAll(e => e == null);
            if (selectedFusingItems.Count < 1) return;
            
            string[] items = new string[selectedFusingItems.Count];
            for (int i = 0; i < selectedFusingItems.Count; ++i)
            {
                items[i] = selectedFusingItems[i].TBId;
            }
            GameClient.instance.RequestRecycleCard(items);
        }

        protected override void Hide()
        {
            base.Hide();

            if (ResourceBar.instance)
            {
                if (GUIManager.Instance.CurrentScreen == "Main")
                    activeResourceBar = true;

                ResourceBar.instance.gameObject.SetActive(activeResourceBar);
            }
        }
    }
}