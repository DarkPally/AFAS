using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AFAS.Library.Android;
using System.Text.RegularExpressions;
namespace AFAS.Library.Core
{
    public class FileCatcherAndroid:FileCatcher
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

        override protected bool checkRootPathExist()
        {
            if(RootPathFileNames.ContainsKey(RootPath))
            {
                return RootPathFileNames[RootPath].Count>0;
            }
            else
            {
                InitAndroidFileExtractor();
                var res = androidFileExtracter.SearchFiles(androidDevice, RootPath,"*", Android.AndroidFileType.file);

                if(res.success)
                {
                    RootPathFileNames.Add(RootPath, res.filesName);
                    return res.filesName.Count > 0;
                }
                else
                {
                    throw new Exception("FileExtracter Error");
                }
                
            }
        }

        override public List<string> DoWork()
        {
            var res = new List<string>();

            if (checkRootPathExist())
            {
                var path = RootPath + RelativePath;
                var fileNames = RootPathFileNames[RootPath];

                if (IsRegEx)
                {
                    Regex r = new Regex(path, RegexOptions.None);
                    var MatchNames = fileNames.Where(c => r.IsMatch(c)).ToList();  
                    if(MatchNames.Count>0)
                    {
                        InitAndroidFileExtractor();
                        foreach(var it in MatchNames)
                        {
                            var pcPath = PCPath + it.Replace('/', '\\');
                            androidFileExtracter.CopyFileFromDevice(androidDevice, it, pcPath);
                            res.Add(pcPath);
                        }

                    }
                }
                else
                {
                    if(fileNames.Contains(path))
                    {
                        var pcPath = PCPath + path.Replace('/', '\\');
                        androidFileExtracter.CopyFileFromDevice(androidDevice, path, pcPath);
                        res.Add(pcPath);
                    }
                }
            }
            return res;
        }
    }
}
