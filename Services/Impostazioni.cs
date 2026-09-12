using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Windows.Forms;
using MicriCancelli.Data;

namespace MicriCancelli.Services
{
    /// <summary>
    /// Parametri di funzionamento, letti dalla tabella "parametri" e tipizzati.
    /// L'istanza è condivisa: <see cref="Carica"/> aggiorna i valori sul posto e tutti i servizi li vedono.
    /// </summary>
    public sealed class Impostazioni
    {
        public const string ChiavePasswordHash = "passwordHash";

        public int LunghezzaCodice { get; private set; } = 6;
        public Keys TastoFine { get; private set; } = Keys.E;
        public int MinutiValidita { get; private set; } = 30;
        public int SecondiGrazia { get; private set; } = 60;
        public int GiorniConservazione { get; private set; } = 30;
        public string IpArduino { get; private set; } = "10.99.5.101";
        public int PortaArduino { get; private set; } = 80;
        public int InchingMs { get; private set; } = 1000;
        public int TimeoutArduinoMs { get; private set; } = 2000;
        public int SecondiRefresh { get; private set; } = 5;
        public string Stampante { get; private set; } = "";
        public string TipoCodice { get; private set; } = "QR";
        public string IntestazioneBiglietto { get; private set; } = "PARCHEGGIO";
        public string PiattaformaUrl { get; private set; } = "https://apia.micricalcio.it";
        public string PiattaformaChiave { get; private set; } = "";
        public string PasswordHash { get; private set; } = "";

        /// <summary>Descrizione di un parametro modificabile dal pannello.</summary>
        public sealed class Definizione
        {
            public string Chiave;
            public string Etichetta;
            public string Predefinito;
            public string Descrizione;
            public Func<string, string> Valida; // restituisce messaggio d'errore o null
            public string[] Scelte;             // se valorizzato, il pannello mostra una tendina
        }

        private static string Intero(string v, int min, int max)
        {
            int n;
            if (!int.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out n)) return "Inserire un numero intero";
            if (n < min || n > max) return "Valore ammesso tra " + min + " e " + max;
            return null;
        }

        public static readonly Definizione[] Definizioni =
        {
            new Definizione { Chiave = "KeyEnd", Etichetta = "Carattere di terminazione", Predefinito = "E",
                Descrizione = "Lettera che chiude il codice letto dallo scanner (contenuta nel QR/barcode). Anche Invio è accettato.",
                Valida = v => v != null && v.Trim().Length == 1 && char.IsLetter(v.Trim()[0]) ? null : "Inserire una sola lettera" },
            new Definizione { Chiave = "code_len", Etichetta = "Cifre del codice", Predefinito = "6",
                Descrizione = "Numero di cifre del codice stampato sul biglietto.", Valida = v => Intero(v, 4, 9) },
            new Definizione { Chiave = "minutiValidita", Etichetta = "Minuti di validità", Predefinito = "30",
                Descrizione = "Entro quanti minuti dall'emissione il biglietto può essere usato.", Valida = v => Intero(v, 1, 1440) },
            new Definizione { Chiave = "secondiGrazia", Etichetta = "Secondi di grazia dopo l'uso", Predefinito = "60",
                Descrizione = "Per quanti secondi una seconda lettura dello stesso biglietto riapre ancora il cancello.", Valida = v => Intero(v, 0, 600) },
            new Definizione { Chiave = "giorniConservazione", Etichetta = "Giorni di conservazione codici", Predefinito = "30",
                Descrizione = "I codici emessi da più di questi giorni vengono cancellati (all'avvio e ogni ora) e i numeri tornano disponibili. Un biglietto vale pochi minuti: 30 giorni bastano per le verifiche.",
                Valida = v => Intero(v, 1, 3650) },
            new Definizione { Chiave = "ipArduino", Etichetta = "Indirizzo IP Arduino", Predefinito = "10.99.5.101",
                Descrizione = "IP della scheda che comanda il cancello.",
                Valida = v => { IPAddress ip; return IPAddress.TryParse((v ?? "").Trim(), out ip) ? null : "Indirizzo IP non valido"; } },
            new Definizione { Chiave = "portaArduino", Etichetta = "Porta Arduino", Predefinito = "80",
                Descrizione = "Porta TCP della scheda.", Valida = v => Intero(v, 1, 65535) },
            new Definizione { Chiave = "inching", Etichetta = "Inching (ms)", Predefinito = "1000",
                Descrizione = "Millisecondi tra il comando 'apri' e il comando 'chiudi'.", Valida = v => Intero(v, 100, 60000) },
            new Definizione { Chiave = "timeoutArduino", Etichetta = "Timeout Arduino (ms)", Predefinito = "2000",
                Descrizione = "Attesa massima per connettersi alla scheda prima di segnalare errore.", Valida = v => Intero(v, 200, 30000) },
            new Definizione { Chiave = "secondiRefresh", Etichetta = "Secondi di refresh tabella", Predefinito = "5",
                Descrizione = "Ogni quanti secondi la tabella dei codici si aggiorna.", Valida = v => Intero(v, 1, 3600) },
            new Definizione { Chiave = "stampante", Etichetta = "Stampante biglietti", Predefinito = "",
                Descrizione = "Nome della stampante termica. Vuoto = stampante predefinita di Windows.", Valida = v => null },
            new Definizione { Chiave = "tipoCodice", Etichetta = "Tipo di codice stampato", Predefinito = "QR",
                Descrizione = "QR richiede uno scanner 2D; CODE39 funziona anche con i lettori laser 1D.",
                Scelte = new[] { "QR", "CODE39" }, Valida = v => v == "QR" || v == "CODE39" ? null : "Scegliere QR o CODE39" },
            new Definizione { Chiave = "intestazione", Etichetta = "Intestazione biglietto", Predefinito = "PARCHEGGIO",
                Descrizione = "Testo stampato sotto il logo.", Valida = v => (v ?? "").Trim().Length <= 30 ? null : "Massimo 30 caratteri" },
            new Definizione { Chiave = "piattaformaUrl", Etichetta = "Indirizzo piattaforma IA", Predefinito = "https://apia.micricalcio.it",
                Descrizione = "Indirizzo dell'API della piattaforma MICRI AI da cui arrivano i comandi di apertura remota.",
                Valida = v => Uri.TryCreate((v ?? "").Trim(), UriKind.Absolute, out var u) && (u.Scheme == "https" || u.Scheme == "http") ? null : "Inserire un indirizzo http(s) valido" },
            new Definizione { Chiave = "piattaformaChiave", Etichetta = "Chiave piattaforma", Predefinito = "",
                Descrizione = "Chiave di collegamento mostrata in Impostazioni > Varco parcheggio della piattaforma. Vuota = apertura remota disattivata.",
                Valida = v => null },
        };

