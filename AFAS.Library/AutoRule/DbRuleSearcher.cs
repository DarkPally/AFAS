using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SQLite;
using System.IO;
using System.Data;
using AFAS.Library;

namespace AFAS.Library.AutoRule
{
    public class DbRuleSearcher:IDisposable
    { 
        public class DbTable
        {
            public string Name { get; set; }
            public List<string> Fields { get; set; }
        }
    
        SQLiteConnection DbConnection = null;

        const string connectFormat = @"Data Source={0};Version=3;";

        const string selectTableFormat = "select * from {0}; ";
        const string selectWhereTableFormat = "select * from {0} where";
        const string whereFormat = " {0} like '%{1}%' ";

        List<DbTable> targetTables;

        public DbRuleSearcher(string path)
        {
            Init(path);
        }
        void Init(string dbPath)
        {
            DbConnection = new SQLiteConnection(String.Format(connectFormat, dbPath));
            DbConnection.Open();
            var tbs = DataCatchHelper.getTableNames(DbConnection);
            var tables = tbs.AsParallel().Select(t => new DbTable() { Name = t, Fields = DataCatchHelper.getFieldNames(DbConnection, t) });
            targetTables = tables.ToList();
        }
        public List<ForensicRuleItemInfo> SearchRule(string keyStr)
        {
            string selectCmd="";
            foreach (var t in targetTables)
            {
                selectCmd += String.Format(selectWhereTableFormat, t.Name);
                for (int i = 0; i < t.Fields.Count;++i )
                {
                    selectCmd += String.Format(whereFormat, t.Fields[i], keyStr);
                    if(i==t.Fields.Count-1)
                    {
                        selectCmd += ";";
                    }
                    else
                    {
                        selectCmd += "or";
                    }
                }
            }

            DataSet ds = new DataSet(); ;
            using (SQLiteDataAdapter da = new SQLiteDataAdapter(selectCmd,DbConnection))
            {
                da.Fill(ds);
            }
            List<ForensicRuleItemInfo> ts = new List<ForensicRuleItemInfo>();
            for(int i=0;i< targetTables.Count;++i)
            {                
                if(ds.Tables[i].Rows.Count!=0)
                {
                    ds.Tables[i].TableName = targetTables[i].Name;
                    ts.Add(getDataCatchInfo(ds.Tables[i]) );
                }
            }
            return ts;
        }

        DataCatchInfo getDataCatchInfo(DataTable dataTable)
        {
            return new DataCatchInfo()
            {
                Type = DataCatchInfo.DataType.Database,
                DataPath = dataTable.TableName
            };
        }

        List<string> getRelatingFields(DataTable table, string keyStr)
        {
            var res=new List<string>();
            for (int i = 0; i < table.Columns.Count; ++i)
            {
                if (table.Rows[0][i].ToString().ToLower().Contains(keyStr.ToLower()))
                {
                    res.Add(table.Columns[i].ColumnName);
                }
            }
            return res;
        }

        public void Close()
        {
            if (DbConnection != null)
            {
                DbConnection.Close();
                DbConnection.Dispose();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        #region Dispose方法
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                if (disposing)
                {
                    // Release managed resources
                    Close();
                }

                // Release unmanaged resources

                m_disposed = true;
            }
        }

        ~DbRuleSearcher()
        {
            Dispose(false);
        }

        private bool m_disposed;
        #endregion
    }
}
