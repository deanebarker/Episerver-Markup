using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using System.Linq;
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
            var handlerPath = MarkupSettings.ResourceHandlerUrlPattern.Split('?').First().Trim(@"\/".ToCharArray());

            RouteTable.Routes.Add(new Route
            (
                handlerPath,
                new MarkupArchiveResourceHandler()
            ));
        }

        public void Uninitialize(InitializationEngine context)
        {
        }
    }
}