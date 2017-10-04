using System;

namespace Markup.Events
{
    public class MarkupEventArgs : EventArgs
    {
        public string FileName { get; set; }
        public Byte[] Bytes { get; set; }
        public string Text { get; set; }

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