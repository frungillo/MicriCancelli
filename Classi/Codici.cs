using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicriCancelli.Classi
{
    public class Codici
    {
        private int id_codice;
        private int codice;
        private string data_emissione;
        private string ora_emissione;
        private string data_uso;
        private string ora_uso;

        public int Id_codice { get => id_codice; set => id_codice = value; }
        public int Codice { get => codice; set => codice = value; }
        public string Data_emissione { get => data_emissione; set => data_emissione = value; }
        public string Ora_emissione { get => ora_emissione; set => ora_emissione = value; }
        public string Data_uso { get => data_uso; set => data_uso = value; }
        public string Ora_uso { get => ora_uso; set => ora_uso = value; }

        public Codici() { }
        
        public static bool isCodiceValido(int id_codice) 
        {
            string dbFilePath = MicriCancelli.Properties.Settings.Default.dbpathFile;
            SQLiteManager manager = new SQLiteManager(dbFilePath);
            Codici codice = new Codici();
            codice=manager.ReadCodice(id_codice);

            DateTime data_emissione=DateTime.Parse(codice.Data_emissione+" "+codice.Ora_emissione);
            if (codice.data_uso == "") // mai usato, scaduto?
            {
                if (data_emissione.AddMinutes(30) >= DateTime.Now) return true;  
                else return false;
            }
            //usato già
            return false;
        }
        public static bool isCodiceEsistente(int id_codice)
        {
            string dbFilePath = MicriCancelli.Properties.Settings.Default.dbpathFile;
            SQLiteManager manager = new SQLiteManager(dbFilePath);
            Codici codice = new Codici();
            codice = manager.ReadCodice(id_codice);
            return !(codice.codice==0);
           
        }
    }
}
