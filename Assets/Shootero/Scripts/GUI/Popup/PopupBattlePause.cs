using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Hiker.Networks.Data.Shootero;

namespace Hiker.GUI.Shootero
{
    using UnityEngine.UI;

    public class PopupBattlePause : PopupBase
    {
        public BuffIcon skillIconPrefab;
        public RectTransform skillParent;
        public Text lbSkill;
        public RectTransform layoutGrp;
        public GroupSettings grpSetting;

        public static PopupBattlePause instance;

        public static PopupBattlePause Create()
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupBattlePause");
            instance = go.GetComponent<PopupBattlePause>();
            instance.Init();

            return instance;
        }

        private void Init()
        {
            ScreenBattle.PauseGame(true);

            var listBuffs = QuanlyNguoichoi.Instance.ListBuffs;

            List<BuffType> listBuffTypes = new List<BuffType>();

            if (listBuffs != null && listBuffs.Count > 0)
            {
                lbSkill.gameObject.SetActive(true);
                foreach (var buff in listBuffs)
                {
                    if (listBuffTypes.Contains(buff.Type) == false)
                    {
                        listBuffTypes.Add(buff.Type);
                    }
                }

                foreach (var buff in listBuffTypes)
                {
                    var icon = Instantiate(skillIconPrefab, skillParent);
                    icon.gameObject.SetActive(true);
                    icon.SetBuffType(buff);
                    icon.lblName.text = string.Format(Localization.Get("ShortLevelLabel"),
                        QuanlyNguoichoi.Instance.PlayerUnit.GetBuffCount(buff));
                }
            }
            else
            {
                lbSkill.gameObject.SetActive(false);
            }

            Hiker.HikerUtils.DoAction(this, () =>
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(skillParent);
                LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGrp);
            }, 0.1f, true);

            if (grpSetting)
            {
                if (grpSetting.ShowHUDToggle)
                {
                    if (QuanlyNguoichoi.Instance && QuanlyNguoichoi.Instance.IsLeoThapMode)
                    {
                        grpSetting.ShowHUDToggle.gameObject.SetActive(true);
                    }
                    else
                    {
                        grpSetting.ShowHUDToggle.gameObject.SetActive(false);
                    }
                }
                if (grpSetting.SkillButtonToggle)
                {
                    if (ScreenBattle.instance.btnActiveSkillleft.gameObject.activeInHierarchy ||
                        ScreenBattle.instance.btnActiveSkillright.gameObject.activeInHierarchy)
                    {
                        grpSetting.SkillButtonToggle.gameObject.SetActive(true);
                    }
                    else
                    {
                        grpSetting.SkillButtonToggle.gameObject.SetActive(false);
                    }
                }
            }
        }

        [GUIDelegate]
        public void OnBtnQuit()
        {
            PopupConfirm.Create(Localization.Get("quit_confirm_msg"), () =>
            {
                QuanlyNguoichoi.Instance.OnPlayerRetreat();
                OnCloseBtnClick();
            }, true);
        }

        protected override void OnCleanUp()
        {
            ScreenBattle.PauseGame(false);
#if UNITY_ANDROID || UNITY_IOS
            Hiker.QualityRenderSetting.instance.SetTargetFPS();
#endif
        }
    }

}

