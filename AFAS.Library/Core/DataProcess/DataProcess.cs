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

        List<ForensicResultItem> dataTables;
        bool loadDataTables()
        {
            if (!Environment.CatchDataTables.ContainsKey(Info.Key) ||
                    Environment.CatchDataTables[Info.Key].Count == 0) return false;
            var t = Environment.CatchDataTables[Info.Key];
            dataTables = new List<ForensicResultItem>();
            foreach (var it in t )
            {
                if(it.IsMutiTableParent)
                {
                    dataTables.AddRange(it.Children);
                }
                else
                {
                    dataTables.Add(it);
                }
            }
            return true;
        }

        object handleDataByScript(object data)
        {
            return Environment.LuaEnvironment.dochunk(Info.Chunk)[0];
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
                    case DataProcessInfo.ProcessType.ScriptName:
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
                        
                        if(Info.Type==DataProcessInfo.ProcessType.Script || Info.Type == DataProcessInfo.ProcessType.ScriptName)
                        {
                            Environment.LuaEnvironment.data = data;
                            Environment.LuaEnvironment.dataTable = table.Table;
                            Environment.LuaEnvironment.parentFile = table.ParentFile;
                            Environment.LuaEnvironment.dataTableRow = table.Table.Rows[i];
                        }
                        
                        var t=funcHandle(data);
                        table.Table.Rows[i][Info.OutputColumnName] = t;
                    }
                }
            }
            
        }
    }
}
