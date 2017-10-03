using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Framework.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Markup.Models
{
    [ContentType(DisplayName = "Markup File", GUID = "EE3BD195-7CB0-4756-AB5F-E5E223CD9831")]
    [MediaDescriptor(ExtensionString = "html,xhtml,htm,svg")]
    public class MarkupFile : MediaData
    {
        [Display(Name = "Required Script URLs", Order = 1, GroupName = SystemTabNames.Content, Description = "URLs of script files (one per line) to be loaded, normally in the page footer.")]
        [UIHint("textarea")]
        public virtual string Scripts { get; set; }

        [Display(Name = "Required Stylesheet URLs", Order = 1, GroupName = SystemTabNames.Content, Description = "URLs of stylesheets (one per line) to be loaded, normally in the HEAD tag.")]
        [UIHint("textarea")]
        public virtual string Styles { get; set; }

        public string ExtractMarkup(string markup)
        {
            markup = Regex.Split(markup, MarkupSettings.ExtractionDelimiters.Start).Last();
            markup = Regex.Split(markup, MarkupSettings.ExtractionDelimiters.End).First();
            return markup;
        }

        public virtual string Markup
        {
            get
            {
                return ExtractMarkup(new StreamReader(BinaryData.OpenRead(), true).ReadToEnd());
            }
        }
    }
}