using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Framework.Blobs;
using EPiServer.Framework.DataAnnotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Markup.Models
{
    [ContentType(DisplayName = "Markup Archive File", GUID = "FE3BD195-7CB0-4756-AB5F-E5E223CD9831")]
    [MediaDescriptor(ExtensionString = "app")]
    public class MarkupArchiveFile : MarkupFile
    {
        public override string Markup
        {
            get
            {
                // I know there's a more graceful to do this...
                var baseName = Path.GetFileNameWithoutExtension(Name);
                foreach (var file in GetFiles())
                {
                    foreach (var extension in MarkupSettings.MarkupExtensions)
                    {
                        if (file.ToLower() == string.Concat(baseName, extension))
                        {
                            return ExtractMarkup(GetText(file));
                        }
                    }
                }
                return string.Empty;
            }
        }

        public List<string> GetFiles()
        {
            return ZipFile.OpenRead(((FileBlob)BinaryData).FilePath).Entries.Select(e => e.FullName).ToList();
        }

        public string GetText(string filePath)
        {
            var bytes = GetBytes(filePath);
            return GetEncoding(bytes).GetString(bytes);
        }

        public Byte[] GetBytes(string filePath)
        {
            var stream = ZipFile.OpenRead(((FileBlob)BinaryData).FilePath).GetEntry(filePath).Open();
            byte[] buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        // Taken from: https://stackoverflow.com/questions/3825390/effective-way-to-find-any-files-encoding
        private Encoding GetEncoding(byte[] bytes)
        {
            var bom = bytes.Take(5).ToArray();

            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;
            return Encoding.ASCII;
        }
    }
}