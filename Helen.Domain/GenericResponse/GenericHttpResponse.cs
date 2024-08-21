using System;
namespace Helen.Domain
{
	public class GenericHttpResponse<T>
	{
		public int? ResponseCode { get; set; }
		public bool? IsSuccessful { get; set; }
		public string? Content { get; set; }
		public T? ResponseObject { get; set; }
	}
}

