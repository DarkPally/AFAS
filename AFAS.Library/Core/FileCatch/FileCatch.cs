using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AFAS.Library.Android;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;

namespace AFAS.Library
{
    public class FileCatch
    {
        public PackageForensic Environment { get; set; }
        public FileCatchInfo Info { get; set; }
        //RegEx,Normal
        public bool IsRegEx { get { return Info.IsRegEx; } }

        public string RelativePath { get { return Info.RelativePath; } }
        //强制统一不以/开头以/为结尾
        //不填即默认初始化为data/data/[PackageName]/
        //public string RootPath { get { return Info.RootPath; } }

        //PC根目录,统一带有\\结尾
        public string PCPath { get { return Environment.PCPath; } }

        protected Dictionary<string, List<string>> RootPathFileNames { get { return AFASManager.Instance.RootPathFileNames; } }

        protected Dictionary<string, List<string>> PrepareRootPaths { get { return Environment.PrepareRootPaths; } }

        protected List<string> PCRootFiles { get { return AFASManager.Instance.PCRootFiles; } }

        public void DoWork()
        {
            switch (Info.RootPathType)
            {
                case RootPathType.Default:
                    doWorkRootDefault();
                    break;
                case RootPathType.Regex:
                    doWorkRootRegex();
                    break;
                case RootPathType.PathPrepare:
                    doWorkRootPrepare();
                    break;
            }
        }

        virtual protected void doWorkRootDefault()
        {
        }
        virtual protected void doWorkRootRegex()
        {
        }
        virtual protected void doWorkRootPrepare()
        {
        }

        protected ForensicResultItem loadFileAttribute(string filePath)
        {
            DataTable dt = new DataTable();
            dt.Columns.AddRange(
                new DataColumn[]
                {
                        new DataColumn("FileName", System.Type.GetType("System.String")),
                        new DataColumn("RelativePath", System.Type.GetType("System.String")),
                        new DataColumn("PCPath", System.Type.GetType("System.String")),
                        new DataColumn("LastWriteTime", System.Type.GetType("System.DateTime")),
                });

            FileInfo info = new FileInfo(filePath);
            DataRow dr = dt.NewRow();
            dr["FileName"] = info.Name;
            dr["RelativePath"] = info.FullName.Substring(Environment.PCPath.Length);
            dr["LastWriteTime"] = info.LastWriteTime;
            dr["PCPath"] = filePath;
            dt.Rows.Add(dr);
            dt.TableName = Info.Key;

            var res = new ForensicResultItem() { Key = Info.Key, Table = dt };
            res.ParentFile = res;
            return res;
        }
        protected ForensicResultItem loadFileAttribute(List<string> filePaths,List<string> filePathMatches)
        {
            DataTable dt = new DataTable();
            dt.Columns.AddRange(
                new DataColumn[]
                {
                        new DataColumn("FileName", System.Type.GetType("System.String")),
                        new DataColumn("RelativePath", System.Type.GetType("System.String")),
                        new DataColumn("PCPath", System.Type.GetType("System.String")),
                        new DataColumn("LastWriteTime", System.Type.GetType("System.DateTime")),
                        new DataColumn("FileNameMatch", System.Type.GetType("System.String")),
                });
            dt.TableName = Info.Key;
            var res = new ForensicResultItem() { Key = Info.Key, Table = dt,
                IsMutiTableParent = true
            };
            for (int i=0;i< filePaths.Count;++i)
            {
                FileInfo info = new FileInfo(filePaths[i]);
                DataRow dr = dt.NewRow();
                dr["FileName"] = info.Name;
                dr["RelativePath"] = info.FullName.Substring(Environment.PCPath.Length);
                dr["LastWriteTime"] = info.LastWriteTime;
                dr["FileNameMatch"] = filePathMatches[i];
                dr["PCPath"] = filePaths[i];
                dt.Rows.Add(dr);
                var tTable=dt.Clone();
                tTable.TableName = filePathMatches[i];
                tTable.ImportRow(dr);
                var t = new ForensicResultItem()
                {
                    Key = Info.Key,
                    ParentData = res,
                    Table = tTable,
                };
                t.ParentFile = t;
                res.Children.Add(t);
            }
            return res;
        }
    }
}
