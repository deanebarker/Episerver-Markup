using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.Web.Resources;
using EPiServer.Web.Mvc;
using Markup.Models;
using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace Markup.Controllers
{
    public class MarkupFileArchiveController : PartialContentController<MarkupArchiveFile>
    {


        private readonly IContentLoader contentLoader;
        private readonly ContentAssetHelper contentAssetHelper;

        public MarkupFileArchiveController(IContentLoader contentLoader, ContentAssetHelper contentAssetHelper)
        {
            this.contentLoader = contentLoader;
            this.contentAssetHelper = contentAssetHelper;
        }

        public override ActionResult Index(MarkupArchiveFile currentContent)
        {
            AddScript(currentContent.Scripts);
            AddStylesheet(currentContent.Styles);

            // Loop every media asset of this block
            foreach (var file in currentContent.GetFiles())
            {
                // Autoload any JS
                if (MarkupSettings.JsExtensions.Contains(Path.GetExtension(file)))
                {
                    AddScript(GetSupportingFileUrl(currentContent.ContentLink.ID, file));
                }

                // Autoload any CSS
                if (MarkupSettings.CssExtensions.Contains(Path.GetExtension(file)))
                {
                    AddStylesheet(GetSupportingFileUrl(currentContent.ContentLink.ID, file));
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

        private string GetSupportingFileUrl(int id, string fileName)
        {
            return string.Format(MarkupSettings.ResourceHandlerUrlPattern, id, fileName);
        }

    }
}
