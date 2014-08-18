using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace GreenSmileSoft.Library.Network.Http
{
    public class ControllerSelector : DefaultHttpControllerSelector
    {
        public static Dictionary<Type, string> controllers = new Dictionary<Type, string>();
        private readonly HttpConfiguration _configuration;
        private object server;
        public ControllerSelector(HttpConfiguration configuration, object server)
            : base(configuration)
        {
            this.server = server;
            _configuration = configuration;
        }

        public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            try
            {
                var assembly = Assembly.LoadFile(controllers[server.GetType()]);
                var types = assembly.GetTypes(); //GetExportedTypes doesn't work with dynamic assemblies
                var matchedTypes = types.Where(i => typeof(IHttpController).IsAssignableFrom(i)).ToList();

                var controllerName = base.GetControllerName(request);
                var matchedController =
                    matchedTypes.FirstOrDefault(i => i.Name.ToLower() == controllerName.ToLower() + "controller");
                return new HttpControllerDescriptor(_configuration, controllerName, matchedController);

            }
            catch (Exception ex)
            {

            }
            return null;
        }
    }
}
