using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WRC_CMS.Repository
{
    public interface ICommon
    {
        int CurrentObjectId { get; }
    }
    public class CommonClass
    {
        public static object GetImage(Stream imgToResize)
        {
            using (var ms = new MemoryStream())
            {
                Image imgToR = Image.FromStream(imgToResize);
                Bitmap b = new Bitmap(100, 100);
                Graphics g = Graphics.FromImage((Image)b);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                g.DrawImage(imgToR, 0, 0, 100, 100);
                g.Dispose();

                b.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);

                return ms.ToArray();// string.Concat(ms.ToArray().Select(k => Convert.ToString(k, 2)));
                //return 0101010101010;
            }
        }

        public static byte[] GetImage(string fileName)
        {
            using (var ms = new MemoryStream())
            {
                Image imgToResize = Image.FromFile(fileName);

                Bitmap b = new Bitmap(100, 100);
                Graphics g = Graphics.FromImage((Image)b);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                g.DrawImage(imgToResize, 0, 0, 100, 100);
                g.Dispose();

                b.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);

                return ms.ToArray();// string.Concat(ms.ToArray().Select(k => Convert.ToString(k, 2)));
                //return 0101010101010;
            }
        }
    }
}