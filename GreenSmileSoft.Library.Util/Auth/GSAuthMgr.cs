using GreenSmileSoft.Library.Util.Globalization;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenSmileSoft.Library.Util.Auth
{
    public class GSAuthMgr
    {
        public const string TEXTRULE = "GSAUTH";
        public static Int32 TextRuleValue = 900000000;
        public static List<GSAuthGroup> GroupList = new List<GSAuthGroup>();

        public static void Initialize(JArray groups)
        {
            TextTable.AddTextRule(TEXTRULE, TextRuleValue);
            foreach(var group in groups)
            {
                GSAuthGroup agroup = new GSAuthGroup()
                {
                    GroupID = group["id"].Value<int>(),
                    GSAuths = new List<GSAuth>()
                };

                JArray auths = group["modules"].Value<JArray>();
                foreach(var auth in auths)
                {
                    GSAuth gsauth = new GSAuth()
                    {
                        ModuleID = auth["id"].Value<int>(),
                        Navigation = auth["navigation"].Value<string>()
                    };
                    agroup.GSAuths.Add(gsauth);
                }
                GroupList.Add(agroup);
            }
        }
    }
}
