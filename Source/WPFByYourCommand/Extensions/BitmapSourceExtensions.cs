using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace WPFByYourCommand.Extensions
{
    public static class BitmapSourceExtensions
    {
        public static bool IsEqual(this BitmapSource image1, BitmapSource image2)
        {
            if (image1 == null || image2 == null)
            {
                return false;
            }
            return image1.ToBytes().SequenceEqual(image2.ToBytes());
        }

        public static byte[] ToBytes(this BitmapSource image)
        {
            byte[] data = new byte[] { };
            if (image != null)
            {
                BmpBitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                using (MemoryStream ms = new MemoryStream())
                {
                    encoder.Save(ms);
                    data = ms.ToArray();
                }
                return data;
            }
            return data;
        }
    }
}
