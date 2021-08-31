using System;
using System.Collections.Generic;

namespace PackageTracker.Models
{
    public class DhlPayload {
        public List<Shipment> Shipments {get; set;}
    }

    public class Shipment {
        public List<Event> Events {get; set;}
    }

    public class Event {
        public DateTime Timestamp { get; set; }
        public string StatusCode { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
    }
    
}