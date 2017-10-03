using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Framework.Blobs;
using EPiServer.Framework.DataAnnotations;
using EPiServer.ServiceLocation;
using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.IO.Compression;
using System.IO;
using System.Collections.Generic;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using System.Web;
using EPiServer;
using System.Web.Routing;
using System.Text;

namespace Markup.Models
{
    [ContentType(GUID = "FE3BD195-7CB0-4756-AB5F-E5E223CD9831")]
    [MediaDescriptor(ExtensionString = "app,htmla")]
    public class MarkupArchiveFile : MarkupFile
    {
        public override string Markup
        {
            get
            {
                var name = Path.GetFileNameWithoutExtension(Name);
                return ExtractHtml(GetTextContent(string.Concat(name, ".html")));
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
