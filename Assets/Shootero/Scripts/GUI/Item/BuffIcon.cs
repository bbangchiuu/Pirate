using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hiker.GUI.Shootero
{
    public class BuffIcon : MonoBehaviour
    {
        public Image iconSkill;
        public Text lblName;
        public Text lblDesc;

        static SpriteCollection buffIconCol;
        SpriteCollection GetBuffIconCollection()
        {
            if (buffIconCol == null)
            {
                buffIconCol = Resources.Load<SpriteCollection>("BuffIcons");
            }
            return buffIconCol;
        }

        public void SetBuffType(BuffType buffType,bool ShowNextLevel = false)
        {
            if (iconSkill)
            {
                iconSkill.sprite = GetBuffIconCollection().GetSprite(string.Format("{0}_Buff", buffType.ToString()));
            }

            if (lblName)
            {
                if (ShowNextLevel)
                {
                    int curr_buff_level = QuanlyNguoichoi.Instance.PlayerUnit.GetBuffCount(buffType);

                    if(curr_buff_level==0 || buffType== BuffType.HEAL)
                        lblName.text = Localization.Get(string.Format("Buff_{0}", buffType.ToString()));
                    else
                        lblName.text = Localization.Get(string.Format("Buff_{0}", buffType.ToString())) + " " + string.Format(Localization.Get("Skill_Level"), curr_buff_level + 1); ;
                }
                else
                {
                    lblName.text = Localization.Get(string.Format("Buff_{0}", buffType.ToString()));
                }
            }

            if (lblDesc)
            {
                lblDesc.text = Localization.Get(string.Format("Buff_{0}_Desc", buffType.ToString()));
            }

        }

        public void ShowDesc(bool isShow)
        {
            if (lblDesc)
            {
                lblDesc.gameObject.SetActive(isShow);
            }
        }
    }
}

