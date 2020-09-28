using CG.Web.MegaApiClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Logica di interazione per DownloadWindow.xaml
    /// </summary>
    public partial class DownloadWindow : Window
    {
        string folders;
        public DownloadWindow(string folders)
        {
            this.folders = folders;
            this.ResizeMode = ResizeMode.CanMinimize;
            InitializeComponent();
            TBR_path.Text = folders;
            Task.Run(() => MegaDownloadAsync());
        }

        public async Task MegaDownloadAsync()
        {
            var client = new MegaApiClient();
            client.LoginAnonymous();
            Uri fileLink = new Uri("https://mega.nz/folder/yoYB0QRB#uy2hyl6Gf8skMUuz_lK3oQ");
            IEnumerable<INode> node = client.GetNodesFromLink(fileLink);
            List<INode> items = node.ToList();
            INode newFile = items[2];
            try
            {                                                                   
                IProgress<double> progressHandler = new Progress<double>(x => updateProgress(x));
                await client.DownloadFileAsync(newFile, folders + "\\" + newFile.Name, progressHandler);
                await BT_done.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        BT_done.IsEnabled = true;
                    }));
                
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("The file is already downloaded");
            }
            client.Logout();
        }

        public void updateProgress(double progress)
        {
            ProgressBar.Dispatcher.BeginInvoke((Action)(() =>
            {
                ProgressBar.Value = progress;
            }));
            TBR_percentage.Dispatcher.BeginInvoke((Action)(() =>
            {
                TBR_percentage.Text = ((int)progress).ToString();
            })); 
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TBR_path_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Process.Start(folders);
            }
            catch (Exception win32Exception)
            {
                //The system cannot find the file specified...
                Console.WriteLine(win32Exception.Message);
            }
        }
    }
}
