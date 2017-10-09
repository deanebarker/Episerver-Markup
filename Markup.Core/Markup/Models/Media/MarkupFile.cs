using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Framework.Blobs;
using EPiServer.Framework.DataAnnotations;
using EPiServer.ServiceLocation;
using Markup.Core.Markup.UI;
using Markup.Events;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Markup.Models.Media
{
    [ContentType(DisplayName = "Markup File", GUID = "EE3BD195-7CB0-4756-AB5F-E5E224CD9831")]
    [MediaDescriptor(ExtensionString = "html,xhtml,htm,svg")]
    public class MarkupFile : MediaData, IMarkupContent
    {
        [Display(Name = "Inline Styles", Order = 2)]
        [UIHint("SimpleCode")]
        public virtual string InlineStyles { get; set; }

        [Display(Name = "Inline Script", Order = 3)]
        [UIHint("SimpleCode")]
        public virtual string InlineScripts { get; set; }

        [Display(Name = "Stylesheet References", Order = 4)]
        [UIHint("SimpleCode")]
        [SimpleCodeEditorSettings(MinHeight = MarkupSettings.SimpleCodeSettings.ReferencesMinHeight)]
        public virtual string StylesheetReferences { get; set; }

        [Display(Name = "Script References", Order = 5)]
        [UIHint("SimpleCode")]
        [SimpleCodeEditorSettings(MinHeight = MarkupSettings.SimpleCodeSettings.ReferencesMinHeight)]
        public virtual string ScriptReferences { get; set; }

        public virtual string Markup
        {
            get
            {
                // A null filename should return the content of the file itself
                return GetTextOfResource(null);
            }
        }

        public byte[] GetBytesOfResource(string filename = null)
        {
            // If no filename was passed in, then we'll get the content of the file itself
            if (filename == null)
            {
                return ((FileBlob)BinaryData).ReadAllBytes();
            }

            var repo = ServiceLocator.Current.GetInstance<IContentRepository>();
            var content = repo.GetChildren<MediaData>(ParentLink).FirstOrDefault(c => c.Name == filename);
            if (content == null)
            {
                return null;
            }
            return content.BinaryData.ReadAllBytes();
        }

        public string GetTextOfResource(string filename = null)
        {
            return GetBytesOfResource(filename).GetString();
        }

        public IEnumerable<string> GetResources()
        {
            var repo = ServiceLocator.Current.GetInstance<IContentRepository>();
            return repo.GetChildren<MediaData>(ParentLink).Select(c => c.Name);
        }
    }
}