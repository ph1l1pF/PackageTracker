using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using PackageTracker.Models;

namespace PackageTracker.Services
{
    public class PackageRepository : IPackageRepository
    {

        private readonly IMongoCollection<Package> _packages;
        private readonly ILogger<PackageRepository> _logger;

        public PackageRepository(IPackagesDatabaseSettings packagesDatabaseSettings, ILogger<PackageRepository> logger)
        {
            var client = new MongoClient(packagesDatabaseSettings.ConnectionString);
            var database = client.GetDatabase(packagesDatabaseSettings.DatabaseName);
            _packages = database.GetCollection<Package>(packagesDatabaseSettings.PackagesCollectionName);
            _logger = logger;
        }
        public IEnumerable<Package> GetPackages(string trackingNo)
        {
            try
            {
                return _packages.Find(s => s.TrackingNo == trackingNo).ToList();
            }
            catch (MongoException e)
            {
                _logger.LogError(e, e.Message);
                return new List<Package>();
            }
        }

        public IEnumerable<Package> GetAllPackages()
        {
            try
            {
                return _packages.Find(s => true).ToList();
            }
            catch (MongoException e)
            {
                _logger.LogError(e, e.Message);
                return new List<Package>();
            }
        }

        public void StorePackages(IEnumerable<Package> packages)
        {
            try
            {
                _packages.InsertMany(packages);
            }
            catch (MongoException e)
            {
                _logger.LogError(e, e.Message);
            }
        }
    }
}