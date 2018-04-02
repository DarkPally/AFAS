﻿using System;
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

    }
}