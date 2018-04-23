using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AFAS.Library.Android;

namespace AFAS.Library.Android
{
    /// <summary>
    /// linux下的7种文件类型
    /// 不过在data目录中应该只有部分类型会存在
    /// </summary>
    public enum AndroidFileType
    {
        alltype = 'a',
        fne = 'w', // file not exist
        directory = 'd',
        file = 'f',
        link = 'l',
        block = 'b',
        character = 'c',
        socket = 's',
        pipe = 'p'
    }

    /// <summary>
    /// 存放文件详细信息的数据结构
    /// </summary>
    public class FileProperty
    {
        public AndroidFileType Type { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public string AccessTime { get; set; }
        public string ModifyTime { get; set; }
    }

    /// <summary>
    /// 函数调用之后统一返回的结果类型
    /// </summary>
    // enum State { noConnection, copyFail, invalidInput, unexpectedOutput, fileNotExist};
    public class FileResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        // public State state;
        public List<FileProperty> FilePropertys { get; set; }
    }

    public class AndroidFileExtracter
    {
        string[] devices;

        public string[] Devices
        {
            get { return devices; }
        }


        public FileResult InitConnection()
        {
            FileResult result = new FileResult();
            try
            {
                devices = AdbHelper.GetDevices();
                if (devices.Length == 0)
                {
                    throw new Exception("No device detected!");
                }
                else
                {
                    result.Success = true;
                    devices = AdbHelper.GetDevices();
                }

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.ToString();
            }
            return result;
        }

        List<string> errorPatterns = new List<string>()
        {
            "No such file or directory",
            "Permission denied"
        };

        void checkResult(string str)
        {            
            foreach(var it in errorPatterns)
            {
                if (str.Contains(it)) throw new Exception(it);
            }
        }
        #region stat Property
        List<FileProperty> parsePropertyFromStat(string[] rawData, string path = "")
        {
            List<FileProperty> result = new List<FileProperty>();

            for (int i = 0; i < rawData.Length - 6; i += 7)
            {
                if (rawData[i].Contains("No such file or directory")) continue;
                if (!rawData[i].Contains("File")) continue;

                FileProperty property = new FileProperty();
                property.ModifyTime = rawData[i + 5].Substring(8, rawData[5].Length - 8);
                property.AccessTime = rawData[i + 4].Substring(8, rawData[4].Length - 8);

                if (rawData[i + 1].Contains("directory")) property.Type = AndroidFileType.directory;
                if (rawData[i + 1].Contains("regular")) property.Type = AndroidFileType.file;
                if (rawData[i + 1].Contains("symbol")) property.Type = AndroidFileType.link;


                string sizePattern = @"(?<=Size: )\d*\b";
                property.Size = Regex.Matches(rawData[i + 1], sizePattern)[0].ToString();
                string pathPattern = @"(?<=File: ).*";
                property.Path = Regex.Matches(rawData[i], pathPattern)[0].ToString().Replace("'", "");


                result.Add(property);
            }
            return result;
        }

        FileProperty getPropertyFromStat(string device, string path)
        {
            FileProperty property = new FileProperty();

            var rawData = AdbHelper.GetProperty(device, path);
            checkResult(rawData[0]);
            property = parsePropertyFromStat(rawData)[0];

            return property;
        }

        public FileResult GetFileInformationFromStat(string device, string path)
        {
            FileResult result = new FileResult();
            try
            {
                result.FilePropertys.Add(getPropertyFromStat(device, path));
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.ToString();
            }
            return result;
        }

        #endregion
        #region ls cmd
        public FileResult ListDirecotry(string device, string path)
        {
            FileResult result = new FileResult();
            try
            {
                result.FilePropertys = new List<FileProperty>();
                foreach (string file in AdbHelper.ListDataFolder(device, path))
                        result.FilePropertys.Add(new FileProperty() { Name=file});
                checkResult(result.FilePropertys[0].Name);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.ToString();
            }
            return result;

        }

        FileProperty parsePropertyFromLS(string rawData, string path)
        {
            var res = rawData.Split(new char[] { ' ' },StringSplitOptions.RemoveEmptyEntries);
            if (res.Length <= 6) return null;
            return new FileProperty()
            {
                Name = res[6],
                Path = path +"/"+ res[6],
                ModifyTime = res[4] + " " + res[5],
                Size = res[3],
            };
            
        }
        List<FileProperty> parsePropertyFromLS(string[] rawData,string path)
        {
            List<FileProperty> result = new List<FileProperty>();

            foreach (var it in rawData)
            {
                var property = parsePropertyFromLS(it, path);
                if(property!=null) result.Add(property);
            }
            return result;
        }

