using System;
using Newtonsoft.Json;

namespace Helen.Repository.Models
{
	public class SmsData
	{
        public bool IsSuccessful { get; set; }
        public string? Message { get; set; }
        public int ResponseCode { get; set; }

         
        public string? Body { get; set; }

        public string? From { get; set; }

        public string? To { get; set; }

        public string? ApiToken { get; set; }

        public string? Gateway { get; set; }

        public string? CustomerReference { get; set; }

        public string? CallbackUrl { get; set; }
        public int Id { get; set; }
        public DateTime SentDate { get; set; }
        public string? Status { get; set; }
        public string? Message_id { get; set; }
        public double Cost { get; set; }
        public string? Currency { get; set; }
        public string? Gateway_used { get; set; }
    }
}
    


