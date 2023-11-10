using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hiker.UI
{
    [RequireComponent(typeof(Image))]
    public class SyncImageWhenEnable : MonoBehaviour
    {
        public Image target;
        Image myImage;
        private void OnEnable()
        {
            if (myImage == null)
            {
                myImage = GetComponent<Image>();
            }

            if (target != null && myImage)
            {
                myImage.sprite = target.sprite;
            }
        }
    }
}

