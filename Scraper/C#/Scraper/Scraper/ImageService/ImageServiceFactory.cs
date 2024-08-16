using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scraper.ImageService.Services;


/**
 * Created by Anders Hofmeister Brønden.
 */


namespace Scraper.ImageService
{
    public class ImageServiceFactory
    {
        public static IImageService? Build(IConfigurationSection config)
        {
            string type = config.GetSection("type").Value ?? "";

            if(type == "Imgur")
            {
                return new Imgur(config);
            }
            else
            {
                return null;
            }
        }
    }
}
