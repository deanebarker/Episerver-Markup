using EPiServer.Web.Mvc;
using Markup.Events;
using Markup.Models.Media;
using System.Web.Mvc;

namespace Markup.Controllers
{
    public class MarkupArchiveFileController : PartialContentController<MarkupArchiveFile>
    {
        public override ActionResult Index(MarkupArchiveFile currentContent)
        {
            MarkupResourceManager.ProcessStandardReferences(currentContent);
            return Content(MarkupEventManager.OutputMarkup(currentContent));
        }
    }
}