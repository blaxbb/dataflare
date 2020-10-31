using DataFlareClient;
using Microsoft.AspNetCore.Mvc;
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
        // POST api/<FlareController>
        [HttpPost]
        public Flare Post(Flare flare)
        {
            if (!FlareStorage.Add(flare))
            {
                throw new Exception("Failed to add flare to storage.");
            }
            return flare;
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
