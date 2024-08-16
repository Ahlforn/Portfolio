using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using System.Text.Json;


/**
 * Created by Anders Hofmeister Brønden.
 */


namespace Scraper.ImageService.Services
{
    public class Imgur : ImageServiceAbstract
    {
        private readonly string destination;
        private readonly string clientId;
        private readonly List<string> tags;
        private readonly int pages;
        private readonly string apiURL;

        private Uri GenerateUrl(string tag, int page) {
            string url = this.apiURL.Replace("{{tagName}}", tag);
            url = url.Replace("{{page}}", page.ToString());
            return new Uri(url);
        }

        public async override Task<IResponseModel> QueryAPI() {
            List<Uri> uris = [];
            
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json")
            );
            HttpClient.DefaultRequestHeaders.Add("Authorization", "Client-ID " + this.clientId);
            HttpClient.DefaultRequestHeaders.Add("User-Agent", "Scraper");

            List<Tag> data = [];
            foreach (string tag in this.tags) {
                Tag? tagData = null;
                for(int i = 1; i <= pages; i++)
                {
                    var res = await HttpClient.GetAsync(GenerateUrl(tag, i));

                    if (!res.IsSuccessStatusCode) continue;
                    ResponseData? result = await JsonSerializer.DeserializeAsync<ResponseData>(res.Content.ReadAsStream());

                    if (result == null) continue;

                    if (result.Status == 200 && result.Data != null)
                    {
                        if (tagData == null)
                            tagData = result.Data;
                        else if(tagData.Items != null && result.Data.Items != null)
                            tagData.Items.AddRange(result.Data.Items);
                    }
                }

                if (tagData == null) continue;
                data.Add(tagData);
            }

            ImgurResponseModel model = new()
            {
                Data = data
            };

            return model;
        }

        public Imgur(IConfigurationSection config) : base(config)
        {
            this.destination = config.GetSection("destination").Value ?? "";
            this.clientId = config.GetSection("clientId").Value ?? "";
            this.apiURL = config.GetSection("API_URL").Value ?? "";
            this.tags = new();
            foreach (IConfigurationSection tag in config.GetSection("tags").GetChildren())
            {
                if (tag.Exists() && tag.Value != null)
                    this.tags.Add(tag.Value);
            }
            this.pages = int.Parse(config.GetSection("pages").Value ?? "1");
        }

        public class ImgurResponseModel : IResponseModel {

            public int Status { get; set; }
            public List<Tag>? Data { get; set; }

            public List<Uri> GetData() {
                List<Uri> uris = [];

                foreach (Tag tag in Data ?? [])
                {
                    foreach (TagItem item in tag.Items ?? [])
                    {
                        foreach (Image image in item.Images ?? [])
                        {
                            if (image.Url != null)
                                uris.Add(new Uri(image.Url));
                        }
                    }
                }

                return uris;
            }
        }

        public class Image
        {
            [JsonPropertyName("id")]
            public string Id { get; set; }
            [JsonPropertyName("type")]
            public string? Type { get; set; }
            [JsonPropertyName("width")]
            public int Width { get; set; }
            [JsonPropertyName("height")]
            public int Height { get; set; }
            [JsonPropertyName("size")]
            public int Size { get; set; }
            [JsonPropertyName("link")]
            public string? Url { get; set; }
        }

        public class TagItem {
            [JsonPropertyName("id")]
            public string? Id { get; set; }
            [JsonPropertyName("title")]
            public string? Title { get; set; }
            [JsonPropertyName("description")]
            public string? Description { get; set; }
            [JsonPropertyName("datetime")]
            public int? Datetime { get; set; }
            [JsonPropertyName("cover_width")]
            public int? Width { get; set; }
            [JsonPropertyName("cover_height")]
            public int? Height { get; set; }
            [JsonPropertyName("images")]
            public List<Imgur.Image>? Images { get; set; }
        }

        public class Tag
        {
            [JsonPropertyName("name")]
            public string? Name { get; set; }
            [JsonPropertyName("description")]
            public string? Description { get; set; }
            [JsonPropertyName("items")]
            public List<Imgur.TagItem>? Items { get; set; }
        }

        public class ResponseData
        {
            [JsonPropertyName("data")]
            public Tag? Data { get; set; }
            [JsonPropertyName("success")]
            public bool Success { get; set; }
            [JsonPropertyName("status")]
            public int Status { get; set; }
        }
    }
}
