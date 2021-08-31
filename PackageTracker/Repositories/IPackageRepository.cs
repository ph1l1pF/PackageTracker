using System.Collections.Generic;
using PackageTracker.Models;

namespace PackageTracker.Services
{
    public interface IPackageRepository
    {
        IEnumerable<Package> GetPackages(string trackingNo);

        void StorePackages(IEnumerable<Package> packages);
    }
}