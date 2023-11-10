using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;
    using UnityEngine.UI;

    public class ItemAvatar : MonoBehaviour
    {
        public Image bkg;
        public Image icon;
        public Text lbLevel;

        public GameObject selectedObj;
        public GameObject lockedObj;
        public GameObject previewObj;
        public GameObject equipedObj;
        public GameObject grpNotify;

        public List<GameObject> listStars;

        SpriteCollection spritesCol;
        public string TBName { get; private set; }
        public ERarity TBRarity { get; private set; }
        public int TBLevel { get; private set; }
        public int TBStar { get; private set; }

        public bool IsPreview
        {
            get
            {
                return previewObj != null && previewObj.activeSelf;
            }
            set
            {
                if (previewObj)
                {
                    previewObj.SetActive(value);
                }
            }
        }

        public bool IsSelected
        {
            get
            {
                return selectedObj != null && selectedObj.activeSelf;
            }
            set
            {
                if (selectedObj)
                {
                    selectedObj.SetActive(value);
                }
            }
        }

        public bool IsLocked
        {
            get
            {
                return lockedObj != null && lockedObj.activeSelf;
            }
            set
            {
                if (lockedObj)
                {
                    lockedObj.SetActive(value);
                }
            }
        }

        public bool IsEquiped
        {
            get
            {
                return equipedObj != null && equipedObj.activeSelf;
            }
            set
            {
                if (equipedObj)
                {
                    equipedObj.SetActive(value);
                }
            }
        }

        public void SetNotifyUpgrade(bool isNotify)
        {
            if (grpNotify) grpNotify.SetActive(isNotify);
        }

        public void SetItem(string name, ERarity rarity, int level, int star = 0, bool showLevel = true)
        {
            TBName = name;
            TBRarity = rarity;
            TBLevel = level;
            TBStar = star;

            if (spritesCol == null)
            {
                spritesCol = Resources.Load<SpriteCollection>("ItemAvatar");
            }

            if (string.IsNullOrEmpty(name))
            {
                bkg.sprite = spritesCol.GetSprite("bgr_frame_item_none");
                icon.gameObject.SetActive(false);
            }
            else
            {
                bkg.sprite = spritesCol.GetSprite("bgr_rank_" + ((int)rarity + 1));
                icon.gameObject.SetActive(true);
                icon.sprite = spritesCol.GetSprite(name);
            }

            lbLevel.text = string.Format(Localization.Get("ShortLevelLabel"), level);
            lbLevel.gameObject.SetActive(showLevel);

            if (listStars.Count > 0)
            {
                for (int i = 0; i < listStars.Count; i++)
                {
                    listStars[i].SetActive(i < star);
                }
                LayoutRebuilder.ForceRebuildLayoutImmediate(listStars[0].transform.parent as RectTransform);
            }
        }

        [GUIDelegate]
        public void OnViewInfoClick(int highlightMaxLevel = 0)
        {
            TrangBiData fakeData = new TrangBiData()
            {
                Level = TBLevel,
                Name = TBName,
                Rarity = TBRarity,
                Star = TBStar,
                CardSlots = new List<string> { null }
            };
            PopupTrangBiInfo.CreateReadonly(fakeData, highlightMaxLevel);
        }
    }
}
