using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Markup
{
    public static class MarkupSettings
    {
        public static class ExtractionDelimiters
        {
            public const string Start = "<!--\\s?start\\s?-->";
            public const string End = "<!--\\s?end\\s?-->";
        }

        public const string ResourceHandlerUrlPattern = "/markup.resource?id={0}&file={1}";

        public static List<string> JsExtensions = new List<string>() { ".js" };
        public static List<string> CssExtensions = new List<string>() { ".css" };

        // This is NOT the extension mapping for MarkupFile. This is list of extensions that MarkupArchiveFile looks for to find its base content.
        public static List<string> MarkupExtensions = new List<string>() { ".html", ".htm" };
    }
}
