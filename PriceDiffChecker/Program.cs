using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GreenScoreChecker
{
    class Program
    {
        public static IWebDriver Browser { get; private set; }
        static void Main(string[] args)
        {
            Console.WriteLine("Steam Guard:");
            string steamGuard = Console.ReadLine();
            Browser = new ChromeDriver();
            Browser.Manage().Timeouts().ImplicitWait.Add(TimeSpan.FromSeconds(30));
            Browser.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(320);
            GoToUrl("https://dotahouse.net/triple");
            Browser.FindElement(By.XPath("/html/body/div/div/a[1]/span")).Click();
            Browser.FindElement(By.XPath("//*[@id=\"steamAccountName\"]")).SendKeys("kartonowy3");
            Browser.FindElement(By.XPath("//*[@id=\"steamPassword\"]")).SendKeys("Huskar28");
            Browser.FindElement(By.XPath("//*[@id=\"imageLogin\"]")).Click();
            var wait = new WebDriverWait(Browser, TimeSpan.FromMinutes(1));
            var clickableElement = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id=\"twofactorcode_entry\"]")));
            Browser.FindElement(By.XPath("//*[@id=\"twofactorcode_entry\"]")).SendKeys(steamGuard);
            Browser.FindElement(By.XPath("//*[@id=\"login_twofactorauth_buttonset_entercode\"]/div[1]")).Click();
            clickableElement = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id=\"promo\"]/div[1]/div[2]/div[2]/a")));
            GoToUrl("https://dotahouse.net/shop/");
            clickableElement = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id=\"triple_pagintion\"]/a[13]")));
            Browser.FindElement(By.XPath("//*[@id=\"triple_pagintion\"]/a[13]")).Click();
            string countPages = Browser.FindElement(By.XPath("//*[@id=\"triple_pagintion\"]/span[1]")).Text;
            Console.WriteLine(countPages);

        }

        public static void GoToUrl(string url)
        {
            while (true)
            {
                try
                {
                    Browser.Navigate().GoToUrl(url);
                    break;
                }
                catch (Exception)
                {

                }
            }
        }
    }
}


