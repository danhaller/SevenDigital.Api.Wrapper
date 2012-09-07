using System.Threading.Tasks;

namespace SevenDigital.Api.Wrapper.Utility.Http.Refactored
{
	public interface IHttpRequestor
	{
		Task<Response> GetDataAsync(RequestData requestData);
	}
}
