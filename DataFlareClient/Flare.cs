using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace DataFlareClient
{
    public class Flare
    {
        public Guid Guid { get; set; }
        public int ShortCode { get; set; }
        public string Tag { get; set; }
        public string Title { get; set; }
        public string Data { get; set; }
        public DateTime Created { get; set; }

        public string Signature { get; set; }

        private object value = null;
        public object Value(Type type) {
            if (value == null)
            {
                value = JsonSerializer.Deserialize(Data, type);
            }
            return value;
        }

        public Flare()
        {

        }

        public Flare(string data)
        {
            Data = data;
        }

        public async Task<bool> Post(string baseUrl)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var content = new StringContent(JsonSerializer.Serialize(this), System.Text.Encoding.UTF8, "application/json");
                    var result = await client.PostAsync(baseUrl, content);
                    var json = await result.Content.ReadAsStringAsync();
                    var flare = JsonSerializer.Deserialize<Flare>(json);
                    Created = flare.Created;
                    Guid = flare.Guid;
                    ShortCode = flare.ShortCode;
                    return true;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }

            return false;
        }

        public static async Task<Flare> Get(string baseUrl, Guid guid)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var result = await client.GetAsync($"{baseUrl}?guid={guid.ToString()}");
                    var json = await result.Content.ReadAsStringAsync();
                    var flare = JsonSerializer.Deserialize<Flare>(json);
                    return flare;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                return null;
            }
        }
        public static async Task<List<Flare>> GetTag(string baseUrl, string tag)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var result = await client.GetAsync($"{baseUrl}/tag/{tag}");
                    var json = await result.Content.ReadAsStringAsync();
                    var flares = JsonSerializer.Deserialize<List<Flare>>(json);
                    return flares;
                }
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                return new List<Flare>();
            }
        }

        public static async Task<Flare> GetShortCode(string baseUrl, string shortCode)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var result = await client.GetAsync($"{baseUrl}/shortcode/{shortCode}");
                    var json = await result.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<Flare>(json);
                }
                
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                return null;
            }
        }
    }
}
