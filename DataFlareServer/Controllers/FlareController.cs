using DataFlareClient;
using DataFlareServer.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DataFlareServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlareController : ControllerBase
    {
        IHubContext<FlareHub> hubContext;
        public FlareController(IHubContext<FlareHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        // POST api/<FlareController>
        [HttpPost]
        public async Task<Flare> Post(Flare flare)
        {
            if (!FlareStorage.Add(flare))
            {
                throw new Exception("Failed to add flare to storage.");
            }
            try
            {
                await hubContext?.Clients?.All?.SendAsync($"flare-{flare.Tag}", flare);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return flare;
        }

        [HttpGet("stats")]
        public IEnumerable<object> Stats()
        {
            return FlareStorage.Stats();
        }


        // GET: api/<FlareController>/tag
        [HttpGet("tag/{tag}")]
        public IEnumerable<Flare> Get(string tag)
        {
            return FlareStorage.GetTag(tag);
        }

        // GET api/<FlareController>/5
        [HttpGet]
        public Flare? Get(Guid guid)
        {
            return FlareStorage.Get(guid);
        }

        // GET api/<FlareController>/5
        [HttpGet("shortcode/{shortCode}")]
        public Flare? GetShortCode(int shortCode)
        {
            return FlareStorage.GetShortCode(shortCode);
        }
    }
}
