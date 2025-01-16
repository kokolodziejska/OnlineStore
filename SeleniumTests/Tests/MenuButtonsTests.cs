using NUnit.Framework;
using OnlineStore.Web;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumTests.Helpers;

namespace SeleniumTests.Tests
{
    public class MenuButtonsTests
    {
        private CustomWebApplicationFactory<TestStartup> _systemUnderTest;
        private IWebDriver _driver;
        private WebDriverHelper _helper;

        [OneTimeSetUp]
        public void OnTestInitialize()
        {
            _systemUnderTest = new CustomWebApplicationFactory<TestStartup>();
        }

        [OneTimeTearDown]
        public void OnTestCleanup()
        {
            _systemUnderTest?.Dispose();
        }

        [SetUp]
        public void SetUp()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless");

            _driver = new ChromeDriver(chromeOptions);
            _driver.Manage().Window.Maximize();
            string baseAddress = _systemUnderTest.GetServerAddress();
            _helper = new WebDriverHelper(_driver, baseAddress);
        }

        [TearDown]
        public void TearDown()
        {
            if (_driver != null)
            {
                try
                {
                    _driver.Quit();
                }
                finally
                {
                    _driver.Dispose();
                }
            }
        }

        public void TestMenuNavigation(string buttonName, string expectedRelativeUrl)
        {
            _helper.NavigateToHomePage();
            _helper.ClickMenuButton(buttonName);
            _helper.WaitForUrlContains(expectedRelativeUrl);
            _helper.AssertCurrentUrl(expectedRelativeUrl);
        }
        [Test]
        public void TestCategoriesButton()
        {
            TestMenuNavigation("Categories", "Categories");
        }

        [Test]
        public void TestProductsButton()
        {
            TestMenuNavigation("Products", "Products");
        }

        [Test]
        public void TestOrdersButton()
        {
            TestMenuNavigation("Orders", "Orders");
        }
    }
}
