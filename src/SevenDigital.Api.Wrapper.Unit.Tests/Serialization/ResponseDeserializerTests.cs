using System.Net;
using NUnit.Framework;
using SevenDigital.Api.Schema;
using SevenDigital.Api.Wrapper.Exceptions;
using SevenDigital.Api.Wrapper.Http;
using SevenDigital.Api.Wrapper.Serialization;

namespace SevenDigital.Api.Wrapper.Unit.Tests.Serialization
{
	[TestFixture]
	public class ResponseDeserializerTests
	{
		[Test]
		public void Can_deserialize_object()
		{
			//success case with well formed response
			const string XmlResponse = "<?xml version=\"1.0\"?><response xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" status=\"ok\"><testObject id=\"1\"> <name>A big test object</name><listOfThings><string>one</string><string>two</string><string>three</string></listOfThings><listOfInnerObjects><InnerObject id=\"1\"><Name>Trevor</Name></InnerObject><InnerObject id=\"2\"><Name>Bill</Name></InnerObject></listOfInnerObjects></testObject></response>";

			var stubResponse = new Response(HttpStatusCode.OK, XmlResponse);

			var xmlSerializer = new ResponseDeserializer<TestObject>();

			Assert.DoesNotThrow(() => xmlSerializer.Deserialize(stubResponse));

			TestObject testObject = xmlSerializer.Deserialize(stubResponse);

			Assert.That(testObject.Id, Is.EqualTo(1));
		}

		[Test]
		public void Can_deserialize_server_error()
		{
			const string ErrorXml = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><response status=\"error\" version=\"1.2\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:noNamespaceSchemaLocation=\"http://api.7digital.com/1.2/static/7digitalAPI.xsd\" ><error code=\"1001\"><errorMessage>Test error</errorMessage></error></response>";
			var response = new Response(HttpStatusCode.InternalServerError, ErrorXml);

			var xmlSerializer = new ResponseDeserializer<TestObject>();

			var ex = Assert.Throws<ApiXmlException>(() => xmlSerializer.Deserialize(response));

			Assert.That(ex.StatusCode, Is.EqualTo(response.StatusCode));
			Assert.That(ex.Message, Is.StringStarting("Error response"));
			Assert.That(ex.Message, Is.StringEnding(ErrorXml));

			Assert.That(ex.Error.ErrorMessage, Is.EqualTo("Test error"));
			Assert.That(ex.Error.Code, Is.EqualTo(1001));
		}

		[Test]
		public void Can_deserialize_well_formed_error()
		{
			const string ErrorXml = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><response status=\"error\" version=\"1.2\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:noNamespaceSchemaLocation=\"http://api.7digital.com/1.2/static/7digitalAPI.xsd\" ><error code=\"1001\"><errorMessage>Test error</errorMessage></error></response>";
			var response = new Response(HttpStatusCode.OK, ErrorXml);

			var xmlSerializer = new ResponseDeserializer<TestObject>();

			var ex = Assert.Throws<ApiXmlException>(() => xmlSerializer.Deserialize(response));

			Assert.That(ex.StatusCode, Is.EqualTo(response.StatusCode));
			Assert.That(ex.Message, Is.StringStarting("Error response"));
			Assert.That(ex.Message, Is.StringEnding(ErrorXml));
			Assert.That(ex.Error.Code, Is.EqualTo(1001));
			Assert.That(ex.Error.ErrorMessage, Is.EqualTo("Test error"));
		}

		//badly formed xmls / response
		[Test]
		public void Should_not_fail_if_xml_is_a_malformed_api_error()
		{
			const string BadError = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><response status=\"error\" version=\"1.2\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:noNamespaceSchemaLocation=\"http://api.7digital.com/1.2/static/7digitalAPI.xsd\" ><error><errorme></errorme></error></response>";
			var response = new Response(HttpStatusCode.OK, BadError);

			var xmlSerializer = new ResponseDeserializer<TestObject>();

			var ex = Assert.Throws<ApiXmlException>(() => xmlSerializer.Deserialize(response));

			Assert.That(ex.StatusCode, Is.EqualTo(response.StatusCode));
			Assert.That(ex.Error.ErrorMessage, Is.StringStarting("XML error parse failed"));
		}

		[Test]
		public void Should_not_fail_if_xml_is_missing_error_code()
		{
			const string ValidXml = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><response status=\"error\" version=\"1.2\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:noNamespaceSchemaLocation=\"http://api.7digital.com/1.2/static/7digitalAPI.xsd\" ><error><errorMessage>An error</errorMessage></error></response>";
			var response = new Response(HttpStatusCode.OK, ValidXml);

			var xmlSerializer = new ResponseDeserializer<TestObject>();

			var ex = Assert.Throws<ApiXmlException>(() => xmlSerializer.Deserialize(response));
			Assert.That(ex.StatusCode, Is.EqualTo(response.StatusCode));
			Assert.That(ex.Error.ErrorMessage, Is.StringStarting("XML error parse failed"));
		}

