using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using iRacingFriendlist.Classes;

namespace iRacingFriendlist.Converters
{
    public class FontSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (FontSizes) value;
            var fontsize = FontSize.Get(val);

            if (parameter != null)
            {
                var mod = (string) parameter;
                if (mod.ToLower().Trim() == "small") return fontsize.Smaller;
                if (mod.ToLower().Trim() == "large") return fontsize.Larger;
                if (mod.ToLower().Trim() == "img") return fontsize.ImageSize;
            }
            return fontsize.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
