using System;
namespace Helen.Domain.Brevo.Response
{
	public class ScheduledEmailResponse
    {
        public int Count { get; set; }
        public List<Batch>? Batches { get; set; }
    }

    public class Batch
        {
            public string? ScheduledAt { get; set; }
            public string? CreatedAt { get; set; }
            public string? Status { get; set; }
        }

        

    }

