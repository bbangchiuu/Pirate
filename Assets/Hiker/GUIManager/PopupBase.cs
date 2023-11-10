using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Hiker.GUI
{
    public class PopupBase : MonoBehaviour
    {
        public bool isModal = false;
        public bool addToBackStack = true;
        public bool hideIsDestroy = true;
        public UnityEngine.Events.UnityEvent onPopupClosed;

        //private Transform mainContainer;

        //public int AddtionalDepth = 0;

        protected virtual void Awake()
        {
            PopupManager.instance.OnCreatePopup(this);
            //if (useStartDropAnim)
            //{
            //    this.CreateAnimOpen();
            //}
            //if (this.adminGroup != null)
            //{
            //    this.adminGroup.SetActive(GameClient.instance.gameData.IsAdmin());
            //}
        }

        protected virtual void OnEnable()
        {
            //this.isLoadedPanel = false;
            PopupManager.instance.OnShowPopup(this);
            //if (this.mainContainer != null)
            //{
            //    this.mainContainer.localPosition = Vector3.up * 3000;
            //}
            //StartCoroutine(this.HandleAnimOpen());
            //base.OnEnable();
        }

        protected virtual void OnDisable()
        {
            //foreach (UIPanel childPanel in this.panelsInPopup)
            //{
            //    childPanel.depth -= AddtionalDepth;
            //    childPanel.sortingOrder = childPanel.depth;
            //}
            //AddtionalDepth = 0;

            PopupManager.instance.OnHidePopup(this);
            onPopupClosed?.Invoke();

            //if (this.ignoreUpdateRes)
            //{
            //    ScreenTown.Instance.UpdateResourceData();
            //}
        }

        public void MakePopupToTop()
        {
            //AddtionalDepth = 3000;
            //foreach (UIPanel childPanel in this.panelsInPopup)
            //{
            //    childPanel.depth += AddtionalDepth;
            //    childPanel.sortingOrder = childPanel.depth;
            //}
            transform.SetAsLastSibling();
        }

        //public void RefreshDepth()
        //{
        //    this.isLoadedPanel = false;
        //    //if (this.panelsInPopup.Length <= 0) return;

        //    int depth = 0;
        //    //depth = this.panelsInPopup[0].depth;
        //    //foreach (var item in this.panelsInPopup)
        //    //{
        //    //    if (item.depth < depth)
        //    //    {
        //    //        item.depth += depth;
        //    //        item.sortingOrder += depth;
        //    //    }
        //    //}
        //}
        //public void ClearDepth()
        //{
        //    //if (this.panelsInPopup.Length <= 0) return;
        //    //var depth = this.panelsInPopup[0].depth;
        //    //foreach (var item in this.panelsInPopup)
        //    //{
        //    //    item.depth -= depth;
        //    //    item.sortingOrder = item.depth;
        //    //}
        //}
        [GUIDelegate]
        protected virtual void OnCleanUp()
        {
        }

        [GUIDelegate]
        public virtual void OnBackBtnClick()
        {
            if (isModal == false && addToBackStack)
            {
                OnCloseBtnClick();
            }
        }

        [GUIDelegate]
        public virtual void OnCloseBtnClick()
        {
            OnCleanUp();
            Hide();
        }

        protected virtual void Hide()
        {
            //if (SoundManager.instance && this.IsPlayCloseSound)
            //{
            //    SoundManager.instance.CloseButtonClick();
            //}
            if (gameObject)
            {
                if (hideIsDestroy)
                {
                    Destroy(gameObject);
                }
                else
                {
                    // ClearDepth();
                    gameObject.SetActive(false);
                }
            }

            //if (GameClient.instance && GameClient.instance.gameData != null) GameClient.instance.gameData.CheckRewardsResponse();
        }

        //private void CreateAnimOpen()
        //{
        //    if (this.popupAnimation != null
        //        && this.mainContainer != null) return;

        //    GameObject obj = new GameObject();
        //    obj.transform.parent = this.transform;
        //    obj.AddComponent<RectTransform>();
        //    obj.transform.localScale = Vector3.one;
        //    obj.transform.localPosition = Vector3.zero;
        //    obj.name = "MainContainer";

        //    for (int i = this.transform.childCount - 1; i >= 0; i--)
        //    {
        //        this.transform.GetChild(i).SetParent(obj.transform);
        //    }
        //    this.mainContainer = obj.transform;
        //    this.mainContainer.localPosition = Vector3.up * 3000;

        //    this.popupAnimation = this.mainContainer.gameObject.AddComponent<Animation>();
        //    this.popupAnimation.AddClip(PopupManager.popupAnimClip, PopupManager.popupAnimClip.name);
        //}

        //private IEnumerator HandleAnimOpen()
        //{
        //    yield return new WaitForSeconds(Time.deltaTime / 2);
        //    // yield return new WaitForEndOfFrame();

        //    if (this.mainContainer != null)
        //    {
        //        this.mainContainer.gameObject.SetActive(false);
        //        this.mainContainer.gameObject.SetActive(true);
        //        this.mainContainer.localPosition = Vector3.zero;
        //    }

        //    if (this.popupAnimation != null && this.useEffectAnimWhenOpen)
        //    {
        //        string animName = PopupManager.popupAnimClip.name;
        //        this.popupAnimation.Play(animName);
        //    }
        //}

        //public void RefreshDepthParticles(bool reLoad = false)
        //{
        //    if (reLoad) this.isLoadedEffect = false;
        //    int depth = 0;
        //    //int depth = this.panelsInPopup[0].sortingOrder;
        //    foreach (var item in this.EffectsInPopup)
        //    {
        //        item.InitParticles(depth);
        //    }
        //}

        //public virtual void ProcessActiveParticle(bool active)
        //{
        //}
    }
}