using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IniParser;

namespace ifme
{
    public class Get
    {
        public static string AttachmentValid(string file)
        {
            FileInfo f = new FileInfo(file);

            if (f.Length >= 1073741824)
                return "application/octet-stream";

            byte[] data = File.ReadAllBytes(file);
            byte[] MagicTTF = { 0x00, 0x01, 0x00, 0x00, 0x00 };
            byte[] MagicOTF = { 0x4F, 0x54, 0x54, 0x4F, 0x00 };
            byte[] MagicWOFF = { 0x77, 0x4F, 0x46, 0x46, 0x00 };
            byte[] check = new byte[5];

            Buffer.BlockCopy(data, 0, check, 0, 5);

            if (MagicTTF.SequenceEqual(check))
                return "application/x-truetype-font";

            if (MagicOTF.SequenceEqual(check))
                return "application/vnd.ms-opentype";

            if (MagicWOFF.SequenceEqual(check))
                return "application/font-woff";

            return "application/octet-stream";
        }

        public static string CodecFormat(string codecId)
        {
            var getFmt = new FileIniDataParser().ReadFile("format.ini", Encoding.UTF8);
            var format = string.Empty;

            try
            {
                format = getFmt["format"][codecId];
            }
            catch (Exception)
            {
                Console.WriteLine("Requested file container not found, using default");
            }
            finally
            {
                if (string.IsNullOrEmpty(format))
                    format = codecId;
            }

            return format;
        }

        public static string FileExtension(string filePath)
        {
            return Path.GetExtension(filePath).Substring(1);
        }
    }
}
