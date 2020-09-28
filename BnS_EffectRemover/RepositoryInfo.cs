using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BnS_EffectRemover
{
    public class RepositoryInfo
    {
        public string backup_folder { get; set; }
        public string coockedPC_folder { get; set; }
        public string coockedPC_eng_folder { get; set; }
        public string update_Download_Path { get; set; }
        public RepositoryInfo(string backup_folder, string coockedPC_folder, string coockedPC_eng_folder)
        {
            this.backup_folder = backup_folder;
            this.coockedPC_folder = coockedPC_folder;
            this.coockedPC_eng_folder = coockedPC_eng_folder;
        }
        public RepositoryInfo(string backup_folder, string coockedPC_folder, string coockedPC_eng_folder, string update_Download_Path)
        {
            this.backup_folder = backup_folder;
            this.coockedPC_folder = coockedPC_folder;
            this.coockedPC_eng_folder = coockedPC_eng_folder;
            this.update_Download_Path = update_Download_Path;
        }
        public RepositoryInfo() { }
    }
}
