using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;
using System.IO;
namespace AFAS.Library.AutoRule
{
    public class FileRuleSearchHelper
    {
        static public bool CheckIsSqlite3(string FilePath)
        {
            try
            {
                using (var fs = File.Open(FilePath, FileMode.Open))
                {
                    BinaryReader br = new BinaryReader(fs);
                    var chars = br.ReadChars(15);
                    var s = new string(chars);
                    if (s == "SQLite format 3")
                    {
                        return true;
                    }

                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return false;

        }

        static public List<string> GetSqlite3Paths(string DictionaryPath,bool searchChildDir=false )
        {
            DirectoryInfo theFolder = new DirectoryInfo(DictionaryPath);
            FileInfo[] thefileInfo = theFolder.GetFiles("*.*", 
                searchChildDir?SearchOption.AllDirectories:SearchOption.TopDirectoryOnly);
            return thefileInfo.AsParallel().Where(f => CheckIsSqlite3(f.FullName)).Select(f=>f.FullName).ToList();
        }

        static public List<string> GetSqlite3Paths(List<string> FilePaths)
        {
            return FilePaths.AsParallel().Where(f => CheckIsSqlite3(f)).Select(f => f).ToList();
        }

        static public bool CheckIsStandardXml(string FilePath)
        {
            try
            {
                using (var fs = File.Open(FilePath, FileMode.Open))
                {
                    BinaryReader br = new BinaryReader(fs);
                    var chars = br.ReadChars(6);
                    var s = new string(chars);
                    if (s == "<?xml ")
                    {
                        return true;
                    }
                }
            }
            catch
            {
            }
            return false;

        }
        static public List<string> GetStandardXmlPaths(string DictionaryPath, bool searchChildDir = false)
        {
            DirectoryInfo theFolder = new DirectoryInfo(DictionaryPath);
            FileInfo[] thefileInfo = theFolder.GetFiles("*.*",
                searchChildDir ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            return thefileInfo.AsParallel().Where(f => CheckIsStandardXml(f.FullName)).Select(f => f.FullName).ToList();
        }
        static public List<string> GetStandardXmlPaths(List<string> FilePaths)
        {
            return FilePaths.AsParallel().Where(f => CheckIsStandardXml(f)).Select(f => f).ToList();
        }
    }
}
