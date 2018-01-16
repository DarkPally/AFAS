using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Newtonsoft.Json;

namespace AFAS.Library
{
    public class ForensicResultItem
    {
       public string Key { get; set; }
       public DataTable Table { get; set; }

       [JsonIgnore]
       public ForensicResultItem ParentFile { get; set; }

       [JsonIgnore]
       public ForensicResultItem ParentData { get; set; }

       string desc;
       public string Desc
        {
            get
            {
                if(desc==null)
                {
                    if(MarkInfo!=null)
                    {
                        return MarkInfo.TableDesc;
                    }
                    if(Table!=null)
                    return Table.TableName;

                    return Key;
                }
                else
                {
                    return desc;
                }
            }
            set
            {
                desc = value;
            }
        }
       //复合表，即本身有Table，且子对象也有一样Key的Table，是分开的
       public bool IsMutiTableParent { get; set; } = false;

       List<ForensicResultItem> children;
       public List<ForensicResultItem> Children
        {
            get
            {
                return  children ?? (children = new List<ForensicResultItem>());
            }
            set
            {
                children = value;
            }
        }

       public bool IsMarkInfoFromParent = false;
       DataMarkInfo markInfo;
       public DataMarkInfo MarkInfo
        {
            get
            {
                if (IsMarkInfoFromParent)
                {
                    if (ParentData != null) return ParentData.markInfo;
                }
                return markInfo;
            }
            set
            {
                markInfo = value;
            }
        }



        [JsonIgnore]
        public ForensicResultItem ParentItemNode { get; set; }
        [JsonIgnore]
        bool hasParentNode = false;

        public void LoadParentNode()
        {
            if (hasParentNode) return;
            if(Children!=null)
            {
                foreach (var it in Children)
                {
                    it.ParentItemNode = this;
                    it.LoadParentNode();
                }
            }
            hasParentNode = true;


        }

        public List<ForensicResultItem> GetItemList()
        {
            List<ForensicResultItem> list = new List<ForensicResultItem>()
            {
                this,
            };
            if (Children != null)
            {
                foreach (var it in Children)
                {
                    list.AddRange(it.GetItemList());
                }
            }
            return list;
        }

        public List<object> GetColumnDataByMark(string mark)
        {
            var res = new List<object>();
            if (MarkInfo != null
                && MarkInfo.ColumnDescs != null)
            {
                foreach (var it in MarkInfo.ColumnDescs)
                {
                    if (it.Mark == mark)
                    {
                        foreach (DataRow dr in Table.Rows)
                        {
                            res.Add(dr[it.Desc]);
                        }
                    }
                }
            }
            return res;
        }

    }
}
