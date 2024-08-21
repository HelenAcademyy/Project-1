using System;
namespace Helen.Domain.Invites.Request
{
	public class CustomerDetailsRequest
    { 
        public CustomerDetail? Customer { get; set; }
        public List<CustomerLocation>? MatchingLocations { get; set; }
    }

}

