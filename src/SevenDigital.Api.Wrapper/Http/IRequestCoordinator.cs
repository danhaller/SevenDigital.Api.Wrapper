using System.Threading.Tasks;

namespace SevenDigital.Api.Wrapper.Http
{
	public interface IRequestCoordinator
	{
		Task<Response> GetDataAsync(RequestData requestData);
		string EndpointUrl(RequestData requestData);

		IHttpClientWrapper HttpClient { get; set; }
	}
}
