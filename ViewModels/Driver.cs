using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace iRacingFriendlist.ViewModels
{
    public class Driver : ViewModelBase
    {
        private int _custId;
        private string _name;
        private bool _isPrivate;
        private bool _isOnline;
        private DateTime? _lastOnline;
        private DateTime? _lastActivity;
        private bool _isStudied;
        private bool _isFriend;
        private bool _isInSession;
        private Helmet _helmet;
        private ImageSource _helmetImage;
        private readonly Session _session;
        private string _noteText;

        public Driver()
        {
            _session = new Session(this);
        }

        public int CustId
        {
            get { return _custId; }
            set
            {
                _custId = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string NoteText
        {
            get { return _noteText; }
            set
            {
                _noteText = value;
                OnPropertyChanged();
            }
        }

        public Helmet Helmet
        {
            get { return _helmet; }
            set
            {
                _helmet = value;
                OnPropertyChanged();
            }
        }

        public ImageSource HelmetImage
        {
            get { return _helmetImage; }
            set
            {
                _helmetImage = value;
                OnPropertyChanged();
            }
        }

        public bool IsPrivate
        {
            get { return _isPrivate; }
            set
            {
                _isPrivate = value;
                OnPropertyChanged();
            }
        }

        public bool IsOnline
        {
            get { return _isOnline; }
            set
            {
                _isOnline = value;
                OnPropertyChanged();
            }
        }

        public bool IsInSession
        {
            get { return _isInSession; }
            set
            {
                _isInSession = value;
                OnPropertyChanged();
            }
        }

        public OnlineStatuses OnlineStatus
        {
            get
            {
                if (IsInSession) return OnlineStatuses.InSession;
                if (IsOnline) return OnlineStatuses.Online;
                return OnlineStatuses.Offline;
            }
        }

        public bool IsFriend
        {
            get { return _isFriend; }
            set
            {
                _isFriend = value;
                OnPropertyChanged();
            }
        }

        public bool IsStudied
        {
            get { return _isStudied; }
            set
            {
                _isStudied = value;
                OnPropertyChanged();
            }
        }

        public DateTime? LastOnline
        {
            get { return _lastOnline; }
            set
            {
                _lastOnline = value;
                OnPropertyChanged();
            }
        }

        public DateTime? LastActivity
        {
            get { return _lastActivity; }
            set
            {
                _lastActivity = value;
                OnPropertyChanged();
            }
        }

        public Session Session
        {
            get { return _session; }
        }

        public enum OnlineStatuses
        {
            Offline,
            Online,
            InSession
        }

        [Flags]
        public enum NotificationOptions
        {
            None = 0,
            OfflineOnline = 1,
            OfflineSession = 2,
            OnlineOffline = 4,
            OnlineSession = 8,
            SessionOffline = 16,
            SessionOnline = 32
        }

        public string OnlineStatusDisplay
        {
            get
            {
                if (this.IsInSession)
                {
                    return this.Session.EventTypeDisplay;
                }
                return string.Empty;
            }
        }

        public string Status
        {
            get
            {
                if (this.IsInSession)
                {
                    string series = "unknown series";
                    if (this.Session.IsHosted)
                    {
                        if (string.IsNullOrWhiteSpace(this.Session.SessionName))
                            return "Hosted: unknown session";
                        return "Hosted: " + this.Session.SessionName;
                    }
                    if (this.Session.Season != null) series = this.Session.Season.SeriesName;
                    return series;
                }
                if (this.IsOnline)
                {
                    string last = "unknown";
                    if (this.LastActivity != null) last = this.LastActivity.Value.ToString("yyyy-MM-dd HH:mm");
                    return "Last activity: " + last;
                }
                else
                {
                    string last = "unknown";
                    if (this.LastOnline != null) last = this.LastOnline.Value.ToString("yyyy-MM-dd HH:mm");
                    return "Last online: " + last;
                }
            }
        }
    }
}
