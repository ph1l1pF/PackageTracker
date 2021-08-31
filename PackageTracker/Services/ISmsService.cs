using PackageTracker.Models;

namespace PackageTracker.Services
{
    public interface ISmsService
    {
         void Send(Package package);
    }
}