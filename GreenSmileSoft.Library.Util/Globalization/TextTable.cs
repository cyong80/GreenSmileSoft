using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GreenSmileSoft.Library.Util.Globalization
{
    public sealed partial class TextTable
    {
        private static TextTable instance = null;
        public static TextTable _
        {
            get
            {
                if (null == instance)
                {
                    instance = new TextTable();
                }
                return instance;
            }
        }
    }

    public sealed partial class TextTable
    {
        private Dictionary<Int32, String> dicText = null;

        private TextTable()
        {
        }

        public String this[Int32 nameid]
        {
            get
            {
                if (dicText.ContainsKey(nameid))
                {
                    return dicText[nameid];
                }
                return "TEXT_ERROR";
            }
        }

        public string this[string rule, Int32 value]
        {
            get
            {
                if(TextRule.ContainsKey(rule))
                {
                    Int32 nameid = TextRule[rule] + value;
                    return this[nameid];
                }
                return "TEXT_ERROR";
            }
        }

        private static Dictionary<string, Int32> TextRule = new Dictionary<string, int>();

        public static void AddTextRule(string rule, Int32 value)
        {
            TextRule.Add(rule, value);
        }

        public void Load(params string[] paths)
        {
            dicText = new Dictionary<int, string>();
            foreach(var path in paths)
            {
                loadTextTable(path);
            }
        }

        private void loadTextTable(string path)
        {
            XElement el = XElement.Load(path);
            StringBuilder sb = new StringBuilder();
            foreach (XElement t in el.Descendants("S"))
            {
                String sid = t.Attribute("ID").Value;
                if (sid.Equals("0"))
                {
                    continue;
                }
                int id = Convert.ToInt32(sid);

                if (dicText.ContainsKey(id))
                {
                    sb.Append(string.Format("dup local id {0} : {1}", path, id));
                }
                else
                {
                    dicText.Add(id, t.Value.Trim());
                }
            }

            if (sb.Length != 0)
            {
                throw new Exception(sb.ToString());
            }
        }
    }
}
