using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClientTracker.Models
{
    public class TrackingEventData
    {
        public string Id { get; set; }      // Needs to be a valid guid
        public string Type { get; set; }    // "goal" or "outcome". Empty = pageEvent
        public string Text { get; set; }
        public string Data { get; set; }
        public string ItemId { get; set; }  // Needs to be a valid guid
        public string OutcomeType { get; set; }        // "interaction". Empty = "page"
        public string OutcomeCurrency { get; set; }    // Currency of outcome
        public string OutcomeValue { get; set; }       // Monetary value of outcome. 0.0m if left empty.
    }
}