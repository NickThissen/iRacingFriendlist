using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using iRacingFriendlist.Classes;

namespace iRacingFriendlist.ViewModels
{
    public class Note
    {
        public Note()
        {
            this.CustId = -1;
            this.Text = string.Empty;
        }

        public Note(int id, string txt)
        {
            this.CustId = id;
            this.Text = txt;
        }

        public int CustId { get; set; }
        public string Text { get; set; }

        public static void Save(Dictionary<int, Note> notes)
        {
            try
            {
                var path = Paths.NotesPath;
                using (var writer = new XmlTextWriter(path, Encoding.Default))
                {
                    writer.Formatting = Formatting.Indented;
                    var serializer = new XmlSerializer(typeof(List<Note>), new[] { typeof(Note) });
                    serializer.Serialize(writer, notes.Values.ToList());
                }
            }
            catch (Exception ex)
            {
                Logging.Instance.LogError("Saving friend notes.", ex);
            }
        }

        public static Dictionary<int, Note> Load()
        {
            var notes = new List<Note>();
            try
            {
                var path = Paths.NotesPath;
                if (File.Exists(path))
                {
                    using (var reader = new XmlTextReader(path))
                    {
                        var serializer = new XmlSerializer(typeof(List<Note>), new[] { typeof(Note) });
                        notes = (List<Note>)serializer.Deserialize(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Instance.LogError("Reading friend notes.", ex);
            }
            return notes.ToDictionary(n => n.CustId);
        }
    }
}
