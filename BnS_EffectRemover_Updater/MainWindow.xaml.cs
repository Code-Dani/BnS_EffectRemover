using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Packaging;
using System.Linq;
using System.Net;
using System.Security.Policy;
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
using Octokit;
using Octokit.Internal;
using System.IO.Compression;
using System.Reflection;
using System.IO;
using Ionic.Zip;

namespace BnS_EffectRemover
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.ResizeMode = ResizeMode.CanMinimize;
            InitializeComponent();
            Process[] ps = Process.GetProcessesByName("BnS_EffectRemover");
            foreach (Process p in ps)
            {
                p.Kill();
            }
            Task.Run(async()=> { 
                await retrieveLatestRelease(); 
            });
        }

        public async Task retrieveLatestRelease()
        {
            try
            {
                var client = new GitHubClient(new ProductHeaderValue("Code-Dani"));
                var releases = await client.Repository.Release.GetLatest("Code-Dani", "BnS_EffectRemover");
                var AllLatestAssets = await client.Repository.Release.GetAllAssets("Code-Dani", "BnS_EffectRemover", releases.Id);
                ReleaseAsset LatestBinary = null;
                foreach (var item in AllLatestAssets)
                {
                    if (item.State.ToLower() == "uploaded")
                    {
                        LatestBinary = item;
                    }
                }
                if (LatestBinary != null)
                {
                    WebClient wb = new WebClient();
                    wb.DownloadProgressChanged += (o, e) =>
                    {
                        UpdateBar(e.ProgressPercentage);
                        if (e.ProgressPercentage == 100)
                        {
                            TBR_download.Dispatcher.BeginInvoke((Action)(() =>
                            {
                                TBR_download.Foreground = new SolidColorBrush(Colors.Green);
                                TBR_download.Text = "downloaded";
                            }));
                        }
                    };
                    wb.DownloadFileAsync(new Uri(LatestBinary.BrowserDownloadUrl), LatestBinary.Name);
                    wb.DownloadFileCompleted += Wb_DownloadFileCompleted;
                }
            }catch(Exception er)
            {
                Console.WriteLine("\n\n\n" + er.Message);
            }
        }

        private void Wb_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            installLatestUpdate();
        }

        public void UpdateBar(int value)
        {
            ProgressBar.Dispatcher.BeginInvoke((Action)(() =>
            {
                ProgressBar.Value = value;
            }));
            TBR_percentage.Dispatcher.BeginInvoke((Action)(() =>
            {
                TBR_percentage.Text = ((int)value).ToString();
            }));
        }

        public void installLatestUpdate()
        {
            string startANDdestinationPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            try
            {
                string[] files = System.IO.Directory.GetFiles(startANDdestinationPath, "*.zip");
                if (files.Count() > 0)
                {
                    TB_console.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        TB_console.Inlines.Add("\n>> Extracting files in the zip folder\n");
                    }));
                    using (Ionic.Zip.ZipFile zip = Ionic.Zip.ZipFile.Read(files[0]))
                    {
                        zip.ExtractAll(startANDdestinationPath, ExtractExistingFileAction.OverwriteSilently);
                    }
                    var Ffiles = Directory.GetFiles(startANDdestinationPath + "/" + "BnS_EffectRemover (By Daniel)");
                    foreach(var f in Ffiles)
                    {
                        if (System.IO.Path.GetFileName(f) != "BnS_EffectRemover_Updater.exe")
                        {
                            if (File.Exists(startANDdestinationPath + "/" + System.IO.Path.GetFileName(f)))
                            {
                                File.Delete(startANDdestinationPath + "/" + System.IO.Path.GetFileName(f));
                            }
                            File.Move(f, startANDdestinationPath + "/" + System.IO.Path.GetFileName(f));
                        }
                    }
                    TB_console.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        TB_console.Inlines.Add(">> Deleting zip file\n");
                    }));
                    File.Delete(files[0]);
                    Directory.Delete(startANDdestinationPath + "/" + "BnS_EffectRemover (By Daniel)", true);
                }
            }catch(Exception er)
            {
                Console.WriteLine(er.Message);
            }

            TBR_installation.Dispatcher.BeginInvoke((Action)(() =>
            {
                TBR_installation.Foreground = new SolidColorBrush(Colors.Green);
                TBR_installation.Text = "installation completed";
            }));

        }
    }
}
