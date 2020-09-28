using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace BnS_EffectRemover
{
    /// <summary>
    /// Logica di interazione per UpdateChecker.xaml
    /// </summary>
    public partial class UpdateChecker : Window
    {
        RepositoryInfo folders;
        public UpdateChecker(RepositoryInfo folders)
        {
            this.folders = folders;
            this.ResizeMode = ResizeMode.CanMinimize;
            InitializeComponent();
            
            if (string.IsNullOrEmpty(folders.update_Download_Path))
            {
                TBR_path.Text = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                TBR_path.ToolTip = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }
            else
            {
                TBR_path.Text = folders.update_Download_Path;
                TBR_path.ToolTip = folders.update_Download_Path;
            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button bt = (Button)sender;
            switch (bt.Name.ToString())
            {
                case "BT_yes":
                    DownloadWindow dw = new DownloadWindow(TBR_path.Text);
                    this.Hide();
                    dw.ShowDialog();
                    this.Close();
                    break;
                case "BT_no":
                    this.Close();
                    break;
            }
        }

        private void Run_MouseUp(object sender, MouseButtonEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = @"C:\";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                TBR_path.Text = dialog.FileName;
                folders.update_Download_Path = dialog.FileName;
                GestioneFileXML.ScriviXml(folders);
            }
        }

    }
}
