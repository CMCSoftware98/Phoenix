using HtmlAgilityPack;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using Phoenix.Shared.Enums;
using Phoenix.Shared.Models;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Frozen;
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
        private readonly RestClient _client;
        private List<MatchResult> _matchResults = new List<MatchResult>();
        private List<MatchResult> _foundMatchResults = new List<MatchResult>();

        public PhoenixStatsCollector(ILogger<PhoenixStatsCollector> logger)
        {
            _logger = logger;
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless");
            
            _webDriver = new ChromeDriver(chromeOptions);

            var options = new RestClientOptions("https://phoenix-api-229361706325.europe-west4.run.app");

            _client = new RestClient(options);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while(true)
            {
                try
                {
                    await GetMatchLinks();

                    await GetOfficialMatchResults();

                    await PostMatchResults();

                    await Task.Delay(TimeSpan.FromSeconds(30));
                }
                catch (Exception ex) {
                    _logger.LogError(ex.Message);
                }
            }
        }

        public async Task GetMatchLinks()
        {

            await _webDriver.Navigate().GoToUrlAsync("https://csgoempire.com/match-betting?bt-path=/cs2-ai/counter-strike-2/de-dust2--bo15--knife-2443129125658038305");

            await Task.Delay(TimeSpan.FromSeconds(10));

            string xPathShadow = "//*[@id=\"bt-inner-page\"]";

            var shadowRoot = _webDriver.FindElement(By.XPath(xPathShadow)).GetShadowRoot();

            // Use a regular expression to match the href pattern
            string pattern = @"/cs2-ai/counter-strike-2/de-dust2--bo15--knife/sas-elite-crew-\d+"; // \d+ matches one or more digits

            // Find all <a> elements
            var links = _webDriver.FindElements(By.TagName("a"));


            var linksInShadowRoot = shadowRoot.FindElements(By.CssSelector("a"));

            _matchResults.RemoveAll(x => x.CreatedDate.AddHours(1) <= DateTimeOffset.UtcNow);

            foreach (var link in linksInShadowRoot)
            {
                string href = link.GetDomAttribute("href");
                if (!string.IsNullOrEmpty(href) && Regex.IsMatch(href, pattern))
                {

                    FrozenSet<MatchResult> matchResults = _matchResults.ToFrozenSet();
                    var found = matchResults.FirstOrDefault(x => x.Url == href);

                    if (found == null)
                    {
                        _logger.LogInformation($"Adding Match Result For Finding: {href}");

                        _matchResults.Add(new MatchResult
                        {
                            Url = href,
                            CTSideScore = 0,
                            TSideScore = 0,
                            MatchCondition = MatchCondition.Queued
                        });
                    }
                }
            }
        }

        public async Task GetOfficialMatchResults()
        {
            var firstMatch = _matchResults.FirstOrDefault(x => !_foundMatchResults.Where(y => y.Url == x.Url).Any());

            if(firstMatch != null)
            {
                await _webDriver.Navigate().GoToUrlAsync($"https://csgoempire.com/match-betting?bt-path={firstMatch.Url}");

                await Task.Delay(TimeSpan.FromSeconds(10));

                string xPathShadow = "//*[@id=\"bt-inner-page\"]";

                var shadowRoot = _webDriver.FindElement(By.XPath(xPathShadow)).GetShadowRoot();

                var resultText = shadowRoot.FindElement(By.CssSelector("[data-editor-id='eventCardStatusLabel']"));

                if(resultText != null)
                {
                    var scores = shadowRoot.FindElements(By.CssSelector("[data-editor-id='scoreBoardScore']"));

                    if (resultText.Text.Contains("Ended"))
                    {
                        _foundMatchResults.Add(new MatchResult
                        {
                            Url = firstMatch.Url,
                            CTSideScore = int.Parse(scores.First().Text),
                            TSideScore = int.Parse(scores.Last().Text),
                            MatchCondition = MatchCondition.Finished
                        });
                    }
                }
            }
        }

        public async Task PostMatchResults()
        {
            foreach (var item in _foundMatchResults)
            {
                var result = await _client.PostJsonAsync<MatchResult>("botstats", item);

                if (result == System.Net.HttpStatusCode.OK)
                {
                    _logger.LogInformation($"Posting Match Result: {item.Url}");   
                }
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
    }
}
