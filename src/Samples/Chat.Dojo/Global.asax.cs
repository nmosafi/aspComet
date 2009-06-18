using System;
using System.Web;

namespace AspComet.WebHost
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            //
            //    If you use a container such as Windsor, you can do something akin to this once you 
            //    have set up your container - the only dependency is currently the message bus:
            //
            // Configuration.InitialiseHttpHandler.WithMessageBus(container.Get<IMessageBus>);
            //
            //    Otherwise, this is useful - it just gives a new message bus with the defualt configuration
            //
            Configuration.InitialiseHttpHandler.WithTheDefaultConfiguration();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}