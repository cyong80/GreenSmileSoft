using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace GreenSmileSoft.Net.Http
{
    public class HttpRequest
    {
        public static string GetString(string url, string post = "", string method = "POST", Action<string> error = null)
        {
            HttpWebRequest request;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);


                request.Method = method;
                byte[] bytes = Encoding.UTF8.GetBytes(post);
                request.ContentLength = bytes.Length;
                using (var dataStream = request.GetRequestStream())
                {
                    dataStream.Write(bytes, 0, bytes.Length);
                }

                try
                {
                    var response = request.GetResponse() as HttpWebResponse;
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    return reader.ReadToEnd();
                }
                catch
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                if (error != null)
                {
                    error(ex.StackTrace);
                }
                return "";
            }
        }

        public static void GetStrem(string url, Action<MemoryStream> callback, string post = "", string method = "POST", Action<long, long> progresscallback = null)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = method;
                byte[] bytearray = Encoding.UTF8.GetBytes(post);
                request.ContentLength = bytearray.Length;
                using (var dataStream = request.GetRequestStream())
                {
                    dataStream.Write(bytearray, 0, bytearray.Length);
                }

                request.BeginGetResponse(new AsyncCallback(beginCallback), new { request = request, callback = callback, progresscallback = progresscallback });
            }catch(Exception ex)
            {
                Trace.WriteLine(ex.StackTrace);
            }
            
        }

        public static void GetStrem(string url, Action<MemoryStream> callback, object post, string method = "POST", Action<long, long> progresscallback = null)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;
            
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, post);
                var bytes = new byte[stream.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(bytes, 0, (int)stream.Length);
                request.ContentLength = bytes.Length;
                using (var dataStream = request.GetRequestStream())
                {
                    dataStream.Write(bytes, 0, bytes.Length);
                }
            }
            request.BeginGetResponse(new AsyncCallback(beginCallback), new { request = request, callback = callback, progresscallback = progresscallback });
        }
        private static void beginCallback(IAsyncResult ar)
        {
            try
            {
                HttpWebRequest request = (ar.AsyncState as dynamic).request;
                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(ar);
                Stream responseStream = response.GetResponseStream();
                long total = response.ContentLength;
                MemoryStream mstream = new MemoryStream();
                Action<long, long> progress = (ar.AsyncState as dynamic).progresscallback;
                byte[] buffer = new byte[2048];
                int bytesRead = 0;
                do
                {
                    bytesRead = responseStream.Read(buffer, 0, buffer.Length);
                    mstream.Write(buffer, 0, bytesRead);
                    if (progress != null)
                    {
                        progress(mstream.Length, total);
                    }
                } while (bytesRead != 0);
                responseStream.Close();

                MemoryStream resultStream = null;
                if (mstream.Length > 0)
                {
                    resultStream = mstream;
                }

                Action<MemoryStream> callback = (ar.AsyncState as dynamic).callback;
                callback(resultStream);
            }
            catch (Exception ex)
            {
                Action<MemoryStream> callback = (ar.AsyncState as dynamic).callback;
                callback(null);
            }
        }

    }
}