        public static IEnumerable<KeyValuePair<string, string>> Predefiniti()
        {
            foreach (var d in Definizioni) yield return new KeyValuePair<string, string>(d.Chiave, d.Predefinito);
            yield return new KeyValuePair<string, string>(ChiavePasswordHash, Properties.Settings.Default.passwordParametriSha256);
        }

        public void Carica(ParametriRepository repo)
        {
            LunghezzaCodice = LeggiIntero(repo, "code_len", LunghezzaCodice);
            MinutiValidita = LeggiIntero(repo, "minutiValidita", MinutiValidita);
            SecondiGrazia = LeggiIntero(repo, "secondiGrazia", SecondiGrazia);
            GiorniConservazione = LeggiIntero(repo, "giorniConservazione", GiorniConservazione);
            PortaArduino = LeggiIntero(repo, "portaArduino", PortaArduino);
            InchingMs = LeggiIntero(repo, "inching", InchingMs);
            TimeoutArduinoMs = LeggiIntero(repo, "timeoutArduino", TimeoutArduinoMs);
            SecondiRefresh = LeggiIntero(repo, "secondiRefresh", SecondiRefresh);
            IpArduino = LeggiTesto(repo, "ipArduino", IpArduino);
            Stampante = LeggiTesto(repo, "stampante", "");
            TipoCodice = LeggiTesto(repo, "tipoCodice", "QR").ToUpperInvariant() == "CODE39" ? "CODE39" : "QR";
            IntestazioneBiglietto = LeggiTesto(repo, "intestazione", IntestazioneBiglietto);
            PiattaformaUrl = LeggiTesto(repo, "piattaformaUrl", PiattaformaUrl);
            PiattaformaChiave = LeggiTesto(repo, "piattaformaChiave", "");
            PasswordHash = LeggiTesto(repo, ChiavePasswordHash, "");

            var tasto = LeggiTesto(repo, "KeyEnd", "E").Trim();
            Keys k;
            if (tasto.Length == 1 && char.IsLetter(tasto[0]) && Enum.TryParse(tasto.ToUpperInvariant(), out k)) TastoFine = k;
            else Log.Avviso("Parametro KeyEnd non valido ('" + tasto + "'), uso " + TastoFine);
        }

        private static int LeggiIntero(ParametriRepository repo, string chiave, int fallback)
        {
            var v = repo.Leggi(chiave);
            int n;
            if (v != null && int.TryParse(v.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out n)) return n;
            if (v != null) Log.Avviso("Parametro " + chiave + " non numerico ('" + v + "'), uso " + fallback);
            return fallback;
        }

        private static string LeggiTesto(ParametriRepository repo, string chiave, string fallback)
        {
            var v = repo.Leggi(chiave);
            return v == null ? fallback : v.Trim();
        }
    }
}
