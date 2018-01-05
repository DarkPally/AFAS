using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;

namespace AFAS.Library
{
    public class DataMark
    {
        public PackageForensic Environment { get; set; }
        public DataMarkInfo Info { get; set; }

        List<DataResultItem> Tables;
        bool loadDataTables()
        {
            if (!Environment.CatchDataTables.ContainsKey(Info.Key)) return false;            
            Tables = Environment.CatchDataTables[Info.Key];
            if (Tables.Count == 0) return false;
            if (!Tables[0].IsMutiTableParent || Info.TableDescType == 0) return true;

            var t = new List<DataResultItem>();
            foreach (var it in Tables)
            {
                t.AddRange(it.Children.Where(c => c.Key == Info.Key));
            }
            if (Info.TableDescType==1)
            {
                Tables = t;
            }
            else
            {
                Tables.AddRange(t);
            }

            return true;
        }

        public void DoWork()
        {
            if(loadDataTables())
            {
                for(int i=0;i< Tables.Count;++i)
                {
                    //Tables[i].Desc = Info.TableDesc == null ? Tables[i].Table.TableName : Info.TableDesc;
                    Tables[i].MarkInfo = Info;
                    if(Info.TableDescScriptChunk != null)
                    {
                        Environment.LuaEnvironment.dataTable = Tables[i];
                        Tables[i].Desc=Environment.LuaEnvironment.dochunk(Info.TableDescScriptChunk)[0];
                    }
                }
                if (Info.NotShowAtRoot == true) return;

                var t = Tables.FirstOrDefault();
                if (t != null && t.ParentData==null)
                {
                    if(Tables.Count==1)
                    {
                        Environment.Result.Children.Add(Tables[0]);
                    }
                    else
                    {
                        var tItem = new DataResultItem()
                        {
                            Children = Tables,
                            MarkInfo=Info
                        };
                        Environment.Result.Children.AddRange(Tables);
                    }
                    
                }    
            }
            
        }
    }
}
