﻿using System;
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

                var res = new List<FileCatchResultItem>();
                var path = RootPath + RelativePath;
                path = path.Replace('/', '\\');
                var fileNames = RootPathFileNames[RootPath];

                if (IsRegEx)
                {
                    Regex r = new Regex(path, RegexOptions.None);
                    var MatchNames = fileNames.Where(c => r.IsMatch(c)).ToList();
                    if (MatchNames.Count > 0)
                    {
                        foreach (var it in MatchNames)
                        {
                            res.Add(new FileCatchResultItem()
                            {
                                FilePath = PCPath + path,
                                DataItems = new List<DataResultItem>(),
                            });
                        }

                    }
                }
                else
                {
                    if (fileNames.Contains(path))
                    {
                        res.Add(new FileCatchResultItem()
                        {
                            FilePath = PCPath + path,
                            DataItems = new List<DataResultItem>(),
                        });
                    }
                }
                Environment.CatchFilePaths.Add(Info.Key, res);
            }
        }
    }
}
