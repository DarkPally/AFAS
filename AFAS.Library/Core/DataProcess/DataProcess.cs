using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Text.RegularExpressions;

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

        object handleDataByScript(object data)
        {
            return AFASManager.Lua.Environment.DoChunk(Info.Chunk, data);
        }


        object handleDataByRegEx(object data)
        {
            Regex regex = new Regex(Info.Content);
            var str = Convert.ToString(data);
            
            var match = regex.Match(str);
            if (match == null) return null;

            return match.Groups[Info.OutputColumnName];
        }

        public void DoWork()
        {
            if(loadDataTables())
            {
                dataTables.ForEach(c => c.Table.Columns.Add(new DataColumn(Info.OutputColumnName)));
                Func<object, object> funcHandle = null;

                switch (Info.Type)
                {
                    case DataProcessInfo.ProcessType.None:
                        return;
                    case DataProcessInfo.ProcessType.Script:
                        funcHandle = handleDataByScript;
                        break;
                    case DataProcessInfo.ProcessType.RegEx:
                        funcHandle = handleDataByRegEx;
                        break;
                }

                foreach (var table in dataTables)
                {
                    for(int i=0;i< table.Table.Rows.Count;++i )
                    {
                        var data = table.Table.Rows[i][Info.ColumnName];
                        var t=funcHandle(data);
                        table.Table.Rows[i][Info.OutputColumnName] = t;
                    }
                }
            }
            
        }
    }
}
