public class GenericResponse<T>where T : class
{
    public bool IsSuccessful { get; set; }
    public string? Message { get; set; }
    public int ResponseCode { get; set; }
    public T? Data { get; set; }
}