using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using OnlineStore.Web.Controllers;
using OnlineStore.Web.Data;
using OnlineStore.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace LoadTests.Tests
{
    [TestFixture]
    public class ProductsTests
    {
        private OnlineStoreContext _mockContext;
        private ProductsController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<OnlineStoreContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _mockContext = new OnlineStoreContext(options);
            _controller = new ProductsController(_mockContext);

            _mockContext.Products.Add(new Product { Name = "Test Product", Price = 10.99M, CategoryId = 1 });
            _mockContext.SaveChanges();
        }

        [Test]
        public async Task Create_TooManyProducts_ShouldReturnRedirectToIndex()
        {
            for (int i = 0; i < 10000; i++)
            {
                _mockContext.Products.Add(new Product { Name = $"Product {i}", Price = 10, CategoryId = 1 });
            }
            await _mockContext.SaveChangesAsync();

            var product = new Product { Name = "New Product", Price = 19.99M, CategoryId = 1 };
            var result = await _controller.Create(product);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectResult = result as RedirectToActionResult;
            Assert.AreEqual("Index", redirectResult.ActionName);
        }

        [Test]
        public async Task Create_ProductWithTooHighPrice_ShouldReturnViewWithErrors()
        {
            var product = new Product { Name = "Luxury Product", Price = 1000000M, CategoryId = 1 };
            var result = await _controller.Create(product);

            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.IsFalse(viewResult.ViewData.ModelState.IsValid);
            Assert.AreEqual("The price is too high.", viewResult.ViewData.ModelState["Price"].Errors.First().ErrorMessage);
        }

        [Test]
        public async Task Create_ProductWithNegativePrice_ShouldReturnViewWithErrors()
        {
            var product = new Product { Name = "Negative Price Product", Price = -10.99M, CategoryId = 1 };
            var result = await _controller.Create(product);

            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.IsFalse(viewResult.ViewData.ModelState.IsValid);
            Assert.AreEqual("The price cannot be negative.", viewResult.ViewData.ModelState["Price"].Errors.First().ErrorMessage);
        }

        [Test]
        public async Task Create_ProductWithTooLongName_ShouldReturnViewWithErrors()
        {
            var product = new Product
            {
                Name = new string('A', 256),
                Price = 19.99M,
                CategoryId = 1
            };

            var result = await _controller.Create(product);

            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.IsFalse(viewResult.ViewData.ModelState.IsValid);
            var errorMessage = viewResult.ViewData.ModelState["Name"].Errors.FirstOrDefault()?.ErrorMessage;
            Assert.AreEqual("The Name cannot be empty or longer than 255 characters.", errorMessage);
        }

        [Test]
        public async Task Create_ProductWithTooLargeCategoryId_ShouldReturnViewWithErrors()
        {
            var product = new Product
            {
                Name = "Test Product",
                Price = 19.99M,
                CategoryId = 9999
            };

            var result = await _controller.Create(product);

            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.IsFalse(viewResult.ViewData.ModelState.IsValid);
            var errorMessage = viewResult.ViewData.ModelState["CategoryId"].Errors.FirstOrDefault()?.ErrorMessage;
            Assert.AreEqual("Invalid CategoryId.", errorMessage);
        }

        [Test]
        public async Task Create_ProductWithNullProductDetail_ShouldReturnViewWithErrors()
        {
            var product = new Product
            {
                Name = "Test Product",
                Price = 19.99M,
                CategoryId = 1,
                ProductDetail = null
            };

            var result = await _controller.Create(product);

            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.IsFalse(viewResult.ViewData.ModelState.IsValid);
            var errorMessage = viewResult.ViewData.ModelState["ProductDetail"].Errors.FirstOrDefault()?.ErrorMessage;
            Assert.AreEqual("ProductDetail is required.", errorMessage);
        }

        [Test]
        public async Task Create_ProductDetailWithTooLongDescription_ShouldReturnViewWithErrors()
        {
            var product = new Product
            {
                Name = "Test Product",
                Price = 19.99M,
                CategoryId = 1,
                ProductDetail = new ProductDetail
                {
                    Description = new string('A', 1001),
                }
            };

            var result = await _controller.Create(product);

            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.IsFalse(viewResult.ViewData.ModelState.IsValid);
            var errorMessage = viewResult.ViewData.ModelState["ProductDetail.Description"].Errors.FirstOrDefault()?.ErrorMessage;
            Assert.AreEqual("The Description cannot be longer than 1000 characters.", errorMessage);
        }

        [TearDown]
        public void TearDown()
        {
            _controller.Dispose();
            _mockContext.Dispose();
        }
    }
}
