using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Newtonsoft.Json;

namespace AFAS.Library
{
    public class DataResultItem
    {
       public string Key { get; set; }
       public DataTable Table { get; set; }

       [JsonIgnore]
       public DataResultItem ParentFile { get; set; }

       [JsonIgnore]
       public DataResultItem ParentData { get; set; }

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

       List<DataResultItem> children;
       public List<DataResultItem> Children
        {
            get
            {
                return  children ?? (children = new List<DataResultItem>());
            }
            set
            {
                children = value;
            }
        }

       DataMarkInfo markInfo;
       public DataMarkInfo MarkInfo
        {
            get
            {
                if (markInfo == null)
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
    }
}
