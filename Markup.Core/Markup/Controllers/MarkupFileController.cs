using EPiServer.Web.Mvc;
using Markup.Events;
using Markup.Models.Media;
using System.Web.Mvc;

namespace Markup.Controllers
{
    public class MarkupFileController : PartialContentController<MarkupFile>
    {
        public override ActionResult Index(MarkupFile currentContent)
        {
            MarkupResourceManager.ProcessStandardReferences(currentContent);
            return Content(MarkupEventManager.OutputMarkup(currentContent));
        }
    }
}