using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using Markup.Models;
using System.IO;
using System.Web;
using System.Web.Routing;

namespace Markup
{
    public class MarkupArchiveResourceHandler : IHttpHandler, IRouteHandler
    {
        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            var id = context.Request.QueryString["id"];
            var file = context.Request.QueryString["file"];

            var repo = ServiceLocator.Current.GetInstance<IContentRepository>();
            var markupArchiveFile = repo.Get<MarkupArchiveFile>(new ContentReference(id));

            context.Response.ContentType = MimeMapping.GetMimeMapping(file);
            context.Response.BinaryWrite(markupArchiveFile.GetFileContent(file));
            context.ApplicationInstance.CompleteRequest();
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return this;
        }
    }
}