using FakeItEasy;
using NUnit.Framework;
using SevenDigital.Api.Wrapper.EndpointResolution.OAuth;

namespace SevenDigital.Api.Wrapper.Unit.Tests.EndpointResolution.OAuth
{
	[TestFixture]
	public class UrlSignerTests
	{
		private const string ConsumerKey = "key";
		private const string ConsumerSecret = "secret";

		[Test]
		public void SignUrlAsString_escapes_those_stupid_plus_signs_and_other_evils_in_signature()
		{
			const string Url = "http://www.example.com?parameter=hello&again=there";

			for (int i = 0; i < 50; i++)
			{
				var signedUrl = new UrlSigner().SignGetUrl(Url, null, null, GetOAuthCredentials());
				var index = signedUrl.IndexOf("oauth_signature");
				var signature = signedUrl.Substring(index + "oauth_signature".Length);

				Assert.That(!signature.Contains("+"), "signature contains a '+' character and isn't being encoded properly");
			}
		}

		[Test]
		public void SignUrl_adds_oauth_signature()
		{
			const string Url = "http://www.example.com?parameter=hello&again=there";

			var signedUrl = new UrlSigner().SignUrl(Url, null, null, GetOAuthCredentials());
			Assert.That(signedUrl.Query.Contains("oauth_signature"));
		}

		private IOAuthCredentials GetOAuthCredentials()
		{
			var oAuthCredentials = A.Fake<IOAuthCredentials>();
			A.CallTo(() => oAuthCredentials.ConsumerKey).Returns(ConsumerKey);
			A.CallTo(() => oAuthCredentials.ConsumerKey).Returns(ConsumerSecret);
			return oAuthCredentials;
		}
	}
}
