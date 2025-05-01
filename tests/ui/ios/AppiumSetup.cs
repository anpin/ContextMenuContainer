using NUnit.Framework;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Appium.Windows;

namespace UITests;


[SetUpFixture]
public class AppiumSetup
{
    public const string iosApplication = "com.apes.ui.maui.sample";

    static string slnRoot =>
        Path.GetRelativePath(Environment.CurrentDirectory,  string.Concat(Enumerable.Repeat($"..{Path.DirectorySeparatorChar}", 4)));
    static string appPath => Path.GetFullPath(Path.Combine(
        slnRoot,
        "app",
        "bin",
        "Debug", 
        "net9.0-ios",
        $"{iosApplication}.ipa"));
    private static AppiumDriver? driver;

    public static AppiumDriver App => driver ?? throw new NullReferenceException("AppiumDriver is null");

    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
        
        var capabilities = new AppiumOptions();
        capabilities.AutomationName = AutomationName.iOSXcuiTest;
        capabilities.DeviceName = "iPhone SE (3rd generation) Simulator";
        capabilities.App =  "com.apes.ui.maui.sample";

        driver = new IOSDriver(capabilities);
    }

    [OneTimeTearDown]
    public void RunAfterAnyTests()
    {
        driver?.Quit();

        // AppiumServerHelper.DisposeAppiumLocalServer();
    }
}
