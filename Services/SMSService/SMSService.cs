using FS.Models.ViewModels;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Services.SMSService
{
    public class SMSService : ISMSService
    {
        private readonly KavenegarInfoVM _kavenegarInfo;
        public SMSService(IOptions<KavenegarInfoVM> kavenegarInfo)
        {
            _kavenegarInfo = kavenegarInfo.Value;
        }

        public async Task SendPublicSMS(string phoneNumber, string message)
        {
            var api = new Kavenegar.KavenegarApi(_kavenegarInfo.ApiKey);
            await api.Send(_kavenegarInfo.Sender, phoneNumber, message);
        }
    }
}
