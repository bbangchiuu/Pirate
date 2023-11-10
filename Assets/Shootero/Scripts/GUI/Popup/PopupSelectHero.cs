using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;
    using UnityEngine.UI;
    using UnityEngine.Video;

    public class PopupSelectHero : PopupBase
    {
        public Transform itemParent;
        public HeroAvatar itemPrefab;

        public Transform visual3DContainer;

        public Text HeroNameTitle;
        public Text HeroNameTitle2;
        public Text HeroNameDesc;
        public Text HeroSkillDesc;
        
        public Button btnSelect;
        public GameObject selectedObj;
        public Image skillImg;

        public Button btnPurchase;
        public Text lbPrice;
        public GameObject gemGrp;
        public Text lbGem;
        public GameObject freeGrp;

        string selectedHero = "";
        string currentVk = "";

        PlayerVisual playerVisual;

        public Button btnHeroUpgrade;
        public Button btnVideo;

        public static PopupSelectHero instance;

        List<HeroAvatar> mListItems = new List<HeroAvatar>();

        int SortBySlot(HeroAvatar it1, HeroAvatar it2)
        {
            var tb1 = GameClient.instance.UInfo.ListHeroes.Find(e => e.ID == it1.name);
            var tb2 = GameClient.instance.UInfo.ListHeroes.Find(e => e.ID == it2.name);
            var cfg1 = ConfigManager.GetUnitStat(it1.TBName);
            var cfg2 = ConfigManager.GetItemConfig(it2.TBName);
            return it1.name.CompareTo(it2.TBName);
            //if (cfg1.Slot == cfg2.Slot)
            //{
            //    if (tb2.Rarity == tb1.Rarity)
            //    {
            //        return tb2.Level - tb1.Level;
            //    }
            //    return tb2.Rarity - tb1.Rarity;
            //}
            //return cfg1.Slot - cfg2.Slot;
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

        public static PopupSelectHero Create()
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupSelectHero");
            instance = go.GetComponent<PopupSelectHero>();
            instance.Init();
            return instance;
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
                playerVisual = Instantiate(Resources.Load<PlayerVisual>("Heroes/" + heroPrefabName),
                    visual3DContainer);
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

                if (heroName.StartsWith("IronMan"))
                {
                    ParticleSystemRenderer[] effs = playerVisual.GetComponentsInChildren<ParticleSystemRenderer>();

                    for (int e = 0; e < effs.Length; e++)
                    {
                        ParticleSystemRenderer eff = effs[e];
                        if (eff)
                        {
                            eff.sortingOrder = eff.sortingOrder + 30;
                        }
                    }
                }
            }

            if (reloadVisual ||
                playerVisual.WeaponName != vuKhiName)
            {
                GameObject vk = playerVisual.LoadWeapon(vuKhiName, false);

                if (vk != null)
                {
                    ParticleSystemRenderer[] effs = vk.GetComponentsInChildren<ParticleSystemRenderer>();

                    for (int e = 0; e < effs.Length; e++)
                    {
                        ParticleSystemRenderer eff = effs[e];
                        if (eff)
                        {
                            eff.sortingOrder = eff.sortingOrder + 30;
                        }
                    }
                }

                //var overrideOrder = playerVisual.gameObject.AddMissingComponent<SetRenderOrder>();
                //if (overrideOrder)
                //{
                //    overrideOrder.listSkinnedMesh = playerVisual.GetComponentsInChildren<SkinnedMeshRenderer>();
                //    overrideOrder.listMesh = playerVisual.GetComponentsInChildren<MeshRenderer>();
                //    overrideOrder.RenderOrder = 19;
                //}
            }
        }

        private void InitItems()
        {
            //if (mInitedItems) return;
            if (GameClient.instance == null ||
                GameClient.instance.UInfo == null ||
                GameClient.instance.UInfo.ListHeroes == null)
            {
                return;
            }

            long crGold = GameClient.instance.UInfo.Gamer.Gold;
            int crHeroStone = 0;
            MaterialData mat = GameClient.instance.UInfo.ListMaterials.Find(e => e.Name == "M_HeroStone");
            if (mat != null) crHeroStone = mat.Num;

            var heroes = ConfigManager.otherConfig["Heroes"];
            int countItem = 0;
            for (int i = 0; i < heroes.Count; ++i)
            {
                var hName = heroes[i].ToString();
                if (ConfigManager.UnitStats.ContainsKey(hName))
                {
                    var packageName = GetPackageName(hName);
                    var offerData = GameClient.instance.UInfo.ListOffers.Find(e => e.PackageName == packageName);
                    var heroData = GameClient.instance.UInfo.ListHeroes.Find(e => e.Name == hName);
                    if (heroData != null || offerData != null)
                    {
                        HeroAvatar itemAvatar;
                        if (mListItems.Count > countItem)
                        {
                            itemAvatar = mListItems[countItem];
                        }
                        else
                        {
                            itemAvatar = Instantiate(itemPrefab, itemParent);
                            mListItems.Add(itemAvatar);
                        }

                        countItem++;

                        itemAvatar.SetItem(hName);
                        var itemBtn = itemAvatar.GetComponent<Button>();
                        itemBtn.onClick.AddListener(() => OnItemClick(itemAvatar));
                        itemAvatar.gameObject.SetActive(true);
                        itemAvatar.IsEquiped = IsEquipedHero(hName);

                        if ((string.IsNullOrEmpty(selectedHero) && itemAvatar.IsEquiped) ||
                            selectedHero == hName)
                        {
                            itemAvatar.IsSelected = true;
                            selectedHero = hName;
                            currentVk = GetCurrentVuKhi();
                            SetHeroInfo(itemAvatar);
                        }
                        else
                        {
                            itemAvatar.IsSelected = false;
                        }

                        itemAvatar.IsLocked = false;
                        //itemAvatar.IsLocked = heroData == null;

                        itemAvatar.SetNotifyUpgrade(false);

                        if (heroData != null && heroData.Level < ConfigManager.HeroUpgradeCfg.GoldCost.Length)
                        {
                            int reqGold = ConfigManager.HeroUpgradeCfg.GoldCost[heroData.Level];
                            int reqHeroStone = ConfigManager.HeroUpgradeCfg.HeroStoneCost[heroData.Level];
                            if (reqGold <= crGold && reqHeroStone <= crHeroStone)
                            {
                                itemAvatar.SetNotifyUpgrade(true);
                            }
                        }
                    }
                }
            }

            for (int i = countItem; i < mListItems.Count; ++i)
            {
                mListItems[i].gameObject.SetActive(false);
            }

            itemPrefab.gameObject.SetActive(false);

            //mInitedItems = true;
        }

        void OnItemClick(HeroAvatar selectedItem)
        {
            if (selectedItem.IsLocked) return;
            //if (selectedItem.IsSelected)
            //{
            //    if (selectedFusingItems.Contains(selectedItem))
            //    {
            //        OnUnSelectFusedItems(selectedItem);
            //        return;
            //    }
            //}

            if (selectedItem.IsSelected == false)
            {
                selectedItem.IsSelected = true;

                for (int i = 0; i < mListItems.Count; ++i)
                {
                    if (mListItems[i] != selectedItem)
                    {
                        mListItems[i].IsSelected = false;
                    }
                }
            }

            selectedHero = selectedItem.TBName;

            SetHeroInfo(selectedItem);
        }

        string GetCurrentVuKhi()
        {
            var uInfo = GameClient.instance.UInfo;
            var curHero = uInfo.ListHeroes.Find(e => e.Name == uInfo.Gamer.Hero);

            if (curHero != null &&
                curHero.ListSlots != null)
            {
                for (int i = 0; i < curHero.ListSlots.Length; ++i)
                {
                    var s = curHero.ListSlots[i];
                    if (s.Slot == SlotType.VuKhi)
                    {
                        if (string.IsNullOrEmpty(s.TrangBi) == false)
                        {
                            var tb = uInfo.ListTrangBi.Find(e => e.ID == s.TrangBi);
                            if (tb != null)
                            {
                                return tb.Name;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            return string.Empty;
        }

        void SetBuyInfo(string packageName)
        {
            var uInfo = GameClient.instance.UInfo;
            var data = uInfo.ListOffers.Find(e => e.PackageName == packageName);
            var offerCfg = data.GetConfig();
            var cost = data.GetCost();
            if (cost > 0)
            {
                btnPurchase.gameObject.SetActive(false);

                gemGrp.gameObject.SetActive(true);
                lbPrice.gameObject.SetActive(false);
                if (freeGrp)
                    freeGrp.gameObject.SetActive(false);
                if (lbGem)
                    lbGem.text = data.GetCost().ToString();
                Hiker.HikerUtils.DoAction(this, () =>
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(lbGem.transform.parent as RectTransform);
                }, 0.05f, true);
                
            }
            else if (cost == 0)
            {
                btnPurchase.gameObject.SetActive(true);

                if (gemGrp)
                    gemGrp.gameObject.SetActive(false);
                lbPrice.gameObject.SetActive(true);
                if (freeGrp)
                    freeGrp.gameObject.SetActive(false);

                lbPrice.text = offerCfg.Price;
            }
            else
            {
                btnPurchase.gameObject.SetActive(false);
                if (gemGrp)
                    gemGrp.gameObject.SetActive(false);
                lbPrice.gameObject.SetActive(false);
                if (freeGrp)
                    freeGrp.gameObject.SetActive(true);
            }

#if IAP_BUILD
            var local_price = UnityIAPManager.instance.GetLocalPrice(data.PackageName);
            if (!string.IsNullOrEmpty(local_price))
            {
                this.lbPrice.text = local_price;
            }
            this.btnPurchase.interactable = !UnityIAPManager.instance.CheckPurchaseIsMissing(data.PackageName);
#endif
        }

        static Dictionary<string, string> tempParas = new Dictionary<string, string>();
        void SetHeroInfo(HeroAvatar item)
        {
            Load3DVisual(item.TBName, currentVk);
            HeroNameTitle.text = Localization.Get(item.TBName + "_Name");
            HeroNameDesc.text = Localization.Get(item.TBName + "_Desc");
            var spritecol = Resources.Load<SpriteCollection>("HeroSkillAvatar");
            if (ConfigManager.GetHeroSkillCfg(item.TBName + "Skill") == null)
            {
                skillImg.gameObject.SetActive(false);
            }
            else
            {
                skillImg.gameObject.SetActive(true);
                skillImg.sprite = spritecol.GetSprite(item.TBName + "Skill");
            }
            tempParas.Clear();
            HeroSkillDesc.text = GetSkillDescription(item.TBName, tempParas);

            var packageName = GetPackageName(item.TBName);

            var uInfo = GameClient.instance.UInfo;
            HeroData hero = uInfo.ListHeroes.Find(e => e.Name == item.TBName);
            int crLevel = 1;
            if (hero != null) crLevel = hero.Level;
            HeroNameTitle2.text = string.Format(Localization.Get("LevelLabel"), crLevel, ConfigManager.GetHeroLevelMax(item.TBName));
            if (hero != null)
            {
                btnPurchase.gameObject.SetActive(false);
                freeGrp.gameObject.SetActive(false);
                gemGrp.gameObject.SetActive(false);
                btnVideo.gameObject.SetActive(false);

                if (item.TBName != uInfo.Gamer.Hero)
                {
                    btnSelect.gameObject.SetActive(true);
                    selectedObj.gameObject.SetActive(false);
                }
                else
                {
                    btnHeroUpgrade.gameObject.SetActive(false);
                    btnSelect.gameObject.SetActive(false);
                    selectedObj.gameObject.SetActive(true);
                }
                btnHeroUpgrade.gameObject.SetActive(!GroupChapter.IsLockLeoThap(uInfo));
            }
            else
            {
                btnVideo.gameObject.SetActive(true);
                btnHeroUpgrade.gameObject.SetActive(false);
                btnSelect.gameObject.SetActive(false);
                selectedObj.gameObject.SetActive(false);
                SetBuyInfo(packageName);
            }
        }

        public void Init()
        {
            InitItems();

            //// update trang bi avatar info
            //for (int i = mListItems.Count - 1; i >= 0; --i)
            //{
            //    var item = mListItems[i];
            //    if (item == null) continue;
            //    var itemData = GameClient.instance.UInfo.ListTrangBi.Find(e => e.ID == item.name);
            //    if (itemData != null)
            //    {
            //        item.SetItem(itemData.Name, itemData.Rarity, itemData.Level);
            //    }
            //    else
            //    {
            //        mListItems.Remove(item);
            //        Destroy(item.gameObject);
            //        //mDicItems.Remove(item.name);
            //    }
            //}
            //// update equipment

            //SortItems();

            //selectedFusingItems.Clear();
            //selectedFusingItems.Add(null);
            //selectedFusingItems.Add(null);
            //selectedFusingItems.Add(null);
            //UpdateItemSelectState();

            PlayerPrefs.SetInt("ShowChangeHeroEff_" + GameClient.instance.UInfo.GID, 1);
            PlayerPrefs.SetInt("ShowUnlockHeroTooltip_" + GameClient.instance.UInfo.GID, 1);
        }

        bool IsEquipedHero(string hName)
        {
            var uInfo = GameClient.instance.UInfo;
            var heroData = uInfo.ListHeroes.Find(e => e.Name == hName);
            if (heroData != null)
            {
                return hName == uInfo.Gamer.Hero;
            }
            return false;
        }

        protected override void Hide()
        {
            base.Hide();
            ScreenMain.instance.grpEquipment.CheckHeroUnlock();
        }

        [GUIDelegate]
        public void OnChooseBtnClick()
        {
            if (selectedHero == GameClient.instance.UInfo.Gamer.Hero) return;

            HeroData heroData = null;
            HeroData curData = null;
            for (int i = GameClient.instance.UInfo.ListHeroes.Count - 1; i >= 0; --i)
            {
                var hData = GameClient.instance.UInfo.ListHeroes[i];
                if (hData != null)
                {
                    if (hData.Name == selectedHero)
                    {
                        heroData = hData;
                    }
                    if (hData.Name == GameClient.instance.UInfo.Gamer.Hero)
                    {
                        curData = hData;
                    }
                }
                if (heroData != null && curData != null) break;
            }

            if (heroData != null && curData != null)
                GameClient.instance.RequestSelectHero(heroData, curData);
        }

        public static string GetPackageName(string hName)
        {
            var packageName = "H_" + hName + "_Offer";
            return packageName;
        }

        [GUIDelegate]
        public void OnBuyByGem()
        {
            var packageName = GetPackageName(selectedHero);
            var mData = GameClient.instance.UInfo.ListOffers.Find(e => e.PackageName == packageName);
            if (mData != null && mData.BuyCount >= mData.StockCount)
            {
                PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("Mes_OutOfStock"));
                return;
            }

            if (mData != null && mData.ExpireTime <= GameClient.instance.ServerTime)
            {
                PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("Mes_OfferTimeOut"));
                return;
            }

            var cost = mData.GetCost();
            if (cost < 0)
            {
                // free
                GameClient.instance.RequestBuyOfferBuyGem(packageName);
            }
            else if (cost > 0)
            {
                if (GameClient.instance.UInfo.Gamer.Gem < cost)
                {
                    PopupBuyGem.Create(() => Init());
                    return;
                }
                GameClient.instance.RequestBuyOfferBuyGem(packageName);
            }

            AnalyticsManager.LogEvent("BUY_HERO_" + selectedHero);
        }

        [GUIDelegate]
        public void OnBtnPurchaseClick()
        {
#if IAP_BUILD
            if (UnityIAPManager.instance.CheckLoginToPurchase() == false)
            {
                return;
            }
#endif

            var packageName = GetPackageName(selectedHero);
            var mData = GameClient.instance.UInfo.ListOffers.Find(e => e.PackageName == packageName);
            if (mData != null && mData.BuyCount >= mData.StockCount)
            {
                PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("Mes_OutOfStock"));
                return;
            }

            if (mData != null && mData.ExpireTime <= GameClient.instance.ServerTime)
            {
                PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("Mes_OfferTimeOut"));
                return;
            }

            RequestPurchase(packageName);
        }

        private void RequestPurchase(string packageName)
        {
#if UNITY_STANDALONE || GM_BUILD
            //  string testReceip = "{\"Store\":\"GooglePlay\",\"TransactionID\":\"GPA.3338-2557-9554-32252\",\"Payload\":\"{\\\"json\\\":\\\"{\\\\\\\"orderId\\\\\\\":\\\\\\\"GPA.3338-2557-9554-32252\\\\\\\",\\\\\\\"packageName\\\\\\\":\\\\\\\"com.hikergames.caravanwar\\\\\\\",\\\\\\\"productId\\\\\\\":\\\\\\\"com.hikergames.caravanwar.personal_deal_2\\\\\\\",\\\\\\\"purchaseTime\\\\\\\":1506941883100,\\\\\\\"purchaseState\\\\\\\":0,\\\\\\\"purchaseToken\\\\\\\":\\\\\\\"dodinjlppgaikbpakneimfij.AO-J1Ozuie_qSeca-Tml9P51Gl2VSX1w_uygJ268khknuD8MDMR72ou81jBPA6bjIK_SIGjhl1w-ve8M5zKxmSAoLhSUEwqhWBemjWtFwufwhd-NsH6lyCsLicwarUS-GgGOUbe6d471ZHZnkRBvY7bkwtZwOpWcs9cuBEkIhhA5fSFrOGZxL5A\\\\\\\"}\\\",\\\"signature\\\":\\\"UMS1HqoIGhGV7e5fi8gzkruxZgaobuhQSca7AROB8QjP7u1r3qthJC+Qry1uqtTmuKj0dp220D9e7VJVQ5MzsdmIOj9TnMOY61MrkT6jF5j0vg5w93a0ddk\\\\/xs\\\\/oKhLnM5W5juus6eP2kvFwXtQ5SuNDYcP7kbHr8gJ6AAeJLYTeIWCZC6xHYfCb7bC4wy6rCz2tDzk8P1TWIckUP6er4QfknPDhewfjSGiw\\\\/jlKYsFfoM6X669JR6FgutWT2fSslNVRF+JQTfKOPuUrHvBvndJnUUbExiVC57lRoOfDYDfzK\\\\/WxtpzkTddrnd8UnOER+mgajHYbcBe8nb3ZvD0oDA==\\\"}\"}";
            //PopupConfirmInputText.Create("Buy Gem Pack", "Password require for sercurity!", (pass) =>
            //{
            GameClient.instance.RequestBuyOfferStore(packageName, string.Empty, "asvrtr4654hfghfgh");
            //});
            return;
#endif

#if IAP_BUILD
            if (UnityIAPManager.instance != null)
                UnityIAPManager.instance.BuyStorePack(packageName);

            AnalyticsManager.LogEvent("IAP_CLICK",
                new AnalyticsParameter("Name", packageName),
                new AnalyticsParameter("Value", this.lbPrice.text));
#endif

            AnalyticsManager.LogEvent("BUY_HERO_" + selectedHero);
        }

        private const string defaultDescColor = "393939FF";
        private const string valueNormalColor = "DF251BFF";
        private const string upgradeColor = "11C102FF";

        //private static bool ProcessDesc(ref string desc, string key, string descKey, float val, Dictionary<string, string> paras)
        //{
        //    if (string.IsNullOrEmpty(descKey))
        //        descKey = "[" + key + "]";

        //    var temp = !paras.ContainsKey(key) ? val : float.Parse(paras[key]);
        //    var delta = val - temp;
        //    if (delta == 0) desc = desc.Replace(descKey, "[vvv]" + val + "[-][xxx]");
        //    else desc = desc.Replace(descKey, "[-][vvv]" + val + "[-][xxx]([-][uuu]+" + delta + "[-][xxx])");
        //    paras[key] = val.ToString();
        //    return delta != 0;
        //}
        //private static bool ProcessDesc(ref string desc, string key, string descKey, long val, Dictionary<string, string> paras)
        //{
        //    if (string.IsNullOrEmpty(descKey))
        //        descKey = "[" + key + "]";

        //    var temp = !paras.ContainsKey(key) ? val : long.Parse(paras[key]);
        //    var delta = val - temp;
        //    if (delta == 0) desc = desc.Replace(descKey, "[-][vvv]" + val + "[-][xxx]");
        //    else desc = desc.Replace(descKey, "[-][vvv]" + val + "[-][xxx]([-][uuu]+" + delta + "[-][xxx])");
        //    paras[key] = val.ToString();
        //    return delta != 0;
        //}

        static void ReplaceDesc(ref string desc, string descKey, long val)
        {
            desc = desc.Replace(descKey + "[f100]", "<color=#vvv>" + val / 100f + "</color>");
            desc = desc.Replace(descKey + "[p100]", "<color=#vvv>" + val / 100f + "%</color>");
            desc = desc.Replace(descKey + "[p]", "<color=#vvv>" + val + "%</color>");
            desc = desc.Replace(descKey + "[i]", "<color=#vvv>" + val + "</color>");
        }

        static void ReplaceDescEnhanced(ref string desc, string descKey, long val, long delta)
        {
            desc = desc.Replace(descKey + "[f100]", "<color=#vvv>" + val / 100f + "</color>(<color=#uuu>" + delta / 100f + "</color>)");
            desc = desc.Replace(descKey + "[p100]", "<color=#vvv>" + val / 100f + "%</color>(<color=#uuu>" + delta / 100f + "%</color>)");
            desc = desc.Replace(descKey + "[p]", "<color=#vvv>" + val + "%</color>(<color=#uuu>" + delta + "%</color>)");
            desc = desc.Replace(descKey + "[i]", "<color=#vvv>" + val + "</color>(<color=#uuu>" + delta + "</color>)");
        }

        private static bool ProcessDesc(ref string desc, string key, string descKey, long val, Dictionary<string, string> paras)
        {
            if (string.IsNullOrEmpty(descKey))
                descKey = "[" + key + "]";

            var temp = !paras.ContainsKey(key) ? val : long.Parse(paras[key]);
            var delta = val - temp;
            // norm desc
            if (delta == 0) ReplaceDesc(ref desc, descKey, val);
            else // upgrade desc
                ReplaceDescEnhanced(ref desc, descKey, val, delta);
            paras[key] = val.ToString();
            return delta != 0;
        }

        public static string GetSkillDescription(string unitName, Dictionary<string, string> paras)
        {
            string keyLocalization = string.Format("{0}_SkillDesc", unitName);
            string desc = string.Empty;

            if (Localization.dictionary.ContainsKey(keyLocalization) == false)
            {
                return desc;
            }
            else
            {
                desc = Localization.Get(keyLocalization);
            }

            //desc = desc.Insert(0, "[xxx]");
            //desc = desc + "[-]";
            //float[] efxDurations = GetSkillEffectDurations(skillName, skillLevel, unit_star);
            //float[] efxDurations3 = GetSkillEffectDurations3(skillName, skillLevel, unit_star);
            //float[] efxValues = GetSkillEffectValues(skillName, skillLevel, unit_star);
            //float value = GetSkillValue(skillName, skillLevel, unit_star);
            //float value2 = GetSkillValue2(skillName, skillLevel, unit_star);
            //float value3 = GetSkillValue3(skillName, skillLevel, unit_star);
            //float value4 = GetSkillValue4(skillName, skillLevel, unit_star);
            //float chance = GetSkillChance(skillName, skillLevel, unit_star);

            //isEnhance = false;

            //float cooldown = GetSkillCooldown(skillName, skillLevel, unit_star);
            //if (cooldown > 0)
            //{
            //    isEnhance |= ProcessDesc(ref desc, "cooldown", string.Empty, cooldown, paras);
            //}

            //isEnhance |= ProcessDesc(ref desc, "chance", string.Empty, chance, paras);
            //isEnhance |= ProcessDesc(ref desc, "value", string.Empty, value, paras);
            //isEnhance |= ProcessDesc(ref desc, "value2", string.Empty, value2, paras);
            //isEnhance |= ProcessDesc(ref desc, "value3", string.Empty, value3, paras);
            //isEnhance |= ProcessDesc(ref desc, "value4", string.Empty, value4, paras);

            if (ConfigManager.HeroSkills.Contains(unitName + "Skill") == false)
            {
                // not have skill (Zora)
                return desc;
            }

            var cfg = ConfigManager.HeroSkills[unitName + "Skill"];
            var jsParam = cfg["Params"];

            for (int i = 0; i < jsParam.Count; ++i)
            {
                ProcessDesc(ref desc, "param" + i, string.Empty, jsParam[i].ToLong(), paras);
            }

            //string summonUnit = GetSkillSummonUnit(skillName);
            //if (!string.IsNullOrEmpty(summonUnit))
            //{
            //    JsonData unitConfig = BattleUnitUtils.GetUnitConfigByName(summonUnit);

            //    var hp = BattleUnitUtils.GetUnitStatsLongValue(unitConfig, "HP", unitLevel);
            //    hp = BattleUnitUtils.GetEnhanceValue(hp, unit_star);
            //    isEnhance |= ProcessDesc(ref desc, "HP", string.Empty, hp, paras);

            //    var damage = BattleUnitUtils.GetUnitStatsLongValue(unitConfig, "damage", unitLevel);
            //    damage = BattleUnitUtils.GetEnhanceValue(damage, unit_star);
            //    isEnhance |= ProcessDesc(ref desc, "damage", string.Empty, damage, paras);
            //}

            //for (int i = 0; i < efxDurations.Length; i++)
            //{
            //    string key = string.Format("[efx_duration_{0}]", i);

            //    isEnhance |= ProcessDesc(ref desc, key, key, efxDurations[i], paras);
            //    //temp = !paras.ContainsKey(key) ? efxDurations[i] : paras[key];
            //    //delta = efxDurations[i] - temp;
            //    //if (delta == 0) desc = desc.Replace(key, "[-][vvv]" + efxDurations[i] + "[-][xxx]");
            //    //else desc = desc.Replace(key, "[-][vvv]" + efxDurations[i] + "[-][xxx]([-][uuu]+" + delta + "[-][xxx])");
            //    //paras[key] = efxDurations[i];
            //}

            //for (int i = 0; i < efxDurations3.Length; i++)
            //{
            //    string key = string.Format("[efx3_duration_{0}]", i);
            //    isEnhance |= ProcessDesc(ref desc, key, key, efxDurations3[i], paras);
            //    //temp = !paras.ContainsKey(key) ? efxDurations3[i] : paras[key];
            //    //delta = efxDurations3[i] - temp;
            //    //if (delta == 0) desc = desc.Replace(key, "[-][vvv]" + efxDurations3[i] + "[-][xxx]");
            //    //else desc = desc.Replace(key, "[-][vvv]" + efxDurations3[i] + "[-][xxx]([-][uuu]+" + delta + "[-][xxx])");
            //    //paras[key] = efxDurations3[i];
            //}

            //for (int i = 0; i < efxValues.Length; i++)
            //{
            //    string key = string.Format("[efx_value_{0}]", i);
            //    isEnhance |= ProcessDesc(ref desc, key, key, efxValues[i], paras);
            //    //temp = !paras.ContainsKey(key) ? efxValues[i] : paras[key];
            //    //delta = efxValues[i] - temp;
            //    //if (delta == 0) desc = desc.Replace(key, "[-][vvv]" + efxValues[i] + "[-][xxx]");
            //    //else desc = desc.Replace(key, "[-][vvv]" + efxValues[i] + "[-][xxx]([-][uuu]+" + delta + "[-][xxx])");
            //    //paras[key] = efxValues[i];
            //}

            desc = desc.Replace("xxx", defaultDescColor);
            desc = desc.Replace("vvv", valueNormalColor);
            desc = desc.Replace("uuu", upgradeColor);

            return desc;
        }

        [GUIDelegate]
        public void OnBtnHeroUpgradeClick()
        {
            if (!string.IsNullOrEmpty(selectedHero))
            {
                HeroData hero = GameClient.instance.UInfo.ListHeroes.Find(e => e.Name == selectedHero);
                if (hero != null) PopupHeroUpgrade.Create(hero);
            }
        }

        [GUIDelegate]
        public void OnBtnVideoClick()
        {
            PopupPlayVideo.Create(new string[]
            {
                Localization.GetLocalization(selectedHero + "_URL3", ConfigManager.language),
                Localization.GetLocalization(selectedHero + "_URL2", ConfigManager.language),
                Localization.GetLocalization(selectedHero + "_URL1", ConfigManager.language),
            }, true);
        }
    }
}