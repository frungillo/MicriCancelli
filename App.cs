using System;
using System.IO;
using MicriCancelli.Data;
using MicriCancelli.Services;

namespace MicriCancelli
{
    /// <summary>Punto di composizione dell'applicazione: crea DB, repository e servizi e li rende disponibili ai form.</summary>
    public static class App
    {
        public static Database Db { get; private set; }
        public static ParametriRepository Parametri { get; private set; }
        public static CodiciRepository Codici { get; private set; }
        public static Impostazioni Impostazioni { get; private set; }
        public static Cancello Cancello { get; private set; }
        public static GestoreAccessi Accessi { get; private set; }
        public static StampaBiglietto Stampa { get; private set; }
        public static ComandiRemoti Remoti { get; private set; }

        public static void Inizializza()
        {
            var percorsoDb = Properties.Settings.Default.dbpathFile;
            if (!Path.IsPathRooted(percorsoDb))
                percorsoDb = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, percorsoDb));

            Log.Info("Avvio MicriCancelli, database: " + percorsoDb);
            Db = new Database(percorsoDb);
            Db.Inizializza();

            Parametri = new ParametriRepository(Db);
            Parametri.AssicuraPredefiniti(Impostazioni.Predefiniti());
            Codici = new CodiciRepository(Db);

            Impostazioni = new Impostazioni();
            Impostazioni.Carica(Parametri);

            Cancello = new Cancello(Impostazioni);
            Accessi = new GestoreAccessi(Codici, Impostazioni, Cancello);
            Stampa = new StampaBiglietto(Impostazioni);
            Remoti = new ComandiRemoti(Impostazioni, Cancello);
        }

        public static void RicaricaImpostazioni()
        {
            Impostazioni.Carica(Parametri);
            Log.Info("Impostazioni ricaricate");
        }
    }
}
