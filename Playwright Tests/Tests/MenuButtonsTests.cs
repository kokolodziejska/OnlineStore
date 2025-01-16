using NUnit.Framework;
using Microsoft.Playwright;
using System.Threading.Tasks;
using OnlineStore.Web;

namespace PlaywrightTests.Tests
{
    public class MenuButtonsTests
    {
        private static CustomWebApplicationFactory<TestStartup> _systemUnderTest;
        private static string BaseUrl;
        private IPlaywright _playwright;
        private IBrowser _browser;
        private IPage _page;
        private const string BasePath = "/";

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            if (_systemUnderTest == null)
            {
                // Create the factory only once
                _systemUnderTest = new CustomWebApplicationFactory<TestStartup>();
                BaseUrl = _systemUnderTest.GetServerAddress();
            }

            _playwright = await Playwright.CreateAsync();

            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true
            });

            var context = await _browser.NewContextAsync();
            _page = await context.NewPageAsync();
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            await _browser.DisposeAsync();
            _systemUnderTest.Dispose();
        }

        [SetUp]
        public async Task SetUp()
        {
            await _page.GotoAsync($"{BaseUrl}{BasePath}");
        }

        // Updated version of TestMenuNavigation using Playwright
        public async Task TestMenuNavigation(string buttonName, string expectedRelativeUrl)
        {
            // Wait for the button to be available
            var button = _page.Locator($"text={buttonName}");
            

            // Click the button
            await button.ClickAsync();

            // Assert that the URL is correct
            Assert.IsTrue(_page.Url.Contains(expectedRelativeUrl), $"Expected URL to contain {expectedRelativeUrl}, but got: {_page.Url}");
        }

        [Test]
        public async Task TestCategoriesButton()
        {
            await TestMenuNavigation("Categories", "Categories");
            
        }

        [Test]
        public async Task TestProductsButton()
        {
            await TestMenuNavigation("Products", "Products");
           
        }

        [Test]
        public async Task TestOrdersButton()
        {
            await TestMenuNavigation("Orders", "Orders");
            
        }
    }
}

