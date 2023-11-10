using System.Collections;
using System.Collections.Generic;

namespace Hiker.Networks.Data
{
    public enum ERROR_CODE
    {
        OK,
        UNKNOW_ERROR,
        INVALIDATE_ACCESS_TOKEN,
        DISPLAY_MESSAGE,
        DISPLAY_MESSAGE_AND_QUIT,
        CONFIG_VERSION_INVALID,
        GAME_VERSION_INVALID,
        IAP_PACKAGE_NOT_TRUE,
        IAP_TRANSACTION_EXISTED,
        INVALID_REQUEST,
        NOT_FINISHED_BATTLE,
        INVALID_INSTALL,
        //USER_NAME_NOT_EXIST,
        //USER_NAME_EXIST,
        //USER_NAME_INVALID,
        //USER_NAME_CHANGED,
    }

    public class ExDataBase
    {
        public string ErrorMessage { get; set; }
        public ERROR_CODE ErrorCode { get; set; }

        public ExDataBase()
        {
            ErrorCode = ERROR_CODE.OK;
        }
    }

    public class GIDRequest
    {
        public long GID;
        public string token;
        public long rqTime;
        public int ver;
        public string configVer;
        public string platform;
        public string lang;

        /// <summary>
        /// is giay phep ready check
        /// </summary>
        public bool coGP;
        /// <summary>
        /// license response code
        /// </summary>
        public int arg0;
        /// <summary>
        /// license response message
        /// </summary>
        public string arg1;
        /// <summary>
        /// license response signature
        /// </summary>
        public string arg2;
        /// <summary>
        /// Unity application installer name
        /// </summary>
        public string instName;
        /// <summary>
        /// Unity application install mode
        /// </summary>
        public string instMode;
        public string pkgName;

        public void UpdateSignatureData()
        {
#if UNITY_ANDROID
            if (HikerKiemTraGiayPhepUngDung.instance &&
                HikerKiemTraGiayPhepUngDung.instance.Status == 2 &&
                HikerKiemTraGiayPhepUngDung.instance.LoiKetNoiMang == false)
            {
                coGP = true;
                arg0 = HikerKiemTraGiayPhepUngDung.instance.Arg0;
                arg1 = HikerKiemTraGiayPhepUngDung.instance.Arg1;
                arg2 = HikerKiemTraGiayPhepUngDung.instance.Arg2;
            }
            else
            {
                coGP = false;
            }
#endif
#if !SERVER_CODE
            instName = UnityEngine.Application.installerName;
            instMode = UnityEngine.Application.installMode.ToString();
            pkgName = UnityEngine.Application.identifier;
#endif
        }

        public void UpdateGIDData()
        {
#if !SERVER_CODE
            GID = GameClient.instance.GID;
            token = GameClient.instance.AccessToken;
            ver = GameClient.GameVersion;
            configVer = GameClient.instance.configVersion;
            platform = GameClient.instance.platform;
            lang = ConfigManager.language;
            
#else
            // GID = GameRequests.MASTER_CLIENT_ID;
            // token = GameRequests.MASTER_CLIENT_TOKEN;
#endif
            rqTime = TimeUtils.Now.Ticks;

            //UpdateSignatureData();
        }
    }
}
