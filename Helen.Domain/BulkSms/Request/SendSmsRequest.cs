using System;
using Newtonsoft.Json;

namespace Helen.Domain.BulkSms.Request
{
    public class SendSmsRequest
    {
        public string? Body { get; set; }
        public string? To { get; set; }
    }
}
