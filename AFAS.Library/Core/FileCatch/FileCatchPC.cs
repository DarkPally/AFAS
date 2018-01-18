using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace AFAS.Library
{
    public class FileCatchPC:FileCatch
    {

        bool checkRootPathExist(string rootPath)
        {
            if (RootPathFileNames.ContainsKey(rootPath))
            {
                return RootPathFileNames[rootPath].Count > 0;
            }
            else
            {
                var pcRoot = PCPath + rootPath.Replace('/', '\\');
                try
                {
                    DirectoryInfo theFolder = new DirectoryInfo(pcRoot);
                    FileInfo[] thefileInfo = theFolder.GetFiles("*.*", SearchOption.AllDirectories);
                    RootPathFileNames.Add(rootPath, thefileInfo.Select(c => c.FullName.Substring(PCPath.Length)).ToList());
                }
                catch
                {
                    RootPathFileNames.Add(rootPath, new List<string>());
                }
                return RootPathFileNames[rootPath].Count > 0;
            }
        }

        void doFileCatchFromRootPath(string rootPath)
        {
            var res = new List<ForensicResultItem>();
            var path = rootPath + RelativePath;
            var realPath = path.Replace("/", "\\");
            var regexPath = path.Replace("/", "\\\\");
            var fileNames = RootPathFileNames[rootPath];

            if (IsRegEx)
            {
                Regex r = new Regex(regexPath);
                var MatchNames = fileNames.Where(c => r.IsMatch(c)).ToList();
                if (MatchNames.Count > 0)
                {
                    var pcPaths = new List<string>();
                    var PathMacthes = new List<string>();

                    foreach (var it in MatchNames)
                    {
                        var tg = r.Match(it).Groups;
                        var pcPath = PCPath + it;
                        pcPaths.Add(pcPath);
                        PathMacthes.Add(tg[tg.Count - 1].Value);
                    }
                    res.Add(loadFileAttribute(pcPaths, PathMacthes));

                }
            }
            else
            {
                if (fileNames.Contains(realPath))
                {
                    res.Add(loadFileAttribute(PCPath + realPath));
                }
            }
            Environment.CatchDataTables.Add(Info.Key, res);
            Environment.DataSource.Children.AddRange(res);
        }

        override protected void doWorkRootDefault()
        {
            if (checkRootPathExist(Info.RootPath))
            {
                doFileCatchFromRootPath(Info.RootPath);
            }
        }

        bool prepareRootPath()
        {
            if (PrepareRootPaths.ContainsKey(Info.RootPath))
            {
                return PrepareRootPaths[Info.RootPath].Count > 0;
            }
            else
            {
                if (PCRootFiles.Count == 0)
                {
                    DirectoryInfo theFolder = new DirectoryInfo(PCPath);
                    FileInfo[] thefileInfo = theFolder.GetFiles("*.*", SearchOption.AllDirectories);
                    PCRootFiles = thefileInfo.Select(c => c.FullName.Substring(PCPath.Length)).ToList();
                }

                var regexPath = (string.Format("(?<CatchedRootPath>.*?){0}",
                       Info.RootPathPrepareRegexes[0].Replace('/', '\\'))).Replace("\\", "\\\\");

                Regex regex = new Regex(regexPath);


                var catchedRootPaths = PCRootFiles.Where(c => regex.IsMatch(c))
                    .Select(c => regex.Match(c).Groups["CatchedRootPath"].Value).Distinct().ToList();

                foreach (var it in catchedRootPaths)
                {
                    for (int i = 1; i < Info.RootPathPrepareRegexes.Count; ++i)
                    {
                        var path = string.Format("{0}{1}", it, Info.RootPathPrepareRegexes[i]);
                        if(!PCRootFiles.Exists(c=>c== path))
                        {
                            catchedRootPaths.Remove(it);
                            break;
                        }
                    }
                }
                for(int i=0;i< catchedRootPaths.Count;++i)
                {
                    catchedRootPaths[i] = catchedRootPaths[i].Replace("\\", "/");
                }
                PrepareRootPaths.Add(Info.RootPath, catchedRootPaths);
                return PrepareRootPaths[Info.RootPath].Count > 0;
            }
        }

        override protected void doWorkRootPrepare()
        {
            if(prepareRootPath())
            {
                var root = PrepareRootPaths[Info.RootPath].First();
                //目前只做第一个匹配到的路径，因为一个用户不可能有两个
                if (checkRootPathExist(root))
                {
                    doFileCatchFromRootPath(root);
                }
            }
        }
    }
}
