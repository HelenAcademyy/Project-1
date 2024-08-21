using System;
namespace Helen.Domain.Google.Response
{
	public class EmailRequest
    {
        public List<string>? To { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public DateTime SentDate { get; set; } = DateTime.Now;
    }

}

