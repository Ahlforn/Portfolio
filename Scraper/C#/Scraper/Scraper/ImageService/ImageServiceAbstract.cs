using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/**
 * Created by Anders Hofmeister Brønden.
 */


namespace Scraper.ImageService
{
    public abstract class ImageServiceAbstract : IImageService
    {
        public string ServiceType { get; }
        public string Destination { get; }
        public int ConcurrentDownloads { get; set; }
        public HttpClient HttpClient { get; set; }

        private readonly ImageServiceLog _downloadLog;

        public ImageServiceAbstract(IConfigurationSection config)
        {
            ServiceType = config.GetSection("type").Value ?? "";
            Destination = config.GetSection("destination").Value ?? "";
            ConcurrentDownloads = config.GetSection("concurrentDownloads").Value != null ? Convert.ToInt32(config.GetSection("concurrentDownloads").Value) : 1;
            HttpClient = new HttpClient();

            if (ServiceType == string.Empty || Destination == string.Empty)
                throw new InvalidConfigException("Invalid config. Missing ServiceType or Destination.");
            
            if(!Path.Exists(Destination))
                throw new InvalidConfigException("Destination directory does not exist.");

            _downloadLog = new ImageServiceLog(Destination);
        }

        public async Task DownloadFile(Uri url) {
            if (_downloadLog.Contains(url))
                return;

            Console.WriteLine("Starting download of " + url);
            var stream = await HttpClient.GetStreamAsync(url);
            string filename;
            if (url.IsFile) {
                filename = Path.GetFileName(url.LocalPath);
            }
            else {
                filename = url.AbsolutePath.Split('/').Last();
            }

            var filestream = new FileStream(Path.Combine(Destination, filename), FileMode.CreateNew);
            await stream.CopyToAsync(filestream);
            await filestream.DisposeAsync();
            await stream.DisposeAsync();
            Console.WriteLine("Finished download of " + url);

            _downloadLog.Add(url, filename);
        }

        public abstract Task<IResponseModel> QueryAPI();

        public async Task Scrape(ProgressCallback callback)
        {
            IResponseModel res = await QueryAPI();
            ThreadPool.SetMaxThreads(ConcurrentDownloads, ConcurrentDownloads);

            List<Uri> uris = res.GetData() ?? [];
            int running = uris.Count;
            AutoResetEvent done = new(false);

            foreach (Uri uri in uris)
            {
                if(!_downloadLog.Contains(uri))
                {
                    callback($"Downloading {running} of {uris.Count}");
                    ThreadPool.QueueUserWorkItem(state => {
                        if (state != null)
                        {
                            Task.Run(async () => await DownloadFile((Uri)state)).Wait();
                        }
                        if (0 == Interlocked.Decrement(ref running))
                        {
                            done.Set();
                        }
                    }, uri);
                }
                else
                {
                    callback($"Skipping {running} of {uris.Count}");
                    if (0 == Interlocked.Decrement(ref running))
                    {
                        done.Set();
                    }
                }
            }
            done.WaitOne();

            _downloadLog.Save();
        }
    }
}
