using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRacingFriendlist.Classes
{
    public static class Paths
    {
        private static string _path;

        static Paths()
        {
            _path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "NickThissen", "iRacingFriendlist");

            if (!Directory.Exists(_path)) Directory.CreateDirectory(_path);
        }

        public static string NotesPath { get { return GetPath(Path.Combine(_path, "notes.xml")); } }
        public static string LogPath { get { return GetPath(Path.Combine(_path, "log.txt")); } }
    
        private static string GetPath(string file)
        {
            var path = Path.GetDirectoryName(file);
            if (path != null && !Directory.Exists(path)) Directory.CreateDirectory(path);
            return file;
        }
    }
}
