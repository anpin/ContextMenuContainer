using NUnit.Framework;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Appium.Windows;

namespace UITests;


[SetUpFixture]
public class AppiumSetup
{
    public const string iosApplication = "APES.MAUI.Sample";

    static string slnRoot =>
        Path.GetRelativePath(Environment.CurrentDirectory,  string.Concat(Enumerable.Repeat($"..{Path.DirectorySeparatorChar}", 6) ));
    static string appPath =>
        Environment.GetEnvironmentVariable("IOS_APP_PATH") ??
        Path.GetFullPath(Path.Combine(
            slnRoot,
            "sample", 
            "bin",
            "Release", 
            "net9.0-ios",
            "iossimulator-x64",
            $"{iosApplication}.app"));
    private static AppiumDriver? driver;

    public static AppiumDriver App => driver ?? throw new NullReferenceException("AppiumDriver is null");

    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
        
        var capabilities = new AppiumOptions();
        capabilities.AutomationName = AutomationName.iOSXcuiTest;
        capabilities.DeviceName = Environment.GetEnvironmentVariable("DEVICE_NAME") ?? "iPhone SE (3rd generation)";
        capabilities.App =  appPath;
        driver = new IOSDriver(capabilities);
    }

    [OneTimeTearDown]
    public void RunAfterAnyTests()
    {
        driver?.Quit();

        // AppiumServerHelper.DisposeAppiumLocalServer();
    }
}
