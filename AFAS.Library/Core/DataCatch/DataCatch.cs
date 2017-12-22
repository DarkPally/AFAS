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

        List<FileCatchResultItem> filePaths;

        bool loadTargetFiles()
        {
            if (!Environment.CatchFilePaths.ContainsKey(Info.Key)) return false;
            filePaths = Environment.CatchFilePaths[Info.Key];
            if(filePaths.Count>0 && filePaths[0].DataItems.Count==0)
            {
                var tRes = new List<DataResultItem>();

                filePaths.ForEach(it =>
                {
                    var t = loadFileAttribute(it.FilePath);
                    var item = new DataResultItem()
                    {
                        ParentFile = it,
                        Table = t
                    };
                    it.DataItems.Add(Info.Key, item);
                    tRes.Add(item);
                });
                Environment.CatchDataTables.Add(Info.Key, tRes);
            }
            return true;
        }

        DataTable loadFileAttribute(string filePath)
        {
            DataTable dt = new DataTable();
            dt.Columns.AddRange(
                new DataColumn[]
                {
                        new DataColumn("FileName", System.Type.GetType("System.String")),
                        new DataColumn("RelativePath", System.Type.GetType("System.String")),
                        new DataColumn("LastWriteTime", System.Type.GetType("System.DateTime")),
                });

            FileInfo info = new FileInfo(filePath);
            DataRow dr = dt.NewRow();
            dr["FileName"] = info.Name;
            dr["RelativePath"] = info.FullName.Substring(Environment.PCPath.Length);
            dr["LastWriteTime"] = info.LastWriteTime;
            dt.Rows.Add(dr);
            dt.TableName = info.Name;

            return dt;
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

            var Results=new List<DataResultItem>();

            if (funcHandle != null)
            {
                foreach (var it in filePaths)
                {                    
                    var table = funcHandle(it.FilePath);
                    if(table!=null)
                    {
                        //table.TableName = Info.TableKey;
                        var item = new DataResultItem()
                        {
                            Key= Info.TableKey,
                            ParentFile = it,
                            Table = table
                        };
                        Results.Add(item);
                        it.DataItems.Add(Info.TableKey,item);
                    }
                }
            }
            else
            {
                foreach (var it in filePaths)
                {
                    var tables = handleDatabaseWithRegEx(it.FilePath);
                    if (tables.Count>0)
                    {
                        foreach(var t in tables)
                        {
                            var item = new DataResultItem()
                            {
                                Key = Info.TableKey,
                                ParentFile = it,
                                Table = t
                            };
                            Results.Add(item);
                            it.DataItems.Add(Info.TableKey, item);
                        }
                    }
                }
            }
            Environment.CatchDataTables.Add(Info.TableKey, Results);
            Environment.DataSource.Children.AddRange(Results);
        }
    }
}
