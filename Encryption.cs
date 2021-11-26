using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace uchet
{
    class Encryption
    {
        public byte[] GetHashPassword(string pass)
        {
            byte[] tmpHash;
            tmpHash = ASCIIEncoding.ASCII.GetBytes(pass);

            tmpHash = new MD5CryptoServiceProvider().ComputeHash(tmpHash);
            return tmpHash;
        }

        public bool CheckPassword(byte[] one, byte[] two)
        {
            bool bEqual = false;
            if (two.Length == one.Length)
            {
                int i = 0;
                while ((i < two.Length) && (two[i] == one[i]))
                {
                    i += 1;
                }
                if (i == two.Length)
                {
                    bEqual = true;
                }
            }

            if (bEqual)
                return true;
            else
                return false;
        }
    }
}
