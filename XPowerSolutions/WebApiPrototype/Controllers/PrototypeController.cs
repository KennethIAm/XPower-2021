using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiPrototype.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrototypeController : ControllerBase
    {
        private List<Guid> _publicGuids = new()
        {
            Guid.NewGuid()
        };

        // GET: api/<PrototypeController>
        [HttpGet]
        public IEnumerable<Guid> Get()
        {
            return _publicGuids;
        }

        // GET api/<PrototypeController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return _publicGuids.FirstOrDefault().ToString();
        }

        // POST api/<PrototypeController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
            _publicGuids.Add(Guid.Parse(value));
        }

        // PUT api/<PrototypeController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<PrototypeController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _publicGuids.RemoveAt(id);
        }
    }
}
