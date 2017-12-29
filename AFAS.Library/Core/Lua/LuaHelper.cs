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
        public static DataResultItem GetFirstItemCatched(PackageForensic package,string TableKey )
        {
            return package.CatchDataTables[TableKey].First();
        }

        public static object GetFirstColumnDataFromItem(DataResultItem item, string ColumnName)
        {
            return item.Table.Rows[0][ColumnName];
        }

        public static object GetFirstColumnData(PackageForensic package, string TableKey, string ColumnName)
        {
            return package.CatchDataTables[TableKey].First().Table.Rows[0][ColumnName];
        }

        public static object GetFirstColumnDataFromParentFile(FileCatchResultItem file, string TableKey, string ColumnName)
        {
            return file.DataItems[TableKey].Table.Rows[0][ColumnName];
        }

        public static void CreateColumn(DataTable table, string ColumnName)
        {
            if (table.Columns.Contains(ColumnName)) return;
            table.Columns.Add(new DataColumn(ColumnName));
        }
    }
}
