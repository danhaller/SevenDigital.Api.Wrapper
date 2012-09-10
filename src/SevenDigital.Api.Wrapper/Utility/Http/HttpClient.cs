using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
using SevenDigital.Api.Wrapper.EndpointResolution;

namespace SevenDigital.Api.Wrapper.Utility.Http
{
	public class HttpClient : IHttpClient
	{
		public async Task<HttpResponseMessage> GetAsync(GetRequest request)
		{
			var httpClient = MakeHttpClient(request.Headers);
			return await httpClient.GetAsync(request.Url);
		}

		public async Task<HttpResponseMessage> PostAsync(PostRequest request)
		{
			var httpClient = MakeHttpClient(request.Headers);

			string postData = request.Parameters.ToQueryString();
			HttpContent content = new StringContent(postData);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

			return await httpClient.PostAsync(request.Url, content);
		}

		private System.Net.Http.HttpClient MakeHttpClient(IDictionary<string, string> headers)
		{
			var httpClient = new System.Net.Http.HttpClient();

			foreach (var header in headers)
			{
				httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
			}

			var productInfo = new ProductInfoHeaderValue("7digital .Net Api Wrapper", "4.5");
			httpClient.DefaultRequestHeaders.UserAgent.Add(productInfo);

			return httpClient;
		}
	}
}