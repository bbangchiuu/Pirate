using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YoutubeExtractor;
using UnityEngine.Video;
using UnityEngine.Networking;
using System.Linq;

public class TestYoutubeExtractor : MonoBehaviour
{
    public string originURL;
    public VideoPlayer player;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        string videoUrl;
        if (YoutubeExtractor.DownloadUrlResolver.TryNormalizeYoutubeUrl(originURL, out videoUrl) == false)
        {
            videoUrl = originURL;
        }
        if (UrlExtractor.GetVideoID(videoUrl, out string videoID, out bool useHttp))
        {
            List<VideoInfo> videoInfos = new List<VideoInfo>();
            yield return StartCoroutine(UrlExtractor.GetStreamUrls(videoInfos, videoID, useHttp));

            List<VideoInfo> listAvailable = new List<VideoInfo>();
            int choose = -1;
            int maxResolution = 0;
            int minResolution = int.MaxValue;
            foreach (VideoInfo videoInfo in videoInfos)
            {
                if (videoInfo.AdaptiveType == AdaptiveType.None &&
                    videoInfo.VideoType == VideoType.Mp4 &&
                    string.IsNullOrEmpty(videoInfo.DownloadUrl) == false)
                {
                    if (maxResolution < videoInfo.Resolution)
                    {
                        maxResolution = videoInfo.Resolution;
                    }
                    if (minResolution > videoInfo.Resolution)
                    {
                        minResolution = videoInfo.Resolution;
                    }
                    listAvailable.Add(videoInfo);
                }
            }
            listAvailable.Sort((e1, e2) => e2.Resolution - e1.Resolution);
            foreach (var videoInfo in listAvailable)
            {
                player.url = videoInfo.DownloadUrl;
                player.Play();
                Debug.Log("Play video " + videoInfo.Resolution + " url=" + videoInfo.DownloadUrl);
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Run(string url, string pageContent)
    {
        IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(originURL, pageContent);
        List<VideoInfo> listAvailable = new List<VideoInfo>();
        int choose = -1;
        int maxResolution = 0;
        int minResolution = int.MaxValue;
        foreach (VideoInfo videoInfo in videoInfos)
        {
            if (videoInfo.AdaptiveType == AdaptiveType.None &&
                videoInfo.VideoType == VideoType.Mp4 &&
                string.IsNullOrEmpty(videoInfo.DownloadUrl) == false)
            {
                if (maxResolution < videoInfo.Resolution)
                {
                    maxResolution = videoInfo.Resolution;
                }
                if (minResolution > videoInfo.Resolution)
                {
                    minResolution = videoInfo.Resolution;
                }
                listAvailable.Add(videoInfo);
            }
        }
        listAvailable.Sort((e1, e2) => e2.Resolution - e1.Resolution);

        foreach (var videoInfo in listAvailable)
        {
            player.url = videoInfo.DownloadUrl;
            player.Play();
            break;
        }
    }
}
