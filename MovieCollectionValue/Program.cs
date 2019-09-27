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

            do
            {
                Console.WriteLine("Input Location of Movie Folder");
                moviesFolderLocation = Console.ReadLine();
            } while (Directory.Exists(moviesFolderLocation) == false);

            List<string> movies = GetVideoFiles(moviesFolderLocation, "*.mkv|*.mp4|*.wav", SearchOption.AllDirectories);

            AmazonScraper scraper = new AmazonScraper(movies.ToArray());
            scraper.Start();
            Console.WriteLine(scraper.collectionValue);
            Console.ReadLine();

            List<string> GetVideoFiles(string directory, string searchPattern, SearchOption searchOption)
            {
                List<string> videoFileNames = new List<string>();

                List<string> allVideoFiles = new List<string>();

                DirectoryInfo dir = new DirectoryInfo(directory);

                string[] searchPatterns = searchPattern.Split("|");

                foreach (string sp in searchPatterns)
                {
                    allVideoFiles.AddRange(Directory.GetFiles(directory, sp, searchOption));
                }

                foreach (string vf in allVideoFiles)
                {
                    var fi = new FileInfo(vf);
                    if (fi.Length > 200000000) videoFileNames.Add(fi.Name);
                }

                foreach(string sp in searchPatterns)
                {
                    for(int i=0;i<videoFileNames.Count;i++)
                    {
                        videoFileNames[i] = videoFileNames[i].Replace(sp.Replace("*",""), "");
                        videoFileNames[i] = videoFileNames[i].Replace("_", "+");
                        videoFileNames[i] = videoFileNames[i].Replace(" ", "+");
                    }
                }

                for(int i=0;i<videoFileNames.Count;i++)
                {
                    videoFileNames[i] = videoFileNames[i].Replace(".", "+");
                    videoFileNames[i] = $@"https://www.amazon.com/s?k={videoFileNames[i]}&i=instant-video&ref=nb_sb_noss_2";
                }

                return videoFileNames;
            }
        }

    }

    class AmazonScraper : WebScraper
    {
        string[] movies;
        public double collectionValue = 0.0;
        public override void Init()
        {
            this.LoggingLevel = WebScraper.LogLevel.All;
            this.Request(movies, Parse);
        }

        public override void Parse(Response response)
        {
            HtmlNode[] nodes = response.Css("#search  div > div:nth-child(2) > div:nth-child(1) > div > div > div.a-row.a-size-base.a-color-secondary > div > a > span");
            string price = nodes != null ? nodes[0].InnerHtml.ToString() : "0.0";

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
            this.movies = movies;
        }
    }
}
