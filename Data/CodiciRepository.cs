using System;
using System.Collections.Generic;
using System.Data.SQLite;
using MicriCancelli.Models;

namespace MicriCancelli.Data
{
    public sealed class CodiciRepository
    {
        private readonly Database _db;

        public CodiciRepository(Database db) { _db = db; }

        public Codice Trova(int numero)
        {
            using (var conn = _db.Apri())
            using (var cmd = new SQLiteCommand("SELECT id_codice, codice, emesso_il, usato_il, tipo FROM codici WHERE codice = @c", conn))
            {
                cmd.Parameters.AddWithValue("@c", numero);
                using (var r = cmd.ExecuteReader())
                    return r.Read() ? Mappa(r) : null;
            }
        }

        public bool Esiste(int numero) => Trova(numero) != null;

        public void Inserisci(Codice c)
        {
            using (var conn = _db.Apri())
            using (var cmd = new SQLiteCommand("INSERT INTO codici(codice, emesso_il, usato_il, tipo) VALUES (@c, @e, @u, @t); SELECT last_insert_rowid()", conn))
            {
                cmd.Parameters.AddWithValue("@c", c.Numero);
                cmd.Parameters.AddWithValue("@e", Database.FormattaData(c.EmessoIl));
                cmd.Parameters.AddWithValue("@u", c.UsatoIl.HasValue ? (object)Database.FormattaData(c.UsatoIl.Value) : DBNull.Value);
                cmd.Parameters.AddWithValue("@t", c.Tipo ?? "QR");
                c.Id = Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public void SegnaUsato(int id, DateTime quando)
        {
            using (var conn = _db.Apri())
            using (var cmd = new SQLiteCommand("UPDATE codici SET usato_il = @u WHERE id_codice = @id", conn))
            {
                cmd.Parameters.AddWithValue("@u", Database.FormattaData(quando));
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>Tutti i codici, dal più recente.</summary>
        public List<Codice> Tutti(int massimo = 0)
        {
            var sql = "SELECT id_codice, codice, emesso_il, usato_il, tipo FROM codici ORDER BY emesso_il DESC, id_codice DESC";
            if (massimo > 0) sql += " LIMIT " + massimo;
            var lista = new List<Codice>();
            using (var conn = _db.Apri())
            using (var cmd = new SQLiteCommand(sql, conn))
            using (var r = cmd.ExecuteReader())
                while (r.Read()) lista.Add(Mappa(r));
            return lista;
        }

        private static Codice Mappa(SQLiteDataReader r) => new Codice
        {
            Id = Convert.ToInt32(r["id_codice"]),
            Numero = Convert.ToInt32(r["codice"]),
            EmessoIl = Database.LeggiData(r["emesso_il"]),
            UsatoIl = Database.LeggiDataOpzionale(r["usato_il"]),
            Tipo = Convert.ToString(r["tipo"])
        };
    }
}
