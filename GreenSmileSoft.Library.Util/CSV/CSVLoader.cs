using System;
using System.Data;
using System.IO;
using System.Text;

namespace GreenSmileSoft.Library.Util.CSV
{
    public class CSVLoader
    {
        private DataTable dt;
        public CSVLoader(string fullPath)
        {
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException();
            }

            bool isfirstLine = true;

            using (FileStream fs = File.Open(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.Default))
                {
                    while (false == sr.EndOfStream)
                    {
                        String line = sr.ReadLine();
                        if (String.IsNullOrEmpty(line))
                        {
                            continue;
                        }

                        if (isfirstLine)
                        {
                            isfirstLine = false;
                            this.dt = new DataTable();

                            String[] columns = line.Split(',');

                            //Column을 만든다.
                            //타입은 무조건 Object형이며 사용할때 타입으로 Convert한다.
                            foreach (string colName in columns)
                            {
                                dt.Columns.Add(new DataColumn(colName.Trim()));
                            }
                            continue;
                        }

                        String[] values = line.Split(',');

                        DataRow row = dt.NewRow();

                        for (int i = 0; i < values.Length; ++i)
                        {
                            //아진짜 마음에 안든다.
                            row[i] = (object)values[i].Trim();
                        }

                        dt.Rows.Add(row);
                    }
                }
            }
        }
        public DataTable CsvTable
        {
            get
            {
                return dt;
            }
        }
    }
}
