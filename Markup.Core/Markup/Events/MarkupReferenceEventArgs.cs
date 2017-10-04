using EPiServer.Core;
using System;

namespace Markup.Events
{
    public class MarkupReferenceEventArgs : EventArgs
    {
        public ContentReference ContentLink { get; set; }
        public string Path { get; set; }
        public bool Cancel { get; set; }

        public MarkupReferenceEventArgs(ContentReference contentLink, string path)
        {
            ContentLink = contentLink;
            Path = path;
        }
    }
}