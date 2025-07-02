using OpenQA.Selenium.Appium.Interactions;
using OpenQA.Selenium.Interactions;
using PointerInputDevice = OpenQA.Selenium.Appium.Interactions.PointerInputDevice;
using NUnit.Framework;

namespace UITests;

/// <summary>
/// Tests for the MAUI Sample app with ContextMenuContainer controls
/// </summary>
public class ContextMenuSampleTest : TestPageBase
{
    protected override string PageName => "sample_page";
    
    [SetUp]
    public void Setup()
    {       
        //Assert.That(PageElement().Displayed, Is.True, "Page should be visible");
        SaveScreenshot("InitialState.png");
        StartScreenRecording();
    }

    [TearDown]
    public void TearDown() => StopScreenRecording();

    // Using base class SaveScreenshot method

    [Test]
    public void Container1_ContextMenuActions()
    {
        try
        {
            SaveScreenshot("Before_Scrolling_To_Container1.png");
            var container = ScrollToAndGetElement("c1_label");
            SaveScreenshot("After_Finding_Container1.png");
            Assert.That(container.Displayed, Is.True, "container1 should be visible");
            
            SaveScreenshot("Container1_Before.png");
            
            var actions = new Actions(App);
            
            ClickAndHold(actions, container);
            
    
            var wait = new WebDriverWait(App, TimeSpan.FromSeconds(10));
            
            var actionItem =  wait.Until(d =>App.FindElement(GetByText("Action 1")));
            SaveScreenshot("Container1_Menu.png");

            Assert.That(actionItem, Is.Not.Null, "Action 1 menu item should be visible");

            actionItem.Click();
            
            SaveScreenshot("Container1_After.png");

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
            SaveScreenshot("Container1_ContextMenu_Error.png");
            Assert.Fail($"Failed to interact with context menu: {ex.Message} {ex.StackTrace}");
        }
    }

    [Test]
    public void Container2_DynamicContextMenuItems()
    {
        SaveScreenshot("Before_Scrolling_To_Container2.png");
        var container = ScrollToAndGetElement("c2_label");
        Assert.That(container, Is.Not.Null);
        SaveScreenshot("After_Finding_Container2.png");
        
        
        var actions = new Actions(App);
        ClickAndHold(actions, container);
        
        
        var wait = new WebDriverWait(App, TimeSpan.FromSeconds(10));
        
        try {
            var destructiveItem = wait.Until(d => App.FindElement(GetByText("Remove context actions"))); 
            destructiveItem.Click();
            
            SaveScreenshot("Container2_Remove_Result.png");
            
            ClickAndHold(actions, container);
            
            
            var restoreItem = wait.Until(d => App.FindElement(GetByText("Give me my actions back!"))); 
            restoreItem.Click();
            
            SaveScreenshot("Container2_ActionsRestored.png");
            
            ClickAndHold(actions, container);
            var verifyItem = wait.Until(d => App.FindElement(GetByText($"Press me 1!"))); 
            verifyItem.Click();
            WaitFor(3);
            var result = GetElement("c2_label");
            Assert.That(result.Text.Contains("clicked"), Is.True, "Result label didn't change");


        }
        catch (WebDriverException ex)
        {
            SaveScreenshot("Container2_ContextMenu_Error.png");
            Assert.Fail($"Failed to interact with dynamic context menu: {ex.Message} {ex.StackTrace}");
        }
    }

