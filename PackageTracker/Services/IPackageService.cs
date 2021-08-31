using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PackageTracker.Models;

namespace PackageTracker.Services
{
    public interface IPackageService
    {
        void StorePackage(DhlPayload dhlPayload, string trackingNo);
        IEnumerable<Package> GetPackages(string packageTrackingNo);
    }
}