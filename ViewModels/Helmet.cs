using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using iRacingFriendlist.Classes;

namespace iRacingFriendlist.ViewModels
{
    public class Helmet
    {
        public int License { get; set; }
        public int Size { get; set; }
        public int Pattern { get; set; }
        public string[] Colors { get; set; }

        public ImageSource DownloadImage()
        {
            var pictureUri = new Uri(GetUrl());
            var image = new BitmapImage(pictureUri);
            image.CacheOption = BitmapCacheOption.OnDemand;
            return image;
        }

        private string GetUrl()
        {
            return Util.GetHelmetUrl(Size, License, Pattern, Colors);
        }
    }
}
