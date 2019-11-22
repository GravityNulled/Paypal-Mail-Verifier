using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Paypal_ValidMail_Checker_Selenium
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("                                            Credit   Card  Checker  ", Color.Cyan);
            //Console.WriteLine("                                              Project by NotCzar", Color.Gold);
            string emailAddress = "omarcymar@gmail.com";

            var service = ChromeDriverService.CreateDefaultService();
            //service.HideCommandPromptWindow = true;
            var options = new ChromeOptions();
            //options.AddArgument("--disable-logging");
            //options.AddArgument("--headless");
            options.LeaveBrowserRunning = false;
            var chrome = new ChromeDriver(service, options);
            chrome.Manage().Timeouts().ImplicitWait= TimeSpan.FromSeconds(30);
            chrome.Navigate().GoToUrl("https://www.paypal.com/donate/buttons");
            chrome.FindElementByXPath("/html/body/div/div[2]/div/div/div[2]/div/div/button[2]").Click();
            chrome.FindElementByXPath("/html/body/div/div[2]/div/div/div[1]/div/div[2]/div[2]/div[3]/button").Click();
            chrome.FindElementByXPath(
                "/html/body/div/div[2]/div/div/div[1]/div/div[3]/div[2]/div[2]/form/div[1]/div[1]/input").SendKeys(emailAddress + Keys.Enter);
            chrome.FindElementByXPath("/html/body/div/div[2]/div/div/div[1]/div/div[3]/div[2]/div[3]/button").SendKeys(Keys.Enter);
            Thread.Sleep(1000);
            Console.Clear();
            var textarea = chrome.PageSource;
            if (textarea.Contains("https://www.paypal.com/cgi-bin/webscr") || textarea.Contains("_donations"))
            {
                Console.WriteLine(emailAddress);
            }
            else if (!textarea.Contains("https://www.paypal.com/cgi-bin/webscr") || textarea.Contains("_donations"))
            {
                Console.WriteLine("Not Working");
            }
            Thread.Sleep(-1);
        }
    }
}
