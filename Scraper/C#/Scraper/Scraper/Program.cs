using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.VisualBasic;
using Scraper.ImageService;

/**
 * Created by Anders Hofmeister Brønden.
 */


IConfiguration settings = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("settings.json")
    .Build();

IConfiguration serviceConfiguration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("services.json")
    .Build();

IEnumerable<IConfigurationSection> services = serviceConfiguration.GetSection("services").GetChildren();

int interval = int.Parse(settings.GetSection("interval").Value ?? "300");

while(true)
{
    Console.WriteLine("Starting scrape...");
    foreach (IConfigurationSection service in services)
    {
        IImageService? imageService = ImageServiceFactory.Build(service);
        if (imageService != null)
        {
            await imageService.Scrape((msg) => { Console.WriteLine(msg); });
        }
    }
    Console.WriteLine($"Sleeping for {interval} seconds...");
    Thread.Sleep(interval * 1000);
}