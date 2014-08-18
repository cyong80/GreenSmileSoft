using GreenSmileSoft.Library.Util.Event;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Handlers;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GreenSmileSoft.Library.Network.Http
{
    public class HttpRequest
    {
        public static string UserAgent { get; set; }
        public static void GetString(string url, Action<string> callback, Action<string, Exception> errorCallback = null)
        {
            HttpClient client = new HttpClient();
            if(UserAgent!= null)
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
            }
            client.GetStringAsync(url).ContinueWith(
                getTask =>
                {
                    if (getTask.IsCanceled)
                    {
                        if(errorCallback != null)
                        {
                            errorCallback("Request was canceled.", null);
                        }
                    }
                    else if (getTask.IsFaulted)
                    {
                        if (errorCallback != null)
                        {
                            errorCallback("Request failed.", getTask.Exception);
                        }
                    }
                    else
                    {
                        callback(getTask.Result);
                    }
                });
        }
        public static void GetByteArray(string url, Action<byte[]> callback, Action<string, Exception> errorCallback = null, Action<long, long> progressCallback = null)
        {
            HttpClient client = null;
            if (progressCallback != null)
            {
                ProgressMessageHandler progress = new ProgressMessageHandler();
                progress.HttpReceiveProgress += (sender, args) =>
                    {
                        progressCallback(args.BytesTransferred, (long)args.TotalBytes);
                    };
                client = HttpClientFactory.Create(progress);
            }
            else
            {
                client = new HttpClient();
            }
            
            if (UserAgent != null)
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
            }

            client.GetByteArrayAsync(url).ContinueWith(getTask =>
            {
                if (getTask.IsCanceled)
                {
                    if (errorCallback != null)
                    {
                        errorCallback("Request was canceled.", null);
                    }
                }
                else if (getTask.IsFaulted)
                {
                    if (errorCallback != null)
                    {
                        errorCallback("Request failed.", getTask.Exception);
                    }
                }
                else
                {
                    callback(getTask.Result);
                }
            });
        }

        public static void Post<T1,T2>(string url,T1 post, Action<T2> callback, Action<string, Exception> errorCallback = null, Action<long, long> progressCallback = null)
        {
            HttpClient client = null;
            if (progressCallback != null)
            {
                ProgressMessageHandler progress = new ProgressMessageHandler();
                progress.HttpReceiveProgress += (sender, args) =>
                {
                    progressCallback(args.BytesTransferred, (long)args.TotalBytes);
                };
                client = HttpClientFactory.Create(progress);
            }
            else
            {
                client = new HttpClient();
            }

            if (UserAgent != null)
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
            }

            var mediaType = new MediaTypeHeaderValue("application/json");
            var jsonFormatter = new JsonMediaTypeFormatter();

            HttpContent content = new ObjectContent<T1>(post, jsonFormatter);

            client.PostAsync(url, content).ContinueWith(
                postTask =>
                {
                    if (postTask.IsCanceled)
                    {
                        if (errorCallback != null)
                        {
                            errorCallback("Request was canceled.", null);
                        }
                    }
                    else if (postTask.IsFaulted)
                    {
                        if (errorCallback != null)
                        {
                            errorCallback("Request failed.", postTask.Exception);
                        }
                    }
                    else
                    {
                        try
                        {
                            Task<T2> task = postTask.Result.Content.ReadAsAsync<T2>();
                            task.Wait();
                            callback(task.Result);
                        }
                        catch(Exception ex)
                        {
                            Trace.WriteLine(ex.ToString());
                            errorCallback("Read error.", ex);
                        }
                    }
                });
        }

        public static void Post<T>(string url,T post, Action<byte[]> callback, Action<string, Exception> errorCallback = null, Action<long, long> progressCallback = null)
        {
            HttpClient client = null;
            if (progressCallback != null)
            {
                ProgressMessageHandler progress = new ProgressMessageHandler();
                progress.HttpReceiveProgress += (sender, args) =>
                {
                    progressCallback(args.BytesTransferred, (long)args.TotalBytes);
                };
                client = HttpClientFactory.Create(progress);
            }
            else
            {
                client = new HttpClient();
            }

            if (UserAgent != null)
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
            }

            var mediaType = new MediaTypeHeaderValue("application/json");
            var jsonFormatter = new JsonMediaTypeFormatter();

            HttpContent content = new ObjectContent<T>(post, jsonFormatter);

            client.PostAsync(url, content).ContinueWith(
                postTask =>
                {
                    if (postTask.IsCanceled)
                    {
                        if (errorCallback != null)
                        {
                            errorCallback("Request was canceled.", null);
                        }
                    }
                    else if (postTask.IsFaulted)
                    {
                        if (errorCallback != null)
                        {
                            errorCallback("Request failed.", postTask.Exception);
                        }
                    }
                    else
                    {
                        try
                        {
                            using (var stream = new MemoryStream())
                            {
                                postTask.Result.Content.CopyToAsync(stream).Wait();
                                byte[] bytes = new byte[stream.Length];
                                stream.Seek(0, SeekOrigin.Begin);
                                stream.Read(bytes, 0, (int)stream.Length);
                                callback(bytes);
                            }
                        }
                        catch(Exception ex)
                        {
                            Trace.WriteLine(ex.ToString());
                            errorCallback("Read error.",ex);
                        }
                    }
                });
        }
    }
}
