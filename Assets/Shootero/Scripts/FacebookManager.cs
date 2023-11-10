using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if FACEBOOK
using Facebook.Unity;
#endif

[Beebyte.Obfuscator.Skip]
public class FacebookManager : MonoBehaviour
{
#if FACEBOOK
    public static FacebookManager Instance { get; set; }

    void Awake()
    {
        Instance = this;

        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    public void DoLogin()
    {
        //var perms = new List<string>() { "public_profile", "email"};
        //FB.LogInWithReadPermissions(perms, AuthCallback);
    }

    public void Disconnect()
    {
        //if(FB.IsLoggedIn)
        //    FB.LogOut();

        //GameClient.instance.DisconnectFacebook();
    }

    //private void AuthCallback(ILoginResult result)
    //{
    //    if (FB.IsLoggedIn)
    //    {
    //        var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
    //        GameClient.instance.ConnectFacebook(aToken.UserId, aToken.TokenString);
    //    }
    //    else
    //    {
    //        MessagePopup2.Create(MessagePopupType.ERROR, Localization.Get("ConnectFacebookFail"));
    //    }
    //}
#endif
}
