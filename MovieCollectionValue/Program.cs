using System;
using System.Collections.Generic;
using System.IO;
using IronWebScraper;

namespace MovieCollectionValue
{
    class Program
    {
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
            }
                AmazonScraper scraper = new AmazonScraper(movies);
                scraper.Start();
        }

    }

    class AmazonScraper : WebScraper
    {
        double value = 0.0;
        List<string> _movies = new List<string>();
        public override void Init()
        {
            this.LoggingLevel = WebScraper.LogLevel.All;
            this.Request(_movies, Parse);
        }

        public override void Parse(Response response)
        {
            HtmlNode[] nodes = response.Css("#search  div > div:nth-child(2) > div:nth-child(1) > div > div > div.a-row.a-size-base.a-color-secondary > div > a > span");
            foreach (HtmlNode node in nodes)
            {
                Console.WriteLine(node.ChildNodes[0].TextContent);
            }
        }

        public AmazonScraper(string[] movies)
        {
            
          for(int i = 0; i < movies.Length; i++)
          {
                _movies.Add($@"https://www.amazon.com/s?k={movies[i]}&i=instant-video&ref=nb_sb_noss_2");
          }

           
        }
    }
}