    [Test]
    public void Container3_NeverEndingCommand()
    {
        // Use ScrollToAndGetElement to find container3 even if it's not visible
        SaveScreenshot("Before_Scrolling_To_Container3.png");
        ScrollToAndGetElement("c3_label");
        SaveScreenshot("After_Finding_Container3.png");
        
        var getLabel = () => App.FindElement(GetBy("c3_label")).Text;
        var initialText = getLabel();
        
        var actions = new Actions(App);
        ClickAndHold(actions, GetElement("c3_label"));
        
        
        var wait = new WebDriverWait(App, TimeSpan.FromSeconds(10));
        
        try {
            var menuItem = wait.Until(d => App.FindElement(GetByText("Start the loop!"))); 
            menuItem.Click();
            
            SaveScreenshot("Container3_BeforeCounterIncrease.png");
            Thread.Sleep(6000);
            var currentText = getLabel();
            SaveScreenshot("Container3_AfterCounterIncrease.png");
            
            Assert.That(currentText, Is.Not.EqualTo(initialText), "Counter should have increased");
        }
        catch (WebDriverException ex)
        {
            SaveScreenshot("Container3_ContextMenu_Error.png");
            Assert.Fail($"Failed to interact with never-ending command: {ex.Message}");
        }
    }
    [Test]
    public void Container4_ConditionalCommand()
    {
        // Scroll to container4 first
        SaveScreenshot("Before_Scrolling_To_Container4.png");
        ScrollToAndGetElement("c4_toggle");
        SaveScreenshot("After_Finding_Container4.png");
        
        var getLabel = () => App.FindElement(GetBy("c4_label")).Text;
        Func<AppiumElement> getToggle = () =>
        {
            var e = App.FindElement(GetBy("c4_toggle"));
            e.DisableCache();
            Console.WriteLine("Got Toggle with text: {0}", e.Text);
                
            return e;
        };
        var toggleClick = () =>
        {
            
            
            var wait = new WebDriverWait(App, TimeSpan.FromSeconds(10));
            wait.Until(d =>
            {
                var t = getToggle();
                var it = t.Text;
                t.Click();
                return getToggle().Text != it;
            });
            

            
        };
        var isEnabled = () =>
        {
            var t = getToggle().Text;
            // Console.WriteLine("Toggle text was: {0}", t);
            return t.Contains("enabled");
        };
        var doTest = (int counter) =>
        {
            var e = GetElement("c4_label");
            var ie = isEnabled();
            Console.WriteLine("Is enabled: {0}",  ie);
            var actions = new Actions(App);
            string initialText = getLabel();
            ClickAndHold(actions,e);
            
            var wait = new WebDriverWait(App, TimeSpan.FromSeconds(10));
            var menuItem = wait.Until(d => App.FindElement(GetByText(ie ? "Should be enabled" : "Should be disabled")));
            menuItem.Click();
            if (App is WindowsDriver && menuItem.Displayed)
            {
                menuItem.Click();
            }

            if (!ie)
            {
                //disabled item won't close the popup, so we need to click outside the popup menu to close it
                Console.WriteLine("Move finger outside the container");
                if (App is WindowsDriver)
                {
                    App.FindElement(GetBy("c4_label")).Click();
                }
                else
                {
                    var finger = new PointerInputDevice(PointerKind.Touch);
                    var sequence = new ActionSequence(finger, 0);
                    sequence.AddAction(finger.CreatePointerMove(CoordinateOrigin.Viewport, 300, 500, TimeSpan.Zero));
                    sequence.AddAction(finger.CreatePointerDown(PointerButton.TouchContact));
                    sequence.AddAction(finger.CreatePause(TimeSpan.FromSeconds(1)));
                    sequence.AddAction(finger.CreatePointerUp(PointerButton.TouchContact));
                    App.PerformActions([sequence]);
                }
            }
            
            string currentText = getLabel();
            SaveScreenshot($"Container4_AfterCounterIncrease_{counter}.png");
            Console.WriteLine("Initial text: [{0}]; Current text:[{1}]", initialText, currentText );
            if(ie)
                Assert.That(currentText, Is.Not.EqualTo(initialText), "Counter should have increased");
            else 
                Assert.That(currentText, Is.EqualTo(initialText), "Counter should not have increased");
                
        };
        try
        {

            Assert.Multiple(() =>
            {
                var c = 1;
                doTest(++c);
                toggleClick();
                doTest(++c);
                toggleClick();
                doTest(++c);
                toggleClick();
                doTest(++c);
                toggleClick();
                doTest(++c);
            });

        }
        catch (WebDriverException ex)
        {
            SaveScreenshot("Container4_ContextMenu_Error.png");
            Assert.Fail($"Failed to interact with conditional command: {ex.Message} {ex.StackTrace}");
        }
    }
}
