using System.Threading.Tasks;
using SevenDigital.Api.Wrapper.Http;

namespace SevenDigital.Api.Wrapper
{
	public interface IFluentApi<T>
	{
		IFluentApi<T> WithParameter(string key, string value);
		IFluentApi<T> ClearParameters();
		IFluentApi<T> ForUser(string token, string secret);
		IFluentApi<T> WithEndpoint(string endpoint);
        IFluentApi<T> UsingClient(IHttpClientWrapper httpClient);
		string EndpointUrl { get; }

		Task<T> PleaseAsync();
	}
}
