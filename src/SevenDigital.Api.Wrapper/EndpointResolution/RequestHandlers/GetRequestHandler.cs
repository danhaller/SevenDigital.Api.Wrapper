using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SevenDigital.Api.Wrapper.Utility.Http;
using SevenDigital.Api.Wrapper.EndpointResolution.OAuth;

namespace SevenDigital.Api.Wrapper.EndpointResolution.RequestHandlers
{

    public class GetRequestHandler : RequestHandler
	{
		private readonly IOAuthCredentials _oAuthCredentials;
		private readonly IUrlSigner _urlSigner;

		public GetRequestHandler(IApiUri apiUri, IOAuthCredentials oAuthCredentials, IUrlSigner urlSigner) : base(apiUri)
		{
			_oAuthCredentials = oAuthCredentials;
			_urlSigner = urlSigner;
		}

        public override Task<HttpResponseMessage> HitEndpointAsync(EndPointInfo endPointInfo)
		{
            GetRequest request = BuildGetRequest(endPointInfo);
            return HttpClient.GetAsync(request);
		}

        private GetRequest BuildGetRequest(EndPointInfo endPointInfo)
        {
            var uri = ConstructEndpoint(endPointInfo);
            var signedUrl = SignHttpGetUrl(uri, endPointInfo);
            return new GetRequest(signedUrl, endPointInfo.Headers);
        }

		private string SignHttpGetUrl(string uri, EndPointInfo endPointInfo)
		{
			if (endPointInfo.IsSigned)
			{
				return _urlSigner.SignGetUrl(uri, endPointInfo.UserToken, endPointInfo.UserSecret, _oAuthCredentials);
			}
			return uri;
		}

		protected override string AdditionalParameters(Dictionary<string, string> newDictionary)
		{
			return string.Format("?oauth_consumer_key={0}&{1}", _oAuthCredentials.ConsumerKey, newDictionary.ToQueryString(true)).TrimEnd('&');
		}
	}
}