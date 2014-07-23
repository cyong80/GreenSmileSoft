using AspNetHost;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace GreenSmileSoft.Net.Http
{
    using Route = KeyValuePair<string, Action<System.Net.HttpListenerContext>>;
    public class AspNetServer : HttpServer
    {
        private static SimpleHost msh;
        public AspNetServer(string host, string port, AuthenticationSchemes auth, string rootpath)
            : base(host, port, auth)
        {
            msh = (SimpleHost)ApplicationHost.CreateApplicationHost(typeof(SimpleHost), "/", rootpath);
        }

        public static Route ServerFolder(string virtualPath, string physicalPath)
        {
            return new Route(virtualPath, ctx =>
            {
                var phys = ctx.Request.RawUrl.Replace(virtualPath, physicalPath);
                if (!File.Exists(phys))
                {
                    ServerError("", 404, "Not Found").Value(ctx);
                    return;
                }
                Console.WriteLine(Path.GetExtension(phys));
                if (isAspNetExtention(Path.GetExtension(phys)))
                {
                    string page = ctx.Request.Url.LocalPath.Replace("/", "");
                    string query = ctx.Request.Url.Query.Replace("?", "");
                    Console.WriteLine("Received request for {0}?{1}", page, query);

                    StreamWriter sw = new StreamWriter(ctx.Response.OutputStream,Encoding.UTF8);
                    msh.ProcessRequest(page, query, sw);
                    sw.Flush();
                    ctx.Response.Close();
                    return;
                }
                
                var contentType = GetContentType(Path.GetExtension(phys));
                ctx.Response.Headers[HttpResponseHeader.ContentType] = contentType;
                var content = File.ReadAllBytes(phys);
                ctx.Response.OutputStream.Write(content, 0, content.Length);
                ctx.Response.OutputStream.Close();
                ctx.Response.Close();
            });
        }
        
        private static bool isAspNetExtention(string ext)
        {
            if(ext == ".aspx" ||
                ext == ".ashx"
                )
            {
                return true;
            }
            return false;
        }
    }
}
