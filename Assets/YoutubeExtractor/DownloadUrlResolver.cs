using System.Collections.Generic;
using System.Linq;
//using System.Net;
using System.Text.RegularExpressions;
using Exception = System.Exception;
using ArgumentNullException = System.ArgumentNullException;
using ArgumentException = System.ArgumentException;

namespace YoutubeExtractor
{
    /// <summary>
    /// Provides a method to get the download link of a YouTube video.
    /// </summary>
    public static class DownloadUrlResolver
    {
        private const string RateBypassFlag = "ratebypass";
        private const string SignatureQuery = "signature";

        /// <summary>
        /// Decrypts the signature in the <see cref="VideoInfo.DownloadUrl" /> property and sets it
        /// to the decrypted URL. Use this method, if you have decryptSignature in the <see
        /// cref="GetDownloadUrls" /> method set to false.
        /// </summary>
        /// <param name="videoInfo">The video info which's downlaod URL should be decrypted.</param>
        /// <exception cref="YoutubeParseException">
        /// There was an error while deciphering the signature.
        /// </exception>
        public static void DecryptDownloadUrl(VideoInfo videoInfo)
        {
            //IDictionary<string, string> queries = HttpHelper.ParseQueryString(videoInfo.DownloadUrl);

            //if (queries.ContainsKey(SignatureQuery))
            //{
            //    string encryptedSignature = queries[SignatureQuery];

            //    string decrypted;

            //    try
            //    {
            //        decrypted = GetDecipheredSignature(videoInfo.HtmlPlayerVersion, encryptedSignature);
            //    }

            //    catch (Exception ex)
            //    {
            //        throw new YoutubeParseException("Could not decipher signature", ex);
            //    }

            //    videoInfo.DownloadUrl = HttpHelper.ReplaceQueryStringParameter(videoInfo.DownloadUrl, SignatureQuery, decrypted);
            //    videoInfo.RequiresDecryption = false;
            //}
        }

        /// <summary>
        /// Gets a list of <see cref="VideoInfo" />s for the specified URL.
        /// </summary>
        /// <param name="videoUrl">The URL of the YouTube video.</param>
        /// <param name="decryptSignature">
        /// A value indicating whether the video signatures should be decrypted or not. Decrypting
        /// consists of a HTTP request for each <see cref="VideoInfo" />, so you may want to set
        /// this to false and call <see cref="DecryptDownloadUrl" /> on your selected <see
        /// cref="VideoInfo" /> later.
        /// </param>
        /// <returns>A list of <see cref="VideoInfo" />s that can be used to download the video.</returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="videoUrl" /> parameter is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="videoUrl" /> parameter is not a valid YouTube URL.
        /// </exception>
        /// <exception cref="VideoNotAvailableException">The video is not available.</exception>
        /// <exception cref="WebException">
        /// An error occurred while downloading the YouTube page html.
        /// </exception>
        /// <exception cref="YoutubeParseException">The Youtube page could not be parsed.</exception>
        public static IEnumerable<VideoInfo> GetDownloadUrls(string videoUrl, string pageSource, bool decryptSignature = true)
        {
            try
            {
                var json = LoadJson(pageSource);
                var playerResponse = GetPlayerResponse(json);
                string videoTitle = GetVideoTitle(playerResponse);

                IEnumerable<LitJson.JsonData> formats = ExtractFormats(playerResponse);

                IEnumerable<VideoInfo> infos = GetVideoInfos(formats, videoTitle).ToList();

                return infos;
            }

            catch (Exception ex)
            {
                if (//ex is WebException || 
                    ex is VideoNotAvailableException)
                {
                    throw;
                }

                ThrowYoutubeParseException(ex, videoUrl);
            }

            return null; // Will never happen, but the compiler requires it
        }

        /// <summary>
        /// Normalizes the given YouTube URL to the format http://youtube.com/watch?v={youtube-id}
        /// and returns whether the normalization was successful or not.
        /// </summary>
        /// <param name="url">The YouTube URL to normalize.</param>
        /// <param name="normalizedUrl">The normalized YouTube URL.</param>
        /// <returns>
        /// <c>true</c>, if the normalization was successful; <c>false</c>, if the URL is invalid.
        /// </returns>
        public static bool TryNormalizeYoutubeUrl(string url, out string normalizedUrl)
        {
            url = url.Trim();

            url = url.Replace("youtu.be/", "youtube.com/watch?v=");
            url = url.Replace("www.youtube", "youtube");
            url = url.Replace("youtube.com/embed/", "youtube.com/watch?v=");

            string httpUrl = "http://youtube.com";
            string httpsUrl = "https://youtube.com";

            if (url.Contains("/v/"))
            {
                url = (url.StartsWith("https") ? httpsUrl : httpUrl) + new System.Uri(url).AbsolutePath.Replace("/v/", "/watch?v=");
            }

            url = url.Replace("/watch#", "/watch?");

            IDictionary<string, string> query = HttpHelper.ParseQueryString(url);

            string v;

            if (!query.TryGetValue("v", out v))
            {
                normalizedUrl = null;
                return false;
            }

            normalizedUrl = (url.StartsWith("https") ? "https://youtube.com/watch?v=" : "http://youtube.com/watch?v=") + v;

            return true;
        }

