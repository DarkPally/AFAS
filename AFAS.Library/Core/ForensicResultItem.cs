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

    }
}
