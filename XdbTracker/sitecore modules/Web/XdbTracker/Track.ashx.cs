using ClientTracker.Models;
using Newtonsoft.Json;
using Sitecore.Analytics;
using Sitecore.Analytics.Data;
using Sitecore.Marketing.Definitions.Events;
using Sitecore.Marketing.Definitions.Outcomes.Model;
using System;
using System.IO;
using System.Web;

namespace XdbTracker.sitecore_modules.Web.XdbTracker
{
    public class Track : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            Init();

            string json = new StreamReader(context.Request.InputStream).ReadToEnd();
            Trigger(JsonConvert.DeserializeObject<TrackingEventData>(json), context);
        }

        private void Init()
        {
            if (!Tracker.IsActive)
            {
                Tracker.StartTracking();
            }

            // Don't track this API call as page view
            Tracker.Current?.Interaction?.CurrentPage?.Cancel();
            Tracker.Current?.Session?.Interaction?.AcceptModifications();
        }

        private void Trigger(TrackingEventData data, HttpContext context)
        {
            if (data == null)
            {
                BadRequest("Please provide valid data.", context);
                return;
            }
            
            if (Guid.TryParse(data.Id, out var id))
            {
                try
                {
                    IEventDefinition evt;

                    switch(data.Type)
                    {
                        case "outcome":
                            TriggerOutcome(Tracker.MarketingDefinitions.Outcomes[id], data, context);
                            return;
                        default:
                            evt = Tracker.MarketingDefinitions.PageEvents[id];
                            break;
                    }

                    if (evt == null)
                    {
                        BadRequest("Please provide a valid EventId", context);
                        return;
                    }

                    PageEventData pageData =
                        new PageEventData(evt.Alias, evt.Id)
                        {
                            Data = data?.Data,
                            Text = data?.Text,
                        };

                    if (Guid.TryParse(data.ItemId, out Guid itemId))
                    {
                        pageData.ItemId = itemId;
                    }
                    
                    Tracker.Current.CurrentPage.Register(pageData);
                }
                catch (Exception ex)
                {
                    BadRequest($"An error occurred: {ex.Message}", context);
                    return;
                }
            }
            else
            {
                BadRequest("Please provide a valid EventId", context);
                return;
            }
        }

        private void TriggerOutcome(IOutcomeDefinition outcomeDefinition, TrackingEventData data, HttpContext context)
        {
            if (outcomeDefinition == null)
            {
                BadRequest("Please provide a valid outcome ID", context);
                return;
            }

            if (data.OutcomeCurrency == null)
            {
                BadRequest("Please provide a currency", context);
                return;
            }

            decimal value = 0.0m;
            decimal.TryParse(data.OutcomeValue, out value);

            if (data.OutcomeType == "interaction")
            {
                Tracker.Current.Interaction.RegisterOutcome(outcomeDefinition, data.OutcomeCurrency, value);
                return;
            }

            Tracker.Current.CurrentPage.RegisterOutcome(outcomeDefinition, data.OutcomeCurrency, value);
        }

        private void BadRequest(string message, HttpContext context)
        {
            context.Response.ContentType = "text/json";
            context.Response.Write($"{{msg:\"{message}}}\"");
            context.Response.StatusCode = 400;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}