using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Appium.Interfaces;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.iOS;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using System;
using System.IO;
using System.Threading;
using NUnit.Framework;

namespace UITests;

public abstract class TestPageBase
{
    protected const string DefaultScreenshotsDir = "Screenshots";
    protected const string ScreenshotsDirEnvVar = "APPIUM_SCREENSHOTS_DIR";
    protected const string RecordingEnvVar = "APPIUM_RECORD_SCREEN";

    private static string? _screenshotsDir;
    protected static string ScreenshotsDir
    {
        get
        {
            if (_screenshotsDir == null)
            {
                string envDir = Environment.GetEnvironmentVariable(ScreenshotsDirEnvVar) ?? "";
                _screenshotsDir = !string.IsNullOrEmpty(envDir) ? envDir : DefaultScreenshotsDir;
                Directory.CreateDirectory(_screenshotsDir);
                Console.WriteLine("Screenshots would be saved to [{0}]", _screenshotsDir);
            }
            return _screenshotsDir;
        }
    }
    
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
        
    protected IWebElement ScrollToAndGetElement(string id)
    {
        try
        {
            // Try to find the element first without scrolling
            return GetElement(id);
        }
        catch (OpenQA.Selenium.NoSuchElementException)
        {
            // Element not found, try to scroll to it
            ScrollToElement(id);
            return GetElement(id);
        }
    }
    
    protected void ScrollToElement(string id)
    {
        switch (App)
        {
            case AndroidDriver androidDriver:
                try
                {
                    // Use Android UIAutomator2 to scroll to the element
                    string uiCommand = $"new UiScrollable(new UiSelector().scrollable(true)).scrollIntoView(new UiSelector().resourceId(\"{id}\"))";
                    androidDriver.FindElement(MobileBy.AndroidUIAutomator(uiCommand));
                    Console.WriteLine($"Scrolled to element: {id}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to scroll to element {id}: {ex.Message}");
                    // Try a general scroll down if the specific scroll fails
                    PerformGeneralScroll();
                }
                break;
            case IOSDriver iosDriver:
                PerformGeneralScroll();
                break;
            default:
                throw new NotImplementedException("Unsupported platform");
        }
    }
    protected void PerformGeneralScroll()
    {
        // Get screen dimensions
        var size = App.Manage().Window.Size;
        
        // Calculate scroll gesture (scroll up from bottom half to top half)
        int startX = size.Width / 2;
        int startY = (int)(size.Height * 0.7);
        int endX = size.Width / 2;
        int endY = (int)(size.Height * 0.3);
        
        // Perform the scroll
        try 
        {
            // Simple scrolling for both Android and iOS
            var actions = new OpenQA.Selenium.Interactions.Actions(App);
            actions.MoveToLocation(startX, startY)
                .ClickAndHold()
                .MoveByOffset(0, startY - endY)
                .Release()
                .Perform();
                
            Console.WriteLine("Performed general scroll");
            
            // Short wait to let scroll complete
            Thread.Sleep(1000);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to perform general scroll: {ex.Message}");
        }
    }

    public IWebElement FindByTagName(string tagName) =>
        App.FindElement(MobileBy.TagName("email"));
    public IWebElement PageElement() => GetElement(PageName);
    public void VerifyPageShown() => Assert.That(PageElement(), Is.Not.Null);

    public string StartScreenRecording()
    {
        if (!IsScreenRecordingEnabled())
        {
            return "";
        }
        
        
        return App switch
        {
            AndroidDriver _ => 
                App.StartRecordingScreen(new AndroidStartScreenRecordingOptions()),
            _ => 
                ""
        };
    }
    
    public string StopScreenRecording()
    {
        if (!IsScreenRecordingEnabled())
        {
            return "";
        }
        
        string recordingData = App switch
        {
            AndroidDriver _ => 
                App.StopRecordingScreen(new AndroidStopScreenRecordingOptions()),
            _ => 
                ""
        };
        
        if (!string.IsNullOrEmpty(recordingData))
        {
            SaveScreenRecording(recordingData);
        }
        
        return recordingData;
    }
    
    protected void SaveScreenshot(string filename)
    {
        string path = Path.Combine(ScreenshotsDir, filename);
        App.GetScreenshot().SaveAsFile(path);
    }
    
    private static void SaveScreenRecording(string base64String)
    {
        if (string.IsNullOrEmpty(base64String))
            return;
            
        string testName = "Unknown";
        if (TestContext.CurrentContext?.Test?.Name != null)
        {
            testName = TestContext.CurrentContext.Test.Name;
        }
        
        string filename = $"Recording_{testName}_{DateTime.Now:yyyyMMdd_HHmmss}.mp4";
        string path = Path.Combine(Directory.GetCurrentDirectory(), ScreenshotsDir, filename);
        
        
        byte[] bytes = Convert.FromBase64String(base64String);
        File.WriteAllBytes(path, bytes);
        
        Console.WriteLine($"Screen recording saved to: {path}");
    }
    // private static bool IsScreenRecordingEnabled() => true;
    private static bool IsScreenRecordingEnabled()
    {
        string value = Environment.GetEnvironmentVariable(RecordingEnvVar) ?? "";
        return !string.IsNullOrEmpty(value) && 
               (value.Equals("1") || value.Equals("true", StringComparison.OrdinalIgnoreCase));
    }
    


}

public enum Platform
{
    Windows,
    iOS,
    Android,
}
