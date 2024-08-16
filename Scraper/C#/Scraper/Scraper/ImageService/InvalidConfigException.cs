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
    public class InvalidConfigException : Exception
    {
        public InvalidConfigException(string message) : base(message)
        {
        }
    }
}
