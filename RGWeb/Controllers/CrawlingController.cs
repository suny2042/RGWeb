using Microsoft.AspNetCore.Mvc;
using RGWeb.Shared;
using RGWeb.ViewModels;
using static RGWeb.Shared.Models.ContentModel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RGWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CrawlingController : ControllerBase
    {
        // GET: api/<CrawlingController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<CrawlingController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<CrawlingController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }


        [HttpPost("Crawler")]
        public string Post_Crawler([FromBody] Payload value)
        {
            StaticData.sContent.Clear();
            ServerInfo.ServerRefreshTime = value.crawlingRefreshTime;

            for (int i = 0; i < value.content.Count; i++)
            {
                ModelDataSet<Content> mds = new();
                for (int j=0; j < value.content[i].Count; j++)
                {
                    mds.InsertRow(value.content[i][j]);
                }    
                StaticData.sContent.Add(mds);
            }

            return "valuePost";
        }

        // PUT api/<CrawlingController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<CrawlingController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
