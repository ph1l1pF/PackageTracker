using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using PackageTracker.Models;

namespace PackageTracker.Services
{
    public class PackageService: IPackageService
    {
        private readonly IPackageRepository _packageRepository;
        private readonly IHttpClientFactory _clientFactory;


        public PackageService(IPackageRepository packageRepository, IHttpClientFactory clientFactory)
        {
            _packageRepository = packageRepository;
            _clientFactory = clientFactory;
        }

        public IEnumerable<Package> GetPackages(string packageTrackingNo)
        {
            return _packageRepository.GetPackages(packageTrackingNo);
        }

        public Package StorePackage(string trackingNo, string productDescription) 
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api-eu.dhl.com/track/shipments?trackingNumber={trackingNo}");
            request.Headers.Add("DHL-API-Key", Environment.GetEnvironmentVariable("DHL-API-Key"));
            var client = _clientFactory.CreateClient();
            var response = client.SendAsync(request);
            if (response.Result.IsSuccessStatusCode)
            {
                var jsonString = response.Result.Content.ReadAsStringAsync().Result;
                var dhlPayload = JsonConvert.DeserializeObject<DhlPayload>(jsonString);
                var lastEvent = dhlPayload.Shipments[0].Events[0];
                var package = new Package {
                    TrackingNo = trackingNo,
                    Status = lastEvent.Status,
                    DeliveryCompany = "DHL",
                    Timestamp = lastEvent.Timestamp,
                    ProductDescription = productDescription
                };
                _packageRepository.StorePackages(new List<Package>{package});                
                return package; 
            }
            return null;
        }
    }
}