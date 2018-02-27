using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Data.SQLite;
using System.Text.RegularExpressions;
using Neo.IronLua;

namespace AFAS.Library
{
    public class DataCatchHelper
    {
        const string connectFormat = @"Data Source={0};Version=3;";


        public static DataTable GetDbDataTable(SQLiteConnection connection, string tableName)
        {
            DataTable dataTable = new DataTable();

            //DataSet ds = new DataSet();
            /*
            using (SQLiteDataAdapter da = new SQLiteDataAdapter(string.Format("select * from [{0}]", tableName)
                , connection))
            {
                da.Fill(dataTable);
            }
            */

            using (SQLiteCommand cmd = new SQLiteCommand(string.Format("select * from [{0}]", tableName), connection))
            {
                using (SQLiteDataReader rdr = cmd.ExecuteReader())
                {
                    
                    dataTable.Load(rdr,LoadOption.PreserveChanges,(s,e)=>
                    {
                        e.Continue = true;
                        DataRow row = dataTable.NewRow();
                        for(int i =0;i<e.Values.Count();++i)
                        {
                            try
                            {
                                row[i] = rdr[i];
                            }
                            catch
                            {
                                //var t = rdr.GetFieldValue<string>(i);
                            }
                        }
                        dataTable.Rows.Add(row);
                    });
                }
            }
            //dataTable = ds.Tables[0];
            dataTable.TableName = tableName;
            return dataTable;
        }

        public static DataTable GetDbDataTable(string dbFilePath, string tableName)
        {
            using (var conn = new SQLiteConnection(String.Format(connectFormat, dbFilePath)))
            {
                conn.Open();
                return GetDbDataTable(conn, tableName);
            }
        }

        static public List<string> getTableNames(SQLiteConnection conn)
        {
            var res = new List<string>();
            var schemaTable = conn.GetSchema("TABLES");
            for (int i = 0; i < schemaTable.Rows.Count; i++)
            {
                var name = schemaTable.Rows[i]["TABLE_NAME"].ToString();
                if (name != "android_metadata" && name != "sqlite_sequence")
                {
                    res.Add(name);
                }
            }
            return res;

        }

        public static List<DataTable> GetDbDataTableWithRegEx(string dbFilePath, string tableNameRegEx)
        {
            var res = new List<DataTable>();

            using (var conn = new SQLiteConnection(String.Format(connectFormat, dbFilePath)))
            {
                conn.Open();
                Regex regex = new Regex(tableNameRegEx);

                var tableNames = getTableNames(conn).Where(c=> regex.IsMatch(c));
                foreach(var it in tableNames)
                {
                    res.Add(GetDbDataTable(conn, it));
                }
            }
            return res;
        }

        public static DataTable GetXmlDataTable(string xmlFilePath, string tableName)
        {
            try
            {
                DataSet ds = new DataSet();
                //读取XML到DataSet 

                StreamReader sr = new StreamReader(xmlFilePath, Encoding.Default);

                ds.ReadXml(sr);

                sr.Close();

                foreach (DataTable it in ds.Tables)
                {
                    if (it.TableName == tableName)
                        return it;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
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

        public static DataTable GetBinaryDataTable(string dbFilePath, LuaChunk chunk)
        {
            return null;
        }
    }
}
