using GreenSmileSoft.Library.Util.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace GreenSmileSoft.Library.Util.Converters
{
    public class DataTableEx
    {
        public static List<T> CreateListFromDataTable<T>(DataTable table) where T : new()
        {
            var list = from row in table.AsEnumerable()
                       select CreateItemFromRow<T>(row);
            return list.ToList();
        }

        public  static T CreateItemFromRow<T>(DataRow row) where T : new()
        {
            IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            T item = new T();
            foreach (var property in properties)
            {
                Mapping attrMapping = Attribute.GetCustomAttribute(property, typeof(Mapping)) as Mapping;
                if (attrMapping != null)
                {
                    if (!row.Table.Columns.Contains(attrMapping.Name) || row[attrMapping.Name] == DBNull.Value)
                    {
                        continue;
                    }
                    property.SetValue(item, row[attrMapping.Name], null);
                }
                else
                {
                    if (!row.Table.Columns.Contains(property.Name) || row[property.Name] == DBNull.Value)
                    {
                        continue;
                    }
                    property.SetValue(item, row[property.Name], null);
                }
            }
            return item;
        }
    }
}
