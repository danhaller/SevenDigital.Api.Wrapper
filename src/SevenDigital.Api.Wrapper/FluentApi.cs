﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using SevenDigital.Api.Schema.OAuth;
using SevenDigital.Api.Wrapper.EndpointResolution;
using SevenDigital.Api.Wrapper.EndpointResolution.OAuth;
using SevenDigital.Api.Schema.Attributes;
using SevenDigital.Api.Wrapper.Utility.Http;
using SevenDigital.Api.Wrapper.Utility.Serialization;

using HttpClient = SevenDigital.Api.Wrapper.Utility.Http.HttpClient;

namespace SevenDigital.Api.Wrapper
{
	public class FluentApi<T> : IFluentApi<T> where T : class
	{
		private readonly EndPointInfo _endPointInfo = new EndPointInfo();
		private readonly IRequestCoordinator _requestCoordinator;
		private readonly IResponseDeserializer<T> _deserializer;

		public FluentApi(IRequestCoordinator requestCoordinator)
		{
			_requestCoordinator = requestCoordinator;

			_deserializer = new ResponseDeserializer<T>();

			ApiEndpointAttribute attribute = typeof(T).GetCustomAttributes(true)
				.OfType<ApiEndpointAttribute>()
				.FirstOrDefault();

			if (attribute == null)
			{
				throw new ArgumentException(string.Format("The Type {0} cannot be used in this way, it has no ApiEndpointAttribute", typeof(T)));
			}

			_endPointInfo.UriPath = attribute.EndpointUri;


			OAuthSignedAttribute isSigned = typeof(T).GetCustomAttributes(true)
				.OfType<OAuthSignedAttribute>()
				.FirstOrDefault();

			if (isSigned != null)
			{
				_endPointInfo.IsSigned = true;
			}

			RequireSecureAttribute isSecure = typeof(T).GetCustomAttributes(true)
				.OfType<RequireSecureAttribute>()
				.FirstOrDefault();

			if (isSecure != null)
			{
				_endPointInfo.UseHttps = true;
			}

			HttpPostAttribute isHttpPost = typeof(T).GetCustomAttributes(true)
				.OfType<HttpPostAttribute>()
				.FirstOrDefault();

			if (isHttpPost != null)
			{
				_endPointInfo.HttpMethod = "POST";
			}
		}

		public FluentApi(IOAuthCredentials oAuthCredentials, IApiUri apiUri)
			: this(new RequestCoordinator(new HttpClient(), new UrlSigner(), oAuthCredentials, apiUri)) { }

		public FluentApi()
			: this(new RequestCoordinator(new HttpClient(), new UrlSigner(), EssentialDependencyCheck<IOAuthCredentials>.Instance, EssentialDependencyCheck<IApiUri>.Instance)) { }

		public IFluentApi<T> WithEndpoint(string endpoint)
		{
			_endPointInfo.UriPath = endpoint;
			return this;
		}

		public IFluentApi<T> UsingClient(IHttpClient httpClient)
		{
			_requestCoordinator.HttpClient = httpClient;
			return this;
		}

		public virtual IFluentApi<T> WithMethod(string methodName)
		{
			_endPointInfo.HttpMethod = methodName;
			return this;
		}

		public virtual IFluentApi<T> WithParameter(string parameterName, string parameterValue)
		{
			_endPointInfo.Parameters[parameterName] = parameterValue;
			return this;
		}

		public virtual IFluentApi<T> ClearParameters()
		{
			_endPointInfo.Parameters.Clear();
			return this;
		}

		public virtual IFluentApi<T> ForUser(string token, string secret)
		{
			_endPointInfo.UserToken = token;
			_endPointInfo.UserSecret = secret;
			return this;
		}

		public virtual IFluentApi<T> ForShop(int shopId)
		{
			WithParameter("shopId", shopId.ToString());
			return this;
		}

		public virtual string EndpointUrl
		{
			get { return _requestCoordinator.ConstructEndpoint(_endPointInfo); }
		}

		public virtual async Task<T> PleaseAsync()
		{
		   var httpResponse = await	_requestCoordinator.HitEndpointAsync(_endPointInfo);
		   var response = await MakeResponse(httpResponse);
		   return _deserializer.Deserialize(response);
		}

		private async Task<Response> MakeResponse(HttpResponseMessage httpResponse)
		{
			string responseBody = await httpResponse.Content.ReadAsStringAsync();
			var headers = HttpHelpers.MapHeaders(httpResponse.Headers);
			Response response = new Response(httpResponse.StatusCode, headers, responseBody);
			return response;
		}

		public IDictionary<string, string> Parameters { get { return _endPointInfo.Parameters; } }
	}
}