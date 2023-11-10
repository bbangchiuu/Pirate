using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hiker.GUI.Shootero
{
    using UnityEngine.UI;

    public class ArmoryAvatar : MonoBehaviour
    {
        public Image icon;
        public Image disableOverlay;
        public Text lbLevel;
        public Text lbName;
        public bool lvlInName = false;

        SpriteCollection spritesCol;

        public void SetItem(string name, ERarity rarity, int level)
        {
            gameObject.name = name;
            if (spritesCol == null)
            {
                spritesCol = Resources.Load<SpriteCollection>("ArmoryAvatar");
            }

            if (spritesCol)
            {
                var sp = spritesCol.GetSprite(gameObject.name);
                icon.sprite = sp;
                disableOverlay.sprite = sp;
            }

            disableOverlay.gameObject.SetActive(level <= 0);
            if (lbLevel)
            {
                if (level > 0)
                {
                    lbLevel.text = string.Format("Lv.{0}", level);
                }
                else
                {
                    lbLevel.text = Localization.Get("Locked");
                }
            }

            if (lvlInName && level > 0)
            {
                string lb = string.Format(Localization.Get("ShortLevelLabel"), level);
                lbName.text = string.Format("{0} {1}", Localization.Get(name + "_Name"), lb);
            }
            else
            {
                lbName.text = Localization.Get(name + "_Name");
            }
        }
    }
}
