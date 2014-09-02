using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRacingFriendlist.Classes
{
    public enum FontSizes
    {
        Tiny = 0,
        Small = 1,
        Normal = 2,
        Large = 3,
        Huge = 4
    }

    public class FontSize
    {
        public FontSize(FontSizes size, double smaller, double normal, double larger, double img)
        {
            this.Size =size;
            this.Smaller = smaller;
            this.Normal = normal;
            this.Larger = larger;
            this.ImageSize = img;
        }

        public double Smaller { get; private set; }
        public double Normal { get; private set; }
        public double Larger { get; private set; }
        public double ImageSize { get; private set; }

        public FontSizes Size { get; private set; }

        private static List<FontSize> _sizes;

        static FontSize()
        {
            _sizes = new List<FontSize>();
            _sizes.Add(new FontSize(FontSizes.Tiny, 10, 11, 13, 20));
            _sizes.Add(new FontSize(FontSizes.Small, 11, 12, 14, 20));
            _sizes.Add(new FontSize(FontSizes.Normal, 12, 14, 16, 30));
            _sizes.Add(new FontSize(FontSizes.Large, 14, 15, 17, 30));
            _sizes.Add(new FontSize(FontSizes.Huge, 15, 16, 18, 45));
        }

        public static FontSize Get(FontSizes size)
        {
            return _sizes.SingleOrDefault(s => s.Size == size) ?? _sizes.Single(s => s.Size == FontSizes.Normal);
        }
    }
}
