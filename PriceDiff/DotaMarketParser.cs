using GreenScoreChecker;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace PriceDiff
{
    class DotaMarketParser
    {
        private readonly ChromeOptions _chromeOptions;
        private static ChromeDriver _browser;

        public DotaMarketParser(ChromeOptions chromeOptions)
        {
            _chromeOptions = chromeOptions;
            _browser = new ChromeDriver(chromeOptions);
        }

        public Item GetItems(string name)
        {
            Item list = new Item();
            string url = "https://market.dota2.net/?s=pop&r=&q=&search=" + name;
            HtmlWeb htmlWeb = new HtmlWeb();
            HtmlDocument doc;
            try
            {
                doc = htmlWeb.Load(url);
            }
            catch (Exception e)
            {
                throw e;
            }
            
            HtmlNode items = doc.DocumentNode.SelectSingleNode("//*[@id='applications']");
            double price = 0;
            if(items != null)
            {
                foreach (var item in items.ChildNodes)
                {
                   var priceNode = item.SelectSingleNode("//a/div/div[2]");
                   if (priceNode != null)
                   {
                       double priceOfItem = 0;
                       string sum = Regex.Replace(priceNode.InnerText, @"\s+", "");
                       double.TryParse(sum, NumberStyles.Currency, CultureInfo.InvariantCulture,out priceOfItem);
                       price = priceOfItem;
                       list.Count++;
                       break;
                   }
                    //var curruncyNode = item.SelectSingleNode("//a/div/div[2]/small");     // If you wnat know what currency default it is RUB
                }
            }
            list.Name = name;
            list.Price = price;
            return list;
        }

        public List<Diff> GetDiffBetweenHouse(IEnumerable<Item> itemsFromHouse)
        {
            try
            {
                List<Diff> diffs = new List<Diff>();
                foreach (var item in itemsFromHouse)
                {
                    var marketItem = GetItems(item.Name);
                    diffs.Add(new Diff { Name = item.Name, HousePrice = item.Price, Percent = marketItem.Price / item.Price });
                }
                return diffs;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void WriteInfoInFile(IEnumerable<Diff> itemsFromMarket)
        {
            itemsFromMarket = itemsFromMarket.OrderBy(m => m.Percent).ToList();
            string fileName = DateTime.Now.ToString().Replace(".", "+").Replace(":", "+");
            foreach (var item in itemsFromMarket)
            {
                Console.WriteLine(item.Name + " " + item.Percent);
                File.AppendAllText($"./DotaMarket+{fileName}.txt", item.Name + " " + item.Percent + " " + item.SteamPrice + " " + item.HousePrice + Environment.NewLine);
            }
        }

        private static void GoToUrl(string url)
        {
            while (true)
            {
                try
                {
                    _browser.Navigate().GoToUrl(url);
                    break;
                }
                catch (Exception)
                {

                }
            }
        }
    }
}
