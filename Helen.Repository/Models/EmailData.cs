using System;
namespace Helen.Repository.Models
{
	public class EmailData
    {
        public int Id { get; set; } 

        public string? To { get; set; } 
        public string? Subject { get; set; } 
        public string? Body { get; set; } 

        public DateTime SentDate { get; set; } 

        public string? ResponseMessage { get; set; } 
        public bool IsSuccessful { get; set; } 

        public string? Message { get; set; } 
    }
}

