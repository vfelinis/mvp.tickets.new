using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace mvp.tickets.domain.Helpers
{
    public static class HashHelper
    {
        public static string GetSHA256Hash(string inputString)
        {
            using (SHA256 mySHA256 = SHA256.Create())
            {
                var bytes = Encoding.Unicode.GetBytes(inputString);
                byte[] data = mySHA256.ComputeHash(bytes);

                string hashValue = Convert.ToBase64String(data);
                return hashValue;
            }
        }
    }
}
