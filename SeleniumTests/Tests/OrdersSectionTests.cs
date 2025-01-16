using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using OnlineStore.Web;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using SeleniumTests.Helpers;

using NUnit.Framework;
using OnlineStore.Web;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumTests.Helpers;

namespace SeleniumTests.Tests
{
    public class OrdersSectionTests
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
            _helper.NavigateToUrl("Products");
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

      
    }
}
