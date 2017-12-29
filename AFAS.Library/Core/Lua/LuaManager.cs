using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AFAS.Library;
using Neo.IronLua;
using System.Reflection;

namespace AFAS.Library
{
    public class LuaManager
    {
        public Lua Engine { get; set; }

        public LuaManager()
        {
            Init();
        }
        
        public void Init()
        {
            Engine = new Lua();
        }

        public LuaGlobal CreateEnvironment()
        {
            var Enviroment = Engine.CreateEnvironment<LuaGlobal>();
            return Enviroment;
        }

        public void Dispose()
        {
            Engine.Dispose();
        }
    }
}
