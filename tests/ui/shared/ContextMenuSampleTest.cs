using System;
using System.Threading;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.iOS;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace UITests;

/// <summary>
/// Tests for the MAUI Sample app with ContextMenuContainer controls
/// </summary>
public class ContextMenuSampleTest : TestPageBase
{
    // We'll use container1 as the main identifier for the page
    protected override string PageName => "sample_page";

    [SetUp]
    public void Setup()
    {
        // Verify the main page is loaded and all containers are present
        Assert.That(PageElement().Displayed, Is.True, "Page should be visible");
        // Save initial state screenshot
        App.GetScreenshot().SaveAsFile("InitialState.png");
    }

    [Test]
    public void Container1_ContextMenuActions()
    {
        try
        {
            var container = GetElement("container1");
            Assert.That(container.Displayed, Is.True, "container1 should be visible");
            
            App.GetScreenshot().SaveAsFile("Container1_Before.png");
            
            var actions = new Actions(App);
            
            actions.ClickAndHold(container).Perform();
    
            var wait = new WebDriverWait(App, TimeSpan.FromSeconds(10));
            
            var actionItem =  wait.Until(d =>App.FindElement(GetByText("Action 1")));
            App.GetScreenshot().SaveAsFile("Container1_Menu.png");

            Assert.That(actionItem, Is.Not.Null, "Action 1 menu item should be visible");

            actionItem.Click();
            
            App.GetScreenshot().SaveAsFile("Container1_After.png");

            var actionResult = wait.Until(d =>  App.FindElement(GetByText("Action 1 pressed!")));
            Assert.That(actionResult, Is.Not.Null, "Action 1 should change text");

            Assert.Catch(() =>
            {
                Console.WriteLine("This should fail as item is not in DOM anymore : {displayed}",
                    actionItem.Displayed);
            });
        }
        catch (WebDriverException ex)
        {
            App.GetScreenshot().SaveAsFile("Container1_ContextMenu_Error.png");
            Assert.Fail($"Failed to interact with context menu: {ex.Message}");
        }
    }

    [Test]
    public void Container2_DynamicContextMenuItems()
    {
        // Find the container2 element
        var container = GetElement("container2");
        
        // Trigger context menu
        var actions = new Actions(App);
        actions.ClickAndHold(container).Perform();
        
        
        // Wait for context menu to appear
        var wait = new WebDriverWait(App, TimeSpan.FromSeconds(10));
        
        try {
            // Find and click on "Press me 1!" menu item
            var destructiveItem = wait.Until(d => App.FindElement(GetByText("Remove context actions"))); 
            destructiveItem.Click();
            
            App.GetScreenshot().SaveAsFile("Container2_Remove_Result.png");
            
            // Trigger context menu again
            actions.ClickAndHold(container).Perform();
            
            // Trigger context menu again
            actions.ClickAndHold(container).Perform();
            
            // Now there should be only one item - "Give me my actions back!"
            var restoreItem = wait.Until(d => App.FindElement(GetByText("Give me my actions back!"))); 
            restoreItem.Click();
            
            App.GetScreenshot().SaveAsFile("Container2_ActionsRestored.png");
            
            // Verify actions are restored by triggering the menu again and checking for original items
            actions.ClickAndHold(container).Perform();
            var verifyItem = wait.Until(d => App.FindElement(GetByText($"Press me 1!"))); 
            verifyItem.Click();
            var result = GetElement("c2_label");
            Assert.That(result.Text.Contains("clicked"), Is.True, "Result label didn't change");


        }
        catch (WebDriverException ex)
        {
            App.GetScreenshot().SaveAsFile("Container2_ContextMenu_Error.png");
            Assert.Fail($"Failed to interact with dynamic context menu: {ex.Message} {ex.StackTrace}");
        }
    }

    [Test]
    public void Container3_NeverEndingCommand()
    {
        // Find the container3 element
        var container = GetElement("container3");
        
        // Get the initial counter value
        var getLabel = () => container.FindElement(GetBy("c3_label")).Text;
        var initialText = getLabel();
        
        // Trigger context menu
        var actions = new Actions(App);
        actions.ClickAndHold(container).Perform();
        
        // Wait for context menu to appear
        var wait = new WebDriverWait(App, TimeSpan.FromSeconds(10));
        
        try {
            var menuItem = wait.Until(d => App.FindElement(GetByText("Start the loop!"))); 
            menuItem.Click();
            
            // Wait for counter to increase (at least 5 seconds)
            App.GetScreenshot().SaveAsFile("Container3_BeforeCounterIncrease.png");
            Thread.Sleep(6000);
            
            // Check if counter increased
            var currentText = getLabel();
            App.GetScreenshot().SaveAsFile("Container3_AfterCounterIncrease.png");
            
            Assert.That(currentText, Is.Not.EqualTo(initialText), "Counter should have increased");
        }
        catch (WebDriverException ex)
        {
            App.GetScreenshot().SaveAsFile("Container3_ContextMenu_Error.png");
            Assert.Fail($"Failed to interact with never-ending command: {ex.Message}");
        }
    }
}