		[Test]
		public void Should_not_fail_if_xml_is_missing_error_message()
		{
			const string ValidXml = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><response status=\"error\" version=\"1.2\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:noNamespaceSchemaLocation=\"http://api.7digital.com/1.2/static/7digitalAPI.xsd\" ><error code=\"123\"></error></response>";
			var response = new Response( HttpStatusCode.OK, ValidXml);

			var xmlSerializer = new ResponseDeserializer<TestObject>();

			var ex = Assert.Throws<ApiXmlException>(() => xmlSerializer.Deserialize(response));
			Assert.That(ex.StatusCode, Is.EqualTo(response.StatusCode));
			Assert.That(ex.Error.ErrorMessage, Is.StringStarting("XML error parse failed"));
		}

		[Test]
		public void Should_throw_api_exception_with_null()
		{
			var apiXmlDeSerializer = new ResponseDeserializer<Status>();

			var apiException = Assert.Throws<ApiXmlException>(() => apiXmlDeSerializer.Deserialize(null));
			Assert.That(apiException.Message, Is.EqualTo("No response"));
		}

		[Test]
		public void Error_captures_http_status_code_from_html()
		{
			const string BadXml = "<html><head>Error</head><body>It did not work<br><hr></body></html>";

			var response = new Response(HttpStatusCode.InternalServerError, BadXml);

			var xmlSerializer = new ResponseDeserializer<TestObject>();

			var ex = Assert.Throws<ApiXmlException>(() => xmlSerializer.Deserialize(response));

			Assert.That(ex, Is.Not.Null);
			Assert.That(ex.Message, Is.StringStarting("Server error:"));
			Assert.That(ex.Message, Is.StringEnding(BadXml));
			Assert.That(ex.StatusCode, Is.EqualTo(response.StatusCode));
		}

		[Test]
		public void turns_html_ok_response_into_error()
		{
			const string BadXml = "<html><head>Error</head><body>Some random html page<br><hr></body></html>";

			var response = new Response( HttpStatusCode.OK, BadXml);

			var xmlSerializer = new ResponseDeserializer<TestObject>();

			var ex = Assert.Throws<ApiXmlException>(() => xmlSerializer.Deserialize(response));

			Assert.That(ex, Is.Not.Null);
			Assert.That(ex.Message, Is.StringStarting("Error trying to deserialize xml response"));
			Assert.That(ex.Message, Is.StringEnding(BadXml));
			Assert.That(ex.StatusCode, Is.EqualTo(response.StatusCode));
		}

		[Test]
		public void Should_handle_plaintext_OauthFail()
		{
			var response = new Response( HttpStatusCode.Unauthorized, 
				"OAuth authentication error: Not authorised - no user credentials provided");

			var xmlSerializer = new ResponseDeserializer<TestObject>();
			var ex = Assert.Throws<ApiXmlException>(() => xmlSerializer.Deserialize(response));

			Assert.That(ex, Is.Not.Null);
			Assert.That(ex.Message, Is.StringStarting("Error response"));
			Assert.That(ex.Message, Is.StringEnding(response.Body));
			Assert.That(ex.StatusCode, Is.EqualTo(response.StatusCode));

			Assert.That(ex.Error.Code, Is.EqualTo(9001));
			Assert.That(ex.Error.ErrorMessage, Is.EqualTo(response.Body));
		}

		[Test]
		public void should_throw_exception_when_deserialize_with_invalid_status()
		{
			const string InvalidStatusXmlResponse = "<?xml version=\"1.0\"?><response status=\"fish\" version=\"1.2\"></response>";
			var response = new Response(HttpStatusCode.OK, InvalidStatusXmlResponse);
			var deserializer = new ResponseDeserializer<TestEmptyObject>();

			var ex = Assert.Throws<ApiXmlException>(() => deserializer.Deserialize(response));

			Assert.That(ex, Is.Not.Null);
			Assert.That(ex.Message, Is.StringStarting("No valid status found in response."));
			Assert.That(ex.StatusCode, Is.EqualTo(response.StatusCode));
		}

		[Test]
		public void should_throw_exception_when_deserialize_with_missing_status()
		{
			const string MissingStatusXmlResponse = "<?xml version=\"1.0\"?><response version=\"1.2\"></response>";
			var response = new Response(HttpStatusCode.OK, MissingStatusXmlResponse);
			var deserializer = new ResponseDeserializer<TestEmptyObject>();

			var ex = Assert.Throws<ApiXmlException>(() => deserializer.Deserialize(response));

			Assert.That(ex, Is.Not.Null);
			Assert.That(ex.Message, Is.StringStarting("No valid status found in response."));
			Assert.That(ex.StatusCode, Is.EqualTo(response.StatusCode));
		}
	}
}