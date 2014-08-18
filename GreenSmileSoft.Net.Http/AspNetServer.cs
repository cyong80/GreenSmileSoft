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
    using GreenSmileSoft.Library.Util.DBS;
    using GreenSmileSoft.Net.Http.DBService;
    using System.Data;
    using System.Runtime.Serialization.Formatters.Binary;
    using Route = KeyValuePair<string, Action<System.Net.HttpListenerContext>>;
    public class AspNetServer : HttpServer
    {
        private static SimpleHost msh;
        public AspNetServer(string host, string port, AuthenticationSchemes auth, string rootpath)
            : base(host, port, auth)
        {
            msh = (SimpleHost)ApplicationHost.CreateApplicationHost(typeof(SimpleHost), "/", rootpath);
        }

        public static Route ServerDB(string DBExecutePath, Dictionary<string,IDbConnection> dbConnections)
        {
            return new Route(DBExecutePath, ctx =>
            {
                string post = "";
                using (var body = ctx.Request.InputStream)
                {
                    var formatter = new BinaryFormatter();
                    //body.Seek(0, SeekOrigin.Begin);
                    var requests = (DBRequest[])formatter.Deserialize(body);
                    var request = requests[0];
                    if(dbConnections.ContainsKey(request.Key))
                    {
                        try
                        {
                            dbConnections[request.Key].Open();
                            using (DBFactory dbFactory = new DBFactory(dbConnections[request.Key]))
                            {
                                DataSet ds = new DataSet();
                                dbFactory.Command.CommandText = request.QueryString;
                                dbFactory.Command.CommandType = request.QueryType;
                                if (request.Parameters != null)
                                {
                                    dbFactory.SetParameters((from p in request.Parameters select new KeyValuePair<string, object>(p.Name, p.Value)).ToList());
                                }
                                dbFactory.DbDataAdapter.Fill(ds);
                                using (var stream = new MemoryStream())
                                {
                                    var rsformatter = new BinaryFormatter();
                                    formatter.Serialize(stream, ds);
                                    var bytes = new byte[stream.Length];
                                    stream.Seek(0, SeekOrigin.Begin);
                                    stream.Read(bytes, 0, (int)stream.Length);
                                    
                                    ctx.Response.ContentLength64 = bytes.Length;
                                    ctx.Response.OutputStream.Write(bytes, 0, bytes.Length);
                                    ctx.Response.OutputStream.Close();
                                    ctx.Response.Close();
                                }
                            }
                            dbConnections[request.Key].Close();
                        }
                        catch(Exception ex) 
                        {
                            if (dbConnections[request.Key].State == ConnectionState.Open)
                            {
                                dbConnections[request.Key].Close();
                            }
                            Console.WriteLine(ex.StackTrace);
                        }
                    }
                    
                }
            });
        }
        public static Route ServerFolder(string virtualPath, string physicalPath)
        {
            return new Route(virtualPath, ctx =>
            {
                var phys = ctx.Request.RawUrl.Replace(virtualPath, physicalPath + @"\");
                Console.WriteLine(phys);
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
