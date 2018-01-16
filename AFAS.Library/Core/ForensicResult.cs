using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Newtonsoft.Json;
using System.IO;

namespace AFAS.Library
{
    public class ForensicResult
    {
        public List<ForensicResultItem> Items { get; set; }

        void loadTableColumnDesc(ForensicResultItem item,ref HashSet<ForensicResultItem> buffer)
        {
            if (buffer.Contains(item)) return;
            buffer.Add(item);
            if (item.Table != null)
            {
                if (item.MarkInfo!=null && item.MarkInfo.ColumnDescs != null)
                {
                    if (item.MarkInfo.OnlyShowDesc == true)
                    {
                        var tNames = item.MarkInfo.ColumnDescs.Select(c => c.Name).ToArray();
                        item.Table = item.Table.DefaultView.ToTable(false, tNames);
                    }

                    foreach (var it in item.MarkInfo.ColumnDescs)
                    {
                        if (it.Desc != null && item.Table.Columns[it.Name]!=null)
                            item.Table.Columns[it.Name].ColumnName = it.Desc;
                    }
                }
            }
            if(item.Children!=null)
            {
                foreach (var it in item.Children)
                {
                    loadTableColumnDesc(it,ref buffer);
                }
            }
            
        }

        public ForensicResult LoadTableColumnDesc()
        {
            var buffer = new HashSet<ForensicResultItem>();
            foreach(var it in Items)
            {
                loadTableColumnDesc(it,ref buffer);
            }
            return this;
        }


        static public void SaveForensicResult(ForensicResult res, string path,bool includeAll=false)
        {
            if(!includeAll)
            {
                res.Items.ForEach(c =>
                {
                    c.Children.RemoveAll(s =>s.Desc== "数据集合");
                });
            }

            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
            jsonSerializerSettings.Formatting = Formatting.Indented;
            jsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            var st = JsonConvert.SerializeObject(res, jsonSerializerSettings);
            var by = Encoding.Default.GetBytes(st);

            using (var fs = File.Create(path))
            {
                fs.Write(by, 0, by.Length);
            }
        }

        static public ForensicResult LoadForensicResult(string path)
        {
            var t = File.ReadAllText(path, Encoding.Default);
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.TypeNameHandling = TypeNameHandling.Auto;

            return JsonConvert.DeserializeObject<ForensicResult>(t, jsonSerializerSettings);
        }


        public List<ForensicResultItem> GetItemList()
        {
            var res = new List<ForensicResultItem>();
            foreach(var it in Items)
            {
                it.LoadParentNode();
                res.AddRange(it.GetItemList());
            }
            return res;
        }

        public List<ForensicResultItem> GetItemWithMark(string mark)
        {
            return GetItemList().Where(c => c.MarkInfo != null && c.MarkInfo.ColumnDescs != null
            && c.MarkInfo.ColumnDescs.Exists(m => m.Mark == mark)).ToList();
        }
    }
}
