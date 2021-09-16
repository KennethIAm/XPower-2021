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
        string testUrl = "https://c875-93-176-82-58.ngrok.io/";

        IWebDriver edgeDriver;

        IWebElement emailInput;

        IWebElement passwordInput;

        IWebElement loginbtn;
        IWebElement logOutbtn;

        DefaultWait<IWebDriver> wait;

        [OneTimeSetUp]
        public void Setup()
        {
            

            
        }



        [TestCase("MailTest1@email.com", "PasswordTest")]
        [TestCase("MailTest2@email.com", "PasswordTest")]
        [TestCase("MailTest3@email.com", "PasswordTest")]
        [TestCase("MailTest4@email.com", "PasswordTest")]
        [TestCase("MailTest5@email.com", "PasswordTest")]
        public void Login_ValidCredentials_ShouldLoginAndRedirectToHomepage(string test1, string test2)
        {
            try
            {
                edgeDriver = new EdgeDriver(@"C:\Users\johan\Desktop\Programming\skole\XPower-2021\XPowerSolutions\XPowerEndToEndTest.Test\bin\Debug");
                wait = new WebDriverWait(edgeDriver, TimeSpan.FromSeconds(10));

                edgeDriver.Url = testUrl;

                emailInput = wait.Until(ExpectedConditions.ElementExists(By.Id("inputEmail")));
                passwordInput = edgeDriver.FindElement(By.Id("inputPassword"));
                loginbtn = edgeDriver.FindElement(By.XPath("/html/body/div[1]/div/div/div/div/form/div[3]/button[2]"));

                emailInput.SendKeys(test1);
                passwordInput.SendKeys(test2);
                loginbtn.Click();

                logOutbtn = wait.Until(ExpectedConditions.ElementExists(By.XPath("/html/body/nav/div/div/span/button")));
                if (logOutbtn == null)
                {
                    Assert.Fail();
                }

                edgeDriver.Close();
                Assert.Pass();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }



        [TestCase("Faillmaill@email.com", "faillpassword")]
        public void LoginFaillTest(string test1, string test2)
        {
            try
            {
                edgeDriver.Url = testUrl;

                emailInput = wait.Until(ExpectedConditions.ElementExists(By.Id("inputEmail")));
                passwordInput = edgeDriver.FindElement(By.Id("inputPassword"));
                loginbtn = edgeDriver.FindElement(By.XPath("/html/body/div[1]/div/div/div/div/form/div[3]/button[2]"));

                emailInput.SendKeys(test1);
                passwordInput.SendKeys(test2);
                loginbtn.Click();

                edgeDriver.Close();
                Assert.Pass();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);

            }
        }



        [TearDown]
        public void TearDown()
        {
            edgeDriver.Dispose();
        }
    }
}
