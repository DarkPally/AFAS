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

        //RegEx,Normal
        override protected bool checkRootPathExist()
        {
            if (RootPathFileNames.ContainsKey(RootPath))
            {
                return RootPathFileNames[RootPath].Count > 0;
            }
            else
            {
                var pcRoot = PCPath + RootPath.Replace('/', '\\');
                try
                {
                    DirectoryInfo theFolder = new DirectoryInfo(pcRoot);
                    FileInfo[] thefileInfo = theFolder.GetFiles("*.*", SearchOption.AllDirectories);
                    RootPathFileNames.Add(RootPath, thefileInfo.Select(c=>c.FullName.Substring(PCPath.Length)).ToList());
                }
                catch
                {
                    RootPathFileNames.Add(RootPath, new List<string>());
                }
                return RootPathFileNames[RootPath].Count > 0;
            }
        }

        override public void DoWork()
        {

            if (checkRootPathExist())
            {

                var res = new List<ForensicResultItem>();
                var path = RootPath + RelativePath;
                var realPath = path.Replace("/", "\\");
                var regexPath = path.Replace("/", "\\\\");
                var fileNames = RootPathFileNames[RootPath];

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
        }
    }
}
