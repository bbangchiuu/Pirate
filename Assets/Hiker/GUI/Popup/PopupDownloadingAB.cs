using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Hiker.GUI
{
    using UnityEngine.UI;

    public class PopupDownloadingAB : PopupBase
    {
        public static PopupDownloadingAB instance;
        public Slider downloadingABProgressBar;
        public Text downloadABText, percentDownloadingLabel;
        private System.Action callBack = null;

        public static void Create(System.Action callBack = null)
        {
            if (instance == null)
            {
                GameObject gObj = PopupManager.instance.GetPopup("PopupDownloadingAB");
                instance = gObj.GetComponent<PopupDownloadingAB>();
            }

            instance.callBack = callBack;
        }


        void Update()
        {
            float progress = 0;
            //if (AssetBundleLoader.m_DownloadingAB.Count > 0 && AssetBundleManager.IsNeedToShowDownloadingGUI)
            //{
            //    //            Debug.Log("Downloading bundle :" + AssetBundleManager.CurrentABDownloading(out progress));
            //    if (downloadingABProgressBar.gameObject.activeSelf == false)
            //    {
            //        downloadingABProgressBar.gameObject.SetActive(true);
            //    }

            //    downloadABText.text = string.Format(Localization.Get("DownloadingAB"), AssetBundleManager.CurrentABDownloading(out progress));
            //    percentDownloadingLabel.text = (AssetBundleManager.SizeOfABDownloading * progress).ToString("0.##") + " MB";
            //    downloadingABProgressBar.value = progress;
            //}
            
        }

        protected override void OnCleanUp()
        {
            if (callBack != null)
            {
                callBack();
            }
        }
    }
}