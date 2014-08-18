using GreenSmileSoft.Library.Util.DBS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenSmileSoft.Library.Util.DBS
{
    [Serializable]
    public class DBRequest
    {
        public string Key { get; set; }
        public CommandType QueryType { get; set; }
        public string QueryString { get; set; }
        public List<Parameter> Parameters { get; set; }
        public DBRequest() { }
        public static DBRequest GetRequest(DBQuery query, params Parameter[] parameters)
        {
            DBRequest request = new DBRequest()
            {
                Key = query.KeyMap,
                QueryType = query.CommandType,
                QueryString = query.QueryString,
                Parameters = new List<Parameter>(query.Parameters)
            };
            request.Parameters.AddRange(parameters);
            return request;
        }
    }
}
