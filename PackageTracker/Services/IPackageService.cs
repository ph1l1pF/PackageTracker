using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PackageTracker.Models;

namespace PackageTracker.Services
{
    public interface IPackageService
    {
        Package StorePackage(string trackingNo, string productDescription);
        IEnumerable<Package> GetPackages(string packageTrackingNo);
    }
}