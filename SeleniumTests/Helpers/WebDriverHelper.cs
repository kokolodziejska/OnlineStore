using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;

namespace SeleniumTests.Helpers
{
    public class WebDriverHelper
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;
        private readonly string _baseAddress;

        public WebDriverHelper(IWebDriver driver, string baseAddress, int timeoutInSeconds = 10)
        {
            _driver = driver;
            _baseAddress = baseAddress;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(timeoutInSeconds));
        }

        // Navigation methods
        public void NavigateToRelativeUrl(string relativeUrl)
        {
            _driver.Navigate().GoToUrl($"{_driver.Url}/{relativeUrl}");
            WaitForUrlContains(relativeUrl);
        }

        public void NavigateToHomePage()
        {
            NavigateToUrl("");
        }

        public void NavigateToUrl(string relativeUrl)
        {
            _driver.Navigate().GoToUrl($"{_baseAddress}/{relativeUrl}");
        }

        public void WaitForUrlContains(string partialUrl)
        {
            _wait.Until(ExpectedConditions.UrlContains(partialUrl));
        }

        // Element interaction methods
        public IWebElement FindElement(By by)
        {
            return _wait.Until(ExpectedConditions.ElementIsVisible(by));
        }

        public void ClickButtonByName(string buttonName)
        {
            var button = FindElement(By.Name(buttonName));
            button.Click();
        }

        public void EnterTextInField(string fieldName, string text)
        {
            var field = FindElement(By.Name(fieldName));
            field.Clear(); // Optional: Clear the field before entering text
            field.SendKeys(text);
        }

        public void ClickButtonInRow(string rowText, string buttonName)
        {
            var button = FindElement(By.XPath($"//tr[td[contains(text(), '{rowText}')]]//a[@name='{buttonName}']"));
            button.Click();
        }

        // Assertion methods
        public void AssertCurrentUrl(string expectedRelativeUrl)
        {
            string currentUrl = _driver.Url;
            string expectedUrl = $"{_baseAddress}/{expectedRelativeUrl}";
            if (!string.Equals(currentUrl, expectedUrl, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception($"Expected URL: {expectedUrl}, but got: {currentUrl}");
            }
        }

        public void AssertValidationMessage(string expectedMessage)
        {
            var validationMessage = FindElement(By.CssSelector("span.text-danger"));
            string actualMessage = validationMessage.Text;
            if (!actualMessage.Equals(expectedMessage))
            {
                throw new Exception($"Validation message mismatch. Expected: '{expectedMessage}', but got: '{actualMessage}'");
            }
        }

        public void AssertValidationMessageSpecifications(string expectedMessage)
        {
            var validationMessage = FindElement(By.CssSelector("span.text-danger.field-validation-error[data-valmsg-for='ProductDetail.Specifications']"));
            string actualMessage = validationMessage.Text;
            if (!actualMessage.Equals(expectedMessage))
            {
                throw new Exception($"Validation message mismatch. Expected: '{expectedMessage}', but got: '{actualMessage}'");
            }
        }

        public void AssertValidationMessageDescription(string expectedMessage)
        {
            var validationMessage = FindElement(By.CssSelector("span.text-danger[data-valmsg-for='ProductDetail.Description']"));
            string actualMessage = validationMessage.Text;
            if (!actualMessage.Equals(expectedMessage))
            {
                throw new Exception($"Validation message mismatch. Expected: '{expectedMessage}', but got: '{actualMessage}'");
            }
        }

        public void AssertValidationMessagePrice(string expectedMessage)
        {
            try
            {
                // Wait for the validation message to be visible (use WebDriverWait)
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
                var validationMessage = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("span.text-danger[data-valmsg-for='Price']")));

                // Get the actual validation message text and trim any leading or trailing whitespace
                string actualMessage = validationMessage.Text.Trim();

                // Check if the actual message matches the expected message
                if (!actualMessage.Equals(expectedMessage))
                {
                    throw new Exception($"Validation message mismatch. Expected: '{expectedMessage}', but got: '{actualMessage}'");
                }
            }
            catch (NoSuchElementException)
            {
                // Handle the case where the validation message element is not found
                throw new Exception("Validation message for 'Price' not found.");
            }
            catch (Exception ex)
            {
                // General exception handling with detailed error message
                throw new Exception($"An error occurred while asserting the validation message: {ex.Message}");
            }
        }



        public void WaitForElement(By by, int timeoutInSeconds = 10)
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(timeoutInSeconds));
            wait.Until(driver => driver.FindElement(by).Displayed);
        }

        public string GetElementText(By by)
        {
            var element = _driver.FindElement(by);
            return element.Text.Trim();
        }

        public void AssertValidationMessageName(string expectedMessage)
        {
            var validationMessage = FindElement(By.CssSelector("span.text-danger[data-valmsg-for='Name']"));
            string actualMessage = validationMessage.Text;
            if (!actualMessage.Equals(expectedMessage))
            {
                throw new Exception($"Validation message mismatch. Expected: '{expectedMessage}', but got: '{actualMessage}'");
            }
        }


        public void AssertRowExistsByText(string text, string errorMessage = "The specified row was not found.")
        {
            var categoryRow = FindElement(By.XPath($"//tr[td[contains(text(),'{text}')]]"));
            if (categoryRow == null)
            {
                throw new Exception(errorMessage);
            }
        }

        // Menu navigation helper
        public void ClickMenuButton(string buttonName)
        {
            var button = FindElement(By.Name(buttonName));
            button.Click();
        }
    }
}
