using System;
namespace Helen.Domain.Paystack.Response
{
    public class PaystackInitializeTransactionResponse
    {
        public bool Status { get; set; }
        public string? Message { get; set; }
        public TransactionData? Data { get; set; }

        public class TransactionData
        {
            public string? AuthorizationUrl { get; set; }
            public string? AccessCode { get; set; }
            public string? Reference { get; set; }
        }
    }
}

