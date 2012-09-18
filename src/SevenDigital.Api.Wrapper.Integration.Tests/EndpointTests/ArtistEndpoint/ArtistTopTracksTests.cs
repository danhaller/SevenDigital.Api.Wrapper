using System;
using NUnit.Framework;
using SevenDigital.Api.Wrapper.Exceptions;
using SevenDigital.Api.Schema.ArtistEndpoint;

namespace SevenDigital.Api.Wrapper.Integration.Tests.EndpointTests.ArtistEndpoint
{
	[TestFixture]
	[Category("Integration")]
	public class ArtistTopTracksTests
	{
		[Test]
		public void Can_hit_endpoint()
		{
			ArtistTopTracks artist = new FluentApi<ArtistTopTracks>()
				.WithParameter("artistId", "1")
				.WithParameter("country", "GB")
				.PleaseAsync()
				.Await();

			Assert.That(artist, Is.Not.Null);
			Assert.That(artist.Tracks.Count, Is.GreaterThan(0));
		}

		[Test]
		public void Can_hit_endpoint_with_fluent_interface()
		{
			var artist = Api<ArtistTopTracks>
				.Create
				.WithArtistId(1)
				.WithParameter("country", "GB")
				.PleaseAsync()
				.Await();
			
			Assert.That(artist, Is.Not.Null);
			Assert.That(artist.Tracks.Count, Is.GreaterThan(0));
		}

		[Test]
		public void Can_handle_pagingerror_with_paging()
		{
			ApiXmlException expectedError = null;
			try
			{
				new FluentApi<ArtistTopTracks>()
					.WithParameter("artistId", "1")
					.WithParameter("page", "2")
					.WithParameter("pageSize", "10")
					.PleaseAsync()
					.Await();
			} 
			catch(ApiXmlException ex)
			{
				expectedError = ex;
			}

			Assert.That(expectedError, Is.Not.Null);
			Assert.That(expectedError.Error, Is.Not.Null);
			Assert.That(expectedError.Error.Code, Is.EqualTo(1003));
			Assert.That(expectedError.Error.ErrorMessage, Is.EqualTo("Requested page out of range"));
		}
	}
}