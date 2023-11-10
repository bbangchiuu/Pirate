using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace YoutubeExtractor
{

    /// <summary>
    /// duongrs
    /// viet lai phan code tu Android Youtube Extractor
    /// https://github.com/HaarigerHarald/android-youtubeExtractor
    /// NOTE: chua ho tro decipher voi cac video dang co dung cipher signature
    /// /// </summary>
    public static class UrlExtractor
    {
        private const string USER_AGENT = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/40.0.2214.115 Safari/537.36";

        const string patCipher = "\"signatureCipher\"\\s*:\\s*\"(.+?)\"";
        const string patUrl = "\"url\"\\s*:\\s*\"(.+?)\"";
        const string patCipherUrl = "url=(.+?)(\\\\\\\\u0026|\\z)";
        const string patEncSig = "s=(.{10,}?)(\\\\\\\\u0026|\\z)";
        const string patItag = "itag=([0-9]+?)(&|\\z)";
        const string patStatusOk = "status=ok(&|,|\\z)";

        public static IEnumerator GetStreamUrls(List<VideoInfo> videoUrls, string videoID, bool useHttp)
        {
            string ytInfoUrl = (useHttp) ? "http://" : "https://";
            ytInfoUrl += "www.youtube.com/get_video_info?video_id=" + videoID + "&eurl="
                        + UnityEngine.Networking.UnityWebRequest.EscapeURL("https://youtube.googleapis.com/v/" + videoID);
#if DEBUG
            Debug.Log("infoUrl: " + ytInfoUrl);
#endif
            var req = UnityWebRequest.Get(ytInfoUrl);
            req.SetRequestHeader("User-Agent", USER_AGENT);
            yield return req.SendWebRequest();
            string streamMap = string.Empty;
            try
            {
                if (req.isHttpError == false && req.isNetworkError == false)
                {
                    streamMap = DownloadHandlerBuffer.GetContent(req);
                }
                else
                {
                    Debug.LogError(req.error);
                    yield break;
                }
            }
            catch(System.Exception e)
            {
                Debug.LogError(e.Message);
                yield break;
            }
            streamMap = UnityWebRequest.UnEscapeURL(streamMap, System.Text.Encoding.UTF8);
            streamMap = streamMap.Replace("\\u0026", "&");
#if DEBUG
            Debug.Log("STREAMMAP");
            Debug.Log(streamMap);
#endif

            bool sigEnc = true;
            bool statusFail = false;

            var m = System.Text.RegularExpressions.Regex.Match(streamMap, patCipher);

            if (m.Success == false)
            {
                sigEnc = false;
                m = System.Text.RegularExpressions.Regex.Match(streamMap, patUrl);
                if (System.Text.RegularExpressions.Regex.Match(streamMap, patStatusOk).Success == false)
                {
                    statusFail = true;
                }
            }

            if (sigEnc || statusFail)
            {
                Debug.LogError("Chua ho tro sigEnc");
                //yield return StartCoroutine(GetSigJS(videoID));
            }

            for (; m.Success; m = m.NextMatch())
            {
                string sig = string.Empty;
                string url;

                var cipher = m.Groups[1].Value;

                if (sigEnc)
                {
#if DEBUG
                    Debug.Log("CIPHER");
                    Debug.Log(cipher);
#endif
                    var m2 = System.Text.RegularExpressions.Regex.Match(cipher, patCipherUrl);
                    if (m2.Success)
                    {
                        url = UnityWebRequest.UnEscapeURL(m2.Groups[1].Value, System.Text.Encoding.UTF8);
#if DEBUG
                        Debug.Log("URL");
                        Debug.Log(url);
#endif
                        m2 = System.Text.RegularExpressions.Regex.Match(cipher, patEncSig);
                        if (m2.Success)
                        {
                            sig = UnityWebRequest.UnEscapeURL(m2.Groups[1].Value, System.Text.Encoding.UTF8);
                            // fix issue #165
                            sig = sig.Replace("\\u0026", "&");
                            sig = sig.Split('&')[0];
#if DEBUG
                            Debug.Log("SIG");
                            Debug.Log(sig);
#endif
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    url = cipher;
#if DEBUG
                    Debug.Log("URL");
                    Debug.Log(url);
#endif
                }

                var mat2 = System.Text.RegularExpressions.Regex.Match(url, patItag);
                if (mat2.Success == false)
                {
                    continue;
                }

                var itag = System.Int32.Parse(mat2.Groups[1].Value);

                VideoInfo videoInfo = null;
                foreach (var info in VideoInfo.Defaults)
                {
                    if (info.FormatCode == itag)
                    {
                        videoInfo = info;
                    }
                }

                if (videoInfo == null)
                {
                    Debug.Log("Itag not in list:" + itag);
                    continue;
                }

                var v = new VideoInfo(videoInfo);
                v.DownloadUrl = url;
                videoUrls.Add(v);
            }

            #region Origin Java Code
            // origin Java code
            //urlConnection.setRequestProperty();
            //        try {
            //            reader = new BufferedReader(new InputStreamReader(urlConnection.getInputStream()));
            //            streamMap = reader.readLine();

            //        } finally
            //{
            //    if (reader != null)
            //        reader.close();
            //    urlConnection.disconnect();
            //}
            //Matcher mat;
            //String curJsFileName;
            //SparseArray<String> encSignatures = null;

            //streamMap = URLDecoder.decode(streamMap, "UTF-8");
            //streamMap = streamMap.replace("\\u0026", "&");

            //parseVideoMeta(streamMap);

            //if (videoMeta.isLiveStream())
            //{
            //    mat = patHlsvp.matcher(streamMap);
            //    if (mat.find())
            //    {
            //        String hlsvp = URLDecoder.decode(mat.group(1), "UTF-8");
            //        SparseArray<YtFile> ytFiles = new SparseArray<>();

            //        getUrl = new URL(hlsvp);
            //        urlConnection = (HttpURLConnection)getUrl.openConnection();
            //        urlConnection.setRequestProperty("User-Agent", USER_AGENT);
            //        try
            //        {
            //            reader = new BufferedReader(new InputStreamReader(urlConnection.getInputStream()));
            //            String line;
            //            while ((line = reader.readLine()) != null)
            //            {
            //                if (line.startsWith("https://") || line.startsWith("http://"))
            //                {
            //                    mat = patHlsItag.matcher(line);
            //                    if (mat.find())
            //                    {
            //                        int itag = Integer.parseInt(mat.group(1));
            //                        YtFile newFile = new YtFile(FORMAT_MAP.get(itag), line);
            //                        ytFiles.put(itag, newFile);
            //                    }
            //                }
            //            }
            //        }
            //        finally
            //        {
            //            reader.close();
            //            urlConnection.disconnect();
            //        }

            //        if (ytFiles.size() == 0)
            //        {
            //            if (LOGGING)
            //                Log.d(LOG_TAG, streamMap);
            //            return null;
            //        }
            //        return ytFiles;
            //    }
            //    return null;
            //}

            //// "use_cipher_signature" disappeared, we check whether at least one ciphered signature
            //// exists int the stream_map.
            //boolean sigEnc = true, statusFail = false;
            //if (!patCipher.matcher(streamMap).find())
            //{
            //    sigEnc = false;
            //    if (!patStatusOk.matcher(streamMap).find())
            //    {
            //        statusFail = true;
            //    }
            //}

            //// Some videos are using a ciphered signature we need to get the
            //// deciphering js-file from the youtubepage.
            //if (sigEnc || statusFail)
            //{
            //    // Get the video directly from the youtubepage
            //    if (CACHING
            //            && (decipherJsFileName == null || decipherFunctions == null || decipherFunctionName == null))
            //    {
            //        readDecipherFunctFromCache();
            //    }
            //    if (LOGGING)
            //        Log.d(LOG_TAG, "Get from youtube page");

            //    getUrl = new URL("https://youtube.com/watch?v=" + videoID);
            //    urlConnection = (HttpURLConnection)getUrl.openConnection();
            //    urlConnection.setRequestProperty("User-Agent", USER_AGENT);
            //    try
            //    {
            //        reader = new BufferedReader(new InputStreamReader(urlConnection.getInputStream()));
            //        String line;
            //        StringBuilder sbStreamMap = new StringBuilder();
            //        while ((line = reader.readLine()) != null)
            //        {
            //            // Log.d("line", line);
            //            sbStreamMap.append(line.replace("\\\"", "\""));
            //        }
            //        streamMap = sbStreamMap.toString();
            //    }
            //    finally
            //    {
            //        reader.close();
            //        urlConnection.disconnect();
            //    }
            //    encSignatures = new SparseArray<>();

            //    mat = patDecryptionJsFile.matcher(streamMap);
            //    if (!mat.find())
            //        mat = patDecryptionJsFileWithoutSlash.matcher(streamMap);
            //    if (mat.find())
            //    {
            //        curJsFileName = mat.group(0).replace("\\/", "/");
            //        if (decipherJsFileName == null || !decipherJsFileName.equals(curJsFileName))
            //        {
            //            decipherFunctions = null;
            //            decipherFunctionName = null;
            //        }
            //        decipherJsFileName = curJsFileName;
            //    }
            //}

            //SparseArray<YtFile> ytFiles = new SparseArray<>();

            //if (sigEnc)
            //{
            //    mat = patCipher.matcher(streamMap);
            //}
            //else
            //{
            //    mat = patUrl.matcher(streamMap);
            //}

            //while (mat.find())
            //{
            //    String sig = null;
            //    String url;
            //    if (sigEnc)
            //    {
            //        String cipher = mat.group(1);
            //        Matcher mat2 = patCipherUrl.matcher(cipher);
            //        if (mat2.find())
            //        {
            //            url = URLDecoder.decode(mat2.group(1), "UTF-8");
            //            mat2 = patEncSig.matcher(cipher);
            //            if (mat2.find())
            //            {
            //                sig = URLDecoder.decode(mat2.group(1), "UTF-8");
            //                // fix issue #165
            //                sig = sig.replace("\\u0026", "&");
            //                sig = sig.split("&")[0];
            //            }
            //            else
            //            {
            //                continue;
            //            }
            //        }
            //        else
            //        {
            //            continue;
            //        }
            //    }
            //    else
            //    {
            //        url = mat.group(1);
            //    }

            //    Matcher mat2 = patItag.matcher(url);
            //    if (!mat2.find())
            //        continue;

            //    int itag = Integer.parseInt(mat2.group(1));

            //    if (FORMAT_MAP.get(itag) == null)
            //    {
            //        if (LOGGING)
            //            Log.d(LOG_TAG, "Itag not in list:" + itag);
            //        continue;
            //    }
            //    else if (!includeWebM && FORMAT_MAP.get(itag).getExt().equals("webm"))
            //    {
            //        continue;
            //    }

            //    // Unsupported
            //    if (url.contains("&source=yt_otf&"))
            //        continue;

            //    if (LOGGING)
            //        Log.d(LOG_TAG, "Itag found:" + itag);

            //    if (sig != null)
            //    {
            //        encSignatures.append(itag, sig);
            //    }

            //    Format format = FORMAT_MAP.get(itag);
            //    YtFile newVideo = new YtFile(format, url);
            //    ytFiles.put(itag, newVideo);
            //}

            //if (encSignatures != null)
            //{

            //    if (LOGGING)
            //        Log.d(LOG_TAG, "Decipher signatures: " + encSignatures.size() + ", videos: " + ytFiles.size());
            //    String signature;
            //    decipheredSignature = null;
            //    if (decipherSignature(encSignatures))
            //    {
            //        lock.lock () ;
            //        try
            //        {
            //            jsExecuting.await(7, TimeUnit.SECONDS);
            //        }
            //        finally
            //        {
            //            lock.unlock();
            //        }
            //    }
            //    signature = decipheredSignature;
            //    if (signature == null)
            //    {
            //        return null;
            //    }
            //    else
            //    {
            //        String[] sigs = signature.split("\n");
            //        for (int i = 0; i < encSignatures.size() && i < sigs.length; i++)
            //        {
            //            int key = encSignatures.keyAt(i);
            //            String url = ytFiles.get(key).getUrl();
            //            url += "&sig=" + sigs[i];
            //            YtFile newFile = new YtFile(FORMAT_MAP.get(key), url);
            //            ytFiles.put(key, newFile);
            //        }
            //    }
            //}

            //if (ytFiles.size() == 0)
            //{
            //    if (LOGGING)
            //        Log.d(LOG_TAG, streamMap);
            //    return null;
            //}
            //return ytFiles;
            #endregion
        }

        public static bool GetVideoID(string ytUrl, out string videoID, out bool useHttp)
        {
            var match = System.Text.RegularExpressions.Regex.Match(ytUrl,
                @"^(http|https)://(www\.|m\.|)youtube\.com/watch\?v=(.+?)$");
            if (match.Success)
            {
                videoID = match.Groups[3].Value;
                useHttp = match.Groups[1].Value == "http";
                return true;
            }
            else
            {
                videoID = string.Empty;
                useHttp = false;
                return false;
            }
        }

        private static IEnumerator GetSigJS(string videoID)
        {
            var getUrl = "https://youtube.com/watch?v=" + videoID;
            UnityWebRequest getReq = UnityWebRequest.Get(getUrl);
            getReq.SetRequestHeader("User-Agent", USER_AGENT);
            yield return getReq.SendWebRequest();
            var buffer = DownloadHandlerBuffer.GetContent(getReq);
            var lines = buffer.Split('\n', '\r');
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (var l in lines)
            {
                if (string.IsNullOrWhiteSpace(l) == false)
                {
                    sb.Append(l.Replace("\\\"", "\""));
                }
            }
            string streamMap = sb.ToString();
            string patDecryptionJsFile = "\\\\/s\\\\/player\\\\/([^\"]+?)\\.js";
            string patDecryptionJsFileWithoutSlash = "/s/player/([^\"]+?).js";
            string patSignatureDecFunction = "(?:\\b|[^a-zA-Z0-9$])([a-zA-Z0-9$]{2})\\s*=\\s*function\\(\\s*a\\s*\\)\\s*\\{\\s*a\\s*=\\s*a\\.split\\(\\s*\"\"\\s*\\)";

            var mat = System.Text.RegularExpressions.Regex.Match(streamMap, patDecryptionJsFile);
            if (mat.Success == false)
            {
                mat = System.Text.RegularExpressions.Regex.Match(streamMap, patDecryptionJsFileWithoutSlash);
            }
            if (mat.Success)
            {
                var curJsFileName = mat.Groups[0].Value.Replace("\\/", "/");
                Debug.Log("jsFileName " + curJsFileName);
            }
        }
    }

}