using System.Net;

namespace DoorsAccess.IntegrationTests;

public class HttpResponse<T>
{
    public T? Result { get; set; }
    public HttpStatusCode StatusCode { get; set; }
}