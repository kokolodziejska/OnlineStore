using Microsoft.Playwright;
using System.Threading.Tasks;

namespace PlaywrightTests.Helpers
{
    public class PlaywrightHelper
    {
        private readonly IPage _page;
        private readonly string _baseAddress;

        public PlaywrightHelper(IPage page, string baseAddress)
        {
            _page = page;
            _baseAddress = baseAddress;
        }

        // Navigation methods
        public async Task NavigateToUrl(string relativeUrl)
        {
            await _page.GotoAsync($"{_baseAddress}/{relativeUrl}");
        }

        public async Task NavigateToRelativeUrl(string relativeUrl)
        {
            await NavigateToUrl(relativeUrl);
        }

        // Element interaction methods
        public async Task ClickButtonByName(string buttonName)
        {
            var button = _page.Locator($"button[name='{buttonName}']");
            await button.ClickAsync();
        }

        public async Task EnterTextInField(string fieldName, string text)
        {
            var field = _page.Locator($"input[name='{fieldName}']");
            await field.FillAsync(text);
        }

        public async Task ClickButtonInRow(string rowText, string buttonName)
        {
            var button = _page.Locator($"//tr[td[contains(text(), '{rowText}')]]//a[@name='{buttonName}']");
            await button.ClickAsync();
        }

        // Assertion methods
        public async Task AssertCurrentUrl(string expectedRelativeUrl)
        {
            var currentUrl = _page.Url;
            var expectedUrl = $"{_baseAddress}/{expectedRelativeUrl}";
            if (!string.Equals(currentUrl, expectedUrl, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception($"Expected URL: {expectedUrl}, but got: {currentUrl}");
            }
        }

        public async Task AssertRowExistsByText(string text, string errorMessage = "The specified row was not found.")
        {
            var row = _page.Locator($"//tr[td[contains(text(),'{text}')]]");
            var count = await row.CountAsync();
            if (count == 0)
            {
                throw new Exception(errorMessage);
            }
        }

        public async Task AssertValidationMessage(string expectedMessage)
        {
            var validationMessage = _page.Locator("span.text-danger");
            var actualMessage = await validationMessage.TextContentAsync();
            if (actualMessage?.Trim() != expectedMessage)
            {
                throw new Exception($"Validation message mismatch. Expected: '{expectedMessage}', but got: '{actualMessage}'");
            }
        }
    }
}
