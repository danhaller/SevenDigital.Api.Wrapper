using System;
using System.Linq.Expressions;
using System.Net;
using FakeItEasy;
using NUnit.Framework;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using SevenDigital.Api.Schema;
using SevenDigital.Api.Wrapper.EndpointResolution;
using SevenDigital.Api.Wrapper.Unit.Tests.Utility.Http;
using SevenDigital.Api.Wrapper.Utility.Http;

namespace SevenDigital.Api.Wrapper.Unit.Tests
{
	[TestFixture]
	public class FluentApiTests
	{
		private const string VALID_STATUS_XML = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><response status=\"ok\" version=\"1.2\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:noNamespaceSchemaLocation=\"http://api.7digital.com/1.2/static/7digitalAPI.xsd\"><serviceStatus><serverTime>2011-05-31T15:31:22Z</serverTime></serviceStatus></response>";

		private readonly Response stubResponse  = new Response(HttpStatusCode.OK, VALID_STATUS_XML);

		[Test]
		public void Should_fire_requestcoordinator_with_correct_endpoint_on_resolve()
		{
			var requestCoordinator = A.Fake<IRequestCoordinator>();
			A.CallTo(() => requestCoordinator.HitEndpointAsync(A<EndPointInfo>.Ignored)).Returns(stubResponse);

			new FluentApi<Status>(requestCoordinator).Please();

			Expression<Func<Response>> callWithEndpointStatus =
				() => requestCoordinator.HitEndpointAsync(A<EndPointInfo>.That.Matches(x => x.UriPath == "status"));

			A.CallTo(callWithEndpointStatus).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void Should_fire_requestcoordinator_with_correct_methodname_on_resolve()
		{
			var requestCoordinator = A.Fake<IRequestCoordinator>();
			A.CallTo(() => requestCoordinator.HitEndpoint(A<EndPointInfo>.Ignored)).Returns(stubResponse);

			new FluentApi<Status>(requestCoordinator).WithMethod("POST").Please();

			Expression<Func<Response>> callWithMethodPost =
				() => requestCoordinator.HitEndpointAsync(A<EndPointInfo>.That.Matches(x => x.HttpMethod == "POST"));

			A.CallTo(callWithMethodPost).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void Should_fire_requestcoordinator_with_correct_parameters_on_resolve()
		{
			var requestCoordinator = A.Fake<IRequestCoordinator>();
			A.CallTo(() => requestCoordinator.HitEndpointAsync(A<EndPointInfo>.Ignored)).Returns(stubResponse);

			new FluentApi<Status>(requestCoordinator).WithParameter("artistId", "123").Please();

			Expression<Func<Response>> callWithArtistId123 =
				() => requestCoordinator.HitEndpointAsync(A<EndPointInfo>.That.Matches(x => x.Parameters["artistId"] == "123"));

			A.CallTo(callWithArtistId123).MustHaveHappened();

		}
		[Test]
		public void Should_use_custom_http_client()
		{
			var fakeRequestCoordinator = A.Fake<IRequestCoordinator>();
			var fakeHttpClient = new FakeHttpClient();

			new FluentApi<Status>(fakeRequestCoordinator).UsingClient(fakeHttpClient);

			Assert.That(fakeRequestCoordinator.HttpClient, Is.EqualTo(fakeHttpClient));
		}

		[Test]
		public async void should_put_payload_in_action_result()
		{
			var requestCoordinator = new FakeRequestCoordinator { StubPayload = stubResponse };

			var status = await new FluentApi<Status>(requestCoordinator)
				.PleaseAsync();

			Assert.That(status, Is.Not.Null);
		}

		public class FakeRequestCoordinator : IRequestCoordinator
		{
			public async Task<HttpResponseMessage> HitEndpointAsync(EndPointInfo endPointInfo)
			{
				throw new NotImplementedException();
			}

			public string ConstructEndpoint(EndPointInfo endPointInfo)
			{
				throw new NotImplementedException();
			}

			public IHttpClient HttpClient
			{
				get { throw new NotImplementedException(); }
				set { throw new NotImplementedException(); }
			}

			public Response StubPayload { get; set; }
		}
	}

}
