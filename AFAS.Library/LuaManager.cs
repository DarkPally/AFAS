using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AFAS.Library;
using Neo.IronLua;

namespace AFAS.Library
{
    public class LuaManager
    {
        public Lua Engine { get; set; }
        public dynamic Environment { get; set; }

        public LuaManager()
        {
            Init();
        }
        
        public void Init()
        {
            Engine = new Lua();
            Environment= Engine.CreateEnvironment<LuaGlobal>();
        }

        public void Dispose()
        {
            Engine.Dispose();
        }
    }
}
