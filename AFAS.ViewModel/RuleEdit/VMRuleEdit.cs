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
    public class VMRuleEdit : VMBase
    {
        public DelegateCommand LoadData
        {
            get
            {
                return loadData ?? (loadData = new DelegateCommand(ExecuteLoadData));
            }
        }
        DelegateCommand loadData;

        RuleManager ruleManager;

        ForensicRulePackage selectedPackage;
        public ForensicRulePackage SelectedPackage
        {
            get { return selectedPackage; }
            set
            {
                if (selectedPackage != value)
                {
                    selectedPackage = value;
                    RaisePropertyChanged("SelectedPackage");
                }
            }
        }

        public List<ForensicRulePackage> Packages
        {
            get
            {
                if (ruleManager == null) return null;
                return ruleManager.Packages;
            }
        }

        public void ExecuteLoadData()
        {
            if(ruleManager==null)
            {
                ruleManager = new RuleManager();
                ruleManager.LoadRulePackages(true);
                RaisePropertyChanged("Packages");
                
            }
        }

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
            if(SelectedPackage==null)
            {
                State = "请选择规则";
                return;
            }

            State = "解析中...";
            Task.Factory.StartNew(() =>
            {
                try
                {
                    if (Path.Last() != '\\') Path += '\\';
                    DataSource= AFASManager.Instance.DoForensicByPackage(SelectedPackage,Path, true);

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
                    DataSource = AFASManager.Instance.DoForensicByPackage(SelectedPackage, Path, false);
                    State = "解析完成！";
                }
                catch (Exception e)
                {
                    State = "解析出现异常";
                    Error = e.Message;
                }

            });
        }

        public DelegateCommand TestData
        {
            get
            {
                return testData ?? (testData = new DelegateCommand(ExecuteTestData));
            }
        }

        DelegateCommand testData;

        public void ExecuteTestData()
        {

            State = "工作中...";
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var t = RuleManager.LoadPackageFromText(SelectedPackage.OrgText);
                    t.OrgText = SelectedPackage.OrgText;
                    t.PackageFilePath = SelectedPackage.PackageFilePath;
                    SelectedPackage = t;
                    //RaisePropertyChanged("Packages");
                    State = "测试成功！";
                }
                catch (Exception e)
                {
                    State = "工作出现异常";
                    Error = e.Message;
                }

            });
        }

        public DelegateCommand SaveData
        {
            get
            {
                return saveData ?? (saveData = new DelegateCommand(ExecuteSaveData));
            }
        }

        DelegateCommand saveData;

        public void ExecuteSaveData()
        {

            State = "保存中...";
            Task.Factory.StartNew(() =>
            {
                try
                {
                    RuleManager.SavePackage(SelectedPackage);
                    State = "保存成功！";
                }
                catch (Exception e)
                {
                    State = "保存出现异常";
                    Error = e.Message;
                }

            });
        }
    }
}
