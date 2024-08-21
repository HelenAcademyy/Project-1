using System;
using Helen.Domain.Enum;

namespace Helen.Domain.Invites
{
	public class CustomerDetail
	{
        public string? Username { get; set; }
        public int Age { get; set; }
        public ProfileStatus Status { get; set; }
        public DateTime ReminderTime { get; set; }
        public bool SendViaMail { get; set; }
        public bool SendViaPhone { get; set; }
        public string? Email { get; set; }
        public decimal Budget { get; set; }
        public bool IsSmoker { get; set; }
        public string? PhoneNumber { get; set; }
        public string Location { get; set; }
        public ReminderFrequency ReminderFrequency { get; set; }
    }
}

