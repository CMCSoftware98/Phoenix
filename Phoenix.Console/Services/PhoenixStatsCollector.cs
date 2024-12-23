using HtmlAgilityPack;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Phoenix.Console.Services
{
    public class PhoenixStatsCollector : IHostedService, IDisposable
    {
        private readonly Timer _timer;
        private readonly ILogger<PhoenixStatsCollector> _logger;
        private readonly IWebDriver _webDriver;
        private List<string> botlinks = new List<string>();

        public PhoenixStatsCollector(ILogger<PhoenixStatsCollector> logger)
        {
            _logger = logger;
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless");

            _webDriver = new ChromeDriver(chromeOptions);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while(true)
            {
                _logger.LogInformation("MyHostedService is starting.");

                await _webDriver.Navigate().GoToUrlAsync("https://csgoempire.com/match-betting?bt-path=/cs2-ai/counter-strike-2/de-dust2--bo15--knife-2443129125658038305");


                await Task.Delay(TimeSpan.FromSeconds(10));

                string xPathShadow = "//*[@id=\"bt-inner-page\"]";

                var shadowRoot = _webDriver.FindElement(By.XPath(xPathShadow)).GetShadowRoot();

                // Use a regular expression to match the href pattern
                string pattern = @"/cs2-ai/counter-strike-2/de-dust2--bo15--knife/sas-elite-crew-\d+"; // \d+ matches one or more digits

                // Find all <a> elements
                var links = _webDriver.FindElements(By.TagName("a"));


                var linksInShadowRoot = shadowRoot.FindElements(By.CssSelector("a"));

                foreach (var link in linksInShadowRoot)
                {
                    string href = link.GetDomAttribute("href");
                    if (!string.IsNullOrEmpty(href) && Regex.IsMatch(href, pattern))
                    {
                        if(!botlinks.Contains(href))
                        {
                            _logger.LogInformation($"Adding Link: {href}");
                            botlinks.Add(href);
                        }
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(30));
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("MyHostedService is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public static string WaitForValueFromXPath(string html, string xpath, int timeoutInSeconds = 10)
        {
            DateTime startTime = DateTime.Now;

            while (DateTime.Now - startTime < TimeSpan.FromSeconds(timeoutInSeconds))
            {
                try
                {
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(html);

                    HtmlNode node = doc.DocumentNode.SelectSingleNode(xpath);

                    if (node != null)
                    {
                        return node.InnerText;
                    }

                    // If the node is not found, wait for a short interval
                    Thread.Sleep(500);
                }
                catch (Exception ex)
                {
                    // Handle exceptions (e.g., log, retry)
                    Thread.Sleep(500);
                }
            }

            // Timeout reached
            return string.Empty;
        }

        private void DoWork(object? state)
        {
            

            _logger.LogInformation("MyHostedService is working.");
            // Perform your background task here
        }
    }
}
