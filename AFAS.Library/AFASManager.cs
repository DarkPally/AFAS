using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AFAS.Library.Core;

namespace AFAS.Library
{
    public class AFASManager
    {
        private static readonly AFASManager instance = new AFASManager();
        public static AFASManager Instance { get { return instance; } }

        private AFASManager()
        {
            Init();
        }

        
        public void Init()
        {

        }
    }
}
