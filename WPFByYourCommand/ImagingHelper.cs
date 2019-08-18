using System;
using System.Windows.Media.Imaging;

namespace WPFByYourCommand
{
    public static class ImagingHelper
    {
        public static BitmapImage LoadImageResource(Uri urisource)
        {
            BitmapImage bmi = new BitmapImage();
            bmi.BeginInit();
            bmi.UriSource = urisource;
            bmi.EndInit();

            bmi.Freeze();
            return bmi;
        }

        public static BitmapImage LoadImageResource(string urisource)
        {
            return LoadImageResource(new Uri(urisource));
        }
    }
}
