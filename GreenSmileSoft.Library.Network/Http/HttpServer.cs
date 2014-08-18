using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Dispatcher;
using System.Web.Http.SelfHost;
using System.Web.Http;
using GreenSmileSoft.Library.Util;
using System.ComponentModel;

namespace GreenSmileSoft.Library.Network.Http
{
    public class HttpServer : NotifyPropertyChanged
    {
        protected HttpSelfHostServer server = null;
        protected HttpSelfHostConfiguration config = null;
        [Browsable(false)]
        public bool IsBusy { get; set; }
        protected virtual void setConfig()
        {
            config.Routes.MapHttpRoute(
                    name: "ActionApi",
                    routeTemplate: "{controller}/{action}/{id}",
                    defaults: new { id = RouteParameter.Optional }
                );
        }

        protected void initServer(string url, bool isStart = false)
        {
            try
            {
                config = new HttpSelfHostConfiguration(new Uri(url));
                config.Services.Replace(typeof(IHttpControllerSelector), new ControllerSelector(config, this));

                setConfig();

                if (isStart)
                {
                    Start();
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void Start()
        {
            server = new HttpSelfHostServer(config);
            server.OpenAsync().ContinueWith(getTask =>
            {
                if (getTask.IsCanceled)
                {

                }
                else if (getTask.IsFaulted)
                {
                }
                else
                {
                    IsBusy = true;
                    OnPropertyChanged("IsBusy");
                    OnStart();
                }
            });
        }

        public virtual void OnStart()
        { }

        public void Stop()
        {
            server.CloseAsync().ContinueWith(getTask =>
            {
                if (getTask.IsCanceled)
                {

                }
                else if (getTask.IsFaulted)
                {
                }
                else
                {
                    server.Dispose();
                    IsBusy = false;
                    OnPropertyChanged("IsBusy");
                    OnStop();
                }
            });
        }
        public virtual void OnStop()
        {

        }
    }
}
