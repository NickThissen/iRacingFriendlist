using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using FontSizeConverter = iRacingFriendlist.Converters.FontSizeConverter;

namespace iRacingFriendlist.Classes
{
    public class FontSizeBindingExtension : SettingBindingExtension
    {
        public FontSizeBindingExtension()
        {
            Initialize();
        }

        public FontSizeBindingExtension(string path)
            : base(path)
        {
            Initialize();
        }

        private void Initialize()
        {
            this.Path = new PropertyPath("FontSize");
            this.Converter = new FontSizeConverter();
        }
    }
}
