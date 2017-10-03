using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Framework.DataAnnotations;
using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using EPiServer.Framework.Web.Resources;

namespace Markup.Models
{
    [ContentType(GUID = "EE3BD195-7CB0-4756-AB5F-E5E223CD9831")]
    [MediaDescriptor(ExtensionString = "html,htm")]
    public class MarkupFile : MediaData
    {
        private static string START_RENDER_KEY = "start";
        private static string END_RENDER_KEY = "end";

        [Display(Order = 1, GroupName = SystemTabNames.Content)]
        [UIHint("textarea")]
        public virtual string Scripts { get; set; }

        [Display(Order = 1, GroupName = SystemTabNames.Content)]
        [UIHint("textarea")]
        public virtual string Styles { get; set; }

        // These are used statically by AppFile as well, to get the HTML out of the zip

        public string ExtractHtml(string html)
        {
            html = Regex.Split(html, string.Concat("<!--\\s?", START_RENDER_KEY, "\\s?-->")).Last();
            html = Regex.Split(html, string.Concat("<!--\\s?", END_RENDER_KEY, "\\s?-->")).First();
            return html;
        }

        public virtual string Markup
        {
            get
            {
                return ExtractHtml(new StreamReader(BinaryData.OpenRead()).ReadToEnd());
            }
        }
    }

   
}
