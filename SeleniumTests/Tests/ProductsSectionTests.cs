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
    public class ProductsSectionTests
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

        [Test]
        public void TestCreateNewButton()
        {
            _helper.ClickButtonByName("Create New");
            _helper.AssertCurrentUrl("Products/Create");
        }

        [Test]
        public void TestBackButton()
        {
            _helper.NavigateToRelativeUrl("Create");
            _helper.ClickButtonByName("Back to List");
            _helper.AssertCurrentUrl("Products");
        }

        [Test]
        public void TestEmptyFormSubmissionSpecifications()
        {
            _helper.NavigateToRelativeUrl("Create");
            _helper.EnterTextInField("Name", "Keczup");
            _helper.EnterTextInField("Price", "12");
            _helper.EnterTextInField("ProductDetail.Description", "Pyszny keczup Pudliszki");
            _helper.ClickButtonByName("Create");
            _helper.AssertValidationMessageSpecifications("Specifications are required.");
        }


        [Test]
        public void TestEmptyFormDescription()
        {
            _helper.NavigateToRelativeUrl("Create");
            _helper.EnterTextInField("Name", "Keczup");
            _helper.EnterTextInField("Price", "12");
            _helper.EnterTextInField("ProductDetail.Specifications", "W 99% z pomidorów");
            _helper.ClickButtonByName("Create");
            _helper.AssertValidationMessageDescription("Description is required.");
        }

        [Test]
        public void TestEmptyFormPrice()
        {
            _helper.NavigateToRelativeUrl("Create");

            
            _helper.EnterTextInField("Name", "Keczup");
            _helper.EnterTextInField("ProductDetail.Description", "Pyszny keczup Pudliszki");
            _helper.EnterTextInField("ProductDetail.Specifications", "W 99% z pomidorów");

           
            _helper.EnterTextInField("Price", "");
            _helper.ClickButtonByName("Create");

            
            _helper.AssertValidationMessagePrice("The value '' is invalid.");  

            
            _helper.EnterTextInField("Price", "-1");
            _helper.ClickButtonByName("Create");

            
            _helper.AssertValidationMessagePrice("Price must be greater than zero.");
        }


        [Test]
        public void TestEmptyFormName()
        {
            _helper.NavigateToRelativeUrl("Create");
            _helper.EnterTextInField("Price", "12");
            _helper.EnterTextInField("ProductDetail.Description", "Pyszny keczup Pudliszki");
            _helper.EnterTextInField("ProductDetail.Specifications", "W 99% z pomidorów");
            _helper.ClickButtonByName("Create");
            _helper.AssertValidationMessageName("Name is required.");
        }

        [Test]
        public void TestCreate()
        {
            

            _helper.NavigateToRelativeUrl("Create");
            _helper.EnterTextInField("Name", "Keczup");
            _helper.EnterTextInField("Price", "12");
            _helper.EnterTextInField("ProductDetail.Description", "Pyszny keczup Pudliszki");
            _helper.EnterTextInField("ProductDetail.Specifications", "W 99% z pomidorów");
            _helper.ClickButtonByName("Create");

            _helper.AssertCurrentUrl("Products");
            _helper.AssertRowExistsByText("Keczup");

        }

        [Test]
        public void TestEditButton()
        {
            _helper.AssertCurrentUrl("Products");
            _helper.ClickButtonInRow("Test Category", "Edit");
            _helper.AssertCurrentUrl("Products/Edit/1");
        }

        [Test]
        public void TestEditButtonSave()
        {
            _helper.NavigateToUrl("Products/Edit/1");
            _helper.ClickButtonByName("Save");
            _helper.AssertCurrentUrl("Products");
        }

        [Test]
        public void TestEditButtonBack()
        {
            _helper.NavigateToUrl("Products/Edit/1");
            _helper.ClickButtonByName("Back to List");
            _helper.AssertCurrentUrl("Products");
        }



        [Test]
        public void TestDetailsButton()
        {
            _helper.AssertCurrentUrl("Products");
            _helper.ClickButtonInRow("Test Category","Details");
            _helper.AssertCurrentUrl("Products/Details/1");
        }

    

        [Test]
        public void TestDetailsButtonBack()
        {
            _helper.NavigateToUrl("Products/Details/1");
            _helper.ClickButtonByName("Back to List");
            _helper.AssertCurrentUrl("Products");
        }

        [Test]
        public void TestDeleteButton()
        {
            _helper.AssertCurrentUrl("Products");
            _helper.ClickButtonInRow("Test Category", "Delete");
            _helper.AssertCurrentUrl("Products/Delete/1");
        }

        [Test]
        public void TestDeleteButtonBack()
        {
            _helper.NavigateToUrl("Products/Delete/1");
            _helper.ClickButtonByName("Back to List");
            _helper.AssertCurrentUrl("Products");
        }
        [Test]
        public void TestzDeleteButtonDelete()
        {
            _helper.NavigateToUrl("Products/Delete/1");
            _helper.ClickButtonByName("Delete");
            _helper.AssertCurrentUrl("Products");
        }



    }
}
