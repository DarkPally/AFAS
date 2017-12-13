using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;

namespace AFAS.Library.Core
{
    public class DataProcess
    {
        public PackageForensic Environment { get; set; }
        public DataProcessInfo Info { get; set; }

        List<DataTable> dataTables;
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
                foreach (DataTable table in dataTables)
                {
                    for(int i=0;i< table.Rows.Count;++i )
                    {
                        var data = table.Rows[i][Info.ColumnName];
                        handleDataByScript(data);
                    }
                }
            }
            
        }
    }
}
