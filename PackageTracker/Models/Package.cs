using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PackageTracker.Models
{
    public class Package
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string TrackingNo { get; set; }
        public string Status { get; set;}
        public DateTime Timestamp {get; set;}
        public string DeliveryCompany { get; set; }
    }
}