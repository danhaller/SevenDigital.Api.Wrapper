using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SevenDigital.Api.Wrapper.EndpointResolution;
using SevenDigital.Api.Wrapper.EndpointResolution.OAuth;

namespace SevenDigital.Api.Wrapper.Utility.Http.Refactored
{
	public class HttpRequestor : IHttpRequestor
	{
		private readonly IUrlSigner _urlSigner;
		private readonly IOAuthCredentials _oAuthCredentials;
		private readonly IApiUri _apiUri;

		public HttpRequestor(IUrlSigner urlSigner, IOAuthCredentials oAuthCredentials, IApiUri apiUri)
		{
			_urlSigner = urlSigner;
			_oAuthCredentials = oAuthCredentials;
			_apiUri = apiUri;
		}

		public Task<Response> GetDataAsync(RequestData requestData)
		{
			if (requestData.HttpMethod == HttpMethod.Get)
			{
				return HitGetEndpoint(requestData);
			}
			if (requestData.HttpMethod == HttpMethod.Post)
			{
				return HitPostEndpoint(requestData);
			}

			throw new ArgumentException("Unimplemented http httpMethod " + requestData.HttpMethod);
		}

		public async Task<Response> HitGetEndpoint(RequestData requestData)
		{
			var uri = ConstructEndpoint(requestData);
			var signedUrl = SignHttpGetUrl(uri, requestData);

			var httpClient = MakeHttpClient(requestData.Headers);
			var httpResponseMessage = await httpClient.GetAsync(signedUrl);

			return await MakeResponse(httpResponseMessage);
		}

		public async Task<Response> HitPostEndpoint(RequestData requestData)
		{
			var uri = ConstructEndpoint(requestData);
			var signedParams = SignHttpPostParams(uri, requestData);

			var httpClient = MakeHttpClient(requestData.Headers);

			string postData = signedParams.ToQueryString();
			HttpContent content = new StringContent(postData);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

			var httpResponseMessage = await httpClient.PostAsync(uri, content);

			return await MakeResponse(httpResponseMessage);
		}

		private string SignHttpGetUrl(string uri, RequestData requestData)
		{
			if (requestData.IsSigned)
			{
				return _urlSigner.SignGetUrl(uri, requestData.UserToken, requestData.UserSecret, _oAuthCredentials);
			}
			return uri;
		}

		private IDictionary<string, string> SignHttpPostParams(string uri, RequestData requestData)
		{
			if (requestData.IsSigned)
			{
				return _urlSigner.SignPostRequest(uri, requestData.UserToken, requestData.UserSecret, _oAuthCredentials, requestData.Parameters);
			}
			return requestData.Parameters;
		}

		public virtual string ConstructEndpoint(RequestData requestData)
		{
			var apiUri = requestData.UseHttps ? _apiUri.SecureUri : _apiUri.Uri;

			var scratchParams = new Dictionary<string, string>(requestData.Parameters);
			var uriString = string.Format("{0}/{1}", apiUri, SubstituteRouteParameters(requestData.UriPath, scratchParams));

			if (requestData.HttpMethod == HttpMethod.Get)
			{
				var oauthParam = "oauth_consumer_key=" + _oAuthCredentials.ConsumerKey;
				var otherQueryParams = scratchParams.ToQueryString(true);
				uriString = uriString + "?" + oauthParam + "&" + otherQueryParams;
			}
			return uriString;
		}

		private static string SubstituteRouteParameters(string endpointUri, IDictionary<string, string> parameters)
		{
			var regex = new Regex("{(.*?)}");
			var result = regex.Matches(endpointUri);
			foreach (var match in result)
			{
				var key = match.ToString().Remove(match.ToString().Length - 1).Remove(0, 1);
				var entry = parameters.First(x => x.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));
				parameters.Remove(entry.Key);
				endpointUri = endpointUri.Replace(match.ToString(), entry.Value);
			}

			return endpointUri.ToLower();
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