using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace AFAS.Library
{
    public class DataResultItem
    {
       
       public DataTable Table { get; set; }

       public FileCatchResultItem ParentFile { get; set; }
       public DataResultItem ParentData { get; set; }

       public string KeyContent { get; set; }

       string desc;
       public string Desc
        {
            get
            {
                if(desc==null)
                {
                    if(MarkInfo!=null)
                    {
                        return MarkInfo.TableDesc + " " + KeyContent;
                    }
                    return Table.TableName + " " + KeyContent;
                }
                else
                {
                    return desc + " " + KeyContent;
                }
            }
            set
            {
                desc = value;
            }
        }

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
