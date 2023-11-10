using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HTTPClient : MonoBehaviour
{
    protected string URL = "http://localhost:54910/Game/game.asmx/";
    private static readonly bool ENCRYPT_DATA = true;

    public delegate void OnSendRequestSuccess();

    public const int REQUEST_TIME_OUT = 10;

    private System.Action RetryAction { get; set; }

    public delegate void OnReceiveResponse(string responseData);
    public delegate void OnErrorResponse(string responseErr);

    public bool IsDisconnected
    {
        get
        {
            return Application.internetReachability == NetworkReachability.NotReachable;
        }
    }

    UnityWebRequest GetUnityRequest(string request_url, WWWForm form, int request_time_out)
    {
        UnityWebRequest webRequest = UnityWebRequest.Post(request_url, form);
        webRequest.timeout = request_time_out;
        if (request_url.StartsWith("https://"))
        {
            webRequest.certificateHandler = new HikerCertificateHandler();
            webRequest.disposeCertificateHandlerOnDispose = true;
        }
        //else
        //{
        //    webRequest.certificateHandler = null;
        //    webRequest.disposeCertificateHandlerOnDispose = false;
        //}
        return webRequest;
    }
    IEnumerator CoSendWebRequest(string request_url,
        WWWForm form,
        OnReceiveResponse onResponse,
        OnErrorResponse onFail,
        bool ignoreError,
        int request_time_out)
    {
        using (var webRequest = GetUnityRequest(request_url, form, request_time_out))
        {
            yield return webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                OnRequestFinish(webRequest, onResponse, onFail, ignoreError);
            }
            else if (ignoreError == false)
            {
                if (webRequest.result == UnityWebRequest.Result.ConnectionError)
                {
                    Hiker.GUI.PopupConfirm.Create(Localization.Get("TimeOutDisconnect"), () =>
                    {
                        SendRequestPri(request_url, form, onResponse, onFail, ignoreError, request_time_out + REQUEST_TIME_OUT);
                    },
                    false,
                    Localization.Get("RetryBtn"));
                }
                else
                if (webRequest.result == UnityWebRequest.Result.ProtocolError ||
                    webRequest.result == UnityWebRequest.Result.DataProcessingError)
                {
                    Hiker.GUI.PopupConfirm.Create(webRequest.error, () =>
                    {
                        SendRequestPri(request_url, form, onResponse, onFail, ignoreError);
                    },
                    false,
                    Localization.Get("RetryBtn"),
                    Localization.Get("NetworkError"));
                }
            }
            else
            {
                //if (string.IsNullOrEmpty(webRequest.error) == false)
                //{
                //    Debug.Log(webRequest.error);
                //}
                OnRequestFinish(webRequest, onResponse, onFail, ignoreError);
            }
        }
    }

    void SendRequestPri(string request_url,
        WWWForm form,
        OnReceiveResponse onResponse,
        OnErrorResponse onFail,
        bool ignoreError,
        int request_time_out = 0)
    {
        if (request_time_out <= 0 || request_time_out > 3 * REQUEST_TIME_OUT) request_time_out = REQUEST_TIME_OUT;
        StartCoroutine(CoSendWebRequest(request_url, form, onResponse, onFail, ignoreError, request_time_out));
    }

    public void SendRequest(string funcName,
        string data,
        OnReceiveResponse onResponse = null,
        bool showLoading = true,
        bool ignoreError = false,
        OnErrorResponse onFail = null,
        byte[] binaryData = null,
        int custom_timeOut = 0)
    {
#if DEBUG
        Debug.Log("SendRequest " + funcName);
#endif
#if VIDEO_BUILD
        showLoading = false;
#endif
        this.RetryAction = () => this.SendRequest(funcName, data, onResponse, showLoading, ignoreError, onFail/*, delay*/);

        //bool isInSplash = (SplashScreen.Instance && SplashScreen.Instance.gameObject.activeSelf) &&
        //                 !(PopupDisplayName.Instance && PopupDisplayName.Instance.gameObject.activeSelf);
        //isInSplash = isInSplash || PopupCloudEffect.instance != null;

        if (showLoading)
        {
            if (Hiker.GUI.PopupNetworkLoading.instance != null &&
                Hiker.GUI.PopupNetworkLoading.instance.gameObject.activeSelf)
            {
                //Debug.Log("requesting...");
            }
            else
            {
                Hiker.GUI.PopupNetworkLoading.Create(Localization.Get("NetworkLoading"));
            }
        }

        //if (funcName != "RequestUpdateDisplayName" && TutorialManager.IsShowing)
        //{
        //    showLoading = true;
        //    onFail = null;
        //}

        string request_url = this.URL + "RequestPost";

        string compressed_data = ReadText.CompressString(data, ENCRYPT_DATA);

        WWWForm form = new WWWForm();
        form.AddField(funcName, compressed_data);

        SendRequestPri(request_url, form, onResponse, onFail, ignoreError, custom_timeOut);
        //HTTPRequest request = new HTTPRequest(new Uri(request_url), HTTPMethods.Post, OnRequestFinish);
        //request.FormUsage = HTTPFormUsage.UrlEncoded;
        //string compressed_data = ReadText.CompressString(data, ENCRYPT_DATA);

        //if (showLoading && GameClient.instance.GID != 0 && !funcName.Contains("_"))
        //{
        //    funcName += "_" + GameClient.instance.GID;
        //}
        //request.AddField(funcName, compressed_data);
        //request.SilkRoadResponse = onResponse;
        //if (onFail != null)
        //{
        //    request.SilkRoadResponseWithFailCode = ((e) => onFail((ERROR_CODE)e));
        //}
        //else
        //{
        //    request.SilkRoadResponseWithFailCode = null;
        //}
        //request.IsShowLoadingRequest = showLoading;
        //if (binaryData != null)
        //{
        //    byte[] compressed = ReadText.CompressByteArray(binaryData);
        //    request.AddField("binary", Convert.ToBase64String(compressed));
        //}
        //request.Timeout = new TimeSpan(0, 0, 0, (int)(REQUEST_TIME_OUT));
        //request.Send();
    }

    void OnRequestFinish(UnityWebRequest webRequest,
        OnReceiveResponse onResponse, OnErrorResponse onFail, bool ignoreError)
    {
        Hiker.GUI.PopupNetworkLoading.Dismiss();
        bool isOk = PostProcessResponse(webRequest, ignoreError);

        if (isOk)
        {
            string responseStr;
            try
            {
                responseStr = webRequest.downloadHandler.text;
            }
            catch (System.NotSupportedException e)
            {
                responseStr = Localization.Get("NetworkError");
            }

            if (string.IsNullOrEmpty(responseStr))
            {
                onResponse?.Invoke(string.Empty);
            }
            else if (ReadText.IsBase64String(responseStr) == false)
            {
                if (ignoreError == false)
                {
                    Hiker.GUI.PopupMessage.Create(Hiker.GUI.MessagePopupType.ERROR, Localization.Get("NetworkError"));
                }
                onFail?.Invoke(Localization.Get("NetworkError"));
            }
            else
            {
                string decompressData = ReadText.DecompressString(responseStr, ENCRYPT_DATA);
                onResponse?.Invoke(decompressData);
            }
        }
        else
        {
            onFail?.Invoke(webRequest != null ? webRequest.error : string.Empty);
        }
    }

    bool PostProcessResponse(UnityWebRequest request, bool ignoreError)
    {
        if (request == null)
        {
            Debug.Log("request null");
            return false;
        }
        //var requestState = request.State;
        //if (response == null)
        //{
        //    Debug.Log("respon null");
        //    Debug.Log(request.State);
        //    requestState = HTTPRequestStates.ConnectionTimedOut;
        //}

        //Debug.Log("PostProcessRespons : " + request.State);
        if (request.result == UnityWebRequest.Result.ProtocolError ||
            request.result == UnityWebRequest.Result.DataProcessingError)
        {
            switch(request.responseCode)
            {
                default:
                    break;
            }
            Debug.Log(request.error);
            if (ignoreError == false)
            {
                Hiker.GUI.PopupMessage.Create(Hiker.GUI.MessagePopupType.ERROR, request.error);
            }

            return false;
        }
        else
        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
            if (ignoreError == false)
            {
                Hiker.GUI.PopupMessage.Create(Hiker.GUI.MessagePopupType.ERROR, request.error);
            }

            return false;
        }
        else
        {
            return true;
        }

        //switch (requestState)
        //{
        //    case HTTPRequestStates.Finished:
        //        {
        //            return true;
        //        }
        //    case HTTPRequestStates.TimedOut:
        //    case HTTPRequestStates.ConnectionTimedOut:
        //        {
        //            if (request.IsShowLoadingRequest)
        //            {
        //                PopupConfirm.Create(Localization.Get("TimeOutDisconnect"), this.RetryAction, false, Localization.Get("TryAgain"));

        //                if (GUIManager.Instance && GUIManager.Instance.CurrentScreen == "Battle")
        //                {
        //                    AnalyticsManager.LogEvent("END_BATTLE_TIME_OUT_DISCONNECT");
        //                }

        //                if (PopupCloudEffect.instance != null)
        //                    PopupCloudEffect.instance.CloseEffect();
        //            }

        //            if (request.SilkRoadResponseWithFailCode != null)
        //                request.SilkRoadResponseWithFailCode((int)ERROR_CODE.UNKNOW_ERROR);

        //            return false;
        //        }
        //    case HTTPRequestStates.Error:
        //        {
        //            string msg = Localization.Get("NETWORK_ERROR");

        //            if (request.Exception != null)
        //            {
        //                msg = msg + " : " + request.Exception.Message;
        //            }

        //            if (request.IsShowLoadingRequest)
        //            {
        //                MessagePopup.Create(MessagePopupType.TEXT, msg);

        //                if (PopupCloudEffect.instance != null)
        //                    PopupCloudEffect.instance.CloseEffect();
        //            }

        //            if (request.SilkRoadResponseWithFailCode != null)
        //                request.SilkRoadResponseWithFailCode((int)ERROR_CODE.UNKNOW_ERROR);

        //            return false;
        //        }
        //    default:
        //        {
        //            if (request.IsShowLoadingRequest)
        //            {
        //                MessagePopup.Create(MessagePopupType.TEXT, "Unknown Error - State : " + request.State.ToString());

        //                if (PopupCloudEffect.instance != null)
        //                    PopupCloudEffect.instance.CloseEffect();
        //            }

        //            if (request.SilkRoadResponseWithFailCode != null)
        //                request.SilkRoadResponseWithFailCode((int)ERROR_CODE.UNKNOW_ERROR);

        //            return false;
        //        }
        //}
    }

    //public void RestartWithURL(string url)
    //{
    //    this.URL = url;
    //    PlayerPrefs.SetString("SERVER_URL", this.URL);
    //    //clear cached
    //    PlayerPrefs.DeleteKey("user_" + GameClient.instance.user);
    //    GUIManager.Instance.RestartGame();
    //}
}
