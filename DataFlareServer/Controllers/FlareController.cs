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
            FlareStorage.Add(flare);
            return flare;
        }


        // GET: api/<FlareController>/tag
        [HttpGet("tag")]
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
        [HttpGet("zzz")]
        public async Task<List<Flare>> ZZZ(string tag)
        {
            return await Flare.GetTag("https://localhost:44308/api/Flare", tag);
        }
    }
}
