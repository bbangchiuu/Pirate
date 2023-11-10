using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;
    using UnityEngine.UI;

    public class HeroAvatar : MonoBehaviour
    {
        public Image bkg;
        public Image icon;
        //public Text lbLevel;

        public GameObject selectedObj;
        public GameObject lockedObj;
        public GameObject equipedObj;
        public GameObject grpNotify;

        SpriteCollection spritesCol;
        public string TBName { get; private set; }
        public ERarity TBRarity { get; private set; }
        public int TBLevel { get; private set; }

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

        public void SetItem(string name
            //, ERarity rarity
            //, int level
            )
        {
            TBName = name;
            //TBRarity = rarity;
            //TBLevel = level;

            if (spritesCol == null)
            {
                spritesCol = Resources.Load<SpriteCollection>("HeroAvatar");
            }

            icon.sprite = spritesCol.GetSprite(name);

            //lbLevel.text = string.Format(Localization.Get("ShortLevelLabel"), level);
        }

        //[GUIDelegate]
        //public void OnViewInfoClick()
        //{
        //    TrangBiData fakeData = new TrangBiData()
        //    {
        //        Level = TBLevel,
        //        Name = TBName,
        //        Rarity = TBRarity
        //    };
        //    PopupTrangBiInfo.CreateReadonly(fakeData);
        //}
    }
}
