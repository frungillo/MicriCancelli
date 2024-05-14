using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using MicriCancelli.Properties;

namespace MicriCancelli.Classi
{
    public static class Commons
    {

        public static string giornoSettimana(int g)
        {
            string[] giorni = { "", "Lunedì", "Martedì", "Mercoledì", "Giovedì", "Venerdì", "Sabato", "Domenica" };

            return giorni[g];

        }
        public static string RandomDigits(int length)
        {
            var random = new Random();
            string s = string.Empty;
            for (int i = 0; i < length; i++)
                s = String.Concat(s, random.Next(10).ToString());
            return s;
        }
        public static Image resizeImage(Image imgToResize, Size size)
        {
            return (Image)(new Bitmap(imgToResize, size));
        }
        public static Image ConvertFromByteArray(byte[] byteArr)
        {
            if (byteArr == null) byteArr = new byte[0];
            if (byteArr?.Length == 0)
            {
                var base64 = "";
                byteArr = Convert.FromBase64String(base64);
            }
            using (var memory = new MemoryStream(byteArr, 0, byteArr.Length))
            {
                try { return Image.FromStream(memory, true); } catch (Exception ex) { return null; }

            }
        }
        public static void CheckSoloNumeri(TextBox tx, bool soloInteri = false)
        {

            if (soloInteri)
            {
                if (!int.TryParse(tx.Text, out var t1) && tx.Text.Length > 0)
                {
                    tx.Text = tx.Text.Remove(tx.Text.Length - 1, 1);
                    tx.SelectionStart = tx.Text.Length;
                    tx.SelectionLength = 0;
                }
                return;
            }
            if (tx.Text.Contains("."))
            {
                tx.Text = tx.Text.Replace(".", ",");
                tx.SelectionStart = tx.Text.Length;
                tx.SelectionLength = 0;
            }
            if (!float.TryParse(tx.Text, out var t) && tx.Text.Length > 0)
            {
                tx.Text = tx.Text.Remove(tx.Text.Length - 1, 1);
                tx.SelectionStart = tx.Text.Length;
                tx.SelectionLength = 0;
            }
        }
        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection props =
            TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                if (prop.PropertyType.AssemblyQualifiedName.Contains("Nullable"))
                {
                    table.Columns.Add(prop.Name, typeof(string));
                }
                else
                {
                    table.Columns.Add(prop.Name, prop.PropertyType);
                }
            }
            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }
        public static void FillDropDownList(DataTable dt, System.Windows.Forms.ComboBox DropDownName, string value, string display)
        {
            //DropDownName.Items.Clear();
            DropDownName.DataSource = dt;
            DropDownName.ValueMember = value;
            DropDownName.DisplayMember = display;

        }
        public static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            // top left arc  
            path.AddArc(arc, 180, 90);

            // top right arc  
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // bottom right arc  
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // bottom left arc 
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }
        public static bool IsMail(string input)
        {
            if (input.Length == 0) return false;
            if (!RegexUtilities.IsValidEmail(input)) return false;
            else return true;
        }
        public static bool IsValidCell(string input)
        {
            return RegexUtilities.IsValidCell(input);
        }
        public static bool IsValidCF(string input)
        {
            return RegexUtilities.IsValidCF_2(input);
        }

        /// <summary>
        /// Apre il cancello 
        /// </summary>
        public static void apri()
        {
            // COMANDA L'APERTURA MANUALE 
            Socket soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Parametri par=new Parametri();
            par = Parametri.GetParametro("ipArduino");
            System.Net.IPAddress ipAdd = System.Net.IPAddress.Parse(par.Value);
            par = Parametri.GetParametro("portaArduino");
            System.Net.IPEndPoint remoteEP = new IPEndPoint(ipAdd, Convert.ToInt32(par.Value));
            soc.Connect(remoteEP);
            byte[] byData = System.Text.Encoding.ASCII.GetBytes("apri*");
            soc.Send(byData);
            soc.Close();
        }

        public static void chiudi() {
            // COMANDA LA CHIUSURA MANUALE 
            Socket soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Parametri par = new Parametri();
            par = Parametri.GetParametro("ipArduino");
            System.Net.IPAddress ipAdd = System.Net.IPAddress.Parse(par.Value);
            par = Parametri.GetParametro("portaArduino");
            System.Net.IPEndPoint remoteEP = new IPEndPoint(ipAdd, Convert.ToInt32(par.Value));
            soc.Connect(remoteEP);
            byte[] byData = System.Text.Encoding.ASCII.GetBytes("chiudi*");
            soc.Send(byData);
            soc.Close();
        }


        public static async Task ApreChiude(int inching) {
            await Task.Run(async () =>
            {
                apri();
                await Task.Delay(inching);
                chiudi();
            });
        }
       

    }
}
