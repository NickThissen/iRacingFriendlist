using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using iRacingFriendlist.Classes;
using iRacingFriendlist.ViewModels;
using iRacingPmToolPluginLibrary;

namespace iRacingFriendlist.Http
{
    public static class Global
    {
        static Global()
        {
            _tracks = new List<Track>();
            _cars = new List<Car>();
            _seasons = new List<Season>();
        }

        private static HttpPost _connection;
        public static HttpPost Connection
        {
            get { return _connection; }
        }

        private static string _user;
        public static string User { get { return _user; } }

        private static List<Track> _tracks;
        public static List<Track> Tracks { get { return _tracks; }}

        private static List<Car> _cars;
        public static List<Car> Cars { get { return _cars; } }

        private static List<Season> _seasons;
        public static List<Season> Seasons { get { return _seasons; } }

        public static void GetContentListings()
        {
            string html = _connection.GetFriendsPage();
            FriendsPageParser.Parse(html);
        }

        public static HttpPost.LoginResults Login(string username, string password)
        {
            _connection = new HttpPost(username, password);
            var result = _connection.Login();

            if (result == HttpPost.LoginResults.Success)
            {
                _user = username;
            }
            else
            {
                _user = string.Empty;
            }

            return result;
        }
    }
}
