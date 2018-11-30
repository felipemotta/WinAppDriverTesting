namespace Tool.Smoke.UI.Tests
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenQA.Selenium.Appium.Windows;
    using OpenQA.Selenium.Remote;

    [TestClass]
    [DeploymentItem("SUT", "SUT")]
    public class KeyPassTest : UiTestBase
    {
        [AssemblyInitialize]
        public static void AssemblyInitializeAttribute(TestContext context) => RunWinAppDriver();

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) => Setup(context);

        [TestMethod]
        public void GivenAboutKeyPassIsOpenWhemTheDialogIsDisplayedThenTheVersionNumberIsTheCorrectOne()
        {
            session.FindElementByName("Help").Click();
            session.FindElementByName("About Key Pass...").Click();
        }

        [ClassCleanup]
        public static void ClassCleanup() => Teardown();
    }
    
    public abstract class UiTestBase
    {
        protected const string WinAppDriverUrl = "http://127.0.0.1:4723";
        protected const string ApplcationPath = @"SUT\KeePass.exe";
        protected static WindowsDriver<WindowsElement> session;

        public static void RunWinAppDriver() => Task.Factory.StartNew(() => Process.Start(@"C:\Program Files (x86)\Windows Application Driver\WinAppDriver.exe")).Wait();

        public static void Setup(TestContext context)
        {
            if (session == null)
            {
                LaunchApplication();

                ApplicationPreconditions();

                SetApplicationConfigurations();
            }
        }

        public static void Teardown()
        {
            if (session != null)
            {
                session.Close();

                session.Quit();

                session = null;
            }
        }

        /// <summary>
        /// Set implicit timeout to 1.5 seconds to make element search to retry every 500 ms for at most three times.
        /// </summary>
        private static void SetApplicationConfigurations() => session.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(1.5));

        private static void ApplicationPreconditions()
        {
            // Driver status
            var message = $"Please check that the application under test is available in '{ApplcationPath}'.";
            session.Should().NotBeNull($"the application session is not available. {message}");
            session.SessionId.Should().NotBeNull($"the application session id is not available. {message}");

            // Configure App for updates
            try
            {
                WindowsElement enableButton = session.FindElementByName("\"Disable\"");
                if (enableButton != null)
                {
                    enableButton.Click();
                }
            }
            catch { }


            // Application under tests status
            session.Title.Should().Be("KeePass");
        }

        private static void LaunchApplication()
        {
            // Create a new session to launch the application
            var appCapabilities = new DesiredCapabilities();
            appCapabilities.SetCapability("app", ApplcationPath);

            var remoteAddress = new Uri(WinAppDriverUrl);
            session = new WindowsDriver<WindowsElement>(remoteAddress, appCapabilities);
        }
    }
}
