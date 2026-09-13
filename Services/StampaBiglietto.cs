using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Drawing.Text;
using System.Linq;
using MicriCancelli.Models;
using QRCoder;

namespace MicriCancelli.Services
{
    /// <summary>
    /// Stampa il biglietto su termica 80 mm con GDI+ (PrintDocument), senza Crystal Reports.
    /// Il taglio carta lo esegue il driver della stampante a fine documento (impostazione "taglio a fine documento").
    /// </summary>
    public sealed class StampaBiglietto
    {
        public const float LarghezzaCartaMm = 80f;
        public const float LarghezzaUtileMm = 72f;   // area stampabile tipica delle termiche 80 mm (576 punti a 203 dpi)
        public const float AltezzaCartaMm = 110f;

        private readonly Impostazioni _imp;

        public StampaBiglietto(Impostazioni imp) { _imp = imp; }

        /// <summary>Contenuto codificato nel QR/barcode: cifre + lettera di terminazione, così lo scanner "digita" esattamente ciò che l'hook aspetta.</summary>
        public string ContenutoCodice(Codice c) => c.NumeroFormattato(_imp.LunghezzaCodice) + _imp.TastoFine.ToString();

        public void Stampa(Codice c)
        {
            var doc = new PrintDocument();
            if (!string.IsNullOrWhiteSpace(_imp.Stampante)) doc.PrinterSettings.PrinterName = _imp.Stampante;
            if (!doc.PrinterSettings.IsValid)
                throw new InvalidOperationException("Stampante non valida o non installata: '" + doc.PrinterSettings.PrinterName + "'. " +
                                                    "Disponibili: " + string.Join(", ", PrinterSettings.InstalledPrinters.Cast<string>()));

            doc.DocumentName = "Biglietto " + c.NumeroFormattato(_imp.LunghezzaCodice);
            doc.PrintController = new StandardPrintController(); // nessuna finestra di avanzamento
            doc.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
            doc.DefaultPageSettings.PaperSize = new PaperSize("Biglietto 80mm", MmACentesimi(LarghezzaCartaMm), MmACentesimi(AltezzaCartaMm));
            doc.PrintPage += (s, e) =>
            {
                e.Graphics.PageUnit = GraphicsUnit.Millimeter;
                // larghezza reale disponibile secondo il driver, così il layout si adatta a stampanti con margini diversi
                float larghezza = Math.Min(LarghezzaCartaMm, e.PageSettings.PrintableArea.Width / 100f * 25.4f);
                Disegna(e.Graphics, c, larghezza);
                e.HasMorePages = false;
            };
            doc.Print();
            Log.Info("Biglietto " + c.NumeroFormattato(_imp.LunghezzaCodice) + " inviato a '" + doc.PrinterSettings.PrinterName + "'");
        }

        /// <summary>Anteprima raster del biglietto (dpi tipici di una termica), utile per test e verifica layout.</summary>
        public Bitmap Anteprima(Codice c, int dpi = 203)
        {
            var bmp = new Bitmap((int)Math.Round(LarghezzaCartaMm / 25.4 * dpi), (int)Math.Round(AltezzaCartaMm / 25.4 * dpi));
            bmp.SetResolution(dpi, dpi);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                g.PageUnit = GraphicsUnit.Millimeter;
                Disegna(g, c, LarghezzaCartaMm);
            }
            return bmp;
        }

        /// <summary>Disegna il biglietto in millimetri partendo da (0,0); restituisce l'altezza usata.</summary>
        public float Disegna(Graphics g, Codice c, float larghezzaCartaMm)
        {
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            float margine = (larghezzaCartaMm - LarghezzaUtileMm) / 2f;
            float utile = LarghezzaUtileMm;
            float centro = larghezzaCartaMm / 2f;
            float y = 3f;

            var numero = c.NumeroFormattato(_imp.LunghezzaCodice);
            var alCentro = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };

