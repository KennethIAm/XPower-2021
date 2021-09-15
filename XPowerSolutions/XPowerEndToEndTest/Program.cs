using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;


namespace XPowerEndToEndTest
{
    class Program
    {
        static void Main(string[] args)
        {













            Console.WriteLine("Test Script");

            IWebDriver driver = new ChromeDriver();

            driver.Navigate().GoToUrl("https://localhost:6001/account/login");

            Thread.Sleep(2000);

            IWebElement emailInput = driver.FindElement(By.Id("inputEmail"));
            IWebElement passwordInput = driver.FindElement(By.Id("inputPassword"));
            IWebElement loginbtn = driver.FindElement(By.XPath("/html/body/div[1]/div/div/div/div/form/div[3]/button[1]"));


            emailInput.SendKeys("MailTest4@email.com");
            passwordInput.SendKeys("PasswordTest");


            loginbtn.Click();




            Console.ReadLine();
        }
    }
}
