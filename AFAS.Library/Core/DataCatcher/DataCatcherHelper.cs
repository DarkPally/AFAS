using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Data.SQLite;
using System.Text.RegularExpressions;

namespace AFAS.Library.Core
{
    public class DataCatcherHelper
    {
        public static DataTable GetDbDataTable(SQLiteConnection connection, string tableName)
        {
            DataTable dataTable = null;

            DataSet ds = new DataSet();
            using (SQLiteDataAdapter da = new SQLiteDataAdapter(string.Format("select * from [{0}]", tableName)
                , connection))
            {
                da.Fill(ds);
            }
            dataTable = ds.Tables[0];
            dataTable.TableName = tableName;
            return dataTable;
        }

        public static DataTable GetDbDataTable(string dbFilePath, string tableName)
        {
            const string connectFormat = @"Data Source={0};Version=3;";

            using (var conn = new SQLiteConnection(String.Format(connectFormat, dbFilePath)))
            {
                conn.Open();
                return GetDbDataTable(conn, tableName);
            }
        }

        public static DataTable GetFileDataTable(string dbFilePath, string regexStr)
        {
            string srcText;
            using (StreamReader sr = new StreamReader(dbFilePath, System.Text.Encoding.Default))
            {
                srcText = sr.ReadToEnd();
            }

            Regex r = new Regex(regexStr, RegexOptions.None);
            var mcs = r.Matches(srcText);
            if (mcs.Count == 0) return null;

            DataTable res = new DataTable();
            string[] groupNames = r.GetGroupNames();
            foreach (var it in groupNames)
            {
                int temp;
                if (int.TryParse(it, out temp)) continue;
                res.Columns.Add(new DataColumn(it));
            }
            foreach (Match match in mcs)
            {
                DataRow dr = res.NewRow();

                foreach (DataColumn it in res.Columns)
                {
                    dr[it.ColumnName] = match.Groups[it.ColumnName].Value;
                }
                res.Rows.Add(dr);
            }
            return res;
        }
    }
}
