using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using SevenDigital.Api.Wrapper.EndpointResolution;

namespace SevenDigital.Api.Wrapper.Utility.Http.Refactored
{
	public class HttpClientWrapper : IHttpClientWrapper
	{
		public async Task<Response> GetAsync(IDictionary<string, string> headers, string url)
		{
			var httpClient = MakeHttpClient(headers);
			var httpResponseMessage = await httpClient.GetAsync(url);
			return await MakeResponse(httpResponseMessage);
		}

		public async Task<Response> PostAsync(IDictionary<string, string> headers, IDictionary<string, string> postParams, string url)
		{
			var httpClient = MakeHttpClient(headers);
			var content = PostParamsToHttpContent(postParams);

			var httpResponseMessage = await httpClient.PostAsync(url, content);
			return await MakeResponse(httpResponseMessage);
		}

		private static HttpContent PostParamsToHttpContent(IDictionary<string, string> postParams)
		{
			string postData = postParams.ToQueryString();
			HttpContent content = new StringContent(postData);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
			return content;
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

		private static async Task<Response> MakeResponse(HttpResponseMessage httpResponse)
		{
			string responseBody = await httpResponse.Content.ReadAsStringAsync();
			var headers = HttpHelpers.MapHeaders(httpResponse.Headers);
			return new Response(httpResponse.StatusCode, headers, responseBody);
		}
	}
}