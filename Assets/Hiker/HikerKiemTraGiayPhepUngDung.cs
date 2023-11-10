using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System;

#if ANTICHEAT
using Int64 = CodeStage.AntiCheat.ObscuredTypes.ObscuredLong;
using Int32 = CodeStage.AntiCheat.ObscuredTypes.ObscuredInt;
using Float = CodeStage.AntiCheat.ObscuredTypes.ObscuredFloat;
using Bool = CodeStage.AntiCheat.ObscuredTypes.ObscuredBool;
using ObString = CodeStage.AntiCheat.ObscuredTypes.ObscuredString;
#else
using Int64 = System.Int64;
using Int32 = System.Int32;
using Float = System.Single;
using Bool = System.Boolean;
using ObString = System.String;
#endif

public class HikerKiemTraGiayPhepUngDung : MonoBehaviour
{
	/*
	 * This is the Java service binder classes.jar
	 */
	public TextAsset ServiceBinder;

	/*
		 * Use the public LVL key from the Android Market publishing section here.
		 */
	private ObString m_PublicKey_Base64;
	/*
	 * Consider storing the public key as RSAParameters.Modulus/.Exponent rather than Base64 to prevent the ASN1 parsing..
	 * These are printed to the logcat below.
	 */
	private ObString m_PublicKey_Modulus_Base64 = "<Set to output from SimpleParseASN1>";
	private ObString m_PublicKey_Exponent_Base64 = "< .. and here >";

    private void Awake()
    {
		instance = this;
    }


	[Beebyte.Obfuscator.ObfuscateLiterals]
	void Start()
	{
#if DEBUG
		m_PublicKey_Base64 =
			"MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAlCclnZwwS67QUA5NvKFra+F6eYmuP6+u2z/Q/BM0xhdjOGOLK/DzdcqDYZ+hHjULhYO2NmCdiFEVFq3CB4c3K407LZpo+rQO9JY6T30XQZNkuPqcvp5dOr1WT5xERM+fnAPtAX5z6hPCAay8z+AkQBhO/4yFeZfd+un/tAMn8TAHStHRmNbgXgiMlAgoxWgJxoebF0AKvbASKoY7yyvHGhK0sDKONS21C9GKj1uXrhguWNY/4YS/IWcYQzIhMu0vu+Ud3kbH4MFM9NZBiJFQnytSB8GICPCn4GiiGf7SE/5Xwuz5ta2P0KaJO1iwXDEW70Xz0lCHena9eSc3jSeEfQIDAQAB";

		m_PublicKey_Modulus_Base64 = 
			"AJQnJZ2cMEuu0FAOTbyha2vhenmJrj+vrts/0PwTNMYXYzhjiyvw83XKg2GfoR41C4WDtjZgnYhRFRatwgeHNyuNOy2aaPq0DvSWOk99F0GTZLj6nL6eXTq9Vk+cRETPn5wD7QF+c+oTwgGsvM/gJEAYTv+MhXmX3frp/7QDJ/EwB0rR0ZjW4F4IjJQIKMVoCcaHmxdACr2wEiqGO8srxxoStLAyjjUttQvRio9bl64YLljWP+GEvyFnGEMyITLtL7vlHd5Gx+DBTPTWQYiRUJ8rUgfBiAjwp+Boohn+0hP+V8Ls+bWtj9CmiTtYsFwxFu9F89JQh3p2vXknN40nhH0=";
		m_PublicKey_Exponent_Base64 =
			"AQAB";
		//		// Either parse the ASN1-formatted public LVL key at runtime (only available when stripping is disabled)..
		//		RSA.SimpleParseASN1(m_PublicKey_Base64, ref m_PublicKey.Modulus, ref m_PublicKey.Exponent);

		//		m_PublicKey_Modulus_Base64 = System.Convert.ToBase64String(m_PublicKey.Modulus);
		//		m_PublicKey_Exponent_Base64 = System.Convert.ToBase64String(m_PublicKey.Exponent);
		//#if DEBUG
		//		// .. and check the logcat for these values ...
		//		Debug.Log("private string m_PublicKey_Modulus_Base64 = \"" + m_PublicKey_Modulus_Base64 + "\";");
		//		Debug.Log("private string m_PublicKey_Exponent_Base64 = \"" + m_PublicKey_Exponent_Base64 + "\";");
		//#endif
		// .. or use pre-parsed keys (and remove the code above).
		m_PublicKey.Modulus = System.Convert.FromBase64String(m_PublicKey_Modulus_Base64);
		m_PublicKey.Exponent = System.Convert.FromBase64String(m_PublicKey_Exponent_Base64);

#endif
#if UNITY_EDITOR
		// duongrs test signature
		Hiker.HikerUtils.DoAction(this, () =>
		{
			TestSignature(0,
				"0|1758612620|com.hikergames.ArcadeHunter|440|ANlOHQMzOrv3W7bzuxyWYHG8FquDNWBINw==|1608017780717:VT=9223372036854775807",
				"IRvXniFUPT30gfiWVVHAa7vMnJJCJypJmZIxsenRAHttIH+mgAJ+Y+KIMQJ/OnbaBNoiuRvCCbgCQN78Aj+5VEZ1yOJYf4141OZy0rD8pUkg+1Xfu9XzzlPyAXfwzSZfd4SZr3f5tfU7GGEGbqMt4gM7lO915IP0weAjlZYGtwx2QKFB/aupP5E7JDlI1Ptti31S+Io/SgGvAjNL3qgBvCGurw8OcpotaH1TS6M+MOLh+F4ZPM2AV0hkJK5sNVTXHI8BfK5+RnnllfygYuatBk8LvhLuKZK5zyrshBHoS/LR3FVkTf763GiURmwYOpfBp3dnyhw1F/sXy2aFXxLEJQ==");
		}, 0.5f);
#endif

		m_RunningOnAndroid = new AndroidJavaClass("android.os.Build").GetRawClass() != System.IntPtr.Zero;
		if (!m_RunningOnAndroid)
			return;

		LoadServiceBinder();

		new SHA1CryptoServiceProvider();    // keep a dummy reference to prevent too aggressive stripping

		m_ButtonMessage = "Check LVL";

		
	}

