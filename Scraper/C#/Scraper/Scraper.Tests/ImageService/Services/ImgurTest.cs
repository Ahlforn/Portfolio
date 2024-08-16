using NUnit.Framework;
using Scraper.ImageService.Services;
using Microsoft.Extensions.Configuration;
using System.Text;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;


/**
 * Created by Anders Hofmeister Brønden.
 */


namespace Scraper.Tests.ImageService.Services
{
    [TestFixture]
    public class ImgurTests
    {
        [Test]
        public void Constructor()
        {
            string json =
            """
                {
                    "service": {
                        "type": "Imgur",
                        "destination": "imgurConstructorTest",
                        "concurrentDownloads": 10,
                        "clientId": "e7884d8510fb09d",
                        "tags": [ "landscape" ],
                        "API_URL": "https://api.imgur.com/3/gallery/t/{{tagName}}/{{sort}}/{{window}}/{{page}}"
                    }
                }
            """;

            MemoryStream jsonStream = new(Encoding.UTF8.GetBytes(json));

            IConfigurationSection config = new ConfigurationBuilder()
                .AddJsonStream(jsonStream)
                .Build()
                .GetSection("service");

            string path = config.GetSection("destination").Value ?? "";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var imgur = new Imgur(config);

            Assert.Multiple(() =>
            {
                Assert.That(imgur.ServiceType, Is.Not.Empty);
                Assert.That(imgur.Destination, Is.Not.Empty);
                Assert.That(imgur.HttpClient, Is.Not.Null);
                Assert.That(imgur.ConcurrentDownloads, Is.EqualTo(10));
            });

            Directory.Delete(path, true);
        }

        [Test]
        public async Task QueryAPI_SuccessfulConnectionToImgur()
        {
            // Arrange
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("services.json")
                .Build();

            IEnumerable<IConfigurationSection> services = config.GetSection("services").GetChildren();

            foreach (IConfigurationSection service in services) {
                string type = service.GetSection("type").Value ?? "";
                string destination = service.GetSection("destination").Value + "QueryAPI_SuccessfulConnectionToImgur";

                if(destination != "" && type == "Imgur") {
                    if (!Directory.Exists(destination))
                        Directory.CreateDirectory(destination);

                    var imgur = new Imgur(service);

                    // Act
                    var result = await imgur.QueryAPI();

                    // Assert
                    Assert.Multiple(() =>
                    {
                        Assert.That(imgur.ServiceType, Is.Not.Empty);
                        Assert.That(imgur.Destination, Is.Not.Empty);
                        Assert.That(imgur.HttpClient, Is.Not.Null);

                        Assert.That(result, Is.Not.Null);
                        Assert.That(result, Is.InstanceOf<Imgur.ImgurResponseModel>());
                    });

                    Directory.Delete(destination, true);
                }
            }
        }

        [Test]
        public async Task QueryAPI_DataHasIntegrety()
        {
            // Arrange
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("services.json")
                .Build();

            IEnumerable<IConfigurationSection> services = config.GetSection("services").GetChildren();

            foreach (IConfigurationSection service in services)
            {
                string type = service.GetSection("type").Value ?? "";
                string destination = service.GetSection("destination").Value + "_QueryAPI_DataHasIntegrety"; ;

                if (destination != "" && type == "Imgur")
                {
                    var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

                    mockHttpMessageHandler
                        .Protected()
                        .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                        .ReturnsAsync(new HttpResponseMessage
                        {
                            StatusCode = HttpStatusCode.OK,
                            Content = new StringContent(File.ReadAllText("ImgurDataSample.json"))
                        });

                    var httpClient = new HttpClient(mockHttpMessageHandler.Object);

                    if (!Directory.Exists(destination))
                        Directory.CreateDirectory(destination);

                    var imgur = new Imgur(service);
                    imgur.HttpClient = httpClient;

                    // Act
                    var result = await imgur.QueryAPI();

                    // Assert
                    Assert.Multiple(() =>
                    {
                        Assert.That(imgur.ServiceType, Is.Not.Empty);
                        Assert.That(imgur.Destination, Is.Not.Empty);
                        Assert.That(imgur.HttpClient, Is.Not.Null);
                        
                        Assert.That(result, Is.Not.Null);
                        Assert.That(result, Is.InstanceOf<Imgur.ImgurResponseModel>());
                        Assert.That(result.GetData().Count, Is.AtLeast(1));
                    });

                    Directory.Delete(destination, true);
                }
            }
        }
    }
}