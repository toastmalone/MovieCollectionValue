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
            

            List<FileInfo> movies = new List<FileInfo>();
            do
            {
                Console.WriteLine("Input Location of Movie Folder");
                moviesFolderLocation = Console.ReadLine();
            } while (Directory.Exists(moviesFolderLocation) == false);

            List<string> videoFilePaths = GetVideoFiles(moviesFolderLocation, "*.mp4|*.mkv|*.wav", SearchOption.AllDirectories);
            List<string> videoFileNames = new List<string>();
            
            //remove small files
            foreach (string vf in videoFilePaths)
            {
                var fi = new FileInfo(vf);
              /*  if (fi.Length > 200000000) */ videoFileNames.Add(fi.Name);
                
            }
            
            for(int i = 0; i < videoFileNames.Count; i++)
            {
                videoFileNames[i] = videoFileNames[i].Replace("_", "+");
                videoFileNames[i] = videoFileNames[i].Replace(" ", "+");
                videoFileNames[i] = $@"https://www.amazon.com/s?k={videoFileNames[i]}&i=instant-video&ref=nb_sb_noss_2";
            }
            

            AmazonScraper scraper = new AmazonScraper(videoFileNames.ToArray());
            scraper.Start();
            Console.WriteLine(scraper.collectionValue);
            Console.ReadLine();

            List<string> GetVideoFiles(string directory, string searchPattern, SearchOption searchOption)
            {
                DirectoryInfo dir = new DirectoryInfo(directory);

                string[] searchPatterns = searchPattern.Split("|");

                List<string> allVideoFiles = new List<string>();

                foreach(string sp in searchPatterns)
                {
                    allVideoFiles.AddRange(Directory.GetFiles(directory, sp, searchOption));
                    for(int i=0; i<allVideoFiles.Count;i++)
                    {
                        allVideoFiles[i] = allVideoFiles[i].Replace(sp.Replace("*", ""), "");
                    }
                }

                return allVideoFiles;
            }
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
            this.Request(movies, Parse);
        }
    }
}
