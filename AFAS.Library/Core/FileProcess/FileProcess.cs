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

        List<FileCatchResultItem> filePaths;
        bool copyTargetFiles()
        {
            try
            {
                if (!Environment.CatchFilePaths.ContainsKey(Info.Key)) return false;
                filePaths = Environment.CatchFilePaths[Info.Key];
                for (int i = 0; i < filePaths.Count; ++i)
                {
                    FileInfo file = new FileInfo(filePaths[i].FilePath);
                    if (file.Exists)
                    {
                        filePaths[i].FilePath += ".proc";
                        // true is overwrite
                        file.CopyTo(filePaths[i].FilePath, true);
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
