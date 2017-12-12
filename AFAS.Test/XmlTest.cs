using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using System.Data;
using System.IO;

namespace AFAS.Test
{
    class XmlTest
    {
        public static void Main1(string[] args)
        {
            var t = GetDataSetByXmlpath(@"C:\Users\zjf\Desktop\ww\www.txt");
        }

        public static DataSet GetDataSetByXmlpath(string strXmlPath)
        {
            try
            {
                DataSet ds = new DataSet();
                //读取XML到DataSet 

                StreamReader sr = new StreamReader(strXmlPath, Encoding.Default);

                ds.ReadXml(sr);

                sr.Close();

                if (ds.Tables.Count > 0)
                    return ds;
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
