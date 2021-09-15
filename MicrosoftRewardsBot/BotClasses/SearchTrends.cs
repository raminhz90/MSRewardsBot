using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicrosoftRewardsBot.BotClasses
{
    class SearchTrends
    {
        public List<string> gettrends(string myJsonResponse)
        {
            List<string> searchterms = new List<string>();
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
            var h = myDeserializedClass.@default.trendingSearchesDays.FirstOrDefault().trendingSearches;
            foreach (TrendingSearch item in h)
            {
                searchterms.Add(item.title.query.ToLower());

                foreach (var relatedQuery in item.relatedQueries)
                {
                    searchterms.Add(relatedQuery.query.ToLower());

                }
            }
            return searchterms;
        }

        public class Title
        {
            public string query { get; set; }
            public string exploreLink { get; set; }
        }

        public class RelatedQuery
        {
            public string query { get; set; }
            public string exploreLink { get; set; }
        }

        public class Image
        {
            public string newsUrl { get; set; }
            public string source { get; set; }
            public string imageUrl { get; set; }
        }

        public class Article
        {
            public string title { get; set; }
            public string timeAgo { get; set; }
            public string source { get; set; }
            public Image image { get; set; }
            public string url { get; set; }
            public string snippet { get; set; }
        }

        public class TrendingSearch
        {
            public Title title { get; set; }
            public string formattedTraffic { get; set; }
            public List<RelatedQuery> relatedQueries { get; set; }
            public Image image { get; set; }
            public List<Article> articles { get; set; }
            public string shareUrl { get; set; }
        }

        public class TrendingSearchesDay
        {
            public string date { get; set; }
            public string formattedDate { get; set; }
            public List<TrendingSearch> trendingSearches { get; set; }
        }

        public class Default
        {
            public List<TrendingSearchesDay> trendingSearchesDays { get; set; }
            public string endDateForNextRequest { get; set; }
            public string rssFeedPageUrl { get; set; }
        }

        public class Root
        {
            public Default @default { get; set; }
        }
    }
}
