using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;
    using UnityEngine.UI;

    public class PopupTinhLuyenTrangBi : PopupBase
    {
        public GameObject[] fusingSlots;

        public Transform itemParent;
        public ItemAvatar itemPrefab;
        public Button btnFuse;

        public GameObject grpMain;
        public GameObject grpSelectSlots;
        string selectedSlot = "";
        public GameObject[] selectedSlotIcons;
        public Text lbEmptyList;

        public static PopupTinhLuyenTrangBi instance;

        //Dictionary<string, ItemAvatar> mDicItems = new Dictionary<string, ItemAvatar>();
        List<ItemAvatar> mListItems = new List<ItemAvatar>();
        List<ItemAvatar> selectedFusingItems = new List<ItemAvatar>();

        public GameObject grpMythicItems;
        public Transform itemMythicParent;
        public ItemAvatar itemMythicPrefab;
        public List<MaterialAvatar> listReturnMaterials;
        public GameObject grpReturnMaterials;
        List<ItemAvatar> mListMythicItems = new List<ItemAvatar>();
        

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
                    return tb2.Level - tb1.Level;
                }
                return tb2.Rarity - tb1.Rarity;
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
                mListItems.Sort(SortBySlot);
            }

            for (int i = 0; i < mListItems.Count; ++i)
            {
                mListItems[i].transform.SetSiblingIndex(i);
            }
        }

        public static PopupTinhLuyenTrangBi Create()
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupTinhLuyenTrangBi");
            instance = go.GetComponent<PopupTinhLuyenTrangBi>();
            instance.Init();
            instance.grpSelectSlots.SetActive(true);
            instance.grpMain.SetActive(false);
            instance.InitMythicItems();
            return instance;
        }

        void ShowReturnMaterials(bool isShow)
        {
            if (isShow)
            {
                List<string> selectedItems = new List<string>();
                for (int i = 0; i < selectedFusingItems.Count; i++)
                {
                    if (selectedFusingItems[i] != null)
                    {
                        selectedItems.Add(selectedFusingItems[i].name);
                    }
                }
                Dictionary<string, int> returnMaterials = ConfigManager.GetMaterialRecycleFromItems(GameClient.instance.UInfo.ListTrangBi, selectedItems, false);

                if (returnMaterials.Count == 0)
                {
                    grpReturnMaterials.SetActive(false);
                }
                else
                {
                    int count = 0;
                    foreach (string matKey in returnMaterials.Keys)
                    {
                        listReturnMaterials[count].SetItem(matKey, returnMaterials[matKey]);
                        listReturnMaterials[count].gameObject.SetActive(true);
                        count++;
                    }
                    for (int i = count; i < listReturnMaterials.Count; i++)
                    {
                        listReturnMaterials[i].gameObject.SetActive(false);
                    }
                    grpReturnMaterials.SetActive(true);
                }
            }
            else
            {
                grpReturnMaterials.SetActive(false);
            }
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
                if (itemData != null && itemData.Rarity == ERarity.Legend)
                {
                    if (mListItems.Exists(e => e.name == itemData.ID) == false)
                    {
                        var itemAvatar = Instantiate(itemPrefab, itemParent);
                        itemAvatar.name = itemData.ID;
                        itemAvatar.SetItem(itemData.Name, itemData.Rarity, itemData.Level);
                        var itemBtn = itemAvatar.GetComponent<Button>();
                        itemBtn.onClick.AddListener(() => OnItemClick(itemAvatar));
                        itemAvatar.gameObject.SetActive(true);

                        itemAvatar.IsEquiped = IsEquipedTrangBi(itemData.ID);
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

        private void InitMythicItems()
        {
            //if (mInitedItems) return;
            if (GameClient.instance == null ||
                GameClient.instance.UInfo == null ||
                ConfigManager.ItemStats == null)
            {
                return;
            }
            

            foreach (string itemName in ConfigManager.ItemStats.Keys)
            {
                var itemRarity = ConfigManager.GetItemDefaultRarity(itemName);
                if (itemRarity == ERarity.Mysthic)
                {
                    if (mListMythicItems.Exists(e => e.name == itemName) == false)
                    {
                        var itemAvatar = Instantiate(itemMythicPrefab, itemMythicParent);
                        itemAvatar.name = itemName;
                        itemAvatar.SetItem(itemName, itemRarity, 1);
                        var itemBtn = itemAvatar.GetComponent<Button>();
                        itemBtn.onClick.AddListener(() => OnMythicItemClick(itemAvatar));
                        itemAvatar.gameObject.SetActive(true);
                        //mDicItems.Add(itemData.ID, itemAvatar);
                        mListMythicItems.Add(itemAvatar);
                    }

                    //var newCell = Instantiate(cellPrefab, itemParrent);
                    //newCell.gameObject.SetActive(true);
                    //newItem.gameObject.SetActive(true);
                    //newCell.AddItem(newItem);
                    //mListCells.Add(newCell);
                }
            }

            //cellPrefab.gameObject.SetActive(false);
            itemMythicPrefab.gameObject.SetActive(false);

            //mInitedItems = true;
        }

        void OnUnSelectFusedItems(ItemAvatar item)
        {
            Debug.Log("OnUnSelectFusedItems");
            for (int i = 0; i < selectedFusingItems.Count; i++)
            {
                if (selectedFusingItems[i] == item 
                    || fusingSlots[i].GetComponentInChildren<ItemAvatar>(true) == item)
                {
                    selectedFusingItems[i] = null;
                }
            }

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
                    item.IsLocked = false;
                }
                else if (!IsEnoughMaterial())
                {
                    item.IsSelected = false;
                    item.IsLocked = false;
                }
                else
                {
                    item.IsSelected = false;
                    item.IsLocked = true;
                }
            }

            for (int i = 0; i < fusingSlots.Length; ++i)
            {
                var slot = fusingSlots[i];
                var preview = slot.GetComponentInChildren<ItemAvatar>(true);
                if (preview == null)
                {
                    preview = Instantiate(itemPrefab, slot.transform);
                    var rect = preview.GetComponent<RectTransform>();
                    rect.anchoredPosition = Vector3.one * 0.5f;
                    preview.transform.localPosition = Vector3.zero;
                    preview.gameObject.SetActive(true);
                    var itemBtn = preview.GetComponent<Button>();
                    itemBtn.onClick.AddListener(() => { OnUnSelectFusedItems(preview); });
                }

                if (selectedFusingItems[i])
                {
                    preview.gameObject.SetActive(true);
                    preview.SetItem(selectedFusingItems[i].TBName, selectedFusingItems[i].TBRarity, selectedFusingItems[i].TBLevel);
                    preview.IsPreview = false;
                }
                else
                {
                    preview.gameObject.SetActive(false);
                }
            }
            ShowReturnMaterials(true);
            if (IsEnoughMaterial() && !string.IsNullOrEmpty(selectedSlot))
            {
                btnFuse.interactable = true;
            }
            else
            {
                btnFuse.interactable = false;
            }
        }

        bool IsEnoughMaterial()
        {
            if(selectedFusingItems[0] != null
                && selectedFusingItems[1] != null
                && selectedFusingItems[2] != null)
            {
                return true;
            }
            return false;
            
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

        void OnMythicItemClick(ItemAvatar selectedItem)
        {
            selectedItem.OnViewInfoClick();
            ////var selectedItem = mListItems.Find(e=> e.name == itemID);
            //if (selectedItem.IsLocked) return;
            //if (selectedItem.IsPreview) return;
            //if (selectedItem.IsSelected)
            //{
            //    if (selectedFusingItems.Contains(selectedItem))
            //    {
            //        OnUnSelectFusedItems(selectedItem);
            //        return;
            //    }
            //}


            //SelectFusingSlot(selectedItem);
        }

        void SelectFusingSlot(ItemAvatar item)
        {
            if (IsEnoughMaterial())
            {
                selectedFusingItems[2] = item;
            }
            else
            {
                for(int i = 0; i < selectedFusingItems.Count; i++)
                {
                    if (!selectedFusingItems[i])
                    {
                        selectedFusingItems[i] = item;
                        break;
                    }
                }
            }

            UpdateItemSelectState();
        }

        bool activeResourceBar = false;
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

            selectedFusingItems.Clear();
            selectedFusingItems.Add(null);
            selectedFusingItems.Add(null);
            selectedFusingItems.Add(null);
            UpdateItemSelectState();

            lbEmptyList.gameObject.SetActive(mListItems.Count == 0);
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
            PopupConfirm.Create(Localization.Get("popup_tinh_luyen_confirm"),()=> 
            {
                if (!IsEnoughMaterial()) return;

                string[] materials = new string[selectedFusingItems.Count];
                for (int i = 0; i < selectedFusingItems.Count; ++i)
                {
                    materials[i] = selectedFusingItems[i].name;
                }
                GameClient.instance.RequestForgeMysthicItem(materials, selectedSlot);
            }, true);            
        }

        [GUIDelegate]
        public void OnMythicItemsBtnClick()
        {
            grpMythicItems.SetActive(true);
        }

        [GUIDelegate]
        public void OnMythicItemsCloseBtnClick()
        {
            grpMythicItems.SetActive(false);
        }

        [GUIDelegate]
        public void OnSelectSlotBtnClick(string _selectSlot)
        {
            //app;y selected slot
            selectedSlot = _selectSlot;
            int idx = 0;
            if(selectedSlot == "VK")
            {
                idx = 0;
            }
            else if (selectedSlot == "MU")
            {
                idx = 1;
            }
            else if (selectedSlot == "AG")
            {
                idx = 2;
            }
            else if (selectedSlot == "OH")
            {
                idx = 3;
            }

            for(int i = 0; i < selectedSlotIcons.Length; i++)
            {
                selectedSlotIcons[i].SetActive(i == idx);
            }

            for(int i = 0; i < mListMythicItems.Count; i++)
            {
                if (mListMythicItems[i].TBName.Contains(selectedSlot))
                {
                    mListMythicItems[i].IsLocked = false;
                }
                else
                {
                    mListMythicItems[i].IsLocked = true;
                }
            }

            grpSelectSlots.SetActive(false);
            grpMain.SetActive(true);
        }

        [GUIDelegate]
        public void OnViewSlotsBtnClick()
        {
            for (int i = 0; i < mListMythicItems.Count; i++)
            {
                mListMythicItems[i].IsLocked = false;
            }

            grpSelectSlots.SetActive(true);
            grpMain.SetActive(false);
        }

        [GUIDelegate]
        public override void OnBackBtnClick()
        {
            if (grpMythicItems.gameObject.activeSelf)
            {
                OnMythicItemsCloseBtnClick();
            }
            else if (grpMain.gameObject.activeSelf)
            {
                OnViewSlotsBtnClick();
            }
            else
            {
                base.OnBackBtnClick();
            }
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

            if(ScreenMain.instance && ScreenMain.instance.grpEquipment)
            {
                if (ScreenMain.instance.grpEquipment.showForgeMenu)
                {
                    PopupTinhLuyenMainMenu.Create();
                }
            }
        }
    }
}