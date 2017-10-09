using EPiServer.Core;
using EPiServer.DataAnnotations;
using Markup.Core.Markup.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        public string GetTextOfResource(string property)
        {
            // This should never be called
            throw new NotImplementedException();
        }

        public byte[] GetBytesOfResource(string property)
        {
            // This should never be called
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetResources()
        {
            // We don't return anything here since the "resources" are added inline above. Put
            // another way, there are no "resources," so returning nothing here is correct.
            return new List<string>();
        }
    }
}