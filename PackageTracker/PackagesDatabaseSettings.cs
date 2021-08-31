namespace PackageTracker 
{
    public class PackagesDatabaseSettings : IPackagesDatabaseSettings
    {
        public string PackagesCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IPackagesDatabaseSettings
    {
        string PackagesCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}