using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SupermarketReceipt;

namespace Supermarket.Test
{
    [TestClass]
    public class SupermarketTest
    {
        [TestMethod]
        public void TenPercentDiscount()
        {
            // ARRANGE
            SupermarketCatalog catalog = new FakeCatalog();
            var toothbrush = new Product("toothbrush", ProductUnit.Each);
            catalog.AddProduct(toothbrush, 0.99);
            var apples = new Product("apples", ProductUnit.Kilo);
            catalog.AddProduct(apples, 1.99);

            var cart = new ShoppingCart();
            cart.AddItemQuantity(apples, 2.5);

            var teller = new Teller(catalog);
            teller.AddSpecialOffer(SpecialOfferType.TenPercentDiscount, toothbrush, 10.0);

            // ACT
            var receipt = teller.ChecksOutArticlesFrom(cart);

            // ASSERT
            receipt.GetTotalPrice().Should().Be(4.975);
            receipt.GetDiscounts().Should().BeEmpty();
            receipt.GetItems().Count.Should().Be(1);

            var receiptItem = receipt.GetItems()[0];
            receiptItem.Product.Should().Be(apples);
            receiptItem.Price.Should().Be(1.99);
            receiptItem.TotalPrice.Should().Be(2.5 * 1.99);
            receiptItem.Quantity.Should().Be(2.5);
        }
    }
}