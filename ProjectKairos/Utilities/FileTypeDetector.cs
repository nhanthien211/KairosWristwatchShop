using System;
using System.IO;
using System.Text;
using System.Web;

namespace ProjectKairos.Utilities
{
    public class FileTypeDetector
    {
        private static readonly string DocXlsSignature = "D0-CF-11-E0-A1-B1-1A-E1";
        private static readonly string JpgSignature = "FF-D8-FF-E0";
        private static readonly string PngSignature = "89-50-4E-47-0D-0A-1A-0A";
        private static readonly string DocxOrXlsxSignature = "50-4B-03-04-14-00-06-00";
        private static readonly string DocSignature = "EC-A5-C1-00";
        private static readonly string ZipSignature = "50-4B-03-04";

        public static bool IsImageFile(HttpPostedFileBase file)
        {
            byte[] bytes = ConvertFileToByteArray(file);
            if (bytes.Length < 8)
            {
                return false;
            }
            string signature = GetFileSignature(bytes);
            file.InputStream.Position = 0;
            return signature.Contains(JpgSignature) || signature.Contains(PngSignature);
        }

        public static bool IsImageFile(Stream file)
        {
            byte[] bytes = ConvertFileToByteArray(file);
            if (bytes.Length < 8)
            {
                return false;
            }
            string signature = GetFileSignature(bytes);
            return signature.Contains(JpgSignature) || signature.Contains(PngSignature);
        }

        public static bool IsExcelFile(HttpPostedFileBase file)
        {
            byte[] bytes = ConvertFileToByteArray(file);
            if (bytes.Length < 8)
            {
                return false;
            }
            string signature = GetFileSignature(bytes);
            if (signature.Contains(DocXlsSignature))
            {
                if (bytes.Length < 512)
                    return false;
                byte[] signartureByte = new byte[4];
                Array.Copy(bytes, 512, signartureByte, 0, signartureByte.Length);
                signature = BitConverter.ToString(signartureByte);
                if (signature == DocSignature)
                    return false;
                return true;
            }

            if (signature.Contains(DocxOrXlsxSignature))
            {
                string fileBody = Encoding.UTF8.GetString(bytes);
                if (fileBody.Contains("word"))
                    return false;
                return true;
            }
            return false;
        }

        public static bool IsZipFile(HttpPostedFileBase file)
        {
            byte[] bytes = ConvertFileToByteArray(file);
            if (bytes.Length < 8)
            {
                return false;
            }
            string signature = GetFileSignature(bytes);
            return signature.Contains(ZipSignature);
        }

        private static string GetFileSignature(byte[] bytes)
        {
            var signartureByte = new byte[8];
            Array.Copy(bytes, signartureByte, signartureByte.Length);
            return BitConverter.ToString(signartureByte);
        }

        private static byte[] ConvertFileToByteArray(HttpPostedFileBase file)
        {
            var array = new Byte[file.ContentLength];
            file.InputStream.Position = 0;
            file.InputStream.Read(array, 0, file.ContentLength);
            return array;
        }

        private static byte[] ConvertFileToByteArray(Stream file)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                file.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
