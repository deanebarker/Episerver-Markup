using EPiServer.Core;
using Markup.Models;
using System;
using System.Text;

namespace Markup.Events
{
    public class MarkupEventArgs : EventArgs
    {
        public string FileName { get; set; }
        public Byte[] Bytes { get; set; }
        public string Text { get; set; }
        public Encoding Encoding { get; set; }
        public IContent ContentData { get; set; } 

        public MarkupEventArgs()
        {
        }

        public MarkupEventArgs(string filename, byte[] bytes, string text)
        {
            FileName = filename;
            Bytes = bytes;
            Text = text;
        }
    }
}