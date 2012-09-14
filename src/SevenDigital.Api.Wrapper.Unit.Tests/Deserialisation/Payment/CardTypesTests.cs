using System.Linq;
using System.Net;
using NUnit.Framework;
using SevenDigital.Api.Schema.Payment;
using SevenDigital.Api.Wrapper.Http;
using SevenDigital.Api.Wrapper.Serialization;

namespace SevenDigital.Api.Wrapper.Unit.Tests.Deserialisation.Payment
{
	[TestFixture]
	public class CardTypeTests
	{
		private const string ResponseBody = " <response status=\"ok\" version=\"1.2\"><cardTypes><cardType id=\"MAESTRO\">Maestro</cardType><cardType id=\"MASTERCARD\">MasterCard</cardType><cardType id=\"VISA\">Visa</cardType><cardType id=\"AMEX\">American Express</cardType></cardTypes></response>";

		private readonly Response _stubResponse = new Response(HttpStatusCode.OK, ResponseBody);

		[Test]
		public void can_deseralize_card_types()
		{
			var xmlSerializer = new ResponseDeserializer<PaymentCardTypes>();

			var result = xmlSerializer.Deserialize(this._stubResponse);

			Assert.That(result.CardTypes.Count(),Is.EqualTo(4));
			var lastCard = result.CardTypes.Last();
			Assert.That(lastCard.Type, Is.EqualTo("American Express"));
			Assert.That(lastCard.Id, Is.EqualTo("AMEX"));
		}
	}
}
