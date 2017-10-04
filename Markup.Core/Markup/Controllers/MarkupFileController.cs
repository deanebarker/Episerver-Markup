using EPiServer;
using EPiServer.Core;
using Markup.Models;
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
            AddReference(null, currentContent.Scripts);
            AddReference(null, currentContent.Styles);

            // Loop every media item inside the same asset folder as this file
            contentLoader.GetChildren<MediaData>(currentContent.ParentLink).ToList().ForEach(media =>
            {
                AddReference(media.ContentLink, media.Name);
            });

            return Content(currentContent.Markup);
        }
    }
}