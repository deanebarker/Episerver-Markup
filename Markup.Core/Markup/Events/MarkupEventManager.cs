using EPiServer.Core;
using System;

namespace Markup.Events
{
    public static class MarkupEventManager
    {
        public static event Action<object, MarkupEventArgs> OnBeforeOutputMarkup = delegate { };

        public static event Action<object, MarkupReferenceEventArgs> OnBeforeAddReference = delegate { };

        public static event Action<object, MarkupEventArgs> OnAfterFileRead = delegate { };

        public static event Action<object, MarkupEventArgs> OnBeforeOutputScript = delegate { };

        public static event Action<object, MarkupEventArgs> OnBeforeOutputStylesheet = delegate { };

        public static string OutputScript(string text, string fileName)
        {
            var e = new MarkupEventArgs(fileName, null, text);
            OnBeforeOutputScript(null, e);
            return e.Text;
        }

        public static string OutputStylesheet(string text, string fileName)
        {
            var e = new MarkupEventArgs(fileName, null, text);
            OnBeforeOutputStylesheet(null, e);
            return e.Text;
        }

        public static string OutputMarkup(string text, string fileName = null)
        {
            var e = new MarkupEventArgs(fileName, null, text);
            OnBeforeOutputMarkup(null, e);
            return e.Text;
        }

        public static bool EvaluateReference(ContentReference contentLink, string path)
        {
            var e = new MarkupReferenceEventArgs(contentLink, path);
            OnBeforeAddReference(null, e);
            return e.Cancel; // If this is NULL, that means "do not add this reference"
        }

        public static void EvaluateFileContents(MarkupEventArgs e)
        {
            OnAfterFileRead(null, e);
        }
    }
}