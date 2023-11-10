using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;
    using UnityEngine.UI;
    using UnityEngine.Video;
    using UnityEngine.Networking;

    public class PopupPlayVideo : PopupBase
    {
        public VideoPlayer videoPlayer;
        public Image spLoading;
        
        public RectTransform videoFrame;
        public RectTransform windowFrame;
        public RectTransform fullFrame;

        bool isLoading = false;
        RenderTexture videoTexture;
        bool fullscreen = true;
        bool resourceBar = false;

        public static PopupPlayVideo instance;
        public static PopupPlayVideo Create(string[] urls, bool fullscreen)
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupPlayVideo");
            instance = go.GetComponent<PopupPlayVideo>();
            instance.fullscreen = fullscreen;
            instance.Init(urls);
            return instance;
        }

        public void Init(string[] urls)
        {
            windowFrame.gameObject.SetActive(fullscreen == false);
            resourceBar = ResourceBar.instance.gameObject.activeSelf;
            ResourceBar.instance.gameObject.SetActive(fullscreen == false);
            fullFrame.gameObject.SetActive(fullscreen);
            if (fullscreen)
            {
                videoFrame.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (float)Screen.width);
                videoFrame.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (float)Screen.height);
            }

            SetAlpha(0, videoFrame.gameObject);
            videoPlayer.errorReceived += VideoPlayer_errorReceived;
            videoPlayer.loopPointReached += VideoPlayer_loopPointReached;
            StartCoroutine(LoadingVideo(urls));
        }

        bool isVideoError = false;

        private void VideoPlayer_errorReceived(VideoPlayer source, string message)
        {
            Debug.Log(message);
            isVideoError = true;
        }

        void InitVideoFrame()
        {
            var canvas = videoFrame.GetComponent<Graphic>().canvas.GetComponent<RectTransform>();
            int width = fullscreen ? Mathf.RoundToInt(canvas.rect.width) : (int)videoPlayer.width;
            int height = fullscreen ? Mathf.RoundToInt(canvas.rect.height) : (int)videoPlayer.height;
            videoFrame.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            videoFrame.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            if (videoTexture != null)
            {
                videoTexture.Release();
            }

            Debug.LogFormat("{0}x{1}", videoPlayer.width, videoPlayer.height);

            videoTexture = new RenderTexture((int)width, (int)height, 16,
                RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
            //videoTexture.width = (int)videoPlayer.width;
            //videoTexture.height = (int)videoPlayer.height;
            videoTexture.Create();
            videoPlayer.targetTexture = videoTexture;
            var videoFrameImg = videoFrame.GetComponent<RawImage>();
            videoFrameImg.texture = videoTexture;
        }

        public static void SetAlpha(float alpha, GameObject go)
        {
            if (go)
            {
                var graphic = go.GetComponent<Graphic>();
                SetAlpha(alpha, graphic);
            }
        }

        public static void SetAlpha(float alpha, Graphic a)
        {
            var c = a.color;
            c.a = alpha;
            a.color = c;
        }

        IEnumerator LoadingVideo(string[] urls)
        {
            isLoading = true;
            bool haveUrl = false;

            foreach (var u in urls)
            {
                if (string.IsNullOrEmpty(u)) continue;
                
                string videoUrl;
                string streamUrl = string.Empty;
                bool isYoutubeUrl = YoutubeExtractor.DownloadUrlResolver.TryNormalizeYoutubeUrl(u, out videoUrl);
                if (isYoutubeUrl)
                {
                    List<YoutubeExtractor.VideoInfo> listStreams = new List<YoutubeExtractor.VideoInfo>();
                    yield return StartCoroutine(CoLoadingYoutubeUrl(videoUrl, listStreams));
                    if (listStreams.Count > 0)
                    {
                        streamUrl = GetUrlYoutube(listStreams, 1080);
                    }
                }
                else
                {
                    streamUrl = u;
                }

                if (string.IsNullOrEmpty(streamUrl) == false)
                {
#if DEBUG
                    Debug.Log("Play video " + streamUrl);
#endif
                    isVideoError = false;
                    videoPlayer.url = streamUrl;
                    videoPlayer.waitForFirstFrame = true;
                    videoPlayer.Prepare();
                    float time = 10;
                    while (videoPlayer.isPrepared == false && time > 0 && isVideoError == false)
                    {
                        yield return null;
                        time -= Time.unscaledDeltaTime;
                    }
                    if (time <= 0 || isVideoError)
                    {
                        videoPlayer.Stop();
                    }
                    else
                    if (videoPlayer.isPrepared)
                    {
                        haveUrl = true;

                        isLoading = false;
                        InitVideoFrame();
                        TweenAlpha.Begin(videoFrame.gameObject, 0.1f, 1f);
                        videoPlayer.frame = 0;
                        videoPlayer.loopPointReached += VideoPlayer_loopPointReached;
                        videoPlayer.Play();
                    }
                }

                yield return null;
                if (haveUrl)
                    break;
            }
        }

        IEnumerator CoLoadingYoutubeUrl(string videoUrl, List<YoutubeExtractor.VideoInfo> videoInfos)
        {
            videoInfos.Clear();
            if (videoUrl == null)
                yield break;

            bool isYoutubeUrl = YoutubeExtractor.DownloadUrlResolver.TryNormalizeYoutubeUrl(videoUrl, out videoUrl);

            if (!isYoutubeUrl)
            {
                yield break;
                //throw new ArgumentException("URL is not a valid youtube URL!");
            }

            if (YoutubeExtractor.UrlExtractor.GetVideoID(videoUrl, out string videoID, out bool useHttp))
            {
                yield return StartCoroutine(YoutubeExtractor.UrlExtractor.GetStreamUrls(videoInfos, videoID, useHttp));
            }
        }

        string GetUrlYoutube(List<YoutubeExtractor.VideoInfo> streamUrls, int preferResolution = 720)
        {
            List<YoutubeExtractor.VideoInfo> listAvailable = new List<YoutubeExtractor.VideoInfo>();
            foreach (var videoInfo in streamUrls)
            {
                if (videoInfo.AdaptiveType == YoutubeExtractor.AdaptiveType.None &&
                    videoInfo.VideoType == YoutubeExtractor.VideoType.Mp4 &&
                    string.IsNullOrEmpty(videoInfo.DownloadUrl) == false)
                {
                    listAvailable.Add(videoInfo);
                }
            }
            //listAvailable.Sort((e1, e2) => e2.Resolution - e1.Resolution);
            int nearestResolution = int.MaxValue;
            int nearestIdx = -1;
            for (int i = 0; i < listAvailable.Count; ++i)
            {
                var dif = Mathf.Abs(listAvailable[i].Resolution - preferResolution);
                if (nearestResolution > dif)
                {
                    nearestIdx = i;
                    nearestResolution = dif;
                }
            }

            return listAvailable[nearestIdx].DownloadUrl;
        }

        //[GUIDelegate]
        //public void OnBtnVideoClick()
        //{
        //    videoPlayer.gameObject.SetActive(true);
        //    //videoPlayer.transform.localScale = Vector3.one * 0.01f;
        //    videoFrame.GetComponent<RawImage>().color = new Color(1, 1, 1, 0);

        //    //var ts = TweenScale.Begin(videoPlayer.gameObject, 0.15f, Vector3.one);
        //    var ts = TweenAlpha.Begin(videoPlayer.gameObject, 0.15f, 1f);

        //    videoPlayer.url = Application.streamingAssetsPath + "/AH_APP_1080x1920.mp4";

        //    videoPlayer.frame = 0;
        //    videoPlayer.Play();
        //    ts.SetOnFinished(() => {
        //        //videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
        //        //if (videoPlayer.targetCamera == null)
        //        //    videoPlayer.targetCamera = GUIManager.Instance.transform.Find("Camera").GetComponent<Camera>();
        //    });
        //}

        protected override void Hide()
        {
            if (fullscreen)
            {
                ResourceBar.instance.gameObject.SetActive(resourceBar);
            }
            
            base.Hide();
        }

        [GUIDelegate]
        public void OnCloseVideoClick()
        {
            videoPlayer.Stop();
            //var ts = TweenScale.Begin(videoPlayer.gameObject, 0.15f, Vector3.zero);
            var ts = TweenAlpha.Begin(videoFrame.gameObject, 0.15f, 0f);
            ts.SetOnFinished(() => { videoPlayer.gameObject.SetActive(false); OnCloseBtnClick(); });
        }

        private void VideoPlayer_loopPointReached(VideoPlayer source)
        {
            OnCloseVideoClick();
        }

        protected override void OnCleanUp()
        {
            if (videoTexture != null)
            {
                videoTexture.Release();
            }
            base.OnCleanUp();
        }

        float dTime = 0.2f;
        void Update()
        {
            if (isLoading)
            {
                if (spLoading.gameObject.activeSelf == false)
                    spLoading.gameObject.SetActive(true);
                dTime -= Time.unscaledDeltaTime;
                if (dTime <= 0)
                {
                    spLoading.transform.Rotate(new Vector3(0, 0, -360 / 12));
                    dTime = Random.Range(0.1f, 0.25f);
                }
            }
            else if (spLoading.gameObject.activeSelf)
            {
                spLoading.gameObject.SetActive(false);
            }
        }
    }
}