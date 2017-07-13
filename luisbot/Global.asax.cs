namespace LuisBot
{
    using System.Web.Http;
    using System.Diagnostics;
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
