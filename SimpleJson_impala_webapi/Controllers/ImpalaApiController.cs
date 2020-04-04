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
            return  App.sqllist.Keys.ToList<string>();
        }
    }
    [ApiController]
    [Route("[controller]")]
    public class queryController : ControllerBase
    {
        [HttpPost]
        public async Task<List<object>> Post(object req)
        {
            Response.Headers.Add("Access-Control-Allow-Headers", "accept, content-type");
            Response.Headers.Add("Access-Control-Allow-Methods", "POST");
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            var task = await Task.Run(() => App.GetQueryResult(req));
            return task;
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
