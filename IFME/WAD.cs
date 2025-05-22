using System.IO;
using System.Drawing;
using System.IO.Compression;
using System.IO.MemoryMappedFiles;

namespace IFME
{
    internal class WAD
    {
        internal static WAD Resource = new(AppPath.Combine("resources.wad"));
        internal static WAD Jason = new(AppPath.Combine("jason.wad"));

        MemoryMappedFile mmf;
        ZipArchive zipArchive;

        internal WAD(string pakFilePath)
        {
            mmf = MemoryMappedFile.CreateFromFile(pakFilePath, FileMode.Open, null, 0, MemoryMappedFileAccess.Read);
            var stream = mmf.CreateViewStream(0, 0, MemoryMappedFileAccess.Read);
            zipArchive = new ZipArchive(stream, ZipArchiveMode.Read);
        }

        internal Image LoadImage(string imageName)
        {
            var entry = zipArchive.GetEntry(imageName);
            if (entry != null)
            {
                using var stream = entry.Open();   // Open the entry stream
                using var ms = new MemoryStream(); // Copy to memory stream to avoid stream disposal issues
                stream.CopyTo(ms);
                ms.Position = 0;
                return Image.FromStream(ms);
            }
            return null;
        }

        internal string LoadText(string textName)
        {
            var entry = zipArchive.GetEntry(textName);
            if (entry != null)
            {
                using var stream = entry.Open();
                using var ms = new MemoryStream(); // Copy to memory stream to avoid stream disposal issues
                stream.CopyTo(ms);
                ms.Position = 0;
                return new StreamReader(ms).ReadToEnd();
            }
            return null;
        }

        internal void Free()
        {
            zipArchive?.Dispose();
            mmf?.Dispose();
        }
    }
}