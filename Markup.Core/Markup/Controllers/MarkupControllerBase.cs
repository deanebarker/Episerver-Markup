using EPiServer.Core;
using EPiServer.Framework.Web.Resources;
using EPiServer.Web.Mvc;
using Markup.Events;
using System;
using System.IO;

namespace Markup.Controllers
{
    public abstract class MarkupControllerBase<T> : PartialContentController<T> where T : IContentData
    {
        protected void AddReference(ContentReference contentLink, string paths)
        {
            if (String.IsNullOrWhiteSpace(paths))
            {
                return;
            }

            foreach (var path in paths.Split(Environment.NewLine.ToCharArray()))
            {
                if (!MarkupEventManager.EvaluateReference(contentLink, path))
                {
                    var resolvedPath = path;
                    if (!ContentReference.IsNullOrEmpty(contentLink))
                    {
                        resolvedPath = string.Format(MarkupSettings.ResourceHandlerUrlPattern, contentLink, path);
                    }

                    if (MarkupSettings.JsExtensions.Contains(Path.GetExtension(path)))
                    {
                        ClientResources.RequireScript(resolvedPath).AtFooter();
                    }

                    if (MarkupSettings.CssExtensions.Contains(Path.GetExtension(path)))
                    {
                        ClientResources.RequireStyle(resolvedPath);
                    }
                }
            }
        }
    }
}