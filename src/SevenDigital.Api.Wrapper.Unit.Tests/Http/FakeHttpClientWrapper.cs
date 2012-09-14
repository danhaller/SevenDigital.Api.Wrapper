using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SevenDigital.Api.Wrapper.Http;

namespace SevenDigital.Api.Wrapper.Unit.Tests.Http
{
	public class FakeHttpClientWrapper : IHttpClientWrapper
	{
        private readonly Response _fakeResponse;

		public FakeHttpClientWrapper()
		{
		}

        public FakeHttpClientWrapper(Response fakeResponse)
		{
			_fakeResponse = fakeResponse;
		}

        public async Task<Response> GetAsync(IDictionary<string, string> headers, string url)
		{
			return await Task.Factory.StartNew(() => _fakeResponse);
		}

        public async Task<Response> PostAsync(IDictionary<string, string> headers, IDictionary<string, string> postParams, string url)
		{
			return await Task.Factory.StartNew(() => _fakeResponse);
		}
	}
}