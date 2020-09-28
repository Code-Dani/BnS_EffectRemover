using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        Window mainwindow;
        public UpdateChecker(Window mainwindow)
        {
            this.mainwindow = mainwindow;
            this.ResizeMode = ResizeMode.CanMinimize;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button bt = (Button)sender;
            switch (bt.Name.ToString())
            {
                case "BT_yes":
                    Process.Start("BnS_EffectRemover_Updater.exe");
                    break;
                case "BT_no":
                    mainwindow.Show();
                    this.Close();
                    break;
            }
        }
    }
}
