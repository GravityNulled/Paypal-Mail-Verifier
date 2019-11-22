using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using TrinitySeal;
using Console = Colorful.Console;

namespace Paypal_Mailer
{
    internal static class Program
    {
        private static void Main()
        {
            Console.WindowHeight = 30;
            Console.WindowWidth = 120;
            Console.WriteLine("                                             Validating Files....", Color.Aqua);
            Seal.Secret = "Zmkbc5xVUZc36QwFzPTfCp78SH8R9nrJrXBLLbOIG38l2";
            Seal.Initialize("1.0");
            System.Console.WriteLine("");
            Console.Write(
                "                                                Login: 1\n                                                Register: 2   -- ",
                Color.DeepPink);

            var options = int.Parse(Console.ReadLine());
            if (options == 1)
                Login();
            else
                Register();
        }

        private static void Register()
        {
            System.Console.WriteLine("");
            System.Console.WriteLine("");
            Console.WriteLine("                                            Registration Page", Color.Magenta);
            Console.Write("Enter your username: ");
            var username = Console.ReadLine();
            Console.Write("Enter your password: ");
            var password = Console.ReadLine();
            Console.Write("Enter your email: ");
            var email = Console.ReadLine();
            Console.Write("Enter your token: ");
            var token = Console.ReadLine();
            Console.WriteLine("Registering...", Color.Green);
            using (var sr = new StreamWriter("Login Details.txt"))
            {
                sr.WriteLine($"Username - {username}\nPass - {password}\nEmail - {email}\n");
            }

            SealCheck.HashChecks();
            if (SealCheck.isValidDLL)
            {
                var response = Seal.Register(username, password, email, token, false);
                if (response)
                {
                    Console.WriteLine("Successfully registered!", Color.Gold);
                    Console.WriteLine("Restart to Login!", Color.GreenYellow);
                    Thread.Sleep(2000);
                    Environment.Exit(0);
                }
                else
                {
                    Console.WriteLine("Failed to register!", Color.Crimson);
                    Thread.Sleep(2000);
                    Environment.Exit(0);
                }
            }
            else
            {
                Environment.Exit(0);
            }
        }

        private static void Login()
        {
            Console.Clear();
            var emailAddressList = File.ReadAllLines("emails.txt").ToList();
            var registered = 0;
            var unRegistered = 0;
            var completed = 0;
            var errors = 0;

            void TitleBar()
            {
                while (true)
                {
                    Console.Title =
                        $"Paypal Valid Mailer | Emails : {emailAddressList.Count} | Checked : {completed} | Registered : {registered} | Uregistered : {unRegistered} | Unkown Errors : {errors}";
                    Thread.Sleep(1000);
                }
            }

            System.Console.WriteLine("");
            System.Console.WriteLine("");
            Console.WriteLine("                                              Login Page", Color.Magenta);

            Console.Write("Enter your username: ");
            var username = Console.ReadLine();
            Console.Write("Enter your password: ");
            var password = Console.ReadLine();
            Console.WriteLine("Authenticating...", Color.Green);
            SealCheck.HashChecks();
            if (SealCheck.isValidDLL)
            {
                var response = Seal.Login(username, password, false);
                if (response)
                {
                    Console.WriteLine("Successfully logged in!", Color.Cyan);
                    Console.Clear();
                    Console.WriteLine("                                               Paypal  Vaild Mail  Checker  ",
                        Color.Cyan);
                    Console.WriteLine("                                             Project by @NotCzar - Telegram",
                        Color.Gold);
                    Console.Write("Enter Thread Amount: ");
                    var th = int.Parse(Console.ReadLine());
                    Task.Factory.StartNew(TitleBar);
                    Parallel.ForEach(emailAddressList, new ParallelOptions { MaxDegreeOfParallelism = th }, account =>
                      {
                      home:
                          try
                          {
                              var emailAddress = account;
                              var service = ChromeDriverService.CreateDefaultService();
                              service.HideCommandPromptWindow = true;
                              var options = new ChromeOptions();
                              options.AddArgument("--disable-logging");
                              options.AddArgument("--headless");
                              options.LeaveBrowserRunning = false;
                              var chrome = new ChromeDriver(service, options);
                              chrome.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
                              chrome.Navigate().GoToUrl("https://www.paypal.com/donate/buttons");
                              chrome.FindElementByXPath("/html/body/div/div[2]/div/div/div[2]/div/div/button[2]").Click();
                              chrome.FindElementByXPath(
                                      "/html/body/div/div[2]/div/div/div[1]/div/div[2]/div[2]/div[3]/button")
                                  .Click();
                              chrome.FindElementByXPath(
                                      "/html/body/div/div[2]/div/div/div[1]/div/div[3]/div[2]/div[2]/form/div[1]/div[1]/input")
                                  .SendKeys(emailAddress + Keys.Enter);
                              chrome.FindElementByXPath(
                                      "/html/body/div/div[2]/div/div/div[1]/div/div[3]/div[2]/div[3]/button")
                                  .SendKeys(Keys.Enter);
                              Thread.Sleep(1500);
                              var textarea = chrome.PageSource;
                              if (textarea.Contains("https://www.paypal.com/cgi-bin/webscr") ||
                                  textarea.Contains("_donations"))
                              {
                                  Console.WriteLine($"[+] Good - {account}", Color.Cyan);
                              writetofile:
                                  try
                                  {
                                      using (var sr = new StreamWriter("Registered Mails.txt", true))
                                      {
                                          sr.WriteLine(emailAddress);
                                      }
                                  }
                                  catch
                                  {
                                      Thread.Sleep(500);
                                      goto writetofile;
                                  }

                                  Interlocked.Increment(ref registered);
                                  Interlocked.Increment(ref completed);
                                //chrome.Close();
                                //chrome.Quit();
                              }
                              else if (!textarea.Contains("https://www.paypal.com/cgi-bin/webscr") ||
                                       textarea.Contains("_donations"))
                              {
                                  wirtetofile:
                                  try
                                  {
                                      Console.WriteLine($"[-] Bad  - {account}", Color.Crimson);
                                      using (var sr = new StreamWriter("Un-Registered Mails.txt", true))
                                      {
                                          sr.WriteLine(account);
                                      }
                                  }
                                  catch
                                  {
                                      Thread.Sleep(500);
                                      goto wirtetofile;
                                  }

                                  Interlocked.Increment(ref unRegistered);
                                  Interlocked.Increment(ref completed);
                                  //chrome.Close();
                                  //chrome.Quit();
                              }

                              savetofile:
                              try
                              {
                                  using (var sr = new StreamWriter("Checked Mails.txt", true))
                                  {
                                      sr.WriteLine(account);
                                  }
                              }
                              catch
                              {
                                  Thread.Sleep(500);
                                  goto savetofile;
                              }

                              chrome.Close();
                              chrome.Quit();
                          }
                          catch (Exception)
                          {
                            // System.Console.WriteLine(exception.Message);
                            Interlocked.Increment(ref errors);
                              goto home;
                          }
                      });


                    Console.WriteLine("");
                    Console.WriteLine("                                            Completed Working!",
                        Color.MediumSpringGreen);
                    Thread.Sleep(-1);
                }
                else
                {
                    Console.WriteLine("Failed to login!", Color.Crimson);
                    Console.Read();
                }
            }

            else
            {
                Environment.Exit(0);
            }
        }
    }
}