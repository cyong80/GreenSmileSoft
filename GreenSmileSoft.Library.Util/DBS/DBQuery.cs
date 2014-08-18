using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace GreenSmileSoft.Library.Util.DBS
{
    public class DBQuery
    {
        public string KeyMap { get; set; }
        public string Key { get; set; }
        public CommandType CommandType { get; set; }
        public string QueryString { get; set; }
        public List<Parameter> Parameters { get; set; }
    }
}