        List<FileProperty> parsePropertyFromLSRecrusive(string[] rawData, string path)
        {
            List<FileProperty> result = new List<FileProperty>();
            int lastIndex = rawData.Length-1;

            for (int i = lastIndex;i>=0;--i)
            {                
                if(rawData[i].StartsWith(path))
                {
                    var tPath = rawData[i].Substring(0, rawData[i].Length-1);
                    for(int j=i+2;j<= lastIndex;++j)
                    {
                        var property = parsePropertyFromLS(rawData[j], tPath);
                        if (property != null) result.Add(property);
                    }
                    lastIndex = i - 1;
                }
            }
            return result;
        }

        public FileResult ListDirecotryInDetail(string device, string path)
        {
            if (path[path.Length - 1] != '/') path += '/';
            string listScript= System.String.Format("ls -ls {0}",path);
            FileResult result = new FileResult();
            try
            {
                var properities = AdbHelper.RunShell(device, listScript);
                checkResult(properities[0]);
                result.FilePropertys = parsePropertyFromLS(properities,path);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.ToString();
            }
            return result;
        }

        public FileResult ListDirecotryRecrusiveInDetail(string device, string path)
        {
            if (path[path.Length - 1] == '/') path = path.Substring(0, path.Length - 1); ;
            string listScript = System.String.Format("ls -Rls {0}", path);
            FileResult result = new FileResult();
            try
            {
                var properities = AdbHelper.RunShell(device, listScript);
                checkResult(properities[0]);
                result.FilePropertys = parsePropertyFromLSRecrusive(properities, path);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.ToString();
            }
            return result;
        }
        #endregion
        public FileResult SearchFiles(string device, string path, string pattern, AndroidFileType fileType)
        {
            FileResult result = new FileResult();
            try
            {
                result.FilePropertys = new List<FileProperty>();
                foreach (string file in AdbHelper.SearchFiles(device, path, pattern, (char)fileType))
                    result.FilePropertys.Add(new FileProperty() { Name = file });
                checkResult(result.FilePropertys[0].Name);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.ToString();
            }
            return result;

        }

        public FileResult CopyFileFromDevice(string device, string devicePath, string pcPath)
        {
            FileResult result = new FileResult();
            try
            {

                FileProperty property = getPropertyFromStat(device, devicePath);
                if (property.Type == AndroidFileType.fne)
                    throw new Exception("Wrong path");

                var pcDictionary = System.IO.Path.GetDirectoryName(pcPath);
                if (!System.IO.Directory.Exists(pcDictionary))
                    System.IO.Directory.CreateDirectory(pcDictionary);

                if (property.Type == AndroidFileType.directory)
                {
                    string fullPath = pcPath + '/' + property.Path.Replace('/', '_');
                    if (!System.IO.Directory.Exists(fullPath))
                        System.IO.Directory.CreateDirectory(fullPath);
                    AdbHelper.CopyFromDevice(device, devicePath, fullPath);
                }
                else
                    AdbHelper.CopyFromDevice(device, devicePath, pcPath);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.ToString();
            }
            return result;
        }

        #region old Verbose
        /*
        public FileResult ListDirecotryVerbose(string device, string path)
        {
            string listScript;
            if (path == "/")
            {
                listScript = System.String.Format(
                                                  "ls {0}|while read i;do " +
                                                  "echo \\\"$(stat \\\"/$i\\\")\\\";" +
                                                  "done", path);
            }
            else
            {
                listScript = System.String.Format(
                                                  "ls {0}|while read i;do " +
                                                  "echo \\\"$(stat \\\"{0}/$i\\\")\\\";" +
                                                  "done", path);
            }

            FileResult result = new FileResult();
            try
            {
                FileProperty property = getPropertyFromStat(device, path);
                if (property.Type == AndroidFileType.directory)
                {
                    var properities = AdbHelper.RunShell(device, listScript);
                    result.FilePropertys = parsePropertyFromStat(properities);
                    result.Success = true;
                }
                else
                {
                    throw new Exception("Wrong path!");
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.ToString();
            }
            return result;

        }

        public FileResult SearchFilesVerbose(string device, string path, string pattern, AndroidFileType fileType)
        {

            string searchScript;
            if (fileType == AndroidFileType.alltype)
            {
                searchScript = System.String.Format(
                                                   "find {0} -name \\\"{1}\\\"|" +
                                                   "while read i;do " +
                                                   "echo \\\"$(stat \\\"$i\\\")\\\"; " +
                                                   "done", path, pattern);
            }
            else
            {
                searchScript = System.String.Format(
                                                   "find {0} -name \\\"{1}\\\" -type {2}|" +
                                                   "while read i;do " +
                                                   "echo \\\"$(stat \\\"$i\\\")\\\"; " +
                                                   "done", path, pattern, (char)fileType);
            }

            FileResult result = new FileResult();
            try
            {
                var properities = AdbHelper.RunShell(device, searchScript);
                result.FilePropertys = parsePropertyFromStat(properities);

                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.ToString();
            }
            return result;

        }
        */
        #endregion
    }
}
