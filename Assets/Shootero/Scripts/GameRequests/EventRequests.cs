using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Hiker.Networks.Data;
using Hiker.Networks.Data.Shootero;
using Hiker.GUI;
using Hiker.GUI.Shootero;

public partial class GameClient : HTTPClient
{
    public void RequestGetGiangSinhData(bool openPopup)
    {
        GIDRequest req = new GIDRequest();
        req.UpdateGIDData();
        SendRequest("RequestGetGiangSinhData", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            UserInfoResponse response = LitJson.JsonMapper.ToObject<UserInfoResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    UpdateUserInfo(response.UInfo);

                    if (PopupGiangSinh.instance)
                    {
                        PopupGiangSinh.instance.Init(false);
                    }
                    else if (openPopup)
                    {
                        PopupGiangSinh.Create(false);
                    }
                }
                else
                {
                    ProcessErrorResponse(response);
                }
            }
        }, openPopup);
    }

    float lastTimeCheckLicense = 0.5f;
    private void Update()
    {
        ProcessGiangSinhRequest();

#if UNITY_ANDROID
        if (mLicenseChecker) // retry check license after fail by network
        {
            if (mLicenseChecker.Status == 0 || 
                (mLicenseChecker.Status == 2 && mLicenseChecker.LoiKetNoiMang))
            {
                if (lastTimeCheckLicense > 0) lastTimeCheckLicense -= Time.unscaledDeltaTime;
                else
                {
                    lastTimeCheckLicense = 5 * 60f;
                    if (Application.internetReachability != NetworkReachability.NotReachable)
                    {
                        mLicenseChecker.BatDauKiemTra();
                    }
                }
            }
        }
#endif
    }

    void ProcessGiangSinhRequest()
    {
        if (curGiangSinhRequest == null)
        {
            if (ListGiangSinhRequest.Count > 0)
            {
                curGiangSinhRequest = ListGiangSinhRequest.Dequeue();
                var drawReq = curGiangSinhRequest as GiangSinhDrawRequest;
                if (drawReq != null)
                {
                    RequestGiangSinhDraw(drawReq);
                    return;
                }
                var mergeRequest = curGiangSinhRequest as GiangSinhMergeRequest;
                if (mergeRequest != null)
                {
                    RequestGiangSinhMerge(mergeRequest);
                    return;
                }

                //var activateRequest = curGiangSinhRequest as GiangSinhActiveItemRequest;
                //if (activateRequest != null)
                //{
                //    RequestGiangSinhActiveItem(activateRequest);
                //    return;
                //}
            }
        }
    }

    Queue<GiangSinhRequest> ListGiangSinhRequest = new Queue<GiangSinhRequest>();
    GiangSinhRequest curGiangSinhRequest = null;

    public void RequestGiangSinhDraw(int act, string state, int slot, string item, string[] table)
    {
        GiangSinhDrawRequest req = new GiangSinhDrawRequest();
        req.UpdateGIDData();
        req.Act = act;
        req.Slot = slot;
        req.Item = item;
        req.Table = new string[table.Length];
        for (int i = 0; i < table.Length; ++i)
        {
            req.Table[i] = table[i];
        }
        req.State = state;
#if UNITY_EDITOR
        Debug.Log("Draw Act " + act + " table "  + string.Join(",", table));
#endif
        // gen secretKey
        req.Key = "HkSecReqGiangSinh";
        var s = LitJson.JsonMapper.ToJson(req);
        var secretKey = Md5Hash.GetMd5Hash(s);
        req.Key = secretKey;

        ListGiangSinhRequest.Enqueue(req);
    }

    void RequestGiangSinhDraw(GiangSinhDrawRequest req)
    {
        SendRequest("RequestGiangSinhDraw", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            UserInfoResponse response = LitJson.JsonMapper.ToObject<UserInfoResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    UpdateUserInfo(response.UInfo);
                }
                else
                {
                    OnRequestGiangSinhFail(req, response);
                    ProcessErrorResponse(response);
                }
            }
            else
            {
                OnRequestGiangSinhFail(req, response);
            }
            if (curGiangSinhRequest == req)
            {
                curGiangSinhRequest = null;
            }
        }, false);
    }

    public void RequestGiangSinhMerge(int act, string state, int slot1, int slot2, string item, string[] table)
    {
        GiangSinhMergeRequest req = new GiangSinhMergeRequest();
        req.UpdateGIDData();
        req.Act = act;
        req.Slot1 = slot1;
        req.Slot2 = slot2;
        req.Item = item;
        req.Table = new string[table.Length];
        for (int i = 0; i < table.Length; ++i)
        {
            req.Table[i] = table[i];
        }
        req.State = state;

        // gen secretKey
        req.Key = "HkSecReqGiangSinh";
        var s = LitJson.JsonMapper.ToJson(req);
        var secretKey = Md5Hash.GetMd5Hash(s);
        req.Key = secretKey;

        ListGiangSinhRequest.Enqueue(req);
    }

    void RequestGiangSinhMerge(GiangSinhMergeRequest req)
    {
        SendRequest("RequestGiangSinhMerge", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            UserInfoResponse response = LitJson.JsonMapper.ToObject<UserInfoResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    UpdateUserInfo(response.UInfo);
                }
                else
                {
                    OnRequestGiangSinhFail(req, response);
                    ProcessErrorResponse(response);
                }
            }
            else
            {
                OnRequestGiangSinhFail(req, response);
            }

            if (curGiangSinhRequest == req)
            {
                curGiangSinhRequest = null;
            }
        }, false);
    }

    public void RequestGiangSinhActiveItem(int act, string state, int slot, string[] table)
    {
        GiangSinhActiveItemRequest req = new GiangSinhActiveItemRequest();
        req.UpdateGIDData();
        req.Act = act;
        req.Slot = slot;
        req.Table = new string[table.Length];
        for (int i = 0; i < table.Length; ++i)
        {
            req.Table[i] = table[i];
        }
        req.State = state;

        // gen secretKey
        req.Key = "HkSecReqGiangSinh";
        var s = LitJson.JsonMapper.ToJson(req);
        var secretKey = Md5Hash.GetMd5Hash(s);
        req.Key = secretKey;
        StartCoroutine(CoWaitRequestGiangSinhActiveItem(req));
    }

    IEnumerator CoWaitRequestGiangSinhActiveItem(GiangSinhActiveItemRequest req)
    {
        PopupNetworkLoading.Create(Localization.Get("NetworkLoading"), 30);
        while (ListGiangSinhRequest.Count > 0 || curGiangSinhRequest != null)
        {
            yield return null;
        }

        RequestGiangSinhActiveItem(req);
    }

    void RequestGiangSinhActiveItem(GiangSinhActiveItemRequest req)
    {
        SendRequest("RequestGiangSinhActiveItem", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            UserInfoResponse response = LitJson.JsonMapper.ToObject<UserInfoResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    UpdateUserInfo(response.UInfo);
                    if (PopupGiangSinh.instance)
                    {
                        PopupGiangSinh.instance.Init(false);
                    }
                }
                else
                {
                    OnRequestGiangSinhFail(req, response);
                    ProcessErrorResponse(response);
                }
            }
            else
            {
                OnRequestGiangSinhFail(req, response);
            }

            if (curGiangSinhRequest == req)
            {
                curGiangSinhRequest = null;
            }
        }, true);
    }

    void OnRequestGiangSinhFail(GiangSinhRequest req, UserInfoResponse response)
    {
#if UNITY_EDITOR
        Debug.Log("Failed Act " + req.Act + " " + LitJson.JsonMapper.ToJson(req));
#endif
        if (PopupGiangSinh.instance)
        {
            PopupGiangSinh.instance.controller.Freeze(0.5f, false);
        }
        if (response != null)
        {
            if (response.UInfo != null)
            {
                UpdateUserInfo(response.UInfo);

                ListGiangSinhRequest.Clear();

                if (PopupGiangSinh.instance)
                {
                    PopupGiangSinh.instance.Init(false);
                }
                //else
                //{
                //    PopupGiangSinh.Create(false);
                //}
                return;
            }
        }

        if (PopupGiangSinh.instance)
        {
            PopupGiangSinh.instance.OnCloseBtnClick();
            RequestGetGiangSinhData(true);
        }
    }
}
