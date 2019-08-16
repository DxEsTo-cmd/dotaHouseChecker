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
            //Thread.Sleep(5000);
            Browser.FindElement(By.XPath("/html/body/div/div/a[1]/span")).Click();
            //Thread.Sleep(5000);
            Browser.FindElement(By.XPath("//*[@id=\"steamAccountName\"]")).SendKeys("kartonowy3");
            Browser.FindElement(By.XPath("//*[@id=\"steamPassword\"]")).SendKeys("Huskar28");
            Browser.FindElement(By.XPath("//*[@id=\"imageLogin\"]")).Click();
            //Thread.Sleep(15000);
            var wait = new WebDriverWait(Browser, TimeSpan.FromMinutes(1));
            var clickableElement = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id=\"twofactorcode_entry\"]")));
            Browser.FindElement(By.XPath("//*[@id=\"twofactorcode_entry\"]")).SendKeys(steamGuard);
            Browser.FindElement(By.XPath("//*[@id=\"login_twofactorauth_buttonset_entercode\"]/div[1]")).Click();
            clickableElement = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id=\"promo\"]/div[1]/div[2]/div[2]/a")));
            GoToUrl("https://dotahouse.net/triple");
            //Thread.Sleep(5000);
            while (true)
            {

                var doc = new HtmlDocument();
                HtmlNodeCollection nodes;
                while (true)
                {

                    try
                    {
                        doc.LoadHtml(Browser.PageSource);
                        HtmlNode timer = doc.DocumentNode.SelectSingleNode("//*[@id='triple_roulette']/div/div[2]/span/span");
                        if (timer.InnerText.StartsWith("0."))
                        {
                                break;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

                string betOnRed = doc.DocumentNode.SelectSingleNode("//*[@id='bet_1']/div/table[1]/tbody/tr/td[2]/span").InnerText;
                string betOnGreen = doc.DocumentNode.SelectSingleNode("//*[@id='bet_2']/div/table[1]/tbody/tr/td[2]/span").InnerText;
                string betOnBlack = doc.DocumentNode.SelectSingleNode("//*[@id='bet_3']/div/table[1]/tbody/tr/td[2]/span").InnerText;

                Thread.Sleep(15000);

                string str = null;
                doc.LoadHtml(Browser.PageSource);
                nodes = doc.DocumentNode.SelectNodes("//*[@id=\"triple_roulette\"]/div/div[4]");
                foreach (var item in nodes)
                {
                    var node = item.ChildNodes[item.ChildNodes.Count - 1];
                    bool check = false;
                    foreach (var attribute in node.Attributes)
                    {
                        if (attribute.Name == "class" && attribute.Value.Contains("num"))
                            check = true;
                    }
                    if (check == true)
                    {
                        str = node.InnerText;
                    }
                }

                Console.WriteLine(str + " Red: " + betOnRed + " Green: " + betOnGreen + " Black " + betOnBlack);
                File.AppendAllText("./info.txt", str + " " + betOnRed.Replace(" ", "") + " " + betOnGreen.Replace(" ", "") + " " + betOnBlack.Replace(" ", "") + Environment.NewLine);
                Thread.Sleep(20000);
            }
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


