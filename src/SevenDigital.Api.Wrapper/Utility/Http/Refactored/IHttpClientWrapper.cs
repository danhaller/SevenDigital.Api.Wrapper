using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SevenDigital.Api.Wrapper.Utility.Http.Refactored
{
	public interface IHttpClientWrapper
	{
		Task<Response> GetAsync(IDictionary<string, string> headers, string url);
		Task<Response> PostAsync(IDictionary<string, string> headers, IDictionary<string, string> postParams, string url);
	}
}
