using System.Collections.Generic;
using PackageTracker.Models;

namespace PackageTracker.Services
{
    public class PackageService: IPackageService
    {
        private readonly IPackageRepository _packageRepository;

        public PackageService(IPackageRepository packageRepository)
        {
            _packageRepository = packageRepository;
        }

        public IEnumerable<Package> GetPackages(string packageTrackingNo)
        {
            return _packageRepository.GetPackages(packageTrackingNo);
        }

        public void StorePackage(DhlPayload dhlPayload, string trackingNo) 
        {
            var lastEvent = dhlPayload.Shipments[0].Events[0];
            var package = new Package {
                TrackingNo = trackingNo,
                Status = lastEvent.Status,
                DeliveryCompany = "DHL",
                Timestamp = lastEvent.Timestamp
            };
            _packageRepository.StorePackages(new List<Package>{package});
        }
    }
}