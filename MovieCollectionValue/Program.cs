using System;
using System.IO;
using IronWebScraper;

namespace MovieCollectionValue
{
    class Program
    {
        static void Main(string[] args)
        {
            string folderLocation;

            folderLocation = Console.ReadLine();

        }
    }

    class AmazonScraper : WebScraper
    {
        public override void Init()
        {
            this.LoggingLevel = WebScraper.LogLevel.All;
            this.Request("amazon.com", Parse);
        }

        public override void Parse(Response response)
        {
            throw new NotImplementedException();
        }
    }
}
