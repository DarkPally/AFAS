using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Data.SQLite;

namespace AFAS.Library.Core
{
    public class DataCatch
    {
        public PackageForensic Environment { get; set; }
        public DataCatchInfo Info { get; set; }

        List<string> filePaths;

        static DataTable fileTableTemplate;
        //const string fileTableName = "FileInfo";
        void initFileTable()
        {
            if(fileTableTemplate==null)
            {
                fileTableTemplate = new DataTable();
                fileTableTemplate.Columns.AddRange(
                    new DataColumn[]
                    {
                        new DataColumn("FileName", System.Type.GetType("System.String")),
                        new DataColumn("RelativePath", System.Type.GetType("System.String")),
                        new DataColumn("LastWriteTime", System.Type.GetType("System.DateTime")),
                    });
            }
        }
        bool loadTargetFiles()
        {
            if (!Environment.CatchFilePaths.ContainsKey(Info.Key)) return false;
            filePaths = Environment.CatchFilePaths[Info.Key];
            return true;
        }

        DataTable loadFileAttribute(string filePath)
        {
            initFileTable();
            DataTable dt = fileTableTemplate.Copy();

            fileTableTemplate.TableName = Info.Key;

            FileInfo info = new FileInfo(filePath);
            DataRow dr = dt.NewRow();
            dr["FileName"] = info.Name;
            dr["RelativePath"] = info.FullName.Substring(Environment.PCPath.Length);
            dr["LastWriteTime"] = info.LastWriteTime;
            dt.Rows.Add(dr);

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

        public void DoWork()
        {
            if (!loadTargetFiles()) return;

            Func<string, DataTable> funcHandle=null;

            switch (Info.Type)
            {
                case DataCatchInfo.FileType.FileAttribute:
                    funcHandle = loadFileAttribute;
                    break;
                case DataCatchInfo.FileType.Binary:
                    funcHandle = handleBinary;
                    break;
                case DataCatchInfo.FileType.Text:
                    funcHandle =  handleText;
                    break;
                case DataCatchInfo.FileType.Xml:
                    funcHandle = handleXml;
                    break;
                case DataCatchInfo.FileType.Database:
                    funcHandle= handleDatabase;
                    break;
                default:
                    break;
            }

            List<DataTable> Results=new List<DataTable>();

            if (funcHandle != null)
            {
                foreach (var it in filePaths)
                {
                    var table = funcHandle(it);
                    if(table!=null)
                    {
                        table.TableName = Info.TableKey;
                        Results.Add(table);   
                    }
                }
            }
            Environment.CatchDataTables.Add(Info.TableKey, Results);
           
        }
    }
}
