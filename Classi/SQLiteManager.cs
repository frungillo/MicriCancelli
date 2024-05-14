
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace MicriCancelli.Classi
{
    public class SQLiteManager
    {
        private string connectionString;

        public SQLiteManager(string dbFilePath)
        {
            connectionString = $"Data Source={dbFilePath}";
        }

        public void InsertCodice(int codice, string dataEmissione, string oraEmissione, string dataUso = null, string oraUso = null)
        {
            string query = @"INSERT INTO codici (codice, data_emissione, ora_emissione, data_uso, ora_uso)
                         VALUES (@codice, @dataEmissione, @oraEmissione, @dataUso, @oraUso)";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@codice", codice);
                command.Parameters.AddWithValue("@dataEmissione", dataEmissione);
                command.Parameters.AddWithValue("@oraEmissione", oraEmissione);
                command.Parameters.AddWithValue("@dataUso", dataUso);
                command.Parameters.AddWithValue("@oraUso", oraUso);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void ReadAllCodici()
        {
            string query = "SELECT * FROM codici";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                connection.Open();
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int idCodice = Convert.ToInt32(reader["id_codice"]);
                        int codice = Convert.ToInt32(reader["codice"]);
                        string dataEmissione = reader["data_emissione"].ToString();
                        string oraEmissione = reader["ora_emissione"].ToString();
                        string dataUso = reader["data_uso"].ToString();
                        string oraUso = reader["ora_uso"].ToString();

                        Console.WriteLine($"ID: {idCodice}, Codice: {codice}, Data Emissione: {dataEmissione}, Ora Emissione: {oraEmissione}, Data Uso: {dataUso}, Ora Uso: {oraUso}");
                    }
                }
            }
        }

        public void UpdateCodice(int idCodice, int codice, string dataEmissione, string oraEmissione, string dataUso = null, string oraUso = null)
        {
            string query = @"UPDATE codici 
                         SET codice = @newCodice, 
                             data_emissione = @dataEmissione, 
                             ora_emissione = @oraEmissione, 
                             data_uso = @dataUso, 
                             ora_uso = @oraUso 
                         WHERE id_codice = @idCodice";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@newCodice", codice);
                command.Parameters.AddWithValue("@dataEmissione", dataEmissione);
                command.Parameters.AddWithValue("@oraEmissione", oraEmissione);
                command.Parameters.AddWithValue("@idCodice", idCodice);
                command.Parameters.AddWithValue("@dataUso", dataUso);
                command.Parameters.AddWithValue("@oraUso", oraUso);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public Codici ReadCodice(int codice)
        {
            Codici cod=new Codici();
            string query = "SELECT * FROM codici WHERE codice = @codice";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@codice", codice);
                connection.Open();
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    reader.Read();
                    if (reader.StepCount==1) 
                    {
                        cod.Id_codice = Convert.ToInt32(reader["id_codice"]);
                        cod.Codice= Convert.ToInt32(reader["codice"]);
                        cod.Data_emissione = reader["data_emissione"].ToString();
                        cod.Ora_emissione= reader["ora_emissione"].ToString();
                        cod.Data_uso = reader["data_uso"].ToString();
                        cod.Ora_uso = reader["ora_uso"].ToString();
                    }
                    return cod;
                    
                }
            }
        }
        public Parametri GetParametro(string key)
        {
            Parametri par = new Parametri();
            string query = "SELECT id_parametro, key, value FROM parametri WHERE key = @key";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            
            {
                command.Parameters.AddWithValue("@key", key);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    reader.Read();
                    if (reader.StepCount == 1)
                    {
                        par.Id_parametro = Convert.ToInt32(reader["id_parametro"]);
                        par.Key = reader["key"].ToString();
                        par.Value = reader["value"].ToString();
                        
                    }
                    reader.Close();
                    return par;
                }
            }
        }
        public void UpdateParametro(int parametroId, string key, string value)
        {
            string query = "UPDATE parametri SET key = @key, value = @value WHERE id_parametro = @parametroId";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                
                command.Parameters.AddWithValue("@key", key);
                command.Parameters.AddWithValue("@value", value);
                command.Parameters.AddWithValue("@parametroId", parametroId);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}
