using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using SevenDigital.Api.Wrapper.EndpointResolution;

namespace SevenDigital.Api.Wrapper.Http
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

		private HttpClient MakeHttpClient(IDictionary<string, string> headers)
		{
			var httpClient = new HttpClient();

			httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip", 1.0));
			httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("UTF8", 0.9));

			httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("7digital-.Net-Api-Wrapper", "4.5"));
			
			foreach (var header in headers)
			{
				httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
			}

			return httpClient;
		}

		private static async Task<Response> MakeResponse(HttpResponseMessage httpResponse)
		{
			var headers = HttpHelpers.MapHeaders(httpResponse.Headers);
			string responseBody = await ReadResponseBody(httpResponse);
			return new Response(httpResponse.StatusCode, headers, responseBody);
		}

		private static async Task<string> ReadResponseBody(HttpResponseMessage httpResponse)
		{
			var contentIsGzipped = HeadersIndicateGippedContent(httpResponse.Content.Headers);

			if (contentIsGzipped)
			{
				var contentStream = await httpResponse.Content.ReadAsStreamAsync();
				var uncompressedStream = new GZipStream(contentStream, CompressionMode.Decompress);
				var reader = new StreamReader(uncompressedStream);

				return await reader.ReadToEndAsync();
			}

			return await httpResponse.Content.ReadAsStringAsync();
		}

		private static bool HeadersIndicateGippedContent(HttpContentHeaders headers)
		{
			return headers.ContentEncoding.Contains("gzip");
		}
	}
}