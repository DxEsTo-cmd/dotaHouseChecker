using HtmlAgilityPack;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
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
            Browser = new OpenQA.Selenium.Chrome.ChromeDriver();
            Browser.Navigate().GoToUrl("https://dotahouse.net/triple");
            Thread.Sleep(5000);
            Browser.FindElement(By.XPath("/html/body/div/div/a[1]/span")).Click();
            Thread.Sleep(5000);
            Browser.FindElement(By.XPath("//*[@id=\"steamAccountName\"]")).SendKeys("kartonowy3");
            Browser.FindElement(By.XPath("//*[@id=\"steamPassword\"]")).SendKeys("Huskar28");
            Browser.FindElement(By.XPath("//*[@id=\"imageLogin\"]")).Click();
            Thread.Sleep(15000);
            Browser.FindElement(By.XPath("//*[@id=\"twofactorcode_entry\"]")).SendKeys(steamGuard);
            Browser.FindElement(By.XPath("//*[@id=\"login_twofactorauth_buttonset_entercode\"]/div[1]")).Click();
            Thread.Sleep(15000);
            Browser.Navigate().GoToUrl("https://dotahouse.net/triple");
            Thread.Sleep(5000);
            string str = "";
            while (true)
            {

                var doc = new HtmlDocument();
                HtmlNodeCollection nodes;
                while (true)
                {
                    try
                    {
                        doc.LoadHtml(Browser.PageSource);
                        nodes = doc.DocumentNode.SelectNodes("//*[@id=\"triple_roulette\"]/div/div[4]");
                        break;
                    }
                    catch (Exception exception)
                    {
                    }
                }
                
                nodes = doc.DocumentNode.SelectNodes("//*[@id=\"triple_roulette\"]/div/div[4]");
                foreach (var item in nodes)
                {
                    var node = item.ChildNodes[item.ChildNodes.Count - 1];
                    Boolean check = false;
                    foreach (var attribute in node.Attributes)
                    {
                            if (attribute.Name == "class" && attribute.Value.Contains("num"))
                                check = true;  
                    }
                    if (check == true)
                    {
                        str = node.InnerText;
                        Console.WriteLine(str);
                        Thread.Sleep(35000);
                    }

                
                }
                
            }
        }
    }
}


