using System;
namespace Helen.Domain.Brevo.Request
{
    public class SendEmailRequest
    {
        public Sender? Sender { get; set; }
        public List<To>? To { get; set; }
        public List<Bcc>? Bcc { get; set; }
        public List<Cc>? Cc { get; set; }
        public string? HtmlContent { get; set; }
        public string? Subject { get; set; }
        public ReplyTo? ReplyTo { get; set; }
        public List<string>? Tags { get; set; }
    }

    public class Bcc
    {
        public string? Email { get; set; }
        public string? Name { get; set; }
    }

    public class Cc
    {
        public string? Email { get; set; }
        public string? Name { get; set; }
    }

    public class ReplyTo
    {
        public string? Email { get; set; }
        public string? Name { get; set; }
    }

    public class Sender
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
    }

    public class To
    {
        public string? Email { get; set; }
        public string? Name { get; set; }
    }

}

