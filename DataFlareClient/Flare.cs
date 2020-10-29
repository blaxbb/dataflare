﻿using System;
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
        public string Tag { get; set; }
        public string Title { get; set; }
        public string Data { get; set; }
        public DateTime Created { get; set; }

        public Flare(Guid guid, string data)
        {
            Guid = guid;
            Data = data;
        }

        public async Task<bool> Post(string baseUrl)
        {
            try
            {
                using (var handler = new HttpClientHandler())
                {
                    handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                    handler.ServerCertificateCustomValidationCallback = (request, cert, chain, policyErrors) => { return true; };
                    using (var client = new HttpClient(handler))
                    {
                        var content = new StringContent(JsonSerializer.Serialize(this), System.Text.Encoding.UTF8, "application/json");
                        var result = await client.PostAsync(baseUrl, content);
                        var json = await result.Content.ReadAsStringAsync();
                        var flare = JsonSerializer.Deserialize<Flare>(json);
                        Created = flare.Created;
                        Guid = flare.Guid;
                        return true;
                    }
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
                using (var handler = new HttpClientHandler())
                {
                    handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                    handler.ServerCertificateCustomValidationCallback = (request, cert, chain, policyErrors) => { return true; };
                    using (var client = new HttpClient(handler))
                    {
                        var result = await client.GetAsync($"{baseUrl}?guid={guid.ToString()}");
                        var json = await result.Content.ReadAsStringAsync();
                        var flare = JsonSerializer.Deserialize<Flare>(json);
                        return flare;
                    }
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
                using (var handler = new HttpClientHandler())
                {
                    handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                    handler.ServerCertificateCustomValidationCallback = (request, cert, chain, policyErrors) => { return true; };
                    using (var client = new HttpClient(handler))
                    {
                        var result = await client.GetAsync($"{baseUrl}/tag?tag={tag}");
                        var json = await result.Content.ReadAsStringAsync();
                        var flares = JsonSerializer.Deserialize<List<Flare>>(json);
                        return flares;
                    }
                }
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                return new List<Flare>();
            }
        }
    }
}
