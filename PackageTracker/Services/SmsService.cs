using System;
using PackageTracker.Models;
using Vonage;
using Vonage.Request;

namespace PackageTracker.Services
{
    public class SmsService : ISmsService
    {
        public void Send(Package package)
        {
            var credentials = Credentials.FromApiKeyAndSecret(Environment.GetEnvironmentVariable("Vonage-API-Key"),Environment.GetEnvironmentVariable("Vonage-API-Secret"));
            var vonageClient = new VonageClient(credentials);

            var response = vonageClient.SmsClient.SendAnSms(new Vonage.Messaging.SendSmsRequest()
            {
                To = Environment.GetEnvironmentVariable("Vonage-Phone-No"),
                From = "PkgTracker",
                Text = $"Das Paket {package.ProductDescription} hat nun folgenden Status:\n{package.Status}"
            });
        }
    }
}