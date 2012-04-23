using System.Xml.Serialization;
using SevenDigital.Api.Schema.Attributes;
using SevenDigital.Api.Wrapper.EndpointResolution.OAuth;

namespace SevenDigital.Api.Schema.OAuth
{
	[ApiEndpoint("oauth/requesttoken/authorise")]
	[XmlRoot("response")]
	[OAuthSigned]
	public class Authorise
	{
	}
}
