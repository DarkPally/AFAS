using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AFAS.Library.Android;
using System.Text.RegularExpressions;
namespace AFAS.Library
{
    public class FileCatchAndroid:FileCatch
    {
        

        AndroidFileExtracter androidFileExtracter=null;
        string androidDevice;

        void InitAndroidFileExtractor()
        {
            if(androidFileExtracter==null)
            {
                androidFileExtracter = new AndroidFileExtracter();
                androidFileExtracter.InitConnection();
                androidDevice = androidFileExtracter.Devices.First();
            }
        }

        bool checkRootPathExist()
        {
            if(RootPathFileNames.ContainsKey(Info.RootPath))
            {
                return RootPathFileNames[Info.RootPath].Count>0;
            }
            else
            {
                InitAndroidFileExtractor();
                var res = androidFileExtracter.SearchFiles(androidDevice, Info.RootPath, "*", Android.AndroidFileType.file);

                if(res.success)
                {
                    RootPathFileNames.Add(Info.RootPath, res.filesName);
                    return res.filesName.Count > 0;
                }
                else
                {
                    throw new Exception("FileExtracter Error");
                }
                
            }
        }

        void doFileCatchFromRootPath(string rootPath)
        {
            var res = new List<ForensicResultItem>();
            var path = rootPath + RelativePath;
            var fileNames = RootPathFileNames[rootPath];

            if (IsRegEx)
            {
                Regex r = new Regex(path, RegexOptions.None);
                var MatchNames = fileNames.Where(c => r.IsMatch(c)).ToList();
                if (MatchNames.Count > 0)
                {
                    InitAndroidFileExtractor();
                    var pcPaths = new List<string>();
                    var PathMacthes = new List<string>();

                    foreach (var it in MatchNames)
                    {
                        var tg = r.Match(it).Groups;
                        var pcPath = PCPath + it.Replace('/', '\\');
                        androidFileExtracter.CopyFileFromDevice(androidDevice, it, pcPath);
                        pcPaths.Add(pcPath);
                        PathMacthes.Add(tg[tg.Count - 1].Value);
                    }
                    res.Add(loadFileAttribute(pcPaths, PathMacthes));

                }
            }
            else
            {
                if (fileNames.Contains(path))
                {
                    var pcPath = PCPath + path.Replace('/', '\\');
                    androidFileExtracter.CopyFileFromDevice(androidDevice, path, pcPath);
                    res.Add(loadFileAttribute(pcPath));
                }
            }
            Environment.CatchDataTables.Add(Info.Key, res);
            Environment.DataSource.Children.AddRange(res);
        }

        override protected void doWorkRootDefault()
        {
            if (checkRootPathExist())
            {
                doFileCatchFromRootPath(Info.RootPath);
            }
            
        }
    }
}
