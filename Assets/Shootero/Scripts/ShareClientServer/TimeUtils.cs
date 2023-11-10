using DateTime = System.DateTime;

public partial class TimeUtils
{
    const int ServerTimeZone = 0; // time-zone offset value (UTC+7)
    /// <summary>
    /// Now in timezone same as server-client
    /// </summary>
    public static DateTime Now { get { return DateTime.UtcNow.AddHours(ServerTimeZone); } }

    /// <summary>
    /// Now in local time-zone
    /// </summary>
    public static DateTime LocalNow { get { return DateTime.Now; } }

    /// <summary>
    /// serverTime need to be Utc type
    /// </summary>
    /// <param name="serverTime"></param>
    /// <returns></returns>
    public static DateTime ConvertServerToLocalTime(DateTime serverTime)
    {
        if (serverTime.Kind == System.DateTimeKind.Local)
        {
            serverTime = serverTime.ToUniversalTime();
        }
        var timeInUtc = serverTime.AddHours(-ServerTimeZone);

        return timeInUtc.ToLocalTime();
    }

    /// <summary>
    /// localTime is time in client pc
    /// </summary>
    /// <param name="localTime"></param>
    /// <returns></returns>
    public static DateTime ConvertLocalToServerTimeZone(DateTime localTime)
    {
        if (localTime.Kind != System.DateTimeKind.Utc)
        {
            localTime = localTime.ToUniversalTime();
        }

        var timeInUtc = ServerTimeZone != 0 ? localTime.AddHours(ServerTimeZone) : localTime;
        return timeInUtc;
    }

    public static DateTime ConvertUtcToServerTimeZone(DateTime utcTime)
    {
        if (utcTime.Kind != System.DateTimeKind.Utc)
        {
            utcTime = utcTime.ToUniversalTime();
        }

        var timeInUtc = ServerTimeZone != 0 ? utcTime.AddHours(ServerTimeZone) : utcTime;
        return timeInUtc;
    }
}