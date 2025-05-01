using NUnit.Framework;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Appium.Windows;

namespace UITests;


[SetUpFixture]
public class AppiumSetup
{
    public const string AndroidApplication = "com.companyname.app";

    public const string AndroidApplicationActivity = $"{AndroidApplication}.MainActivity";
    static AppiumOptions? _appiumOptions = null;
    static AppiumOptions AppiumOptions
    {
        get
        {
            if (_appiumOptions == null)
            {
                _appiumOptions = new AppiumOptions();
                _appiumOptions.AddAdditionalAppiumOption(MobileCapabilityType.DeviceName, "Moto_G5_Plus__XT1685_");
                _appiumOptions.AddAdditionalAppiumOption(MobileCapabilityType.Udid, "ZY2247KNS6");
                _appiumOptions.AddAdditionalAppiumOption(MobileCapabilityType.PlatformName, "Android");
                _appiumOptions.AddAdditionalAppiumOption(AndroidMobileCapabilityType.AppPackage, "com.companyname.app");
                _appiumOptions.AddAdditionalAppiumOption(AndroidMobileCapabilityType.AppActivity, "crc6492a0fe1559608a27.MainActivity");
                _appiumOptions.AddAdditionalAppiumOption(AndroidMobileCapabilityType.AppWaitDuration, 10000);
            }
            return _appiumOptions;
        }
    }

    static string slnRoot =>
        Path.GetRelativePath(Environment.CurrentDirectory,  string.Concat(Enumerable.Repeat($"..{Path.DirectorySeparatorChar}", 4)));
    static string appPath => Path.GetFullPath(Path.Combine(
        slnRoot,
        "app",
        "bin",
        "Debug", 
        "net9.0-android",
        $"{AndroidApplication}.apk"));
    private static AppiumDriver? driver;

    public static AppiumDriver App => driver ?? throw new NullReferenceException("AppiumDriver is null");

    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
        
        var capabilities = new AppiumOptions();
        capabilities.AutomationName = AutomationName.AndroidUIAutomator2;
        capabilities.DeviceName = "Android Emulator";
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
