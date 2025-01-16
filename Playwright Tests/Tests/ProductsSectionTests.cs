using Microsoft.Playwright;
using NUnit.Framework;
using OnlineStore.Web;
using System.Threading.Tasks;

namespace PlaywrightTests.Tests
{
    public class ProductsSectionTests
    {
        private static CustomWebApplicationFactory<TestStartup> _systemUnderTest;
        private static string BaseUrl;
        private IPlaywright _playwright;
        private IBrowser _browser;
        private IPage _page;
        private const string BasePath = "/Products";

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            if (_systemUnderTest == null)
            {
                _systemUnderTest = new CustomWebApplicationFactory<TestStartup>();
                BaseUrl = _systemUnderTest.GetServerAddress();
            }

            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
            var context = await _browser.NewContextAsync();
            _page = await context.NewPageAsync();
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            // Dispose of the browser properly
            if (_browser != null)
            {
                await _browser.DisposeAsync();
            }

            _systemUnderTest?.Dispose();
        }

        [SetUp]
        public async Task SetUp()
        {
            if (_page.IsClosed)
            {
                // Ensure the page is open before navigating
                var context = await _browser.NewContextAsync();
                _page = await context.NewPageAsync();
            }
            await _page.GotoAsync($"{BaseUrl}{BasePath}");
        }

        [TearDown]
        public async Task TearDown()
        {
            // Properly close the page after each test to avoid premature closure errors
            if (_page != null && !_page.IsClosed)
            {
                await _page.CloseAsync();
            }
        }

        [Test]
        public async Task TestCreateNewButton()
        {
            await _page.ClickAsync("text=Create New");
            Assert.AreEqual($"{BaseUrl}{BasePath}/Create", _page.Url);
        }

        [Test]
        public async Task TestBackButton()
        {
            await _page.GotoAsync($"{BaseUrl}{BasePath}/Create");
            await _page.ClickAsync("text=Back to List");
            Assert.AreEqual($"{BaseUrl}{BasePath}", _page.Url);
        }

        [Test]
        public async Task TestCreate()
        {
            await _page.GotoAsync($"{BaseUrl}/Categories/Create");

            await _page.FillAsync("input[name='Name']", "Test Category 2");
            await _page.ClickAsync("input[name='Create']");

            await _page.GotoAsync($"{BaseUrl}{BasePath}/Create");

            // Fill in the fields for the product
            await _page.FillAsync("input[name='Name']", "Keczup");
            await _page.FillAsync("input[name='Price']", "12");
            await _page.FillAsync("textarea[name='ProductDetail.Description']", "Pyszny keczup Pudliszki");
            await _page.FillAsync("textarea[name='ProductDetail.Specifications']", "W 99% z pomidorów");

            // Click the 'Create' button to submit the form
            await _page.ClickAsync("button[name='Create']");
            
        }
        [Test]
        public async Task TestDeleteButtonBack()
        {
            // Go to the delete page for the product
            await _page.GotoAsync($"{BaseUrl}{BasePath}/Delete/1");

            // Wait for the 'Back to List' button to be visible
            var backButton = _page.Locator("text=Back to List");
            await backButton.WaitForAsync();

            // Click the 'Back to List' button
            await backButton.ClickAsync();

            // Ensure the URL has returned to the products list page
            Assert.IsTrue(_page.Url.StartsWith($"{BaseUrl}{BasePath}"));
        }

        

        [Test]
        public async Task TestzDeleteButtonDelete()
        {
            await _page.GotoAsync($"{BaseUrl}{BasePath}/Delete/1");
            await _page.ClickAsync("text=Delete");
            Assert.IsTrue(_page.Url.StartsWith($"{BaseUrl}{BasePath}"));
        }

        [Test]
        public async Task TestDetailsButton()
        {
            await _page.ClickAsync("text=Details >> nth=0");
            Assert.IsTrue(_page.Url.StartsWith($"{BaseUrl}{BasePath}/Details"));
        }

        [Test]
        public async Task TestDetailsButtonBack()
        {
            await _page.GotoAsync($"{BaseUrl}{BasePath}/Details/1");
            await _page.ClickAsync("text=Back to List");
            Assert.IsTrue(_page.Url.StartsWith($"{BaseUrl}{BasePath}"));
        }

        [Test]
        public async Task TestEditButton()
        {
            await _page.ClickAsync("text=Edit >> nth=0"); // Adjust nth index if needed
            Assert.IsTrue(_page.Url.StartsWith($"{BaseUrl}{BasePath}/Edit"));
        }

        [Test]
        public async Task TestEditButtonSave()
        {
            await _page.GotoAsync($"{BaseUrl}{BasePath}/Edit/1");
            await _page.ClickAsync("text=Save");
            Assert.IsTrue(_page.Url.StartsWith($"{BaseUrl}{BasePath}"));
        }

        [Test]
        public async Task TestEditButtonBack()
        {
            await _page.GotoAsync($"{BaseUrl}{BasePath}/Edit/1");
            await _page.ClickAsync("text=Back to List");
            Assert.IsTrue(_page.Url.StartsWith($"{BaseUrl}{BasePath}"));
        }


    }
}
