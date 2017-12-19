using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace AFAS.Library.Core
{
    public class DataResultItem
    {
       
       public DataTable Table { get; set; }

       public FileCatchResultItem ParentFile { get; set; }
       public DataResultItem ParentData { get; set; }

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
