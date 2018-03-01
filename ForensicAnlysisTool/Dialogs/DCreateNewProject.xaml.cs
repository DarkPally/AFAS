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
using System.Windows.Shapes;
using DevExpress.Xpf.Core;


namespace ForensicAnlysisTool.Dialogs
{
    /// <summary>
    /// Interaction logic for DCreateNewProject.xaml
    /// </summary>
    public partial class DCreateNewProject : ThemedWindow
    {
        public DCreateNewProject()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog()
            {
                Description = "请选择工作目录",
            };
            if (tb_fileAddress.Text != "")
            {
                dialog.SelectedPath = tb_fileAddress.Text;
            }
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                tb_fileAddress.Text = dialog.SelectedPath;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            /*
            var btn = (sender as Button);
            if(btn.Command!=null)
            btn.Command.Execute(btn.CommandParameter);
            */
            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
