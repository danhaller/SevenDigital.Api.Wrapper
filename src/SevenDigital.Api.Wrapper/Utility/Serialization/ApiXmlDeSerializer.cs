using System;
using SevenDigital.Api.Wrapper.Exceptions;

namespace SevenDigital.Api.Wrapper.Utility.Serialization
{
	public class ApiXmlDeSerializer<T> : IDeSerializer<T> where T : class
	{
		private readonly IDeSerializer<T> _deSerializer;
		private readonly IXmlErrorHandler _xmlErrorHandler;

		public ApiXmlDeSerializer(IDeSerializer<T> deSerializer, IXmlErrorHandler xmlErrorHandler) {
			_deSerializer = deSerializer;
			_xmlErrorHandler = xmlErrorHandler;
		}

		public T DeSerialize(string response)
		{
			var responseNode = _xmlErrorHandler.GetResponseAsXml(response);
			_xmlErrorHandler.AssertError(responseNode);
			string resourceNode = responseNode.FirstNode != null ? responseNode.FirstNode.ToString() : responseNode.ToString();
			
			try
			{
				return _deSerializer.DeSerialize(resourceNode);	
			}
			catch(InvalidOperationException ioex)
			{
				throw new ApiXmlException("Error deserializing response from API", ioex);
			}
		}
	}
}