            // Logo in alto
            using (var logo = Properties.Resources.logo_micri)
            {
                float larghezzaLogo = 36f;
                float altezzaLogo = larghezzaLogo * logo.Height / logo.Width;
                g.DrawImage(logo, new RectangleF(centro - larghezzaLogo / 2f, y, larghezzaLogo, altezzaLogo));
                y += altezzaLogo + 1f;
            }

            // Intestazione
            using (var f = new Font("Arial", 11f, FontStyle.Bold))
            {
                g.DrawString(_imp.IntestazioneBiglietto, f, Brushes.Black, new RectangleF(margine, y, utile, 6f), alCentro);
                y += 6f;
            }
            using (var penna = new Pen(Color.Black, 0.4f))
                g.DrawLine(penna, margine + 6f, y, margine + utile - 6f, y);
            y += 2f;

            // Codice QR o barcode
            var contenuto = ContenutoCodice(c);
            if (string.Equals(c.Tipo, "CODE39", StringComparison.OrdinalIgnoreCase))
            {
                float altezza = 13f;
                Code39.Disegna(g, contenuto, new RectangleF(margine + 4f, y, utile - 8f, altezza));
                y += altezza + 2f;
            }
            else
            {
                float lato = 30f;
                using (var generatore = new QRCodeGenerator())
                using (var dati = generatore.CreateQrCode(contenuto, QRCodeGenerator.ECCLevel.M))
                using (var qr = new QRCode(dati))
                using (var immagine = qr.GetGraphic(10, Color.Black, Color.White, true))
                {
                    var interp = g.InterpolationMode;
                    g.InterpolationMode = InterpolationMode.NearestNeighbor; // moduli netti, niente sfumature
                    g.PixelOffsetMode = PixelOffsetMode.Half;
                    g.DrawImage(immagine, new RectangleF(centro - lato / 2f, y, lato, lato));
                    g.InterpolationMode = interp;
                }
                y += lato + 0.5f;
            }

            // Numero in chiaro, grande e spaziato
            using (var f = new Font("Consolas", 19f, FontStyle.Bold))
            {
                var spaziato = string.Join(" ", numero.ToCharArray());
                g.DrawString(spaziato, f, Brushes.Black, new RectangleF(margine, y, utile, 10f), alCentro);
                y += 10f;
            }

            // Dati di emissione e validità
            using (var f = new Font("Arial", 7.5f))
            using (var fPiccolo = new Font("Arial", 6.5f))
            {
                g.DrawString("Emesso il " + c.EmessoIl.ToString("dd/MM/yyyy") + " alle " + c.EmessoIl.ToString("HH:mm"),
                    f, Brushes.Black, new RectangleF(margine, y, utile, 4.5f), alCentro);
                y += 4.5f;
                g.DrawString("Valido per l'ingresso entro le " + c.EmessoIl.AddMinutes(_imp.MinutiValidita).ToString("HH:mm") +
                             " (" + _imp.MinutiValidita + " minuti)", f, Brushes.Black, new RectangleF(margine, y, utile, 4.5f), alCentro);
                y += 5.5f;
                g.DrawString("Avvicinare il codice al lettore del varco.\nIl biglietto vale per un solo passaggio.",
                    fPiccolo, Brushes.Black, new RectangleF(margine, y, utile, 7f), alCentro);
                y += 7.5f;
            }

            using (var penna = new Pen(Color.Black, 0.3f) { DashStyle = DashStyle.Dash })
                g.DrawLine(penna, margine, y, margine + utile, y);
            y += 1.5f;
            using (var f = new Font("Arial", 6f, FontStyle.Italic))
            {
                g.DrawString("Scuola Calcio MICRI  -  #MICRI", f, Brushes.Black, new RectangleF(margine, y, utile, 3.5f), alCentro);
                y += 4f;
            }
            return y;
        }

        private static int MmACentesimi(float mm) => (int)Math.Round(mm / 25.4f * 100f);
    }
}
