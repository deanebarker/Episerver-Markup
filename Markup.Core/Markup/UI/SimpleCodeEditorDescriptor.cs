using EPiServer.Shell.ObjectEditing;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Markup.Core.Markup.UI
{
    [EditorDescriptorRegistration(TargetType = typeof(String), UIHint = "SimpleCode")]
    public class CodeEditorDescriptor : EditorDescriptor
    {
        public CodeEditorDescriptor()
        {
            ClientEditingClass = "dijit/form/Textarea";
        }

        public override void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
        {
            base.ModifyMetadata(metadata, attributes);
            SetEditorAttribute(metadata, "style", style);
            SetEditorAttribute(metadata, "spellcheck", spellcheck);
            SetEditorAttribute(metadata, "onkeydown", onKeyDown);
            SetEditorAttribute(metadata, "oninput", onInput);
        }

        private void SetEditorAttribute(ExtendedMetadata metadata, string attributeName, string value)
        {
            if (metadata.EditorConfiguration.ContainsKey(attributeName))
            {
                metadata.EditorConfiguration.Remove(attributeName);
            }
            metadata.EditorConfiguration.Add(attributeName, Condense(value));
        }

        // Yes, yes, there are dedicated JS/CSS minifier libraries. But they all require external
        // dependencies, and I didn't see value in forcing you to do that for something this trivial.
        private string Condense(string value)
        {
            value = value.Trim();
            value = value.Replace(Environment.NewLine, " ");
            value = Regex.Replace(value, @";\s+", "; ");
            value = Regex.Replace(value, @"{\s+", "{ ");
            value = Regex.Replace(value, @"\s+}", " }");
            return value;
        }

        private string spellcheck = "false";

        private string style = @"
            width: 100%;
			min-height: 200px; /* needs to be in px */
			max-height: 1000px; /* needs to be in px */
            font-family: monospace;
            padding: 10px;
            font-size: 12px;
            line-height: 16px;
            border-radius: 0;
            background-color: rgb(250,250,250);
			white-space: pre;";

        private string onKeyDown = @"
            var tabReplacement = '    ';
            var keyCode = event.keyCode || event.which;
            if (keyCode == 9) {
                event.preventDefault();
                var start = this.selectionStart;
                var end = this.selectionEnd;
                this.value = this.value.substring(0, start) + tabReplacement + this.value.substring(end);
                this.selectionEnd = start + tabReplacement.length;
            }";

        private string onInput = @"
			var maxHeight = this.style.maxHeight.replace('px','');
            var minHeight = this.style.minHeight.replace('px','');
			var newHeight = Math.max(this.scrollHeight, minHeight);
			newHeight = Math.min(newHeight, maxHeight);
            this.style.height = newHeight + 'px';
            ";
    }
}