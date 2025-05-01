using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.iOS;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Support.UI;

namespace UITests;

public class LoginPageTest : TestPageBase
{
    protected override string PageName => "container1"; 
    

    [Test]
    public void Login()
    {
        if (PageElement().Displayed)
        {
            App.GetScreenshot().SaveAsFile($"{nameof(Login)}.png");
        }
        else
        {
            Assert.Fail("Unexpected page");
        }
    }
}
