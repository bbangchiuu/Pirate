using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hiker.GUI.Shootero
{
    using UnityEngine.UI;

    public class MaterialAvatar : AvatarBase
    {
        public Image icon;
        public Text lbQuantity;
        public Image frame;
        public Transform bg;
        //public GameObject selectedObj;
        //public GameObject lockedObj;
        //public GameObject previewObj;

        SpriteCollection spritesCol;
        public string MaterialName { get; private set; }
        public long MaterialQuantity { get; private set; }

        public void SetItem(string name, long quantity)
        {
            MaterialName = name;
            MaterialQuantity = quantity;
            bool isShowFrame = true;
            if(name.Contains("M_TheLuc"))
            {
                isShowFrame = false;
            }
            if (spritesCol == null)
            {
                spritesCol = Resources.Load<SpriteCollection>("MaterialAvatar");
            }

            if(bg == null)
            {
                bg = this.transform.Find("bg");
            }
            if (bg != null)
            {
                bg.gameObject.SetActive(isShowFrame);
            }

            if (frame != null)
            {
                if (name.StartsWith("RE_"))
                {
                    var reSplits = name.Split('_');
                    var rarity = int.Parse(reSplits[2]);
                    frame.sprite = spritesCol.GetSprite("bgr_rank_" + ((int)rarity + 1));
                }
                else
                {
                    frame.sprite = spritesCol.GetSprite("treasure_Material");
                }
                frame.gameObject.SetActive(isShowFrame);
            }

            icon.gameObject.SetActive(true);
            icon.sprite = spritesCol.GetSprite("icon_" + name);

            lbQuantity.text = string.Format(Localization.Get("ShortQuantityLabel"), quantity);

            tooltip.enabled = showTooltip;
            if (showTooltip)
            {
                if (tooltip != null)
                {
                    tooltip.text = Localization.Get(name + "_Desc");
                }
            }
        }
    }
}
