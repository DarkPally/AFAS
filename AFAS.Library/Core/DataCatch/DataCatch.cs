using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Data.SQLite;

namespace AFAS.Library
{
    public class DataCatch
    {
        public PackageForensic Environment { get; set; }
        public DataCatchInfo Info { get; set; }

        ForensicResultItem fileResultItem;
        List<string> filePaths;
        bool loadTargetFiles()
        {
            if (!Environment.CatchDataTables.ContainsKey(Info.Key)) return false;
            fileResultItem = Environment.CatchDataTables[Info.Key][0];
            filePaths = new List<string>();
            for (int i=0;i< fileResultItem.Table.Rows.Count;++i)
            {
                filePaths.Add(Convert.ToString( fileResultItem.Table.Rows[i]["PCPath"]));
            }
            return true;
        }

        DataTable handleBinary(string filePath)
        {
            return DataCatchHelper.GetBinaryDataTable(filePath, Info.Chunk);

        }

        DataTable handleText(string filePath)
        {
            return DataCatchHelper.GetFileDataTable(filePath, Info.DataPath);
        }

        DataTable handleXml(string filePath)
        {
            return DataCatchHelper.GetXmlDataTable(filePath, Info.DataPath);
        }

        DataTable handleDatabase(string filePath)
        {
            return DataCatchHelper.GetDbDataTable(filePath, Info.DataPath);
        }

        public List<DataTable> handleDatabaseWithRegEx(string filePath)
        {
            return DataCatchHelper.GetDbDataTableWithRegEx(filePath, Info.DataPath);
        }

        public void DoWork()
        {
            if (!loadTargetFiles()) return;

            Func<string, DataTable> funcHandle=null;

            switch (Info.Type)
            {
                case DataCatchInfo.DataType.None:
                    return;
                case DataCatchInfo.DataType.Binary:
                    funcHandle = handleBinary;
                    break;
                case DataCatchInfo.DataType.Text:
                    funcHandle =  handleText;
                    break;
                case DataCatchInfo.DataType.Xml:
                    funcHandle = handleXml;
                    break;
                case DataCatchInfo.DataType.Database:
                    funcHandle= handleDatabase;
                    break;
                default:
                    break;
            }

            var Results=new List<ForensicResultItem>();

            if (funcHandle != null)
            {
                int i = 0;
                foreach (var it in filePaths)
                {                    
                    var table = funcHandle(it);
                    if(Info.Select != null)
                    {
                        var drArr = table.Select(Info.Select);
                        DataTable dtNew = table.Clone();
                        for (int j = 0; j < drArr.Length; j++)
                        {
                            dtNew.ImportRow(drArr[j]);
                        }
                        table = dtNew;
                    }
                    if(table!=null)
                    {
                        //table.TableName = Info.TableKey;
                        var item = new ForensicResultItem()
                        {
                            Key= Info.TableKey,
                            ParentFile = fileResultItem.IsMutiTableParent? fileResultItem.Children[i]:fileResultItem,
                            Table =  table                            
                        };
                        if (Info.Desc != null) item.Desc = Info.Desc;
                        Results.Add(item);
                        if(Info.CatchToFileDataTree)
                        item.ParentFile.Children.Add(item);
                    }
                    ++i;
                }
            }
            else
            {
                int i = 0;
                foreach (var it in filePaths)
                {
                    var tables = handleDatabaseWithRegEx(it);
                    if (tables.Count>0)
                    {
                        var itemParemt = new ForensicResultItem()
                        {
                            Key = Info.TableKey,
                            IsMutiTableParent=true,
                            ParentFile = fileResultItem.IsMutiTableParent ? fileResultItem.Children[i] : fileResultItem,
                        };

                        if (Info.Desc != null) itemParemt.Desc = Info.Desc;
                        foreach (var t in tables)
                        {
                            var table = t;
                            if (Info.Select != null)
                            {
                                var drArr = table.Select(Info.Select);
                                DataTable dtNew = table.Clone();
                                for (int j = 0; j < drArr.Length; j++)
                                {
                                    dtNew.ImportRow(drArr[j]);
                                }
                                table = dtNew;
                            }
                            var item = new ForensicResultItem()
                            {
                                Key = Info.TableKey,
                                ParentFile = itemParemt.ParentFile,
                                Table = table
                            };
                            itemParemt.Children.Add(item);
                        }
                        Results.Add(itemParemt);
                        if (Info.CatchToFileDataTree)
                            itemParemt.ParentFile.Children.Add(itemParemt);
                    }
                    ++i;
                }
            }
            Environment.CatchDataTables.Add(Info.TableKey, Results);
            Environment.DataSource.Children.AddRange(Results);
        }
    }
}
