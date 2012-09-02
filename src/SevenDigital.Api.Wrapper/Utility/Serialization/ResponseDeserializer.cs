using System;
using System.Net;
using System.Xml.Linq;
using SevenDigital.Api.Schema;
using SevenDigital.Api.Wrapper.Exceptions;

namespace SevenDigital.Api.Wrapper.Utility.Serialization
{
    using System.Net.Http;
    using System.Threading.Tasks;

    public class ResponseDeserializer<T> : IResponseDeserializer<T> where T : class
	{
		private const int DefaultErrorCode = 9001;

        public async Task<T> Deserialize(HttpResponseMessage response)
        {
            var bodyString = await response.Content.ReadAsStringAsync();
            CheckResponse(response, bodyString);
            return ParsedResponse(response, bodyString);
		}

        private void CheckResponse(HttpResponseMessage response, string responseBody)
		{
			if (response == null)
			{
				throw new ApiXmlException("No response");
			}

            if (string.IsNullOrEmpty(responseBody))
			{
				throw new ApiXmlException("No response body", response.StatusCode);
			}

            var startOfMessage = StartOfMessage(responseBody);
			var messageIsXml = IsXml(startOfMessage);

			if (messageIsXml && IsApiErrorResponse(startOfMessage))
			{
                var error = ParseError(responseBody);
                throw new ApiXmlException("Error response:\n" + responseBody, response.StatusCode, error);
			}

			if (IsServerError((int)response.StatusCode))
			{
                throw new ApiXmlException("Server error:\n" + responseBody, response.StatusCode);
			}

			if (!messageIsXml && response.StatusCode != HttpStatusCode.OK)
			{
				var error = new Error
					{
						Code = DefaultErrorCode,
                        ErrorMessage = responseBody
					};
                throw new ApiXmlException("Error response:\n" + responseBody, response.StatusCode, error);
			}

			if (messageIsXml && !IsApiOkResponse(startOfMessage))
			{
				throw new ApiXmlException("No valid status found in response. Status must be one of 'ok' or 'error':\n" + responseBody, response.StatusCode);
			}
		}

		private static string StartOfMessage(string bodyMarkup)
		{
			int maxLength = Math.Min(bodyMarkup.Length, 512);
			return bodyMarkup.Substring(0, maxLength);
		}

		private bool IsXml(string bodyMarkup)
		{
			return bodyMarkup.Contains("<?xml");
		}

		private bool IsApiOkResponse(string bodyMarkup)
		{
			return bodyMarkup.Contains("<response") && bodyMarkup.Contains("status=\"ok\"");
		}

		private bool IsApiErrorResponse(string bodyMarkup)
		{
			return bodyMarkup.Contains("<response")  && bodyMarkup.Contains("status=\"error\"");
		}

		private bool IsServerError(int httpStatusCode)
		{
			return httpStatusCode >= 500;
		}

		private Error ParseError(string xml)
		{
			try
			{
				var xmlDoc = XDocument.Parse(xml);
				var responseNode = xmlDoc.FirstNode as XElement;
				var errorNode = responseNode.FirstNode as XElement;
				var errorMessage = errorNode.FirstNode as XElement;

				int errorCode = ReadErrorCode(errorNode);

				return new Error
				    {
						Code = errorCode,
				       	ErrorMessage = errorMessage.Value
				    };
			}
			catch(Exception ex)
			{
				return new Error
					{
						Code = DefaultErrorCode,
						ErrorMessage =  "XML error parse failed: " + ex
					};
			}
		}

		private int ReadErrorCode(XElement errorNode)
		{
			var attribute = errorNode.Attribute("code");
			return int.Parse(attribute.Value);
		}

        private static T ParsedResponse(HttpResponseMessage response, string responseBody)
		{
			try
			{
				var deserializer = new StringDeserializer<T>();
                return deserializer.Deserialize(responseBody);
			}
			catch (Exception ex)
			{
                string message = string.Format("Error trying to deserialize xml response\n{0}", responseBody);
				throw new ApiXmlException(message, response.StatusCode, ex);
			}
		}
	}
}