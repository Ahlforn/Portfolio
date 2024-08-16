using System;
using System.Collections.Generic;
using System.IO.Enumeration;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


/**
 * Created by Anders Hofmeister Brønden.
 */


namespace Scraper.ImageService
{
    public class ImageServiceLog
    {
        private string _destination;
        private Log _log;
        private string _filename = "log.json";
        private SpinLock _lock = new();

        public ImageServiceLog(string destination)
        {
            this._destination = destination;
            this._log = LoadLogFile(destination) ?? new Log();
        }

        private Log? LoadLogFile(string destination)
        {
            if (!File.Exists(Path.Combine(destination, this._filename)))
                return null;

            string json = File.ReadAllText(Path.Combine(destination, this._filename));
            return JsonSerializer.Deserialize<Log>(json);
        }

        public void Add(Uri uri, string filename, DateTime timestamp)
        {
            bool gotLock = false;
            try
            {
                _lock.Enter(ref gotLock);
                var record = new ImageRecord
                {
                    Filename = filename,
                    Uri = uri,
                    Timestamp = timestamp
                };
                this._log.Entries.Add(record);
            }
            finally
            {
                if(gotLock)
                    _lock.Exit();
            }
        }

        public void Add(Uri uri, string filename)
        {
            this.Add(uri, filename, DateTime.Now);
        }

        public void Remove(Uri uri)
        {
            bool gotLock = false;
            try
            {
                _lock.Enter(ref gotLock);
                this._log.Entries.RemoveAll(x => x.Uri == uri);
            }
            finally
            {
                if(gotLock)
                    _lock.Exit();
            }
        }

        public void Remove(string filename)
        {
            bool gotLock = false;
            try
            {
                _lock.Enter(ref gotLock);
                this._log.Entries.RemoveAll(x => x.Filename == filename);
            }
            finally
            {
                if(gotLock)
                    _lock.Exit(gotLock);
            }
            
        }

        public bool Contains(Uri uri)
        {
            bool gotLock = false;
            try
            {
                _lock.Enter(ref gotLock);
                return this._log.Entries.Any(x => x.Uri == uri);
            }
            finally
            {
                if(gotLock)
                    _lock.Exit();
            }
        }

        public bool Contains(string filename)
        {
            bool gotLock = false;
            try
            {
                _lock.Enter(ref gotLock);
                return this._log.Entries.Any(x => x.Filename == filename);
            }
            finally
            {
                if(gotLock)
                    _lock.Exit();
            }
        }

        public int EntryCount()
        {
            return this._log.Entries.Count;
        }

        public void Save()
        {
            bool gotLock = false;
            try
            {
                _lock.Enter(ref gotLock);
                string json = JsonSerializer.Serialize(this._log);
                File.WriteAllText(Path.Combine(this._destination, this._filename), json);
            }
            finally
            {
                if(gotLock)
                    _lock.Exit();
            }
        }

        internal class Log
        {
            public List<ImageRecord> Entries { get; set; } = [];
        }

        internal class ImageRecord
        {
            public string Filename { get; set; }
            public Uri Uri { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }
}
