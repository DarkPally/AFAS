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
    public class LuaTestScript
    {
        public static int func(int c)
        {
            return c + 1;
        }
    }

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
            //ScriptLib.Add("func",new Func<int,int>(c=>{ return c+1; }));
            //ScriptLib.Add("func",);
            //LoadScriptLibFromClass(typeof(LuaTestScript));
        }

        /*
        public void LoadScriptLibFromClass(Type classType)
        {
            MethodInfo[] mif = classType.GetMethods();
            foreach (MethodInfo mf in mif)
            {
                if (mf.IsStatic && mf.IsPublic)
                {
                    ScriptLib.Add(mf.Name, mf.CreateDelegate(typeof(Delegate)));
                }
            }
        }


        Dictionary<string, Delegate> ScriptLib { get; set; }=new Dictionary<string, Delegate>();
        */
        public LuaGlobal CreateEnvironment()
        {
            var Enviroment = Engine.CreateEnvironment<LuaGlobal>();

            /*
            foreach(var it in ScriptLib)
            {
                Enviroment.DefineFunction(it.Key, it.Value);
            }
            */

            return Enviroment;
        }

        public void Dispose()
        {
            Engine.Dispose();
        }
    }
}
