using System.Text.Json.Nodes;
using BigDataIndexing.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using StackExchange.Redis;

namespace BigDataIndexing.WebApi.Controllers;


[ApiController]
[Route("v1/[Controller]")]
[Authorize]
public class PlanController : ControllerBase
{
    private readonly IDatabase _redisDatabase;
    private readonly JsonValidator _jsonSchemaValidator;
    private readonly IConnectionMultiplexer _redis;


    public PlanController(RedisService redisService, JsonValidator jsonSchemaValidator)
    {
        _redisDatabase = redisService.GetDatabase();
        _jsonSchemaValidator = jsonSchemaValidator;
        _redis = redisService.GetConnectionMultiplexer();
    }
    
    [HttpPost]
    public IActionResult CreatePlan([FromBody] JsonObject planJson)
    {
        var json = planJson.ToString();
        if (!_jsonSchemaValidator.ValidateJson(json, out IList<string> errors))
        {
            return BadRequest(new { Errors = errors });
        }

        var planId = planJson["objectId"]?.ToString();
        if (_redisDatabase.KeyExists(planId))
        {
            return Conflict(new { Message = "A plan with this ID already exists." });
        }
        var etag = json.GetHashCode().ToString();
        _redisDatabase.StringSet(planId, json);
        Response.Headers["ETag"] = etag;
        return CreatedAtAction(nameof(GetPlan), new { id = planId }, planJson);
    }
    
    [HttpGet("{id}")]
     public IActionResult GetPlan(string id, [FromHeader(Name = "If-None-Match")] string ifNoneMatch = null)
     {
         var planJson = _redisDatabase.StringGet(id).ToString();
         if (string.IsNullOrEmpty(planJson))
         {
             return NotFound();
         }
 
         
         var etag = planJson.GetHashCode().ToString();

         if (!string.IsNullOrEmpty(ifNoneMatch))
         {
             if (ifNoneMatch.Equals(etag))
             {
                 return StatusCode(StatusCodes.Status304NotModified);
             }
         }
         Response.Headers["ETag"] = etag;
         return Ok(JsonConvert.DeserializeObject<Plan>(planJson));
         
     }
    

    [HttpGet]
    public IActionResult GetAllPlans()
    {
        try
        {
            var server = _redisDatabase.Multiplexer.GetServer(_redisDatabase.Multiplexer.GetEndPoints()[0]);
            var keys = server.Keys(pattern: "*");

            var plans = new List<Plan>();
            foreach (var key in keys)
            {
                string planJson = _redisDatabase.StringGet(key);
                if (!String.IsNullOrEmpty(planJson))
                {
                    try
                    {
                        var plan = JsonConvert.DeserializeObject<Plan>(planJson);
                        plans.Add(plan);
                    }
                    catch (JsonReaderException)
                    {
                        return BadRequest();
                    }
                }
            }

            if (plans.Count == 0)
            {
                return NotFound(new { Message = "No plans found." });
            }

            return Ok(plans);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
        }
    }
    
    [HttpDelete("{id}")]
    public IActionResult DeletePlan(string id)
    {
        if (!_redisDatabase.KeyExists(id))
        {
            return NotFound();
        }

        _redisDatabase.KeyDelete(id);
        return NoContent();
    }
    [HttpPatch("{id}")]
    public IActionResult PatchPlan(string id, [FromBody] JsonObject patch, [FromHeader(Name = "If-Match")] string ifMatch = null)
    {
        if (patch == null)
        {
            return BadRequest();
        }
        if (!_jsonSchemaValidator.ValidateJson(patch.ToString(), out IList<string> errors))
        {
            return BadRequest(new { Errors = errors });
        }

        var planJson = _redisDatabase.StringGet(id).ToString();
        if (string.IsNullOrEmpty(planJson))
        {
            return NotFound();
        }

        var etag = planJson.GetHashCode().ToString();

        if (!string.IsNullOrEmpty(ifMatch) && !ifMatch.Equals(etag))
        {
            return StatusCode(StatusCodes.Status412PreconditionFailed, new { Message = "ETag mismatch" });
        }
        
        _redisDatabase.StringSet(id, patch.ToString());
        Response.Headers["ETag"] = patch.ToString().GetHashCode().ToString();
        return Ok(patch);
    }
}
