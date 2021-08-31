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
                var existingPackages = new List<Package>();
                foreach(var package in packages)
                {
                    existingPackages.AddRange(_packages.Find(p => p.TrackingNo == package.TrackingNo).ToEnumerable());
                }

                foreach( var existingPackage in existingPackages)
                {
                    var filter = Builders<Package>.Filter.Eq("TrackingNo", existingPackage.TrackingNo);
                    var updatedPackage = packages.FirstOrDefault(p => p.TrackingNo == existingPackage.TrackingNo);
                    var update = Builders<Package>.Update.Set("Status", updatedPackage.Status);
                    // update = Builders<Package>.Update.AddToSet("Timestamp", updatedPackage.Timestamp);
                    _packages.UpdateOne(filter, update);
                }

                var newPackages = packages.Where(p => !existingPackages.Select(ep => ep.TrackingNo).Contains(p.TrackingNo));
                if(newPackages.Any()) _packages.InsertMany(newPackages);
            }
            catch (MongoException e)
            {
                _logger.LogError(e, e.Message);
            }
        }
    }
}