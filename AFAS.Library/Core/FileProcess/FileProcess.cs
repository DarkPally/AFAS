using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AFAS.Library
{
    public class FileProcess
    {
        public PackageForensic Environment { get; set; }
        public FileProcessInfo Info { get; set; }

        ForensicResultItem filePathItem;
        List<string> filePaths;

        bool copyTargetFiles()
        {
            try
            {
                if (!Environment.CatchDataTables.ContainsKey(Info.Key) ||
                    Environment.CatchDataTables[Info.Key].Count==0) return false;
                filePathItem = Environment.CatchDataTables[Info.Key][0];
                
                filePaths = new List<string>();
                for (int i = 0; i < filePathItem.Table.Rows.Count; ++i)
                {
                    var path = Convert.ToString(filePathItem.Table.Rows[i]["PCPath"]);
                    FileInfo file = new FileInfo(path);
                    if (file.Exists)
                    {
                        path += ".proc";
                        filePathItem.Table.Rows[i]["PCPath"] = path ;
                        // true is overwrite
                        file.CopyTo(path, true);
                        filePaths.Add(path);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void DoWork()
        {
            if(copyTargetFiles())
            {
                foreach (var it in filePaths)
                {
                    Environment.LuaEnvironment.DoChunk(Info.Chunk, it);
                }
            }
            
        }
    }
}
