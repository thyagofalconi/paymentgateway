using PaymentGateway.Domain.DataEncryption.Interfaces;
using System;
using System.Security.Cryptography;
using System.Text;

namespace PaymentGateway.Domain.DataEncryption
{
    public class DataEncryptor : IDataEncryptor
    {
        private readonly string _publicKey;
        private readonly string _privateKey;

        public DataEncryptor(string publicKey, string privateKey)
        {
            _publicKey = publicKey;
            _privateKey = privateKey;
        }

        public string Encrypt(string data)
        {
            
            var testData = Encoding.UTF8.GetBytes(data);

            using (var rsa = new RSACryptoServiceProvider(1024))
            {
                try
                {                 
                    rsa.FromXmlString(_publicKey);

                    var encryptedData = rsa.Encrypt(testData, true);

                    var base64Encrypted = Convert.ToBase64String(encryptedData);

                    return base64Encrypted;
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }

            }
        }

        public string Decrypt(string data)
        {
            using (var rsa = new RSACryptoServiceProvider(1024))
            {
                try
                {
                    var base64Encrypted = data;
                 
                    rsa.FromXmlString(_privateKey);

                    var resultBytes = Convert.FromBase64String(base64Encrypted);
                    var decryptedBytes = rsa.Decrypt(resultBytes, true);
                    var decryptedData = Encoding.UTF8.GetString(decryptedBytes);
                    return decryptedData.ToString();
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }
    }
}
