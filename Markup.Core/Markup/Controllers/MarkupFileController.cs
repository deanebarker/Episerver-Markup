using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.Blobs;
using EPiServer.Web.Routing;
using Markup.Models;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace Markup.Controllers
{
    public class MarkupFileController : MarkupControllerBase<MarkupFile>
    {
        private readonly IContentLoader contentLoader;

        public MarkupFileController(IContentLoader contentLoader)
        {
            this.contentLoader = contentLoader;
        }

        public override ActionResult Index(MarkupFile currentContent)
        {
            AddScript(currentContent.Scripts);
            AddStylesheet(currentContent.Styles);

            // Loop every media asset of this block
            contentLoader.GetChildren<MediaData>(currentContent.ParentLink).ToList().ForEach(media =>
            {
                var extension = Path.GetExtension(((FileBlob)media.BinaryData).FilePath);
                var path = UrlResolver.Current.GetUrl(media);
                AddReference(extension, path);
            });

            return Content(currentContent.Markup);
        }
    }
}