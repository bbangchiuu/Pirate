using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hiker.GUI
{
    using Hiker.GUI.Shootero;
    using System;
    using UnityEngine.UI;

    public class PopupBuyGem : PopupBase
    {
        public TreasureGemPackItem[] GemPacks;
        public Action OnPopupClose;

        public static PopupBuyGem Instance { get; set; }

        public static void Create(Action _onPopupClose = null)
        {
            if (Instance == null)
            {
                GameObject obj = PopupManager.instance.GetPopup("PopupBuyGem", false, Vector3.zero);
                Instance = obj.GetComponent<PopupBuyGem>();
            }
            Instance.InitItems();
            Instance.OnPopupClose = _onPopupClose;
        }

        public void InitItems()
        {
            if (ResourceBar.instance)
            {
                ResourceBar.instance.gameObject.SetActive(true);
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
        }

        protected override void OnCleanUp()
        {
            if (ScreenMain.instance && ResourceBar.instance && !ScreenMain.instance.isActiveAndEnabled)
            {
                ResourceBar.instance.gameObject.SetActive(false);
            }
            //if (SoundManager.instance && this.IsPlayCloseSound)
            //{
            //    SoundManager.instance.CloseButtonClick();
            //}
            if (OnPopupClose != null)
            {
                OnPopupClose();
                OnPopupClose = null;
            }

            if (Instance == this) Instance = null;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }
    }
}