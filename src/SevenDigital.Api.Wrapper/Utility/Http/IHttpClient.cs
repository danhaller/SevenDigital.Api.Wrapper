using System.Net.Http;
using System.Threading.Tasks;

namespace SevenDigital.Api.Wrapper.Utility.Http
{
	public interface IHttpClient
	{
		Task<HttpResponseMessage> GetAsync(GetRequest request);
		Task<HttpResponseMessage> PostAsync(PostRequest request);
	}
}