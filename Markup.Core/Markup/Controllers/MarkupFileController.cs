using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.Web.Resources;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;
using Markup.Models;
using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace Markup.Controllers
{
    public class MarkupFileController : PartialContentController<MarkupFile>
    {
        private readonly IContentLoader contentLoader;
        private readonly ContentAssetHelper contentAssetHelper;

        public MarkupFileController(IContentLoader contentLoader, ContentAssetHelper contentAssetHelper)
        {
            this.contentLoader = contentLoader;
            this.contentAssetHelper = contentAssetHelper;
        }

        public override ActionResult Index(MarkupFile currentContent)
        {
            var assetsFolder = contentAssetHelper.GetAssetOwner(currentContent.ContentLink);

            AddScript(currentContent.Scripts);
            AddStylesheet(currentContent.Styles);

            // Loop every media asset of this block
            foreach (MediaData media in contentLoader.GetChildren<MediaData>(currentContent.ParentLink))
            {
                // Autoload any JS
                if (Path.GetExtension(media.Name) == ".js")
                {
                    AddScript(UrlResolver.Current.GetUrl(media));
                }

                // Autoload any CSS
                if (Path.GetExtension(media.Name) == ".css")
                {
                    AddStylesheet(UrlResolver.Current.GetUrl(media));
                }
            }

            return Content(currentContent.Markup);
        }


        public void AddScript(string value)
        {
            (value ?? string.Empty).Split(Environment.NewLine.ToCharArray()).ToList().ForEach(s =>
            {
                ClientResources.RequireScript(s).AtFooter();
            });
        }

        public void AddStylesheet(string value)
        {
            (value ?? string.Empty).Split(Environment.NewLine.ToCharArray()).ToList().ForEach(s =>
            {
                ClientResources.RequireStyle(s);
            });
        }
    }
}
