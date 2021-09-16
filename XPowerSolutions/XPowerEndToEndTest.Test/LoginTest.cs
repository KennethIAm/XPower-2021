using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Threading;

namespace XPowerEndToEndTest.Test
{
    public class LoginTest
    {
        string testUrl = "http://7351-93-176-82-58.ngrok.io/account/login";

        IWebDriver edgeDriver;

        IWebElement emailInput;

        IWebElement passwordInput;

        IWebElement loginbtn;

        DefaultWait<IWebDriver> wait;

        [OneTimeSetUp]
        public void Setup()
        {
            edgeDriver = new EdgeDriver(@"C:\Users\johan\Desktop\Programming\skole\XPower-2021\XPowerSolutions\XPowerEndToEndTest.Test\bin\Debug");

            wait = new WebDriverWait(edgeDriver, TimeSpan.FromSeconds(10));
        }



        [Test]
        public void OpenChrome_OpensTheWebBrowser_ShouldOpenWeb()
        {
            edgeDriver.Url = testUrl;

            emailInput = wait.Until(ExpectedConditions.ElementExists(By.Id("inputEmail")));
            passwordInput = edgeDriver.FindElement(By.Id("inputPassword"));
            loginbtn = edgeDriver.FindElement(By.XPath("/html/body/div[1]/div/div/div/div/form/div[3]/button[2]"));

            emailInput.SendKeys("MailTest4@email.com");
            passwordInput.SendKeys("PasswordTest");
            loginbtn.Click();

            edgeDriver.Close();
            Assert.Pass();
        }



        [TearDown]
        public void TearDown()
        {
            edgeDriver.Dispose();
        }
    }
}