	private RSAParameters m_PublicKey = new RSAParameters();

	/*
	 *
	 */
	[Beebyte.Obfuscator.ObfuscateLiterals]
	private void LoadServiceBinder()
	{
		byte[] classes_jar = ServiceBinder.bytes;

		m_Activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
		m_PackageName = m_Activity.Call<string>("getPackageName");

		string cachePath = System.IO.Path.Combine(m_Activity.Call<AndroidJavaObject>("getCacheDir").Call<string>("getPath"), m_PackageName);
		System.IO.Directory.CreateDirectory(cachePath);

		System.IO.File.WriteAllBytes(cachePath + "/classes.jar", classes_jar);
		System.IO.Directory.CreateDirectory(cachePath + "/odex");

		AndroidJavaObject dcl = new AndroidJavaObject("dalvik.system.DexClassLoader",
													  cachePath + "/classes.jar",
													  cachePath + "/odex",
													  null,
													  m_Activity.Call<AndroidJavaObject>("getClassLoader"));
        try
		{
			m_LVLCheckType = dcl.Call<AndroidJavaObject>("findClass", "com.unity3d.plugin.lvl.ServiceBinder");
			System.IO.Directory.Delete(cachePath, true);
        }
		catch (System.IO.IOException e)
        {
			Debug.LogWarning(e.Message);
        }
		catch (AndroidJavaException e2)
        {
			Debug.LogWarning(e2.Message);
		}
	}

	private bool m_RunningOnAndroid = false;

	private AndroidJavaObject m_Activity;
	private AndroidJavaObject m_LVLCheckType;

	private AndroidJavaObject m_LVLCheck = null;

