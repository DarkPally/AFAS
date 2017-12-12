using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AFAS.Library.Core
{
    public class FileProcesser
    {
        public PackageForensic Environment { get; set; }
        public FileProcesserInfo Info { get; set; }

        List<string> filePaths;
        void copyTargetFiles()
        {
            filePaths = Environment.CatchFilePaths[Info.Key];
            for (int i=0; i< filePaths.Count; ++i)
            {
                FileInfo file = new FileInfo(filePaths[i]);
                if (file.Exists)
                {
                    filePaths[i] += ".proc";
                    // true is overwrite
                    file.CopyTo(filePaths[i], true);
                }
            }
        }

        public void DoWork()
        {
            copyTargetFiles();
            foreach(var it in filePaths)
            {
                AFASManager.Lua.Environment.DoChunk(Info.Chunk,it);
            }
        }
    }
}
