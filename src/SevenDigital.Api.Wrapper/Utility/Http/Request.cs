using System;
using System.Collections.Generic;

namespace SevenDigital.Api.Wrapper.Utility.Http
{
	[Serializable]
	public class Request : IRequest
	{
		private readonly string _url;
		private readonly IDictionary<string, string> _parameters;

		public Request(string url)
		{
			_url = url;
		}

		public Request(string url, IDictionary<string, string> parameters)
		{
			_url = url;
			_parameters = parameters;
		}

		public Request()
		{
			_url = string.Empty;
		}
		
		public string Url
		{
			get { return _url; }
		}

		public IDictionary<string, string> Parameters
		{
			get { return _parameters; }
		}
	}
}