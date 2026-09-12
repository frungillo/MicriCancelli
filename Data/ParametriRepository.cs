using System;
using System.Collections.Generic;
using System.Data.SQLite;
using MicriCancelli.Models;

namespace MicriCancelli.Data
{
    public sealed class ParametriRepository
    {
        private readonly Database _db;

        public ParametriRepository(Database db) { _db = db; }

        /// <summary>Valore del parametro, oppure null se la chiave non esiste.</summary>
        public string Leggi(string chiave)
        {
            using (var conn = _db.Apri())
            using (var cmd = new SQLiteCommand("SELECT value FROM parametri WHERE key = @k", conn))
            {
                cmd.Parameters.AddWithValue("@k", chiave);
                var v = cmd.ExecuteScalar();
                return v == null || v is DBNull ? null : Convert.ToString(v);
            }
        }

        /// <summary>Inserisce o aggiorna (upsert) il parametro.</summary>
        public void Scrivi(string chiave, string valore)
        {
            using (var conn = _db.Apri())
            using (var cmd = new SQLiteCommand("INSERT INTO parametri(key, value) VALUES (@k, @v) ON CONFLICT(key) DO UPDATE SET value = excluded.value", conn))
            {
                cmd.Parameters.AddWithValue("@k", chiave);
                cmd.Parameters.AddWithValue("@v", valore ?? "");
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>Crea le chiavi mancanti con il valore predefinito, senza toccare quelle esistenti.</summary>
        public void AssicuraPredefiniti(IEnumerable<KeyValuePair<string, string>> predefiniti)
        {
            using (var conn = _db.Apri())
            using (var tx = conn.BeginTransaction())
            {
                foreach (var kv in predefiniti)
                    using (var cmd = new SQLiteCommand("INSERT OR IGNORE INTO parametri(key, value) VALUES (@k, @v)", conn))
                    {
                        cmd.Parameters.AddWithValue("@k", kv.Key);
                        cmd.Parameters.AddWithValue("@v", kv.Value ?? "");
                        cmd.ExecuteNonQuery();
                    }
                tx.Commit();
            }
        }

        public List<Parametro> Tutti()
        {
            var lista = new List<Parametro>();
            using (var conn = _db.Apri())
            using (var cmd = new SQLiteCommand("SELECT id_parametro, key, value FROM parametri ORDER BY key", conn))
            using (var r = cmd.ExecuteReader())
                while (r.Read())
                    lista.Add(new Parametro { Id = Convert.ToInt32(r["id_parametro"]), Chiave = Convert.ToString(r["key"]), Valore = Convert.ToString(r["value"]) });
            return lista;
        }
    }
}
