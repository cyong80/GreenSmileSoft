using GreenSmileSoft.Library.Util.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenSmileSoft.Library.Util.Auth
{
    public class GSAuthGroup
    {
        public int GroupID { get; set; }
        public string GroupName
        {
            get
            {
                return TextTable._[GSAuthMgr.TEXTRULE, GroupID];
            }
        }
        public List<GSAuth> GSAuths { get; set; }
    }
}
