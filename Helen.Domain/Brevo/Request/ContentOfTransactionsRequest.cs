using System;
namespace Helen.Domain.Brevo.Request
{
	public class ContentOfTransactionsRequest
    {
        public string? Type { get; set; }
        public Properties? Properties { get; set; }
    }
    public class Body
        {
            public string? Type { get; set; }
        }

        public class Date
        {
            public string? Type { get; set; }
        }

        public class Email
        {
            public string? Type { get; set; }
        }

        public class Events
        {
            public string? Type { get; set; }
            public Items? Items { get; set; }
        }

        public class Items
        {
            public string? Type { get; set; }
            public Properties? Properties { get; set; }
        }

        public class Name
        {
            public string? Type { get; set; }
        }

        public class Properties
        {
            public Email? Email { get; set; }
            public Subject? Subject { get; set; }
            public TemplateId? TemplateId { get; set; }
            public Date? Date { get; set; }
            public Events? Events { get; set; }
            public Body? Body { get; set; }
            public Name? Name { get; set; }
            public Time? Time { get; set; }
        }

        public class Subject
        {
            public string? Type { get; set; }
        }

        public class TemplateId
        {
            public string? Type { get; set; }
        }

        public class Time
        {
            public string? Type { get; set; }
        }

    }

