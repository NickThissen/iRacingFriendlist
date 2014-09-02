using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRacingFriendlist.Classes
{
    public class Logging
    {
        private string _path;

        private Logging()
        {
            _path = Paths.LogPath;
            if (!File.Exists(_path))
            {
                File.WriteAllText(_path, "Logfile created at " + GetDate() + ".\n==============\n\n");
            }
        }

        private static Logging _instance;
        public static Logging Instance { get { return _instance ?? (_instance = new Logging()); } }

        public void LogError(string action, Exception ex)
        {
            LogMessage("ERROR", action + "\n" + ex.Message + "\n" + ex.ToString());
        }

        public void LogMessage(string type, string message)
        {
            var builder = new StringBuilder();
            builder.Append(GetDate());
            builder.Append(" - " + type + ":\n");
            builder.Append(message + "\n");
            builder.Append("------------------\n\n");
            
            File.AppendAllText(_path, builder.ToString());
        }

        private string GetDate()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