	private string m_ButtonMessage = "Invalid LVL key!\nCheck the source...";
	private bool m_ButtonEnabled = true;

	private string m_PackageName;
	private int m_Nonce;

	private bool m_LVL_Received = false;
	private string m_ResponseCode_Received;
	private string m_PackageName_Received;
	private int m_Nonce_Received;
	private int m_VersionCode_Received;
	private string m_UserID_Received;
	private string m_Timestamp_Received;
	private int m_MaxRetry_Received;
	private string m_LicenceValidityTimestamp_Received;
	private string m_GracePeriodTimestamp_Received;
	private string m_UpdateTimestamp_Received;
	private string m_FileURL1_Received = "";
	private string m_FileURL2_Received = "";
	private string m_FileName1_Received;
	private string m_FileName2_Received;
	private int m_FileSize1_Received;
	private int m_FileSize2_Received;
	private string m_LicensingURL_Received = "";

	public static HikerKiemTraGiayPhepUngDung instance;

	/// <summary>
	/// 0 is start
	/// 1 is requesting
	/// 2 is received
	/// </summary>
	public int Status { get; private set; }

	public bool LoiKetNoiMang { get { return Arg0 == ERROR_CONTACTING_SERVER; } }
	Int32 mArg0;
	public int Arg0 { get { return mArg0; } }
	public ObString Arg1 { get; private set; }
	public ObString Arg2 { get; private set; }

	public long CurTimeStamp { get; private set; }

	//void OnGUI()
	//{
	//	if (!m_RunningOnAndroid)
	//	{
	//		GUI.Label(new Rect(10, 10, Screen.width - 10, 20), "Use LVL checks only on the Android device!");
	//		return;
	//	}
	//	GUI.enabled = m_ButtonEnabled;
	//	if (GUI.Button(new Rect(10, 10, 450, 300), m_ButtonMessage))
	//	{
	//		m_ButtonMessage = "Checking...";
	//		m_ButtonEnabled = false;

	//		m_Nonce = new System.Random().Next();

	//		object[] param = new object[] { new AndroidJavaObject[] { m_Activity } };
	//		AndroidJavaObject[] ctors = m_LVLCheckType.Call<AndroidJavaObject[]>("getConstructors");
	//		m_LVLCheck = ctors[0].Call<AndroidJavaObject>("newInstance", param);
	//		m_LVLCheck.Call("create", m_Nonce, new AndroidJavaRunnable(Process));
	//	}
	//	GUI.enabled = true;

	//	if (m_LVLCheck != null || m_LVL_Received)
	//	{
	//		GUI.Label(new Rect(10, 320, 450, 20), "Requesting LVL response:");
	//		GUI.Label(new Rect(20, 340, 450, 20), "Package name  = " + m_PackageName);
	//		GUI.Label(new Rect(20, 360, 450, 20), "Request nonce = 0x" + m_Nonce.ToString("X"));
	//	}

