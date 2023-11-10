using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hiker.GUI
{
    using UnityEngine.UI;

    public class PopupNetworkLoading : PopupBase
    {
        public Image spLoading;
        public Text lbLoading;
        private float _TimeSinceRequest = float.MaxValue;

        public float TimeSinceRequest
        {
            get { return _TimeSinceRequest; }
            set { _TimeSinceRequest = value; }
        }
        static public PopupNetworkLoading instance = null;

        public static void Dismiss()
        {
            if (instance != null)
            {
                instance.Hide();
                instance = null;
            }
        }

        public static void Create(string str, float time = 20f)
        {
            if (instance == null)
            {
                GameObject obj = PopupManager.instance.GetPopup("PopupNetworkLoading", false, Vector3.zero);
                instance = obj.GetComponent<PopupNetworkLoading>();
            }

            instance.lbLoading.text = str;
            instance.TimeSinceRequest = time;
            instance.gameObject.SetActive(true);
        }

        float dTime = 0.2f;
        public void Update()
        {
            dTime -= Time.unscaledDeltaTime;
            if (dTime <= 0)
            {
                spLoading.transform.Rotate(new Vector3(0, 0, -360 / 12));
                dTime = Random.Range(0.1f, 0.25f);
            }
            //base.Update();
        }
    }
}