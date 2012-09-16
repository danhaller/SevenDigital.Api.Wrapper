using System;
using NUnit.Framework;
using SevenDigital.Api.Wrapper.Exceptions;
using SevenDigital.Api.Schema.ArtistEndpoint;
using SevenDigital.Api.Schema.LockerEndpoint;

namespace SevenDigital.Api.Wrapper.Integration.Tests.Exceptions
{
	[TestFixture]
	public class ApiXmlExceptionTests
	{
		[Test]
		public async void Should_fail_correctly_if_xml_error_returned()
		{
			// -- Deliberate error response
			Console.WriteLine("Trying artist/details without artistId parameter...");

			ApiXmlException apiXmlException = null;

			try
			{
				await Api<Artist>.Create.PleaseAsync();
			}
			catch (ApiXmlException ex)
			{
				apiXmlException = ex;
			}

			Assert.That(apiXmlException, Is.Not.Null);
			Assert.That(apiXmlException.Error.Code, Is.EqualTo(1001));
			Assert.That(apiXmlException.Error.ErrorMessage, Is.EqualTo("Missing parameter artistId."));
		}

		[Test]
		public async void Should_fail_correctly_if_non_xml_error_returned_eg_unauthorised()
		{
			// -- Deliberate unauthorized response
			Console.WriteLine("Trying user/locker without any credentials...");
			ApiXmlException apiXmlException = null;

			try
			{
				await Api<Locker>.Create.PleaseAsync();
			}
			catch (ApiXmlException ex)
			{
				apiXmlException = ex;
			}

			Assert.That(apiXmlException, Is.Not.Null);
			Assert.That(apiXmlException.Error.Code, Is.EqualTo(9001));
			Assert.That(apiXmlException.Error.ErrorMessage, Is.EqualTo("OAuth authentication error: Resource requires access token"));
		}
	}
}