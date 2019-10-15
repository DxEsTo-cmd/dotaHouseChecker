using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using PriceDiff;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace GreenScoreChecker
{
    class Item
    {
        public string Name { get; set; }
        public double Price { get;  set;  }
        public int Count { get; set; }
    }

    class Diff
    {
        public string Name { get; set; }
        public double HousePrice { get; set; }
        public double SteamPrice { get; set; }
        public double Percent { get; set; }
    }

    class Program
    {
        static ChromeOptions chromeOptions = new ChromeOptions();

        private static DotaMarketParser dotaMarket;
        public static IWebDriver Browser { get; private set; }
        static void Main(string[] args)
        {
            chromeOptions.AddArgument(@"--user-data-dir=C:\User\User Data");
            chromeOptions.AddArgument("--profile-directory=Selenium");
            Browser = new ChromeDriver(chromeOptions);
            dotaMarket = new DotaMarketParser(chromeOptions);
           
            Browser = new ChromeDriver(chromeOptions);   
            List<Item> houseItems = GetCSGOHouseItems();
            List<Diff> difflist = GetSteamItems(houseItems);
            difflist = difflist.OrderBy(m => m.Percent).ToList();
            string fileName = DateTime.Now.ToString().Replace(".", "+").Replace(":", "+");
            foreach (var item in difflist)
            {
                Console.WriteLine(item.Name + " " + item.Percent);
                File.AppendAllText($"./CSGO+{fileName}.txt", item.Name + " " + item.Percent + " " + item.SteamPrice + " " + item.HousePrice + Environment.NewLine);
            }

            ///Different in price between DotaMarket and DotaHouse
            List<Diff> diffDmAndDh = dotaMarket.GetDiffBetweenHouse(houseItems); // Different in price between DotaMarket and DotaHouse
            dotaMarket.WriteInfoInFile(diffDmAndDh);
            ///

            houseItems = GetDOTAHouseItems();
            difflist = GetSteamItems(houseItems).OrderBy(m => m.Percent).ToList();
            foreach (var item in difflist)
            {
                Console.WriteLine(item.Name + " " + item.Percent);
                File.AppendAllText($"./DOTA+{fileName}.txt", item.Name + " " + item.Percent + " " + item.SteamPrice + " " + item.HousePrice + Environment.NewLine);
            }

        }

        private static List<Diff> GetSteamItems(List<Item> houseItems)
        {
            List<Diff> list = new List<Diff>();
            HtmlDocument doc = new HtmlDocument();
            foreach (var item in houseItems)
            {
                Thread.Sleep(300);
                try
                {
                    GoToUrl("https://steamcommunity.com/market/search?q=" + item.Name.Replace(" ", "+"));
                    doc.LoadHtml(Browser.PageSource);
                    double price = Convert.ToDouble(doc.DocumentNode.SelectSingleNode("//*[@id=\"result_0\"]/div[1]/div[2]/span[1]/span[1]").InnerText.Replace(" pуб.", "").Replace(",", "."), CultureInfo.InvariantCulture);
                    Console.WriteLine(item.Name + " " + item.Price + " " + price);
                    list.Add(new Diff() { Name = item.Name, HousePrice = item.Price, SteamPrice = price, Percent = price / item.Price });
                } catch(Exception exception)
                {
                    continue;
                }
            }
            return list;
        }
        private static List<Item> GetDOTAHouseItems()
        {
            GoToUrl("https://dotahouse.net/shop/");
            List<Item> list = new List<Item>();
            HtmlDocument doc;
            HtmlWeb htmlWeb = new HtmlWeb();
            doc = htmlWeb.Load("https://dotahouse.net/shop/");
            int page = 1;
            while (true)
            {
                var node = doc.DocumentNode.SelectSingleNode("//*[@id=\"withdraw_items\"]");
                int i = 0;
                foreach (var item in node.ChildNodes)
                {
                    if (i == node.ChildNodes.Count - 1) break;
                    if (i % 2 == 0)
                    {
                        string name = item.ChildNodes[3].ChildNodes[1].ChildNodes[0].InnerText;
                        string price = item.ChildNodes[3].ChildNodes[5].ChildNodes[1].GetAttributeValue("data-price", null);
                        string count = item.ChildNodes[3].ChildNodes[1].ChildNodes[1].ChildNodes[1].InnerText;
                        list.Add(new Item()
                        {
                            Count = Convert.ToInt32(count),
                            Name = name,
                            Price = Convert.ToDouble(price, CultureInfo.InvariantCulture)
                        });
                        Console.WriteLine(name + " " + count + " " + price);
                    }
                    i++;
                }
                if (node.LastChild.LastChild.GetAttributeValue("class", null).Contains("inactive"))
                    break;
                doc = htmlWeb.Load("https://dotahouse.net/shop/?page=" + (++page));
            }
            return list;
        }

        private static List<Item> GetCSGOHouseItems()
        {
            GoToUrl("https://csgo.house/shop/");
            List<Item> list = new List<Item>();
            HtmlDocument doc;
            HtmlWeb htmlWeb = new HtmlWeb();
            doc = htmlWeb.Load("https://csgo.house/shop/");
            int page = 1;
            while (true)
            {

                var node = doc.DocumentNode.SelectSingleNode("//*[@id=\"withdraw_items\"]");
                int i = 0;
                foreach (var item in node.ChildNodes)
                {
                    if (i == node.ChildNodes.Count - 1) break;
                    if (i % 2 == 0)
                    {
                        string name = item.ChildNodes[3].ChildNodes[1].ChildNodes[0].InnerText;
                        string price = item.ChildNodes[3].ChildNodes[5].ChildNodes[1].GetAttributeValue("data-price", null);
                        string count = item.ChildNodes[3].ChildNodes[1].ChildNodes[1].ChildNodes[1].InnerText;
                        list.Add(new Item()
                        {
                            Count = Convert.ToInt32(count),
                            Name = name,
                            Price = Convert.ToDouble(price, CultureInfo.InvariantCulture)
                        });
                        Console.WriteLine(name + " " + count + " " + price);
                    }
                    i++;
                }
                if (node.LastChild.LastChild.GetAttributeValue("class", null).Contains("inactive"))
                    break;
                doc = htmlWeb.Load("https://csgo.house/shop/?page=" + (++page));
            }
            return list;
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



