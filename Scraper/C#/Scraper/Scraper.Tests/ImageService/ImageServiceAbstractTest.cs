namespace Scraper.Tests.ImageService;

using NUnit.Framework;
using Scraper.ImageService;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Threading.Tasks;

using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;


/**
 * Created by Anders Hofmeister Brønden.
 */


public class ImageServiceAbstractTest
{
    [SetUp]
    public void Setup()
    {
        
    }

    [Test]
    public void ConstructorSuccesfulParseConfig()
    {
        Directory.CreateDirectory("./ImageServiceAbstractTest-ConstructorSuccesfulParseConfig");

        string? json =
        @"
        {
            ""service"": {
                ""type"": ""ImageService"",
                ""concurrentDownloads"": 10,
                ""destination"": ""./ImageServiceAbstractTest-ConstructorSuccesfulParseConfig""
            }
        }
        ";

        MemoryStream jsonStream = new MemoryStream(Encoding.UTF8.GetBytes(json.ToString() ?? ""));
        IConfigurationSection config = new ConfigurationBuilder().AddJsonStream(jsonStream).Build().GetSection("service");

        ImageServiceAbstract imageService = new ImageServiceAbstractTestClass(config);
        Assert.That(imageService, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(imageService.ServiceType, Is.EqualTo("ImageService"));
            Assert.That(imageService.Destination, Is.EqualTo("./ImageServiceAbstractTest-ConstructorSuccesfulParseConfig"));
            Assert.That(imageService.ConcurrentDownloads, Is.EqualTo(10));
        });

        Directory.Delete("./ImageServiceAbstractTest-ConstructorSuccesfulParseConfig", true);
    }

    [Test]
    public void ConstructorFailedWithNoServiceType() {
        Directory.CreateDirectory("./ImageServiceAbstractTest-ConstructorFailedWithNoServiceType");

        string? json =
        @"
        {
            ""service"": {
               ""type"": """",
                ""destination"": ""./ImageServiceAbstractTest-ConstructorFailedWithNoServiceType""
            }
        }
        ";

        MemoryStream jsonStream = new MemoryStream(Encoding.UTF8.GetBytes(json.ToString() ?? ""));
        IConfigurationSection config = new ConfigurationBuilder().AddJsonStream(jsonStream).Build().GetSection("service");

        Assert.Throws<InvalidConfigException>(() => new ImageServiceAbstractTestClass(config));

        Directory.Delete("./ImageServiceAbstractTest-ConstructorFailedWithNoServiceType", true);
    }

    [Test]
    public void ConstructorWithNoConcurrentDownloads()
    {
        Directory.CreateDirectory("./ImageServiceAbstractTest-ConstructorWithNoConcurrentDownloads");

        string? json =
        @"
        {
            ""service"": {
               ""type"": ""service"",
                ""destination"": ""./ImageServiceAbstractTest-ConstructorWithNoConcurrentDownloads""
            }
        }
        ";

        MemoryStream jsonStream = new MemoryStream(Encoding.UTF8.GetBytes(json.ToString() ?? ""));
        IConfigurationSection config = new ConfigurationBuilder().AddJsonStream(jsonStream).Build().GetSection("service");

        Assert.That(new ImageServiceAbstractTestClass(config).ConcurrentDownloads, Is.EqualTo(1));

        Directory.Delete("./ImageServiceAbstractTest-ConstructorWithNoConcurrentDownloads", true);
    }

    [Test]
    public void ConstructorFailedWithNoDestination() {
        Directory.CreateDirectory("./ImageServiceAbstractTest-ConstructorFailedWithNoDestination");

        string? json = 
        @"
        {
            ""service"": {
               ""type"": ""service"",
                ""destination"": """"
            }
        }
        ";

        MemoryStream jsonStream = new MemoryStream(Encoding.UTF8.GetBytes(json.ToString() ?? ""));
        IConfigurationSection config = new ConfigurationBuilder().AddJsonStream(jsonStream).Build().GetSection("service");

        Assert.Throws<InvalidConfigException>(() => new ImageServiceAbstractTestClass(config));

        Directory.Delete("./ImageServiceAbstractTest-ConstructorFailedWithNoDestination", true);
    }

    [Test]
    public void ConstructorFailedWithMalformedConfig() {
        Directory.CreateDirectory("./ImageServiceAbstractTest-ConstructorFailedWithMalformedConfig");

        string? json = 
        @"
        {
            ""service"": {
               ""t"": ""service"",
            }
        }
        ";

        MemoryStream jsonStream = new MemoryStream(Encoding.UTF8.GetBytes(json.ToString() ?? ""));
        IConfigurationSection config = new ConfigurationBuilder().AddJsonStream(jsonStream).Build().GetSection("service");

        Assert.Throws<InvalidConfigException>(() => new ImageServiceAbstractTestClass(config));

        Directory.Delete("./ImageServiceAbstractTest-ConstructorFailedWithMalformedConfig", true);
    }

    [Test]
    public void ConstructorFailedWithNonExistingDestination() {
        string? json = 
        @"
        {
            ""service"": {
               ""type"": ""service"",
                ""destination"": ""./WrongDirectory""
            }
        }
        ";

        MemoryStream jsonStream = new MemoryStream(Encoding.UTF8.GetBytes(json.ToString() ?? ""));
        IConfigurationSection config = new ConfigurationBuilder().AddJsonStream(jsonStream).Build().GetSection("service");

        Assert.Throws<InvalidConfigException>(() => new ImageServiceAbstractTestClass(config));
    }

    [Test]
    public void DownloadTest() {
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("Test File", Encoding.UTF8, "text/plain"),
            });

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);

        Directory.CreateDirectory("./ImageServiceAbstractTest-DownloadTest");
        
        string? json =
        @"
        {
            ""service"": {
               ""type"": ""ImageService"",
                ""destination"": ""./ImageServiceAbstractTest-DownloadTest""
            }
        }
        ";

        MemoryStream jsonStream = new MemoryStream(Encoding.UTF8.GetBytes(json.ToString() ?? ""));
        IConfigurationSection config = new ConfigurationBuilder().AddJsonStream(jsonStream).Build().GetSection("service");

        ImageServiceAbstract imageService = new ImageServiceAbstractTestClass(config)
        {
            HttpClient = httpClient
        };

        imageService.DownloadFile(new Uri("https://localhost/test.txt"));

        Assert.That(File.Exists("./ImageServiceAbstractTest-DownloadTest/test.txt"), Is.True);

        Directory.Delete("./ImageServiceAbstractTest-DownloadTest", true);
    }

    [TearDown]
    public void CleanUp() {
        
    }

    public class ImageServiceAbstractTestClass(IConfigurationSection config) : ImageServiceAbstract(config)
    {
        public override Task<IResponseModel> QueryAPI()
        {
            throw new NotImplementedException();
        }
    }

    public class APIReturnModel : IResponseModel{
        public string? Status { get; set; }
        public List<Uri>? Data { get; set; }
        public List<Uri> GetData() {
            return this.Data ?? new List<Uri>();
        }
    }
}