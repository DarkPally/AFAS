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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AFAS.WPF.Pages
{
    /// <summary>
    /// PForensic.xaml 的交互逻辑
    /// </summary>
    public partial class PAnalysis : UserControl
    {
        public PAnalysis()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.OpenFileDialog();
            if (tb_fileAddress.Text != "")
            {
                dialog.InitialDirectory = tb_fileAddress.Text;
            }
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                tb_fileAddress.Text = dialog.FileName;
            }
        }
    }
}
