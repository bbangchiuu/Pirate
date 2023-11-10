using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hiker.GUI
{
    public class PopupManager : MonoBehaviour
    {
        static PopupManager _instance = null;
        public static PopupManager instance
        {
            get
            {
                if (_instance == null)
                    _instance = GameObject.Find("PopupContainer").GetComponent<PopupManager>();
                return _instance;
            }
        }

        //private static AnimationClip _popupAnimClip { get; set; }
        //public static AnimationClip popupAnimClip
        //{
        //    get
        //    {
        //        if (_popupAnimClip == null)
        //        {
        //            _popupAnimClip = Resources.Load("FX/GUI_FX_popupOpen_anim") as AnimationClip;
        //        }
        //        return _popupAnimClip;
        //    }
        //}

        [System.NonSerialized]
        public List<PopupBase> activePopups = new List<PopupBase>();
        Stack<PopupBase> backStackPopups = new Stack<PopupBase>();

        private static Vector3 POPUP_DEFAULT_POSITION = Vector3.zero;
        private static Dictionary<string, GameObject> popupsPref = new Dictionary<string, GameObject>();
        private static Dictionary<string, GameObject> popupsCache = new Dictionary<string, GameObject>();


        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }

            this.ClearAll();
        }

        public void ClearAll()
        {
            for (int i = 0; i < this.transform.childCount; i++)
            {
                if (this.transform.GetChild(i) != null)
                {
                    Destroy(this.transform.GetChild(i).gameObject);
                }
            }

            this.activePopups.Clear();
            //popupsCache.Clear();
            //hideUnitPopupCount = 0;
        }

        public bool HaveBackStackPopup()
        {
            return backStackPopups.Count > 0;
        }

        public void OnBackBtnClick()
        {
            if (HaveBackStackPopup())
            {
                var popup = backStackPopups.Peek();
                popup.OnBackBtnClick();
            }
        }

        public void OnShowPopup(PopupBase popup)
        {
            //if (popup == null) return;
            //int depth = 100 + maxPopupDepth * 10;
            //popup.depthConst = maxPopupDepth;

            //foreach (UIPanel childPanel in popup.panelsInPopup)
            //{
            //    childPanel.depth += depth;
            //    childPanel.sortingOrder = childPanel.depth;
            //}

            //popup.RefreshDepthParticles(false);

            //if (popup.addMaskBackground && this.activePopups.Count > 0 &&
            //    this.activePopups[this.activePopups.Count - 1].backSprite != null)
            //{
            //    var top_popup = this.activePopups[this.activePopups.Count - 1];
            //    top_popup.backSprite.SetActive(false);
            //    top_popup.ProcessActiveParticle(false);
            //}

            //if (popup.backSprite != null)
            //{
            //    popup.backSprite.SetActive(true);
            //    if (popup.tapToClose)
            //    {
            //        popup.backSprite.GetComponent<UIEventListener>().onClick = (go) =>
            //        {
            //            popup.SendMessage("OnCloseBtnClick");
            //        };
            //    }
            //    else
            //    {
            //        popup.backSprite.GetComponent<UIEventListener>().onClick = null;
            //    }
            //}

            //if (popup.hasTopMenu) GUIManager.Instance.UpdateTopMenu(depth + 5, depth + 5);
            if (this.activePopups.Contains(popup) == false) this.activePopups.Add(popup);
            if (popup.addToBackStack) backStackPopups.Push(popup);
            //if (popup.hideTownUnits)
            //{
            //    UpdateHideUnitPopup(1);
            //}

            //if (ScreenBattle.Instance)
            //{
            //    ScreenBattle.Instance.ProcessActiveParticle(false);
            //}
        }

        public void OnHidePopup(PopupBase popup)
        {
            if (popup == null) return;
            int index = this.activePopups.FindIndex(v => v == popup);
            //if (index <= 0 && GUIManager.Instance != null)
            //{
            //    GUIManager.Instance.UpdateTopMenu(1, 1);
            //}
            //else if (popup.hasTopMenu)
            //{
            //    PopupBase before = this.activePopups[index - 1];
            //    int minDepth = before.panelsInPopup[0].depth;
            //    if (!before.hasTopMenu) GUIManager.Instance.UpdateTopMenu(minDepth - 1, minDepth - 1);
            //}

            //if (index >= 0)
            //{
            //    int depth = 100 + popup.depthConst * 10;
            //    UIPanel[] panels = popup.panelsInPopup;

            //    foreach (UIPanel childPanel in panels)
            //    {
            //        if (childPanel == null) continue;
            //        childPanel.depth -= depth;
            //        childPanel.sortingOrder = childPanel.depth;
            //    }
            //}

            this.activePopups.RemoveAt(index);

            if (backStackPopups.Contains(popup))
            {
                var tempList = Hiker.Util.ListPool<PopupBase>.Claim();
                var peek = backStackPopups.Count > 0 ? backStackPopups.Peek() : null;
                while (backStackPopups.Count > 0 && peek != popup)
                {
                    tempList.Add(backStackPopups.Pop());
                    peek = backStackPopups.Count > 0 ? backStackPopups.Peek() : null;
                }

                if (peek == popup)
                {
                    backStackPopups.Pop();
                }

                for (int i = tempList.Count - 1; i >= 0; --i)
                {
                    backStackPopups.Push(tempList[i]);
                }
                Hiker.Util.ListPool<PopupBase>.Release(tempList);
            }

            //if (popup.hideTownUnits)
            //{
            //    UpdateHideUnitPopup(-1);
            //}

            //if (this.activePopups.Count > 0)
            //{
            //    var top_popup = this.activePopups[this.activePopups.Count - 1];
            //    if (top_popup.addMaskBackground) top_popup.backSprite.SetActive(true);
            //    top_popup.ProcessActiveParticle(true);
            //}
            ////else if (TutorialManager.IsShowing == false && TutorialManager.Instance && 
            ////    GUIManager.Instance.CurrentScreen == "Town" && ScreenTown.Instance && ScreenTown.Instance.holdingPopups.Count == 0)
            ////{
            ////    TutorialManager.Instance.ProcessShowTutorialByType(false, true, popup);
            ////}

            ////if (ScreenBattle.Instance)
            ////{
            ////    ScreenBattle.Instance.ProcessActiveParticle(this.activePopups.Count == 0);
            ////}
        }

        public void OnCreatePopup(PopupBase popup)
        {
            if (popup == null) return;
            //if (popup.GetComponent<UIPanel>() == null)
            //{
            //    UIPanel panel = popup.gameObject.AddComponent<UIPanel>();
            //    panel.depth = 0;
            //    panel.sortingOrder = 0;
            //}

            //if (popup.addMaskBackground)
            //{
            //    GameObject obj = Instantiate(blackSpritePref) as GameObject;
            //    obj.transform.parent = popup.transform;
            //    obj.transform.localScale = Vector3.one;
            //    obj.transform.localPosition = Vector3.zero;
            //    popup.backSprite = obj;
            //}

            //if (!popup.hideIsDestroy)
            //{
            //    popupsCache[popup.gameObject.name] = popup.gameObject;
            //}
        }

        private GameObject GetPopupPref(string popupName)
        {
            GameObject pref = null;
            if (!popupsPref.ContainsKey(popupName))
            {
                pref = Resources.Load("Popups/" + popupName) as GameObject;

                //if (pref == null)
                //    pref = ResourcesManager.instance ? ResourcesManager.instance.GetPopupPrefab(popupName).gameObject : null;

                popupsPref[popupName] = pref;
            }
            else
            {
                pref = popupsPref[popupName];
                if (pref == null)
                {
                    popupsPref.Remove(popupName);
                    this.GetPopupPref(popupName);
                }
            }
            return pref;
        }

        public GameObject GetPopup(string popupName, bool useDefaultPos = true, Vector3 pos = default(Vector3), bool isMultiInstance = false)
        {
            GameObject obj = null;
            string name = popupName + "(Clone)";
            if (!popupsCache.ContainsKey(name) || isMultiInstance)
            {
                GameObject pref = this.GetPopupPref(popupName);
                obj = Instantiate(pref, transform);
            }
            else
            {
                obj = popupsCache[name];
                if (obj == null)
                {
                    popupsCache.Remove(name);
                    this.GetPopup(popupName, useDefaultPos, pos);
                }
            }
            obj.SetActive(true);
            if (useDefaultPos) pos = POPUP_DEFAULT_POSITION;
            if (this.activePopups.Count > 1)
            {
                //pos.z = this.activePopups[this.activePopups.Count - 2].transform.localPosition.z - 700;
            }
            obj.transform.localPosition = pos;


            return obj;
        }
    }
}