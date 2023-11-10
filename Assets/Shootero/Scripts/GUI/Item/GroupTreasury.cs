using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hiker.GUI.Shootero
{
    using UnityEngine.UI;
    using UnityEngine.UI.Extensions;
    public class GroupTreasury : NetworkDataSync
    {
        public ScrollRect scrollView;
        public GameObject OfferTitles;
        public HorizontalScrollSnap scrollOffers;
        public TreasureOfferItem offerItemPrefab;
        public TreasureGemPackItem[] GemPacks;

        public TreasureChestItem[] Chests;

        public PremiumCard premiumCard;

        bool mInitedItems = false;
        bool autoScrollOffer = false;
        Animator animatorChest;

        protected override void OnEnable()
        {
            base.OnEnable();
            //InitChest("Chest_Ambush");
            InitItems();
        }

        public override void SyncNetworkData()
        {
            //InitItems();
            if (GameClient.instance == null ||
                GameClient.instance.UInfo == null ||
                GameClient.instance.UInfo.ListOffers == null)
            {
                return;
            }

            if (GameClient.instance.UInfo.GetCurrentChapter() >= ConfigManager.GetPremiumChapterUnlock())
            {
                premiumCard.transform.parent.gameObject.SetActive(true);
                premiumCard.SetItems();
            }
            else
            {
                premiumCard.transform.parent.gameObject.SetActive(false);
            }


            for (int i = 0;i < Chests.Length; i++)
            {
                Chests[i].InitChest();
            }

            var gemOffers = GameClient.instance.UInfo.ListOffers.FindAll(
                e => e.Type == Networks.Data.Shootero.OfferType.GemOffer);
            gemOffers.Sort((e1, e2) => e1.PackageName.CompareTo(e2.PackageName));
            for (int i = 0; i < GemPacks.Length; ++i)
            {
                var gemPack = GemPacks[i];
                if (gemPack != null)
                {
                    if (gemOffers.Count > i)
                    {
                        gemPack.gameObject.SetActive(true);
                        gemPack.SetItem(gemOffers[i]);
                    }
                    else
                    {
                        gemPack.gameObject.SetActive(false);
                    }
                }
            }

            #region Offers
            //var otherOffers = GameClient.instance.UInfo.ListOffers.FindAll(
            //    e => 
            //    e.Type != Networks.Data.Shootero.OfferType.GemOffer &&
            //    e.StockCount > 0 &&
            //    e.ExpireTime > TimeUtils.Now &&
            //    e.BuyCount < e.StockCount
            //    );

            //if (otherOffers.Count > 0)
            //{
            //    OfferTitles.gameObject.SetActive(true);
            //    scrollOffers.gameObject.SetActive(true);

            //    for (int i = 0; i < otherOffers.Count; ++i)
            //    {
            //        TreasureOfferItem offerItem = null;
            //        if (scrollOffers.ChildObjects.Length > i)
            //        {
            //            offerItem = scrollOffers.ChildObjects[i].GetComponent<TreasureOfferItem>();
            //        }
            //        else
            //        {
            //            offerItem = Instantiate(offerItemPrefab, scrollOffers._screensContainer);
            //            offerItem.gameObject.SetActive(true);
            //            scrollOffers.AddChild(offerItem.gameObject);
            //        }

            //        offerItem.SetItem(otherOffers[i]);
            //    }

            //    for (int i = scrollOffers.ChildObjects.Length - 1; i >= otherOffers.Count; --i)
            //    {
            //        scrollOffers.RemoveChild(i, out GameObject removed);
            //        if (removed)
            //        {
            //            removed.SetActive(false);
            //            Destroy(removed);
            //        }
            //    }

            //    autoScrollOffer = true;
            //}
            //else
            {
                autoScrollOffer = false;
                OfferTitles.gameObject.SetActive(false);
                scrollOffers.gameObject.SetActive(false);
            }
            #endregion
        }

        int scrollDirect = 0;
        float timeAutoScroll = 0;
        private void Update()
        {
            if (mInitedItems == false) return;
            if (GameClient.instance == null ||
                GameClient.instance.UInfo == null ||
                GameClient.instance.UInfo.ListOffers == null)
            {
                return;
            }

            bool isTooltipActive = BoundTooltipItem.Instance != null && BoundTooltipItem.Instance.IsActive;
            if (Input.GetMouseButtonDown(0))
            {
                if (isTooltipActive)
                {
                    if (scrollView.enabled)
                    {
                        scrollView.enabled = false;
                        scrollOffers.GetComponent<ScrollRect>().enabled = false;
                    }

                }
                else
                {
                    if (!scrollView.enabled)
                    {
                        scrollView.enabled = true;
                        scrollOffers.GetComponent<ScrollRect>().enabled = true;
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (isTooltipActive)
                {
                    if (scrollView.enabled)
                    {
                        scrollView.enabled = false;
                        scrollOffers.GetComponent<ScrollRect>().enabled = false;
                    }

                }
                else
                {
                    if (!scrollView.enabled)
                    {
                        scrollView.enabled = true;
                        scrollOffers.GetComponent<ScrollRect>().enabled = true;
                    }
                }
            }


            if (autoScrollOffer 
                && !isTooltipActive)
            {
                if (ScreenMain.instance && ScreenMain.instance.IsShowingShopPanel())
                {
                    if (scrollOffers.ChildObjects.Length > 1)
                    {
                        if (scrollDirect == 0)
                            scrollDirect = 1;

                        var curPage = scrollOffers.CurrentPage;
                        if (curPage == scrollOffers.ChildObjects.Length - 1)
                        {
                            scrollDirect = -1;
                        }
                        else if (curPage == 0 && scrollDirect == -1)
                        {
                            scrollDirect = 1;
                        }

                        timeAutoScroll += Time.unscaledDeltaTime;
                        if (timeAutoScroll >= 2f)
                        {
                            if (scrollDirect > 0)
                            {
                                scrollOffers.NextScreen();
                            }
                            else if (scrollDirect < 0)
                            {
                                scrollOffers.PreviousScreen();
                            }
                            timeAutoScroll = 0;
                        }
                    }
                    else
                    {
                        scrollDirect = 0;
                    }
                }
            }
        }

        [GUIDelegate]
        public void OnUserScrollOffer()
        {
            scrollDirect = 0;
            autoScrollOffer = false;
        }

        private void InitItems()
        {
            if (mInitedItems) return;
            if (GameClient.instance == null ||
                GameClient.instance.UInfo == null ||
                GameClient.instance.UInfo.ListOffers == null)
            {
                return;
            }

            SyncNetworkData();

            mInitedItems = true;
        }
    }
}