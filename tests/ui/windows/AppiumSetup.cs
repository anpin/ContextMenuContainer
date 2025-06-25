using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;

namespace UITests;

[SetUpFixture]
public class AppiumSetup
{
    public const string windowsApplication = "APES.MAUI.Sample";
	
	static string slnRoot =>
        Path.GetRelativePath(Environment.CurrentDirectory,  string.Concat(Enumerable.Repeat($"..{Path.DirectorySeparatorChar}", 6) ));
    static string appPath =>
        Environment.GetEnvironmentVariable("WINDOWS_APP_PATH") ??
        Path.GetFullPath(Path.Combine(
            slnRoot,
            "sample", 
            "bin",
            "Release", 
            "net9.0-windows10.0.19041.0", 
			"win-x64", 
			"publish",
            $"{windowsApplication}.exe"));
	private static AppiumDriver? driver;

	public static AppiumDriver App => driver ?? throw new NullReferenceException("AppiumDriver is null");

	[OneTimeSetUp]
	public void RunBeforeAnyTests()
	{
		// If you started an Appium server manually, make sure to comment out the next line
		// This line starts a local Appium server for you as part of the test run
		AppiumServerHelper.StartAppiumLocalServer();

		var windowsOptions = new AppiumOptions
		{
			// Specify windows as the driver, typically don't need to change this
			AutomationName = "windows",
			// Always Windows for Windows
			PlatformName = "Windows",
			// The identifier of the deployed application to test
			// App = "f0276ba8-3171-4a5d-9410-ac37ee234b34_9zz4h110yvjzm!App",
			App = appPath,
		};

		// Note there are many more options that you can use to influence the app under test according to your needs

		driver = new WindowsDriver(windowsOptions);
	}

	[OneTimeTearDown]
	public void RunAfterAnyTests()
	{
		driver?.Quit();

		// If an Appium server was started locally above, make sure we clean it up here
		// AppiumServerHelper.DisposeAppiumLocalServer();
	}
}
