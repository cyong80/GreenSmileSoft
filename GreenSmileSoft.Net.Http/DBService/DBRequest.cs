using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenSmileSoft.Net.Http.DBService
{
    [Serializable]
    public class DBRequest
    {
        public string Key { get; set; }
        public CommandType QueryType { get; set; }
        public string QueryString { get; set; }
        public List<KeyValuePair<string, object>> Parameters { get; set; }
        public DBRequest() { }
    }
}
