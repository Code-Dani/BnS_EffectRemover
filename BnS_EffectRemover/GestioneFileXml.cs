using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;

namespace BnS_EffectRemover
{
    static class GestioneFileXML
    {
        static string path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\"; //percorso con \ finale
        static string nomeFile = "path.xml";

        public static void ScriviXml(RepositoryInfo rep) //write xml
        {
            try
            {
                //Istanzio l'oggetto serializzatore
                XmlSerializer xmls = new XmlSerializer(typeof(RepositoryInfo));
                StreamWriter sw = new StreamWriter(path + nomeFile, false); //aggiunge
                xmls.Serialize(sw, rep);
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Errore in scrittura file {0}\n{1}", path + nomeFile, e.Message); //messaggio di errore
            }
        }
        public static RepositoryInfo LeggiLista() //read xml
        {
            try
            {
                XmlSerializer xmls = new XmlSerializer(typeof(RepositoryInfo));
                StreamReader sr = new StreamReader(path+nomeFile);
                RepositoryInfo list = (RepositoryInfo)xmls.Deserialize(sr);
                sr.Close();
                return list;

            }
            catch (Exception e)
            {
                Console.WriteLine("Errore in scrittura file {0}\n{1}", path+nomeFile, e.Message); //messaggio di errore
                return new RepositoryInfo();
            }
        }
    }
}
