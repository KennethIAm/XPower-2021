using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using XPowerClassLibrary.Users;

namespace XPowerEndToEndTest.Test
{
    public class CreateUserTest
    {
        string testUrl = "https://17c8-93-176-82-58.ngrok.io/";

        IWebDriver edgeDriver;
        IWebElement emailInput;
        IWebElement passwordInput;
        IWebElement logOutbtn;
        IWebElement createNavbtn;
        IWebElement username;
        IWebElement reEnterPassword;
        IWebElement Createbtn;
        IWebElement burgerMenu;

        DefaultWait<IWebDriver> wait;


        [OneTimeSetUp]
        public void Setup()
        {
        }

        [TestCase("Test", "PasswordTest", "Testuser1")]
        [TestCase("Test", "PasswordTest", "Testuser2")]
        [TestCase("Test", "PasswordTest", "Testuser3")]
        [TestCase("Test", "PasswordTest", "Testuser4")]
        [TestCase("Test", "PasswordTest", "Testuser5")]
        public void CreateUser_ValidCredentials_ShouldCreateUserAndNavigateToLogin(string mail, string pass, string usernameinput)
        {
            try
            {
                edgeDriver = new EdgeDriver(@"D:\_Projects\XPower-2021\XPowerSolutions\XPowerEndToEndTest.Test");
                wait = new WebDriverWait(edgeDriver, TimeSpan.FromSeconds(10));

                edgeDriver.Url = testUrl;

                createNavbtn = wait.Until(ExpectedConditions.ElementExists(By.XPath("/html/body/div[1]/div/div/div/div/form/div[3]/button[1]")));
                createNavbtn.Click();

                var rnd = new Random(DateTime.Now.ToString().GetHashCode()).Next(100, 10000).ToString();
                mail = mail + rnd + "@Test.dk";

                username = wait.Until(ExpectedConditions.ElementExists(By.Id("inputUsername")));
                emailInput = edgeDriver.FindElement(By.Id("inputEmail"));
                passwordInput = edgeDriver.FindElement(By.Id("inputPassword"));
                reEnterPassword = edgeDriver.FindElement(By.Id("inputReEnterPassword"));
                Createbtn = edgeDriver.FindElement(By.XPath("/html/body/div[1]/div/div/div/div/form/div[4]/button[2]"));

                emailInput.SendKeys(mail);
                username.SendKeys(usernameinput);
                passwordInput.SendKeys(pass);
                reEnterPassword.SendKeys(pass);


                Stopwatch benchmarkWatch = new Stopwatch();
                benchmarkWatch.Start();
                

                Createbtn.Click();
                //*[@id="inputUsername"]
                try
                {
                    burgerMenu = wait.Until(ExpectedConditions.ElementExists(By.XPath("/html/body/nav/div/button")));
                    burgerMenu.Click();
                    logOutbtn = wait.Until(ExpectedConditions.ElementExists(By.XPath("/html/body/nav/div/div/span/button")));
                    burgerMenu.Click();

                    benchmarkWatch.Stop();
                    Console.WriteLine(benchmarkWatch.ElapsedMilliseconds);
                    Assert.Pass();
                }
                catch (Exception)
                {
                    try
                    {
                        logOutbtn = wait.Until(ExpectedConditions.ElementExists(By.XPath("/html/body/nav/div/div/span/button")));
                        Assert.Pass();
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }

                edgeDriver.Close();
                Assert.Pass();
            }
            catch (Exception)
            {
                throw;
            }
        }


        [TearDown]
        public void TearDown()
        {
            edgeDriver.Dispose();
        }
    }
}
