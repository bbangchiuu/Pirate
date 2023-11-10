using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hiker.GUI
{
    public class GUIDelegateAttribute : System.Attribute {}

    public class GUIManager : MonoBehaviour
    {
        public static GUIManager Instance { get { if (instance == null) instance = FindObjectOfType<GUIManager>(); return instance; } }
        static GUIManager instance = null;

        public Transform ScreenContainer;

        public string LastScreen { get; set; }
        public string CurrentScreen { get; set; }
        [System.NonSerialized]
        public Dictionary<string, ScreenBase> screens = new Dictionary<string, ScreenBase>();

        public GraphicRaycaster graphicRaycasterPopupContainer;
        public EventSystem m_EventSystem;
        public Camera screenSpaceCamera;

        ScreenBase LoadScreen(string screen_name)
        {
            GameObject go = null;

            if (go == null)
            {
                go = Instantiate(Resources.Load<GameObject>("Screens/Screen" + screen_name), ScreenContainer);
            }

            if (go == null)
            {
                return null;
            }

            go.transform.localScale = Vector3.one;
            //go.SetActive(false);

            ScreenBase screen_base = go.GetComponent<ScreenBase>();

            if (screen_base == null)
            {
                return null;
            }

            if (screens.ContainsKey(screen_name))
            {
                screens[screen_name] = screen_base;
            }
            else
            {
                screens.Add(screen_name, screen_base);
            }

            return screen_base;
        }

        void Awake()
        {
            //this.ProcessResolution();

            if (instance != null)
            {
                GameObject.Destroy(gameObject);
                return;
            }
            else
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }

            LoadSoundAndMusicSetting();

            this.ClearScreen();

            ////if (GameManager.Instance)
            ////{
            ////    for (int i = 0; i < GameManager.Instance.transform.childCount; i++)
            ////    {
            ////        var child = GameManager.Instance.transform.GetChild(i);
            ////        if (child.name.Contains("Camera")) continue;
            ////        child.gameObject.SetActive(false);
            ////    }
            ////}

            //this.ViewportHeight = Screen.height * this.ViewportWidth / Screen.width;
            ////ConfigManager.InitLanguage();
            //var need_request_permissions = false;

            //if (!need_request_permissions)
            //{
            //    this.ProcessStartGame();
            //}
        }

        void OnGoToBattle()
        {
            AudioListener camera_listener = Camera.main.GetComponent<AudioListener>();
            if (camera_listener != null)
                camera_listener.enabled = false;

            if (QuanlyNguoichoi.Instance && QuanlyNguoichoi.Instance.PlayerUnit)
            {
                var playerListener = QuanlyNguoichoi.Instance.PlayerUnit.GetComponent<AudioListener>();
                if (playerListener && playerListener.enabled)
                {
                    playerListener.enabled = true;
                }
            }

            //if(SoundEnable==false)
            //{
            //    AudioListener.volume = 0f;
            //}
            AudioListener.volume = SoundEnable ? 1f : 0f;
        }

        void OnGoToMainScreen()
        {
            //AudioListener.volume = 1f;
            AudioListener.volume = SoundEnable ? 1f : 0f;

            if (GameClient.instance.UInfo.Gamer != null && GameClient.instance.UInfo.Gamer.TheLuc.Val < 5)
            {
                if (UnityEngine.PlayerPrefs.GetInt("FirstTimeShowAdsThisSession", 0) < 1
                    && UnityEngine.PlayerPrefs.GetInt("FirstTimeShowAdsThisSessionStartBattle", 0) > 0)
                {
                    UnityEngine.PlayerPrefs.SetInt("FirstTimeShowAdsThisSession", 1);
                    UnityEngine.PlayerPrefs.SetInt("FirstTimeShowAdsThisSessionStartBattle", 0);
                    GUI.PopupMissingTheLuc.Create();
                }
            }

            if (PlayerPrefs.GetInt("ShowTruongThanhOffer_" + GameClient.instance.UInfo.GID, 0) == 0
                && Shootero.ScreenMain.instance && Shootero.ScreenMain.instance.grpChapter
                && GameClient.instance.UInfo.GetCurrentChapter() >= ConfigManager.TruongThanhCfg.ChapterUnlock
                    && Shootero.ScreenMain.instance.grpChapter.btnTruongThanhOffer.gameObject.activeSelf)
            {
                PlayerPrefs.SetInt("ShowTruongThanhOffer_" + GameClient.instance.UInfo.GID, 1);
                if (GameClient.instance.UInfo.Gamer.TruongThanhData.purchased == false)
                    Shootero.PopupTruongThanhOffer.Create();
            }

            if (QuanlyNguoichoi.Instance && QuanlyNguoichoi.Instance.PlayerUnit)
            {
                var playerListener = QuanlyNguoichoi.Instance.PlayerUnit.GetComponent<AudioListener>();
                if (playerListener && playerListener.enabled)
                {
                    playerListener.enabled = false;
                }
            }

            AudioListener camera_listener = Camera.main.GetComponent<AudioListener>();
            if (camera_listener != null)
                camera_listener.enabled = true;
        }

        public void ClearScreen()
        {
            //UserInfoGadget.instance = null;
            //ResourceGadget.instance = null;

            ScreenContainer.DestroyChildren();
            this.screens.Clear();
        }

        public ScreenBase SetScreen(string screen_name, System.Action onScreenActive = null)
        {
            if(screen_name=="Battle")
            {
                OnGoToBattle();
            }

            if (screen_name == "Main")
            {
                OnGoToMainScreen();
            }

            //if (this.ViewportEffect == null)
            //{
            //    this.ViewportEffect = GameObject.Instantiate(Resources.Load("UI/ViewportEffect")) as GameObject;
            //    this.ViewportEffect.transform.parent = this.transform;
            //    this.ViewportEffect.transform.localScale = Vector3.one;
            //    this.ViewportEffect.transform.localPosition = Vector3.zero;
            //}

            if (screens.ContainsKey(screen_name) == false)
            {
                ScreenBase screen = LoadScreen(screen_name);
            }

            if (CurrentScreen == screen_name)
            {
                var screen = this.screens[CurrentScreen];
                screen.OnActive();
                if (onScreenActive != null) onScreenActive();

                return screen;
            }

            Debug.Log("SetScreen :" + screen_name);
            LastScreen = CurrentScreen;

            ScreenBase curScreen = null;

            if (string.IsNullOrEmpty(CurrentScreen) == false &&
                screens.ContainsKey(CurrentScreen))
            {
                curScreen = screens[CurrentScreen];
                curScreen.OnDeactive();
                curScreen.gameObject.SetActive(false);
            }

            curScreen = screens[screen_name];
            CurrentScreen = screen_name;
            //UIPanel panel = (screens[screen_name].transform.parent).GetComponent<UIPanel>();
            if (!curScreen.gameObject.activeSelf)
                curScreen.gameObject.SetActive(true);

            curScreen.OnActive();

            if (onScreenActive != null) onScreenActive();

            return curScreen;
        }

        public void RestartGame()
        {

        }

        //public static void OnLocalize()
        //{
        //    if (Instance != null)
        //    {
        //        var l = Instance.GetComponentsInChildren<UILocalize>();
        //        foreach (var lo in l)
        //        {
        //            lo.OnLocalize();
        //        }
        //    }

        //    //UIRoot.Broadcast("OnLocalize");
        //}

        private void Start()
        {
            //GUIManager.Instance.SetScreen("Loading");
        }

        public AudioSource musicSource;
        public bool MusicEnable = true;
        public bool SoundEnable = true;

        public void LoadSoundAndMusicSetting()
        {
            MusicEnable = (PlayerPrefs.GetInt("MusicEnable", 1) == 1);
            SoundEnable = (PlayerPrefs.GetInt("SoundEnable", 1) == 1);
        }

        public void SaveSoundAndMusicSetting()
        {
            if (MusicEnable)
                PlayerPrefs.SetInt("MusicEnable",1);
            else
                PlayerPrefs.SetInt("MusicEnable",0);

            if (SoundEnable)
                PlayerPrefs.SetInt("SoundEnable", 1);
            else
                PlayerPrefs.SetInt("SoundEnable", 0);

        }

        public void MuteAudio()
        {
            AudioListener.volume = 0;
        }

        public void UnmuteAudio()
        {
            AudioListener.volume = SoundEnable ? 1f : 0f;
        }

        public void ApplySoundAndMusicSetting()
        {
            if (MusicEnable == false)
            {
                musicSource.Stop();
                musicSource.volume = 0;
            }
            else if (GUIManager.instance.CurrentScreen == "Main")
            {
                StartFadeInMusic();
            }

            AudioListener.volume = SoundEnable ? 1f : 0f;
        }

        public void StartFadeInMusic()
        {
            if (MusicEnable)
            {
                musicSource.volume = 0f;
                this.StopCoroutine("FadeOut");
                this.StopCoroutine("FadeIn");
                this.StartCoroutine("FadeIn", 2f);
            }
        }
        public void StartFadeOutMusic()
        {
            if (MusicEnable)
            {
                this.StopCoroutine("FadeOut");
                this.StopCoroutine("FadeIn");
                this.StartCoroutine("FadeOut", 1f);
            }
        }



        public IEnumerator FadeOut(float FadeTime)
        {
            AudioSource audioSource = musicSource;

            float startVolume = audioSource.volume;
            float downVolumnInFrame = startVolume * Time.deltaTime / FadeTime;
            while (audioSource.volume > 0)
            {
                audioSource.volume -= downVolumnInFrame;
                yield return null;
            }
            audioSource.Stop();
            audioSource.volume = 0;
            //Destroy(audioSource);
        }

        public IEnumerator FadeIn(float FadeTime)
        {
            float maxVolumn = 0.6f;
            AudioSource audioSource = musicSource;

            float upVolumnInFrame = maxVolumn * Time.deltaTime / FadeTime;

            if (audioSource.isPlaying == false)
                audioSource.Play();

            while (audioSource.volume < maxVolumn)
            {
                audioSource.volume += upVolumnInFrame;
                yield return null;
            }
            audioSource.volume = maxVolumn;
        }

        public void PlaySound(string path, Vector3 pos, float vol)
        {
            AudioClip clip = Resources.Load(path) as AudioClip;

            if (clip != null)
            {
                NGUITools.PlaySound(clip);
            }
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                if (PopupManager.instance.HaveBackStackPopup())
                {
                    PopupManager.instance.OnBackBtnClick();
                }
                else
                {
                    if (screens[CurrentScreen].OnBackBtnClick() == false)
                    {
                        if (CurrentScreen == "Main")
                        {
                            PopupConfirm.Create(Localization.Get("quit_confirm_msg"), () =>
                            {
                                Application.Quit();
                            }, true, string.Empty, Localization.Get("quit_confirm_title"));
                        }
                        else
                        if (CurrentScreen != "Main" && CurrentScreen != LastScreen)
                        {
                            SetScreen(LastScreen);
                        }
                    }
                }
            }
        }

        public bool IsTouchHover(Vector3 mousePos, GraphicRaycaster rayCaster, int hoverLayer)
        {
            var m_PointerEventData = new PointerEventData(m_EventSystem);
            m_PointerEventData.position = mousePos;

            List<RaycastResult> results = Hiker.Util.ListPool<RaycastResult>.Claim();
            rayCaster.Raycast(m_PointerEventData, results);
            var re = false;
            for (int i = 0; i < results.Count; ++i)
            {
                var result = results[i];
                if (result.gameObject.layer == hoverLayer)
                {
                    re = true;
                    break;
                }
            }
            Hiker.Util.ListPool<RaycastResult>.Release(results);
            return re;
        }

        public bool IsTouchItem(Vector3 mousePos)
        {
            return IsTouchHover(mousePos, graphicRaycasterPopupContainer, LayerMan.itemLayer);
        }
    }

}

