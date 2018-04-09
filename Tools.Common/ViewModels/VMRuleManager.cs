using System;
using DevExpress.Mvvm;
using AFAS.Library;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Tools.Common.ViewModel
{
    public class RuleFileNode
    {
        public ForensicRulePackage Package { get; set; }
        public string Desc { get; set; }
        public List<RuleFileNode> Children { get; set; }

        public DelegateCommand CloseTab
        {
            get
            {
                return closeTab ?? (closeTab = new DelegateCommand(ExecuteCloseTab));
            }
        }

        DelegateCommand closeTab;

        public void ExecuteCloseTab()
        {
            VMMain.Instance.VMRuleManager.ExecuteCloseTab(this);
        }
    }

    public class VMRuleManager : ViewModelBase
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

        RuleFileNode selectedItem;
        public RuleFileNode SelectedItem
        {
            get { return selectedItem; }
            set
            {
                if (selectedItem != value)
                {
                    selectedItem = value;
                }
            }
        }

        public DelegateCommand InitRuleLibrary
        {
            get
            {
                return initRuleLibrary ?? (initRuleLibrary = new DelegateCommand(ExecuteInitRuleLibrary));
            }
        }

        DelegateCommand initRuleLibrary;
        RuleManager ruleManager;
        
        public void ExecuteInitRuleLibrary()
        {
            if (ruleManager == null)
            {
                ruleManager = new RuleManager();
            }

            Task.Factory.StartNew(() =>
            {

                ruleManager.LoadRulePackages(true);

                var tList = new List<RuleFileNode>
                {
                    new RuleFileNode()
                    {
                        Desc = "本地规则",
                        Children = new List<RuleFileNode>()
                        {
                            new RuleFileNode()
                            {
                                Desc="Android",
                            },
                            new RuleFileNode()
                            {
                                Desc="IOS",
                            },
                        }
                    }
                };
                tList[0].Children[0].Children = ruleManager.Packages.Where(c => !c.Desc.Contains("IOS")).Select(d => new RuleFileNode()
                {
                    Package = d,
                    Desc = d.Desc,
                }).ToList();
                tList[0].Children[1].Children = ruleManager.Packages.Where(c => c.Desc.Contains("IOS")).Select(d => new RuleFileNode()
                {
                    Package = d,
                    Desc = d.Desc,
                }).ToList();
                DataSource = tList;
            });
        }

        public ObservableCollection<RuleFileNode> CurrentEditPackages { get; set; } = new ObservableCollection<RuleFileNode>();

        public DelegateCommand OpenSelectedFile
        {
            get
            {
                return openSelectedFile ?? (openSelectedFile = new DelegateCommand(ExecuteOpenSelectedFile));
            }
        }

        DelegateCommand openSelectedFile;

        public void ExecuteOpenSelectedFile()
        {
            if (SelectedItem == null || SelectedItem.Package == null) return;

            if(!CurrentEditPackages.Contains(SelectedItem))
            {
                CurrentEditPackages.Add(SelectedItem);
                SelectedTabIndex = CurrentEditPackages.Count - 1;
            }
            else
            {
                SelectedTabIndex=CurrentEditPackages.FindIndex(c=>c==SelectedItem);
            }
        }

        int selectedTabIndex;
        public int SelectedTabIndex
        {
            get { return selectedTabIndex; }
            set
            {
                if (selectedTabIndex != value)
                {
                    selectedTabIndex = value;
                    RaisePropertyChanged("SelectedTabIndex");
                }
            }
        }

        public void ExecuteCloseTab(RuleFileNode node)
        {
            CurrentEditPackages.Remove(node);

        }

        public DelegateCommand SaveCurrentRule
        {
            get
            {
                return saveCurrentRule ?? (saveCurrentRule = new DelegateCommand(ExecuteSaveCurrentRule));
            }
        }

        DelegateCommand saveCurrentRule;

        public void ExecuteSaveCurrentRule()
        {
            if (SelectedTabIndex>= CurrentEditPackages.Count) return;
            RuleManager.SavePackage(CurrentEditPackages[selectedTabIndex].Package);
            VMMain.Instance.State = CurrentEditPackages[selectedTabIndex].Package.Desc + " 规则保存成功";
        }

        public DelegateCommand TestCurrentRule
        {
            get
            {
                return testCurrentRule ?? (testCurrentRule = new DelegateCommand(ExecuteTestCurrentRule));
            }
        }

        DelegateCommand testCurrentRule;

        public void ExecuteTestCurrentRule()
        {
            if (SelectedTabIndex >= CurrentEditPackages.Count) return;
            VMMain.Instance.State = CurrentEditPackages[SelectedTabIndex].Desc+" 测试中...";
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var curPack = CurrentEditPackages[SelectedTabIndex].Package;
                    var t = RuleManager.LoadPackageFromText(curPack.OrgText);
                    t.OrgText = curPack.OrgText;
                    t.PackageFilePath = curPack.PackageFilePath;
                    CurrentEditPackages[SelectedTabIndex].Package = t;
                    VMMain.Instance.State = CurrentEditPackages[SelectedTabIndex].Desc + " 测试成功！";
                }
                catch (Exception e)
                {
                    VMMain.Instance.State = CurrentEditPackages[SelectedTabIndex].Desc + " 测试出现异常";
                    VMMain.Instance.Error = e.Message;
                }

            });
        }

        public DelegateCommand RunCurrentRule
        {
            get
            {
                return runCurrentRule ?? (runCurrentRule = new DelegateCommand(ExecuteRunCurrentRule));
            }
        }

        DelegateCommand runCurrentRule;

        public void ExecuteRunCurrentRule()
        {
            if (SelectedTabIndex >= CurrentEditPackages.Count) return;
             if (VMMain.Instance.VMConfig.WorkPath == null || VMMain.Instance.VMConfig.WorkPath == "")
            {
                VMMain.Instance.State = "请正确设置工作路径";
                return;
            }
            var path = VMMain.Instance.VMConfig.WorkPath;
            var pack = CurrentEditPackages[SelectedTabIndex].Package;
            VMMain.Instance.State = pack.Desc+ " 运行中...";
            Task.Factory.StartNew(() =>
            {
                try
                {
                    if (path.Last() != '\\') path += '\\';
                    VMMain.Instance.VMForensicResult.DataSource= AFASManager.Instance.DoForensicByPackage(pack, path, true);

                    VMMain.Instance.State = pack.Desc+ " 工作完成！";
                }
                catch(Exception e)
                {
                    VMMain.Instance.State = "工作出现异常";
                    VMMain.Instance.Error = e.Message;
                }

            });
        }

        public DelegateCommand LoadRuleFromFile
        {
            get
            {
                return loadRuleFromFile ?? (loadRuleFromFile = new DelegateCommand(ExecuteLoadRuleFromFile));
            }
        }

        DelegateCommand loadRuleFromFile;

        public void ExecuteLoadRuleFromFile()
        {
            var ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.InitialDirectory = VMMain.Instance.VMConfig.RulePath;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = ofd.FileName;
                string name = System.IO.Path.GetFileNameWithoutExtension(path);
                var t = CurrentEditPackages.FirstOrDefault(c => c.Package.PackageFilePath == path);
                if (t == null)
                {
                    var tNode = new RuleFileNode()
                    {
                        Desc= name,
                        Package=new ForensicRulePackage()
                        {
                            Desc= name,
                            PackageFilePath= path,
                            OrgText= System.IO.File.ReadAllText(path, System.Text.Encoding.Default)
                        }
                    };

                    CurrentEditPackages.Add(tNode);
                    SelectedTabIndex = CurrentEditPackages.Count - 1;
                }
                else
                {
                    SelectedTabIndex = CurrentEditPackages.FindIndex(c => c == t);
                }

            }



        }

    }
}