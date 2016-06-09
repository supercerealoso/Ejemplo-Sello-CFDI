using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sello
{
    class Program
    {
        public string Sellar(string keyFile, string pass, string cadena)
        {
            string path = "C:\\algun\\directorio";

            // Escribir archivo UTF8 de la cadena
            var tempCadena = path + "\\openssl\\cadena" + DateTime.Now.ToString("yyMMddhhmmss");
            System.IO.File.WriteAllText(tempCadena, cadena);

            // Digestion SHA1
            var tempSha = path + "\\openssl\\sha" + DateTime.Now.ToString("yyMMddhhmmss");
            var opensslPath = path + "\\openssl\\openssl.exe";
            Process process = new Process();
            process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            process.StartInfo.FileName = opensslPath;
            process.StartInfo.Arguments = "dgst -sha1 " + tempCadena;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.ErrorDialog = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();

            String codificado = "";
            codificado = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            String codificado2 = "";
            // Si quieren cambiar este ciclo por un string.IndexOf('='), adelante
            for (int i = 0; i < codificado.Length; i++)
            {
                if (codificado[i] == '=')
                {
                    codificado2 = codificado.Substring(i + 2);
                    break;
                }
            }
            System.IO.File.WriteAllText(tempSha, codificado2);

            // Crear .pem del .key
            var tempPem = path + "\\openssl\\pem" + DateTime.Now.ToString("yyMMddhhmmss");
            Process process2 = new Process();
            process2.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            process2.StartInfo.FileName = opensslPath;
            process2.StartInfo.Arguments = "pkcs8 -inform DER -in " + keyFile + " -passin pass:" + pass + " -out " + tempPem;
            process2.StartInfo.UseShellExecute = false;
            process2.StartInfo.ErrorDialog = false;
            process2.StartInfo.RedirectStandardOutput = true;
            process2.Start();
            process2.WaitForExit();

            // Generar sello
            Process process3 = new Process();
            process3.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            process3.StartInfo.FileName = opensslPath;
            process3.StartInfo.Arguments = "dgst -sha1 -sign " + tempPem + " " + tempCadena;
            process3.StartInfo.UseShellExecute = false;
            process3.StartInfo.ErrorDialog = false;
            process3.StartInfo.RedirectStandardOutput = true;
            process3.Start();

            // Codificar en Base64
            String selloTxt = process3.StandardOutput.ReadToEnd();
            String b64 = Convert.ToBase64String(Encoding.Default.GetBytes(selloTxt));
            process3.WaitForExit();

            // Borrar archivos temporales
            File.Delete(tempCadena);
            File.Delete(tempSha);
            File.Delete(tempPem);

            return b64;
        }

        static void Main(string[] args)
        {
        }
    }
}
