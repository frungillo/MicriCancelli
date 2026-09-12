using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using MicriCancelli.Models;

namespace MicriCancelli.Services
{
    /// <summary>Costruisce la tabella mostrata nelle griglie (frmMain e frmTabella) a partire dai codici.</summary>
    public static class TabellaCodici
    {
        public static DataTable Costruisci(IEnumerable<Codice> codici, Impostazioni imp)
        {
            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("CODICE", typeof(string));
            dt.Columns.Add("EMESSO IL", typeof(string));
            dt.Columns.Add("USATO IL", typeof(string));
            dt.Columns.Add("STATO", typeof(string));

            var adesso = DateTime.Now;
            foreach (var c in codici)
            {
                string stato;
                if (c.UsatoIl.HasValue) stato = "Usato";
                else if (adesso > c.EmessoIl.AddMinutes(imp.MinutiValidita)) stato = "Scaduto";
                else stato = "Valido fino alle " + c.EmessoIl.AddMinutes(imp.MinutiValidita).ToString("HH:mm");

                dt.Rows.Add(c.Id, c.NumeroFormattato(imp.LunghezzaCodice),
                    c.EmessoIl.ToString("dd/MM/yyyy HH:mm"),
                    c.UsatoIl.HasValue ? c.UsatoIl.Value.ToString("dd/MM/yyyy HH:mm:ss") : "",
                    stato);
            }
            return dt;
        }

        public static void ApplicaLayout(DataGridView griglia)
        {
            if (griglia.Columns.Count < 5) return;
            griglia.Columns[0].Visible = false;
            griglia.Columns[1].Width = 70;
            griglia.Columns[2].Width = 105;
            griglia.Columns[3].Width = 120;
            griglia.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            griglia.ClearSelection();
        }
    }
}
