using System;

namespace MicriCancelli.Models
{
    /// <summary>Biglietto emesso per il varco: un numero a N cifre stampato come QR (o Code39).</summary>
    public class Codice
    {
        public int Id { get; set; }
        public int Numero { get; set; }
        public DateTime EmessoIl { get; set; }
        public DateTime? UsatoIl { get; set; }
        public string Tipo { get; set; } = "QR";

        /// <summary>Numero formattato con zeri iniziali alla lunghezza richiesta.</summary>
        public string NumeroFormattato(int lunghezza) => Numero.ToString().PadLeft(lunghezza, '0');
    }
}
