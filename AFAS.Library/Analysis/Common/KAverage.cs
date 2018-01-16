using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAverage
{
    /*
    public class PointModelCompareByX : IEqualityComparer<PointModel>
    {
        public bool Equals(PointModel x, PointModel y)
        {
            if (x == null || y == null)
                return false;
            if (x.IsEqual(y))
                return true;
            else
                return false;
        }

        public int GetHashCode(PointModel obj)
        {
            return obj.GetHashCode();
        }
    }
    */
    /*样本点的模型*/
    public class PointModel
    {
        public double[] X { get; set; }
        public ClassModel Class { get; set; }
        /*获取两点间的欧式距离*/
        public double GetDistance(PointModel y)
        {
            return Math.Sqrt(GetDistanceSquare(y));
        }

        /*获取两点间的欧式距离的平方*/
        public double GetDistanceSquare(PointModel y)
        {
            double sum=0;
            for(int i=0;i< X.Count(); ++i)
            {
                sum+=(X[i] - y.X[i]) * (X[i] - y.X[i]);
            }
            return sum;
        }

        public PointModel AddPoint(PointModel y)
        {
            for (int i = 0; i < X.Count(); ++i)
            {
               X[i]+= y.X[i];
            }
            return this;
        }
        public PointModel Multiply(double m)
        {
            for (int i = 0; i < X.Count(); ++i)
            {
                X[i] *= m;
            }
            return this;
        }

        public bool IsEqual(PointModel y)
        {
            for (int i = 0; i < X.Count(); ++i)
            {
                if (X[i] != y.X[i]) return false;
            }
            return true;
        }

        public PointModel()
        {
            Class = null;
        }
    }
    /*聚类的模型*/
    public class ClassModel
    {
        /*该聚类内的样本*/
        public List<PointModel> Samples { get; set; }

        /*该聚类当前的中心点*/
        public PointModel CenterPoint { get; set; }

        /*该聚类当前是否已收敛*/
        public bool EndFlag { get; set; }
        public ClassModel(PointModel initialCenter)
        {
            CenterPoint = initialCenter;

            //保证样本数量不可能为0
            Samples = new List<PointModel>() { initialCenter };

            EndFlag = false;
        }
        public double GetDistanceSquareToCenter(PointModel p)
        {
            return CenterPoint.GetDistanceSquare(p);
        }
        public void CaculateNextCenter()
        {
            var tempPoint = new PointModel()
            {
                X = new double[Samples[0].X.Count()],
            };
            foreach(var i in Samples)
            {
                tempPoint.AddPoint(i);
            }
            double x = 1 / (double)Samples.Count;

            tempPoint.Multiply(x);

            EndFlag = tempPoint.IsEqual(CenterPoint);

            CenterPoint = tempPoint;

        }
    }
    /*引擎模型，即全部的运算环境*/
    public class EngineModel
    {
        /*全部样本*/
        public List<PointModel> Samples { get; set; }
        /*期望的聚类数量*/
        public int ClassCount { get; set; }
        /*聚类的集合*/
        public List<ClassModel> Classes { get; set; }

        void singleProcess()
        {
            Parallel.ForEach(Samples, point =>
            {
                var temp = new Dictionary<ClassModel, double>();
                
                Parallel.ForEach(Classes, (item) =>
                {
                    lock(temp)
                    {
                        temp.Add(item, item.GetDistanceSquareToCenter(point));
                    }
                });
                var minClass = temp.OrderBy(x => x.Value).First().Key;

                if (minClass != point.Class)
                {
                    if (point.Class != null)
                    {
                        point.Class.Samples.Remove(point);
                    }
                    lock(minClass.Samples)
                    {
                        minClass.Samples.Add(point);
                    }
                    point.Class = minClass;
                }
            });
        }
        public void Run()
        {
            try 
            {
                initialize();

                bool flag=true;
                while(flag)
                {
                    singleProcess();
                    Parallel.ForEach(Classes, c =>
                    {
                        c.CaculateNextCenter();
                    });
                    flag =(Classes.Count(x => x.EndFlag == false))>0;
                }                
                
            }
            catch(Exception e)
            {
               throw new Exception("运算异常："+ e.Message);
            }
        }

        void initialize()
        {
            if (ClassCount==0)
            {
                throw new Exception("K值为0，无法计算");
            }
            //var tS = Samples.Distinct(new PointModelCompareByX()).Take(ClassCount).ToList() ;
            if (ClassCount> Samples.Count)
            {
                throw new Exception("K值大于样本数量，无法计算");
            }
            Classes = new List<ClassModel>();

            for(int i=0;i<ClassCount;++i)
            {
                var t = new ClassModel(Samples[i]);
                Samples[i].Class = t;
                Classes.Add(t);
            }           
        }
    }
}
