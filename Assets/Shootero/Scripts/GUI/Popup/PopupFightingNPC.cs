using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hiker.GUI.Shootero;
using UnityEngine.UI;

namespace Hiker.GUI.Shootero
{
    public class PopupFightingNPC : PopupBase
    {
        FightingNPC visual;

        public Text TitleLabel;
        public Text DescLabel;

        public static PopupFightingNPC instance;

        public static PopupFightingNPC Create(FightingNPC visual, string NPCName)
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupFightingNPC");
            instance = go.GetComponent<PopupFightingNPC>();
            instance.Init();
            instance.visual = visual;

            instance.TitleLabel.text = Localization.Get("Popup" +NPCName + "Title");
            instance.DescLabel.text = Localization.Get("Popup" + NPCName + "Message");

            return instance;
        }

        private void Init()
        {
        }

        protected override void OnCleanUp()
        {
            if (visual)
            {
                visual.SpawnBots();
            }
        }
    }

}

