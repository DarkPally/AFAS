using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Neo.IronLua;
using JiebaNet.Segmenter;
using JiebaNet.Analyser;
using KAverage;

namespace AFAS.Library
{
    public class WordWeight
    {
        public string Word { get; set; }
        public double Weight { get; set; }
    }

    public class ForensicAnalysis
    {
        KeywordExtractor Extractor { get; set; }
        TfidfExtractor TfidfExtractor { get; set; }
        TextRankExtractor TextRankExtractor { get; set; }
        public enum ExtractAlgorithm
        {
            TextRank,
            TF_IDF,
        };
        public ExtractAlgorithm Algorithm { get; set; } = ExtractAlgorithm.TextRank;
        public int KeywordCount { get; set; } = 20;
        public string UserTextMark { get; set; } = "UserText";

        void init()
        {
            if(Extractor==null)
            {
                TextRankExtractor = new TextRankExtractor();
                TfidfExtractor = new TfidfExtractor();
            }
            if (Algorithm == ExtractAlgorithm.TextRank) Extractor = TextRankExtractor;
            if (Algorithm == ExtractAlgorithm.TF_IDF) Extractor = TfidfExtractor;
        }

        public IEnumerable<WordWeight> ExtractTagsWithWeight(ForensicResultItem item)
        {
            init();
            string res = string.Join(" ", item.GetColumnDataByMark(UserTextMark));
            return Extractor.ExtractTagsWithWeight(res, KeywordCount).Select(c => new WordWeight()
            {
                Weight = c.Weight,
                Word = c.Word,
            });
        }

        public IEnumerable<WordWeight> ExtractTagsWithWeight(ForensicResult resource)
        {
            init();
            var source = new List<object>();
            foreach (var it in resource.GetItemWithMark(UserTextMark))
            {
                source.AddRange(it.GetColumnDataByMark(UserTextMark));
            }
            var t=string.Join(" ", source);
            return Extractor.ExtractTagsWithWeight(t, KeywordCount).Select(c=>new WordWeight()
            {
                Weight=c.Weight,
                Word=c.Word,
            });
        }

        public IEnumerable<WordWeight> ExtractTagsWithWeightFromTXTFile(string path)
        {
            init();
            var str = System.IO.File.ReadAllText(path);
            return Extractor.ExtractTagsWithWeight(str, KeywordCount).Select(c => new WordWeight()
            {
                Weight = c.Weight,
                Word = c.Word,
            });
        }

        public void Classify(ForensicResult resource,int Kcount=3)
        {
            init();
            var source = new List<object>();
            Dictionary<string, int> WordIndexMap = new Dictionary<string, int>();
            List<string> WordList=new List<string>();
            var WordWeight = new Dictionary<ForensicResultItem, IEnumerable<WordWeight>>();

            foreach (var it in resource.GetItemWithMark(UserTextMark))
            {
                var tTags = ExtractTagsWithWeight(it);
                if(tTags.Count()>0)
                {
                    WordWeight.Add(it, tTags);
                    WordList.AddRange(tTags.Select(c => c.Word));
                }
            }
            WordList=WordList.Distinct().ToList();
            for (int i= 0;i < WordList.Count;++i)
            {
                WordIndexMap.Add(WordList[i], i);
            }
            ;

            var tEngine = new EngineModel()
            {
                ClassCount = Kcount,
                Samples= WordWeight.Values.Select(c =>
                {
                    var tX = new double[WordList.Count];
                    foreach (var it in c)
                    {
                        tX[WordIndexMap[it.Word]] = it.Weight;
                    }
                    var t = new PointModel()
                    {
                        X = tX,
                    };
                    return t;
                }).ToList()
            };

            tEngine.Run();
            var res = tEngine.Classes;

        }



    }
}
