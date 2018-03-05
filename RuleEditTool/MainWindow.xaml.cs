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

        public class TT
        {
            public string Name { get; set; }
        }
        private void BarButtonItem_ItemClick_CreateNewRule(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {

            DocumentGroup_main.ItemsSource = new List<TT>()
            {
                new TT(){ Name="aaaa"},
                new TT(){ Name="bbbb"},
            };
        }
    }
}
