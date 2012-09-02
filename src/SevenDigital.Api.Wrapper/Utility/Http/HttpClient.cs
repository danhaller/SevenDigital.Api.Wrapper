using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using SevenDigital.Api.Wrapper.EndpointResolution;

namespace SevenDigital.Api.Wrapper.Utility.Http
{
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    public class HttpClient : IHttpClient
	{
	    private readonly System.Net.Http.HttpClient httpClient;

        public HttpClient(Dictionary<string, string> headers)
        {
            httpClient = new System.Net.Http.HttpClient();

            foreach (var header in headers)
            {
                httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
        }

        public async Task<HttpResponseMessage> GetAsync(IRequest request)
        {
            return await httpClient.GetAsync(request.Url);
        }

        public async Task<HttpResponseMessage> PostAsync(IRequest request)
		{
            string postData = request.Parameters.ToQueryString();
            HttpContent content = new StringContent(postData);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            return await httpClient.PostAsync(request.Url, content);
        }
	}
}