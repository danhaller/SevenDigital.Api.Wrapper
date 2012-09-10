using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SevenDigital.Api.Wrapper.Utility.Http;
using SevenDigital.Api.Wrapper.EndpointResolution.OAuth;

namespace SevenDigital.Api.Wrapper.EndpointResolution.RequestHandlers
{
	public class PostRequestHandler : RequestHandler
	{
		private readonly IOAuthCredentials _oAuthCredentials;
		private readonly IUrlSigner _urlSigner;

		public PostRequestHandler(IApiUri apiUri, IOAuthCredentials oAuthCredentials, IUrlSigner urlSigner) : base(apiUri)
		{
			_oAuthCredentials = oAuthCredentials;
			_urlSigner = urlSigner;
		}

		public override Task<HttpResponseMessage> HitEndpointAsync(EndPointInfo endPointInfo)
		{
			var request = BuildPostRequest(endPointInfo); 
			return HttpClient.PostAsync(request);
		}

		private PostRequest BuildPostRequest(EndPointInfo endPointInfo)
		{
			var uri = ConstructEndpoint(endPointInfo);
			var signedParams = SignHttpPostParams(uri, endPointInfo);

			return new PostRequest(uri, endPointInfo.Headers, signedParams);
		}

		private IDictionary<string, string> SignHttpPostParams(string uri, EndPointInfo endPointInfo)
		{
			if (endPointInfo.IsSigned)
			{
				return _urlSigner.SignPostRequest(uri, endPointInfo.UserToken, endPointInfo.UserSecret, _oAuthCredentials, endPointInfo.Parameters);
			}
			return endPointInfo.Parameters;
		}

		protected override string AdditionalParameters(Dictionary<string, string> newDictionary)
		{
			return String.Empty;
		}
	}
}