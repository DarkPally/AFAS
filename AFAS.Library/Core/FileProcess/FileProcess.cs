using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AFAS.Library.Core
{
    public class FileProcess
    {
        public PackageForensic Environment { get; set; }
        public FileProcessInfo Info { get; set; }

        List<string> filePaths;
        bool copyTargetFiles()
        {
            try
            {
                if (!Environment.CatchFilePaths.ContainsKey(Info.Key)) return false;
                filePaths = Environment.CatchFilePaths[Info.Key];
                for (int i = 0; i < filePaths.Count; ++i)
                {
                    FileInfo file = new FileInfo(filePaths[i]);
                    if (file.Exists)
                    {
                        filePaths[i] += ".proc";
                        // true is overwrite
                        file.CopyTo(filePaths[i], true);
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
                    AFASManager.Lua.Environment.DoChunk(Info.Chunk, it);
                }
            }
            
        }
    }
}
