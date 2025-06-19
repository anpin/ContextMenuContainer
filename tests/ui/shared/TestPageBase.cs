using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Appium.Interfaces;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.iOS;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Support.UI;
using System;
using System.IO;
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
