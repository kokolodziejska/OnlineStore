using NUnit.Framework;
using OnlineStore.Web;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumTests.Helpers;

namespace SeleniumTests.Tests
{
    public class CategoriesSectionTests
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
            _helper.NavigateToUrl ("Categories");
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

        [Test]
        public void TestCreateNewButton()
        {
            _helper.ClickButtonByName("Create New");
            _helper.AssertCurrentUrl("Categories/Create");
        }

        [Test]
        public void TestBackButton()
        {
            _helper.NavigateToRelativeUrl("Create");
            _helper.ClickButtonByName("Back to List");
            _helper.AssertCurrentUrl("Categories");
        }

        [Test]
        public void TestCreate()
        {
            _helper.NavigateToRelativeUrl("Create");

            _helper.EnterTextInField("Name", "Test Category 2");
            _helper.ClickButtonByName("Create");

            _helper.AssertCurrentUrl("Categories");
            _helper.AssertRowExistsByText("Test Category 2");
        }

        [Test]
        public void TestEmptyFormSubmission()
        {
            _helper.NavigateToRelativeUrl("Create");
            _helper.ClickButtonByName("Create");
            _helper.AssertValidationMessage("The Name field is required.");
        }

        [Test]
        public void TestEditButton()
        {
            _helper.NavigateToUrl("Categories");
            _helper.ClickButtonInRow("Test Category", "Edit");
            _helper.AssertCurrentUrl("Categories/Edit/1");
        }

        [Test]
        public void TestEditButtonSave()
        {
            _helper.NavigateToUrl("Categories/Edit/1");
            _helper.ClickButtonByName("Save");
            _helper.AssertCurrentUrl("Categories");
        }

        [Test]
        public void TestEditButtonBack()
        {
            _helper.NavigateToUrl("Categories/Edit/1");
            _helper.ClickButtonByName("Back to List");
            _helper.AssertCurrentUrl("Categories");
        }

        [Test]
        public void TestDetailsButton()
        {
            _helper.ClickButtonInRow("Test Category", "Details");
            _helper.AssertCurrentUrl("Categories/Details/1");
        }

        [Test]
        public void TestDetailButtonBack()
        {
            _helper.NavigateToUrl("Categories/Details/1");
            _helper.ClickButtonByName("Back to List");
            _helper.AssertCurrentUrl("Categories");
        }

        [Test]
        public void TestDetailButtonEdit()
        {
            _helper.NavigateToUrl("Categories/Details/1");
            _helper.ClickButtonByName("Edit");
            _helper.AssertCurrentUrl("Categories/Edit/1");
        }


        [Test]
        public void TestDeleteButton()
        {
            _helper.ClickButtonInRow("Test Category", "Delete");
            _helper.AssertCurrentUrl("Categories/Delete/1");

        }
        [Test]
        public void TestDeleteButtonBack()
        {
            _helper.NavigateToUrl("Categories/Delete/1");
            _helper.ClickButtonByName("Back to List");
            _helper.AssertCurrentUrl("Categories");

        }
        [Test]
        public void TestzDeleteButtonDelete()
        {
            _helper.NavigateToUrl("Categories/Delete/1");
            _helper.ClickButtonByName("Delete");
            _helper.AssertCurrentUrl("Categories");

        }
        [Test]
        public void TestCreate2()
        {
            _helper.NavigateToRelativeUrl("Create");

            _helper.EnterTextInField("Name", "Test Category 2");
            _helper.ClickButtonByName("Create");

            _helper.AssertCurrentUrl("Categories");
            _helper.AssertRowExistsByText("Test Category 2");
        }
    }
}
