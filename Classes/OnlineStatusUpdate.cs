using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using iRacingFriendlist.ViewModels;

namespace iRacingFriendlist.Classes
{
    public class OnlineStatusUpdate
    {
        private readonly Driver _driver;
        private Driver.OnlineStatuses _from;

        public OnlineStatusUpdate(Driver driver)
        {
            this.SessionDisplay = string.Empty;
            this.Time = DateTime.Now;
            _driver = driver;
        }

        public DateTime Time { get; private set; }

        public string TimeDisplay
        {
            get { return this.Time.ToString("HH:mm:ss"); }
        }

        public Driver Driver
        {
            get { return _driver; }
        }

        public Driver.OnlineStatuses From
        {
            get { return _from; }
            set
            {
                _from = value;
                this.To = value;
            }
        }

        public Driver.OnlineStatuses To { get; set; }

        public string FromDisplay
        {
            get { return From == Driver.OnlineStatuses.InSession ? "In session" : From.ToString(); }
        }

        public string ToDisplay
        {
            get { return To == Driver.OnlineStatuses.InSession ? "In session" : To.ToString(); }
        }

        public string SessionDisplay { get; private set; }

        public void SetSession(Session session)
        {
            string role = "";
            if (session.UserRole == Session.UserRoles.Watching) role = " (spectating)";
            if (session.UserRole == Session.UserRoles.Spotting) role = " (spotting)";
            this.SessionDisplay = string.Format("{0}: {1}, {2}{3}.",
                                                session.IsHosted ? "Hosted" : "Series",
                                                session.IsHosted ? session.SessionName : session.SeriesName,
                                                session.EventTypeDisplay,
                                                role);
        }

        public bool IsUpdate
        {
            get { return this.From != this.To; }
        }
    }
}
