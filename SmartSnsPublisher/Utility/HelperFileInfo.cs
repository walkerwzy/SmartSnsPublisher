using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSnsPublisher.Utility
{
    public static class HelperFileInfo
    {

        ///// <summary>
        ///// 获取文件的真实媒体类型。目前只支持JPG, GIF, PNG, BMP四种图片文件。
        ///// </summary>
        ///// <param name="fileData">文件字节流</param>
        ///// <returns>媒体类型</returns>
        //public static string GetMimeType(byte[] fileData)
        //{
        //    string suffix = GetFileSuffix(fileData);
        //    string mimeType;

        //    switch (suffix)
        //    {
        //        case "JPG": mimeType = "image/jpeg"; break;
        //        case "GIF": mimeType = "image/gif"; break;
        //        case "PNG": mimeType = "image/png"; break;
        //        case "BMP": mimeType = "image/bmp"; break;
        //        default: mimeType = "application/octet-stream"; break;
        //    }

        //    return mimeType;
        //}

        ///// <summary>
        ///// 获取文件的真实后缀名。目前只支持JPG, GIF, PNG, BMP四种图片文件。
        ///// </summary>
        ///// <param name="fileData">文件字节流</param>
        ///// <returns>JPG, GIF, PNG or null</returns>
        //public static string GetFileSuffix(byte[] fileData)
        //{
        //    if (fileData == null || fileData.Length < 10)
        //    {
        //        return null;
        //    }

        //    if (fileData[0] == 'G' && fileData[1] == 'I' && fileData[2] == 'F')
        //    {
        //        return "GIF";
        //    }
        //    if (fileData[1] == 'P' && fileData[2] == 'N' && fileData[3] == 'G')
        //    {
        //        return "PNG";
        //    }
        //    if (fileData[6] == 'J' && fileData[7] == 'F' && fileData[8] == 'I' && fileData[9] == 'F')
        //    {
        //        return "JPG";
        //    }
        //    if (fileData[0] == 'B' && fileData[1] == 'M')
        //    {
        //        return "BMP";
        //    }
        //    return null;
        //}

        public enum ImageFormat
        {
            bmp,
            jpeg,
            gif,
            tiff,
            png,
            unknown
        }

        public static ImageFormat GetImageFormat(byte[] bytes)
        {
            // see http://www.mikekunz.com/image_file_header.html  
            var bmp = Encoding.ASCII.GetBytes("BM");     // BMP
            var gif = Encoding.ASCII.GetBytes("GIF");    // GIF
            var png = new byte[] { 137, 80, 78, 71 };    // PNG
            var tiff = new byte[] { 73, 73, 42 };         // TIFF
            var tiff2 = new byte[] { 77, 77, 42 };         // TIFF
            var jpeg = new byte[] { 255, 216, 255, 224 }; // jpeg
            var jpeg2 = new byte[] { 255, 216, 255, 225 }; // jpeg canon

            if (bmp.SequenceEqual(bytes.Take(bmp.Length)))
                return ImageFormat.bmp;

            if (gif.SequenceEqual(bytes.Take(gif.Length)))
                return ImageFormat.gif;

            if (png.SequenceEqual(bytes.Take(png.Length)))
                return ImageFormat.png;

            if (tiff.SequenceEqual(bytes.Take(tiff.Length)))
                return ImageFormat.tiff;

            if (tiff2.SequenceEqual(bytes.Take(tiff2.Length)))
                return ImageFormat.tiff;

            if (jpeg.SequenceEqual(bytes.Take(jpeg.Length)))
                return ImageFormat.jpeg;

            if (jpeg2.SequenceEqual(bytes.Take(jpeg2.Length)))
                return ImageFormat.jpeg;

            return ImageFormat.unknown;
        }

        /// <summary>
        /// 从图片流读取真实文件类型，同时返出扩展名和mime type
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="extName">返回扩展名</param>
        /// <returns>mimy type</returns>
        public static string GetImageMIMEType(byte[] bytes, out string extName)
        {
            var ext = GetImageFormat(bytes);
            extName = "." + Enum.GetName(typeof(ImageFormat), ext);
            return string.Format("image/{0}", extName.TrimStart('.'));
        }
    }

}
