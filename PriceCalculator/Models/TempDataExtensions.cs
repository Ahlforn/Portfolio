using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Text;

namespace PriceCalculator.Models
{
    public static class TempDataExtensions
    {
        public static void Put<T>(this ITempDataDictionary tempData, string key, T value) where T : class
        {
            MemoryStream stream = new MemoryStream();
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, value);

            StreamReader reader = new StreamReader(stream);
            tempData[key] = reader.ReadToEnd();
        }

        public static T Get<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            object o;
            tempData.TryGetValue(key, out o);

            byte[] data = Encoding.UTF8.GetBytes((string)o);
            MemoryStream stream = new MemoryStream(data);
            IFormatter formatter = new BinaryFormatter();

            T output;
            try
            {
                output = (T)formatter.Deserialize(stream);
            }
            catch(Exception ex)
            {
                output = null;
            }
            return o == null ? null : output;
        }
    }
}
