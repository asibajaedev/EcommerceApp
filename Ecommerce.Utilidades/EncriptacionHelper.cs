using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Reflection.Metadata;

namespace Ecommerce.Utilidades
{
    public static class EncriptacionHelper
    {
        public static string GenerarSalt()
        {
            Random random = new Random();
            var salt = "";
            for (int i = 0; i < 27; i++)
            {
                int numero = random.Next(65, 90);
                char letra = Convert.ToChar(numero);
                salt += letra;
            }

            return salt;
        }

        public static string EncriptarContraseña(string contraseña, string salt)
        {
            string contenido = contraseña + salt;
            SHA256 sha256 = SHA256.Create();
            byte[] salida = Encoding.UTF8.GetBytes(contenido);

            for (int i = 0; i < 255; i++)
            {
                salida = sha256.ComputeHash(salida);
            }

            sha256.Clear();

            StringBuilder sb = new StringBuilder();
            foreach (byte b in salida)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();            
        }
    }
}
