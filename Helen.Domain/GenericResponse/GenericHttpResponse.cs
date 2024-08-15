using System;
namespace Helen.Domain
{
	public class GenericHttpResponse<T>
	{
		public string? ResponseCode { get; set; }
		public string? Content { get; set; }
		public T? ResponseObject { get; set; }
	}
}

