using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using Markup.Models;
using System;
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
            // Get the inbound data
            if (String.IsNullOrWhiteSpace(context.Request.QueryString[MarkupSettings.HandlerArgs.Content]) || (String.IsNullOrWhiteSpace(context.Request.QueryString[MarkupSettings.HandlerArgs.File])))
            {
                throw NotFound();
            }
            var id = context.Request.QueryString[MarkupSettings.HandlerArgs.Content];
            var file = context.Request.QueryString[MarkupSettings.HandlerArgs.File];

            // Get the content of the file
            var repo = ServiceLocator.Current.GetInstance<IContentRepository>();
            Byte[] content;
            try
            {
                var markupArchiveFile = repo.Get<MarkupArchiveFile>(new ContentReference(id));
                content = markupArchiveFile.GetBytes(file);
            }
            catch (Exception e)
            {
                throw NotFound();
            }

            // Output
            context.Response.ContentType = MimeMapping.GetMimeMapping(file);
            context.Response.BinaryWrite(content);
            context.ApplicationInstance.CompleteRequest();
        }

        private HttpException NotFound()
        {
            return new HttpException(404, "Not Found");
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return this;
        }
    }
}