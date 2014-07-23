using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace GreenSmileSoft.Net.Http
{
    using Route = KeyValuePair<string, Action<System.Net.HttpListenerContext>>;
    public class HttpServer
    {
        private HttpListener listener;
        public Route[] Routes = { };
        public HttpServer(string host, string port, AuthenticationSchemes auth)
        {
            listener = new HttpListener();
            listener.Prefixes.Add(string.Format("http://127.0.0.1:{0}/", port));
            listener.Prefixes.Add(string.Format("http://{0}:{1}/",host, port));
            listener.AuthenticationSchemes = auth;
        }
        public void AcceptOne()
        {
            var ctx = listener.GetContext();
            try
            {
                var route = Routes.First(r => ctx.Request.RawUrl.StartsWith(r.Key));
                route.Value(ctx);
            }
            catch(Exception ex)
            {
                ctx.Response.Close();
            }
        }
        public void Start()
        {
            listener.Start();
        }
        public void Close()
        {
            if (listener != null)
            {
                listener.Close();
            }
        }

        public static Route ServerError(string path, int errorCode, string content)
        {
            return new Route(path, ctx =>
            {
                ctx.Response.StatusCode = errorCode;
                ctx.Response.StatusDescription = content;
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(content);
                ctx.Response.ContentLength64 = buffer.Length;
                ctx.Response.OutputStream.Write(buffer, 0, buffer.Length);
                ctx.Response.OutputStream.Close();
                ctx.Response.Close();
            });
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
                
                var contentType = GetContentType(Path.GetExtension(phys));
                ctx.Response.Headers[HttpResponseHeader.ContentType] = contentType;
                var content = File.ReadAllBytes(phys);
                ctx.Response.OutputStream.Write(content, 0, content.Length);
                ctx.Response.OutputStream.Close();
                ctx.Response.Close();
            });
        }

        protected static string GetContentType(string ext)
        {
            switch(ext)
            {
                case ".js":
                    return "text/javascript";
                case ".htm":
                case ".html":
                    return "text/html";
                case ".png":
                    return "image/png";
                case ".jpg":
                    return "image/jpg";
                case ".css":
                    return "text/css";
                default:
                    return "application/octet-stream";
            }
        }
    }
}
