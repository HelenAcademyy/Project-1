using System;
namespace Helen.Domain.Paystack.Request
{
	public class InitializeTransactionRequest
    {
        public string? Email { get; set; }
        public string? Amount { get; set; }
    }
}

