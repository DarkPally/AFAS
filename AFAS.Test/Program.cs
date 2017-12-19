using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Neo.IronLua;

namespace AFAS.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            // Create the Lua script engine
            using (Lua l = new Lua())
            {
                dynamic g = l.CreateEnvironment(); // create a environment
                
                g.dochunk("a = 'Hallo World!';", "test.lua"); // create a variable in lua
                Console.WriteLine(g.a); // access a variable in c#
                g.dochunk("function add(b) return b + 3; end;", "test.lua"); // create a function in lua
                Console.WriteLine("Add(3) = {0}", g.add(3)); // call the function in c#
            }
            */
            PackageTest.Main1(args);
            Console.ReadKey();
        }
    }
}
