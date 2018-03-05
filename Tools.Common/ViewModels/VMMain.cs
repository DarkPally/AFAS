using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.Mvvm;

namespace Tools.Common.ViewModel
{
    public class VMMain:ViewModelBase
    {
        private static readonly VMMain instance = new VMMain();
        public static VMMain Instance { get { return instance; } }

        private VMMain()
        {
        }
        public VMForensicResult VMForensicResult { get; } = new VMForensicResult();
        public VMForensic VMForensic { get; } = new VMForensic();
        public VMTagAnalysis VMTagAnalysis { get; } = new VMTagAnalysis();
        public VMTimeAnalysis VMTimeAnalysis { get; } = new VMTimeAnalysis();

        string state = "准备就绪";
        public string State
        {
            get { return state; }
            set
            {
                if (state != value)
                {
                    state = value;
                    RaisePropertyChanged("State");
                }
            }
        }

        string error;
        public string Error
        {
            get { return error; }
            set
            {
                if (error != value)
                {
                    error = value;
                    RaisePropertyChanged("Error");
                }
            }
        }
    }
}
