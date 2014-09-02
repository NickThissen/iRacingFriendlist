using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace iRacingFriendlist.Classes
{
    public class GradientColor
    {
        public GradientColor()
            : this(Colors.Transparent, Colors.Transparent)
        {
        }

        public GradientColor(Color top, Color bottom)
            : this(top, top, bottom)
        {
        }

        public GradientColor(Color top, Color middle, Color bottom)
        {
            this.Top = top;
            this.Middle = middle;
            this.Bottom = bottom;
        }

        public Color Top { get; set; }
        public Color Middle { get; set; }
        public Color Bottom { get; set; }

        //public static GradientColor FromColor(Color color)
        //{
        //    var top = GetLighter(color);
        //    var bottom = GetDarker(color);
        //    return new GradientColor(top, color, bottom);
        //}

        //public static Color GetDarker(Color original)
        //{
        //    return ChangeLuminosity(original, -0.2);
        //}

        //public static Color GetLighter(Color original)
        //{
        //    return ChangeLuminosity(original, 0.2);
        //}

        //public static Color ChangeLuminosity(Color original, double factor)
        //{
        //    factor = 1 + factor;
        //    var r = (byte)Math.Round(Math.Min(Math.Max(0, original.R * factor), 255));
        //    var b = (byte)Math.Round(Math.Min(Math.Max(0, original.B * factor), 255));
        //    var g = (byte)Math.Round(Math.Min(Math.Max(0, original.G * factor), 255));
        //    return Color.FromRgb(r, b, g);
        //}

        public LinearGradientBrush GetBrush(bool diagonal = false, bool includeMiddle = false)
        {
            var brush = new LinearGradientBrush();
            brush.GradientStops.Add(new GradientStop(this.Top, 0));
            if (includeMiddle) brush.GradientStops.Add(new GradientStop(this.Middle, 0.5));
            brush.GradientStops.Add(new GradientStop(this.Bottom, 1));

            brush.StartPoint = new Point(0, 0);
            if (diagonal)
                brush.EndPoint = new Point(1,1);
            else
                brush.EndPoint = new Point(0,1);

            return brush;
        }
    }
}
