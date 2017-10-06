using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Framework.DataAnnotations;
using Markup.Events;
using System.ComponentModel.DataAnnotations;

namespace Markup.Models
{
    [ContentType(DisplayName = "Markup File", GUID = "EE3BD195-7CB0-4756-AB5F-E5E223CD9831")]
    [MediaDescriptor(ExtensionString = "html,xhtml,htm,svg")]
    public class MarkupFile : MediaData
    {
        [Display(Name = "Required Script URLs", Order = 1, GroupName = SystemTabNames.Content, Description = "URLs of script files (one per line) to be loaded, normally in the page footer.")]
        [UIHint("SimpleCode")]
        public virtual string Scripts { get; set; }

        [Display(Name = "Required Stylesheet URLs", Order = 1, GroupName = SystemTabNames.Content, Description = "URLs of stylesheets (one per line) to be loaded, normally in the HEAD tag.")]
        [UIHint("SimpleCode")]
        public virtual string Styles { get; set; }

        public virtual string Markup
        {
            get
            {
                return MarkupEventManager.OutputMarkup(MarkupFileReader.GetText(this), Name);
            }
        }
    }
}