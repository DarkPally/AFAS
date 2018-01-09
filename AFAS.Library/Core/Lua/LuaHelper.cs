using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Data;

namespace AFAS.Library
{
    public class LuaHelper
    {
        public static ForensicResultItem GetFirstItemCatched(PackageForensic package,string TableKey )
        {
            return package.CatchDataTables[TableKey].First();
        }

        public static object GetFirstColumnDataFromItem(ForensicResultItem item, string ColumnName)
        {
            return item.Table.Rows[0][ColumnName];
        }

        public static object GetFirstColumnData(PackageForensic package, string TableKey, string ColumnName)
        {
            return package.CatchDataTables[TableKey].First().Table.Rows[0][ColumnName];
        }

        public static object GetFirstColumnDataFromParentFile(ForensicResultItem fileItem, string TableKey, string ColumnName)
        {
            return fileItem.Children.First(c=>c.Key==TableKey).Table.Rows[0][ColumnName];
        }

        public static void CreateColumn(DataTable table, string ColumnName)
        {
            if (table.Columns.Contains(ColumnName)) return;
            table.Columns.Add(new DataColumn(ColumnName));
        }
    }
}
