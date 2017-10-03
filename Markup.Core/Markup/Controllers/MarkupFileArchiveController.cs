using Markup.Models;
using System.IO;
using System.Web.Mvc;

namespace Markup.Controllers
{
    public class MarkupFileArchiveController : MarkupControllerBase<MarkupArchiveFile>
    {
        public override ActionResult Index(MarkupArchiveFile currentContent)
        {
            AddScript(currentContent.Scripts);
            AddStylesheet(currentContent.Styles);

            // Add references for all the other files in the zip archive
            currentContent.GetFiles().ForEach(file =>
            {
                var extension = Path.GetExtension(file);
                var path = GetSupportingFileUrl(currentContent.ContentLink.ID, file);
                AddReference(extension, path);
            });

            return Content(currentContent.Markup);
        }

        private string GetSupportingFileUrl(int id, string fileName)
        {
            return string.Format(MarkupSettings.ResourceHandlerUrlPattern, id, fileName);
        }
    }
}