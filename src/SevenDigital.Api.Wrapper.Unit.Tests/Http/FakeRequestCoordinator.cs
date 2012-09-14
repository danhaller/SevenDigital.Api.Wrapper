using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SevenDigital.Api.Wrapper.Http;

namespace SevenDigital.Api.Wrapper.Unit.Tests.Http
{
    public class FakeRequestCoordinator : IRequestCoordinator
    {
		public async Task<Response> GetDataAsync(RequestData requestData)
		{
            return await Task.Factory.StartNew(() => FakeResponse);
		}

        public string EndpointUrl(RequestData requestData)
		{
			throw new NotImplementedException();
		}

		public IHttpClientWrapper HttpClient
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

        public Response FakeResponse { get; set; }
	}
}
