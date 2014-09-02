using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iRacingFriendlist.Http;

namespace iRacingFriendlist.ViewModels
{
    public class Session
    {
        private readonly Driver _driver;

        public Session(Driver driver)
        {
            _driver = driver;

            this.EventType = EventTypes.None;
        }

        public long SessionId { get; set; }
        public long SubsessionId { get; set; }
        public int SessionTypeId { get; set; }
        public EventTypes EventType { get; set; }
        public int SeriesId { get; set; }
        public int SeasonId { get; set; }
        public int CarId { get; set; }
        public int TrackId { get; set; }
        public bool RegistrationOpen { get; set; }
        public DateTime? StartTime { get; set; }
        public UserRoles UserRole { get; set; }
        public bool IsHosted { get; set; }
        public string SessionName { get; set; }

        public string SeriesName
        {
            get { return this.Season == null ? "-" : this.Season.SeriesName; }
        }

        public Season Season { get; private set; }
        public Car Car { get; private set; }
        public Track Track { get; private set; }
        
        public string StartTimeDisplay
        {
            get
            {
                if (this.StartTime == null) return "-";
                return this.StartTime.Value.ToString("HH:mm");
            }
        }

        public bool IsSpectator
        {
            get { return this.UserRole != UserRoles.Driving; }
        }

        public string EventTypeDisplay
        {
            get
            {
                if (this.EventType == EventTypes.None) return "?";
                if (this.EventType == EventTypes.TimeTrial) return "TT";
                return this.EventType.ToString().FirstOrDefault().ToString();
            }
        }

        public void SetValues()
        {
            this.Season = Global.Seasons.FirstOrDefault(s => s.SeasonId == this.SeasonId && s.SeriesId == this.SeriesId);
            this.Car = Global.Cars.FirstOrDefault(c => c.Id == this.CarId);
            this.Track = Global.Tracks.FirstOrDefault(t => t.Id == this.TrackId);
        }

        public enum EventTypes
        {
            None = 0,
            Test = 1,
            Practice = 2,
            Qualify = 3,
            TimeTrial = 4,
            Race = 5
        }

        public enum UserRoles
        {
            Driving = 0,
            Spotting = 1,
            Watching = 2
        }
    }
}