        private static IEnumerable<LitJson.JsonData> ExtractFormats(LitJson.JsonData playerResponse)
        {
            var streamingData = playerResponse["streamingData"];
            var formats = streamingData["formats"];

            for (int i = 0; i < formats.Count; ++i)
            {
                var fmt = formats[i];
                yield return fmt;
            }
            var adaptiveFormats = streamingData["adaptiveFormats"];
            for (int i = 0; i < adaptiveFormats.Count; ++i)
            {
                var fmt = adaptiveFormats[i];
                yield return fmt;
            }
        }

        //private static string GetDecipheredSignature(string htmlPlayerVersion, string signature)
        //{
        //    return Decipherer.DecipherWithVersion(signature, htmlPlayerVersion);
        //}

        private static string GetHtml5PlayerVersion(LitJson.JsonData json)
        {
            var regex = new Regex(@"player-(.+?).js");

            string js = json["assets"]["js"].ToString();

            return regex.Match(js).Result("$1");
        }

        private static string GetStreamMap(LitJson.JsonData json)
        {
            var streamMap = json["args"]["url_encoded_fmt_stream_map"];

            string streamMapString = streamMap == null ? null : streamMap.ToString();

            if (streamMapString == null || streamMapString.Contains("been+removed"))
            {
                throw new VideoNotAvailableException("Video is removed or has an age restriction.");
            }

            return streamMapString;
        }

        private static LitJson.JsonData GetPlayerResponse(LitJson.JsonData json)
        {
            var args = json["args"];
            var jsonStr = args["player_response"].ToString();
            var player_response = LitJson.JsonMapper.ToObject(jsonStr);
            return player_response;
        }

        private static IEnumerable<VideoInfo> GetVideoInfos(IEnumerable<LitJson.JsonData> fmts, string videoTitle)
        {
            var downLoadInfos = new List<VideoInfo>();

            foreach (var fmt in fmts)
            {
                string itag = fmt["itag"].ToString();

                int formatCode = int.Parse(itag);

                VideoInfo info = VideoInfo.Defaults.SingleOrDefault(videoInfo => videoInfo.FormatCode == formatCode);

                if (info != null)
                {
                    info = new VideoInfo(info)
                    {
                        DownloadUrl = fmt["url"].ToString(),
                        Title = videoTitle,
                        RequiresDecryption = false
                    };
                }
                else
                {
                    info = new VideoInfo(formatCode)
                    {
                        DownloadUrl = fmt["url"].ToString(),
                    };
                }

                downLoadInfos.Add(info);
            }

            return downLoadInfos;
        }

        private static string GetVideoTitle(LitJson.JsonData playerResponse)
        {
            var title = string.Empty;
            var videoDetails = playerResponse["videoDetails"];
            if (videoDetails.Contains("title"))
            {
                var titleJson = videoDetails["title"];
                title = titleJson.ToString();
            }

            //return title == null ? string.Empty : title.ToString();
            return title;
        }

        //private static string GetVideoTitle(LitJson.JsonData json)
        //{
        //    var title = string.Empty;
        //    var args = json["args"];
        //    if (args.Contains("title"))
        //    {
        //        var titleJson = args["title"];
        //        title = titleJson.ToString();
        //    }

        //    //return title == null ? string.Empty : title.ToString();
        //    return title;
        //}

        private static bool IsVideoUnavailable(string pageSource)
        {
            const string unavailableContainer = "<div id=\"watch-player-unavailable\">";

            return pageSource.Contains(unavailableContainer);
        }

        private static LitJson.JsonData LoadJson(string pageSource)
        {
            //string pageSource = HttpHelper.DownloadString(url);

            if (IsVideoUnavailable(pageSource))
            {
                throw new VideoNotAvailableException();
            }

            var dataRegex = new Regex(@"ytplayer\.config\s*=\s*(\{.+?\});", RegexOptions.Multiline);

            string extractedJson = dataRegex.Match(pageSource).Result("$1");
            var s = IronSourceJSON.Json.Deserialize(extractedJson) as Dictionary<string, object>;

            return LitJson.JsonMapper.ToObject(extractedJson);
        }

        private static void ThrowYoutubeParseException(Exception innerException, string videoUrl)
        {
            throw new YoutubeParseException("Could not parse the Youtube page for URL " + videoUrl + "\n" +
                                            "This may be due to a change of the Youtube page structure.\n" +
                                            "Please report this bug at www.github.com/flagbug/YoutubeExtractor/issues", innerException);
        }

        private class ExtractionInfo
        {
            public bool RequiresDecryption { get; set; }

            public System.Uri Uri { get; set; }
        }
    }
}