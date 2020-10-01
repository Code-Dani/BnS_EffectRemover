using Microsoft.Win32;
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
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Globalization;
using System.IO;
using System.Diagnostics;
using Microsoft.WindowsAPICodePack.Shell.Interop;
using MaterialDesignThemes.Wpf;
using System.Threading;
using System.Windows.Forms;
using System.Reflection;
using System.Security.Policy;
using System.Windows.Threading;
using System.Web.Caching;
using System.Windows.Controls.Primitives;
using Octokit;

namespace BnS_EffectRemover
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string buttonName;
        RepositoryInfo folders;
        Dictionary<string,bool> checkboxStateDict = new Dictionary<string, bool>();
        int CleanCount = 0;
        double TimerMinutes = 0;
        DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Background);

        public MainWindow()
        {
            timer.Tick += Timer_Tick;
            folders = GestioneFileXML.LeggiLista();
            InitializeComponent();
            this.ResizeMode = ResizeMode.CanMinimize;
            if (!string.IsNullOrEmpty(folders.backup_folder) && !string.IsNullOrEmpty(folders.coockedPC_folder))
            {
                Backup_folder.Text = folders.backup_folder;
                CoockedPC_folder.Text = folders.coockedPC_folder;
                if (folders.coockedPC_folder.Contains(@"contents\bns\CookedPC") && string.IsNullOrEmpty(folders.coockedPC_eng_folder))
                {
                    folders.coockedPC_eng_folder = folders.coockedPC_folder + @"\..\..\Local\NCWEST\ENGLISH\CookedPC";
                    GestioneFileXML.ScriviXml(folders);
                }
                
            }
            
        }

        //manage the path textblock
        private void _Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button tb1 = (System.Windows.Controls.Button)sender;
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = @"C:\";
            dialog.IsFolderPicker = true;
            switch (tb1.Name)
            {
                case "Backup_button":
                    if(dialog.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        Backup_folder.Text = dialog.FileName;
                        folders.backup_folder = dialog.FileName;
                        GestioneFileXML.ScriviXml(folders);
                        TB_console.Inlines.Add("\n-----------------\n!! Backup folder path saved\n");
                    }
                    break;
                case "CoockedPC_button":
                    if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        CoockedPC_folder.Text = dialog.FileName;
                        folders.coockedPC_folder = dialog.FileName;
                        GestioneFileXML.ScriviXml(folders);
                        TB_console.Inlines.Add("\n-----------------\n!! CookedPC folder path saved\n");
                        if (Directory.Exists(folders.coockedPC_folder))
                        {
                            Console.WriteLine(System.IO.Path.GetDirectoryName(folders.coockedPC_folder));
                            if (dialog.FileName.Contains(@"contents\bns\CookedPC"))
                            {
                                folders.coockedPC_eng_folder = folders.coockedPC_folder + @"\..\..\Local\NCWEST\ENGLISH\CookedPC";
                                GestioneFileXML.ScriviXml(folders);
                            }
                        }
                    }
                    break;
            }
            
        }
        private void _ClickFunctions(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button obj = (System.Windows.Controls.Button)sender;
            buttonName = obj.Name;
            checkboxStateDict.Clear();
            checkboxStateDict.Add("otherSFX", (bool)CB_otherSFX.IsChecked);
            checkboxStateDict.Add("Sounds", (bool)CB_Sounds.IsChecked);
            checkboxStateDict.Add("DMG", (bool)CB_DMG.IsChecked);
            checkboxStateDict.Add("Archer", (bool)CB_Archer.IsChecked);
            checkboxStateDict.Add("Assassin", (bool)CB_Assassin.IsChecked);
            checkboxStateDict.Add("BD", (bool)CB_BD.IsChecked);
            checkboxStateDict.Add("BM", (bool)CB_BM.IsChecked);
            checkboxStateDict.Add("Destro", (bool)CB_Destro.IsChecked);
            checkboxStateDict.Add("FM", (bool)CB_FM.IsChecked);
            checkboxStateDict.Add("Gunner", (bool)CB_Gunner.IsChecked);
            checkboxStateDict.Add("KFM", (bool)CB_KFM.IsChecked);
            checkboxStateDict.Add("SF", (bool)CB_SF.IsChecked);
            checkboxStateDict.Add("Sum", (bool)CB_Summ.IsChecked);
            checkboxStateDict.Add("Warden", (bool)CB_Warden.IsChecked);
            checkboxStateDict.Add("Warlock", (bool)CB_Warlock.IsChecked);
            checkboxStateDict.Add("Assassin_3RD", (bool)CB_Assasin_3RD.IsChecked);
            checkboxStateDict.Add("BM_3RD", (bool)CB_BM_3RD.IsChecked);
            checkboxStateDict.Add("Destroyer_3RD", (bool)CB_Destroyer_3RD.IsChecked);
            checkboxStateDict.Add("FM_3RD", (bool)CB_FM_3RD.IsChecked);
            checkboxStateDict.Add("KFM_3RD", (bool)CB_KFM_3RD.IsChecked);
            checkboxStateDict.Add("Destro_Dummy", (bool)CB_Destro_Dummy.IsChecked);
            checkboxStateDict.Add("FM_Dummy", (bool)CB_FM_Dummy.IsChecked);
            checkboxStateDict.Add("FM_3RD_ANI", (bool)CB_FM_3RD_ANI.IsChecked);
            checkboxStateDict.Add("Astromancer", (bool)CB_Astromancer.IsChecked);
            //
            checkboxStateDict.Add("CB_Assassin_ANI", (bool)CB_Assassin_ANI.IsChecked);
            checkboxStateDict.Add("CB_Summoner_ANI", (bool)CB_Summoner_ANI.IsChecked);
            checkboxStateDict.Add("CB_KFM_ANI", (bool)CB_KFM_ANI.IsChecked);
            checkboxStateDict.Add("CB_Gunner_ANI", (bool)CB_Gunner_ANI.IsChecked);
            checkboxStateDict.Add("CB_Destro_ANI", (bool)CB_Destro_ANI.IsChecked);
            checkboxStateDict.Add("CB_Warden_ANI", (bool)CB_Warden_ANI.IsChecked);
            checkboxStateDict.Add("CB_Archer_ANI", (bool)CB_Archer_ANI.IsChecked);
            checkboxStateDict.Add("CB_FM_ANI", (bool)CB_FM_ANI.IsChecked);
            checkboxStateDict.Add("CB_BM_ANI", (bool)CB_BM_ANI.IsChecked);
            checkboxStateDict.Add("CB_BD_ANI", (bool)CB_BD_ANI.IsChecked);
            checkboxStateDict.Add("CB_Warlock_ANI", (bool)CB_Warlock_ANI.IsChecked);
            checkboxStateDict.Add("CB_SF_ANI", (bool)CB_SF_ANI.IsChecked);

            Thread t1 = new Thread(new ThreadStart(ThreadDoMoving));
            t1.Name = "Effect Remover thread";
            t1.Start();
        }
        public void ThreadDoMoving()
        {
            TB_console.Dispatcher.BeginInvoke((Action)(() =>
            {
                TB_console.Inlines.Add("----------------- \n");
            }));
            //TB_console.Inlines.Add("----------------- \n");
            if (Process.GetProcessesByName("Client").Length > 0)
            {
                // Is running
                System.Windows.MessageBox.Show("Blade and Soul is running, please close the game and then proceed with the operation");
            }
            else
            {
                switch (buttonName)
                {
                    case "Restore": //manage the restore function -> moves every upk file found in the backup folder to the coockedPC folder
                        if (Directory.Exists(folders.backup_folder))
                        {
                            string[] files = Directory.GetFiles(folders.backup_folder, "*.upk");
                            try
                            {
                                foreach (var item in files)
                                {
                                    if (System.IO.Path.GetFileName(item) != Class_UPK_s.DMG[0])
                                    {
                                        File.Move(item, System.IO.Path.Combine(folders.coockedPC_folder, System.IO.Path.GetFileName(item)));
                                        TB_console.Dispatcher.BeginInvoke((Action)(() =>
                                        {
                                            TB_console.Inlines.Add(">> Restoring: " + System.IO.Path.GetFileName(item) + "\n");
                                        }));
                                    }
                                    else
                                    {
                                        File.Move(item, System.IO.Path.Combine(folders.coockedPC_eng_folder, System.IO.Path.GetFileName(item)));
                                        TB_console.Dispatcher.BeginInvoke((Action)(() =>
                                        {
                                            TB_console.Inlines.Add(">> Restoring: " + System.IO.Path.GetFileName(item) + "\n");
                                        }));
                                    }
                                }
                            }
                            catch (Exception er)
                            {
                                Console.WriteLine("Error: " + er.Message);
                            }
                        }
                        break;
                    case "Remove":  //CB stands for checkbox, it checks if a determinate checkbox is checked and if it is it proceed with moving the given upk names into the backup folder
                        if (Directory.Exists(folders.coockedPC_folder) && Directory.Exists(folders.backup_folder))
                        {
                            string[] files = Directory.GetFiles(folders.coockedPC_folder, "*.upk");
                            string[] files_eng = Directory.GetFiles(folders.coockedPC_eng_folder, "*.upk");
                            if (checkboxStateDict["otherSFX"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.OtherSFX, folders.backup_folder, "<< otherSFX:");
                            }
                            if (checkboxStateDict["Sounds"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.Sounds, folders.backup_folder, "<< Sounds:");
                            }
                            if (checkboxStateDict["DMG"] == true)
                            {
                                CheckBoxOperations(files_eng, Class_UPK_s.DMG, folders.backup_folder, "<< DMG text:");
                            }
                            if (checkboxStateDict["Archer"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.Archer, folders.backup_folder, "<< Archer:");
                            }
                            if (checkboxStateDict["Assassin"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.Assassin, folders.backup_folder, "<< Assassin:");
                            }
                            if (checkboxStateDict["BD"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.BD, folders.backup_folder, "<< Blade Dancer:");
                            }
                            if (checkboxStateDict["BM"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.BM, folders.backup_folder, "<< Blade Master:");
                            }
                            if (checkboxStateDict["Destro"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.Destro, folders.backup_folder, "<< Destroyer:");
                            }
                            if (checkboxStateDict["FM"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.FM, folders.backup_folder, "<< Force Master:");
                            }
                            if (checkboxStateDict["Gunner"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.Gunner, folders.backup_folder, "<< Gunslinger:");
                            }
                            if (checkboxStateDict["KFM"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.KFM, folders.backup_folder, "<< Kung Fu Master:");
                            }
                            if (checkboxStateDict["SF"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.SF, folders.backup_folder, "<< Soul Fighter:");
                            }
                            if (checkboxStateDict["Sum"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.Summoner, folders.backup_folder, "<< Summoner:");
                            }
                            if (checkboxStateDict["Warden"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.Warden, folders.backup_folder, "<< Warden:");
                            }
                            if (checkboxStateDict["Warlock"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.Warlock, folders.backup_folder, "<< Warlock:");
                            }
                            if (checkboxStateDict["Assassin_3RD"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.Assassin_3RD, folders.backup_folder, "<< Assassin 3rd:");
                            }
                            if (checkboxStateDict["BM_3RD"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.BM_3RD, folders.backup_folder, "<< Blade Master 3rd:");
                            }
                            if (checkboxStateDict["Destroyer_3RD"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.Destro_3RD, folders.backup_folder, "<< Destroyer 3rd:");
                            }
                            if (checkboxStateDict["FM_3RD"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.FM_3RD, folders.backup_folder, "<< Force Master 3rd:");
                            }
                            if (checkboxStateDict["KFM_3RD"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.KFM_3RD, folders.backup_folder, "<< Kung Fu Master 3rd:");
                            }
                            if (checkboxStateDict["Astromancer"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.Astromancer, folders.backup_folder, "<< Astromancer:");
                            }
                            //dummy
                            if (checkboxStateDict["Destro_Dummy"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.Destro_Dummy, folders.backup_folder, "<< Destroyer dummy:");
                            }
                            if (checkboxStateDict["FM_Dummy"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.FM_Dummy, folders.backup_folder, "<< Force Master dummy:");
                            }
                            //animation
                            if (checkboxStateDict["FM_3RD_ANI"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.FM_3RD_ANI, folders.backup_folder, "<< Force Master 3rd animation:");
                            }
                            if (checkboxStateDict["CB_Assassin_ANI"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.Assassin_ANI, folders.backup_folder, "<< Assassin animation:");
                            }
                            if (checkboxStateDict["CB_Summoner_ANI"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.Summoner_ANI, folders.backup_folder, "<< Summoner animation:");
                            }
                            if (checkboxStateDict["CB_KFM_ANI"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.KFM_ANI, folders.backup_folder, "<< Kung Fu Master animation:");
                            }
                            if (checkboxStateDict["CB_Gunner_ANI"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.Gunner_ANI, folders.backup_folder, "<< Gunslinger animation:");
                            }
                            if (checkboxStateDict["CB_Destro_ANI"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.Destro_ANI, folders.backup_folder, "<< Destroyer animation:");
                            }
                            if (checkboxStateDict["CB_Warden_ANI"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.Warden_ANI, folders.backup_folder, "<< Warden animation:");
                            }
                            if (checkboxStateDict["CB_Archer_ANI"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.Archer_ANI, folders.backup_folder, "<< Archer animation:");
                            }
                            if (checkboxStateDict["CB_FM_ANI"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.FM_ANI, folders.backup_folder, "<< Force Master animation:");
                            }
                            if (checkboxStateDict["CB_BM_ANI"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.BM_ANI, folders.backup_folder, "<< Blade Master animation:");
                            }
                            if (checkboxStateDict["CB_BD_ANI"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.BD_ANI, folders.backup_folder, "<< Blade Dancer animation:");
                            }
                            if (checkboxStateDict["CB_Warlock_ANI"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.Warlock_ANI, folders.backup_folder, "<< Warlock animation:");
                            }
                            if (checkboxStateDict["CB_SF_ANI"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.SF_ANI, folders.backup_folder, "<< Soul Fighter animation:");
                            }

                        }
                        break;     
                    case "Add": //CB stands for checkbox, it checks if a determinate checkbox is checked and if it is it proceed with moving the given upk names into the coockedPC folder
                        if (Directory.Exists(folders.backup_folder) && Directory.Exists(folders.coockedPC_folder))
                        {
                            string[] files = Directory.GetFiles(folders.backup_folder, "*.upk");
                            if (checkboxStateDict["otherSFX"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.OtherSFX, folders.coockedPC_folder, ">> otherSFX:");
                            }
                            if (checkboxStateDict["Sounds"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.Sounds, folders.coockedPC_folder, ">> Sounds:");
                            }
                            if (checkboxStateDict["DMG"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.DMG, folders.coockedPC_eng_folder, ">> DMG:");
                            }
                            if (checkboxStateDict["Archer"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.Archer, folders.coockedPC_folder, ">> Archer:");
                            }
                            if (checkboxStateDict["Assassin"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.Assassin, folders.coockedPC_folder, ">> Assassin:");
                            }
                            if (checkboxStateDict["BD"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.BD, folders.coockedPC_folder, ">> Blade Dancer:");
                            }
                            if (checkboxStateDict["BM"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.BM, folders.coockedPC_folder, ">> Blade Master:");
                            }
                            if (checkboxStateDict["Destro"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.Destro, folders.coockedPC_folder, ">> Destroyer:");
                            }
                            if (checkboxStateDict["FM"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.FM, folders.coockedPC_folder, ">> Force Master:");
                            }
                            if (checkboxStateDict["Gunner"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.Gunner, folders.coockedPC_folder, ">> Gunslinger:");
                            }
                            if (checkboxStateDict["KFM"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.KFM, folders.coockedPC_folder, ">> Kung Fu Master:");
                            }
                            if (checkboxStateDict["SF"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.SF, folders.coockedPC_folder, ">> Soul Fighter:");
                            }
                            if (checkboxStateDict["Sum"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.Summoner, folders.coockedPC_folder, ">> Summoner:");
                            }
                            if (checkboxStateDict["Warden"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.Warden, folders.coockedPC_folder, ">> Warden:");
                            }
                            if (checkboxStateDict["Warlock"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.Warlock, folders.coockedPC_folder, ">> Warlock:");
                            }
                            if (checkboxStateDict["Assassin_3RD"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.Assassin_3RD, folders.coockedPC_folder, ">> Assassin 3rd:");
                            }
                            if (checkboxStateDict["BM_3RD"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.BM_3RD, folders.coockedPC_folder, ">> Blade Master 3rd:");
                            }
                            if (checkboxStateDict["Destroyer_3RD"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.Destro_3RD, folders.coockedPC_folder, ">> Destroyer 3rd:");
                            }
                            if (checkboxStateDict["FM_3RD"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.FM_3RD, folders.coockedPC_folder, ">> Force Master 3rd:");
                            }
                            if (checkboxStateDict["KFM_3RD"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.KFM_3RD, folders.coockedPC_folder, ">> Kung Fu Master 3rd:");
                            }
                            if (checkboxStateDict["Astromancer"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.Astromancer, folders.coockedPC_folder, ">> Astromancer:");
                            }
                            //dummy
                            if (checkboxStateDict["Destro_Dummy"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.Destro_Dummy, folders.coockedPC_folder, ">> Destroyer dummy:");
                            }
                            if (checkboxStateDict["FM_Dummy"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.FM_Dummy, folders.coockedPC_folder, ">> Force Master dummy:");
                            }
                            //animation
                            if (checkboxStateDict["FM_3RD_ANI"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.FM_3RD_ANI, folders.coockedPC_folder, ">> Force Master 3rd animation:");
                            }
                            //animation
                            if (checkboxStateDict["FM_3RD_ANI"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.FM_3RD_ANI, folders.coockedPC_folder, ">> Force Master 3rd animation:");
                            }
                            if (checkboxStateDict["CB_Assassin_ANI"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.Assassin_ANI, folders.coockedPC_folder, ">> Assassin animation:");
                            }
                            if (checkboxStateDict["CB_Summoner_ANI"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.Summoner_ANI, folders.coockedPC_folder, ">> Summoner animation:");
                            }
                            if (checkboxStateDict["CB_KFM_ANI"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.KFM_ANI, folders.coockedPC_folder, ">> Kung Fu Master animation:");
                            }
                            if (checkboxStateDict["CB_Gunner_ANI"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.Gunner_ANI, folders.coockedPC_folder, ">> Gunslinger animation:");
                            }
                            if (checkboxStateDict["CB_Destro_ANI"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.Destro_ANI, folders.coockedPC_folder, ">> Destroyer animation:");
                            }
                            if (checkboxStateDict["CB_Warden_ANI"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.Warden_ANI, folders.coockedPC_folder, ">> Warden animation:");
                            }
                            if (checkboxStateDict["CB_Archer_ANI"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.Archer_ANI, folders.coockedPC_folder, ">> Archer animation:");
                            }
                            if (checkboxStateDict["CB_FM_ANI"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.FM_ANI, folders.coockedPC_folder, ">> Force Master animation:");
                            }
                            if (checkboxStateDict["CB_BM_ANI"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.BM_ANI, folders.coockedPC_folder, ">> Blade Master animation:");
                            }
                            if (checkboxStateDict["CB_BD_ANI"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.BD_ANI, folders.coockedPC_folder, ">> Blade Dancer animation:");
                            }
                            if (checkboxStateDict["CB_Warlock_ANI"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.Warlock_ANI, folders.coockedPC_folder, ">> Warlock animation:");
                            }
                            if (checkboxStateDict["CB_SF_ANI"] == true)
                            {
                                CheckBoxOperations(files, Class_UPK_s.SF_ANI, folders.coockedPC_folder, ">> Soul Fighter animation:");
                            }
                        }
                        break;
                }
            }
        }
        //manage the "select all" checkbox
        private void SA_checkbox(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.CheckBox obj = (System.Windows.Controls.CheckBox)sender;
            switch (obj.IsChecked.ToString())
            {
                case "True":
                    CB_Assassin.IsChecked = true;
                    CB_BD.IsChecked = true;
                    CB_BM.IsChecked = true;
                    CB_Destro.IsChecked = true;
                    CB_FM.IsChecked = true;
                    CB_Gunner.IsChecked = true;
                    CB_KFM.IsChecked = true;
                    CB_SF.IsChecked = true;
                    CB_Summ.IsChecked = true;
                    CB_Warden.IsChecked = true;
                    CB_Warlock.IsChecked = true;
                    CB_Archer.IsChecked = true;
                    CB_Assasin_3RD.IsChecked = true;
                    CB_BM_3RD.IsChecked = true;
                    CB_Destroyer_3RD.IsChecked = true;
                    CB_FM_3RD.IsChecked = true;
                    CB_KFM_3RD.IsChecked = true;
                    CB_Sounds.IsChecked = true;
                    CB_otherSFX.IsChecked = true;
                    CB_DMG.IsChecked = true;
                    CB_Astromancer.IsChecked = true;
                    break;
                case "False":
                    CB_Assassin.IsChecked = false;
                    CB_BD.IsChecked = false;
                    CB_BM.IsChecked = false;
                    CB_Destro.IsChecked = false;
                    CB_FM.IsChecked = false;
                    CB_Gunner.IsChecked = false;
                    CB_KFM.IsChecked = false;
                    CB_SF.IsChecked = false;
                    CB_Summ.IsChecked = false;
                    CB_Warden.IsChecked = false;
                    CB_Warlock.IsChecked = false;
                    CB_Archer.IsChecked = false;
                    CB_Assasin_3RD.IsChecked = false;
                    CB_BM_3RD.IsChecked = false;
                    CB_Destroyer_3RD.IsChecked = false;
                    CB_FM_3RD.IsChecked = false;
                    CB_KFM_3RD.IsChecked = false;
                    CB_Sounds.IsChecked = false;
                    CB_otherSFX.IsChecked = false;
                    CB_DMG.IsChecked = false;
                    CB_Astromancer.IsChecked = false;
                    break;
            }
        }
        private void StatusCheck(object sender, EventArgs e)
        {
            this.Hide();
            UpdateChecker upc = new UpdateChecker(this);
            bool check = true;
            Task.Run(async () => { 
                check = await checkLatestVersion();
                if (check == false) {
                    await upc.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        upc.ShowDialog();
                    }));
                }
                else
                {
                    await this.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        this.Show();
                    }));
                    await upc.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        upc.Close();
                    }));
                }
            });
        }
        public async Task<bool> checkLatestVersion()
        {
            var client = new GitHubClient(new ProductHeaderValue("Code-Dani"));
            var releases = await client.Repository.Release.GetLatest("Code-Dani", "BnS_EffectRemover");
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            if (releases != null)
            {
                if (releases.TagName == fvi.FileVersion)
                {
                    await LB_status.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        LB_status.Content = "Status: version up to date";
                        LB_status.Foreground = new SolidColorBrush(Colors.Green);
                    }));
                }
                else
                {
                    await LB_status.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        LB_status.Content = "Status: version outdated";
                        LB_status.Foreground = new SolidColorBrush(Colors.Red);
                    }));
                    return false;
                }
            }
            return true;
        }
        public void CheckBoxOperations(string[] files, string[] Class_Upks, string folderPath, string consoleText)
        {
            foreach (var temp in Class_Upks)
            {
                foreach (var item in files)
                {
                    if (System.IO.Path.GetFileName(item) == temp)
                    {
                        try
                        {
                            if (folderPath == folders.backup_folder)
                            {
                                if (File.Exists(System.IO.Path.Combine(folders.backup_folder, System.IO.Path.GetFileName(item))))
                                {
                                    File.Delete(System.IO.Path.Combine(folderPath, System.IO.Path.GetFileName(item)));
                                }
                            }
                            File.Move(item, System.IO.Path.Combine(folderPath, System.IO.Path.GetFileName(item)));
                            TB_console.Dispatcher.BeginInvoke((Action)(() =>
                            {
                                TB_console.Inlines.Add(consoleText + " " + temp + "\n");
                            }));
                            SV_console.Dispatcher.BeginInvoke((Action)(() =>
                            {
                                SV_console.ScrollToEnd();
                            }));
                            //TB_console.Inlines.Add(consoleText + " " + temp + "\n");
                        }
                        catch (Exception er)
                        {
                            Console.WriteLine("Error: " + er.Message);
                        }
                    }
                }
            }
        }
        private void donateNow(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.paypal.com/paypalme/satrianodaniel");
        }
        private void SW_CacheCleaner(object sender, RoutedEventArgs e)
        {
            
            ToggleButton SW_CC = (ToggleButton)sender;
            switch (SW_CC.IsChecked)
            {
                case true:
                    if(CB_Cmins.SelectedItem != null)
                    {
                        CB_Cmins.IsEnabled = false;
                        ComboBoxItem item = (ComboBoxItem)CB_Cmins.SelectedItem;
                        TimerMinutes = double.Parse(item.Tag.ToString());
                        timer.Interval = TimeSpan.FromSeconds((double)1);
                        timer.Start();
                    }
                    break;
                case false:
                    timer.Stop();
                    CleanCount = 0;
                    CB_Cmins.IsEnabled = true;
                    break;
                default:
                    break;
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                ProcessStartInfo info = new ProcessStartInfo(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\EmptyStandbyList.exe");
                info.UseShellExecute = false;
                info.CreateNoWindow = true;
                info.Verb = "runas";
                Process.Start(info);
                TB_console.Dispatcher.BeginInvoke((Action)(() =>
                {
                    TB_console.Inlines.Add("!! RAM cache cleaned "+ CleanCount + " times\n");
                }));
                CleanCount++;
                if(CleanCount == 1)
                {
                    timer.Interval = TimeSpan.FromMinutes(TimerMinutes);
                }
            }
            catch(Exception exp)
            {
                Console.WriteLine(exp.Message);
                timer.Stop();
                TB_console.Dispatcher.BeginInvoke((Action)(() =>
                {
                    TB_console.Inlines.Add("!! Something went wrong, cleaning process stopped\n");
                }));
            }
        }
        private void SA_ANI_checkbox(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.CheckBox obj = (System.Windows.Controls.CheckBox)sender;
            switch (obj.IsChecked.ToString())
            {
                case "True":
                    CB_Assassin_ANI.IsChecked = true;
                    CB_Summoner_ANI.IsChecked = true;
                    CB_KFM_ANI.IsChecked = true;
                    CB_Gunner_ANI.IsChecked = true;
                    CB_Destro_ANI.IsChecked = true;
                    CB_Warden_ANI.IsChecked = true;
                    CB_FM_ANI.IsChecked = true;
                    CB_FM_3RD_ANI.IsChecked = true;
                    CB_BM_ANI.IsChecked = true;
                    CB_BD_ANI.IsChecked = true;
                    CB_Warlock_ANI.IsChecked = true;
                    CB_SF_ANI.IsChecked = true;
                    CB_Archer_ANI.IsChecked = true;
                    break;
                case "False":
                    CB_Assassin_ANI.IsChecked = false;
                    CB_Summoner_ANI.IsChecked = false;
                    CB_KFM_ANI.IsChecked = false;
                    CB_Gunner_ANI.IsChecked = false;
                    CB_Destro_ANI.IsChecked = false;
                    CB_Warden_ANI.IsChecked = false;
                    CB_FM_ANI.IsChecked = false;
                    CB_FM_3RD_ANI.IsChecked = false;
                    CB_BM_ANI.IsChecked = false;
                    CB_BD_ANI.IsChecked = false;
                    CB_Warlock_ANI.IsChecked = false;
                    CB_SF_ANI.IsChecked = false;
                    CB_Archer_ANI.IsChecked = false;
                    break;
            }
        }
        private void tbBrowseOpenDialog(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.TextBlock tb = (System.Windows.Controls.TextBlock)sender;
            if(!tb.Text.Equals("Browse BnS patch backup folder") && !tb.Text.Equals("Browse BnS CookedPC folder"))
            {
                try
                {
                    Process.Start(tb.Text);
                }
                catch (Exception win32Exception)
                {
                    //The system cannot find the file specified...
                    Console.WriteLine(win32Exception.Message);
                }
            }
        }
    }
}
