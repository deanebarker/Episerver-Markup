using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Framework.Blobs;
using EPiServer.Framework.DataAnnotations;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Markup.Models.Media
{
    [ContentType(DisplayName = "Markup Archive File", GUID = "FE3BD195-7CB0-4856-AB5F-E5E223CD9831")]
    [MediaDescriptor(ExtensionString = "app")]
    public class MarkupArchiveFile : MarkupFile, IMarkupContent
    {
        public override string Markup
        {
            get
            {
                // I know there's a more graceful to do this...
                var baseName = Path.GetFileNameWithoutExtension(Name);
                foreach (var file in GetResources())
                {
                    foreach (var extension in MarkupSettings.MarkupExtensions)
                    {
                        if (file.ToLower() == string.Concat(baseName, extension))
                        {
                            return GetTextOfResource(file);
                        }
                    }
                }
                return string.Empty;
            }
        }

        public new string GetTextOfResource(string filename)
        {
            return GetBytesOfResource(filename).GetString();
        }

        public new byte[] GetBytesOfResource(string filename)
        {
            if (filename == null)
            {
                return null;
            }

            // This a zip file. We need to extract the file bytes from it
            var blob = (FileBlob)BinaryData;
            var stream = ZipFile.OpenRead(blob.FilePath).GetEntry(filename).Open();

            byte[] buffer = new byte[16 * 1024];
            using (var memoryStream = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    memoryStream.Write(buffer, 0, read);
                }
                stream.Close();
                return memoryStream.ToArray();
            }
        }

        public new IEnumerable<string> GetResources()
        {
            var blob = (FileBlob)BinaryData;
            return ZipFile.OpenRead(blob.FilePath).Entries.Select(e => e.FullName);
        }
    }
}