using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iRacingFriendlist.Http;
using iRacingFriendlist.ViewModels;
using Newtonsoft.Json.Linq;

namespace iRacingFriendlist.Classes
{
    public static class FriendlistParser
    {
        public static DriverCollection ParseJson(string json)
        {
            var drivers = new DriverCollection();

            // Parse json string
            var obj = JObject.Parse(json);

            // Find 'fsRacers' array
            var fsRacers = (JArray)obj["fsRacers"];
            foreach (dynamic row in fsRacers)
            {
                // For each row, get driver details
                var custId = Convert<int>(row["custid"]);
                var name = Convert<string>(row["name"]);
                var hidden = Convert<bool>(row["hidden"]);

                var driver = new Driver();
                driver.Name = HttpUtility.UrlDecode(name);
                driver.CustId = custId;
                driver.IsPrivate = hidden;
                
                if (!hidden)
                {
                    var lastSeen = Convert<long>(row["lastSeen"]);
                    var lastOnline = Convert<long>(row["lastLogin"]);

                    driver.Session.SessionId = Convert<long>(row["sessionId"], -1);
                    driver.Session.SeasonId = Convert<int>(row["seasonId"], -1);
                    driver.Session.SeriesId = Convert<int>(row["seriesId"], -1);
                    driver.Session.EventType = (Session.EventTypes) Convert<int>(row["eventTypeId"], 0);
                    driver.Session.CarId = Convert<int>(row["carId"]);
                    driver.Session.TrackId = Convert<int>(row["trackId"]);
                    driver.Session.StartTime = Util.DateTimeFromUnixTimestampMillis(Convert<long>(row["startTime"]));
                    driver.Session.UserRole = Convert<Session.UserRoles>(row["userRole"]);
                    driver.Session.RegistrationOpen = Convert<bool>(row["regOpen"]);

                    var privateses = (JObject) row["privateSession"];
                    if (privateses.Count > 0 && driver.Session.SeasonId == 0 && driver.Session.SeriesId == 0)
                    {
                        driver.Session.IsHosted = true;
                        driver.Session.SessionName = HttpUtility.UrlDecode(Convert<string>(privateses["sessionName"]));
                    }

                    driver.Session.SetValues();
                    
                    var helmetObj = (JObject) row["helmet"];
                    var helmet = new Helmet();
                    helmet.License = Convert<int>(helmetObj["ll"]);
                    helmet.Pattern = Convert<int>(helmetObj["hp"]);
                    helmet.Colors = new[]
                    {
                        Convert<string>(helmetObj["c1"]),
                        Convert<string>(helmetObj["c2"]),
                        Convert<string>(helmetObj["c3"])

                    };
                    helmet.Size = 4;
                    driver.Helmet = helmet;

                    driver.LastOnline = Util.DateTimeFromUnixTimestampMillis(lastOnline);
                    driver.IsInSession = driver.Session.SessionId >= 0;

                    if (lastSeen == 0)
                    {
                        driver.IsOnline = false;
                        driver.LastActivity = null;
                    }
                    else
                    {
                        driver.IsOnline = true;
                        driver.LastActivity = Util.DateTimeFromUnixTimestampMillis(lastSeen);
                    }
                }
                drivers.Add(driver);
            }

            return drivers;
        }

        private static T Convert<T>(dynamic obj, T defaultValue = default(T))
        {
            if (obj == null) return defaultValue;
            return (T) obj;
        }
    }
}
