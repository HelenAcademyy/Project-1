using System;
namespace Helen.Domain.Brevo.Response
{
	public class ListOfTransactionalEmailRequest
    {
        public int? Count { get; set; }
        public List<TransactionalEmail>? TransactionalEmails { get; set; }
    }

    public class TransactionalEmail
    {
        public string? Email { get; set; }
        public string? Subject { get; set; }
        public int? TemplateId { get; set; }
        public string? MessageId { get; set; }
        public string? Uuid { get; set; }
        public DateTime? Date { get; set; }
    }
}

