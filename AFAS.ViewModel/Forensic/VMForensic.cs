﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Prism.Mvvm;
using Prism.Commands;

using AFAS.Library;

namespace AFAS.ViewModel
{
    public class VMForensic : VMBase
    {

        public DelegateCommand DoWork
        {
            get
            {
                return doWork ?? (doWork = new DelegateCommand(ExecuteWork));
            }
        }

        DelegateCommand doWork;

        public void ExecuteWork()
        {

            if (Path == null || Path == "")
            {
                State = "请输入根目录";
                return;
            }

            State = "解析中...";
            Task.Factory.StartNew(() =>
            {
                try
                {
                    if (Path.Last() != '\\') Path += '\\';
                    DataSource= AFASManager.Instance.DoForensic(Path, true);

                    State = "解析完成！";
                }
                catch(Exception e)
                {
                    State = "解析出现异常";
                    Error = e.Message;
                }

            });
        }

        public DelegateCommand DoWorkFromADB
        {
            get
            {
                return doWorkFromAdb ?? (doWorkFromAdb = new DelegateCommand(ExecuteWorkFromADB));
            }
        }

        DelegateCommand doWorkFromAdb;

        public void ExecuteWorkFromADB()
        {
            
            State = "解析中...";
            Task.Factory.StartNew(() =>
            {
                try
                {
                    Path = System.Environment.CurrentDirectory + "\\temp\\";
                    DataSource = AFASManager.Instance.DoForensic(Path, false);
                    State = "解析完成！";
                }
                catch (Exception e)
                {
                    State = "解析出现异常";
                    Error = e.Message;
                }

            });
        }
    }
}