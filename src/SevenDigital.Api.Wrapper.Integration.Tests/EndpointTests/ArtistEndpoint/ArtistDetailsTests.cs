﻿using NUnit.Framework;
using SevenDigital.Api.Schema.ArtistEndpoint;

namespace SevenDigital.Api.Wrapper.Integration.Tests.EndpointTests.ArtistEndpoint
{
	[TestFixture]
	public class ArtistDetailsTests
	{
		[Test]
		public async void Can_hit_endpoint_with_fluent_interface()
		{
			var artist = await Api<Artist>
				.Create
				.WithArtistId(1)
				.PleaseAsync();

			Assert.That(artist, Is.Not.Null);
			Assert.That(artist.Name, Is.EqualTo("Keane"));
			Assert.That(artist.SortName, Is.EqualTo("Keane"));
			Assert.That(artist.Url, Is.StringStarting("http://www.7digital.com/artists/keane/"));
			Assert.That(artist.Image, Is.EqualTo("http://cdn.7static.com/static/img/artistimages/00/000/000/0000000001_150.jpg"));
		}

		[Test]
		public async void Can_hit_endpoint_with_fluent_async_api()
		{
			Artist artist = await Api<Artist>
				.Create
				.WithArtistId(1)
				.PleaseAsync();

			Assert.That(artist, Is.Not.Null);
			Assert.That(artist.Name, Is.EqualTo("Keane"));
			Assert.That(artist.SortName, Is.EqualTo("Keane"));
			Assert.That(artist.Url, Is.StringStarting("http://www.7digital.com/artists/keane/"));
			Assert.That(artist.Image, Is.EqualTo("http://cdn.7static.com/static/img/artistimages/00/000/000/0000000001_150.jpg"));
		}
	}
}