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

        private readonly IMongoCollection<Package> _packageCollection;
        private readonly ILogger<PackageRepository> _logger;
        private readonly ISmsService _smsService;

        public PackageRepository(IPackagesDatabaseSettings packagesDatabaseSettings, ILogger<PackageRepository> logger, ISmsService smsService)
        {
            var client = new MongoClient(packagesDatabaseSettings.ConnectionString);
            var database = client.GetDatabase(packagesDatabaseSettings.DatabaseName);
            _packageCollection = database.GetCollection<Package>(packagesDatabaseSettings.PackagesCollectionName);
            _logger = logger;
            _smsService = smsService;
        }
        public IEnumerable<Package> GetPackages(string trackingNo)
        {
            try
            {
                return _packageCollection.Find(s => s.TrackingNo == trackingNo).ToList();
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
                return _packageCollection.Find(s => true).ToList();
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
                    existingPackages.AddRange(_packageCollection.Find(p => p.TrackingNo == package.TrackingNo).ToEnumerable());
                }

                foreach( var existingPackage in existingPackages)
                {
                    var updatedPackage = packages.FirstOrDefault(p => p.TrackingNo == existingPackage.TrackingNo);
                    UpdateField("Status", updatedPackage.Status, existingPackage.TrackingNo);
                    UpdateField("StatusCode", updatedPackage.StatusCode, existingPackage.TrackingNo);
                    UpdateField("Timestamp", updatedPackage.Timestamp, existingPackage.TrackingNo);

                    if(updatedPackage.StatusCode == "delivered" && existingPackage.StatusCode != updatedPackage.StatusCode)
                    {
                        _smsService.Send(updatedPackage);
                    }
                }

                var newPackages = packages.Where(p => !existingPackages.Select(ep => ep.TrackingNo).Contains(p.TrackingNo));
                if(newPackages.Any()) _packageCollection.InsertMany(newPackages);
            }
            catch (MongoException e)
            {
                _logger.LogError(e, e.Message);
            }
        }

        private void UpdateField(string field, object newValueForField, string trackingNo)
        {
            var filter = Builders<Package>.Filter.Eq("TrackingNo", trackingNo);
            var update = Builders<Package>.Update.Set(field, newValueForField);
            _packageCollection.UpdateOne(filter, update);
        }
    }
}