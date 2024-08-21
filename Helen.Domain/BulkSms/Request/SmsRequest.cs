using System;
namespace Helen.Domain.BulkSms.Request
{
	public class SmsRequest
    {
        public string? body { get; set; }
        public string? from { get; set; }
        public string? to { get; set; }
        public string? api_token { get; set; }
        public string? gateway { get; set; }
        public string? customerReference { get; set; }
        public string? callbackUrl { get; set; }
    }
}

