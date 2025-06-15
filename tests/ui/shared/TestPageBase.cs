using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Appium.Interfaces;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.iOS;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Support.UI;

namespace UITests;

public abstract class TestPageBase
{
    
    protected AppiumDriver App => AppiumSetup.App;
    
    abstract protected string PageName { get; }

    protected By GetBy(string id) => App switch
    {
        WindowsDriver _ => MobileBy.AccessibilityId(id),
        _ => MobileBy.Id(id),
    };

     protected By GetByText(string id) => App switch
    {
        AndroidDriver _ => MobileBy.AndroidUIAutomator($"new UiSelector().text(\"{id}\")"),
        
        _ => MobileBy.LinkText(id),
    };

    protected IWebElement GetElement(string id) => 
        App.FindElement(GetBy(id));

    public IWebElement FindByTagName(string tagName) =>
        App.FindElement(MobileBy.TagName("email"));
    public IWebElement PageElement() => GetElement(PageName);
    public void VerifyPageShown() => Assert.That(PageElement(), Is.Not.Null);


}

public enum Platform
{
    Windows,
    iOS,
    Android,
}
