using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using iRacingFriendlist.Http;
using iRacingFriendlist.ViewModels;

namespace iRacingFriendlist.Classes
{
    public static class FriendsPageParser
    {
        private const string TrackListing = @"var TrackListing = extractJSON('";
        private const string CarListing = @"var CarListing = extractJSON('";
        private const string SeasonListing = @"var SeasonListing = extractJSON('";

        public static void Parse(string html)
        {
            var lines =
                (html.Split(new[] {'\n'}, StringSplitOptions.RemoveEmptyEntries)).Select(l => l.Trim()).ToList();
            
            ExtractTracks(lines);
            ExtractCars(lines);
            ExtractSeasons(lines);
        }

        private static void ExtractTracks(List<string> lines)
        {
            var line = lines.FirstOrDefault(l => l.StartsWith(TrackListing));
            if (!string.IsNullOrWhiteSpace(line))
            {
                // Extract JSON
                // Format:     var TrackListing = extractJSON('<<json>>');
                // Remove first part 
                // Remove last three characters ');
                var json = line.Substring(TrackListing.Length, line.Length - TrackListing.Length - 3);

                Global.Tracks.AddRange(Track.ParseJson(json));
            }
        }

        private static void ExtractCars(List<string> lines)
        {
            var line = lines.FirstOrDefault(l => l.StartsWith(CarListing));
            if (!string.IsNullOrWhiteSpace(line))
            {
                // Extract JSON
                // Format:     var CarListing = extractJSON('<<json>>');
                // Remove first part 
                // Remove last three characters ');
                var json = line.Substring(CarListing.Length, line.Length - CarListing.Length - 3);

                Global.Cars.AddRange(Car.ParseJson(json));
            }
        }

        private static void ExtractSeasons(List<string> lines)
        {
            var line = lines.FirstOrDefault(l => l.StartsWith(SeasonListing));
            if (!string.IsNullOrWhiteSpace(line))
            {
                // Extract JSON
                // Format:     var SeasonListing = extractJSON('<<json>>');
                // Remove first part 
                // Remove last three characters ');
                var json = line.Substring(SeasonListing.Length, line.Length - SeasonListing.Length - 3);

                Global.Seasons.AddRange(Season.ParseJson(json));
            }
        }
    }
}
