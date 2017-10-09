using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using Markup.Events;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Routing;

namespace Markup
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class MarkupInit : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            // This should accurately pull the path from the pattern...
            var handlerPath = MarkupSettings.ResourceHandlerUrl;

            RouteTable.Routes.Add(new Route
            (
                handlerPath,
                new MarkupResourceHandler()
            ));

            // This is the default behavior, to extract HTML from between comments
            MarkupEventManager.OnBeforeOutputMarkup += (sender, e) =>
            {
                if (e.Text != null)
                {
                    e.Text = Regex.Split(e.Text, MarkupSettings.ExtractionDelimiters.Start).Last();
                    e.Text = Regex.Split(e.Text, MarkupSettings.ExtractionDelimiters.End).First();
                }
            };
        }

        public void Uninitialize(InitializationEngine context)
        {
        }
    }
}