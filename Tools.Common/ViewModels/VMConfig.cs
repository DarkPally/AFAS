using System;
using DevExpress.Mvvm;
using AFAS.Library;

namespace Tools.Common.ViewModel
{
    public class VMConfig : ViewModelBase
    {
        string workPath= System.Environment.CurrentDirectory + "\\temp\\";
        public string WorkPath
        {
            get { return workPath; }
            set
            {
                if (workPath != value)
                {
                    workPath = value;
                    RaisePropertyChanged("WorkPath");
                }
            }
        }

        string rulePath = System.Environment.CurrentDirectory + "\\RuleLibrary\\";
        public string RulePath
        {
            get { return rulePath; }
            set
            {
                if (rulePath != value)
                {
                    rulePath = value;
                    RaisePropertyChanged("RulePath");
                }
            }
        }
    }
}