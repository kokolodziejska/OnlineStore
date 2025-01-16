using Microsoft.Playwright;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using System.Threading.Tasks;
using OnlineStore.Web; // Replace with your actual app namespace

namespace PlaywrightTests.Tests
{
    [TestFixture]
    public class CategoriesSectionTests
    {
        private static CustomWebApplicationFactory<TestStartup> _systemUnderTest;
        private static string BaseUrl;
        private IPlaywright _playwright;
        private IBrowser _browser;
        private IPage _page;
        private const string BasePath = "/Categories";

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
            await _page.GotoAsync($"{BaseUrl}{BasePath}/Create");

            await _page.FillAsync("input[name='Name']", "Test Category 2");
            await _page.ClickAsync("input[name='Create']");
            

            //Assert.AreEqual($"{BaseUrl}{BasePath}/Create", _page.Url);
            //Assert.IsTrue(await _page.Locator("text=Test Category 2").IsVisibleAsync());
        }

     
        public async Task TestEmptyFormSubmission()
        {
            await _page.GotoAsync($"{BaseUrl}{BasePath}/Create");

            // Click the submit button to trigger validation
            await _page.ClickAsync("text=Create");

            // Wait for the validation message with increased timeout
            var validationMessage = _page.Locator(".validation-summary-errors");

            // Explicitly wait for the validation message to appear
            await validationMessage.WaitForAsync(new LocatorWaitForOptions { Timeout = 2000 }); // Increase timeout to 60 seconds

            // Assert the validation message text
            var actualMessage = await validationMessage.InnerTextAsync();
            Assert.AreEqual("The Name field is required.", actualMessage.Trim());
        }

        [Test]
        public async Task TestEditButton()
        {
            await _page.GotoAsync($"{BaseUrl}{BasePath}");
            await _page.ClickAsync("text=Edit >> nth=0"); // Adjust nth index if needed
            Assert.IsTrue(_page.Url.StartsWith($"{BaseUrl}{BasePath}/Edit"));
        }

        [Test]
        public async Task TestEditButtonEdit()
        {
            await _page.GotoAsync($"{BaseUrl}{BasePath}/Edit/1");
            await _page.ClickAsync("text=Edit");
            Assert.IsTrue(_page.Url.StartsWith($"{BaseUrl}{BasePath}"));
        }

        [Test]
        public async Task TestEditButtonBack()
        {
            await _page.GotoAsync($"{BaseUrl}{BasePath}/Edit/1");
            await _page.ClickAsync("text=Back to List");
            Assert.IsTrue(_page.Url.StartsWith($"{BaseUrl}{BasePath}"));
        }

        [Test]
        public async Task TestDetailsButton()
        {
            await _page.ClickAsync("text=Details >> nth=0"); // Adjust nth index if needed
            Assert.IsTrue(_page.Url.StartsWith($"{BaseUrl}{BasePath}/Details"));
        }

        [Test]
        public async Task TestDeleteButton()
        {
            await _page.ClickAsync("text=Delete >> nth=0"); // Adjust nth index if needed
            Assert.IsTrue(_page.Url.StartsWith($"{BaseUrl}{BasePath}/Delete"));
        }

        [Test]
        public async Task TestDetailButtonEdit()
        {
            await _page.GotoAsync($"{BaseUrl}{BasePath}/Details/1");
            await _page.ClickAsync("text=Edit");
            Assert.IsTrue(_page.Url.StartsWith($"{BaseUrl}{BasePath}/Edit"));
        }


        [Test]
        public async Task TestDeleteButtonBack()
        {
            await _page.GotoAsync($"{BaseUrl}{BasePath}/Delete/1");
            await _page.ClickAsync("text=Back to List");
            Assert.IsTrue(_page.Url.StartsWith($"{BaseUrl}{BasePath}"));
        }

        [Test]
        public async Task TestzDeleteButtonDelete()
        {
            await _page.GotoAsync($"{BaseUrl}{BasePath}/Delete/1");
            await _page.ClickAsync("input[name='Delete']");
            Assert.IsTrue(_page.Url.StartsWith($"{BaseUrl}{BasePath}"));
        }

    


    }
}
