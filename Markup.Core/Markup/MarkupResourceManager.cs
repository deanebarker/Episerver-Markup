using EPiServer.Core;
using EPiServer.Framework.Web.Resources;
using Markup.Events;
using Markup.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Markup
{
    public static class MarkupResourceManager
    {
        public static void ProcessStandardReferences(IMarkupContent content)
        {
            // Add each line of the script references
            GetLines(content.ScriptReferences).ForEach(u =>
            {
                AddScriptReference((IContent)content, u);
            });

            // Add each line of the style references
            GetLines(content.StylesheetReferences).ForEach(u =>
            {
                AddStylesheetReference((IContent)content, u);
            });

            // Add the inlines
            AddInlineScript((IContent)content, content.InlineScripts);
            AddInlineStyles((IContent)content, content.InlineStyles);

            // Add the resoureces
            content.GetResources().ToList().ForEach(r =>
            {
                AddReference((IContent)content, r);
            });
        }

        // This is for when we don't know what the reference might be too
        public static void AddReference(IContent content, string path)
        {
            if (MarkupSettings.JsExtensions.Contains(Path.GetExtension(path)))
            {
                AddScriptReference(content, path);
            }

            if (MarkupSettings.CssExtensions.Contains(Path.GetExtension(path)))
            {
                AddStylesheetReference(content, path);
            }
        }

        public static void AddStylesheetReference(IContent content, string path)
        {
            if (!MarkupEventManager.EvaluateReference(content.ContentLink, path))
            {
                ClientResources.RequireStyle(ResolveReferencePath(content, path));
            }
        }

        public static void AddScriptReference(IContent content, string path)
        {
            if (!MarkupEventManager.EvaluateReference(content.ContentLink, path))
            {
                ClientResources.RequireScript(ResolveReferencePath(content, path)).AtFooter();
            }
        }

        public static void AddInlineStyles(IContent content, string css)
        {
            if (String.IsNullOrWhiteSpace(css))
            {
                return;
            }
            css = MarkupEventManager.OutputStylesheet(css, null, content);
            ClientResources.RequireStyleInline(css);
        }

        public static void AddInlineScript(IContent content, string script)
        {
            if (String.IsNullOrWhiteSpace(script))
            {
                return;
            }
            script = MarkupEventManager.OutputScript(script, null, content);
            ClientResources.RequireScriptInline(script);
        }

        private static string ResolveReferencePath(IContent content, string path)
        {
            if(path.StartsWith("http"))
            {
                // This would be an absolute URL
                return path;
            }

            if(path.StartsWith("/"))
            {
                // This would be a relative URL to this same site (which is legal, though...rare?)
                return path;
            }

            if (!ContentReference.IsNullOrEmpty(content.ContentLink))
            {
                var builder = new UriBuilder()
                {
                    Path = MarkupSettings.ResourceHandlerUrl
                };
                var query = HttpUtility.ParseQueryString(builder.Query);
                query[MarkupSettings.HandlerArgs.Content] = content.ContentLink.ID.ToString();
                query[MarkupSettings.HandlerArgs.File] = path;
                builder.Query = query.ToString();

                path = builder.Uri.PathAndQuery;
            }
            return path;
        }

        private static List<string> GetLines(string value)
        {
            return (value ?? string.Empty)
                .Split(Environment.NewLine.ToCharArray())
                .Where(l => !String.IsNullOrWhiteSpace(l))
                .ToList();
        }
    }
}