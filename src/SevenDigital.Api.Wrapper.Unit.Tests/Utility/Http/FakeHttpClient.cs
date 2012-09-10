using System;
using System.Net.Http;
using System.Threading.Tasks;
using SevenDigital.Api.Wrapper.Http;

namespace SevenDigital.Api.Wrapper.Unit.Tests.Utility.Http
{
	public class FakeHttpClient : IHttpClient
	{
		private readonly HttpResponseMessage _fakeResponse;

		public FakeHttpClient()
		{
		}

		public FakeHttpClient(HttpResponseMessage fakeResponse)
		{
			_fakeResponse = fakeResponse;
		}

		public async Task<HttpResponseMessage> GetAsync(GetRequest request)
		{
			return await Task.Factory.StartNew(() => _fakeResponse);
		}

		public async Task<HttpResponseMessage> PostAsync(PostRequest request)
		{
			return await Task.Factory.StartNew(() => _fakeResponse);
		}
	}
}