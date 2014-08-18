using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace GreenSmileSoft.Library.Util.DBS
{
    public class DBQueryManager
    {
        public static List<string> Duplicate
        {
            get;
            private set;
        }

        public static Dictionary<string, Dictionary<string, DBQuery>> QuerySet = new Dictionary<string, Dictionary<string, DBQuery>>();

        public static DBQuery GetQuery(string server, string key)
        {
            if(!QuerySet.ContainsKey(server) || !QuerySet[server].ContainsKey(key))
            {
                return null;
            }
            return QuerySet[server][key];
        }
        public static void SetDbQueries(Assembly ass)
        {
            Duplicate = new List<string>();
            var paths = from p in ass.GetManifestResourceNames()
                             where p.Contains(".dbq")
                             select p;

            Duplicate = new List<string>();
            foreach(var path in paths)
            {
                StreamReader reader = new StreamReader(ass.GetManifestResourceStream(path));
                string xmlstring = reader.ReadToEnd();
                XDocument doc = XDocument.Parse(xmlstring);
                ReadXml(doc);
            }
            
        }
        public static void SetDbQueries(FileInfo[] configPath)
        {
            if(configPath == null || configPath.Length == 0)
            {
                throw new FileNotFoundException(string.Format("File Not found {0}", configPath));
            }

            Duplicate = new List<string>();
            foreach(FileInfo f in configPath)
            {
                XDocument doc = XDocument.Load(f.FullName);
                ReadXml(doc);
            }
        }

        private static void ReadXml(XDocument doc)
        {
            foreach (XElement server in doc.Descendants("server"))
            {
                string serverName = server.Attribute("name").Value;
                foreach (XElement db in server.Descendants("db"))
                {
                    string keymap = string.Format("{0}.{1}", serverName, db.Attribute("name").Value.ToLower());

                    if (!QuerySet.ContainsKey(keymap))
                    {
                        QuerySet.Add(keymap, new Dictionary<string, DBQuery>());
                    }

                    foreach (XElement query in db.Descendants("query"))
                    {
                        string key = query.Attribute("key").Value;
                        if (QuerySet[keymap].ContainsKey(key))
                        {
                            string dupKey = string.Format("{0}.{1}", "Duplicate", keymap, key);
                            Duplicate.Add(dupKey);
                            continue;
                        }
                        DBQuery dbq = new DBQuery() { Key = key, KeyMap = keymap };
                        dbq.QueryString = query.Element("querystring").Value;
                        if (query.Attribute("type") != null)
                        {
                            dbq.CommandType = (CommandType)Enum.Parse(typeof(CommandType), query.Attribute("type").Value);
                        }
                        else
                        {
                            dbq.CommandType = CommandType.Text;
                        }
                        dbq.Parameters = (from p in query.Descendants("param")
                                          select new Parameter() { Name = p.Attribute("name").Value, Value = p.Attribute("value").Value }).ToList();

                        QuerySet[keymap].Add(key, dbq);
                    }
                }
            }
        }

    }
}
