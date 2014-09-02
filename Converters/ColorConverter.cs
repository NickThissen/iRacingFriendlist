using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using iRacingFriendlist.Classes;
using iRacingFriendlist.ViewModels;

namespace iRacingFriendlist.Converters
{
    public class ColorConverter : IValueConverter
    {
        private static GradientColor DefaultColor = new GradientColor(Color.FromArgb(230, 210, 210, 210), Color.FromArgb(230, 150, 150, 150));
        private static GradientColor OnlineColor = new GradientColor(Color.FromArgb(255, 137, 187, 255), Color.FromArgb(255, 0, 60, 155));
        private static GradientColor InSessionColor = new GradientColor(Color.FromArgb(255, 170, 255, 137), Colors.LimeGreen, Color.FromArgb(255, 41, 155, 0));
        //private static GradientColor WatchingColor = new GradientColor(Color.FromArgb(255, 250, 110, 255), Color.FromArgb(255, 201, 0, 208), Color.FromArgb(255, 115, 0, 119));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Driver)
                return Convert(((Driver) value).OnlineStatus, targetType, parameter);
            if (value is Driver.OnlineStatuses)
                return Convert((Driver.OnlineStatuses) value, targetType, parameter);

            throw new NotImplementedException();
        }

        private object Convert(Driver.OnlineStatuses status, Type targetType, object parameter)
        {
            if (parameter == null)
            {
                if (status == Driver.OnlineStatuses.InSession) return Return(InSessionColor.Middle, targetType);
                if (status == Driver.OnlineStatuses.Online) return Return(OnlineColor.Middle, targetType);
                return Return(DefaultColor.Middle, targetType);
            }

            if (parameter.ToString() == "gradient")
            {
                if (status == Driver.OnlineStatuses.InSession) return InSessionColor.GetBrush(true);
                if (status == Driver.OnlineStatuses.Online) return OnlineColor.GetBrush(true);
                return DefaultColor.GetBrush(true);
            }
            return Return(Colors.Transparent, targetType);
        }

        private object Return(Color color, Type type)
        {
            if (type == typeof(Brush)) return new SolidColorBrush(color);
            return color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
