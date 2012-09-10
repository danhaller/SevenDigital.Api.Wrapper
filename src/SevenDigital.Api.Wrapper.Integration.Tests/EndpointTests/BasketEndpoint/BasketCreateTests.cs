﻿using System;
using System.Linq;
using NUnit.Framework;
using SevenDigital.Api.Schema.Basket;

namespace SevenDigital.Api.Wrapper.Integration.Tests.EndpointTests.BasketEndpoint
{
	[TestFixture]
	public class BasketCreateTests
	{
		private const int EXPECTED_RELEASE_ID = 160553;
		private string _basketId;
		private const int EXPECTED_TRACK_ID = 1693930;
		
		[TestFixtureSetUp]
		public async void Can_create_basket()
		{
			Basket basketCreate = await Api<Basket>.Create
				.Create()
				.WithParameter("country", "GB")
				.PleaseAsync();

			Assert.That(basketCreate, Is.Not.Null);
			Assert.That(basketCreate.Id, Is.Not.Empty);
			_basketId = basketCreate.Id;
		}

		[Test]
		public async void Can_retrieve_that_basket()
		{
			Basket basket = await Api<Basket>.Create
				.WithParameter("basketId", _basketId)
				.PleaseAsync();

			Assert.That(basket, Is.Not.Null);
			Assert.That(basket.Id, Is.EqualTo(_basketId));
		}

		[Test]
		public async void Can_add_and_remove_release_to_that_basket()
		{
			Basket basket = await Api<Basket>.Create
				.AddItem(new Guid(_basketId), EXPECTED_RELEASE_ID)
				.PleaseAsync();

			Assert.That(basket, Is.Not.Null);
			Assert.That(basket.Id, Is.EqualTo(_basketId));
			Assert.That(basket.BasketItems.Items.Count, Is.GreaterThan(0));
			Assert.That(basket.BasketItems.Items.FirstOrDefault().ReleaseId, Is.EqualTo(EXPECTED_RELEASE_ID.ToString()));

			int toRemove = basket.BasketItems.Items.FirstOrDefault().Id;
			basket = await Api<Basket>.Create
				.RemoveItem(new Guid(_basketId), toRemove) 
				.PleaseAsync();

			Assert.That(basket, Is.Not.Null);
			Assert.That(basket.Id, Is.EqualTo(_basketId));
			Assert.That(basket.BasketItems.Items.Where(x=>x.Id == toRemove).Count(), Is.EqualTo(0));
		}

		[Test]
		public async void Can_add_and_remove_track_to_that_basket()
		{
			Basket basket = await Api<Basket>.Create
				.AddItem(new Guid(_basketId), EXPECTED_RELEASE_ID, EXPECTED_TRACK_ID)
				.PleaseAsync();

			Assert.That(basket, Is.Not.Null);Assert.That(basket.Id, Is.EqualTo(_basketId));
			Assert.That(basket.BasketItems.Items.Count, Is.GreaterThan(0));
			Assert.That(basket.BasketItems.Items.FirstOrDefault().TrackId, Is.EqualTo(EXPECTED_TRACK_ID.ToString()));

			int toRemove = basket.BasketItems.Items.FirstOrDefault().Id;
			basket = await new FluentApi<Basket>()
				.RemoveItem(new Guid(_basketId), toRemove) 
				.PleaseAsync();

			Assert.That(basket, Is.Not.Null);
			Assert.That(basket.Id, Is.EqualTo(_basketId));
			Assert.That(basket.BasketItems.Items.Where(x => x.Id == toRemove).Count(), Is.EqualTo(0));
		}

		[Test]
		public async void Should_show_amount_due()
		{
			Basket basket = await Api<Basket>.Create
				.AddItem(new Guid(_basketId), EXPECTED_RELEASE_ID)
				.PleaseAsync();

			Assert.That(basket.BasketItems.Items.First().AmountDue.Amount, Is.EqualTo("7.99"));
			Assert.That(basket.BasketItems.Items.First().AmountDue.FormattedAmount, Is.EqualTo("£7.99"));
			Assert.That(basket.AmountDue.Amount, Is.EqualTo("7.99"));
			Assert.That(basket.AmountDue.FormattedAmount, Is.EqualTo("£7.99"));
		}

	}
}
