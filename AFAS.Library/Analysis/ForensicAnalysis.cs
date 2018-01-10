using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Neo.IronLua;
using JiebaNet.Segmenter;
using JiebaNet.Analyser;

namespace AFAS.Library
{
    public class WordWeight
    {
        public string Word { get; set; }
        public double Weight { get; set; }
    }
    public class ForensicAnalysis
    {
        List<string> getText(ForensicResultItem item)
        {
            var res = new List<string>();
            if (item.MarkInfo != null
                && item.MarkInfo.ColumnDescs != null)
            {
                foreach(var it in item.MarkInfo.ColumnDescs)
                {
                    if(it.Mark=="UserText")
                    {
                        foreach (DataRow dr in item.Table.Rows)
                        {
                            res.Add(Convert.ToString(dr[it.Desc]));
                        }
                    }
                }
            }
            if(item.Children!=null)
            {
                foreach (var it in item.Children)
                {
                    res.AddRange(getText(it));
                }
            }
            return res;
        }

        public IEnumerable<WordWeight> ExtractTagsWithWeight(ForensicResult resource)
        {
            var source = new List<string>();
            foreach (var it in resource.Items)
            {
                source.AddRange(getText(it));
            }
            var t=string.Join(" ", source);
            var ex = new TextRankExtractor();
            return ex.ExtractTagsWithWeight(t,200).Select(c=>new WordWeight()
            {
                Weight=c.Weight,
                Word=c.Word,
            });
        }

        public IEnumerable<WordWeight> ExtractTagsWithWeightFromTXTFile(string path)
        {
           
            var ex = new TextRankExtractor();
            var str = System.IO.File.ReadAllText(path);
            return ex.ExtractTagsWithWeight(str, 200).Select(c => new WordWeight()
            {
                Weight = c.Weight,
                Word = c.Word,
            });
        }
    }
}
