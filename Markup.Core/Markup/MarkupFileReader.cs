using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.Blobs;
using EPiServer.ServiceLocation;
using Markup.Events;
using Markup.Models;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Markup
{
    public static class MarkupFileReader
    {
        private static IContentRepository repo = ServiceLocator.Current.GetInstance<IContentRepository>();
        private static IContentLoader loader = ServiceLocator.Current.GetInstance<IContentLoader>();
        private static ContentAssetHelper assetHelper = ServiceLocator.Current.GetInstance<ContentAssetHelper>();

        public static event Action<object, MarkupEventArgs> OnAfterFileRead = delegate { };

        public static Byte[] GetBytes(IContent content, string fileName = null)
        {
            byte[] bytes = null;

            if (fileName == null && !(content is MediaData))
            {
                // There is no filename, and this is not a media item
                // We got nothing here
                return null;
            }

            if (content is MarkupArchiveFile)
            {
                // This a zip file. We need to extract the file bytes from it
                var blob = ((FileBlob)((MediaData)content).BinaryData);
                var stream = ZipFile.OpenRead(blob.FilePath).GetEntry(fileName).Open();

                byte[] buffer = new byte[16 * 1024];
                using (var memoryStream = new MemoryStream())
                {
                    int read;
                    while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        memoryStream.Write(buffer, 0, read);
                    }
                    stream.Close();
                    return EvaluateFileRead(content.Name, memoryStream.ToArray());
                }
            }

            if (content is MediaData)
            {
                // This is an asset, but NOT a zip file
                // This is our easiest case
                var blob = (FileBlob)((MediaData)content).BinaryData;
                return EvaluateFileRead(content.Name, GetBytesFromBlob(blob));
            }

            // We have a filename, and this content is not a media item, so we need to look in the asset folder for the content
            var assetFolder = assetHelper.GetOrCreateAssetFolder(content.ContentLink).ContentLink;
            foreach (var item in repo.GetChildren<MediaData>(assetFolder))
            {
                if (item.Name == fileName)
                {
                    return EvaluateFileRead(content.Name, GetBytesFromBlob((FileBlob)item.BinaryData));
                }
            }

            return null;
        }

        private static byte[] EvaluateFileRead(string filename, byte[] bytes)
        {
            var e = new MarkupEventArgs(filename, bytes, null);
            e.Encoding = GetEncoding(bytes);
            MarkupEventManager.EvaluateFileContents(e);
            return e.Bytes;
        }

        public static Byte[] GetBytes(ContentReference contentLink, string fileName = null)
        {
            var content = repo.Get<IContent>(contentLink);
            return GetBytes(content, fileName);
        }

        public static string GetText(ContentReference contentLink, string fileName = null)
        {
            var bytes = GetBytes(contentLink, fileName);
            return GetEncoding(bytes).GetString(bytes);
        }

        public static string GetText(IContent content, string fileName = null)
        {
            var bytes = GetBytes(content, fileName);
            return GetText(bytes);
        }

        public static string GetText(Byte[] bytes)
        {
            return GetEncoding(bytes).GetString(bytes);
        }

        private static Byte[] GetBytesFromBlob(FileBlob binaryData)
        {
            var fileStream = new FileStream(binaryData.FilePath, FileMode.Open);
            byte[] buffer = new byte[16 * 1024];
            using (var memoryStream = new MemoryStream())
            {
                int read;
                while ((read = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    memoryStream.Write(buffer, 0, read);
                }
                fileStream.Close();
                return memoryStream.ToArray();
            }
        }

        // Taken from: https://stackoverflow.com/questions/3825390/effective-way-to-find-any-files-encoding
        private static Encoding GetEncoding(byte[] bytes)
        {
            var bom = bytes.Take(5).ToArray();
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;
            return Encoding.ASCII;
        }
    }
}