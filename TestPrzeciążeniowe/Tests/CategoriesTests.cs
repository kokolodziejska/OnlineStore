using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using OnlineStore.Web.Controllers;
using OnlineStore.Web.Data;
using OnlineStore.Domain.Models;

namespace LoadTests.Tests
{
    [TestFixture]
    public class CategoriesControllerTests
    {
        private OnlineStoreContext _context;
        private CategoriesController _controller;

        [SetUp]
        public void Setup()
        {
            // Tworzenie opcji bazy danych w pamięci
            var options = new DbContextOptionsBuilder<OnlineStoreContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            // Tworzymy nowy kontekst bazy danych
            _context = new OnlineStoreContext(options);

            // Tworzymy kontroler z nowo utworzonym kontekstem bazy danych
            _controller = new CategoriesController(_context);

            // Dodajemy dane do bazy przed testem
            _context.Categories.AddRange(
                new Category { Name = "Category 1" },
                new Category { Name = "Category 2" }
            );
            _context.SaveChanges();
        }

        [Test]
        public async Task Create_LargeNumberOfCategories_ShouldHandleOverload()
        {
            // Przygotowanie dużej liczby kategorii do dodania
            var categories = Enumerable.Range(1, 1000).Select(i => new Category
            {
                Name = $"Category {i}"
            }).ToList();

            foreach (var category in categories)
            {
                // Próbujemy dodać kategorię
                var result = await _controller.Create(category);

                // Sprawdzamy, czy nie wystąpił błąd
                Assert.IsInstanceOf<RedirectToActionResult>(result);
            }

            // Sprawdzenie, czy wszystkie kategorie zostały zapisane w bazie danych
            var savedCategories = _context.Categories.ToList();
            Assert.AreEqual(1002, savedCategories.Count);
        }

        [TearDown]
        public void TearDown()

        {
            _context.Dispose();
            _controller.Dispose();
        }

        [Test]
        public async Task Create_SingleCategory_ShouldReturnRedirectToAction()
        {
            // Przygotowanie jednej kategorii
            var category = new Category { Name = "Test Category" };

            // Próbujemy dodać kategorię
            var result = await _controller.Create(category);

            // Sprawdzamy, czy zwrócony wynik to RedirectToAction (czyli poprawne przekierowanie)
            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }
    }
}
