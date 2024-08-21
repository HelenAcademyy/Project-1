using System;
using Newtonsoft.Json;

namespace Helen.Domain.Brevo.Response
{
	public class CreateEmailCampaignResponse
    {
        [JsonProperty("event", NullValueHandling = NullValueHandling.Ignore)]
        public string? Event { get; set; }

        [JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
        public string? Email { get; set; }

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public int? Id { get; set; }

        [JsonProperty("date", NullValueHandling = NullValueHandling.Ignore)]
        public string? Date { get; set; }

        [JsonProperty("ts", NullValueHandling = NullValueHandling.Ignore)]
        public int? Ts { get; set; }

        [JsonProperty("message-id", NullValueHandling = NullValueHandling.Ignore)]
        public string? MessageId { get; set; }

        [JsonProperty("ts_event", NullValueHandling = NullValueHandling.Ignore)]
        public int? TsEvent { get; set; }

        [JsonProperty("subject", NullValueHandling = NullValueHandling.Ignore)]
        public string? Subject { get; set; }

        [JsonProperty("tag", NullValueHandling = NullValueHandling.Ignore)]
        public string? Tag { get; set; }

        [JsonProperty("sending_ip", NullValueHandling = NullValueHandling.Ignore)]
        public string? SendingIp { get; set; }

        [JsonProperty("ts_epoch", NullValueHandling = NullValueHandling.Ignore)]
        public long? TsEpoch { get; set; }

        [JsonProperty("tags", NullValueHandling = NullValueHandling.Ignore)]
        public List<string>? Tags { get; set; }
    }
    
}

