using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Threading;

namespace XPowerEndToEndTest.Test
{
    public class CreateUserTest
    {
        string testUrl = "http://7351-93-176-82-58.ngrok.io/account/login";

        IWebDriver edgeDriver;
        IWebElement emailInput;
        IWebElement passwordInput;
        IWebElement loginbtn;
        IWebElement logOutbtn;
        IWebElement createNavbtn;
        IWebElement username;
        IWebElement reEnterPassword;
        IWebElement Createbtn;

        DefaultWait<IWebDriver> wait;

        [OneTimeSetUp]
        public void Setup()
        {
            edgeDriver = new EdgeDriver(@"C:\Users\johan\Desktop\Programming\skole\XPower-2021\XPowerSolutions\XPowerEndToEndTest.Test\bin\Debug");

            wait = new WebDriverWait(edgeDriver, TimeSpan.FromSeconds(10));
        }

        [Test]
        public void TestSuccessCreateUser()
        {
            edgeDriver.Url = testUrl;

            createNavbtn = wait.Until(ExpectedConditions.ElementExists(By.XPath("/html/body/div[1]/div/div/div/div/form/div[3]/button[1]")));

            emailInput = edgeDriver.FindElement(By.Id("inputEmail"));
            username = edgeDriver.FindElement(By.Id("inputUsername"));
            passwordInput = edgeDriver.FindElement(By.Id("inputPassword"));
            reEnterPassword = edgeDriver.FindElement(By.Id("inputReEnterPassword"));

            emailInput.SendKeys("MailTest20@email.com");
            username.SendKeys("Testacc20");
            passwordInput.SendKeys("PasswordTest");
            reEnterPassword.SendKeys("PasswordTest");
            loginbtn.Click();

            logOutbtn = wait.Until(ExpectedConditions.ElementExists(By.XPath("/html/body/nav/div/div/span/button")));
            logOutbtn.Click();

            edgeDriver.Close();
            Assert.Pass();
        }

        [Test]
        public void TestFaillCreateUser()
        {
        }



        [TearDown]
        public void TearDown()
        {
            edgeDriver.Dispose();
        }
    }
}
