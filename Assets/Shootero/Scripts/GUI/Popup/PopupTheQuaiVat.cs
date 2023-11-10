using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;
    using UnityEngine.UI;

    public class PopupTheQuaiVat : PopupBase
    {
        public Transform itemParent;
        public CardAvatar itemPrefab;

        public Text lbBtnOpenByDust, lbBtnOpenByGem;
        public Button btnOpenByDust, btnOpenByGem;
        public Text emptyList;
        public Text lbFilter;

        public static PopupTheQuaiVat instance;

        bool isEnoughGem = false;
        bool isEnoughDust = false;

        List<CardAvatar> mListItems = new List<CardAvatar>();

        const int MaxFilter = 4;
        int filter = MaxFilter;

        public List<TweenPosition> listDustIcons;
        public TweenPosition twGrpDusts;

        int SortByRarity(CardAvatar it1, CardAvatar it2)
        {
            if (it1 == null || it2 == null) return 0;
            if (it1.gameObject.activeSelf && it2.gameObject.activeSelf == false) return -1;
            if (it1.gameObject.activeSelf == false && it2.gameObject.activeSelf) return 1;
            if (it1.gameObject.activeSelf == false && it2.gameObject.activeSelf == false) return 0;

            var tb1 = GameClient.instance.UInfo.listCards.Find(e => e.ID == it1.TBId);
            var tb2 = GameClient.instance.UInfo.listCards.Find(e => e.ID == it2.TBId);
            if (tb1 == null || tb2 == null) return 1;
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

        public static PopupTheQuaiVat Create()
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupTheQuaiVat");
            instance = go.GetComponent<PopupTheQuaiVat>();
            //EventDelegate.Add(instance.twGrpDusts.onFinished, () =>
            //{
            //    instance.SyncNetworkData();
            //});
            instance.filter = MaxFilter;
            instance.OnFilter();
            instance.Init();
            return instance;
        }
        public void Init()
        {
            if (GameClient.instance == null ||
                 GameClient.instance.UInfo == null)
            {
                return;
            }

            UserInfo uInfo = GameClient.instance.UInfo;
            ChestConfig chestCfg = ConfigManager.ChestCfg["Chest_UnitCard"];
            int gemCost = chestCfg.unlockKey["Gem"];
            int dustCost = chestCfg.unlockKey["M_CardDust"];

            var matData = uInfo.ListMaterials.Find(e => e.Name == "M_CardDust");
            int haveDust = 0;
            if (matData != null)
            {
                haveDust = matData.Num;
            }

            if (haveDust >= dustCost)
            {
                isEnoughDust = true;
                lbBtnOpenByDust.text = string.Format("{0}/{1}", haveDust, dustCost);
            }
            else
            {
                isEnoughDust = false;
                lbBtnOpenByDust.supportRichText = true;
                lbBtnOpenByDust.text = string.Format("<color=red>{0}</color>/{1}", haveDust, dustCost);
            }

            if (uInfo.Gamer.Gem >= gemCost)
            {
                isEnoughGem = true;
                lbBtnOpenByGem.text = string.Format("{0}", gemCost);
            }
            else
            {
                isEnoughGem = false;
                lbBtnOpenByGem.supportRichText = true;
                lbBtnOpenByGem.text = string.Format("<color=red>{0}</color>", gemCost);
            }
            
            btnOpenByDust.interactable = isEnoughDust;
            btnOpenByGem.interactable = isEnoughGem;

            LayoutRebuilder.ForceRebuildLayoutImmediate(lbBtnOpenByGem.transform.parent as RectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(lbBtnOpenByDust.transform.parent as RectTransform);

            int countItem = 0;
            if (uInfo.listCards != null && uInfo.listCards.Count > 0)
            {
                for (int i = 0; i < uInfo.listCards.Count; ++i)
                {
                    CardData cardData = uInfo.listCards[i];
                    if (ConfigManager.CardStats.ContainsKey(cardData.Name))
                    {
                        var cardCfg = ConfigManager.GetCardConfig(cardData);
                        if (filter != MaxFilter && (int)cardCfg.Slot != filter)
                        {
                            continue;
                        }

                        CardAvatar itemAvatar;
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

                        itemAvatar.SetItem(cardData.Name,cardData.Rarity, cardData.ID);
                        var itemBtn = itemAvatar.GetComponent<Button>();
                        itemBtn.onClick.AddListener(() => OnItemClick(cardData));
                        itemAvatar.gameObject.SetActive(true);

                        itemAvatar.IsLocked = false;
                        itemAvatar.IsEquiped = IsEquipedCard(cardData.ID);
                        itemAvatar.SetNotifyUpgrade(false);
                    }
                }
            }

            for (int i = countItem; i < mListItems.Count; ++i)
            {
                mListItems[i].gameObject.SetActive(false);
            }

            itemPrefab.gameObject.SetActive(false);

            SortItems();

            emptyList.gameObject.SetActive(countItem == 0);
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
                        if(tbData != null && tbData.CardSlots != null)
                        {
                            if (tbData.CardSlots.Contains(cardID)) return true;
                        }
                    }
                }
            }
            return false;
        }

        public void SyncNetworkData()
        {
            Init();
        }

        void OnItemClick(CardData selectedItem)
        {
            PopupCardInfo.Create(selectedItem, false, true, false);
        }

        void OnFilter()
        {
            if (filter == MaxFilter)
            {
                lbFilter.text = Localization.Get("All").ToUpper();
            }
            else
            {
                lbFilter.text = Localization.Get(((SlotType)filter).ToString());
            }
        }

        [GUIDelegate]
        public void OnBtnFilterClick()
        {
            filter = (filter + 1) % (MaxFilter + 1);

            OnFilter();

            Init();
        }

        public void PlayDustEffect()
        {

            if (listDustIcons != null)
            {
                foreach (TweenPosition twDust in listDustIcons)
                {
                    if (twDust == null)
                    {
                        continue;
                    }
                    Vector3 startPos = new Vector3(Random.Range(-400f, 400f), Random.Range(-500f, 500f), 0);
                    twDust.transform.localPosition = startPos;
                    twDust.from = startPos;
                    twDust.to = Vector3.zero;
                    twDust.ResetToBeginning();
                    twDust.enabled = true;
                    twDust.PlayForward();
                }
            }
            if (twGrpDusts && twGrpDusts.transform)
            {
                twGrpDusts.transform.localPosition = Vector3.zero;
                twGrpDusts.from = Vector3.zero;
                twGrpDusts.to = PopupTheQuaiVat.instance.lbBtnOpenByDust.transform.localPosition;
                twGrpDusts.ResetToBeginning();
                twGrpDusts.enabled = true;
                twGrpDusts.gameObject.SetActive(true);
                twGrpDusts.PlayForward();
            }
        }

        [GUIDelegate]
        public void OnBtnOpenDustClick()
        {
            if (isEnoughDust)
            {
                GameClient.instance.RequestOpenUnitCardChestByDust();
            }
        }

        [GUIDelegate]
        public void OnBtnOpenGemClick()
        {
            if (isEnoughGem)
            {
                GameClient.instance.RequestOpenUnitCardChestByGem();
            }
        }

        [GUIDelegate]
        public void OnBtnCardChestInfoClick()
        {
            PopupAllCards.Create();
        }

        [GUIDelegate]
        public void OnBtnRecycleCardsClick()
        {
            PopupRecycleCard.Create();
        }
    }
}