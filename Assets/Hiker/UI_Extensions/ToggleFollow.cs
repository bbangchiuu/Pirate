using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hiker.UI
{
    [RequireComponent(typeof(Toggle))]
    public class ToggleFollow : MonoBehaviour
    {
        public GameObject follower;
        public float delay = 0f;
        Toggle mToggle;
        private void Awake()
        {
            mToggle = GetComponent<Toggle>();
            mToggle.onValueChanged.AddListener(OnTogleValueChanged);
        }

        void OnTogleValueChanged(bool isActive)
        {
            if (delay <= 0)
            {
                if (isActive)
                {
                    if (follower != null)
                    {
                        var ts = TweenPosition.Begin(follower, 0.15f, transform.position);
                        ts.worldSpace = true;
                    }
                }
            }
            else
            {
                Hiker.HikerUtils.DoAction(this, () =>
                {
                    if (mToggle.isOn)
                    {
                        if (follower != null)
                        {
                            var ts = TweenPosition.Begin(follower, 0.15f, transform.position);
                            ts.worldSpace = true;
                        }
                    }
                }, delay, true);
            }
        }
    }
}

