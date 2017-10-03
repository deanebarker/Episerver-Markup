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
                            return ExtractMarkup(GetTextContent(file));
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

        public string GetTextContent(string filePath, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            return encoding.GetString(GetFileContent(filePath));
        }

        public Byte[] GetFileContent(string filePath)
        {
            var entry = ZipFile.OpenRead(((FileBlob)BinaryData).FilePath).GetEntry(filePath);
            if (entry == null)
            {
                return null;
            }

            var stream = entry.Open();
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}
