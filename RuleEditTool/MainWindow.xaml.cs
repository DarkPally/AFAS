using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.Xpf.Core;
using Tools.Common.Dialogs;
using Tools.Common.ViewModel;
using Tools.Common.Windows;

namespace RuleEditTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ThemedWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = VMMain.Instance;
        }
        private void BarButtonItem_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            var d = new DCreateNewProject();
            d.Owner = this;
            d.DataContext = VMMain.Instance.VMForensic;
            d.Show();
        }

        private void BarButtonItem_ItemClick_timeAnalysis(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            p_timeAnalysis.AutoHideExpandState = DevExpress.Xpf.Docking.Base.AutoHideExpandState.Visible;
        }

        private void BarButtonItem_ItemClick_CreateNewRule(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            var d = new DCreateNewRule();
            d.Owner = this;
            d.DataContext = VMMain.Instance.VMRuleManager;
            d.Show();
        }

        private void BarButtonItem_ItemClick_AndroidFileObserver(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            var d = new WAndroidFileObserver();
            d.Owner = this;
            d.DataContext = VMMain.Instance.VMAndroidFileObserver;
            d.Show();
        }

        private void BarButtonItem_ItemClick_AndroidFileSearcher(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            var d = new WAndroidFileSearcher();
            d.Owner = this;
            d.DataContext = VMMain.Instance.VMAndroidFileSearcher;
            d.Show();
        }
    }
}
