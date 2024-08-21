using System;
namespace Helen.Domain.Brevo.Response
{
	public class ContentOfTransactionsResponse
    {
        public string? Email { get; set; }
        public string? Subject { get; set; }
        public int TemplateId { get; set; }
        public DateTime? Date { get; set; }
        public List<Event>? Events { get; set; }
        public string? Body { get; set; }
    }
    public class Event
    {
        public string? Name { get; set; }
        public DateTime? Time { get; set; }
    }

}

