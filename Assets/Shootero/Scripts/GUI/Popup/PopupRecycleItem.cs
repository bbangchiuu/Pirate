using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using UnityEngine.UI;

    public class PopupRecycleItem : PopupBase
    {
        public Transform matParent;
        public MaterialAvatar matPrefab;

        public Transform itemParent;
        public ItemAvatar itemPrefab;
        public Button btnFuse;

        public GameObject emptyItems;
        public GameObject emptyMaterials;

        public static PopupRecycleItem instance;

        List<MaterialAvatar> mListMaterials = new List<MaterialAvatar>();

        List<ItemAvatar> mListItems = new List<ItemAvatar>();
        List<ItemAvatar> selectedFusingItems = new List<ItemAvatar>();
        bool activeResourceBar = false;

        int SortByRarity(ItemAvatar it1, ItemAvatar it2)
        {
            var tb1 = GameClient.instance.UInfo.ListTrangBi.Find(e => e.ID == it1.name);
            var tb2 = GameClient.instance.UInfo.ListTrangBi.Find(e => e.ID == it2.name);
            var cfg1 = ConfigManager.GetItemConfig(tb1);
            var cfg2 = ConfigManager.GetItemConfig(tb2);
            var dif = (int)tb1.Rarity - (int)tb2.Rarity; // asc rarity
            if (dif == 0)
            {
                if (tb1.Level == tb2.Level)
                {
                    var s = cfg1.Slot - cfg2.Slot;
                    if (s == 0)
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
                    else
                    {
                        return s;
                    }
                }
                else
                {
                    return tb1.Level - tb2.Level; // asc level

                }
            }
            return dif;
        }

        int SortBySlot(ItemAvatar it1, ItemAvatar it2)
        {
            var tb1 = GameClient.instance.UInfo.ListTrangBi.Find(e => e.ID == it1.name);
            var tb2 = GameClient.instance.UInfo.ListTrangBi.Find(e => e.ID == it2.name);
            var cfg1 = ConfigManager.GetItemConfig(tb1);
            var cfg2 = ConfigManager.GetItemConfig(tb2);
            if (cfg1.Slot == cfg2.Slot)
            {
                if (tb2.Rarity == tb1.Rarity)
                {
                    return tb1.Level - tb2.Level; // asc level
                }
                return tb1.Rarity - tb2.Rarity; // asc rarity
            }

            return cfg1.Slot - cfg2.Slot;
        }

        private void SortItems()
        {
            //lbSorting.text = Localization.Get(string.Format("Sort{0}", Sorting.ToString()));

            //if (Sorting == SortingItems.ByRarity)
            //{
            //    mListItems.Sort(SortByRarity);
            //}
            //else
            {
                mListItems.Sort(SortByRarity);
            }

            for (int i = 0; i < mListItems.Count; ++i)
            {
                mListItems[i].transform.SetSiblingIndex(i);
            }
        }

        public static PopupRecycleItem Create()
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupRecycleItem");
            instance = go.GetComponent<PopupRecycleItem>();
            instance.Init();
            return instance;
        }

        private void InitItems()
        {
            //if (mInitedItems) return;
            if (GameClient.instance == null ||
                GameClient.instance.UInfo == null ||
                GameClient.instance.UInfo.ListTrangBi == null)
            {
                return;
            }

            foreach (var itemData in GameClient.instance.UInfo.ListTrangBi)
            {
                if (itemData != null &&
                    itemData.Rarity < ERarity.Legend &&
                    IsEquipedTrangBi(itemData.ID) == false)
                {
                    if (mListItems.Exists(e => e.name == itemData.ID) == false)
                    {
                        var itemAvatar = Instantiate(itemPrefab, itemParent);
                        itemAvatar.name = itemData.ID;
                        itemAvatar.SetItem(itemData.Name, itemData.Rarity, itemData.Level);
                        var itemBtn = itemAvatar.GetComponent<Button>();
                        itemBtn.onClick.AddListener(delegate { OnItemClick(itemAvatar); });
                        itemAvatar.gameObject.SetActive(true);
                        //mDicItems.Add(itemData.ID, itemAvatar);
                        mListItems.Add(itemAvatar);
                    }

                    //var newCell = Instantiate(cellPrefab, itemParrent);
                    //newCell.gameObject.SetActive(true);
                    //newItem.gameObject.SetActive(true);
                    //newCell.AddItem(newItem);
                    //mListCells.Add(newCell);
                }
            }

            //cellPrefab.gameObject.SetActive(false);
            itemPrefab.gameObject.SetActive(false);

            //mInitedItems = true;
        }

        void OnUnSelectFusedItems(ItemAvatar item)
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
            selectedFusingItems.RemoveAll(e => e == null);
            string[] selectedItems = new string[selectedFusingItems.Count];
            for (int i = 0; i < selectedFusingItems.Count; ++i)
            {
                selectedItems[i] = selectedFusingItems[i].name;
            }

            int count = 0;
            if (selectedItems.Length > 0)
            {
                var materials = ConfigManager.GetMaterialRecycleFromItems(GameClient.instance.UInfo.ListTrangBi, selectedItems);
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

            //cellPrefab.gameObject.SetActive(false);
            matPrefab.gameObject.SetActive(false);
            emptyMaterials.SetActive(count <= 0);
        }

        void OnItemClick(ItemAvatar selectedItem)
        {
            //var selectedItem = mListItems.Find(e=> e.name == itemID);
            if (selectedItem.IsLocked) return;
            if (selectedItem.IsPreview) return;
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

        void SelectFusingSlot(ItemAvatar item)
        {
            if (item.IsLocked || item.IsPreview) return;

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
                var itemData = GameClient.instance.UInfo.ListTrangBi.Find(e => e.ID == item.name);
                if (itemData != null)
                {
                    item.SetItem(itemData.Name, itemData.Rarity, itemData.Level);
                }
                else
                {
                    mListItems.Remove(item);
                    Destroy(item.gameObject);
                    //mDicItems.Remove(item.name);
                }
            }
            // update equipment

            SortItems();

            UpdateItemSelectState();

            emptyItems.SetActive(mListItems.Count == 0);
        }

        bool IsEquipedTrangBi(string itemID)
        {
            var uInfo = GameClient.instance.UInfo;
            var heroData = uInfo.ListHeroes.Find(e => e.Name == uInfo.Gamer.Hero);
            if (heroData != null)
            {
                foreach(var slot in heroData.ListSlots)
                {
                    if (string.IsNullOrEmpty(slot.TrangBi) == false && slot.TrangBi == itemID)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        [GUIDelegate]
        public void OnFusingBtnClick()
        {
            selectedFusingItems.RemoveAll(e => e == null);
            if (selectedFusingItems.Count < 1) return;
            
            string[] items = new string[selectedFusingItems.Count];
            for (int i = 0; i < selectedFusingItems.Count; ++i)
            {
                items[i] = selectedFusingItems[i].name;
            }
            GameClient.instance.RequestRecycleItems(items);
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