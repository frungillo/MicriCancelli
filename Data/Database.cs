using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using MicriCancelli.Services;

namespace MicriCancelli.Data
{
    /// <summary>
    /// Apre il DB SQLite e porta lo schema alla versione corrente.
    /// Le date sono salvate come testo ISO (yyyy-MM-dd HH:mm:ss): ordinabile e indipendente dalla cultura del PC.
    /// </summary>
    public sealed class Database
    {
        public const string FormatoData = "yyyy-MM-dd HH:mm:ss";
        private const int VersioneSchema = 2;

        private readonly string _connectionString;
        private readonly int _giorniLegacyDaCopiare;

        public string Percorso { get; }

        /// <summary>True se in questo avvio lo schema è stato migrato (dopo conviene compattare il file).</summary>
        public bool MigrazioneEseguita { get; private set; }

        /// <param name="giorniLegacyDaCopiare">Nella migrazione dal vecchio schema si copiano solo i codici emessi in questi ultimi giorni: i più vecchi sono scaduti da tempo.</param>
        public Database(string percorso, int giorniLegacyDaCopiare = 30)
        {
            Percorso = percorso;
            _giorniLegacyDaCopiare = giorniLegacyDaCopiare;
            _connectionString = new SQLiteConnectionStringBuilder
            {
                DataSource = percorso,
                FailIfMissing = false,
                BusyTimeout = 3000
            }.ToString();
        }

        public SQLiteConnection Apri()
        {
            var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            return conn;
        }

        public static string FormattaData(DateTime d) => d.ToString(FormatoData, CultureInfo.InvariantCulture);

        public static DateTime LeggiData(object valore) =>
            DateTime.ParseExact(Convert.ToString(valore), FormatoData, CultureInfo.InvariantCulture);

        public static DateTime? LeggiDataOpzionale(object valore)
        {
            if (valore == null || valore is DBNull) return null;
            var s = Convert.ToString(valore);
            return string.IsNullOrWhiteSpace(s) ? (DateTime?)null : LeggiData(s);
        }

        /// <summary>Crea le tabelle se mancano e migra i dati legacy (date dd/MM/yyyy in colonne separate).</summary>
        public void Inizializza()
        {
            var cartella = Path.GetDirectoryName(Percorso);
            if (!string.IsNullOrEmpty(cartella)) Directory.CreateDirectory(cartella);

            using (var conn = Apri())
            {
                var versione = Convert.ToInt32(Scalar(conn, "PRAGMA user_version"));
                if (versione >= VersioneSchema) return;

                using (var tx = conn.BeginTransaction())
                {
                    MigraAVersione2(conn);
                    Esegui(conn, "PRAGMA user_version = " + VersioneSchema);
                    tx.Commit();
                }
                MigrazioneEseguita = true;
                Log.Info("Schema DB portato alla versione " + VersioneSchema);
            }
        }

        private void MigraAVersione2(SQLiteConnection conn)
        {
            bool codiciLegacy = TabellaEsiste(conn, "codici") && ColonnaEsiste(conn, "codici", "data_emissione");
            bool parametriLegacy = TabellaEsiste(conn, "parametri") && !IndiceUnicoSuChiave(conn);

            if (codiciLegacy) Esegui(conn, "ALTER TABLE codici RENAME TO codici_legacy");
            Esegui(conn, @"CREATE TABLE IF NOT EXISTS codici (
                              id_codice INTEGER PRIMARY KEY AUTOINCREMENT,
                              codice    INTEGER NOT NULL UNIQUE,
                              emesso_il TEXT    NOT NULL,
                              usato_il  TEXT    NULL,
                              tipo      TEXT    NOT NULL DEFAULT 'QR')");
            Esegui(conn, "CREATE INDEX IF NOT EXISTS ix_codici_emesso ON codici(emesso_il)");
            if (codiciLegacy) CopiaCodiciLegacy(conn);

            if (parametriLegacy) Esegui(conn, "ALTER TABLE parametri RENAME TO parametri_legacy");
            Esegui(conn, @"CREATE TABLE IF NOT EXISTS parametri (
                              id_parametro INTEGER PRIMARY KEY AUTOINCREMENT,
                              key   TEXT NOT NULL UNIQUE,
                              value TEXT)");
            if (parametriLegacy)
            {
                Esegui(conn, "INSERT OR IGNORE INTO parametri(key, value) SELECT key, value FROM parametri_legacy ORDER BY id_parametro");
                Esegui(conn, "DROP TABLE parametri_legacy");
            }
        }

