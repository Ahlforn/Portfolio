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
    public delegate void ProgressCallback(string msg);
    public interface IImageService
    {
        string ServiceType { get; }
        string Destination { get; }

        Task Scrape(ProgressCallback callback);
    }
}
