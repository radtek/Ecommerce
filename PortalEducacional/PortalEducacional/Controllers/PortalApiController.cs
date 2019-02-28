using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using PortalEducacional.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PortalEducacional.Controllers
{
    [Route("api/[controller]")]
    public class PortalApiController : Controller
    {
        private readonly ApplicationDbContext _context;
        public PortalApiController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet]
        [Route("GetPDF/{identificador}")]
        public ActionResult GetPDF(string identificador)
        {
            var novoIdentificador = int.Parse(identificador);
            var repositorioPDF = _context.RepositorioPDF.FirstOrDefault(x => x.ID == novoIdentificador);
            var mem = new MemoryStream(repositorioPDF.PDF);

            return File(mem, "application/pdf"); ;           
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
