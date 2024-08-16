using NUnit.Framework;
using Microsoft.Extensions.Configuration;
using System.Text;
using Scraper.ImageService;
using Scraper.ImageService.Services;


/**
 * Created by Anders Hofmeister Brønden.
 */


namespace Scraper.Tests.ImageService
{
    [TestFixture]
    public class ImageServiceFactoryTests
    {
        [SetUp]
        public void Setup()
        {
            Directory.CreateDirectory("./ImageServiceFactoryTests");
        }

        [Test]
        public void Build_ReturnsImgurService_WhenTypeIsImgur()
        {
            // Arrange
            string? json = 
            @"
            {
                ""service"": {
                    ""type"": ""Imgur"",
                    ""destination"": ""./ImageServiceFactoryTests""
                }
            }
            ";

            MemoryStream jsonStream = new(Encoding.UTF8.GetBytes(json.ToString() ?? ""));
            IConfigurationSection config = new ConfigurationBuilder().AddJsonStream(jsonStream).Build().GetSection("service");
            
            // Act
            var service = ImageServiceFactory.Build(config);

            // Assert
            Assert.That(service, Is.Not.Null);
            Assert.That(service, Is.InstanceOf<Imgur>());
        }

        [Test]
        public void Build_ReturnsNull_WhenTypeIsNotImgur()
        {
            // Arrange
            Dictionary<string, string?>? cfg = new()
            {
                { "service:type", "NotImgur" }
            };

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(cfg)
                .Build();

            // Act
            var service = ImageServiceFactory.Build(config.GetSection("service"));

            // Assert
            Assert.That(service, Is.Null);
        }

        [TearDown]
        public void CleanUp() {
            Directory.Delete("./ImageServiceFactoryTests", true);
        }
    }
}