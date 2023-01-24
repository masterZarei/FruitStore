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
            try
            {
                var api = new Kavenegar.KavenegarApi(_kavenegarInfo.ApiKey);
                var result = await api.Send(_kavenegarInfo.Sender, phoneNumber, message);

            }
            catch (Kavenegar.Core.Exceptions.ApiException ex)
            {
                // در صورتی که خروجی وب سرویس 200 نباشد این خطارخ می دهد.
                throw new Exception(ex.Message);
            }
            catch (Kavenegar.Core.Exceptions.HttpException ex)
            {
                // در زمانی که مشکلی در برقرای ارتباط با وب سرویس وجود داشته باشد این خطا رخ می دهد
                throw new Exception(ex.Message);
            }
        }
    }
}