        /// <summary>
        /// Converte le righe legacy recenti (ultimi <see cref="_giorniLegacyDaCopiare"/> giorni) e poi elimina la
        /// tabella legacy: un biglietto vale pochi minuti, lo storico più vecchio è solo peso (centinaia di
        /// migliaia di righe dopo anni di esercizio). Le righe non interpretabili vengono scartate e loggate.
        /// </summary>
        private void CopiaCodiciLegacy(SQLiteConnection conn)
        {
            int copiate = 0, scartate = 0, vecchie = 0;
            var limite = DateTime.Now.AddDays(-_giorniLegacyDaCopiare);
            var righe = new List<object[]>();
            using (var cmd = new SQLiteCommand("SELECT codice, data_emissione, ora_emissione, data_uso, ora_uso FROM codici_legacy ORDER BY id_codice", conn))
            using (var r = cmd.ExecuteReader())
                while (r.Read())
                    righe.Add(new[] { r[0], r[1], r[2], r[3], r[4] });

            foreach (var riga in righe)
            {
                int numero;
                DateTime emesso;
                if (!int.TryParse(Convert.ToString(riga[0]), out numero) ||
                    !ProvaLeggiDataLegacy(Convert.ToString(riga[1]), Convert.ToString(riga[2]), out emesso))
                {
                    scartate++;
                    Log.Info("Migrazione: riga legacy non convertibile, scartata: " + string.Join(" | ", riga));
                    continue;
                }
                if (emesso < limite) { vecchie++; continue; }
                DateTime usato;
                var usatoIl = ProvaLeggiDataLegacy(Convert.ToString(riga[3]), Convert.ToString(riga[4]), out usato) ? (DateTime?)usato : null;

                using (var ins = new SQLiteCommand("INSERT OR IGNORE INTO codici(codice, emesso_il, usato_il, tipo) VALUES (@c, @e, @u, 'CODE39')", conn))
                {
                    ins.Parameters.AddWithValue("@c", numero);
                    ins.Parameters.AddWithValue("@e", FormattaData(emesso));
                    ins.Parameters.AddWithValue("@u", usatoIl.HasValue ? (object)FormattaData(usatoIl.Value) : DBNull.Value);
                    copiate += ins.ExecuteNonQuery();
                }
            }
            Esegui(conn, "DROP TABLE codici_legacy");
            Log.Info("Migrazione codici legacy: " + copiate + " copiate (ultimi " + _giorniLegacyDaCopiare + " giorni), " +
                     vecchie + " più vecchie non copiate, " + scartate + " non interpretabili; tabella legacy eliminata");
        }

        private static readonly string[] FormatiLegacy =
        {
            "dd/MM/yyyy HH:mm", "dd/MM/yyyy HH:mm:ss", "yyyy/MM/dd HH:mm", "yyyy-MM-dd HH:mm", "yyyy-MM-dd HH:mm:ss", "d/M/yyyy H:mm"
        };

        private static bool ProvaLeggiDataLegacy(string data, string ora, out DateTime risultato)
        {
            risultato = default(DateTime);
            if (string.IsNullOrWhiteSpace(data)) return false;
            var testo = (data.Trim() + " " + (ora ?? "").Trim()).Trim();
            if (testo == data.Trim()) testo += " 00:00";
            return DateTime.TryParseExact(testo, FormatiLegacy, CultureInfo.InvariantCulture, DateTimeStyles.None, out risultato);
        }

        private static bool TabellaEsiste(SQLiteConnection conn, string nome)
        {
            using (var cmd = new SQLiteCommand("SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name=@n", conn))
            {
                cmd.Parameters.AddWithValue("@n", nome);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private static bool ColonnaEsiste(SQLiteConnection conn, string tabella, string colonna)
        {
            using (var cmd = new SQLiteCommand("PRAGMA table_info(" + tabella + ")", conn))
            using (var r = cmd.ExecuteReader())
                while (r.Read())
                    if (string.Equals(Convert.ToString(r["name"]), colonna, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        private static bool IndiceUnicoSuChiave(SQLiteConnection conn)
        {
            using (var cmd = new SQLiteCommand("PRAGMA index_list(parametri)", conn))
            using (var r = cmd.ExecuteReader())
                while (r.Read())
                    if (Convert.ToInt32(r["unique"]) == 1) return true;
            return false;
        }

        private static void Esegui(SQLiteConnection conn, string sql)
        {
            using (var cmd = new SQLiteCommand(sql, conn)) cmd.ExecuteNonQuery();
        }

        private static object Scalar(SQLiteConnection conn, string sql)
        {
            using (var cmd = new SQLiteCommand(sql, conn)) return cmd.ExecuteScalar();
        }
    }
}
