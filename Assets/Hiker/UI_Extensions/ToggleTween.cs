using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hiker.UI
{
    [RequireComponent(typeof(Toggle))]
    public class ToggleTween : MonoBehaviour
    {
        public UITweener[] tweeners;
        Toggle mToggle;
        private void Awake()
        {
            mToggle = GetComponent<Toggle>();
            mToggle.onValueChanged.AddListener(OnTogleValueChanged);
        }

        void OnTogleValueChanged(bool isActive)
        {
            foreach (var t in tweeners)
            {
                if (t)
                {
                    if (isActive)
                    {
                        t.PlayForward();
                    }
                    else
                    {
                        t.PlayReverse();
                    }
                }
            }
        }
    }
}

