using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;

namespace AFAS.Library.Core
{
    public class DataAssociate
    {
        public PackageForensic Environment { get; set; }
        public DataAssociateInfo Info { get; set; }

        List<DataResultItem> parentTables;

        List<DataResultItem> childTables;
        bool loadDataTables()
        {
            if (!Environment.CatchDataTables.ContainsKey(Info.ParentKey)) return false;
            parentTables = Environment.CatchDataTables[Info.ParentKey];

            if (!Environment.CatchDataTables.ContainsKey(Info.ChildKey)) return false;
            childTables = Environment.CatchDataTables[Info.ChildKey];
            return true;
        }

        public void DoWork()
        {
            if(loadDataTables())
            {
                var res = new List<DataResultItem>();
                for(int i=0;i< parentTables.Count;++i)
                {
                    var keyContents = parentTables[i].Table.AsEnumerable().Select(c => c.Field<string>(Info.ParentKey)).Distinct();
                    foreach(var it in keyContents)
                    {   
                        var list = childTables[i].Table.AsEnumerable().Where(c => c.Field<string>(Info.ChildKey)==it);
                        DataTable dtNew = childTables[i].Table.Clone();
                        foreach(var dr in list)
                        {
                            dtNew.ImportRow(dr);
                        }
                        dtNew.TableName = it;
                        var t = new DataResultItem()
                        {
                            ParentData= childTables[i],
                            ParentFile = childTables[i].ParentFile,
                            Table = dtNew,
                        };
                        res.Add(t);
                        if (parentTables[i].Children == null) parentTables[i].Children = new List<DataResultItem>();
                        parentTables[i].Children.Add(t);
                    }
                }
                Environment.CatchDataTables.Add(Info.Key, res);


            }
            
        }
    }
}
