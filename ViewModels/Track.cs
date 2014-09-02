using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iRacingFriendlist.Http;
using Newtonsoft.Json.Linq;

namespace iRacingFriendlist.ViewModels
{
    public class Track
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Config { get; set; }

        public static List<Track> ParseJson(string json)
        {
            var tracks = new List<Track>();

            var array = JArray.Parse(json);
            foreach (dynamic row in array)
            {
                var track = new Track();
                track.Id = Convert<int>(row["id"]);
                track.Name = HttpUtility.UrlDecode(Convert<string>(row["name"]));
                track.Config = HttpUtility.UrlDecode(Convert<string>(row["config"]));
                tracks.Add(track);
            }

            return tracks;
        }

        private static T Convert<T>(dynamic obj, T defaultValue = default(T))
        {
            if (obj == null) return defaultValue;
            return (T)obj;
        }
    }
}
