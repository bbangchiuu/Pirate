using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;
    using UnityEngine.UI;
    public class GroupEquipment : NetworkDataSync
    {
        public ScrollRect scrollView;
        public Text heroName;
        public Text lbHP;
        public Text lbDMG;

        //public DragAndDropCell[] slots;
        public GameObject[] slots;

        //public DragAndDropCell cellPrefab;
        public Transform itemParrent;
        public ItemAvatar itemPrefab;

        public Transform materialParent;
        public MaterialAvatar materialPrefab;

        public Text lbSorting;
        public Transform visual3DContainer;

        public GameObject btnForge;
        public TweenScale btnForgeFX;

        public GameObject changedHeroGrp;
        public GameObject unlockedHeroTooltip;
        public TweenScale btnChangeHeroFX;
        public Button btnChangehero;
        public Image heroAvatar;
        public GameObject heroUpgradeNotification;

        public GameObject btnOpenCardInven;
        public Button btnXemThongTinAdv;

        PlayerVisual playerVisual;

        //bool mInitedItems = false;
        Dictionary<string, ItemAvatar> mDicItems = new Dictionary<string, ItemAvatar>();
        List<ItemAvatar> mListItems = new List<ItemAvatar>();

        List<MaterialAvatar> mListMaterials = new List<MaterialAvatar>();

        UnitStat mBasicStat;
        int mVkAtkSpd;
        List<StatMod> mListMods;

        public enum SortingItems
        {
            ByRarity,
            BySlot
        }
        public SortingItems Sorting = SortingItems.ByRarity;

        public EquipmentSlot[] GetEquipmentSlots()
        {
            EquipmentSlot[] result = new EquipmentSlot[slots.Length];
            for (int i = 0; i < result.Length; ++i)
            {
                result[i] = new EquipmentSlot();
                result[i].Slot = (SlotType)System.Enum.Parse(typeof(SlotType), slots[i].name);
                var item = slots[i].GetComponentInChildren<ItemAvatar>();
                if (item != null)
                {
                    result[i].TrangBi = item.name;
                }
                else
                {
                    result[i].TrangBi = string.Empty;
                }
            }

            return result;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            SyncNetworkData();
        }

        [GUIDelegate]
        public void OnBtnChangeHero()
        {
            //Debug.Log("OnBtnChangeHero");
            //PopupMessage.Create(MessagePopupType.TEXT, "[OnBtnChangeHero] Chua lam` popup Hero");
            PopupSelectHero.Create();
        }

        void ThaoTrangBi(GameObject slot)
        {
            EquipedCardsSimpleVisual equipedCards = slot.GetComponentInChildren<EquipedCardsSimpleVisual>(true);
            if (equipedCards != null) equipedCards.gameObject.SetActive(false);

            var oldAva = slot.GetComponentInChildren<ItemAvatar>(true);
            if (oldAva != null)
            {
                //oldAva.transform.SetParent(itemParrent, false);
                oldAva.gameObject.SetActive(false);
                //mListItems.Add(oldAva);
                var curItem = mListItems.Find(e => e.name == oldAva.name);
                if (curItem != null)
                {
                    curItem.IsEquiped = false;
                    curItem.SetNotifyUpgrade(false);
                }
            }
        }

        void LapTrangBi(GameObject slot, string itemID)
        {
            if (mDicItems.ContainsKey(itemID))
            {
                ThaoTrangBi(slot);
                var index = mListItems.FindIndex(e => e.name == itemID);
                var curItem = slot.GetComponentInChildren<ItemAvatar>(true);
                if (curItem == null)
                    curItem = Instantiate(mListItems[index]);
                curItem.gameObject.SetActive(true);
                curItem.name = itemID;
                curItem.transform.SetParent(slot.transform, false);
                var rectTrans = curItem.GetComponent<RectTransform>();
                rectTrans.anchoredPosition = Vector3.one * 0.5f;
                curItem.transform.localPosition = Vector3.zero;
                var originTrans = mListItems[index].GetComponent<RectTransform>();
                rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, originTrans.rect.size.x);
                rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, originTrans.rect.size.y);
                //mListItems.RemoveAt(index);
                curItem.IsEquiped = false;
                curItem.SetItem(mListItems[index].TBName, mListItems[index].TBRarity, mListItems[index].TBLevel, mListItems[index].TBStar);

                var tbData = GameClient.instance.UInfo.ListTrangBi.Find(e => e.ID == itemID);
                string msg = GameClient.instance.UInfo.CheckUpgradeItem(tbData, Localization.language,
                    listCheckMat, out long changedGold, out int changedGem);

                curItem.SetNotifyUpgrade(string.IsNullOrEmpty(msg));
                mListItems[index].SetNotifyUpgrade(string.IsNullOrEmpty(msg));

                var button = curItem.GetComponent<Button>();
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(delegate { OnItemClick(itemID); });
                mListItems[index].IsEquiped = true;

                //feo: simple card slots
                EquipedCardsSimpleVisual equipedCards = slot.GetComponentInChildren<EquipedCardsSimpleVisual>(true);
                if (equipedCards)
                    equipedCards.transform.SetAsLastSibling();

                if (equipedCards != null && GameClient.instance.UInfo.listCards != null)
                {
                    List<int> slotStatus = new List<int>();
                    int maxSlot = ConfigManager.GetMaxCardSlotByRarity(tbData.Rarity);
                    for (int i = 0; i < maxSlot; i++)
                    {
                        if(i >= tbData.CardSlots.Count)
                        {
                            slotStatus.Add(-1);
                        }
                        else if (string.IsNullOrEmpty(tbData.CardSlots[i]))
                        {
                            slotStatus.Add(0);
                        }
                        else
                        {
                            CardData cardData = GameClient.instance.UInfo.listCards.Find(e => e.ID == tbData.CardSlots[i]);
                            slotStatus.Add((int)cardData.Rarity);
                        }

                    }
                    equipedCards.ShowItemSlots(slotStatus);
                    equipedCards.gameObject.SetActive(true);
                }
            }
        }

        bool IsSlotHaveItem(GameObject slot)
        {
            var curItem = slot.GetComponentInChildren<ItemAvatar>();
            return curItem != null;
        }

        public void LapTrangBi(TrangBiData tb)
        {
            var cfg = ConfigManager.GetItemConfig(tb);
            string slotStr = cfg.Slot.ToString();

            foreach (var s in slots)
            {
                if (s.name == slotStr)
                {
                    if (IsSlotHaveItem(s) == false)
                    {
                        LapTrangBi(s, tb.ID);
                        return;
                    }
                }
            }

            // chua lap
            foreach (var s in slots)
            {
                if (s.name == slotStr)
                {
                    //if (IsSlotHaveItem(s) == false)
                    {
                        LapTrangBi(s, tb.ID);
                        return;
                    }
                }
            }
        }

        public void ThaoTrangBi(TrangBiData tb)
        {
            if (mDicItems.ContainsKey(tb.ID))
            {
                var item = mDicItems[tb.ID];
                foreach (var s in slots)
                {
                    var itemInSlot = s.GetComponentInChildren<ItemAvatar>(true);
                    if (itemInSlot != null && itemInSlot.gameObject.activeInHierarchy && itemInSlot.name == tb.ID)
                    {
                        ThaoTrangBi(s);
                        return;
                    }
                }
            }
        }

        public void CheckHeroUnlock()
        {
            int curChap = GameClient.instance.UInfo.GetCurrentChapter();
            if (curChap < ConfigManager.GetChapterUnlockHero())
            {
                changedHeroGrp.gameObject.SetActive(false);
            }
            else
            {
                changedHeroGrp.gameObject.SetActive(true);
                var unlockedPref = PlayerPrefs.GetInt("ShowUnlockHeroTooltip_" + GameClient.instance.UInfo.GID, 0);
                if (unlockedPref <= 0)
                {
                    unlockedHeroTooltip.gameObject.SetActive(true);
                }
                else
                {
                    unlockedHeroTooltip.gameObject.SetActive(false);
                }
                var changeHeroPref = PlayerPrefs.GetInt("ShowChangeHeroEff_" + GameClient.instance.UInfo.GID, 0);
                if (changeHeroPref == 0)
                {
                    btnChangeHeroFX.enabled = true;
                    btnChangeHeroFX.PlayForward();
                }
                else
                {
                    btnChangeHeroFX.ResetToBeginning();
                    btnChangeHeroFX.enabled = false;
                }

                heroUpgradeNotification.SetActive(false);
                long crGold = GameClient.instance.UInfo.Gamer.Gold;
                int crHeroStone = 0;
                MaterialData mat = GameClient.instance.UInfo.ListMaterials.Find(e => e.Name == "M_HeroStone");
                if (mat != null) crHeroStone = mat.Num;
                for (int i = 0; i < GameClient.instance.UInfo.ListHeroes.Count; i++)
                {
                    if (GameClient.instance.UInfo.ListHeroes[i].Name != GameClient.instance.UInfo.Gamer.Hero) continue;
                    if (GameClient.instance.UInfo.ListHeroes[i].Level >= ConfigManager.HeroUpgradeCfg.GoldCost.Length) continue;

                    int reqGold = ConfigManager.HeroUpgradeCfg.GoldCost[GameClient.instance.UInfo.ListHeroes[i].Level];
                    int reqHeroStone = ConfigManager.HeroUpgradeCfg.HeroStoneCost[GameClient.instance.UInfo.ListHeroes[i].Level];
                    if (reqGold <= crGold && reqHeroStone <= crHeroStone)
                    {
                        heroUpgradeNotification.SetActive(true);
                        break;
                    }
                }
            }
        }

        public bool showForgeMenu = false;
        public override void SyncNetworkData()
        {
            if (GameClient.instance == null ||
                GameClient.instance.UInfo == null ||
                GameClient.instance.UInfo.Gamer == null)
            {
                return;
            }

            List<TrangBiData> listLegendItems = GameClient.instance.UInfo.ListTrangBi.FindAll(e => e.Rarity == ERarity.Legend);
            List<TrangBiData> listMythicItems = GameClient.instance.UInfo.ListTrangBi.FindAll(e => e.Rarity == ERarity.Mysthic);

            int curChap = GameClient.instance.UInfo.GetCurrentChapter();
            btnXemThongTinAdv.gameObject.SetActive(curChap >= ConfigManager.GetChapUnlockAdvanceStat());

            btnOpenCardInven.SetActive(GroupChapter.IsLockSanThe(GameClient.instance.UInfo) == false);

            btnForge.SetActive(false);
            showForgeMenu = false;

            if (listLegendItems.Count >= 4
                || listMythicItems.Count > 0)
            {
                btnForge.SetActive(true);
                if (PlayerPrefs.GetInt("ShowBtnForgeEffect_" + GameClient.instance.UInfo.GID, 0) == 0)
                {
                    btnForgeFX.enabled = true;
                    btnForgeFX.PlayForward();
                }
                else
                {
                    btnForgeFX.enabled = false;
                }
            }

            if (listMythicItems.Count > 1
                || (listMythicItems.Count == 1 && listLegendItems.Count >= 3)
                || PlayerPrefs.GetInt("ShowBtnForgeEffect_2_" + GameClient.instance.UInfo.GID, 0) == 1)
            {
                showForgeMenu = true;
                if (PlayerPrefs.GetInt("ShowBtnForgeEffect_2_" + GameClient.instance.UInfo.GID, 0) == 0)
                {
                    btnForgeFX.enabled = true;
                    btnForgeFX.PlayForward();
                }
                else
                {
                    btnForgeFX.enabled = false;
                }
            }

            CheckHeroUnlock();
        
            InitItems();

            //if (mInitedItems == false) return;

            // update trang bi avatar info
            foreach (var item in mDicItems)
            {
                var ava = item.Value.GetComponent<ItemAvatar>();
                var itemData = GameClient.instance.UInfo.ListTrangBi.Find(e => e.ID == item.Key);
                if (itemData != null)
                {
                    ava.SetItem(itemData.Name, itemData.Rarity, itemData.Level, itemData.Star);

                    //string msg = GameClient.instance.UInfo.CheckUpgradeItem(itemData, Localization.language,
                    //    listCheckMat, out long changedGold, out int changedGem);

                    //ava.SetNotifyUpgrade(string.IsNullOrEmpty(msg));
                }
            }

            // update equipment
            string hero = GameClient.instance.UInfo.Gamer.Hero;
            heroName.text = hero;

            var heroData = GameClient.instance.UInfo.ListHeroes.Find(e => e.Name == hero);
            string vuKhiName = string.Empty;

            for (int i = 0; i < heroData.ListSlots.Length; ++i)
            {
                var slotData = heroData.ListSlots[i];

                if (i < slots.Length)
                {
                    TrangBiData item = null;
                    if (slotData != null && string.IsNullOrEmpty(slotData.TrangBi) == false)
                    {
                        item = GameClient.instance.UInfo.ListTrangBi.Find(e => e.ID == slotData.TrangBi);
                    }

                    if (item != null) // co trang bi
                    {
                        if (slotData.Slot == SlotType.VuKhi)
                        {
                            vuKhiName = item.Name;
                        }

                        LapTrangBi(slots[i], item.ID);
                        //string msg = GameClient.instance.UInfo.CheckUpgradeItem(itemData, Localization.language,
                        //    listCheckMat, out long changedGold, out int changedGem);

                        //ava.SetNotifyUpgrade(string.IsNullOrEmpty(msg));
                    }
                    else
                    {
                        ThaoTrangBi(slots[i]);
                    }
                }
            }

            Load3DVisual(hero, vuKhiName);

            SortItems();

            UpdateBasicInfo();
        }

        void Load3DVisual(string heroName, string weaponName)
        {
            if (string.IsNullOrEmpty(heroName))
                heroName = ConfigManager.DefaultHeroCodeName;

            string vuKhiName = weaponName;

            string vuKhiType = "Melee";
            if (string.IsNullOrEmpty(vuKhiName) == false)
            {
                var cfg = ConfigManager.GetItemConfig(vuKhiName);
                if (cfg != null)
                {
                    if (cfg.VuKhiType == "Gun")
                    {
                        vuKhiType = cfg.VuKhiType;
                    }
                }
            }
            string heroPrefabName = heroName + vuKhiType;

            bool reloadVisual = false;
            if (playerVisual != null)
            {
                if (playerVisual.name != heroPrefabName)
                {
                    playerVisual.gameObject.SetActive(false);
                    Destroy(playerVisual.gameObject);
                    playerVisual = null;
                }
            }

            if (playerVisual == null)
            {
                playerVisual = Instantiate(Resources.Load<PlayerVisual>("Heroes/" + heroPrefabName), visual3DContainer);
                playerVisual.name = heroPrefabName;
                playerVisual.transform.localPosition = ConfigManager.GetHeroViewOffset(heroPrefabName);
                playerVisual.transform.localEulerAngles = ConfigManager.GetHeroViewRotation(heroPrefabName);
                playerVisual.transform.localScale = ConfigManager.GetHeroViewScale(heroPrefabName);
                reloadVisual = true;

                var layerUI = LayerMask.NameToLayer("UI");
                playerVisual.transform.SetLayer(layerUI, true);

                if (heroName == "BeastMaster")
                {
                    var chim = Instantiate(Resources.Load<GameObject>("Heroes/ChimBM"), playerVisual.transform);
                    chim.transform.localPosition = new Vector3(1.19f, 0.36f, 0.91f);
                    chim.transform.localEulerAngles = new Vector3(-18.7f, -33.7f, 9.4f);
                    chim.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
                    chim.transform.SetLayer(layerUI, true);
                }
            }

            if (reloadVisual ||
                playerVisual.WeaponName != vuKhiName)
            {
                playerVisual.LoadWeapon(vuKhiName, false);
            }
        }

        public void UpdateBasicInfo()
        {
            var heroData = GameClient.instance.UInfo.ListHeroes.Find(e => e.Name == heroName.text);

            mBasicStat = UnitStatUtils.BuildStatFromHeroEquipment(heroData,
                GameClient.instance.UInfo.ListTrangBi,
                GameClient.instance.UInfo.ListArmories,
                GameClient.instance.UInfo.ListHeroes,
                GameClient.instance.UInfo.listCards,
                out mVkAtkSpd,
                out mListMods);

            lbHP.text = string.Format("{0}: {1}", Localization.Get("Stat_HP"), mBasicStat.HP);
            lbDMG.text = string.Format("{0}: {1}", Localization.Get("Stat_ATK"), mBasicStat.DMG);

            var spriteCol = Resources.Load<SpriteCollection>("HeroAvatar");
            heroAvatar.sprite = spriteCol.GetSprite(heroData.Name);
        }

        static Dictionary<string, int> listCheckMat = new Dictionary<string, int>();

        private void InitItems()
        {
            //if (mInitedItems) return;
            if (GameClient.instance == null ||
                GameClient.instance.UInfo == null ||
                GameClient.instance.UInfo.ListTrangBi == null)
            {
                return;
            }

            for (int i = mListItems.Count - 1; i >= 0; --i)
            {
                var item = mListItems[i];
                if (item == null) continue;

                var itemData = GameClient.instance.UInfo.ListTrangBi.Find(e => e.ID == item.name);
                if (itemData != null)
                {
                    item.SetItem(itemData.Name, itemData.Rarity, itemData.Level, itemData.Star);
                    item.IsEquiped = false;
                }
                else
                {
                    mListItems.RemoveAt(i);
                    mDicItems.Remove(item.name);
                    Destroy(item.gameObject);
                    //mDicItems.Remove(item.name);
                }
            }

            foreach (var itemData in GameClient.instance.UInfo.ListTrangBi)
            {
                if (itemData != null)
                {
                    if (mDicItems.ContainsKey(itemData.ID) == false)
                    {
                        var itemAvatar = Instantiate(itemPrefab, itemParrent);
                        itemAvatar.name = itemData.ID;
                        itemAvatar.SetItem(itemData.Name, itemData.Rarity, itemData.Level, itemData.Star);
                        var itemBtn = itemAvatar.GetComponent<Button>();
                        itemBtn.onClick.AddListener(() => OnItemClick(itemData.ID));
                        itemAvatar.gameObject.SetActive(true);
                        mDicItems.Add(itemData.ID, itemAvatar);
                        mListItems.Add(itemAvatar);
                        itemAvatar.IsEquiped = false;
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

            if (GameClient.instance.UInfo.ListMaterials != null)
            {
                foreach (var matData in GameClient.instance.UInfo.ListMaterials)
                {
                    if (matData != null)
                    {
                        var curAva = mListMaterials.Find(e => e.name == matData.ID);
                        if (curAva == null)
                        {
                            var matAva = Instantiate(materialPrefab, materialParent);
                            matAva.name = matData.ID;
                            matAva.SetItem(matData.Name, matData.Num);
                            var itemBtn = matAva.GetComponent<Button>();
                            itemBtn.onClick.AddListener(() => OnMaterialClick(matData.ID));
                            matAva.gameObject.SetActive(true);

                            mListMaterials.Add(matAva);
                        }
                        else
                        {
                            curAva.SetItem(matData.Name, matData.Num);
                        }
                        //var newCell = Instantiate(cellPrefab, itemParrent);
                        //newCell.gameObject.SetActive(true);
                        //newItem.gameObject.SetActive(true);
                        //newCell.AddItem(newItem);
                        //mListCells.Add(newCell);
                    }
                }
            }
            //HikerUtils.DoAction(this, () =>
            //{
            //    LayoutRebuilder.MarkLayoutForRebuild(itemParrent as RectTransform);
            //}, 0.1f, true);
            //HikerUtils.DoAction(this, () =>
            //{
            //    LayoutRebuilder.MarkLayoutForRebuild(itemParrent.parent as RectTransform);
            //}, 0.2f, true);

            HikerUtils.DoAction(this, () =>
            {
                //var grp = itemParrent.parent.parent.GetComponent<VerticalLayoutGroup>();
                //grp.enabled = false;
                //grp.enabled = true;
                //LayoutRebuilder.MarkLayoutForRebuild(itemParrent as RectTransform);
                //LayoutRebuilder.MarkLayoutForRebuild(itemParrent.parent as RectTransform);
                LayoutRebuilder.ForceRebuildLayoutImmediate(itemParrent.parent.parent as RectTransform);
            }, 0.15f, true);
        }

        void OnMaterialClick(string matID)
        {
            Debug.Log("Mat Click");
        }

        void OnItemClick(string itemID)
        {
            var tb = GameClient.instance.UInfo.ListTrangBi.Find(e => e.ID == itemID);
            if (tb != null)
            {
                PopupTrangBiInfo.Create(tb, mListItems.Find(e => e.name == itemID).IsEquiped);
            }
        }

        int SortByRarity(ItemAvatar it1, ItemAvatar it2)
        {
            var tb1 = GameClient.instance.UInfo.ListTrangBi.Find(e => e.ID == it1.name);
            var tb2 = GameClient.instance.UInfo.ListTrangBi.Find(e => e.ID == it2.name);
            var cfg1 = ConfigManager.GetItemConfig(tb1);
            var cfg2 = ConfigManager.GetItemConfig(tb2);
            var dif = (int)tb2.Rarity - (int)tb1.Rarity;
            if (dif == 0)
            {
                if (cfg1.Slot == cfg2.Slot)
                {
                    if (tb1.Level == tb2.Level)
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

                    return tb2.Level - tb1.Level;
                }

                return cfg1.Slot - cfg2.Slot;
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
                    if (tb1.Level == tb2.Level)
                    {
                        return tb1.ID.CompareTo(tb2.ID);
                    }

                    return tb2.Level - tb1.Level;
                }
                return tb2.Rarity - tb1.Rarity;
            }
            return cfg1.Slot - cfg2.Slot;
        }

        private void SortItems()
        {
            lbSorting.text = Localization.Get(string.Format("Sort{0}", Sorting.ToString()));

            if (Sorting == SortingItems.ByRarity)
            {
                mListItems.Sort(SortByRarity);
            }
            else
            {
                mListItems.Sort(SortBySlot);
            }

            for (int i = 0; i < mListItems.Count; ++i)
            {
                mListItems[i].transform.SetSiblingIndex(i);
            }
            //var grid = itemParrent.GetComponent<GridLayoutGroup>();
            //grid.or
            //for (int i = 0; i < mListCells.Count - 1; ++i)
            //{
            //    var firstItem = mListCells[i].GetItem();
            //    if (firstItem == null)
            //    {
            //        for (int j = i + 1; j < mListCells.Count; ++j)
            //        {
            //            var nextItem = mListCells[j].GetItem();
            //            if (nextItem != null)
            //            {
            //                mListCells[i].SwapItems(mListCells[i], mListCells[j]);
            //            }
            //        }
            //    }
            //}
        }

        public void RequestSetEquip()
        {
            var hero = GameClient.instance.UInfo.ListHeroes.Find(e => e.Name == heroName.text);
            if (hero != null)
            {
                var slots = GetEquipmentSlots();
                hero.ListSlots = slots;
                GameClient.instance.RequestSetHeroEquipment(hero.ID, slots);
            }
        }
        [GUIDelegate]
        public void BtnSortingClick()
        {
            Sorting = 1 - Sorting;
            SortItems();
        }
        [GUIDelegate]
        public void OnBtnFuseClick()
        {
            if (btnForgeFX != null)
            {
                btnForgeFX.ResetToBeginning();
                btnForgeFX.enabled = false;
                btnForgeFX.transform.localScale = Vector3.one;

                PlayerPrefs.SetInt("ShowBtnForgeEffect_" + GameClient.instance.UInfo.GID, 1);
            }

            if (showForgeMenu)
            {
                PopupTinhLuyenMainMenu.Create();
                PlayerPrefs.SetInt("ShowBtnForgeEffect_2_" + GameClient.instance.UInfo.GID, 1);
            }
            else
            {
                PopupTinhLuyenTrangBi.Create();
            }
            

            
        }
        [GUIDelegate]
        public void BtnRecycleClick()
        {
            PopupRecycleItem.Create();
        }
        [GUIDelegate]
        public void BtnUnitCardClick()
        {
            PopupTheQuaiVat.Create();
        }

        [GUIDelegate]
        public void BtnXemThongTinHeroClick()
        {
            PopupThongTinNangCao.Create(GameClient.instance.UInfo.Gamer.Hero, mBasicStat, mVkAtkSpd, mListMods);
        }

        [GUIDelegate]
        public void BtnHeroUpgradeDbTapClick()
        {
            if (checkHeroUpgradeDoubleTap)
            {
                HeroData hero = GameClient.instance.UInfo.ListHeroes.Find(e => e.Name == GameClient.instance.UInfo.Gamer.Hero);
                if (hero != null) PopupHeroUpgrade.Create(hero);
            }
            else
            {
                StartCoroutine(HeroUpgradeDoubleTapTime(0.2f));
            }
        }

        bool checkHeroUpgradeDoubleTap = false;
        IEnumerator HeroUpgradeDoubleTapTime(float time)
        {
            checkHeroUpgradeDoubleTap = true;
            yield return new WaitForSeconds(time);
            checkHeroUpgradeDoubleTap = false;
        }

        ///// <summary>
        ///// Operate all drag and drop requests and events from children cells
        ///// </summary>
        ///// <param name="desc"> request or event descriptor </param>
        //void OnSimpleDragAndDropEvent(DragAndDropCell.DropEventDescriptor desc)
        //{
        //    //// Get control unit of source cell
        //    //DummyControlUnit sourceSheet = desc.sourceCell.GetComponentInParent<DummyControlUnit>();
        //    //// Get control unit of destination cell
        //    //DummyControlUnit destinationSheet = desc.destinationCell.GetComponentInParent<DummyControlUnit>();
        //    var source = desc.sourceCell;
        //    var target = desc.destinationCell;
        //    var itemData = GameClient.instance.UInfo.ListTrangBi.Find(e => e.ID == desc.item.name);
        //    var itemCfg = ConfigManager.GetItemConfig(itemData);

        //    switch (desc.triggerType)                                               // What type event is?
        //    {
        //        case DragAndDropCell.TriggerType.DropRequest:                       // Request for item drag (note: do not destroy item on request)
        //            //Debug.Log("Request " + desc.item.name + " from " + sourceSheet.name + " to " + destinationSheet.name);
        //            {
        //                // if target is equipment slot
        //                if (System.Enum.TryParse(target.name, out SlotType slotType))
        //                {

        //                    if (slotType == itemCfg.Slot)
        //                    {
        //                        desc.permission = true;
        //                    }
        //                    else
        //                    {
        //                        desc.permission = false;
        //                    }
        //                }
        //                else // if target is inventory
        //                {
        //                    // if source is from inventory
        //                    if (System.Enum.TryParse(source.name, out SlotType sourceSlot))
        //                    {
        //                        int firstEmptyCell = -1;
        //                        for (int i = 0; i < mListCells.Count; ++i) // destination will be first 
        //                        {
        //                            if (mListCells[i].GetItem() == null)
        //                            {
        //                                firstEmptyCell = i;
        //                                break;
        //                            }
        //                        }

        //                        if (firstEmptyCell >= 0)
        //                        {
        //                            if (mListCells[firstEmptyCell] != desc.destinationCell)
        //                            {
        //                                mListCells[firstEmptyCell].SwapItems(desc.destinationCell, mListCells[firstEmptyCell]);
        //                            }

        //                            desc.permission = true;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        desc.permission = true;
        //                    }
        //                }
        //            }
        //            break;
        //        case DragAndDropCell.TriggerType.DropEventEnd:                      // Drop event completed (successful or not)
        //            {
        //                if (System.Enum.TryParse(target.name, out SlotType slotType))
        //                {
        //                }
        //                else if (System.Enum.TryParse(target.name, out SlotType sourceSlot))
        //                {
        //                    // source is equipment slot
        //                    if (desc.permission == false)
        //                    {
        //                        for (int i = 0; i < mListCells.Count; ++i)
        //                        {
        //                            if (mListCells[i].GetItem() == null)
        //                            {
        //                                desc.destinationCell = mListCells[i];
        //                                break;
        //                            }
        //                        }
        //                    }
        //                }

        //                if (desc.permission == true)                                    // If drop successful (was permitted before)
        //                {
        //                    //Debug.Log("Successful drop " + desc.item.name + " from " + sourceSheet.name + " to " + destinationSheet.name);
        //                    SortItems();
        //                }
        //                else                                                            // If drop unsuccessful (was denied before)
        //                {
        //                    //Debug.Log("Denied drop " + desc.item.name + " from " + sourceSheet.name + " to " + destinationSheet.name);
        //                }

        //                RequestSetEquip();
        //            }
        //            break;
        //        //case DragAndDropCell.TriggerType.ItemAdded:                         // New item is added from application
        //        //    Debug.Log("Item " + desc.item.name + " added into " + destinationSheet.name);
        //        //    break;
        //        //case DragAndDropCell.TriggerType.ItemWillBeDestroyed:               // Called before item be destructed (can not be canceled)
        //        //    Debug.Log("Item " + desc.item.name + " will be destroyed from " + sourceSheet.name);
        //        //    break;
        //        default:
        //            Debug.Log("Unknown drag and drop event");
        //            break;
        //    }
        //}
    }
}