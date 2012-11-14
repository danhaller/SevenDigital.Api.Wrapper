﻿using NUnit.Framework;
using SevenDigital.Api.Schema.ArtistEndpoint;

namespace SevenDigital.Api.Wrapper.Integration.Tests.EndpointTests.ArtistEndpoint
{
	[TestFixture]
	[Category("Integration")]
	public class ArtistSearchTests
	{
		[Test]
		public async void Can_hit_endpoint()
		{
			ArtistSearch artist = await new FluentApi<ArtistSearch>()
				.WithParameter("q", "pink")
				.WithParameter("country", "GB")
				.PleaseAsync();

			Assert.That(artist, Is.Not.Null);
		}

		[Test]
		public async void Can_do_similar_to_browse()
		{
			var artist = await Api<ArtistSearch>
				.Create
				.WithQuery("radiohe")
				.WithParameter("sort","popularity+desc")
				.PleaseAsync();

			Assert.That(artist, Is.Not.Null);
		}

		[Test]
		public async void Can_hit_endpoint_with_fluent_interface()
		{
			ArtistSearch artistSearch = await Api<ArtistSearch>
				.Create
				.WithQuery("pink")
				.WithParameter("country", "GB")
				.PleaseAsync();

			Assert.That(artistSearch, Is.Not.Null);
		}

		[Test]
		public async void Can_hit_endpoint_with_paging()
		{
			ArtistSearch artistBrowse = await Api<ArtistSearch>.Create
				.WithParameter("q", "pink")
				.WithParameter("page", "2")
				.WithParameter("pageSize", "20")
				.PleaseAsync();

			Assert.That(artistBrowse, Is.Not.Null);
			Assert.That(artistBrowse.Page, Is.EqualTo(2));
			Assert.That(artistBrowse.PageSize, Is.EqualTo(20));
		}

		[Test]
		public async void Can_get_multiple_results()
		{
			ArtistSearch artistSearch = await Api<ArtistSearch>.Create
				.WithParameter("q", "pink")
				.WithParameter("page", "1")
				.WithParameter("pageSize", "20")
				.PleaseAsync();

			Assert.That(artistSearch.Results.Count, Is.GreaterThan(1));
		}

		[Test]
		public async void Can_get_multiple_results_with_new_Fluent_api_overload()
		{
			var api = new FluentApi<ArtistSearch>(new AppSettingsCredentials(), new ApiUri());
			var artistSearch = await api
				.ForShop(34)
				.WithQuery("pink")
				.WithPageNumber(1)
				.WithPageSize(20)
				.PleaseAsync();

			Assert.That(artistSearch.Results.Count, Is.GreaterThan(1));
		}
	}
}