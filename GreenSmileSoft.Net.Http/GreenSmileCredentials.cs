using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GreenSmileSoft.Net.Http
{
    public class GreenSmileCredentials : ICredentials
    {
        public NetworkCredential GetCredential(Uri uri, string authType)
        {
            throw new NotImplementedException();
        }
    }
}
