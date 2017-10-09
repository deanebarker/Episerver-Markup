using EPiServer.Core;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Markup.Core.Markup.UI
{
    [EditorDescriptorRegistration(TargetType = typeof(String), UIHint = "SimpleCode")]
    [EditorDescriptorRegistration(TargetType = typeof(String), UIHint = "SimpleCodeSmall")]
    public class SimpleCodeEditorDescriptor : EditorDescriptor
    {
        public SimpleCodeEditorDescriptor()
        {
            ClientEditingClass = "dijit/form/Textarea";
        }

        public override void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
        {
            dynamic contentMetadata = metadata;
            var ownerContent = contentMetadata.OwnerContent;
            var settings = GetSettings(ownerContent, metadata.PropertyName);

            base.ModifyMetadata(metadata, attributes);
            SetEditorAttribute(metadata, "style", string.Format(style, settings.MinHeight, settings.MaxHeight));
            SetEditorAttribute(metadata, "spellcheck", spellcheck);
            SetEditorAttribute(metadata, "onkeydown", string.Format(onKeyDown, settings.TabReplacement));
            SetEditorAttribute(metadata, "oninput", onInput);
            SetEditorAttribute(metadata, "onkeyup", onKeyUp);
        }

        private SimpleCodeEditorSettingsAttribute GetSettings(IContent content, string propertyName)
        {
            // There HAS to be a better way of doing this...
            var type = content.GetType();
            if (type.Name.EndsWith("Proxy"))
            {
                type = type.BaseType;
            }

            var property = type.GetProperty(propertyName);
            var attribute = property.GetCustomAttributes(typeof(SimpleCodeEditorSettingsAttribute), true).FirstOrDefault();
            return (SimpleCodeEditorSettingsAttribute)attribute ?? new SimpleCodeEditorSettingsAttribute();
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
            value = Regex.Replace(value, @"}\s+", "} ");
            value = Regex.Replace(value, @"\s+{", " {");
            return value;
        }

        private string spellcheck = "false";

        private string style = @"
            position: relative;
			min-height: {0}px;
			max-height: {1}px;
            font-family: monospace;
            padding: 10px;
            font-size: 12px;
            line-height: 16px;
            border-radius: 0;
            background-color: rgb(250,250,250);
			white-space: pre;";

        private string onKeyDown = @"
            var tabReplacement = '{0}';
            var keyCode = event.keyCode || event.which;
            if (keyCode == 9) {{
                event.preventDefault();
                var start = this.selectionStart;
                var end = this.selectionEnd;
                this.value = this.value.substring(0, start) + tabReplacement + this.value.substring(end);
                this.selectionEnd = start + tabReplacement.length;
            }}";

        private string onKeyUp = @"
			var keyCode = event.keyCode || event.which;
			if(keyCode == 13)
			{
                var start = this.selectionStart;
				var lines = this.value.substring(0, start).split(/\r?\n/);
				if(lines.length == 1)
				{
					return;
				}
				var lastLine = lines[lines.length - 2];
				var leadingSpaces = lastLine.substring(0, lastLine.search(/\S|$/));
                var end = this.selectionEnd;
                this.value = this.value.substring(0, start) + leadingSpaces + this.value.substring(end);
                this.selectionEnd = start + leadingSpaces.length;
			}";

        private string onInput = @"
			var maxHeight = this.style.maxHeight.replace('px','');
            var minHeight = this.style.minHeight.replace('px','');
			var newHeight = Math.max(this.scrollHeight, minHeight);
			newHeight = Math.min(newHeight, maxHeight);
            this.style.height = newHeight + 'px';
            ";
    }

    public class SimpleCodeEditorSettingsAttribute : Attribute
    {
        public string TabReplacement { get; set; }
        public int MinHeight { get; set; }
        public int MaxHeight { get; set; }

        public SimpleCodeEditorSettingsAttribute()
        {
            TabReplacement = "    ";
            MinHeight = 300;
            MaxHeight = 1000;
        }
    }
}