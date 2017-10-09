using EPiServer.Web.Mvc;
using Markup.Events;
using Markup.Models.Blocks;
using System.Web.Mvc;

namespace Markup.Controllers
{
    public class MarkupBlockController : BlockController<MarkupBlock>
    {
        public override ActionResult Index(MarkupBlock currentBlock)
        {
            MarkupResourceManager.ProcessStandardReferences(currentBlock);

            return Content(MarkupEventManager.OutputMarkup(currentBlock));
        }
    }
}