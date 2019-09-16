using System;
using System.Collections.Generic;
using System.IO;
using IronWebScraper;

namespace MovieCollectionValue
{
    class Program
    {
        public double value = 0.0;
        static void Main(string[] args)
        {
            string moviesFolderLocation;
            string[] movies;
            do
            {
                Console.WriteLine("Input Location of Movie Folder");
                moviesFolderLocation = Console.ReadLine();
            } while (Directory.Exists(moviesFolderLocation) == false);

            movies = Directory.GetDirectories(moviesFolderLocation);
            for(int i = 0; i < movies.Length; i++)
            {
                movies[i] = movies[i].Replace(moviesFolderLocation + "\\", "");
                movies[i] = movies[i].Replace(" ", "+");
                movies[i] = $@"https://www.amazon.com/s?k={movies[i]}&i=instant-video&ref=nb_sb_noss_2";
            }

            AmazonScraper scraper = new AmazonScraper(movies);
            scraper.Start();
            Console.WriteLine(scraper.collectionValue);
            Console.ReadLine();
        }

    }

    class AmazonScraper : WebScraper
    {
        public double collectionValue = 0.0;
        public override void Init()
        {
            this.LoggingLevel = WebScraper.LogLevel.All;
        }

        public override void Parse(Response response)
        {
            HtmlNode[] nodes = response.Css("#search  div > div:nth-child(2) > div:nth-child(1) > div > div > div.a-row.a-size-base.a-color-secondary > div > a > span");
            string price = nodes[0].InnerHtml.ToString();
            if(price.Contains("to buy"))
            {
                double movieValue = 0.0;
                price = price.Replace("from $", "");
                price = price.Replace("to buy", "");
                if(double.TryParse(price, out movieValue))
                {
                    collectionValue += movieValue;
                }

                
            }
        }

        public AmazonScraper(string[] movies)
        {
            this.Request(movies, Parse);
        }
    }
}
