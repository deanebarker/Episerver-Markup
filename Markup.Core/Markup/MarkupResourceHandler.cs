using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using Markup.Events;
using System;
using System.Web;
using System.Web.Routing;

namespace Markup
{
    public class MarkupResourceHandler : IHttpHandler, IRouteHandler
    {
        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            // Get the inbound data
            if (String.IsNullOrWhiteSpace(context.Request.QueryString[MarkupSettings.HandlerArgs.Content]))
            {
                throw NotFound();
            }
            var id = context.Request.QueryString[MarkupSettings.HandlerArgs.Content];

            String file = null;
            if (!String.IsNullOrWhiteSpace(context.Request.QueryString[MarkupSettings.HandlerArgs.File]))
            {
                file = context.Request.QueryString[MarkupSettings.HandlerArgs.File];
            }

            // Get the content of the file
            var content = ServiceLocator.Current.GetInstance<IContentRepository>().Get<IContent>(new ContentReference(id));
            var data = MarkupFileReader.GetBytes(new ContentReference(id), file);
            if (content == null)
            {
                throw NotFound();
            }

            // Output
            context.Response.ContentType = MimeMapping.GetMimeMapping(file ?? content.Name);
            switch (context.Response.ContentType)
            {
                case "text/css":
                    var css = MarkupFileReader.GetText(data);
                    context.Response.Write(MarkupEventManager.OutputStylesheet(css, file ?? content.Name));
                    break;

                case "application/x-javascript":
                case "application/javascript":
                    var js = MarkupFileReader.GetText(data);
                    context.Response.Write(MarkupEventManager.OutputScript(js, file ?? content.Name));
                    break;

                default:
                    context.Response.BinaryWrite(data);
                    break;
            }

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