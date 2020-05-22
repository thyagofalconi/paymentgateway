using System.Threading.Tasks;

namespace PaymentGateway.Domain.DataEncryption.Interfaces
{
    public interface IDataEncryptor
    {
        string Encrypt(string data);

        string Decrypt(string data);
    }
}
