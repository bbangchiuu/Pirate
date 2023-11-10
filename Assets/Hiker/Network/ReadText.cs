using System.Collections;
using System.Text;
using LZ4;
using System.Text.RegularExpressions;
using Base64FormattingOptions = System.Base64FormattingOptions;
using Convert = System.Convert;

public class ReadText
{
    public static bool IsBase64String(string s)
    {
        s = s.Trim();
        return (s.Length % 4 == 0) && Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
    }

    public static readonly Aes aes = new Aes();

    public static string DecompressString(string compressedStr, bool decrypt = false)
    {
        if (compressedStr == null) return null;
        if (compressedStr.Length == 0) return null;
//        if (IsBase64String(compressedStr) == false)
//        {
//#if FIRE_BASE
//            Firebase.Crashlytics.Crashlytics.Log("Non64= " + compressedStr);
//#endif
//        }

        byte[] compressedData = Convert.FromBase64String(compressedStr);
        if (decrypt)
            compressedData = aes.DecryptBytes(compressedData);
        byte[] inputData = LZ4Codec.Unwrap(compressedData);
        return Encoding.UTF8.GetString(inputData);
    }

    public static string CompressString(string str, bool encrypt = false)
    {
        byte[] inputData = Encoding.UTF8.GetBytes(str);
        byte[] compressedData = LZ4Codec.Wrap(inputData);
        if (encrypt)
            compressedData = aes.EncryptBytes(compressedData);
        return Convert.ToBase64String(compressedData, Base64FormattingOptions.None);
    }
    public static byte[] DecompressByteArray(byte[] compressedData, bool decrypt)
    {
        if (decrypt)
            compressedData = aes.DecryptBytes(compressedData);
        byte[] inputData = LZ4Codec.Unwrap(compressedData);
        return inputData;
    }

    public static byte[] CompressStringToByteArray(string str)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(str);
        byte[] data = LZ4Codec.Wrap(bytes);
        return data;
    }

}
