using Markup.Models;
using System.Web.Mvc;

namespace Markup.Controllers
{
    public class MarkupFileArchiveController : MarkupControllerBase<MarkupArchiveFile>
    {
        public override ActionResult Index(MarkupArchiveFile currentContent)
        {
            AddReference(null, currentContent.Scripts);
            AddReference(null, currentContent.Styles);

            // Add references for all the other files in the zip archive
            currentContent.GetFiles().ForEach(file =>
            {
                AddReference(currentContent.ContentLink, file);
            });

            return Content(currentContent.Markup);
        }
    }
}