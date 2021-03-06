﻿using System;
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
        private Dictionary<string, string> auths = new Dictionary<string, string>();
        public Dictionary<string,string> Auths
        {
            private get
            {
                return auths;
            }
            set
            {
                auths = value;
            }
        }
        private HttpListener listener;
        public bool IsBusy
        {
            get
            {
                if(listener!=null)
                {
                    return listener.IsListening;
                }
                else
                {
                    return false;
                }
                
            }
        }
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
            if(listener.AuthenticationSchemes == AuthenticationSchemes.Basic)
            {
                bool credentials = false;
                HttpListenerBasicIdentity identity = (HttpListenerBasicIdentity)ctx.User.Identity;
                if(auths.ContainsKey(identity.Name))
                {
                    if(auths[identity.Name] == identity.Password)
                    {
                        credentials = true;
                    }
                }
                if(!credentials)
                {
                    ctx.Response.Close();
                    return;
                }
            }

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
                auths.Clear();
                listener.Close();
            }
        }
        public static Route ServerAction(string path, Func<string,byte[]> action)
        {
            return new Route(path, ctx =>
            {
                Console.WriteLine("ServerAction : " + path);
                string post = "";
                using(var body = ctx.Request.InputStream)
                {
                    using(var reader = new StreamReader(body,Encoding.UTF8))
                    {
                        post = reader.ReadToEnd();
                    }
                }
                byte[] bytes = action(post);
                ctx.Response.ContentLength64 = bytes.Length;
                ctx.Response.OutputStream.Write(bytes, 0, bytes.Length);
                ctx.Response.OutputStream.Close();
                ctx.Response.Close();
            });
        }

        public static Route ServerAction(string path, Func<byte[]> action)
        {
            return new Route(path, ctx =>
            {
                Console.WriteLine("ServerAction : " + path);
                byte[] bytes = action();
                ctx.Response.ContentLength64 = bytes.Length;
                ctx.Response.OutputStream.Write(bytes, 0, bytes.Length);
                ctx.Response.OutputStream.Close();
                ctx.Response.Close();
            });
        }

        public static Route ServerError(string path, int errorCode, string content)
        {
            return new Route(path, ctx =>
            {
                Console.WriteLine("ServerError : " + path);
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
                Console.WriteLine("ServerFolder : " + virtualPath);
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
                case ".json":
                    return "Application/json";
                default:
                    return "application/octet-stream";
            }
        }
    }
}
