using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRacingFriendlist.Classes
{
    public static class Util
    {
        public const int MinUpdateFreq = 10;
        public static int CapUpdateFreq(int freq)
        {
            if (freq < 10) freq = 10;
            return freq;
        }

        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static DateTime DateTimeFromUnixTimestampMillis(long millis)
        {
            return UnixEpoch.AddMilliseconds(millis).ToLocalTime();
        }

        public static string GetHelmetUrl(int size, int license, int hp, string[] colors)
        {
            return string.Format("http://127.0.0.1:32034/helmet.png?size={0}&pat={1}&lic={2}&colors={3}",
                size,
                hp,
                license,
                string.Join(",", colors));
        }

        public enum PopupLocations
        {
            BottomRight = 0,
            BottomLeft = 1,
            TopLeft = 2,
            TopRight = 3
        }

    }
}
