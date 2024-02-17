using NodaTime;
using NodaTime.Extensions;

namespace NerdCritica.Domain.Utils;

public static class DateTimeHelper
{
    private static readonly DateTimeZone BrasiliaTimeZone = DateTimeZoneProviders.Tzdb["America/Sao_Paulo"];
    private static readonly DateTimeZone NewYorkTimeZone = DateTimeZoneProviders.Tzdb["America/New_York"];

    public static DateTime NowInBrasilia()
    {
        var now = SystemClock.Instance.InZone(BrasiliaTimeZone).GetCurrentZonedDateTime();
        return new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Millisecond, DateTimeKind.Local);
    }

    public static DateTime NowInNewYork()
    {
        var now = SystemClock.Instance.InZone(NewYorkTimeZone).GetCurrentZonedDateTime();
        return new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Millisecond, DateTimeKind.Local);
    }
}