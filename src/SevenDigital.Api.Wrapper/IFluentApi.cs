﻿using System;
using SevenDigital.Api.Wrapper.Utility.Http;

namespace SevenDigital.Api.Wrapper
{
    using System.Threading.Tasks;

    // [AD] DO NOT PUT THE OUR BACK IN, NOT SUPPORTED IN WINDOWS PHONE
	// ReSharper disable TypeParameterCanBeVariant
	public interface IFluentApi<T>
	// ReSharper restore TypeParameterCanBeVariant
	{
		IFluentApi<T> WithParameter(string key, string value);
		IFluentApi<T> ClearParameters();
		IFluentApi<T> ForUser(string token, string secret);
		IFluentApi<T> WithEndpoint(string endpoint);
		IFluentApi<T> UsingClient(IHttpClient httpClient);
		string EndpointUrl { get; }

		Task<T> PleaseAsync();
	}
}
