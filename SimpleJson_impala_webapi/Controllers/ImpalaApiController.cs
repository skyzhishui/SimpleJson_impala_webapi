using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SimpleJson_impala_webapi.Controllers
{
    [ApiController]
    [Route("/")]
    public class routeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            Response.Headers.Add("Access-Control-Allow-Headers", "accept, content-type");
            Response.Headers.Add("Access-Control-Allow-Methods", "POST");
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            return Ok("200 ok");
        }
    }
    [ApiController]
    [Route("[controller]")]
    public class searchController : ControllerBase
    {
        [HttpPost]
        public List<string> Post(dynamic req)
        {
            Response.Headers.Add("Access-Control-Allow-Headers", "accept, content-type");
            Response.Headers.Add("Access-Control-Allow-Methods", "POST");
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            return App.sqllist.Keys.ToList<string>();
        }
    }
    [ApiController]
    [Route("[controller]")]
    public class queryController : ControllerBase
    {
        [HttpPost]
        public List<object> Post(object req)
        {
            Response.Headers.Add("Access-Control-Allow-Headers", "accept, content-type");
            Response.Headers.Add("Access-Control-Allow-Methods", "POST");
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            List<object> result = new List<object>();
            try
            {
                var json = JsonConvert.DeserializeObject<JObject>(req.ToString());
                var targets = json["targets"].Value<JArray>();
                if (targets == null)
                    return result;
                for (int i = 0; i < targets.Count; i++)
                {
                    string target = targets[i]["target"].ToString();
                    string type = targets[i]["type"].ToString();
                    result.Add(App.GetResult(target, type));
                }
            }
            catch 
            {
                
            }
            return result;
        }
    }
    
    [ApiController]
    [Route("[controller]")]
    public class annotationsController : ControllerBase
    {

        [HttpPost]
        public IActionResult Post(dynamic req)
        {
            Response.Headers.Add("Access-Control-Allow-Headers", "accept, content-type");
            Response.Headers.Add("Access-Control-Allow-Methods", "POST");
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            return Ok("200 ok");
        }
    }
}
