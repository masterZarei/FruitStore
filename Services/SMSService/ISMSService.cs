using System.Threading.Tasks;

namespace Services.SMSService
{
    public interface ISMSService
    {
        Task SendPublicSMS(string phoneNumber, string message);
    }
}
