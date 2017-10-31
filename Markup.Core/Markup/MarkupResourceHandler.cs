using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using Markup.Events;
using Markup.Models;
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

            // Get the content
            var content = ServiceLocator.Current.GetInstance<IContentRepository>().Get<IContent>(new ContentReference(id));

            // Get the bytes of the requested resource
            var data = ((IMarkupContent)content).GetBytesOfResource(file);
            if (data == null && data.Length == 0)
            {
                // That file didn't exist
                throw NotFound();
            }

            // Output
            context.Response.ContentType = MimeMapping.GetMimeMapping(file ?? content.Name);
            switch (context.Response.ContentType)
            {
                case "text/css":
                    context.Response.Write(MarkupEventManager.OutputStylesheet(data.GetString(), file ?? content.Name, content));
                    break;

                case "application/x-javascript":
                case "application/javascript":
                    context.Response.Write(MarkupEventManager.OutputScript(data.GetString(), file ?? content.Name, content));
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