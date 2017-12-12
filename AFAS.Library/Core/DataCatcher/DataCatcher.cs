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
    public class DataCatcher
    {
        public PackageForensic Environment { get; set; }
        public DataCatcherInfo Info { get; set; }

        List<string> filePaths;

        public DataSet Result { get; set; }

        static DataTable fileTableTemplate;

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
        void loadTargetFiles()
        {
            filePaths = Environment.CatchFilePaths[Info.Key];
        }

        DataTable loadFileAttribute()
        {
            initFileTable();
            DataTable dt = fileTableTemplate.Copy();

            foreach (var it in filePaths)
            {
                FileInfo info = new FileInfo(it);
                DataRow dr = dt.NewRow();
                dr["FileName"] = info.Name;
                dr["RelativePath"] = info.FullName.Substring(Environment.PCPath.Length);
                dr["LastWriteTime"] = info.LastWriteTime;
                dt.Rows.Add(dr);
            }        
            return dt;
        }


        void handleBinary()
        {
            Result = new DataSet();
            Result.Tables.Add(loadFileAttribute());

        }

        void handleText()
        {
            Result = new DataSet();
            Result.Tables.Add(loadFileAttribute());
        }

        void handleXml()
        {
            Result = new DataSet();
            Result.Tables.Add(loadFileAttribute());
        }

        void handleDatabase()
        {
            Result = new DataSet();
            Result.Tables.Add(loadFileAttribute());

            foreach (var it in filePaths)
            {
                //DataCatcherHelper.get
            }
        }

        public void DoWork()
        {
            loadTargetFiles();

            switch (Info.Type)
            {
                case DataCatcherInfo.DataType.Binary:
                    handleBinary();
                    break;
                case DataCatcherInfo.DataType.Text:
                    handleText();
                    break;
                case DataCatcherInfo.DataType.Xml:
                    handleXml();
                    break;
                case DataCatcherInfo.DataType.Database:
                    handleDatabase();
                    break;
                default:
                    break;
            }
           
        }
    }
}
