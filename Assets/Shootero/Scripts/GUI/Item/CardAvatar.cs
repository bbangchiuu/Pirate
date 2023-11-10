using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;
    using UnityEngine.UI;

    public class CardAvatar : MonoBehaviour
    {
        public Image bg;
        public Image icon;
        public Image back;
        //public Text lbLevel;

        public GameObject selectedObj;
        public GameObject lockedObj;
        public GameObject equipedObj;
        public GameObject grpNotify;
        public GameObject grpEmpty;
        public GameObject grpBack;

        SpriteCollection spritesCol;

        public string TBName { get; private set; }
        public ERarity TBRarity { get; private set; }
        public string TBId { get; private set; }

        TweenRotation twRot;

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

        public bool IsEmpty
        {
            get
            {
                return grpEmpty != null && grpEmpty.activeSelf;
            }
            set
            {
                if (grpEmpty)
                {
                    grpEmpty.SetActive(value);
                    icon.gameObject.SetActive(!value);
                    bg.gameObject.SetActive(!value);
                }
            }
        }

        public bool IsBack
        {
            get
            {
                return grpBack != null && grpBack.activeSelf;
            }
            set
            {
                if (grpBack)
                {
                    grpBack.SetActive(value);
                    icon.gameObject.SetActive(!value);
                    bg.gameObject.SetActive(!value);
                }
            }
        }

        public void SetNotifyUpgrade(bool isNotify)
        {
            if (grpNotify) grpNotify.SetActive(isNotify);
        }

        public void SetItem(string name, ERarity rarity = ERarity.Common, string id = "")
        {
            IsEmpty = false;

            TBName = name;
            TBRarity = rarity;
            TBId = id;

            if (spritesCol == null)
            {
                spritesCol = Resources.Load<SpriteCollection>("CardAvatar");
            }

            icon.sprite = spritesCol.GetSprite(TBName);
            bg.sprite = spritesCol.GetSprite("frame_card_" + ((int)rarity));
            back.sprite = spritesCol.GetSprite("frame_card_back_" + ((int)rarity));

            //lbLevel.text = string.Format(Localization.Get("ShortLevelLabel"), level);
        }

        public void PlayFlip()
        {
            if (twRot == null)
            {
                twRot = this.GetComponent<TweenRotation>();
                EventDelegate.Add(twRot.onFinished, () =>
                {
                    if (twRot.direction == AnimationOrTween.Direction.Forward)
                    {
                        IsBack = false;
                        twRot.enabled = true;
                        twRot.PlayReverse();
                    }
                    else
                    {
                        //this.transform.rotation = Quaternion.identity;
                    }
                });
            }

            
            twRot.ResetToBeginning();
            twRot.enabled = true;
            twRot.PlayForward();
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
