using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using Newtonsoft.Json;
using PackageTracker.Models;

namespace PackageTracker.Services
{
    public class PackageService : IPackageService
    {
        private readonly IPackageRepository _packageRepository;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ISmsService _smsService;
        private Timer _timer;



        public PackageService(IPackageRepository packageRepository, IHttpClientFactory clientFactory, ISmsService smsService)
        {
            _packageRepository = packageRepository;
            _clientFactory = clientFactory;
            _smsService = smsService;
        }

        public IEnumerable<Package> GetPackages(string packageTrackingNo)
        {
            return _packageRepository.GetPackages(packageTrackingNo);
        }

        public void StartUpdatingPackages()
        {
            _timer = new Timer(UpdateAllPackages, null, 0, 1000000);
        }

        private void UpdateAllPackages(Object o)
        {
            var packages = _packageRepository.GetAllPackages();
            foreach (var package in packages)
            {
                StorePackage(package.TrackingNo, package.ProductDescription);
            }
        }

        public Package StorePackage(string trackingNo, string productDescription)
        {
            var package = RequestPackage(trackingNo, productDescription);
            if (package != null)
            {
                _packageRepository.StorePackages(new List<Package> { package });
                return package;
            }
            return null;
        }

        private Package RequestPackage(string trackingNo, string productDescription)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api-eu.dhl.com/track/shipments?trackingNumber={trackingNo}");
            request.Headers.Add("DHL-API-Key", Environment.GetEnvironmentVariable("DHL-API-Key"));
            var client = _clientFactory.CreateClient();

            for (var i = 0; i < 10; i++)
            {
                var response = client.SendAsync(request);
                if (response.Result.IsSuccessStatusCode)
                {
                    var jsonString = response.Result.Content.ReadAsStringAsync().Result;
                    var dhlPayload = JsonConvert.DeserializeObject<DhlPayload>(jsonString);
                    var lastEvent = dhlPayload.Shipments[0].Events[0];
                    var package = new Package
                    {
                        TrackingNo = trackingNo,
                        Status = lastEvent.Status,
                        DeliveryCompany = "DHL",
                        Timestamp = lastEvent.Timestamp,
                        ProductDescription = productDescription
                    };
                    return package;
                }
            }

            return null;
        }
    }
}