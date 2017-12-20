using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;

namespace AFAS.Library
{
    public class DataProcess
    {
        public PackageForensic Environment { get; set; }
        public DataProcessInfo Info { get; set; }

        List<DataResultItem> dataTables;
        bool loadDataTables()
        {
            if (!Environment.CatchDataTables.ContainsKey(Info.Key)) return false;
            dataTables = Environment.CatchDataTables[Info.Key];
            return true;
        }

        void handleDataByScript(object data)
        {
            AFASManager.Lua.Environment.DoChunk(Info.Chunk, data);
        }
        public void DoWork()
        {
            if(loadDataTables())
            {
                foreach (var table in dataTables)
                {
                    for(int i=0;i< table.Table.Rows.Count;++i )
                    {
                        var data = table.Table.Rows[i][Info.ColumnName];
                        handleDataByScript(data);
                    }
                }
            }
            
        }
    }
}
