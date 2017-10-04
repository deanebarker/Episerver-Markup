using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Framework.Blobs;
using EPiServer.Framework.DataAnnotations;
using Markup.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

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
                // These are the extensions of the MediaDescriptor attribute on MarkupFile
                //var validExtensions = ((MediaDescriptorAttribute)typeof(MarkupFile).GetCustomAttributes(typeof(MediaDescriptorAttribute), true).First()).Extensions;

                // I know there's a more graceful to do this...
                var baseName = Path.GetFileNameWithoutExtension(Name);
                foreach (var file in GetFiles())
                {
                    foreach (var extension in MarkupSettings.MarkupExtensions)
                    {
                        if (file.ToLower() == string.Concat(baseName, extension))
                        {
                            var text = MarkupFileReader.GetText(this, file);
                            return MarkupEventManager.OutputMarkup(text, file);
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

        public Byte[] GetBytes(string filePath)
        {
            return MarkupFileReader.GetBytes(this, filePath);
        }
    }
}