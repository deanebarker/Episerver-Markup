using EPiServer;
using EPiServer.Core;
using EPiServer.DataAnnotations;
using EPiServer.Framework.Blobs;
using EPiServer.ServiceLocation;
using Markup.Core.Markup.UI;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Markup.Models.Blocks
{
    [ContentType(DisplayName = "Markup Block", Description = "Used to insert raw markup.", GUID = "426CF12E-1F01-4EA0-922F-0778314DDAF0")]
    public class MarkupBlock : BlockData, IMarkupContent
    {
        [Display(Name = "Markup", Order = 1)]
        [UIHint("SimpleCode")]
        public virtual string Markup { get; set; }

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

        public string GetTextOfResource(string filename)
        {
            return GetBytesOfResource(filename).GetString();
        }

        public byte[] GetBytesOfResource(string filename)
        {
            return GetAssets().FirstOrDefault(a => a.Name == filename).BinaryData.ReadAllBytes();
        }

        public IEnumerable<string> GetResources()
        {
            return GetAssets().Select(a => a.Name);
        }

        private IEnumerable<MediaData> GetAssets()
        {
            var contentAssetHelper = ServiceLocator.Current.GetInstance<ContentAssetHelper>();
            var assetFolder = contentAssetHelper.GetOrCreateAssetFolder(((IContent)this).ContentLink);
            var repo = ServiceLocator.Current.GetInstance<IContentRepository>();
            return repo.GetChildren<MediaData>(assetFolder.ContentLink);
        }
    }
}