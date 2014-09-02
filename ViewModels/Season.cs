using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iRacingFriendlist.Http;
using Newtonsoft.Json.Linq;

namespace iRacingFriendlist.ViewModels
{
    public class Season
    {
        public int SeasonId { get; set; }
        public int SeriesId { get; set; }
        public string SeriesName { get; set; }
        public string SeasonName { get; set; }

        public static List<Season> ParseJson(string json)
        {
            var seasons = new List<Season>();

            var array = JArray.Parse(json);
            foreach (dynamic row in array)
            {
                var season = new Season();
                season.SeriesId = Convert<int>(row["seriesid"]);
                season.SeasonId = Convert<int>(row["seasonid"]);
                season.SeriesName = HttpUtility.UrlDecode(Convert<string>(row["seriesname"]));
                season.SeasonName = HttpUtility.UrlDecode(Convert<string>(row["seasonshortname"]));
                seasons.Add(season);
            }

            return seasons;
        }

        private static T Convert<T>(dynamic obj, T defaultValue = default(T))
        {
            if (obj == null) return defaultValue;
            return (T)obj;
        }
    }
}
