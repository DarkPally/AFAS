using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DevExpress.Xpf.Core;

using Tools.Common.ViewModel;
using Tools.Common.Dialogs;
namespace Tools.Common.Windows
{
    /// <summary>
    /// WAndroidFileObserve.xaml 的交互逻辑
    /// </summary>
    public partial class WAndroidFileSearcher : ThemedWindow
    {
        public WAndroidFileSearcher()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var d = new DGenerateAutoRule();
            d.Owner = this;
            d.DataContext = VMMain.Instance.VMAutoRuleGenerator;
            d.Show();
        }
    }
}
