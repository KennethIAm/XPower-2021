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
    public class LoginTest
    {
        string testUrl = "https://17c8-93-176-82-58.ngrok.io/";

        IWebDriver edgeDriver;

        IWebElement emailInput;

        IWebElement passwordInput;

        IWebElement loginbtn;
        IWebElement burgerMenu;
        IWebElement logOutbtn;
        IWebElement errorBox;


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
        public void Login_ValidCredentials_ShouldLoginAndRedirectToHomepage(string mail, string pass)
        {

            try
            {
                edgeDriver = new EdgeDriver(@"D:\_Projects\XPower-2021\XPowerSolutions\XPowerEndToEndTest.Test");
                wait = new WebDriverWait(edgeDriver, TimeSpan.FromSeconds(10));

                edgeDriver.Url = testUrl;

                emailInput = wait.Until(ExpectedConditions.ElementExists(By.Id("inputEmail")));
                passwordInput = edgeDriver.FindElement(By.Id("inputPassword"));
                loginbtn = edgeDriver.FindElement(By.XPath("/html/body/div[1]/div/div/div/div/form/div[3]/button[2]"));

                emailInput.SendKeys(mail);
                passwordInput.SendKeys(pass);


                Stopwatch benchmarkWatch = new Stopwatch();
                benchmarkWatch.Start();
                

                loginbtn.Click();

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
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                edgeDriver.Close();
            }



        }



      [TestCase("Faillmaill1@email.com", "faillpassword")]
      [TestCase("Faillmaill2@email.com", "faillpassword")]
      [TestCase("Faillmaill3@email.com", "faillpassword")]
      [TestCase("Faillmaill4@email.com", "faillpassword")]
      [TestCase("Faillmaill5@email.com", "faillpassword")]
        public void Login_IncorrectCredentials_ShouldFaillLoginAndGetAnErrorMessage(string test1, string test2)
        {
            try
            {
                edgeDriver = new EdgeDriver(@"D:\_Projects\XPower-2021\XPowerSolutions\XPowerEndToEndTest.Test");
                wait = new WebDriverWait(edgeDriver, TimeSpan.FromSeconds(10));

                edgeDriver.Url = testUrl;

                emailInput = wait.Until(ExpectedConditions.ElementExists(By.Id("inputEmail")));
                passwordInput = edgeDriver.FindElement(By.Id("inputPassword"));
                loginbtn = edgeDriver.FindElement(By.XPath("/html/body/div[1]/div/div/div/div/form/div[3]/button[2]"));

                emailInput.SendKeys(test1);
                passwordInput.SendKeys(test2);
                loginbtn.Click();
                Stopwatch benchmarkWatch = new Stopwatch();
                benchmarkWatch.Start();
                

                try
                {
                    errorBox = wait.Until(ExpectedConditions.ElementExists(By.XPath("/html/body/div[1]/div/div/div/div/div")));
                    
                    benchmarkWatch.Stop();
                    Console.WriteLine(benchmarkWatch.ElapsedMilliseconds);

                    Assert.Pass();
                }
                catch (Exception)
                {
                    throw;
                }
            }
            catch (Exception)
            {
                throw;
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
