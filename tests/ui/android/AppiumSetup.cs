using NUnit.Framework;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Appium.Windows;

namespace UITests;


[SetUpFixture]
public class AppiumSetup
{
    public const string AndroidApplication = "com.apes.ui.maui.sample";

    public const string AndroidApplicationActivity = $"{AndroidApplication}.MainActivity";
    
    static string slnRoot =>
        Path.GetRelativePath(Environment.CurrentDirectory,  string.Concat(Enumerable.Repeat($"..{Path.DirectorySeparatorChar}", 6) ));
    static string appPath =>
        Environment.GetEnvironmentVariable("ANDROID_APP_PATH") ??
        Path.GetFullPath(Path.Combine(
            slnRoot,
            "samples", 
            "APES.UI.MAUI.Sample",
            "bin",
            "Release", 
            "net9.0-android",
            $"{AndroidApplication}-Signed.apk"));
    private static AppiumDriver? driver;

    public static AppiumDriver App => driver ?? throw new NullReferenceException("AppiumDriver is null");

    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
        
        var capabilities = new AppiumOptions();
        capabilities.AutomationName = AutomationName.AndroidUIAutomator2;
        capabilities.DeviceName = "emulator-5554";
        capabilities.App = appPath;
        

        driver = new AndroidDriver(capabilities);
    }

    [OneTimeTearDown]
    public void RunAfterAnyTests()
    {
        driver?.Quit();

        // AppiumServerHelper.DisposeAppiumLocalServer();
    }
}
