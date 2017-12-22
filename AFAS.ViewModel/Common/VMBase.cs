using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Prism.Mvvm;
using Prism.Commands;

using AFAS.Library;

namespace AFAS.ViewModel
{
    public class VMBase : BindableBase
    {
        object dataSource;
        public object DataSource
        {
            get { return dataSource; }
            set
            {
                if (dataSource != value)
                {
                    dataSource = value;
                    RaisePropertyChanged("DataSource");
                }
            }
        }

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

        string path = System.Environment.CurrentDirectory + "\\temp\\";
        public string Path
        {
            get { return path; }
            set
            {
                if (path != value)
                {
                    path = value;
                    RaisePropertyChanged("Path");
                }
            }
        }


        
    }
}
