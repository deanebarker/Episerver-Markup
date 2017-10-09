using System.Collections.Generic;

namespace Markup.Models
{
    public interface IMarkupContent
    {
        string Markup { get; }
        string InlineStyles { get; }
        string InlineScripts { get; }
        string StylesheetReferences { get; }
        string ScriptReferences { get; }

        string GetTextOfResource(string id);

        byte[] GetBytesOfResource(string id);

        IEnumerable<string> GetResources();
    }
}