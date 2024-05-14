using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicriCancelli.Classi
{
    public class Parametri
    {
        private int id_parametro;
        private string key;
        private string value;

        public int Id_parametro { get => id_parametro; set => id_parametro = value; }
        public string Key { get => key; set => key = value; }
        public string Value { get => value; set => this.value = value; }

        public Parametri() { }
        public static Parametri GetParametro(string key)
        {
            string dbFilePath = MicriCancelli.Properties.Settings.Default.dbpathFile;
            SQLiteManager manager = new SQLiteManager(dbFilePath);
            Parametri p = new Parametri();
            p = manager.GetParametro(key);
            return p;
        }
        public static void UpdateParametro(Parametri p) 
        {
            string dbFilePath = MicriCancelli.Properties.Settings.Default.dbpathFile;
            SQLiteManager manager = new SQLiteManager(dbFilePath);
            manager.UpdateParametro(p.Id_parametro,p.Key,p.Value);
        }

    }
}