	//	if (m_LVLCheck == null && m_LVL_Received)
	//	{
	//		GUI.Label(new Rect(10, 420, 450, 20), "Received LVL response:");
	//		GUI.Label(new Rect(20, 440, 450, 20), "Response code  = " + m_ResponseCode_Received);
	//		GUI.Label(new Rect(20, 460, 450, 20), "Package name   = " + m_PackageName_Received);
	//		GUI.Label(new Rect(20, 480, 450, 20), "Received nonce = 0x" + m_Nonce_Received.ToString("X"));
	//		GUI.Label(new Rect(20, 500, 450, 20), "Version code = " + m_VersionCode_Received);
	//		GUI.Label(new Rect(20, 520, 450, 20), "User ID   = " + m_UserID_Received);
	//		GUI.Label(new Rect(20, 540, 450, 20), "Timestamp = " + m_Timestamp_Received);
	//		GUI.Label(new Rect(20, 560, 450, 20), "Max Retry = " + m_MaxRetry_Received);
	//		GUI.Label(new Rect(20, 580, 450, 20), "License Validity = " + m_LicenceValidityTimestamp_Received);
	//		GUI.Label(new Rect(20, 600, 450, 20), "Grace Period = " + m_GracePeriodTimestamp_Received);
	//		GUI.Label(new Rect(20, 620, 450, 20), "Update Since = " + m_UpdateTimestamp_Received);
	//		GUI.Label(new Rect(20, 640, 450, 20), "Main OBB URL = " + m_FileURL1_Received.Substring(0,
	//														Mathf.Min(m_FileURL1_Received.Length, 50)) + "...");
	//		GUI.Label(new Rect(20, 660, 450, 20), "Main OBB Name = " + m_FileName1_Received);
	//		GUI.Label(new Rect(20, 680, 450, 20), "Main OBB Size = " + m_FileSize1_Received);
	//		GUI.Label(new Rect(20, 700, 450, 20), "Patch OBB URL = " + m_FileURL2_Received.Substring(0,
	//														Mathf.Min(m_FileURL2_Received.Length, 50)) + "...");
	//		GUI.Label(new Rect(20, 720, 450, 20), "Patch OBB Name = " + m_FileName2_Received);
	//		GUI.Label(new Rect(20, 740, 450, 20), "Patch OBB Size = " + m_FileSize2_Received);
	//		GUI.Label(new Rect(20, 760, 450, 20), "Licensing URL = " + m_LicensingURL_Received.Substring(0,
	//														Mathf.Min(m_LicensingURL_Received.Length, 50)) + "...");
	//	}
	//}

	[Beebyte.Obfuscator.ObfuscateLiterals]
	public void BatDauKiemTra()
    {
		if (m_RunningOnAndroid == false) return;

		m_ButtonMessage = "Checking...";
        m_ButtonEnabled = false;

        m_Nonce = new System.Random().Next();

        object[] param = new object[] { new AndroidJavaObject[] { m_Activity } };
        AndroidJavaObject[] ctors = m_LVLCheckType.Call<AndroidJavaObject[]>("getConstructors");
        m_LVLCheck = ctors[0].Call<AndroidJavaObject>("newInstance", param);
        m_LVLCheck.Call("create", m_Nonce, new AndroidJavaRunnable(Process));
    }

    private void Update()
    {
		if (m_RunningOnAndroid == false) return;

        if (m_LVLCheck != null || m_LVL_Received)
        {
			// requesting
			Status = 1;
        }
		if (m_LVLCheck == null && m_LVL_Received)
        {
			Status = 2;
		}
    }

    internal static Dictionary<string, string> DecodeExtras(string query)
	{
		Dictionary<string, string> result = new Dictionary<string, string>();

		if (query.Length == 0)
			return result;

		string decoded = query;
		int decodedLength = decoded.Length;
		int namePos = 0;
		bool first = true;

		while (namePos <= decodedLength)
		{
			int valuePos = -1, valueEnd = -1;
			for (int q = namePos; q < decodedLength; q++)
			{
				if (valuePos == -1 && decoded[q] == '=')
				{
					valuePos = q + 1;
				}
				else if (decoded[q] == '&')
				{
					valueEnd = q;
					break;
				}
			}

			if (first)
			{
				first = false;
				if (decoded[namePos] == '?')
					namePos++;
			}

			string name, value;

			if (valuePos == -1)
			{

				name = null;
				valuePos = namePos;
			}
			else
			{
				name = WWW.UnEscapeURL(decoded.Substring(namePos, valuePos - namePos - 1));
			}

			if (valueEnd < 0)
			{
				namePos = -1;
				valueEnd = decoded.Length;
			}
			else
			{
				namePos = valueEnd + 1;
			}

			value = WWW.UnEscapeURL(decoded.Substring(valuePos, valueEnd - valuePos));

			result.Add(name, value);
			if (namePos == -1)
				break;
		}
		return result;
	}

