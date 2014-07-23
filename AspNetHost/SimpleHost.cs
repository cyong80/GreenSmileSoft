using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

namespace AspNetHost
{
    public class SimpleHost : MarshalByRefObject
    {
        public void ProcessRequest(string p, string q, TextWriter tw)
        {
            SimpleWorkerRequest swr = new SimpleWorkerRequest(p, q, tw); 
            HttpRuntime.ProcessRequest(swr);
        }
    }
}
