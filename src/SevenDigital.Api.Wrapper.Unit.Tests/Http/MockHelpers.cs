using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using SevenDigital.Api.Wrapper.Http;

namespace SevenDigital.Api.Wrapper.Unit.Tests.Http
{
	public static class MockHelpers
	{
		public static void MockGetAsync(this IHttpClientWrapper httpClient, Response response)
		{
			A.CallTo(() => httpClient.GetAsync(A<IDictionary<string, string>>.Ignored, A<string>.Ignored))
				.ReturnsLazily(() => Task.Factory.StartNew(() => response));
		}

		public static void GetAsyncOnUrlMustHaveHappened(this IHttpClientWrapper httpClient, string expected)
		{
			A.CallTo(() => httpClient.GetAsync(
				A<IDictionary<string, string>>.Ignored,
				A<string>.That.Matches(y => y == expected)))
				.MustHaveHappened();
		}

		public static void GetAsyncOnUrlMustHaveHappenedOnce(this IHttpClientWrapper httpClient, string expected)
		{
			A.CallTo(() => httpClient.GetAsync(
				A<IDictionary<string, string>>.Ignored,
				A<string>.That.Matches(y => y == expected)))
				.MustHaveHappened(Repeated.Exactly.Once);
		}

		public static void GetAsyncOnUrlContainingMustHaveHappenedOnce(this IHttpClientWrapper httpClient, string expected)
		{
			A.CallTo(() => httpClient.GetAsync(
				A<IDictionary<string, string>>.Ignored,
				A<string>.That.Matches(y => y.Contains(expected))))
				.MustHaveHappened(Repeated.Exactly.Once);
		}
		public static void MockGetDataAsync(this IRequestCoordinator httpClient, Response response)
		{
			A.CallTo(() => httpClient.GetDataAsync(A<RequestData>.Ignored))
				.ReturnsLazily(() => Task.Factory.StartNew(() => response));
		}

	}
}
