using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Threading;

namespace XPowerEndToEndTest.Test
{
    public class LogOutTest
    {
        string testUrl = "https://localhost:6001/account/login";

        IWebDriver edgeDriver;

        IWebElement emailInput;

        IWebElement passwordInput;

        IWebElement loginbtn;

        IWebElement logOutbtn;

        DefaultWait<IWebDriver> wait;

        [OneTimeSetUp]
        public void Setup()
        {
            edgeDriver = new EdgeDriver(@"D:\_Projects\XPower-2021\XPowerSolutions\XPowerEndToEndTest.Test");

            wait = new WebDriverWait(edgeDriver, TimeSpan.FromSeconds(10));
        }



        [Test]
        public void LogOutSuccessTest()
        {
            edgeDriver.Url = testUrl;

            emailInput = wait.Until(ExpectedConditions.ElementExists(By.Id("inputEmail")));
            passwordInput = edgeDriver.FindElement(By.Id("inputPassword"));
            loginbtn = edgeDriver.FindElement(By.XPath("/html/body/div[1]/div/div/div/div/form/div[3]/button[2]"));

            emailInput.SendKeys("MailTest4@email.com");
            passwordInput.SendKeys("PasswordTest");
            loginbtn.Click();

            logOutbtn = wait.Until(ExpectedConditions.ElementExists(By.XPath("/html/body/nav/div/div/span/button")));
            logOutbtn.Click();

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
