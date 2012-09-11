using System.Threading.Tasks;

namespace SevenDigital.Api.Wrapper.Http
{
	public interface IHttpRequestor
	{
		Task<Response> GetDataAsync(RequestData requestData);
		string EndpointUrl(RequestData requestData);

		IHttpClientWrapper HttpClient { get; set; }
	}
}
