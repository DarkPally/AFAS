using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AFAS.Library.Android
{
    public enum FileOBState
    {
        None,
        UnChanged,
        Create,
        Changed,
        Deleted,
        UnInterested
    }
    public class FilePropertyOB:FileProperty
    {
        public FileOBState OBState { get; set; }

        public bool IsPropertyEqual(FilePropertyOB other)
        {
            return size == other.size && modifyTime == other.modifyTime;
        }
    }

    public class AndroidFileObserver
    {
        public List<FilePropertyOB> OldFileProperties { get; set; }
        public List<FilePropertyOB> NewFileProperties { get; set; }

        public string ObservePath { get; set; }
        public AndroidFileExtracter androidFileExtracter { get; set; }
        public string androidDevice { get; set; }

        public void Init()
        {
            var res=androidFileExtracter.ListDirecotryVerbose(androidDevice, ObservePath);
            var t = res.filesProperty.Select(c => new FilePropertyOB()
            {
                accessTime=c.accessTime,
                modifyTime=c.modifyTime,
                path=c.path,
                size=c.size,
                type=c.type,
                OBState=FileOBState.UnChanged,
            }).ToList();

            OldFileProperties = t;
            NewFileProperties = t;
        }

        public void Update()
        {
            var res = androidFileExtracter.ListDirecotryVerbose(androidDevice, ObservePath);
            var t = res.filesProperty.Select(c => new FilePropertyOB()
            {
                accessTime = c.accessTime,
                modifyTime = c.modifyTime,
                path = c.path,
                size = c.size,
                type = c.type,
                OBState = FileOBState.UnChanged,
            }).ToList();
            foreach(var it in t)
            {
                var temp = OldFileProperties.Find(c => c.path == it.path);
                if (temp==null)
                {
                    it.OBState = FileOBState.Create;
                }
                else if(temp.OBState==FileOBState.UnInterested)
                {
                    it.OBState = FileOBState.UnInterested;
                }
                else if(!it.IsPropertyEqual(temp))
                {
                    it.OBState = FileOBState.Changed;
                }
            }
            NewFileProperties = t;
        }

        public void ReplaceOldOB()
        {
            OldFileProperties = NewFileProperties;
        }
       
    };
}
