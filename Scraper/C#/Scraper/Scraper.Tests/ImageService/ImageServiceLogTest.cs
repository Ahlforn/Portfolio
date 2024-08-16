using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Scraper.ImageService;


/**
 * Created by Anders Hofmeister Brønden.
 */


namespace Scraper.Tests.ImageService
{
    internal class ImageServiceLogTest
    {
        private string _basePath = "ImageServiceLogTest-";

        [Test]
        public void ConstructorLogFileDoesNotExists()
        {
            string path = _basePath + "ConstructorLogFileDoesNotExists";

            Directory.CreateDirectory(path);

            var log = new ImageServiceLog(path);

            Assert.That(log.EntryCount, Is.EqualTo(0));

            log.Add(new Uri("https://192.168.0.1/test1.png"), "test1.png");
            log.Add(new Uri("https://192.168.0.1/test2.png"), "test2.png");
            log.Add(new Uri("https://192.168.0.1/test3.png"), "test3.png", DateTime.Now);
            log.Save();

            Assert.Multiple(() =>
            {
                Assert.That(log.EntryCount, Is.EqualTo(3));
                Assert.That(log.Contains("test1.png"), Is.EqualTo(true));
                Assert.That(log.Contains(new Uri("https://192.168.0.1/test2.png")), Is.EqualTo(true));
                Assert.That(log.Contains("test4.png"), Is.EqualTo(false));
            });

            Directory.Delete(path, true);
        }

        [Test]

        public void ConstructorLogFileExists()
        {
            string path = _basePath + "ConstructorLogFileExists";

            Directory.CreateDirectory(path);

            string json = """
                {
                    "Entries": [
                        {
                            "Filename": "test1.png",
                            "Timestamp": "2024-08-11T22:45:40.3662926+02:00",
                            "Uri": "https://192.168.0.1/test1.png"
                        },
                        {
                            "Filename": "test2.png",
                            "Timestamp": "2024-08-11T22:45:40.3665053+02:00",
                            "Uri": "https://192.168.0.1/test2.png"
                        },
                        {
                            "Filename": "test3.png",
                            "Timestamp": "2024-08-11T22:45:40.3665075+02:00",
                            "Uri": "https://192.168.0.1/test3.png"
                        }
                    ]
                }
            """;

            using StreamWriter output = new(Path.Combine(path, "log.json"));
            output.Write(json);
            output.Dispose();

            var log = new ImageServiceLog(path);

            Assert.Multiple(() =>
            {
                Assert.That(log.EntryCount, Is.EqualTo(3));
                Assert.That(log.Contains("test1.png"), Is.EqualTo(true));
                Assert.That(log.Contains(new Uri("https://192.168.0.1/test2.png")), Is.EqualTo(true));
                Assert.That(log.Contains("test4.png"), Is.EqualTo(false));
            });

            Directory.Delete(path, true);
        }

        [Test]
        public void RemoveEntry()
        {
            string path = _basePath + "RemoveEntry";

            Directory.CreateDirectory(path);

            string json = """
                {
                    "Entries": [
                        {
                            "Filename": "test1.png",
                            "Timestamp": "2024-08-11T22:45:40.3662926+02:00",
                            "Uri": "https://192.168.0.1/test1.png"
                        },
                        {
                            "Filename": "test2.png",
                            "Timestamp": "2024-08-11T22:45:40.3665053+02:00",
                            "Uri": "https://192.168.0.1/test2.png"
                        }
                    ]
                }
            """;

            using StreamWriter output = new(Path.Combine(path, "log.json"));
            output.Write(json);
            output.Dispose();

            var log = new ImageServiceLog(path);

            Assert.Multiple(() =>
            {
                Assert.That(log.EntryCount(), Is.EqualTo(2));
                Assert.That(log.Contains(new Uri("https://192.168.0.1/test1.png")), Is.EqualTo(true));

                log.Remove(new Uri("https://192.168.0.1/test1.png"));

                Assert.That(log.EntryCount(), Is.EqualTo(1));
                Assert.That(log.Contains(new Uri("https://192.168.0.1/test1.png")), Is.EqualTo(false));
                Assert.That(log.Contains("test2.png"), Is.EqualTo(true));

                log.Remove("test2.png");

                Assert.That(log.Contains("test2.png"), Is.EqualTo(false));
                Assert.That(log.EntryCount(), Is.EqualTo(0));
            });

            Directory.Delete(path, true);
        }
    }
}