	private System.Int64 ConvertEpochSecondsToTicks(System.Int64 secs)
	{
		System.DateTime epoch = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
		System.Int64 seconds_to_100ns_ticks = 10 * 1000;
		System.Int64 max_seconds_allowed = (System.DateTime.MaxValue.Ticks - epoch.Ticks)
												/ seconds_to_100ns_ticks;
		if (secs < 0)
			secs = 0;
		if (secs > max_seconds_allowed)
			secs = max_seconds_allowed;
		return epoch.Ticks + secs * seconds_to_100ns_ticks;
	}

	private const int ERROR_CONTACTING_SERVER = 0x101;
	private const int ERROR_INVALID_PACKAGE_NAME = 0x102;
	private const int ERROR_NON_MATCHING_UID = 0x103;

	[Beebyte.Obfuscator.ObfuscateLiterals]
	private void Process()
	{
		m_LVL_Received = true;
		m_ButtonMessage = "Check LVL";
		m_ButtonEnabled = true;

		if (m_LVLCheck == null)
			return;

		int responseCode = m_LVLCheck.Get<int>("_arg0");
		string message = m_LVLCheck.Get<string>("_arg1");
		string signature = m_LVLCheck.Get<string>("_arg2");

		mArg0 = responseCode;
		Arg1 = message;
		Arg2 = signature;

		m_LVLCheck = null;

		m_ResponseCode_Received = responseCode.ToString();
		if (responseCode < 0 || string.IsNullOrEmpty(message) || string.IsNullOrEmpty(signature))
		{
			m_PackageName_Received = "<Failed>";
			return;
		}
#if DEBUG
		byte[] message_bytes = System.Text.Encoding.UTF8.GetBytes(message);
		byte[] signature_bytes = System.Convert.FromBase64String(signature);
		RSACryptoServiceProvider csp = new RSACryptoServiceProvider();
		csp.ImportParameters(m_PublicKey);
		SHA1Managed sha1 = new SHA1Managed();
		bool match = csp.VerifyHash(sha1.ComputeHash(message_bytes), CryptoConfig.MapNameToOID("SHA1"), signature_bytes);

		if (!match)
		{
			m_ResponseCode_Received = "<Failed>";
			m_PackageName_Received = "<Invalid Signature>";
			return;
		}

		int index = message.IndexOf(':');
		string mainData, extraData;
		if (-1 == index)
		{
			mainData = message;
			extraData = "";
		}
		else
		{
			mainData = message.Substring(0, index);
			extraData = index >= message.Length ? "" : message.Substring(index + 1);
		}

		string[] vars = mainData.Split('|');        // response | nonce | package | version | userid | timestamp

		if (vars[0].CompareTo(responseCode.ToString()) != 0)
		{
			m_ResponseCode_Received = "<Failed>";
			m_PackageName_Received = "<Response Mismatch>";
			return;
		}

		m_ResponseCode_Received = vars[0];
		m_Nonce_Received = System.Convert.ToInt32(vars[1]);
		m_PackageName_Received = vars[2];
		m_VersionCode_Received = System.Convert.ToInt32(vars[3]);
		m_UserID_Received = vars[4];
		System.Int64 ticks = ConvertEpochSecondsToTicks(System.Convert.ToInt64(vars[5]));
		m_Timestamp_Received = new System.DateTime(ticks).ToLocalTime().ToString();
		CurTimeStamp = ticks;

		if (!string.IsNullOrEmpty(extraData))
		{
			Dictionary<string, string> extrasDecoded = DecodeExtras(extraData);

			if (extrasDecoded.ContainsKey("GR"))
			{
				m_MaxRetry_Received = System.Convert.ToInt32(extrasDecoded["GR"]);
			}
			else
			{
				m_MaxRetry_Received = 0;
			}

			if (extrasDecoded.ContainsKey("VT"))
			{
				ticks = ConvertEpochSecondsToTicks(System.Convert.ToInt64(extrasDecoded["VT"]));
				m_LicenceValidityTimestamp_Received = new System.DateTime(ticks).ToLocalTime().ToString();
			}
			else
			{
				m_LicenceValidityTimestamp_Received = null;
			}

			if (extrasDecoded.ContainsKey("GT"))
			{
				ticks = ConvertEpochSecondsToTicks(System.Convert.ToInt64(extrasDecoded["GT"]));
				m_GracePeriodTimestamp_Received = new System.DateTime(ticks).ToLocalTime().ToString();
			}
			else
			{
				m_GracePeriodTimestamp_Received = null;
			}

			if (extrasDecoded.ContainsKey("UT"))
			{
				ticks = ConvertEpochSecondsToTicks(System.Convert.ToInt64(extrasDecoded["UT"]));
				m_UpdateTimestamp_Received = new System.DateTime(ticks).ToLocalTime().ToString();
			}
			else
			{
				m_UpdateTimestamp_Received = null;
			}

			if (extrasDecoded.ContainsKey("FILE_URL1"))
			{
				m_FileURL1_Received = extrasDecoded["FILE_URL1"];
			}
			else
			{
				m_FileURL1_Received = "";
			}

			if (extrasDecoded.ContainsKey("FILE_URL2"))
			{
				m_FileURL2_Received = extrasDecoded["FILE_URL2"];
			}
			else
			{
				m_FileURL2_Received = "";
			}

			if (extrasDecoded.ContainsKey("FILE_NAME1"))
			{
				m_FileName1_Received = extrasDecoded["FILE_NAME1"];
			}
			else
			{
				m_FileName1_Received = null;
			}

			if (extrasDecoded.ContainsKey("FILE_NAME2"))
			{
				m_FileName2_Received = extrasDecoded["FILE_NAME2"];
			}
			else
			{
				m_FileName2_Received = null;
			}

			if (extrasDecoded.ContainsKey("FILE_SIZE1"))
			{
				m_FileSize1_Received = System.Convert.ToInt32(extrasDecoded["FILE_SIZE1"]);
			}
			else
			{
				m_FileSize1_Received = 0;
			}

			if (extrasDecoded.ContainsKey("FILE_SIZE2"))
			{
				m_FileSize2_Received = System.Convert.ToInt32(extrasDecoded["FILE_SIZE2"]);
			}
			else
			{
				m_FileSize2_Received = 0;
			}

			if (extrasDecoded.ContainsKey("LU"))
			{
				m_LicensingURL_Received = extrasDecoded["LU"];
			}
			else
			{
				m_LicensingURL_Received = "";
			}
		}
#endif
	}
#if DEBUG
	public void TestSignature(int responseCode, string message, string signature)
    {
		m_ResponseCode_Received = responseCode.ToString();
		if (responseCode < 0 || string.IsNullOrEmpty(message) || string.IsNullOrEmpty(signature))
		{
			m_PackageName_Received = "<Failed>";
			return;
		}

		byte[] message_bytes = System.Text.Encoding.UTF8.GetBytes(message);
		byte[] signature_bytes = System.Convert.FromBase64String(signature);
		RSACryptoServiceProvider csp = new RSACryptoServiceProvider();
		csp.ImportParameters(m_PublicKey);
		var pKey = csp.ExportParameters(false);
		Debug.Log("M = " + Convert.ToBase64String(pKey.Modulus));
		Debug.Log("E = " + Convert.ToBase64String(pKey.Exponent));
		SHA1Managed sha1 = new SHA1Managed();
		var h1 = sha1.ComputeHash(message_bytes);
		var oid = CryptoConfig.MapNameToOID("SHA1");
		bool match = csp.VerifyHash(h1, oid, signature_bytes);

		if (!match)
		{
			m_ResponseCode_Received = "<Failed>";
			m_PackageName_Received = "<Invalid Signature>";
			return;
		}

		int index = message.IndexOf(':');
		string mainData, extraData;
		if (-1 == index)
		{
			mainData = message;
			extraData = "";
		}
		else
		{
			mainData = message.Substring(0, index);
			extraData = index >= message.Length ? "" : message.Substring(index + 1);
		}

		string[] vars = mainData.Split('|');        // response | nonce | package | version | userid | timestamp

		if (vars[0].CompareTo(responseCode.ToString()) != 0)
		{
			m_ResponseCode_Received = "<Failed>";
			m_PackageName_Received = "<Response Mismatch>";
			return;
		}

		m_ResponseCode_Received = vars[0];
		m_Nonce_Received = System.Convert.ToInt32(vars[1]);
		m_PackageName_Received = vars[2];
		m_VersionCode_Received = System.Convert.ToInt32(vars[3]);
		m_UserID_Received = vars[4];
		System.Int64 ticks = ConvertEpochSecondsToTicks(System.Convert.ToInt64(vars[5]));
		m_Timestamp_Received = new System.DateTime(ticks).ToLocalTime().ToString();
		CurTimeStamp = ticks;

		if (!string.IsNullOrEmpty(extraData))
		{
			Dictionary<string, string> extrasDecoded = DecodeExtras(extraData);

			if (extrasDecoded.ContainsKey("GR"))
			{
				m_MaxRetry_Received = System.Convert.ToInt32(extrasDecoded["GR"]);
			}
			else
			{
				m_MaxRetry_Received = 0;
			}

			if (extrasDecoded.ContainsKey("VT"))
			{
				ticks = ConvertEpochSecondsToTicks(System.Convert.ToInt64(extrasDecoded["VT"]));
				m_LicenceValidityTimestamp_Received = new System.DateTime(ticks).ToLocalTime().ToString();
			}
			else
			{
				m_LicenceValidityTimestamp_Received = null;
			}

			if (extrasDecoded.ContainsKey("GT"))
			{
				ticks = ConvertEpochSecondsToTicks(System.Convert.ToInt64(extrasDecoded["GT"]));
				m_GracePeriodTimestamp_Received = new System.DateTime(ticks).ToLocalTime().ToString();
			}
			else
			{
				m_GracePeriodTimestamp_Received = null;
			}

			if (extrasDecoded.ContainsKey("UT"))
			{
				ticks = ConvertEpochSecondsToTicks(System.Convert.ToInt64(extrasDecoded["UT"]));
				m_UpdateTimestamp_Received = new System.DateTime(ticks).ToLocalTime().ToString();
			}
			else
			{
				m_UpdateTimestamp_Received = null;
			}

			if (extrasDecoded.ContainsKey("FILE_URL1"))
			{
				m_FileURL1_Received = extrasDecoded["FILE_URL1"];
			}
			else
			{
				m_FileURL1_Received = "";
			}

			if (extrasDecoded.ContainsKey("FILE_URL2"))
			{
				m_FileURL2_Received = extrasDecoded["FILE_URL2"];
			}
			else
			{
				m_FileURL2_Received = "";
			}

			if (extrasDecoded.ContainsKey("FILE_NAME1"))
			{
				m_FileName1_Received = extrasDecoded["FILE_NAME1"];
			}
			else
			{
				m_FileName1_Received = null;
			}

			if (extrasDecoded.ContainsKey("FILE_NAME2"))
			{
				m_FileName2_Received = extrasDecoded["FILE_NAME2"];
			}
			else
			{
				m_FileName2_Received = null;
			}

			if (extrasDecoded.ContainsKey("FILE_SIZE1"))
			{
				m_FileSize1_Received = System.Convert.ToInt32(extrasDecoded["FILE_SIZE1"]);
			}
			else
			{
				m_FileSize1_Received = 0;
			}

			if (extrasDecoded.ContainsKey("FILE_SIZE2"))
			{
				m_FileSize2_Received = System.Convert.ToInt32(extrasDecoded["FILE_SIZE2"]);
			}
			else
			{
				m_FileSize2_Received = 0;
			}

			if (extrasDecoded.ContainsKey("LU"))
			{
				m_LicensingURL_Received = extrasDecoded["LU"];
			}
			else
			{
				m_LicensingURL_Received = "";
			}
		}
	}
#endif
}
