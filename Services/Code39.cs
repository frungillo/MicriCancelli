using System;
using System.Collections.Generic;
using System.Drawing;

namespace MicriCancelli.Services
{
    /// <summary>Disegno diretto di un codice a barre Code 39 (senza librerie), per i lettori laser 1D.</summary>
    public static class Code39
    {
        // 9 elementi per carattere, alternati barra/spazio a partire da una barra; n = stretto, w = largo
        private static readonly Dictionary<char, string> Pattern = new Dictionary<char, string>
        {
            {'0',"nnnwwnwnn"},{'1',"wnnwnnnnw"},{'2',"nnwwnnnnw"},{'3',"wnwwnnnnn"},{'4',"nnnwwnnnw"},
            {'5',"wnnwwnnnn"},{'6',"nnwwwnnnn"},{'7',"nnnwnnwnw"},{'8',"wnnwnnwnn"},{'9',"nnwwnnwnn"},
            {'A',"wnnnnwnnw"},{'B',"nnwnnwnnw"},{'C',"wnwnnwnnn"},{'D',"nnnnwwnnw"},{'E',"wnnnwwnnn"},
            {'F',"nnwnwwnnn"},{'G',"nnnnnwwnw"},{'H',"wnnnnwwnn"},{'I',"nnwnnwwnn"},{'J',"nnnnwwwnn"},
            {'K',"wnnnnnnww"},{'L',"nnwnnnnww"},{'M',"wnwnnnnwn"},{'N',"nnnnwnnww"},{'O',"wnnnwnnwn"},
            {'P',"nnwnwnnwn"},{'Q',"nnnnnnwww"},{'R',"wnnnnnwwn"},{'S',"nnwnnnwwn"},{'T',"nnnnwnwwn"},
            {'U',"wwnnnnnnw"},{'V',"nwwnnnnnw"},{'W',"wwwnnnnnn"},{'X',"nwnnwnnnw"},{'Y',"wwnnwnnnn"},
            {'Z',"nwwnwnnnn"},{'-',"nwnnnnwnw"},{'.',"wwnnnnwnn"},{' ',"nwwnnnwnn"},{'$',"nwnwnwnnn"},
            {'/',"nwnwnnnwn"},{'+',"nwnnnwnwn"},{'%',"nnnwnwnwn"},{'*',"nwnnwnwnn"}
        };

        /// <summary>Larghezza totale in moduli stretti del testo (con i due asterischi di inizio/fine).</summary>
        public static int Moduli(string testo)
        {
            var completo = "*" + testo.ToUpperInvariant() + "*";
            return completo.Length * (6 + 3 * 3) + (completo.Length - 1);
        }

        /// <summary>Disegna il codice nel rettangolo indicato (unità del Graphics), barre nere su fondo bianco.</summary>
        public static void Disegna(Graphics g, string testo, RectangleF area)
        {
            var completo = "*" + testo.ToUpperInvariant() + "*";
            float modulo = area.Width / Moduli(completo.Substring(1, completo.Length - 2));
            float x = area.X;
            using (var nero = new SolidBrush(Color.Black))
            {
                foreach (var c in completo)
                {
                    string p;
                    if (!Pattern.TryGetValue(c, out p)) throw new ArgumentException("Carattere non codificabile in Code39: " + c);
                    for (int i = 0; i < 9; i++)
                    {
                        float w = (p[i] == 'w' ? 3 : 1) * modulo;
                        if (i % 2 == 0) g.FillRectangle(nero, x, area.Y, w, area.Height);
                        x += w;
                    }
                    x += modulo; // spazio tra caratteri
                }
            }
        }
    }
}
