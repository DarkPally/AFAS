using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;

namespace AFAS.Library
{
    public class DataAssociate
    {
        public PackageForensic Environment { get; set; }
        public DataAssociateInfo Info { get; set; }

        List<DataResultItem> parentTables;

        List<DataResultItem> childTables;
        bool loadDataTables()
        {
            if (!Environment.CatchDataTables.ContainsKey(Info.ParentTableKey)) return false;
            parentTables = Environment.CatchDataTables[Info.ParentTableKey];

            if (!Environment.CatchDataTables.ContainsKey(Info.ChildTableKey)) return false;
            childTables = Environment.CatchDataTables[Info.ChildTableKey];
            return true;
        }

        DataResultItem getDescItem(DataResultItem parent, string keyContent)
        {
            var t = parent.Children.FirstOrDefault(c => c.Desc == keyContent);
            if (t!=null) return t;

            var list = parent.Table.AsEnumerable().Where(c=>Convert.ToString(c[Info.ParentTableColumn])==keyContent);
            DataTable dtNew = parent.Table.Clone();
            foreach (var dr in list)
            {
                dtNew.ImportRow(dr);
            }

            t = new DataResultItem()
            {                
                Desc = keyContent,
                Table=dtNew
            };
            parent.Children.Add(t);
            return t;
        }

        void handleChildAssociateColumn()
        {

            var res = new List<DataResultItem>();
            for (int i = 0; i < parentTables.Count; ++i)
            {
                var keyContents = parentTables[i].Table.AsEnumerable().Select(c => Convert.ToString(c[Info.ParentTableColumn])).Distinct();
                
                foreach (var it in keyContents)
                {
                    var tKey = getDescItem(parentTables[i], it);

                    for(int j=0;j< childTables.Count;++j)
                    {
                        if (Info.Type == DataAssociateInfo.AssociateType.InSameFile &&
                            childTables[j].ParentFile != parentTables[i].ParentFile) continue;

                        var list = childTables[j].Table.AsEnumerable().Where(c => Convert.ToString(c[Info.AssociateColumn]) == it);
                        if (list.Count() == 0) continue;

                        DataTable dtNew = childTables[j].Table.Clone();
                        foreach (var dr in list)
                        {
                            dtNew.ImportRow(dr);
                        }
                        var t = new DataResultItem()
                        {
                            ParentData = childTables[i],
                            ParentFile = childTables[i].ParentFile,
                            Table = dtNew,
                        };
                        res.Add(t);
                        tKey.Children.Add(t);
                    }
                   
                }
            }
            if(Info.Key!=null)
            Environment.CatchDataTables.Add(Info.Key, res);
        }

        void handleChildFileTableKey()
        {

            for (int i = 0; i < parentTables.Count; ++i)
            {
                var keyContents = parentTables[i].Table.AsEnumerable().Select(c => Convert.ToString(c[Info.ParentTableColumn])).Distinct();
                foreach (var it in keyContents)
                {
                    var tKey = getDescItem(parentTables[i], it);
                    childTables.ForEach(c =>
                    {
                        var fileTable = c.ParentFile.DataItems[c.ParentFile.Key];
                        if (Convert.ToString(fileTable.Table.Rows[0][Info.AssociateColumn]) == it)
                        {
                            tKey.Children.Add(c);
                        }
                    });
                }
               
                
            }
        }

        public void DoWork()
        {
            if(loadDataTables())
            {
                switch (Info.Type)
                {
                    case DataAssociateInfo.AssociateType.None:
                        return;
                    case DataAssociateInfo.AssociateType.InSameFile:
                    case DataAssociateInfo.AssociateType.AcrossFile:
                        handleChildAssociateColumn();
                        break;
                    case DataAssociateInfo.AssociateType.ChildFileColumn:
                        handleChildFileTableKey();
                        break;
                }

            }

        }
    }
}
