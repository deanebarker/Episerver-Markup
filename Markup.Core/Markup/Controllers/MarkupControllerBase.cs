using EPiServer.Core;
using EPiServer.Framework.Web.Resources;
using EPiServer.Web.Mvc;
using System;
using System.Linq;

namespace Markup.Controllers
{
    public abstract class MarkupControllerBase<T> : PartialContentController<T> where T : IContentData
    {
        protected void AddReference(string extension, string path)
        {
            // Autoload any JS
            if (MarkupSettings.JsExtensions.Contains(extension))
            {
                AddScript(path);
            }

            // Autoload any CSS
            if (MarkupSettings.CssExtensions.Contains(extension))
            {
                AddStylesheet((path));
            }
        }

        protected void AddScript(string value)
        {
            (value ?? string.Empty).Split(Environment.NewLine.ToCharArray()).ToList().ForEach(s =>
            {
                ClientResources.RequireScript(s).AtFooter();
            });
        }

        protected void AddStylesheet(string value)
        {
            (value ?? string.Empty).Split(Environment.NewLine.ToCharArray()).ToList().ForEach(s =>
            {
                ClientResources.RequireStyle(s);
            });
        }
    }
}