using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace Markup
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class MarkupInit : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {

            RouteTable.Routes.Add(new Route
            (
                "resource.app",
                new MarkupArchiveResourceHandler()
            ));
        }

        public void Uninitialize(InitializationEngine context)
        {
            //throw new NotImplementedException();
        }
    }
}