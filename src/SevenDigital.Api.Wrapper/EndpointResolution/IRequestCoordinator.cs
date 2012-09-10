using System.Net.Http;
using System.Threading.Tasks;
using SevenDigital.Api.Wrapper.Utility.Http;

namespace SevenDigital.Api.Wrapper.EndpointResolution
{
	public interface IRequestCoordinator
	{
		Task<HttpResponseMessage> HitEndpointAsync(EndPointInfo endPointInfo);

		string ConstructEndpoint(EndPointInfo endPointInfo);
		IHttpClient HttpClient { get; set; }
	}
}