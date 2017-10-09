using EPiServer.Core;
using Markup.Models;
using System;
using System.IO;

namespace Markup.Events
{
    public static class MarkupEventManager
    {
        public static event Action<object, MarkupEventArgs> OnBeforeOutputMarkup = delegate { };

        public static event Action<object, MarkupReferenceEventArgs> OnBeforeAddReference = delegate { };

        public static event Action<object, MarkupEventArgs> OnBeforeOutputScript = delegate { };

        public static event Action<object, MarkupEventArgs> OnBeforeOutputStylesheet = delegate { };

        public static string OutputScript(string text, string fileName, IContent contentData = null)
        {
            var e = new MarkupEventArgs(fileName, null, text);
            e.ContentData = contentData;
            OnBeforeOutputScript(null, e);
            return e.Text;
        }

        public static string OutputStylesheet(string text, string fileName, IContent contentData = null)
        {
            var e = new MarkupEventArgs(fileName, null, text);
            e.ContentData = contentData;
            OnBeforeOutputStylesheet(null, e);
            return e.Text;
        }

        public static string OutputMarkup(IMarkupContent content)
        {
            var filename = ((IContent)content).Name;
            if (String.IsNullOrWhiteSpace(Path.GetExtension(filename)))
            {
                filename = string.Concat(filename, ".html");
            }
            var e = new MarkupEventArgs(filename, null, content.Markup);
            e.ContentData = (IContent)content;
            OnBeforeOutputMarkup(null, e);
            return e.Text;
        }

        public static bool EvaluateReference(ContentReference contentLink, string path)
        {
            var e = new MarkupReferenceEventArgs(contentLink, path);
            OnBeforeAddReference(null, e);
            return e.Cancel;
        }
    }
}