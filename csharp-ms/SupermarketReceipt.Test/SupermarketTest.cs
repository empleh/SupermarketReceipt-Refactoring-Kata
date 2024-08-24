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
        SupermarketCatalog catalog;
        Product toothbrush;
        Product apples;
        Product rice;

        [TestInitialize]
        public void BeforeEachTest()
        {
            catalog = new FakeCatalog();
            toothbrush = new Product("toothbrush", ProductUnit.Each);
            rice = new Product("rice", ProductUnit.Each);
            apples = new Product("apples", ProductUnit.Kilo);
        }

        private ShoppingCart SetupStandardCart()
        {
            catalog.AddProduct(toothbrush, 1);
            catalog.AddProduct(apples, 2);
            catalog.AddProduct(rice, 1);

            var cart = new ShoppingCart();
            cart.AddItemQuantity(apples, 2.5);
            cart.AddItemQuantity(toothbrush, 1);
            cart.AddItemQuantity(rice, 3);

            return cart;
        }

        [TestMethod]
        public void NoDiscounts()
        {
            // ARRANGE
            var cart = SetupStandardCart();
            var teller = new Teller(catalog);
            // ACT
            var receipt = teller.ChecksOutArticlesFrom(cart);

            // ASSERT
            VerifyTotals(receipt, 9, 0, 3);

            VerifyItem(receipt, apples, 0, 2, 5, 2.5);
            VerifyItem(receipt, toothbrush, 1, 1, 1, 1);
            VerifyItem(receipt, rice, 2, 1, 3, 3);
        }

        private static void VerifyTotals(Receipt receipt, double totalPrice, int discountCount, int itemCount)
        {
            receipt.GetTotalPrice().Should().Be(totalPrice);
            receipt.GetDiscounts().Count.Should().Be(discountCount);
            receipt.GetItems().Count.Should().Be(itemCount);
        }

        [TestMethod]
        public void TenPercentDiscount()
        {
            // ARRANGE
            var cart = SetupStandardCart();

            var teller = new Teller(catalog);
            teller.AddSpecialOffer(SpecialOfferType.TenPercentDiscount, toothbrush, 10.0);

            // ACT
            var receipt = teller.ChecksOutArticlesFrom(cart);

            // ASSERT
            VerifyTotals(receipt, 8.9, 1, 3);
            VerifyDiscount(receipt, toothbrush, 0, "10% off", -0.1);
        }

        [TestMethod]
        public void ThreeForTwoDiscount()
        {
            // ARRANGE
            var cart = SetupStandardCart();

            var teller = new Teller(catalog);
            teller.AddSpecialOffer(SpecialOfferType.ThreeForTwo, rice, 1);

            // ACT
            var receipt = teller.ChecksOutArticlesFrom(cart);

            // ASSERT
            VerifyTotals(receipt, 8, 1, 3);
            VerifyDiscount(receipt, rice, 0, "3 for 2", -1);
        }

        private static void VerifyDiscount(Receipt receipt, Product product, int index, string description, double discountAmount)
        {
            var discount = receipt.GetDiscounts()[index];
            discount.Product.Should().Be(product);
            discount.Description.Should().Be(description);
            discount.DiscountAmount.Should().Be(discountAmount);
        }

        private static void VerifyItem(Receipt receipt, Product product, int index, double perPrice, double totalPrice, double quantity)
        {
            var receiptItem = receipt.GetItems()[index];
            receiptItem.Product.Should().Be(product);
            receiptItem.Price.Should().Be(perPrice);
            receiptItem.TotalPrice.Should().Be(totalPrice);
            receiptItem.Quantity.Should().Be(quantity);
        }
    }
}