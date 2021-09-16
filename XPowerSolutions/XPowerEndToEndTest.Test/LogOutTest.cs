using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace XPowerEndToEndTest.Test
{
    public class LogOutTest
    {
        string testUrl = "http://2380-93-176-82-58.ngrok.io/";

        IWebDriver edgeDriver;
        IWebElement emailInput;
        IWebElement passwordInput;
        IWebElement loginbtn;
        IWebElement burgerMenu;
        IWebElement logOutbtn;
        IWebElement loginpagetext;

        DefaultWait<IWebDriver> wait;

        [OneTimeSetUp]
        public void Setup()
        {
        }



        [TestCase("MailTest1@email.com", "PasswordTest")]
        [TestCase("MailTest6@email.com", "PasswordTest")]
        [TestCase("MailTest3@email.com", "PasswordTest")]
        [TestCase("MailTest4@email.com", "PasswordTest")]
        [TestCase("MailTest5@email.com", "PasswordTest")]
        public void LogOut_ValidUserSession_ShouldLoginAndRedirectToLoginpageThenLogOut(string mail, string pass)
        {
            try
            {
                edgeDriver = new EdgeDriver(@"C:\Users\johan\Desktop\Programming\skole\XPower-2021\XPowerSolutions\XPowerEndToEndTest.Test\bin\Debug");
                wait = new WebDriverWait(edgeDriver, TimeSpan.FromSeconds(10));

                edgeDriver.Url = testUrl;

                emailInput = wait.Until(ExpectedConditions.ElementExists(By.Id("inputEmail")));
                passwordInput = edgeDriver.FindElement(By.Id("inputPassword"));
                loginbtn = edgeDriver.FindElement(By.XPath("/html/body/div[1]/div/div/div/div/form/div[3]/button[2]"));

                emailInput.SendKeys(mail);
                passwordInput.SendKeys(pass);
                loginbtn.Click();

                burgerMenu = wait.Until(ExpectedConditions.ElementExists(By.XPath("/html/body/nav/div/button")));
                burgerMenu.Click();
                logOutbtn = wait.Until(ExpectedConditions.ElementExists(By.XPath("/html/body/nav/div/div/span/button")));
                logOutbtn.Click();

                Thread.Sleep(1000);
                var url = edgeDriver.Url;
                if (url == "http://2380-93-176-82-58.ngrok.io/account/login")
                {
                    Assert.Pass();
                }
                else
                    Assert.Fail();


            }
            finally
            {
                edgeDriver.Close();
            }
        }


        [TearDown]
        public void TearDown()
        {
            edgeDriver.Dispose();
        }
    }
}
